// <copyright file="Font.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using ImageSharp.Drawing.Paths;
    using ImageSharp.Drawing.Shapes;
    using NOpenType;

    /// <summary>
    /// Provides access to a loaded font and provides configuration options for how it should be rendered.
    /// </summary>
    public sealed class Font
    {
        private readonly Typeface typeface;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        public Font(Stream fontStream)
        {
            this.typeface = OpenTypeReader.Read(fontStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="prototype">The prototype from which to copy all the settings.</param>
        public Font(Font prototype)
        {
            // clone out the setting in here
            this.typeface = prototype.typeface;
            this.Size = prototype.Size;
            this.EnableKerning = prototype.EnableKerning;
        }

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily => this.typeface.Name;

        /// <summary>
        /// Gets the font veriant.
        /// </summary>
        /// <value>
        /// The font veriant.
        /// </value>
        public string FontVeriant => this.typeface.FontSubFamily;

        /// <summary>
        /// Gets or sets the size. This defaults to 10.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public float Size { get; set; } = 10; // as good a size any for a defaut size.

        /// <summary>
        /// Gets or sets the height of the line in relation to the <see cref="Size"/>.
        /// </summary>
        /// <value>
        /// The height of the line.
        /// </value>
        public float LineHeight { get; set; } = 1.5f; // as good a size any for a defaut size.

        /// <summary>
        /// Gets or sets the width of the tab in number of spaces.
        /// </summary>
        /// <value>
        /// The width of the tab.
        /// </value>
        public float TabWidth { get; set; } = 4; // as good a size any for a defaut size.

        /// <summary>
        /// Gets or sets a value indicating whether to enable kerning. This defaults to true.
        /// </summary>
        /// <value>
        /// <c>true</c> if kerning is enabled otherwise <c>false</c>.
        /// </value>
        public bool EnableKerning { get; set; } = true;

        /// <summary>
        /// Measures the text with settings from the font.
        /// </summary>
        /// <param name="text">The text to mesure.</param>
        /// <returns>
        /// a <see cref="SizeF" /> of the mesured height and with of the text
        /// </returns>
        public SizeF Measure(string text)
        {
            var shapes = this.GenerateContours(text);
            RectangleF fillBounds;
            if (shapes.Length == 1)
            {
                fillBounds = shapes[0].Bounds;
            }
            else
            {
                var polysmaxX = shapes.Max(x => x.Bounds.Right);
                var polysminX = shapes.Min(x => x.Bounds.Left);
                var polysmaxY = shapes.Max(x => x.Bounds.Bottom);
                var polysminY = shapes.Min(x => x.Bounds.Top);

                fillBounds = new RectangleF(polysminX, polysminY, polysmaxX - polysminX, polysmaxY - polysminY);
            }

            return fillBounds.Size;
        }

        /// <summary>
        /// Generates the contours.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// Returns a collection of shapes making up each glyph and the realtive posion to the origin 0,0.
        /// </returns>
        public IShape[] GenerateContours(string str)
        {
            Vector2 origin = Vector2.Zero;

            // TODO add support for clipping (complex polygons should help here)
            // TODO add support for wrapping (line heights)
            var glyphs = new List<GlyphPolygon>();

            var glyphPathBuilder = new GlyphPathBuilderPolygons(this.typeface);

            float scale = this.typeface.CalculateScale(this.Size);

            bool enable_kerning = this.EnableKerning;
            ushort prevIdx = 0;
            var j = str.Length;
            float computedLineHeight = this.LineHeight * this.Size;
            bool startOfLine = true;

            var spaceIndex = (ushort)this.typeface.LookupIndex(' ');
            var spaceWidth = this.typeface.GetAdvanceWidthFromGlyphIndex(spaceIndex) * scale;

            for (int i = 0; i < j; ++i)
            {
                char c = str[i];
                bool doKerning = enable_kerning && !startOfLine;
                startOfLine = false;

                switch (c)
                {
                    case '\n':
                        origin.Y += computedLineHeight;
                        origin.X = 0;
                        startOfLine = true;
                        break;
                    case '\r':
                        // ignore '\r's
                        break;
                    case ' ':
                        origin.X += spaceWidth;
                        prevIdx = spaceIndex;
                        break;
                    case '\t':
                        origin.X += spaceWidth * this.TabWidth;
                        prevIdx = spaceIndex;
                        break;
                    default:
                        var glyIndex = (ushort)this.typeface.LookupIndex(c);
                        var glyph = glyphPathBuilder.BuildGlyph(glyIndex, this.Size, scale, origin);
                        if (glyph != null)
                        {
                            glyphs.Add(glyph);
                        }

                        // this advWidth in font design unit
                        float advWidth = this.typeface.GetAdvanceWidthFromGlyphIndex(glyIndex) * scale;
                        if (doKerning)
                        {
                            // check kerning
                            advWidth += this.typeface.GetKernDistance(prevIdx, glyIndex) * scale;
                        }

                        origin.X += advWidth;
                        prevIdx = glyIndex;
                        break;
                }
            }

            return glyphs.ToArray();
        }
    }
}
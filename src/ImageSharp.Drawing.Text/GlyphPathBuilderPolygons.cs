// <copyright file="GlyphPathBuilderPolygons.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using ImageSharp.Drawing.Paths;
    using ImageSharp.Drawing.Shapes;
    using NRasterizer;

    /// <summary>
    /// Used to convert the fint glyphs into GlyphPolygons for rendering.
    /// </summary>
    /// <seealso cref="NOpenType.GlyphPathBuilderBase" />
    internal class GlyphPathBuilderPolygons
    {
        private object locker = new object();
        private Dictionary<char, GlyphPolygon> glyphCache = new Dictionary<char, GlyphPolygon>();

        const int pointsPerInch = 72;
        const int resolution = 96;
        private readonly Typeface typeface;
        private Renderer renderer;
        private GlyphRasterizer raterizer;
        private readonly int scaleDown;
        private readonly int scaleUp;

        public float CalculateScale(float sizeInPointUnit, int resolution = 96)
        {
            return ((sizeInPointUnit * resolution) / (pointsPerInch * typeface.UnitsPerEm));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlyphPathBuilderPolygons" /> class.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <param name="fontSize">Size of the font.</param>
        public GlyphPathBuilderPolygons(Typeface typeface, float fontSize)
        {
            this.typeface = typeface;
            //this.offset = new Vector2(0, fontSize);
            //var scaleFactor = CalculateScale(fontSize);
            //this.scale = new Vector2(scaleFactor, -scaleFactor);

            this.scaleUp = (int)Math.Round(fontSize * resolution);
            this.scaleDown = EmSquare.Size * resolution;
            this.raterizer = new GlyphRasterizer();
            this.renderer = new NRasterizer.Renderer(typeface, raterizer);
        }

        /// <summary>
        /// Builds the glyph.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>
        /// Returns the polygon for the requested glyph
        /// </returns>
        public GlyphPolygon BuildGlyphs(char character)
        {
            if (this.glyphCache.ContainsKey(character))
            {
                return this.glyphCache[character];
            }

            // building a glyph isn't thread safe because it uses a class wide state while building
            lock (this.locker)
            {
                if (this.glyphCache.ContainsKey(character))
                {
                    return this.glyphCache[character];
                }

                var glyIndex = (ushort)this.typeface.LookupIndex(character);

                var glyph = typeface.Lookup(character);
                //render each glyph at 0,0 we will offset when rendering

                this.raterizer.CurrentCharacter = character;
                this.raterizer.CurrentIndex = glyIndex;
                renderer.RenderGlyph(0, 0, scaleUp, scaleDown, glyph);
                var result = this.raterizer.Glyph;
                    
                if (result == null)
                {
                    return null;
                }

                this.glyphCache.Add(character, result);
                return result;
            }
        }

    }


    internal class GlyphRasterizer : NRasterizer.IGlyphRasterizer
    {
        private GlyphPolygon _glyph;
        public GlyphPolygon Glyph => _glyph;

        public char CurrentCharacter { get; internal set; }
        public ushort CurrentIndex { get; internal set; }

        private static readonly Vector2 TwoThirds = new Vector2(2f / 3f);

        private List<Polygon> polygons = new List<Polygon>();
        private List<ILineSegment> segments = new List<ILineSegment>();
        private Vector2 lastPoint;
        private Vector2 offset;
        private Vector2 scale;
        

        /// <summary>
        /// Called when [curve3].
        /// </summary>
        /// <param name="p2x">The P2X.</param>
        /// <param name="p2y">The p2y.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void OnCurve3(short p2x, short p2y, short x, short y)
        {
        }

        /// <summary>
        /// Called when [curve4].
        /// </summary>
        /// <param name="p2x">The P2X.</param>
        /// <param name="p2y">The p2y.</param>
        /// <param name="p3x">The P3X.</param>
        /// <param name="p3y">The p3y.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void OnCurve4(short p2x, short p2y, short p3x, short p3y, short x, short y)
        {
        }

        /// <summary>
        /// Called when [line to].
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void LineTo(short x, short y)
        {
            var endPoint = this.offset + (new Vector2(x, y) * this.scale);
            this.segments.Add(new LinearLineSegment(this.lastPoint, endPoint));
            this.lastPoint = endPoint;
        }

        /// <summary>
        /// Called when [move to].
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void MoveTo(short x, short y)
        {
        }

        public void BeginRead(int countourCount)
        {
        }

        public void EndRead()
        {
            throw new NotImplementedException();
        }

        public void LineTo(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void Curve3(double p2x, double p2y, double x, double y)
        {
            throw new NotImplementedException();
        }

        public void Curve4(double p2x, double p2y, double p3x, double p3y, double x, double y)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void CloseFigure()
        {
            if (this.segments.Any())
            {
                this.polygons.Add(new Polygon(this.segments.ToArray()));
                this.segments.Clear();
            }
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }
    }
}

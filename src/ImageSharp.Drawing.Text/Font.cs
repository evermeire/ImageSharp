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
    using NRasterizer;

    /// <summary>
    /// Provides access to a loaded font and provides configuration options for how it should be rendered.
    /// </summary>
    public sealed class Font
    {
        private readonly Typeface typeface;
        private static OpenTypeReader fontReader = new OpenTypeReader();

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        public Font(Stream fontStream)
        {
            this.typeface = fontReader.Read(fontStream);
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
        }

        /// <summary>
        /// Gets the typeface.
        /// </summary>
        /// <value>
        /// The typeface.
        /// </value>
        internal Typeface Typeface => typeface;

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily => "to be fixed";// this.typeface.FontFamily;

        /// <summary>
        /// Gets the font veriant.
        /// </summary>
        /// <value>
        /// The font veriant.
        /// </value>
        public string FontVeriant => "to be fixed";//this.innerFont.FontVeriant;

        /// <summary>
        /// Gets or sets the size. This defaults to 10.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size { get; set; } = 10; // as good a size any for a defaut size.

        ///// <summary>
        ///// Gets or sets the height of the line in relation to the <see cref="Size"/>.
        ///// </summary>
        ///// <value>
        ///// The height of the line.
        ///// </value>
        //public float LineHeight { get; set; } = 1.5f; // as good a size any for a defaut size.


        /// <summary>
        /// Creates the renderer.
        /// </summary>
        /// <param name="rasterizer">The rasterizer.</param>
        /// <returns>A renderer that can draw on a <see cref="IGlyphRasterizer"/>.</returns>
        internal NRasterizer.Renderer CreateRenderer(IGlyphRasterizer rasterizer)
        {
            return new NRasterizer.Renderer(typeface, rasterizer);
        }
    }
}
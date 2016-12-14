﻿// <copyright file="SolidBrush.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Brushes
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;
    using Processors;

    /// <summary>
    /// Provides an implementaion of a solid brush for painting solid color areas.
    /// </summary>
    public class SolidBrush : SolidBrush<Color, uint>
    {
        public SolidBrush(Color color) : base(color) { }
    }

    /// <summary>
    /// Provides an implementaion of a solid brush for painting solid color areas.
    /// </summary>
    /// <typeparam name="TColor">The type of the color.</typeparam>
    /// <typeparam name="TPacked">The type of the packed.</typeparam>
    public class SolidBrush<TColor, TPacked> : IBrush<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        private readonly TColor color;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrush{TColor, TPacked}"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SolidBrush(TColor color)
        {
            this.color = color;
        }

        /// <summary>
        /// Creates the applicator for this brush.
        /// </summary>
        /// <param name="region">The region the brush will be applied to.</param>
        /// <returns></returns>
        /// <remarks>
        /// The <paramref name="region" /> when being applied to things like shapes would ussually be the
        /// bounding box of the shape not necessarily the bounds of the whole image
        /// </remarks>
        public IBrushApplicator<TColor, TPacked> CreateApplicator(RectangleF region)
        {
            return new SolidBrushApplicator(color);
        }

        private class SolidBrushApplicator : IBrushApplicator<TColor, TPacked>
        {

            private TColor color;

            public SolidBrushApplicator(TColor color)
            {           
                this.color = color;
            }

            public TColor GetColor(Vector2 point)
            {
                return color;
            }
        }
    }
}

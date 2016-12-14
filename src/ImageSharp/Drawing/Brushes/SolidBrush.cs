// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Brushes
{
    using Processing;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;
    using Processors;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.SolidBrush{ImageSharp.Color, System.UInt32}" />
    public class SolidBrush : SolidBrush<Color, uint>
    {
        public SolidBrush(Color color) : base(color) { }
    }

    /// <summary>
    /// A brush representing a Solid color fill
    /// </summary>
    /// <seealso cref="ImageSharp.Brushs.IBrush" />
    public class SolidBrush<TColor, TPacked> : IBrush<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        private readonly TColor color;
        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SolidBrush(TColor color)
        {
            this.color = color;
        }

        private class SolidBrushApplicator : IBrushApplicator<TColor, TPacked>
        {

            private TColor color;

            /// <summary>
            /// Initializes a new instance of the <see cref="SolidBrushApplicator{TColor, TPacked}"/> class.
            /// </summary>
            /// <param name="color">The color.</param>
            public SolidBrushApplicator(TColor color)
            {
                //convert to correct color space                
                this.color = color;
            }
            
            
            public TColor GetColor(Vector2 point)
            {
                return color;
            }
        }

        public IBrushApplicator<TColor, TPacked> CreateApplicator(RectangleF region)
        {
            //as Vector4 implementation
            return new SolidBrushApplicator(color);
        }
    }
}

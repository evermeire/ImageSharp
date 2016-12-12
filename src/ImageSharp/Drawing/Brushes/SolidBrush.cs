// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using Processing;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;
    
    /// <summary>
    /// A brush representing a Solid color fill
    /// </summary>
    /// <seealso cref="ImageSharp.Brushs.IBrush" />
    public class SolidBrush : IBrush
    {
        private readonly Color color;
        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SolidBrush(Color color)
        {
            this.color = color;
        }

        private class SolidBrushApplicator<TColor, TPacked> : BrushApplicatorBase<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {

            private TColor color = default(TColor);
            private bool hasOpacitySet = false;

            /// <summary>
            /// Initializes a new instance of the <see cref="SolidBrushApplicator{TColor, TPacked}"/> class.
            /// </summary>
            /// <param name="color">The color.</param>
            public SolidBrushApplicator(Vector4 color)
            {
                //convert to correct color space                
                this.color.PackFromVector4(color);
                hasOpacitySet = color.W != 1;
            }
            
            public override bool RequiresComposition
            {
                get
                {
                    return hasOpacitySet;
                }
            }

            public virtual TColor[] GetColor(int startX, int endX, int Y)
            {
                var result = new TColor[endX - startX + 1];
                for (var x = startX; x <= endX; x++)
                {
                    result[x - startX] = color;
                }
                return result;
            }

            public override TColor[,] GetColor(int startX, int startY, int endX, int endY)
            {
                var maxX = endX - startX;
                var maxY = endY - startY;
                var colors = new TColor[maxX+1, maxY+1];
                for (var x = 0; x <= maxX; x++)
                {
                    for (var y = 0; y <= maxY; y++)
                    {
                        colors[x, y] = color;
                    }
                }

                return colors;
            }

            public override TColor GetColor(int x, int y)
            {
                return color;
            }
        }

        public IBrushApplicator<TColor, TPacked> CreateApplicator<TColor, TPacked>(RectangleF region)
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
        {
            //as Vector4 implementation
            return new SolidBrushApplicator<TColor, TPacked>(color.ToVector4());
        }
    }
}

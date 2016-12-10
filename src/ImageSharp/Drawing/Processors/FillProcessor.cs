// <copyright file="DrawProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    using Drawing;
    using ImageSharp.Processors;

    /// <summary>
    /// Combines two images together by blending the pixels.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class FillProcessor<TColor, TPacked> : ImageFilter<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// The epsilon for comparing floating point numbers.
        /// </summary>
        private const float Epsilon = 0.001f;

        /// <summary>
        /// The brush to apply.
        /// </summary>
        private readonly IBrush brush;
        private readonly FillLayer layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrushProcessor{T,TP}"/> class.
        /// </summary>
        /// <param name="brush">
        /// The brush to apply to currently processing image.
        /// </param>
        public FillProcessor(IBrush brush, FillLayer layer = FillLayer.OverSource)
        {
            this.layer = layer;
            this.brush = brush;            
        }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;

            // Align start/end positions.
            int minX = Math.Max(0, startX);
            int maxX = Math.Min(source.Width, endX);
            int minY = Math.Max(0, startY);
            int maxY = Math.Min(source.Height, endY);

            // Reset offset if necessary.
            if (minX > 0)
            {
                startX = 0;
            }

            if (minY > 0)
            {
                startY = 0;
            }

            //calculate 

            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            {
                using (var applicator = brush.CreateApplicator(sourceRectangle))
                {
                    Parallel.For(
                        minY,
                        maxY,
                        this.ParallelOptions,
                        y =>
                        {
                            int offsetY = y - startY;
                            for (int x = minX; x < maxX; x++)
                            {
                                int offsetX = x - startX;

                                var color = applicator.GetColor(offsetX, offsetY).ToVector4();
                                Vector4 backgroundColor = sourcePixels[offsetX, offsetY].ToVector4();

                                if(this.layer == FillLayer.UnderSource)
                                {
                                    //we want the brush color under the source image, flip the color order
                                    var tmp = color;
                                    color = backgroundColor;
                                    backgroundColor = tmp;
                                }

                                float a = color.W;

                                if (Math.Abs(a) < Epsilon)
                                {
                                    color = backgroundColor;
                                }
                                else if (a < 1 && a > 0)
                                {
                                    color.W = 1;
                                    color = Vector4.Lerp(backgroundColor, color, a);
                                }

                                TColor packed = default(TColor);
                                packed.PackFromVector4(color);
                                sourcePixels[offsetX, offsetY] = packed;
                            }                            
                        });
                }
            }
        }        
    }
}
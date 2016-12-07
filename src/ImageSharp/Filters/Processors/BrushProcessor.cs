// <copyright file="DrawProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    using Brushes;
    using Shapes;

    /// <summary>
    /// Combines two images together by blending the pixels.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class BrushProcessor<TColor, TPacked> : ImageFilter<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// The brush to apply.
        /// </summary>
        private readonly IBrush brush;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrushProcessor{T,TP}"/> class.
        /// </summary>
        /// <param name="brush">
        /// The brush to apply to currently processing image.
        /// </param>
        public BrushProcessor(IBrush brush)
        {
            this.brush = brush;

            // Don't Parallelize processing
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
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
                            var packed = brush.GetColor(sourcePixels, offsetX, offsetY);
                            sourcePixels[offsetX, offsetY] = packed;
                        }
                    });
            }
        }        
    }
}
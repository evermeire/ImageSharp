// <copyright file="FillProcessor.cs" company="James Jackson-South">
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
    /// Using the bursh as a source of pixels colors blends the brush color with source.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class FillProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        private const float Epsilon = 0.001f;

        private readonly IBrush<TColor, TPacked> brush;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FillProcessor{TColor, TPacked}"/> class.
        /// </summary>
        /// <param name="brush">The brush to source pixel colors from.</param>      
        public FillProcessor(IBrush<TColor, TPacked> brush)
        {
            this.brush = brush;
        }

        /// <inheritdoc/>
        protected override void OnApply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle)
        {
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;
            int startY = sourceRectangle.Y;
            int endY = sourceRectangle.Bottom;

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
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
            // create a pixcel applicator (this will be important when there is a gradient or image brush)
            // should probably be disposable if we want an image brush so we can dispose of the underlying 
            // PixelAccessor<TColor, TPacked> that it would need, wonder if its woth doing now to prevent 
            // api breaks later.
            var applicator = brush.CreateApplicator(sourceRectangle);

            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            {
                Parallel.For(
                    minY,
                    maxY,
                    this.ParallelOptions,
                    y =>
                    {
                        int offsetY = y - startY;
                        
                        var colors = applicator.GetColor(minX, maxX-1, offsetY);

                        for (int x = minX; x < maxX; x++)
                        {
                            int offsetX = x - startX;
                            int offsetColorX = x - minX;
                            
                            Vector4 backgroundVector = sourcePixels[offsetX, offsetY].ToVector4();
                            Vector4 sourceVector = colors[offsetColorX].ToVector4();
                            
                            var finalColor = Vector4BlendTransforms.PremultipliedLerp(backgroundVector, sourceVector, 1);
                            finalColor.W = backgroundVector.W;

                            TColor packed = default(TColor);
                            packed.PackFromVector4(finalColor);
                            sourcePixels[offsetX, offsetY] = packed;
                        }
                    });
            }
        }
    }
}
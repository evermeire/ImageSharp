﻿// <copyright file="ResizeProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods that allow the resizing of images using various algorithms.
    /// </summary>
    /// <remarks>
    /// This version and the <see cref="CompandingResizeProcessor{TColor,TPacked}"/> have been separated out to improve performance.
    /// </remarks>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class ResizeProcessor<TColor, TPacked> : ResamplingWeightedProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeProcessor{TColor, TPacked}"/> class.
        /// </summary>
        /// <param name="sampler">The sampler to perform the resize operation.</param>
        /// <param name="width">The target width.</param>
        /// <param name="height">The target height.</param>
        public ResizeProcessor(IResampler sampler, int width, int height)
            : base(sampler, width, height, new Rectangle(0, 0, width, height))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeProcessor{TColor, TPacked}"/> class.
        /// </summary>
        /// <param name="sampler">The sampler to perform the resize operation.</param>
        /// <param name="width">The target width.</param>
        /// <param name="height">The target height.</param>
        /// <param name="resizeRectangle">
        /// The <see cref="Rectangle"/> structure that specifies the portion of the target image object to draw to.
        /// </param>
        public ResizeProcessor(IResampler sampler, int width, int height, Rectangle resizeRectangle)
            : base(sampler, width, height, resizeRectangle)
        {
        }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            // Jump out, we'll deal with that later.
            if (source.Width == this.Width && source.Height == this.Height && sourceRectangle == this.ResizeRectangle)
            {
                return;
            }

            // Reset the values as the rectangle can be altered by ResizeRectangle.
            startY = this.ResizeRectangle.Y;
            endY = this.ResizeRectangle.Bottom;

            int width = this.Width;
            int height = this.Height;
            int startX = this.ResizeRectangle.X;
            int endX = this.ResizeRectangle.Right;

            int minX = Math.Max(0, startX);
            int maxX = Math.Min(width, endX);
            int minY = Math.Max(0, startY);
            int maxY = Math.Min(height, endY);

            TColor[] target = new TColor[width * height];

            if (this.Sampler is NearestNeighborResampler)
            {
                // Scaling factors
                float widthFactor = sourceRectangle.Width / (float)this.ResizeRectangle.Width;
                float heightFactor = sourceRectangle.Height / (float)this.ResizeRectangle.Height;

                using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
                using (PixelAccessor<TColor, TPacked> targetPixels = target.Lock<TColor, TPacked>(width, height))
                {
                    Parallel.For(
                        minY,
                        maxY,
                        this.ParallelOptions,
                        y =>
                        {
                            // Y coordinates of source points
                            int originY = (int)((y - startY) * heightFactor);

                            for (int x = minX; x < maxX; x++)
                            {
                                // X coordinates of source points
                                targetPixels[x, y] = sourcePixels[(int)((x - startX) * widthFactor), originY];
                            }
                        });
                }

                // Break out now.
                source.SetPixels(width, height, target);
                return;
            }

            // Interpolate the image using the calculated weights.
            // A 2-pass 1D algorithm appears to be faster than splitting a 1-pass 2D algorithm
            // First process the columns. Since we are not using multiple threads startY and endY
            // are the upper and lower bounds of the source rectangle.
            TColor[] firstPass = new TColor[width * source.Height];
            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            using (PixelAccessor<TColor, TPacked> firstPassPixels = firstPass.Lock<TColor, TPacked>(width, source.Height))
            using (PixelAccessor<TColor, TPacked> targetPixels = target.Lock<TColor, TPacked>(width, height))
            {
                Parallel.For(
                    0,
                    sourceRectangle.Height,
                    this.ParallelOptions,
                    y =>
                    {
                        for (int x = minX; x < maxX; x++)
                        {
                            // Ensure offsets are normalised for cropping and padding.
                            Weight[] horizontalValues = this.HorizontalWeights[x - startX].Values;

                            // Destination color components
                            Vector4 destination = Vector4.Zero;

                            for (int i = 0; i < horizontalValues.Length; i++)
                            {
                                Weight xw = horizontalValues[i];
                                destination += sourcePixels[xw.Index, y].ToVector4() * xw.Value;
                            }

                            TColor d = default(TColor);
                            d.PackFromVector4(destination);
                            firstPassPixels[x, y] = d;
                        }
                    });

                // Now process the rows.
                Parallel.For(
                    minY,
                    maxY,
                    this.ParallelOptions,
                    y =>
                    {
                        // Ensure offsets are normalised for cropping and padding.
                        Weight[] verticalValues = this.VerticalWeights[y - startY].Values;

                        for (int x = 0; x < width; x++)
                        {
                            // Destination color components
                            Vector4 destination = Vector4.Zero;

                            for (int i = 0; i < verticalValues.Length; i++)
                            {
                                Weight yw = verticalValues[i];
                                destination += firstPassPixels[x, yw.Index].ToVector4() * yw.Value;
                            }

                            TColor d = default(TColor);
                            d.PackFromVector4(destination);
                            targetPixels[x, y] = d;
                        }
                    });
            }

            source.SetPixels(width, height, target);
        }
    }
}
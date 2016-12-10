﻿// <copyright file="ConvolutionProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a sampler that uses a 2 dimensional matrix to perform convolution against an image.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class ConvolutionProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvolutionProcessor{TColor,TPacked}"/> class.
        /// </summary>
        /// <param name="kernelXY">The 2d gradient operator.</param>
        public ConvolutionProcessor(float[][] kernelXY)
        {
            this.KernelXY = kernelXY;
        }

        /// <summary>
        /// Gets the 2d gradient operator.
        /// </summary>
        public virtual float[][] KernelXY { get; }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            float[][] kernelX = this.KernelXY;
            int kernelLength = kernelX.GetLength(0);
            int radius = kernelLength >> 1;

            int sourceY = sourceRectangle.Y;
            int sourceBottom = sourceRectangle.Bottom;
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;
            int maxY = sourceBottom - 1;
            int maxX = endX - 1;

            TColor[] target = new TColor[source.Width * source.Height];
            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            using (PixelAccessor<TColor, TPacked> targetPixels = target.Lock<TColor, TPacked>(source.Width, source.Height))
            {
                Parallel.For(
                startY,
                endY,
                this.ParallelOptions,
                y =>
                {
                    if (y >= sourceY && y < sourceBottom)
                    {
                        for (int x = startX; x < endX; x++)
                        {
                            float rX = 0;
                            float gX = 0;
                            float bX = 0;

                            // Apply each matrix multiplier to the color components for each pixel.
                            for (int fy = 0; fy < kernelLength; fy++)
                            {
                                int fyr = fy - radius;
                                int offsetY = y + fyr;

                                offsetY = offsetY.Clamp(0, maxY);

                                for (int fx = 0; fx < kernelLength; fx++)
                                {
                                    int fxr = fx - radius;
                                    int offsetX = x + fxr;

                                    offsetX = offsetX.Clamp(0, maxX);

                                    Vector4 currentColor = sourcePixels[offsetX, offsetY].ToVector4();
                                    float r = currentColor.X;
                                    float g = currentColor.Y;
                                    float b = currentColor.Z;

                                    rX += kernelX[fy][fx] * r;
                                    gX += kernelX[fy][fx] * g;
                                    bX += kernelX[fy][fx] * b;
                                }
                            }

                            float red = rX;
                            float green = gX;
                            float blue = bX;

                            TColor packed = default(TColor);
                            packed.PackFromVector4(new Vector4(red, green, blue, sourcePixels[x, y].ToVector4().W));
                            targetPixels[x, y] = packed;
                        }
                    }
                });
            }

            source.SetPixels(source.Width, source.Height, target);
        }
    }
}
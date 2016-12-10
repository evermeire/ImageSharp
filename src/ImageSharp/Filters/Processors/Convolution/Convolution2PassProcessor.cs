﻿// <copyright file="Convolution2PassProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a sampler that uses two one-dimensional matrices to perform two-pass convolution against an image.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class Convolution2PassProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Convolution2PassProcessor{TColor,TPacked}"/> class.
        /// </summary>
        /// <param name="kernelX">The horizontal gradient operator.</param>
        /// <param name="kernelY">The vertical gradient operator.</param>
        public Convolution2PassProcessor(float[][] kernelX, float[][] kernelY)
        {
            this.KernelX = kernelX;
            this.KernelY = kernelY;
        }

        /// <summary>
        /// Gets the horizontal gradient operator.
        /// </summary>
        public float[][] KernelX { get; }

        /// <summary>
        /// Gets the vertical gradient operator.
        /// </summary>
        public float[][] KernelY { get; }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            float[][] kernelX = this.KernelX;
            float[][] kernelY = this.KernelY;
            int width = source.Width;
            int height = source.Height;

            TColor[] target = new TColor[width * height];
            TColor[] firstPass = new TColor[width * height];

            this.ApplyConvolution(width, height, firstPass, source.Pixels, sourceRectangle, startY, endY, kernelX);
            this.ApplyConvolution(width, height, target, firstPass, sourceRectangle, startY, endY, kernelY);

            source.SetPixels(width, height, target);
        }

        /// <summary>
        /// Applies the process to the specified portion of the specified <see cref="ImageBase{TColor, TPacked}"/> at the specified location
        /// and with the specified size.
        /// </summary>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <param name="target">The target pixels to apply the process to.</param>
        /// <param name="source">The source pixels. Cannot be null.</param>
        /// <param name="sourceRectangle">
        /// The <see cref="Rectangle"/> structure that specifies the portion of the image object to draw.
        /// </param>
        /// <param name="startY">The index of the row within the source image to start processing.</param>
        /// <param name="endY">The index of the row within the source image to end processing.</param>
        /// <param name="kernel">The kernel operator.</param>
        private void ApplyConvolution(int width, int height, TColor[] target, TColor[] source, Rectangle sourceRectangle, int startY, int endY, float[][] kernel)
        {
            int kernelHeight = kernel.Length;
            int kernelWidth = kernel[0].Length;
            int radiusY = kernelHeight >> 1;
            int radiusX = kernelWidth >> 1;

            int sourceBottom = sourceRectangle.Bottom;
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;
            int maxY = sourceBottom - 1;
            int maxX = endX - 1;

            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock<TColor, TPacked>(width, height))
            using (PixelAccessor<TColor, TPacked> targetPixels = target.Lock<TColor, TPacked>(width, height))
            {
                Parallel.For(
                startY,
                endY,
                this.ParallelOptions,
                y =>
                {
                    for (int x = startX; x < endX; x++)
                    {
                        Vector4 destination = default(Vector4);

                        // Apply each matrix multiplier to the color components for each pixel.
                        for (int fy = 0; fy < kernelHeight; fy++)
                        {
                            int fyr = fy - radiusY;
                            int offsetY = y + fyr;

                            offsetY = offsetY.Clamp(0, maxY);

                            for (int fx = 0; fx < kernelWidth; fx++)
                            {
                                int fxr = fx - radiusX;
                                int offsetX = x + fxr;

                                offsetX = offsetX.Clamp(0, maxX);

                                Vector4 currentColor = sourcePixels[offsetX, offsetY].ToVector4();
                                destination += kernel[fy][fx] * currentColor;
                            }
                        }

                        TColor packed = default(TColor);
                        packed.PackFromVector4(destination);
                        targetPixels[x, y] = packed;
                    }
                });
            }
        }
    }
}
﻿// <copyright file="InvertProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="IImageFilteringProcessor{TColor,TPacked}"/> to invert the colors of an <see cref="Image{TColor, TPacked}"/>.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class InvertProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct, IEquatable<TPacked>
    {
        /// <inheritdoc/>
        protected override void OnApply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle)
        {
            int startY = sourceRectangle.Y;
            int endY = sourceRectangle.Bottom;
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;
            Vector3 inverseVector = Vector3.One;

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
                                Vector4 color = sourcePixels[offsetX, offsetY].ToVector4();
                                Vector3 vector = inverseVector - new Vector3(color.X, color.Y, color.Z);

                                TColor packed = default(TColor);
                                packed.PackFromVector4(new Vector4(vector, color.W));
                                sourcePixels[offsetX, offsetY] = packed;
                            }
                        });
            }
        }
    }
}
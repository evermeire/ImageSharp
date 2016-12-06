// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushs
{

    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// A brush representing a Solid color fill
    /// </summary>
    /// <seealso cref="ImageSharp.Brushs.IBrush" />
    public class SolidBrush: IBrush
    {
        public ParallelOptions ParallelOptions { get; set; } = Bootstrapper.Instance.ParallelOptions;

        private readonly Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SolidBrush(Color color)
        {
            this.color = color;
        }

        /// <summary>
        /// Applies the brush to the source.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sourceRectangle">The source rectangle.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endY">The end y.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Apply<TColor, TPacked>(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
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

            

            Vector4 backgroundColor = color.ToVector4();
            TColor packed = default(TColor);
            packed.PackFromVector4(backgroundColor);

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
                            sourcePixels[offsetX, offsetY] = packed;
                        }
                    });
            }
        }
    }
}

// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushes
{
    using Shapes;
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
        public void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source, IMask mask)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            var sourceRectangle = mask.Bounds;
            // if/when we support antiailiasing we might/will need to expand the bound to account

            int startY = sourceRectangle.Y;
            int endY = sourceRectangle.Bottom;
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
                            var dist = mask.Distance(offsetX, offsetY);
                            if (dist == 0)
                            {
                                //inside mask full color
                                sourcePixels[offsetX, offsetY] = packed;
                            }
                            else if (dist < 1)
                            {
                                Vector4 color = sourcePixels[offsetX, offsetY].ToVector4();

                                var alpha = (1 - dist);

                                color = Vector4.Lerp(color, backgroundColor, alpha > 0 ? alpha : backgroundColor.W);
                                
                                TColor newPacked = default(TColor);
                                newPacked.PackFromVector4(color);
                                sourcePixels[offsetX, offsetY] = newPacked;
                            }
                            // TODO add distance ability to effect opactiy based on distance
                        }
                    });
            }
        }
    }
}

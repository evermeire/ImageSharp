﻿// <copyright file="BlendProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// Combines two images together by blending the pixels.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class BlendProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlendProcessor{TColor,TPacked}"/> class.
        /// </summary>
        /// <param name="image">The image to blend with the currently processing image.</param>
        /// <param name="size">The size to draw the blended image.</param>
        /// <param name="location">The location to draw the blended image.</param>
        /// <param name="alpha">The opacity of the image to blend. Between 0 and 100.</param>
        public BlendProcessor(Image<TColor, TPacked> image, Size size, Point location, int alpha = 100)
        {
            Guard.MustBeBetweenOrEqualTo(alpha, 0, 100, nameof(alpha));
            this.Image = image;
            this.Size = size;
            this.Alpha = alpha;
            this.Location = location;
        }

        /// <summary>
        /// Gets the image to blend.
        /// </summary>
        public Image<TColor, TPacked> Image { get; private set; }

        /// <summary>
        /// Gets the alpha percentage value.
        /// </summary>
        public int Alpha { get; }

        /// <summary>
        /// Gets the size to draw the blended image.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// Gets the location to draw the blended image.
        /// </summary>
        public Point Location { get; }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            if (this.Image.Bounds.Size != this.Size)
            {
                this.Image = this.Image.Resize(this.Size.Width, this.Size.Height);
            }

            // Align start/end positions.
            Rectangle bounds = this.Image.Bounds;
            int minX = Math.Max(this.Location.X, sourceRectangle.X);
            int maxX = Math.Min(this.Location.X + bounds.Width, sourceRectangle.Width);
            int minY = Math.Max(this.Location.Y, startY);
            int maxY = Math.Min(this.Location.Y + bounds.Height, endY);

            float alpha = this.Alpha / 100F;

            using (PixelAccessor<TColor, TPacked> toBlendPixels = this.Image.Lock())
            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            {
                Parallel.For(
                    minY,
                    maxY,
                    this.ParallelOptions,
                    y =>
                        {
                            for (int x = minX; x < maxX; x++)
                            {
                                Vector4 backgroundVector = sourcePixels[x, y].ToVector4();
                                Vector4 sourceVector = toBlendPixels[x - minX, y - minY].ToVector4();

                                // Lerping colors is dependent on the alpha of the blended color
                                backgroundVector = Vector4BlendTransforms.PremultipliedLerp(backgroundVector, sourceVector, alpha);

                                TColor packed = default(TColor);
                                packed.PackFromVector4(backgroundVector);
                                sourcePixels[x, y] = packed;
                            }
                        });
            }
        }
    }
}
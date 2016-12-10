﻿// <copyright file="Grayscale.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    using Processors;

    /// <summary>
    /// Extension methods for the <see cref="Image{TColor, TPacked}"/> type.
    /// </summary>
    public static partial class ImageExtensions
    {
        /// <summary>
        /// Applies Grayscale toning to the image.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="source">The image this method extends.</param>
        /// <param name="mode">The formula to apply to perform the operation.</param>
        /// <returns>The <see cref="Image{TColor, TPacked}"/>.</returns>
        public static Image<TColor, TPacked> Grayscale<TColor, TPacked>(this Image<TColor, TPacked> source, GrayscaleMode mode = GrayscaleMode.Bt709)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            return Grayscale(source, source.Bounds, mode);
        }

        /// <summary>
        /// Applies Grayscale toning to the image.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="source">The image this method extends.</param>
        /// <param name="rectangle">
        /// The <see cref="Rectangle"/> structure that specifies the portion of the image object to alter.
        /// </param>
        /// <param name="mode">The formula to apply to perform the operation.</param>
        /// <returns>The <see cref="Image{TColor, TPacked}"/>.</returns>
        public static Image<TColor, TPacked> Grayscale<TColor, TPacked>(this Image<TColor, TPacked> source, Rectangle rectangle, GrayscaleMode mode = GrayscaleMode.Bt709)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            IImageFilteringProcessor<TColor, TPacked> processor = mode == GrayscaleMode.Bt709
                ? (IImageFilteringProcessor<TColor, TPacked>)new GrayscaleBt709Processor<TColor, TPacked>()
                : new GrayscaleBt601Processor<TColor, TPacked>();

            return source.Process(rectangle, processor);
        }
    }
}

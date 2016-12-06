// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushs
{

    using System;

    /// <summary>
    /// interface preresenting a brush
    /// </summary>
    public interface IBrush
    {
        /// <summary>
        /// Applies the brush to the source.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sourceRectangle">The source rectangle.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endY">The end y.</param>
        void Apply<TColor, TPacked>(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct;
    }
}

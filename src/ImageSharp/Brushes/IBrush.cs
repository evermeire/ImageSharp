// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushes
{
    using Shapes;
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
        void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source, IMask mask)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct;
    }
}

// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processing
{
    using System;

    /// <summary>
    /// interface preresenting a brush
    /// </summary>
    public interface IPenApplicator<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        RectangleF RequiredRegion { get; }
        /// <summary>
        /// Gets the color for a single pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        ColoredPointInfo<TColor, TPacked> GetColor(PointInfo info);
    }
}

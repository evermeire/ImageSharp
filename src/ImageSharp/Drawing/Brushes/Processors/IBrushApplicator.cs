// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Numerics;

    /// <summary>
    /// interface preresenting a brush
    /// </summary>
    public interface IBrushApplicator<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
       
        /// <summary>
        /// Gets the color for a single pixel.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        TColor GetColor(Vector2 point);
    }
}

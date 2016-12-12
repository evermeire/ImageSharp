// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;

    /// <summary>
    /// interface preresenting a brush
    /// </summary>
    public interface IPen<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        IBrush<TColor, TPacked> Brush { get; }
        float Width { get; }
    }
}

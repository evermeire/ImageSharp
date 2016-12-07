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
        TColor GetColor<TColor, TPacked>(PixelAccessor<TColor, TPacked> source, int x, int y)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct;
    }
}

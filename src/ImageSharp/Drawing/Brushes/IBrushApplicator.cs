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
    public interface IBrushApplicator : IDisposable
    {
        Color GetColor(int x, int y);
    }
}

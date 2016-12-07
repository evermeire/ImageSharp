// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Shapes
{

    using System;

    /// <summary>
    /// interface preresenting a brush
    /// </summary>
    public interface IMask
    {
        Rectangle Bounds { get; }

        float Distance(int x, int y);        
    }
}

﻿// <copyright file="IBrush.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;
    using Processors;

    /// <summary>
    /// Brush represents a logical configuration of a brush whcih can be used to source pixel colors
    /// </summary>
    /// <typeparam name="TColor">The type of the color.</typeparam>
    /// <typeparam name="TPacked">The type of the packed.</typeparam>
    /// <remarks>
    /// A brush is a simple class that will return an <see cref="IBrushApplicator{TColor, TPacked}" /> that will perform the
    /// logic for converting a pixel location to a <typeparamref name="TColor"/>.
    /// </remarks>
    public interface IBrush<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        /// <summary>
        /// Creates the applicator for this brush.
        /// </summary>
        /// <param name="region">The region the brush will be applied to.</param>
        /// <returns>The brush applicator for this brush</returns>
        /// <remarks>
        /// The <paramref name="region" /> when being applied to things like shapes would usually be the
        /// bounding box of the shape not necessarily the bounds of the whole image
        /// </remarks>
        IBrushApplicator<TColor, TPacked> CreateApplicator(RectangleF region);
    }
}

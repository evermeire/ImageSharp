// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;
    using Processors;

    /// <summary>
    /// interface representing a brush
    /// </summary>
    /// <remarks>
    /// A brush is a simple interface that will return an <see cref="IBrushApplicator"/> that will perform the
    /// logic for converting a pixel location to a <see cref="Color"/>.
    /// </remarks>
    public interface IBrush<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        /// <summary>
        /// Creates the applicator for this bursh.
        /// </summary>
        /// <param name="region">The region the brush will be applied to.</param>
        /// <returns></returns>
        /// <remarks>
        /// The <paramref name="region"/> when being applied to things like shapes would ussually be the 
        /// bounding box of the shape not necorserrally the shape of the whole image 
        /// </remarks>
        IBrushApplicator<TColor, TPacked> CreateApplicator(RectangleF region);
    }
}

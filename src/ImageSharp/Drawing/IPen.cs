// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;
    using Processing;

    /// <summary>
    /// interface preresenting a brush
    /// </summary>
    public interface IPen<TColor, TPacked>
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
        IPenApplicator<TColor, TPacked> CreateApplicator(RectangleF region);
    }
}

﻿// <copyright file="IPen.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Pens
{
    using System;
    using Processors;

    /// <summary>
    /// interface preresenting a Pen
    /// </summary>
    public interface IPen<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        /// <summary>
        /// Creates the applicator for applying this pen to an Image
        /// </summary>
        /// <param name="region">The region the pen will be applied to.</param>
        /// <returns></returns>
        /// <remarks>
        /// The <paramref name="region"/> when being applied to things like shapes would ussually be the 
        /// bounding box of the shape not necorserrally the shape of the whole image 
        /// </remarks>
        IPenApplicator<TColor, TPacked> CreateApplicator(RectangleF region);
    }
}

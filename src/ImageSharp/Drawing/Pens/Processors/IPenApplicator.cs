﻿// <copyright file="IPenApplicator.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Pens.Processors
{
    using System;
    using Paths;


    /// <summary>
    /// primitive that converts a <see cref="PointInfo"/> into a color and a distance away from the drawable part of the path.
    /// </summary>    
    /// <typeparam name="TColor">The type of the color.</typeparam>
    /// <typeparam name="TPacked">The type of the packed.</typeparam>
    public interface IPenApplicator<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// Gets the required region.
        /// </summary>
        /// <value>
        /// The required region.
        /// </value>
        RectangleF RequiredRegion { get; }

        /// <summary>
        /// Gets a <see cref="ColoredPointInfo{TColor, TPacked}"/> from a point represented by a <see cref="PointInfo"/>.
        /// </summary>
        /// <param name="info">The information to extract color details about.</param>
        /// <returns></returns>
        ColoredPointInfo<TColor, TPacked> GetColor(PointInfo info);
    }
}

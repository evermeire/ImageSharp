﻿// <copyright file="IShape.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>


namespace ImageSharp.Drawing.Shapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using Paths;

    /// <summary>
    /// Represents a closed set of paths making up a single shape.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{ImageSharp.Drawing.Paths.IPath}" />
    public interface IShape : IEnumerable<IPath>
    {
        /// <summary>
        /// Gets the bounding box of this shape.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        RectangleF Bounds { get; }

        /// <summary>
        /// the distance of the point from the outline of the shape, if the value is negative it is inside the polygon bounds
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        float Distance(int x, int y);
    }
}

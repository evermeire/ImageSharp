﻿// <copyright file="Path.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Paths
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// A aggragate of <see cref="ILineSegment"/>s making a single logical path 
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.Paths.IPath" />
    public class Path : IPath
    {
        private readonly InternalPath innerPath;

        internal Path(params ILineSegment[] segment)
        {
            innerPath = new InternalPath(segment, false);
        }


        /// <summary>
        /// Gets the bounds enclosing the path
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public RectangleF Bounds => innerPath.Bounds;

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is closed; otherwise, <c>false</c>.
        /// </value>
        public bool IsClosed => false;

        /// <summary>
        /// Gets the length of the path
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public float Length => innerPath.Length;

        /// <summary>
        /// Returns the current <see cref="ILineSegment" /> a simple linear path.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector2> AsSimpleLinearPath()
        {
            return this.innerPath.Points;
        }

        /// <summary>
        /// Calcualtes the distance along and away from the path for a specified point.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public PointInfo Distance(int x, int y)
        {
            return innerPath.DistanceFromPath(new Vector2(x, y));
        }
    }
}
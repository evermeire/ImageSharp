// <copyright file="Polygon.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Shapes
{
    using Paths;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using System.Collections;

    /// <summary>
    /// A shape made up of a single path made up of one of more <see cref="ILineSegment"/>s
    /// </summary>
    public sealed class Polygon : IShape, IPath
    {
        private float[] constant;
        private IEnumerable<IPath> pathCollection;
        private float[] multiple;
        private readonly InternalPath innerPath;

        /// <summary>
        /// Gets the bounding box of this shape.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public RectangleF Bounds => innerPath.Bounds;

        /// <summary>
        /// Gets the length of the path
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public float Length => innerPath.Length;

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is closed; otherwise, <c>false</c>.
        /// </value>
        public bool IsClosed => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="segments">The segments.</param>
        public Polygon(params ILineSegment[] segments)
            : this((IEnumerable<ILineSegment>)segments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="segments">The segments.</param>
        public Polygon(IEnumerable<ILineSegment> segments)
        {
            innerPath = new InternalPath(segments, true);
            pathCollection = new[] { this };
        }

        /// <summary>
        /// the distance of the point from the outline of the shape, if the value is negative it is inside the polygon bounds
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public float Distance(int x, int y)
        {
            var point = new Vector2(x, y);

            bool isInside = innerPath.PointInPolygon(point);

            var dist = innerPath.DistanceFromPath(point);

            if (isInside)
            {
                return -dist.DistanceFromPath;
            }
            else
            {
                return dist.DistanceFromPath;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IPath> GetEnumerator()
        {
            return pathCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pathCollection.GetEnumerator();
        }
        
        PointInfo IPath.Distance(int x, int y)
        {
            return innerPath.DistanceFromPath(new Vector2(x,y));
        }

        /// <summary>
        /// Returns the current shape as a simple linear path.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector2> AsSimpleLinearPath()
        {
            return innerPath.points;
        }
    }
}

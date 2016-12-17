// <copyright file="GlyphPolygon.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using ImageSharp.Drawing.Paths;
    using ImageSharp.Drawing.Shapes;

    /// <summary>
    /// a specailist polygon that understand typeface glyphs and knows that they should never overlap.
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.Shapes.IShape" />
    internal class GlyphPolygon : IShape
    {
        private readonly Polygon[] polygons;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlyphPolygon"/> class.
        /// </summary>
        /// <param name="polygons">The polygons.</param>
        public GlyphPolygon(Polygon[] polygons)
        {
            this.polygons = polygons;

            var minX = this.polygons.Min(x => x.Bounds.Left);
            var maxX = this.polygons.Max(x => x.Bounds.Right);
            var minY = this.polygons.Min(x => x.Bounds.Top);
            var maxY = this.polygons.Max(x => x.Bounds.Bottom);

            this.Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Gets the bounding box of this shape.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public RectangleF Bounds { get; }

        /// <summary>
        /// the distance of the point from the outline of the shape, if the value is negative it is inside the polygon bounds
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        /// Returns the distance from the shape to the point
        /// </returns>
        public float Distance(Vector2 point)
        {
            float dist = float.MaxValue;
            bool inside = false;
            foreach (var shape in this.polygons)
            {
                var d = shape.Distance(point);

                if (d <= 0)
                {
                    // we are inside a poly
                    d = -d;  // flip the sign
                    inside ^= true; // flip the inside flag
                }

                if (d < dist)
                {
                    dist = d;
                }
            }

            if (inside)
            {
                return -dist;
            }

            return dist;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IPath> GetEnumerator()
        {
            return ((IEnumerable<IPath>)this.polygons).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.polygons.GetEnumerator();
        }
    }
}

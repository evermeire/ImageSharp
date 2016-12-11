// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Polygons
{
    using Paths;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// a <see cref="SimplePolygon"/> represents a contiguos bound region 
    /// that will act as a hole or a solid
    /// </summary>
    internal class SimplePolygon : IShape
    {
        private float[] constant;
        private float[] multiple;
        private readonly InternalPath innerPath;

        public RectangleF Bounds => innerPath.Bounds;

        public SimplePolygon(params ILineSegment[] segments)
            : this((IEnumerable<ILineSegment>)segments)
        {
        }

        public SimplePolygon(IEnumerable<Vector2> points)
            : this(new LinearLineSegment(points))
        {
        }

        public SimplePolygon(IEnumerable<ILineSegment> segments)
        {
            innerPath = new InternalPath(segments, true);
        }

        bool PointInPolygon(Vector2 point)
        {
            if (!innerPath.Bounds.Contains(point.X, point.Y))
            {
                return false;
            }

            // create a point we know is outside the polygon
            var origon = new Vector2(innerPath.Bounds.Left - 1, point.Y);

            var points = this.innerPath.CrossingPoints(origon, point);

            return points.Count() % 2 == 1;
        }

        public float Distance(Vector2 point)
        {
            bool isInside = PointInPolygon(point);

            var dist = innerPath.DistanceFromPath(point);

            if (isInside)
            {
                return -dist;
            }
            else
            {
                return dist;
            }
        }

        float IShape.Distance(int x, int y)
        {
            return this.Distance(new Vector2(x, y));
        }
    }
}

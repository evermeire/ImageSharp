// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Paths
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    internal class InternalPath
    {
        public Vector2[] Points;

        internal InternalPath(IEnumerable<ILineSegment> segment)
        {
            Guard.NotNull(segment, nameof(segment));

            Points = CalculatePoints(segment);

            var minX = Points.Min(x=>x.X);
            var maxX = Points.Max(x => x.X);
            var minY = Points.Min(x => x.Y);
            var maxY = Points.Max(x => x.Y);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        private static Vector2[] CalculatePoints(IEnumerable<ILineSegment> segments)
        {
            return CalculatePointsInner(segments).ToArray();
        }

        private static IEnumerable<Vector2> CalculatePointsInner(IEnumerable<ILineSegment> segments)
        {
            //used to deduplicate
            HashSet<Vector2> pointHash = new HashSet<Vector2>();

            foreach (var segment in segments)
            {
                var points = segment.Simplify();
                foreach (var p in points)
                {
                    if (!pointHash.Contains(p))
                    {
                        pointHash.Add(p);
                        yield return p;
                    }
                }
            }
        }

        private float DistanceSquared(Vector2 start, Vector2 end,  Vector2 point)
        {
            var px = end.X - start.X;
            var py = end.Y - start.Y;

            float something = px * px + py * py;

            var u = ((point.X - start.X) * px + (point.Y - start.Y) * py) / something;

            if (u > 1)
            {
                u = 1;
            }
            else if (u < 0)
            {
                u = 0;
            }

            var x = start.X + u * px;
            var y = start.Y + u * py;

            var dx = x - point.X;
            var dy = y - point.Y;

            return dx * dx + dy * dy;
        }

        public float DistanceFromPath(Vector2 point, bool closedPath)
        {
            float distance = float.MaxValue;
            var polyCorners = Points.Length;

            if(!closedPath)
            {
                polyCorners -= 1;
            }

            for (var i = 0; i < polyCorners; i++)
            {
                var next = i + 1;
                if (closedPath && next == polyCorners)
                {
                    next = 0;
                }

                var lastDistance = DistanceSquared(Points[i], Points[next], point);

                if (lastDistance < distance)
                {
                    distance = lastDistance;
                }
            }

            return (float)Math.Sqrt(distance);
        }

        public RectangleF Bounds
        {
            get;
        }
    }
}

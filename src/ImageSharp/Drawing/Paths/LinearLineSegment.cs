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

    public class LinearLineSegment : ILineSegment
    {
        public Vector2[] Points;
        public LinearLineSegment(IEnumerable<PointF> points)
            : this(points?.Select(x => x.ToVector2()))
        {
        }

        public LinearLineSegment(IEnumerable<Point> points)
            : this(points?.Select(x => x.ToVector2()))
        {
        }

        public LinearLineSegment(params PointF[] points)
            : this(points?.Select(x => x.ToVector2()))
        {
        }

        internal LinearLineSegment(IEnumerable<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));
            Guard.MustBeGreaterThanOrEqualTo(points.Count(), 2, nameof(points));

            Points = points.ToArray();

            var minX = Points.Min(x => x.X);
            var maxX = Points.Max(x => x.X);
            var minY = Points.Min(x => x.Y);
            var maxY = Points.Max(x => x.Y);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
        
        private float DistanceSquared(Vector2 start, Vector2 end, Vector2 point)
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

        private float DistanceFromPath(Vector2 point)
        {
            float distance = float.MaxValue;
            var polyCorners = Points.Length -1;
            
            for (var i = 0; i < polyCorners; i++)
            {
                var next = i + 1;
                
                var lastDistance = DistanceSquared(Points[i], Points[next], point);

                if (lastDistance < distance)
                {
                    distance = lastDistance;
                }
            }

            return (float)Math.Sqrt(distance);
        }

        /// <summary>
        /// the distance of the point from the outline of the shape, if the value is negative it is inside the polygon bounds
        /// </summary>
        /// <param name="point">The point</param>
        /// <returns></returns>
        public float Distance(PointF point)
        {
            return DistanceFromPath(point.ToVector2());
        }

        /// <summary>
        /// the distance of the point from the outline of the shape, if the value is negative it is inside the polygon bounds
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        float IPath.Distance(int x, int y)
        {
            return DistanceFromPath(new Vector2(x, y));
        }

        /// <summary>
        /// Returns all the points the 2 lines cross.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public IEnumerable<PointF> CrossingPoints(PointF start, PointF end)
        {
            
            var lineCounts = this.Points.Length-1;
            var startVector = start.ToVector2();
            var endVector = end.ToVector2();
            List<PointF> discovered = new List<ImageSharp.PointF>();
            for (var i=0; i< lineCounts; i++)
            {
                var startLine = Points[i];
                var endLine = Points[i+1];

                if (endVector == startLine || endVector == endLine)
                {
                    //if the end is exactly on the end of the line then its laread crossing the line
                    discovered.Add(end);
                }
                else if (startVector == startLine || startVector == endLine)
                {
                    //if the start is exactly on the end of the line then its laread crossing the line
                    discovered.Add(start);
                }
                else
                {
                    var point = LineIntersectionPoint(startLine, endLine, startVector, endVector);

                    if (point != null)
                    {
                        var p = point.Value;
                        // if the found point is exactly on an end point then we are likly skimming the line
                        // and thus outside or inside but we cound as 2 hits so skip it
                        if (p != startLine && p != endLine)
                        {
                            discovered.Add(new PointF(p));
                        }
                    }
                }
            }

            return discovered;
        }

        
        
        Vector2? LineIntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            float py1 = end1.Y - start1.Y;
            float px1 = start1.X - end1.X;
            float other1 = py1 * start1.X + px1 * start1.Y;
            
            float py2 = end2.Y - start2.Y;
            float px2 = start2.X - end2.X;
            float other2 = py2 * start2.X + px2 * start2.Y;

            float delta = py1 * px2 - py2 * px1;
            if (delta < 0.00001f)
            {
                return null;
            }
            var point = new Vector2(
                (px2 * other1 - px1 * other2) / delta,
                (py1 * other2 - py2 * other1) / delta
            );

            var distSquaredTotal = (start1 - end1).LengthSquared();
            var distSquaredStart = (start1 - point).LengthSquared();
            var distSquaredEnd = (end1 - point).LengthSquared();
            var finalDistance = (distSquaredStart + distSquaredEnd) - distSquaredTotal;

            if (finalDistance < 0.00001f)
            {
                return point;
            }

            return null;
        }
        
        public RectangleF Bounds
        {
            get;
        }

        public PointF Start
        {
            get
            {
                return new PointF(Points[0]);
            }
        }

        public PointF End
        {
            get
            {
                return new PointF(Points[Points.Length-1]);
            }
        }
    }
}

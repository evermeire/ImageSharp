// <copyright file="LinearLineSegment.cs" company="James Jackson-South">
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

    /// <summary>
    /// Represents a seriese of control points that will be joined by staight lines
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.Paths.ILineSegment" />
    public class LinearLineSegment : ILineSegment
    {
        private Vector2[] points;

        public LinearLineSegment(IEnumerable<PointF> points)
            : this(points?.Select(x => x.ToVector2()).ToArray())
        {
        }

        public LinearLineSegment(IEnumerable<Point> points)
            : this(points?.Select(x => x.ToVector2()).ToArray())
        {
        }

        public LinearLineSegment(params PointF[] points)
            : this(points?.Select(x => x.ToVector2()).ToArray())
        {
        }

        internal LinearLineSegment(Vector2 start, Vector2 end)
            : this(new[] { start, end })
        {
        }

        internal LinearLineSegment(Vector2[] points)
        {
            Guard.NotNull(points, nameof(points));
            Guard.MustBeGreaterThanOrEqualTo(points.Count(), 2, nameof(points));

            this.points = points;

            //var minX = Points.Min(x => x.X);
            //var maxX = Points.Max(x => x.X);
            //var minY = Points.Min(x => x.Y);
            //var maxY = Points.Max(x => x.Y);

            //Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public IEnumerable<Vector2> AsSimpleLinearPath()
        {
            return points;
        }
    }
}

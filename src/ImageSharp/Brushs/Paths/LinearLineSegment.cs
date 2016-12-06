// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushs
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public class LinearLineSegment : ILineSegment
    {
        public IReadOnlyList<Point> ControlPoints { get; }

        public LinearLineSegment(params Point[] points)
        {
            Guard.NotNull(points, nameof(points));
            Guard.MustBeGreaterThanOrEqualTo(points.Length, 2, nameof(points));

            ControlPoints = new ReadOnlyCollection<Point>(points.ToList());
        }

        public IEnumerable<SimpleLineSegment> Simplify(float quality)
        {
            var lastPoint = ControlPoints.First();
            foreach (var point in ControlPoints.Skip(1))
            {
                yield return new SimpleLineSegment(lastPoint, point);
                lastPoint = point;
            }
        }
    }
}

// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Shapes
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public class LinearLineSegment : ILineSegment
    {
        private readonly IEnumerable<Vector2> controlPoints;

        internal LinearLineSegment(IEnumerable<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));
            Guard.MustBeGreaterThanOrEqualTo(points.Count(), 2, nameof(points));

            controlPoints = points;
        }

        public LinearLineSegment(params Point[] points)
           : this(points?.Select(x=>x.ToVector2()))
        {
        }

        public LinearLineSegment(IEnumerable<Point> points)
           : this(points?.Select(x => x.ToVector2()))
        {
          
        }

        public IEnumerable<Vector2> Simplify()
        {
            return controlPoints;
        }
    }
}

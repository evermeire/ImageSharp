﻿// <copyright file="IImageSampler.cs" company="James Jackson-South">
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
        public IReadOnlyList<Vector2> ControlPoints { get; }

       
        internal LinearLineSegment(IEnumerable<Vector2> points)
        {
            Guard.NotNull(points, nameof(points));
            Guard.MustBeGreaterThanOrEqualTo(points.Count(), 2, nameof(points));

            ControlPoints = new ReadOnlyCollection<Vector2>(points.ToList());
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
            return ControlPoints;
        }
    }
}

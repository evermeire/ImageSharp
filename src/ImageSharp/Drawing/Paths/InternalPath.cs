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
        private readonly ILineSegment[] segments;

        internal InternalPath(IEnumerable<ILineSegment> segments, bool ensureClosed)
        {
            Guard.NotNull(segments, nameof(segments));

            this.segments = FixSegments(segments, ensureClosed);
            
            var minX = segments.Min(x => x.Bounds.Top);
            var maxX = segments.Max(x => x.Bounds.Bottom);
            var minY = segments.Min(x => x.Bounds.Left);
            var maxY = segments.Max(x => x.Bounds.Right);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        private ILineSegment[] FixSegments(IEnumerable<ILineSegment> segments, bool ensureClosed)
        {
            List<ILineSegment> results = new List<ILineSegment>();
            PointF? first = null;
            PointF? last = null;
            foreach(var s in segments)
            {
                if(first == null)
                {
                    first = s.Start;
                }

                if(last != null)
                {
                    if(s.Start != last.Value)
                    {
                        // is there a gap between segments?
                        // add int a linear segment joining them
                        results.Add(new LinearLineSegment(last.Value, s.Start));
                    }
                }

                results.Add(s);

                last = s.End;
            }

            if (ensureClosed)
            {
                if (first.Value != last.Value)
                {
                    // is there a gap between last segment and first segment?
                    // add in a linear segment joining them
                    results.Add(new LinearLineSegment(last.Value, first.Value));
                }
            }

            return results.ToArray();
        }


        public IEnumerable<Vector2> CrossingPoints(Vector2 start, Vector2 end)
        {
            var s = new PointF(start);
            var e = new PointF(end);

            var points = this.segments.SelectMany(x => x.CrossingPoints(s, e)).Select(x=>x.ToVector2()).ToList();
            return points;
        }

        public float DistanceFromPath(Vector2 point)
        {
            var p = new PointF(point);
            return this.segments.Select(x => x.Distance(p)).Min();
        }

        public RectangleF Bounds
        {
            get;
        }
    }
}

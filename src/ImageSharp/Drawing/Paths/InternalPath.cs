﻿// <copyright file="IImageSampler.cs" company="James Jackson-South">
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
        internal readonly Vector2[] points;
        private readonly bool closedPath;

        internal InternalPath(IEnumerable<ILineSegment> segments, bool isClosedPath)
        {
            Guard.NotNull(segments, nameof(segments));

            this.points = FixSegments(segments);
            this.closedPath = isClosedPath;
            
            var minX = points.Min(x => x.X);
            var maxX = points.Max(x => x.X);
            var minY = points.Min(x => x.Y);
            var maxY = points.Max(x => x.Y);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        private Vector2[] FixSegments(IEnumerable<ILineSegment> segments)
        {
            return segments.SelectMany(x => x.AsSimpleLinearPath()).ToArray();
        }

        private float[] constant;
        private float[] multiple;
        object locker = new object();
        bool calculated = false;
        
        private void CalculateConstants()
        {
            if (calculated) return;

            lock (locker)
            {
                if (calculated) return;

                var poly = points;
                var polyCorners = poly.Length;
                constant = new float[polyCorners];
                multiple = new float[polyCorners];
                int i, j = polyCorners - 1;

                for (i = 0; i < polyCorners; i++)
                {
                    if (poly[j].Y == poly[i].Y)
                    {
                        constant[i] = poly[i].X;
                        multiple[i] = 0;
                    }
                    else
                    {
                        constant[i] = poly[i].X - (poly[i].Y * poly[j].X) / (poly[j].Y - poly[i].Y) + (poly[i].Y * poly[i].X) / (poly[j].Y - poly[i].Y);
                        multiple[i] = (poly[j].X - poly[i].X) / (poly[j].Y - poly[i].Y);
                    }
                    j = i;
                }

                calculated = true;
            }
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

        public float DistanceFromPath(Vector2 point)
        {
            float distance = float.MaxValue;
            var polyCorners = points.Length;

            if (!closedPath)
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

                var lastDistance = DistanceSquared(points[i], points[next], point);

                if (lastDistance < distance)
                {
                    distance = lastDistance;
                }
            }

            return (float)Math.Sqrt(distance);
        }


        public bool PointInPolygon(Vector2 point)
        {
            //can only be closed if its a closed path
            if (!closedPath)
            {
                return false;
            }

            if (!Bounds.Contains(point.X, point.Y))
            {
                return false;
            }

            CalculateConstants();

            var poly = points;
            var polyCorners = poly.Length;

            var j = polyCorners - 1;
            bool oddNodes = false;

            for (var i = 0; i < polyCorners; i++)
            {
                if ((poly[i].Y < point.Y && poly[j].Y >= point.Y
                || poly[j].Y < point.Y && poly[i].Y >= point.Y))
                {
                    oddNodes ^= (point.Y * multiple[i] + constant[i] < point.X);
                }
                j = i;
            }

            return oddNodes;
        }

        public RectangleF Bounds
        {
            get;
        }
    }
}

// <copyright file="InternalPath.cs" company="James Jackson-South">
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
    /// Internal logic for interigating linear paths.
    /// </summary>
    internal class InternalPath
    {
        private readonly Vector2[] points;
        private readonly bool closedPath;
        private readonly Lazy<float> totalDistance;

        private float[] constant;
        private float[] multiple;
        private float[] distance;
        private object locker = new object();
        private bool calculated = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalPath"/> class.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="isClosedPath">if set to <c>true</c> [is closed path].</param>
        internal InternalPath(IEnumerable<ILineSegment> segments, bool isClosedPath)
        {
            Guard.NotNull(segments, nameof(segments));

            this.points = this.FixSegments(segments);
            this.closedPath = isClosedPath;

            var minX = this.points.Min(x => x.X);
            var maxX = this.points.Max(x => x.X);
            var minY = this.points.Min(x => x.Y);
            var maxY = this.points.Max(x => x.Y);

            this.Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            this.totalDistance = new Lazy<float>(this.CalculateLength);
        }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        internal Vector2[] Points => this.points;

        private Vector2[] FixSegments(IEnumerable<ILineSegment> segments)
        {
            return segments.SelectMany(x => x.AsSimpleLinearPath()).ToArray();
        }

        private float CalculateLength()
        {
            float length = 0;
            var polyCorners = this.points.Length;

            if (!this.closedPath)
            {
                polyCorners -= 1;
            }

            for (var i = 0; i < polyCorners; i++)
            {
                var next = i + 1;
                if (this.closedPath && next == polyCorners)
                {
                    next = 0;
                }

                length += Vector2.Distance(this.points[i], this.points[next]);
            }

            return length;
        }

        private void CalculateConstants()
        {
            // http://alienryderflex.com/polygon/ source for point in polygon logic

            if (this.calculated)
            {
                return;
            }

            lock (this.locker)
            {
                if (this.calculated)
                {
                    return;
                }

                var poly = this.points;
                var polyCorners = poly.Length;
                constant = new float[polyCorners];
                multiple = new float[polyCorners];
                distance = new float[polyCorners];
                int i, j = polyCorners - 1;

                this.distance[0] = 0;

                for (i = 0; i < polyCorners; i++)
                {
                    this.distance[j] = this.distance[i] + Vector2.Distance(poly[i], poly[j]);
                    if (poly[j].Y == poly[i].Y)
                    {
                        this.constant[i] = poly[i].X;
                        this.multiple[i] = 0;
                    }
                    else
                    {
                        var subtracted = poly[j] - poly[i];
                        this.constant[i] = (poly[i].X - ((poly[i].Y * poly[j].X) / subtracted.Y)) + ((poly[i].Y * poly[i].X) / subtracted.Y);
                        this.multiple[i] = subtracted.X / subtracted.Y;
                    }
                    j = i;
                }

                this.calculated = true;
            }
        }

        private bool CalculateShorterDistance(Vector2 start, Vector2 end, Vector2 point, ref PointInfoInternal info)
        {
            // TODO look at this maths and see if we can use more efficiant Vector2 maths?
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

            var dist = dx * dx + dy * dy;

            if (info.DistanceSquared > dist)
            {
                info.DistanceSquared = dist;
                info.x = x;
                info.y = y;
                return true;
            }
            return false;
        }

        public PointInfo DistanceFromPath(Vector2 point)
        {
            this.CalculateConstants();

            var internalInfo = new PointInfoInternal();
            internalInfo.DistanceSquared = float.MaxValue;//set it to max so that CalculateShorterDistance can reduce it back down

            var polyCorners = this.points.Length;

            if (!this.closedPath)
            {
                polyCorners -= 1;
            }
            
            int closestPoint = 0;
            for (var i = 0; i < polyCorners; i++)
            {
                var next = i + 1;
                if (this.closedPath && next == polyCorners)
                {
                    next = 0;
                }

                if (this.CalculateShorterDistance(this.points[i], this.points[next], point, ref internalInfo))
                {
                    closestPoint = i;
                }
            }

            return new PointInfo
            {
                DistanceAlongPath = this.distance[closestPoint] + Vector2.Distance(this.points[closestPoint], point),
                DistanceFromPath = (float)Math.Sqrt(internalInfo.DistanceSquared),
                SearchPoint = point,
                ClosestPointOnPath = new Vector2(internalInfo.x, internalInfo.y)
            };
        }


        public bool PointInPolygon(Vector2 point)
        {
            //can only be closed if its a closed path
            if (!this.closedPath)
            {
                return false;
            }

            if (!this.Bounds.Contains(point.X, point.Y))
            {
                return false;
            }

            this.CalculateConstants();

            var poly = this.points;
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


        public Vector2 Start
        {
            get
            {
                return points[0];
            }
        }

        public Vector2 End
        {
            get
            {
                if (closedPath)
                {
                    return points[0];
                }
                else
                {

                    return points[points.Length-1];
                }
            }
        }

        public float Length => totalDistance.Value;

        private struct PointInfoInternal
        {
            public float DistanceSquared;
            public float x;
            public float y;
        }


    }
}

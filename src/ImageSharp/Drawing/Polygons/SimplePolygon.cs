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
            innerPath = new InternalPath(segments);
            CalculateConstants();
        }


        private void CalculateConstants()
        {
            var poly = innerPath.Points;
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
        }
        
        bool PointInPolygon(Vector2 point)
        {
            if (!innerPath.Bounds.Contains(point.X, point.Y))
            {
                return false;
            }

            var poly = innerPath.Points;
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

        public float Distance(Vector2 point)
        {
            bool isInside = PointInPolygon(point);

            var dist = innerPath.DistanceFromPath(point, true);

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

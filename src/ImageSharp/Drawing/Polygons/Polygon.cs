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
    /// a <see cref="Polygon"/> represents a contiguose bound region 
    /// </summary>
    public sealed class Polygon : IShape
    {
        private float[] constant;
        private float[] multiple;
        private readonly InternalPath innerPath;

        public RectangleF Bounds => innerPath.Bounds;

        public Polygon(params ILineSegment[] segments)
            : this((IEnumerable<ILineSegment>)segments)
        {
        }

        public Polygon(IEnumerable<ILineSegment> segments)
        {
            innerPath = new InternalPath(segments, true);
        }
        
        public float Distance(Vector2 point)
        {
            bool isInside = innerPath.PointInPolygon(point);

            var dist = innerPath.DistanceFromPath(point);

            if (isInside)
            {
                return -dist;
            }
            else
            {
                return dist;
            }
        }

        public float Distance(int x, int y)
        {
            return this.Distance(new Vector2(x, y));
        }        
    }
}

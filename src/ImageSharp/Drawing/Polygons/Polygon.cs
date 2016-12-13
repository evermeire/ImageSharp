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
    using System.Collections;

    /// <summary>
    /// a <see cref="Polygon"/> represents a contiguose bound region 
    /// </summary>
    public sealed class Polygon : IShape, IPath
    {
        private float[] constant;
        private IEnumerable<IPath> pathCollection;
        private float[] multiple;
        private readonly InternalPath innerPath;

        public RectangleF Bounds => innerPath.Bounds;

        public Vector2 Start => innerPath.Start;

        public Vector2 End => innerPath.Start;

        public float Length => innerPath.Length;

        public bool IsClosed => true;

        public Polygon(params ILineSegment[] segments)
            : this((IEnumerable<ILineSegment>)segments)
        {
        }

        public Polygon(IEnumerable<ILineSegment> segments)
        {
            innerPath = new InternalPath(segments, true);
            pathCollection = new[] { this };
        }
        
        public float Distance(Vector2 point)
        {
            bool isInside = innerPath.PointInPolygon(point);

            var dist = innerPath.DistanceFromPath(point);

            if (isInside)
            {
                return -dist.DistanceFromPath;
            }
            else
            {
                return dist.DistanceFromPath;
            }
        }

        public float Distance(int x, int y)
        {
            return this.Distance(new Vector2(x, y));
        }
        
        public IEnumerator<IPath> GetEnumerator()
        {
            return pathCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pathCollection.GetEnumerator();
        }
        
        PointInfo IPath.Distance(int x, int y)
        {
            return innerPath.DistanceFromPath(new Vector2(x,y));
        }
    }
}

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
    /// a <see cref="BezierPolygon"/> represents a contiguose bound region 
    /// </summary>
    public sealed class BezierPolygon : IShape
    {
        Polygon innerPolygon;
        
        public BezierPolygon(params Point[] points)
        {
            innerPolygon = new Polygon(new BezierLineSegment(points));
        }

        public BezierPolygon(params PointF[] points)
        {
            innerPolygon = new Polygon(new BezierLineSegment(points));
        }

        public RectangleF Bounds => innerPolygon.Bounds;

        public float Distance(int x, int y) => innerPolygon.Distance(x, y);

        public IEnumerator<IPath> GetEnumerator()
        {
            return innerPolygon.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerPolygon.GetEnumerator();
        }
    }
}

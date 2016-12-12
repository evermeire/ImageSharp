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
    /// a <see cref="BezierPolygon"/> represents a contiguose bound region 
    /// </summary>
    public sealed class LinearPolygon : IShape
    {
        Polygon innerPolygon;
        
        public LinearPolygon(params Point[] points)
        {
            innerPolygon = new Polygon(new LinearLineSegment(points));
        }

        public LinearPolygon(params PointF[] points)
        {
            innerPolygon = new Polygon(new LinearLineSegment(points));
        }

        public RectangleF Bounds => innerPolygon.Bounds;

        public float Distance(int x, int y) => innerPolygon.Distance(x, y);
    }
}

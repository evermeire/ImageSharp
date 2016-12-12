// <copyright file="DrawProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using Drawing;
    
    internal class DrawPathProcessor<TColor, TPacked> : DrawShapeProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        public DrawPathProcessor(IPen<TColor, TPacked> pen, IPath path) 
            : base(pen, new PathToShapeConverter(path))
        {
        }

        private class PathToShapeConverter : IShape
        {
            private readonly IPath path;

            public PathToShapeConverter(IPath path)
            {
                this.path = path;
            }

            public RectangleF Bounds => path.Bounds;

            public float Distance(int x, int y) => path.Distance(x, y);
        }
    }
}
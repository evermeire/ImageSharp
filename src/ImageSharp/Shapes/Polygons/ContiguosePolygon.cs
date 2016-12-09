// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Shapes.Polygons
{
    using Brushes;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// a <see cref="ContiguosePolygon"/> represents a contiguos bound region with one or 
    /// more holes bound within its shape
    /// </summary>
    internal class ContiguosePolygon
    { 
        private Lazy<Rectangle> bounds;
        private IEnumerable<SimplePolygon> holes;
        private SimplePolygon outline;
        

        public ContiguosePolygon(SimplePolygon outline, IEnumerable<SimplePolygon> holes)
        {
            this.outline = outline;
            this.holes = holes;
        }

        public Rectangle Bounds => outline.Bounds;        

        private void Simplify(IEnumerable<SimplePolygon> source)
        {


            //lets merge all holes that 
            throw new NotImplementedException();
        }
    }
}

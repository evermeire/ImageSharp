// <copyright file="ComplexPolygon.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>


namespace ImageSharp.Drawing.Shapes
{
    using ImageSharp.Drawing.Paths;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a complex polygon made up of one or more outline 
    /// polygons and one or more holes to punch out of them.
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.Shapes.IShape" />
    public sealed class ComplexPolygon : IShape
    {
        const float clipperScaleFactor = 100f;

        private IEnumerable<IShape> holes;
        private IEnumerable<IShape> outlines;
        IEnumerable<IPath> paths = null;

        bool pathsFixed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPolygon"/> class.
        /// </summary>
        /// <param name="outline">The outline.</param>
        /// <param name="holes">The holes.</param>
        public ComplexPolygon(IShape outline, params IShape[] holes)
            : this(new[] { outline }, (IEnumerable<IShape>)holes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPolygon"/> class.
        /// </summary>
        /// <param name="outlines">The outlines.</param>
        /// <param name="holes">The holes.</param>
        public ComplexPolygon(IEnumerable<IShape> outlines, IEnumerable<IShape> holes)
        {
            Guard.NotNull(outlines, nameof(outlines));
            Guard.MustBeGreaterThanOrEqualTo(outlines.Count(), 1, nameof(outlines));

            FixAndSetShapes(outlines, holes);

            var minX = outlines.Min(x => x.Bounds.Left);
            var maxX = outlines.Max(x => x.Bounds.Right);
            var minY = outlines.Min(x => x.Bounds.Top);
            var maxY = outlines.Max(x => x.Bounds.Bottom);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        private void AddPoints(ClipperLib.Clipper clipper, IShape shape, bool isHole)
        {
            foreach (var path in shape)
            {
                var points = path.AsSimpleLinearPath();
                var clipperPoints = new List<ClipperLib.IntPoint>();
                foreach (var point in points)
                {
                    var p = point * clipperScaleFactor;

                    clipperPoints.Add(new ClipperLib.IntPoint((long)p.X, (long)p.Y));
                }
                clipper.AddPath(clipperPoints,
                    (isHole ? ClipperLib.PolyType.ptClip : ClipperLib.PolyType.ptSubject),
                    path.IsClosed);
            }
        }

        private void AddPoints(ClipperLib.Clipper clipper, IEnumerable<IShape> shapes, bool isHole)
        {
            foreach (var shape in shapes)
            {
                AddPoints(clipper, shape, isHole);
            }
        }

        private void ExtractOutlines(ClipperLib.PolyNode tree, List<Polygon>  outlines, List<Polygon> holes)
        {
            if (tree.Contour.Any())
            {
                //convert the Clipper Contour from scaled ints back down to the origional size (this is going to be lossy but not significantly)
                var polygon = new Polygon(new LinearLineSegment(tree.Contour.Select(x => new Vector2(x.X / clipperScaleFactor, x.Y / clipperScaleFactor)).ToArray()));

                if (tree.IsHole)
                {
                    holes.Add(polygon);
                }
                else
                {
                    outlines.Add(polygon);
                }
            }

            foreach (var c in tree.Childs)
            {
                ExtractOutlines(c, outlines, holes);
            }
        }

        private void FixAndSetShapes(IEnumerable<IShape> outlines, IEnumerable<IShape> holes)
        {
            var clipper = new ClipperLib.Clipper();

            //add the outlines and the holes to clipper, scaling up from the float source to the int based system clipper uses
            AddPoints(clipper, outlines, false);
            AddPoints(clipper, holes, true);

            var tree = new ClipperLib.PolyTree();
            clipper.Execute(ClipperLib.ClipType.ctDifference, tree);

            
            List<Polygon> newOutlines = new List<Polygon>();
            List<Polygon> newHoles = new List<Polygon>();
            
            //convert the 'tree' back to paths
            ExtractOutlines(tree, newOutlines, newHoles);

            this.outlines = newOutlines;
            this.holes = newHoles;

            //extract the final list of paths out of the new polygons we just converted down to.
            paths = newOutlines.Union(newHoles).ToArray();
        }

        public RectangleF Bounds { get; }

        float IShape.Distance(int x, int y)
        {
            // get the outline we are closest to the center of
            // by rights we should only be inside 1 outline
            // othersie we will start returning the distanct to the nearest shape
            var dist = outlines.Select(o => o.Distance(x, y)).OrderBy(p => p).First();

            if (dist < 0)//inside poly
            {
                foreach (var hole in this.holes)
                {
                    var distFromHole = hole.Distance(x, y);

                    //less than zero we are inside shape
                    if (distFromHole <= 0)
                    {
                        //invert distance
                        dist = distFromHole * -1;
                        break;
                    }
                }
            }

            return dist;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IPath> GetEnumerator()
        {
            return paths.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
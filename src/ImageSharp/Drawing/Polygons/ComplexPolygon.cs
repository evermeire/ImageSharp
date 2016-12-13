using ImageSharp.Drawing.Paths;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageSharp.Drawing.Polygons
{
    public sealed class ComplexPolygon : IShape
    {
        private IEnumerable<IShape> holes;
        private IEnumerable<IShape> outlines;
        IEnumerable<IPath> paths = null;

        bool pathsFixed = false;

        public ComplexPolygon(IShape outline, params IShape[] holes)
            : this(outline, (IEnumerable<IShape>)holes)
        {
        }

        public ComplexPolygon(IShape outline, IEnumerable<IShape> holes)
        {
            Guard.NotNull(outline, nameof(outline));

            FixAndSetShapes(outline, holes);

            var minX = outlines.Min(x => x.Bounds.Left);
            var maxX = outlines.Max(x => x.Bounds.Right);
            var minY = outlines.Min(x => x.Bounds.Top);
            var maxY = outlines.Max(x => x.Bounds.Bottom);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        const float clipperScaleFactor = 100f;
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
                    isHole ? ClipperLib.PolyType.ptClip : ClipperLib.PolyType.ptSubject,
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

        private void ExtractOutlines(ClipperLib.PolyNode tree, List<IShape>  outlines, List<IShape> holes)
        {
            if (tree.Contour.Any())
            {
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
        private void FixAndSetShapes(IShape outline, IEnumerable<IShape> holes)
        {
            // TODO this needs fixing using clipper to simplify all the shapes to get the paths returned properly
            var clipper = new ClipperLib.Clipper();

            AddPoints(clipper, outline, false);
            AddPoints(clipper, holes, true);

            var tree = new ClipperLib.PolyTree();
            clipper.Execute(ClipperLib.ClipType.ctDifference, tree);

            //convert the 'tree' back to paths
            List<IShape> newOutlines = new List<Drawing.IShape>();
            List<IShape> newHoles = new List<Drawing.IShape>();

            ExtractOutlines(tree, newOutlines, newHoles);

            this.outlines = newOutlines;
            this.holes = newHoles;

            paths = newOutlines.SelectMany(x => x).Union(newHoles.SelectMany(x => x)).ToArray();
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
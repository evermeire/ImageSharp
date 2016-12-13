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

        public ComplexPolygon(IShape outline, params IShape[] holes)
            : this(outline, (IEnumerable<IShape>)holes)
        {
        }

        public ComplexPolygon(IShape outline, IEnumerable<IShape> holes)
        {
            Guard.NotNull(outline, nameof(outline));

            this.outlines = new[] { outline };
            this.holes = holes ?? Enumerable.Empty<IShape>();

            var minX = outlines.Min(x => x.Bounds.Left);
            var maxX = outlines.Max(x => x.Bounds.Right);
            var minY = outlines.Min(x => x.Bounds.Top);
            var maxY = outlines.Max(x => x.Bounds.Bottom);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        private void FixPolygons()
        {
            // TODO iterate over all the polygons and fix any overlapping verticies 
            // making sure we never have any overlapping polygons
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
                foreach (var hole in holes)
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
            // TODO this needs fixing using clipper to simplify all the shapes to get the paths returned properly

            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
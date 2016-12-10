using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageSharp.Drawing.Polygons
{
    internal class ComplexPolygon : IShape
    {
        private readonly IEnumerable<SimplePolygon> simplePolygons;
        private IEnumerable<SimplePolygon> holes;
        private IEnumerable<SimplePolygon> outlines;

        public ComplexPolygon(IEnumerable<ILineSegment> segments) 
            :this(new SimplePolygon(segments), null)
        { }

        public ComplexPolygon(SimplePolygon outline, IEnumerable<SimplePolygon> holes)
        {
            Guard.NotNull(outline, nameof(outline));

            this.outlines = new[] { outline };
            this.holes = holes ?? Enumerable.Empty<SimplePolygon>();

            var minX = outlines.Min(x => x.Bounds.Left);
            var maxX = outlines.Max(x => x.Bounds.Right);
            var minY = outlines.Min(x => x.Bounds.Top);
            var maxY = outlines.Max(x => x.Bounds.Bottom);

            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - maxY);
        }
        
        private void FixPolygons()
        {
            // TODO iterate over all the polygons and fix any overlapping verticies 
            // making sure we never have any overlapping polygons
        }

        public RectangleF Bounds { get; }

        public float Distance(Vector2 point)
        {
            // get the outline we are closest to the center of
            // by rights we should only be inside 1 outline
            // othersie we will start returning the distanct to the nearest shape
            var dist = outlines.Select(o=>o.Distance(point)).OrderBy(x=>x).First();

            if (dist == 0)
            {
                foreach (var hole in holes)
                {
                    var distFromHole = hole.Distance(point);

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

        float IShape.Distance(int x, int y)
        {
            return this.Distance(new Vector2( x, y));
        }
    }
}

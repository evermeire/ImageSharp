using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSharp.Shapes.Polygons
{
    internal class ComplexPolygon : IShape
    {
        private readonly IEnumerable<SimplePolygon> simplePolygons;
        private IEnumerable<SimplePolygon> holes;
        private IEnumerable<SimplePolygon> outlines;
        private readonly Lazy<Rectangle> lazyRect;

        public ComplexPolygon(IEnumerable<ILineSegment> segments) 
            :this(new[] { new SimplePolygon(segments) })
        { }

        public ComplexPolygon(IEnumerable<SimplePolygon> simplePolygons)
        {
            this.simplePolygons = simplePolygons;
            this.lazyRect = new Lazy<Rectangle>(() =>
            {
                this.Simplify();

                var minY = outlines.Min(x => x.Bounds.Top);
                var maxY = outlines.Min(x => x.Bounds.Bottom);

                var minX = outlines.Min(x => x.Bounds.Left);
                var maxX = outlines.Min(x => x.Bounds.Right);

                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            });
        }

        private void Simplify()
        {
            outlines = simplePolygons.Where(x => !x.IsHole).ToList();
            holes = simplePolygons.Where(x => x.IsHole).ToList();
        }

        public Rectangle Bounds => lazyRect.Value;

        public float Distance(float x, float y)
        {
            var dist = outlines.Min(o=>o.Distance(x, y));

            if (dist == 0)
            {
                foreach (var hole in holes)
                {
                    var distFromHole = hole.Distance(x, y);

                    if (distFromHole != 0)
                    {
                        //we are in the hole 
                        dist = distFromHole;
                        break;
                    }
                }
            }

            return dist;
        }

        float IShape.Distance(int x, int y)
        {
            return this.Distance(x, y);
        }
    }
}

using ImageSharp.Drawing.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp.Drawing.Paths;
using System.Collections;
using System.Numerics;

namespace ImageSharp.Drawing
{
    public class GlyphPolygon : IShape
    {
        private readonly Polygon[] polygons;

        public GlyphPolygon(Polygon[] polygons)
        {
            this.polygons = polygons;

            var minX = this.polygons.Min(x => x.Bounds.Left);
            var maxX = this.polygons.Max(x => x.Bounds.Right);
            var minY = this.polygons.Min(x => x.Bounds.Top);
            var maxY = this.polygons.Max(x => x.Bounds.Bottom);

            this.Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public RectangleF Bounds { get; }

        public float Distance(Vector2 point)
        {
            float dist = float.MaxValue;
            bool inside = false;
            foreach (var shape in this.polygons)
            {
                var d = shape.Distance(point);

                if (d <= 0)
                {
                    // we are inside a poly
                    d = -d;  // flip the sign
                    inside ^= true; // flip the inside flag
                }

                if (d < dist)
                {
                    dist = d;
                }
            }

            if (inside)
            {
                return -dist;
            }

            return dist;
        }

        public IEnumerator<IPath> GetEnumerator()
        {
            return ((IEnumerable<IPath>)this.polygons).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.polygons.GetEnumerator();
        }
    }
}

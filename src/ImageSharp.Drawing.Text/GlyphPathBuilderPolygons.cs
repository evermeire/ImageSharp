using ImageSharp.Drawing.Paths;
using ImageSharp.Drawing.Shapes;
using NOpenType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageSharp.Drawing
{
    public class GlyphPathBuilderPolygons : NOpenType.GlyphPathBuilderBase
    {
        List<Polygon> polygons = new List<Polygon>();
        List<ILineSegment> segments = new List<ILineSegment>();
        private Vector2 lastPoint;
        private Vector2 offset;
        private Vector2 scale;

        public GlyphPathBuilderPolygons(Typeface typeface)
            : base(typeface)
        {

        }
        protected override void OnBeginRead(int countourCount)
        {
            segments.Clear();
            polygons.Clear();
        }

        protected override void OnEndRead()
        {
            
        }

        protected override void OnCloseFigure()
        {
            if (segments.Any())
            {
                polygons.Add(new Polygon(segments.ToArray()));
                segments.Clear();
            }
        }

        static readonly Vector2 twoThirds = new Vector2(2f / 3f);

        protected override void OnCurve3(short p2x, short p2y, short x, short y)
        {
            var controlPoint = offset + (new Vector2(p2x, p2y) * scale);
            var endPoint = offset + (new Vector2(x, y) * scale);

            var c1 = ((controlPoint - lastPoint) * (twoThirds)) + lastPoint;
            var c2 = ((controlPoint - endPoint) * (twoThirds)) + endPoint;


            segments.Add( new BezierLineSegment(lastPoint, c1, c2, endPoint));

            this.lastPoint = endPoint;
        }

        protected override void OnCurve4(short p2x, short p2y, short p3x, short p3y, short x, short y)
        {
            var endPoint = offset + (new Vector2(x , y) * scale);
            var c1 = offset + (new Vector2(p2x, p2y) * scale);
            var c2 = offset + (new Vector2(p3x, p3y) * scale);

            segments.Add(new BezierLineSegment(lastPoint, c1, c2, endPoint));
            this.lastPoint = endPoint;
        }

        protected override void OnLineTo(short x, short y)
        {
            var endPoint = offset + (new Vector2(x, y) * scale);
            segments.Add(new LinearLineSegment(lastPoint, endPoint));
            this.lastPoint = endPoint;
        }

        protected override void OnMoveTo(short x, short y)
        {
            //we close of the current segemnts in here
            if (segments.Any())
            {
                polygons.Add(new Polygon(segments.ToArray()));
                segments.Clear();
            }

            this.lastPoint = offset + (new Vector2(x, y) * scale);
        }

        public IShape BuildGlyph(ushort idx, float sizeInPoints, Vector2 offset)
        {
            this.offset = offset + new Vector2(0, sizeInPoints);
            

           var scale = this.TypeFace.CalculateScale(sizeInPoints);

            this.scale = new Vector2(scale, -scale);
            BuildFromGlyphIndex(idx, sizeInPoints);
            if (polygons.Any())
            {
                var result = new ComplexPolygon(polygons.ToArray());
                polygons.Clear();

                return result;
            }else
            {
                return null;
            }
        }
    }    
}

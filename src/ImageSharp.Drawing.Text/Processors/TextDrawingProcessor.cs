using ImageSharp.Drawing.Paths;
using ImageSharp.Drawing.Pens;
using ImageSharp.Drawing.Shapes;
using ImageSharp.Processors;
using NRasterizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageSharp.Drawing.Processors
{
    public class TextDrawingProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct, IEquatable<TPacked>
    {
        private readonly IBrush<TColor, TPacked> brush;
        private readonly GraphicsOptions options;
        private readonly IPen<TColor, TPacked> pen;
        private readonly Vector2 position;
        private readonly int resolution;
        private readonly int size;
        private readonly string text;
        private readonly Typeface typeface;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDrawingProcessor{TColor, TPacked}"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="typeface">The typeface.</param>
        /// <param name="size">The size.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="position">The position.</param>
        /// <param name="options">The options.</param>
        public TextDrawingProcessor(string text, IPen<TColor, TPacked> pen, IBrush<TColor, TPacked> brush, Typeface typeface, int size, int resolution, Vector2 position, GraphicsOptions options)
        {
            this.text = text;
            this.pen = pen;
            this.brush = brush;
            this.position = position;
            this.size = size;
            this.typeface = typeface;
            this.options = options;
            this.resolution = resolution;
        }

        protected override void OnApply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle)
        {
            var rasterizer = new ShapeRasterizer(source, sourceRectangle, brush, pen, this.size, resolution, position, this.options);
            var renderer = new NRasterizer.Renderer(this.typeface, rasterizer);
            renderer.Render(0, 0, this.text, this.size, 72);
        }

        private class ShapeRasterizer : NRasterizer.IGlyphRasterizer
        {
            private static readonly Vector2 TwoThirds = new Vector2(2f / 3f);

            private List<IShape> glyphs = new List<IShape>();
            private List<Polygon> polygons = new List<Polygon>();
            private List<ILineSegment> segments = new List<ILineSegment>();
            private Vector2 lastPoint;
            private Vector2 offset;
            private Vector2 scale;
            private readonly ImageBase<TColor, TPacked> image;
            private readonly IBrush<TColor, TPacked> brush;
            private readonly IPen<TColor, TPacked> pen;
            private readonly GraphicsOptions options;
            private readonly Rectangle sourceRectangle;

            public ShapeRasterizer(ImageBase<TColor, TPacked> image, Rectangle sourceRectangle, IBrush<TColor, TPacked> brush, IPen<TColor, TPacked> pen, float size, float resolution, Vector2 position, GraphicsOptions options)
            {
                this.offset = position;
                this.sourceRectangle = sourceRectangle;
                this.image = image;
                this.options = options;
                this.brush = brush;
                this.pen = pen;
                var scalingFactor = size * resolution;
                //this.scale = new Vector2(scalingFactor, -scalingFactor);

                this.scale = Vector2.One;
            }

            public void BeginRead(int countourCount)
            {
                //start of charachter rendering

                //reset everything here
                this.segments.Clear();
                this.polygons.Clear();
            }

            public void CloseFigure()
            {
                if (this.segments.Any())
                {
                    this.polygons.Add(new Polygon(this.segments.ToArray()));
                    this.segments.Clear();
                }
            }

            public void Curve3(double p2x, double p2y, double x, double y)
            {
                var controlPoint = this.offset + (new Vector2((float)p2x, (float)p2y) * this.scale);
                var endPoint = this.offset + (new Vector2((float)x, (float)y) * this.scale);

                var c1 = ((controlPoint - this.lastPoint) * TwoThirds) + this.lastPoint;
                var c2 = ((controlPoint - endPoint) * TwoThirds) + endPoint;

                this.segments.Add(new BezierLineSegment(this.lastPoint, c1, c2, endPoint));

                this.lastPoint = endPoint;
            }

            public void Curve4(double p2x, double p2y, double p3x, double p3y, double x, double y)
            {
                var endPoint = this.offset + (new Vector2((float)x, (float)y) * this.scale);
                var c1 = this.offset + (new Vector2((float)p2x, (float)p2y) * this.scale);
                var c2 = this.offset + (new Vector2((float)p3x, (float)p3y) * this.scale);

                this.segments.Add(new BezierLineSegment(this.lastPoint, c1, c2, endPoint));
                this.lastPoint = endPoint;
            }

            public void EndRead()
            {
                //this is the end of each glyph
                if (this.polygons.Any())
                {
                    this.glyphs.Add(new GlyphPolygon(this.polygons.ToArray()));
                    this.polygons.Clear();
                }
            }

            public void LineTo(double x, double y)
            {
                var endPoint = this.offset + (new Vector2((float)x, (float)y) * this.scale);
                this.segments.Add(new LinearLineSegment(this.lastPoint, endPoint));
                this.lastPoint = endPoint;
            }

            public void MoveTo(double x, double y)
            {
                // we close of the current segemnts in here
                if (this.segments.Any())
                {
                    this.polygons.Add(new Polygon(this.segments.ToArray()));
                    this.segments.Clear();
                }

                this.lastPoint = this.offset + (new Vector2((float)x, (float)y) * this.scale);
            }

            public void Flush()
            {
                var glyphsArray = this.glyphs.ToArray();
                if (brush != null)
                {
                    var fillProcessor = new FillShapeProcessor<TColor, TPacked>(this.brush, glyphsArray, Vector2.Zero, options);
                    fillProcessor.Apply(image, sourceRectangle);
                }
                if (pen != null)
                {
                    var drawProcessor = new DrawPathProcessor<TColor, TPacked>(this.pen, glyphsArray, Vector2.Zero, options);
                    drawProcessor.Apply(image, sourceRectangle);
                }
            }
        }
    }
}

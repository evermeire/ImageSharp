// <copyright file="GlyphPathBuilderPolygons.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using ImageSharp.Drawing.Paths;
    using ImageSharp.Drawing.Shapes;
    using NOpenType;

    /// <summary>
    /// Used to convert the fint glyphs into GlyphPolygons for rendering.
    /// </summary>
    /// <seealso cref="NOpenType.GlyphPathBuilderBase" />
    internal class GlyphPathBuilderPolygons : NOpenType.GlyphPathBuilderBase
    {
        private static readonly Vector2 TwoThirds = new Vector2(2f / 3f);
        private List<Polygon> polygons = new List<Polygon>();
        private List<ILineSegment> segments = new List<ILineSegment>();
        private Vector2 lastPoint;
        private Vector2 offset;
        private Vector2 scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlyphPathBuilderPolygons"/> class.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        public GlyphPathBuilderPolygons(Typeface typeface)
            : base(typeface)
        {
        }

        /// <summary>
        /// Builds the glyph.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="sizeInPoints">The size in points.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the polygon fro the request glyph scaled and offset</returns>
        public GlyphPolygon BuildGlyph(ushort idx, float sizeInPoints, float scale, Vector2 offset)
        {
            this.offset = offset + new Vector2(0, sizeInPoints);

            this.scale = new Vector2(scale, -scale);
            this.BuildFromGlyphIndex(idx, sizeInPoints);
            if (this.polygons.Any())
            {
                var result = new GlyphPolygon(this.polygons.ToArray());
                this.polygons.Clear();

                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called when [begin read].
        /// </summary>
        /// <param name="countourCount">The countour count.</param>
        protected override void OnBeginRead(int countourCount)
        {
            this.segments.Clear();
            this.polygons.Clear();
        }

        /// <summary>
        /// Called when [end read].
        /// </summary>
        protected override void OnEndRead()
        {
        }

        /// <summary>
        /// Called when [close figure].
        /// </summary>
        protected override void OnCloseFigure()
        {
            if (this.segments.Any())
            {
                this.polygons.Add(new Polygon(this.segments.ToArray()));
                this.segments.Clear();
            }
        }

        /// <summary>
        /// Called when [curve3].
        /// </summary>
        /// <param name="p2x">The P2X.</param>
        /// <param name="p2y">The p2y.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        protected override void OnCurve3(short p2x, short p2y, short x, short y)
        {
            var controlPoint = this.offset + (new Vector2(p2x, p2y) * this.scale);
            var endPoint = this.offset + (new Vector2(x, y) * this.scale);

            var c1 = ((controlPoint - this.lastPoint) * TwoThirds) + this.lastPoint;
            var c2 = ((controlPoint - endPoint) * TwoThirds) + endPoint;

            this.segments.Add(new BezierLineSegment(this.lastPoint, c1, c2, endPoint));

            this.lastPoint = endPoint;
        }

        /// <summary>
        /// Called when [curve4].
        /// </summary>
        /// <param name="p2x">The P2X.</param>
        /// <param name="p2y">The p2y.</param>
        /// <param name="p3x">The P3X.</param>
        /// <param name="p3y">The p3y.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        protected override void OnCurve4(short p2x, short p2y, short p3x, short p3y, short x, short y)
        {
            var endPoint = this.offset + (new Vector2(x, y) * this.scale);
            var c1 = this.offset + (new Vector2(p2x, p2y) * this.scale);
            var c2 = this.offset + (new Vector2(p3x, p3y) * this.scale);

            this.segments.Add(new BezierLineSegment(this.lastPoint, c1, c2, endPoint));
            this.lastPoint = endPoint;
        }

        /// <summary>
        /// Called when [line to].
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        protected override void OnLineTo(short x, short y)
        {
            var endPoint = this.offset + (new Vector2(x, y) * this.scale);
            this.segments.Add(new LinearLineSegment(this.lastPoint, endPoint));
            this.lastPoint = endPoint;
        }

        /// <summary>
        /// Called when [move to].
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        protected override void OnMoveTo(short x, short y)
        {
            // we close of the current segemnts in here
            if (this.segments.Any())
            {
                this.polygons.Add(new Polygon(this.segments.ToArray()));
                this.segments.Clear();
            }

            this.lastPoint = this.offset + (new Vector2(x, y) * this.scale);
        }
    }
}

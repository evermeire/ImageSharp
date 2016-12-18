﻿//Apache2, 2014-2016,   WinterDev
using System;
using System.Collections.Generic;
namespace NOpenType
{
    /// <summary>
    /// gerneral glyph path builder
    /// </summary>
    public class GlyphPathBuilder : GlyphPathBuilderBase
    {
        IGlyphRasterizer _rasterizer;
        float scale;
        public GlyphPathBuilder(Typeface typeface, IGlyphRasterizer ras)
            : base(typeface)
        {
            this._rasterizer = ras;
        }
        protected override void OnBeginRead(int countourCount)
        {
            scale = TypeFace.CalculateScale(SizeInPoints);
            _rasterizer.BeginRead(countourCount);
        }
        protected override void OnCloseFigure()
        {
            _rasterizer.CloseFigure();
        }
        protected override void OnCurve3(short p2x, short p2y, short x, short y)
        {
            _rasterizer.Curve3(p2x * scale, p2y * scale, x * scale, y * scale);
        }
        protected override void OnCurve4(short p2x, short p2y, short p3x, short p3y, short x, short y)
        {
            _rasterizer.Curve4(p2x * scale, p2y * scale, p3x * scale, p3y * scale, x * scale, y * scale);
        }
        protected override void OnLineTo(short x, short y)
        {
            _rasterizer.LineTo(x * scale, y * scale);
        }
        protected override void OnMoveTo(short x, short y)
        {
            _rasterizer.MoveTo(x * scale, y * scale);
        }
        protected override void OnEndRead()
        {
            _rasterizer.EndRead();
        }
        public IGlyphRasterizer Rasterizer { get; set; }

    }

}
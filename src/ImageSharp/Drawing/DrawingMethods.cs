// <copyright file="Draw.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    using Drawing;
    using Drawing.Paths;
    using Drawing.Polygons;
    using Drawing.Processors;
    using Processors;

    /// <summary>
    /// Extension methods for the <see cref="Image{TColor, TPacked}"/> type.
    /// </summary>
    public static partial class ImageExtensions
    {
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(pen, shape));
        }

        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(new Pen(brush, thickness), shape));
        }

        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(new Pen(brush, thickness), new SimplePolygon(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(pen, new SimplePolygon(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(new Pen(brush, thickness), new SimplePolygon(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(pen, new SimplePolygon(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> DrawPath<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, IPath path)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(new Pen(brush, thickness), path));
        }

        public static Image<TColor, TPacked> DrawPath<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, IPath path)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(pen, path));
        }
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(new Pen(brush, thickness), new Path(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(pen, new Path(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(new Pen(brush, thickness), new Path(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(pen, new Path(new LinearLineSegment(points))));
        }
    }
}

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
        public static Image<TColor, TPacked> Fill<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            return source.Process(new FillProcessor<TColor, TPacked>(brush));
        }

        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, IShape shape)
          where TColor : struct, IPackedPixel<TPacked>
          where TPacked : struct
        {
            return source.Process(new FillShapeProcessor<TColor, TPacked>(brush, shape));
        }

        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new FillShapeProcessor<TColor, TPacked>(brush, new SimplePolygon(new LinearLineSegment(points))));
        }

        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, Point[] points)
         where TColor : struct, IPackedPixel<TPacked>
         where TPacked : struct
        {
            return source.Process(new FillShapeProcessor<TColor, TPacked>(brush, new SimplePolygon(new LinearLineSegment(points))));
        }
    }
}

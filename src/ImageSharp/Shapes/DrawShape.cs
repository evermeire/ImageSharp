// <copyright file="Draw.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    using Shapes;
    using Processors;
    using Brushes;
    using Shapes.Polygons;

    /// <summary>
    /// Extension methods for the <see cref="Image{TColor, TPacked}"/> type.
    /// </summary>
    public static partial class ImageExtensions
    {

        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new FillShapeProcessor<TColor, TPacked>(brush, shape));
        }

        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new FillShapeProcessor<TColor, TPacked>(brush, new SimplePolygon(new LinearLineSegment(points))));
        }


        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(brush, thickness, shape));
        }

        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(brush, thickness, new SimplePolygon(new LinearLineSegment(points))));
        }
    }
}

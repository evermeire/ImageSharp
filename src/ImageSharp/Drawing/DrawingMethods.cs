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
        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(pen, shape));
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided brush at the provided thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(new Pen(brush, thickness), shape));
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided brush at the provided thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(new Pen(brush, thickness), new SimplePolygon(new LinearLineSegment(points))));
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided Pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(pen, new SimplePolygon(new LinearLineSegment(points))));
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided brush at the provided thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(new Pen(brush, thickness), new SimplePolygon(new LinearLineSegment(points))));
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided Pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawShapeProcessor<TColor, TPacked>(pen, new SimplePolygon(new LinearLineSegment(points))));
        }

        /// <summary>
        /// Draws the path with the bursh at the privdied thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawPath<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, IPath path)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen(brush, thickness), path);
        }

        /// <summary>
        /// Draws the path with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawPath<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, IPath path)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(pen, path));
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen(brush, thickness), new Path(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(pen, new Path(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen(brush, thickness), new Path(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(pen, new Path(new LinearLineSegment(points)));
        }








        /// <summary>
        /// Draws the provided Points as an open Bezier path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen(brush, thickness), new Path(new BezierLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Bezier path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(pen, new Path(new BezierLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Bezier path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen(brush, thickness), new Path(new BezierLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Bezier path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IPen pen, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(pen, new Path(new BezierLineSegment(points)));
        }

    }
}

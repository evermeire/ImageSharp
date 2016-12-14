﻿// <copyright file="Draw.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    using Drawing;
    using Drawing.Brushes;
    using Drawing.Paths;
    using Drawing.Pens;
    using Drawing.Processors;
    using Drawing.Shapes;
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(pen, shape));
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(new Pen<TColor, TPacked>(brush, thickness), shape);
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided brush at the provided thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="shape">The shape.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, IShape shape)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(new SolidBrush<TColor, TPacked>(color), thickness, shape);
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(new Pen<TColor, TPacked>(brush, thickness), new Polygon(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided brush at the provided thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(new SolidBrush<TColor, TPacked>(color), thickness, points);
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided Pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(pen, new Polygon(new LinearLineSegment(points)));
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(new Pen<TColor, TPacked>(brush, thickness), new Polygon(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided brush at the provided thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(new SolidBrush<TColor, TPacked>(color), thickness, points);
        }

        /// <summary>
        /// Draws the provided Points as a closed Linear Polygon with the provided Pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPolygon(pen, new Polygon(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the path with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="path">The path.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPath<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, IPath path)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.Process(new DrawPathProcessor<TColor, TPacked>(pen, path));
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPath<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, IPath path)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen<TColor, TPacked>(brush, thickness), path);
        }

        /// <summary>
        /// Draws the path with the bursh at the privdied thickness.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="path">The path.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawPath<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, IPath path)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new SolidBrush<TColor, TPacked>(color), thickness, path);
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen<TColor, TPacked>(brush, thickness), new Path(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawLines(new SolidBrush<TColor, TPacked>(color), thickness, points);
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, PointF[] points)
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen<TColor, TPacked>(brush, thickness), new Path(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawLines(new SolidBrush<TColor, TPacked>(color), thickness, points);
        }

        /// <summary>
        /// Draws the provided Points as an open Linear path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawLines<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, Point[] points)
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen<TColor, TPacked>(brush, thickness), new Path(new BezierLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Bezier path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, PointF[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawBeziers(new SolidBrush<TColor, TPacked>(color), thickness, points);
        }

        /// <summary>
        /// Draws the provided Points as an open Bezier path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, PointF[] points)
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
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(new Pen<TColor, TPacked>(brush, thickness), new Path(new BezierLineSegment(points)));
        }

        /// <summary>
        /// Draws the provided Points as an open Bezier path at the provided thickness with the supplied brush
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, float thickness, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawBeziers(new SolidBrush<TColor, TPacked>(color), thickness, points);
        }

        /// <summary>
        /// Draws the provided Points as an open Bezier path with the supplied pen
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawBeziers<TColor, TPacked>(this Image<TColor, TPacked> source, IPen<TColor, TPacked> pen, Point[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawPath(pen, new Path(new BezierLineSegment(points)));
        }
    }
}

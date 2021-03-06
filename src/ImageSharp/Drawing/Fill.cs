﻿// <copyright file="Fill.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    using System;
    using System.Numerics;
    using Drawing;
    using Drawing.Brushes;
    using Drawing.Paths;
    using Drawing.Processors;
    using Drawing.Shapes;

    /// <summary>
    /// Extension methods for the <see cref="Image{TColor, TPacked}"/> type.
    /// </summary>
    public static partial class ImageExtensions
    {
        /// <summary>
        /// Flood fills the image with the specified brush.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> Fill<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct, IEquatable<TPacked>
        {
            return source.Process(new FillProcessor<TColor, TPacked>(brush));
        }

        /// <summary>
        /// Flood fills the image with the specified color.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> Fill<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct, IEquatable<TPacked>
        {
            return source.Fill(new SolidBrush<TColor, TPacked>(color));
        }

        /// <summary>
        /// Flood fills the image in the shape o fhte provided polygon with the specified brush..
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="options">The graphics options.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> Fill<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, IShape shape, GraphicsOptions options)
          where TColor : struct, IPackedPixel<TPacked>
          where TPacked : struct, IEquatable<TPacked>
        {
            return source.Process(new FillShapeProcessor<TColor, TPacked>(brush, shape, options));
        }

        /// <summary>
        /// Flood fills the image in the shape o fhte provided polygon with the specified brush..
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="shape">The shape.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> Fill<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, IShape shape)
          where TColor : struct, IPackedPixel<TPacked>
          where TPacked : struct, IEquatable<TPacked>
        {
            return source.Process(new FillShapeProcessor<TColor, TPacked>(brush, shape, GraphicsOptions.Default));
        }

        /// <summary>
        /// Flood fills the image in the shape o fhte provided polygon with the specified brush..
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> Fill<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, IShape shape, GraphicsOptions options)
          where TColor : struct, IPackedPixel<TPacked>
          where TPacked : struct, IEquatable<TPacked>
        {
            return source.Fill(new SolidBrush<TColor, TPacked>(color), shape, options);
        }

        /// <summary>
        /// Flood fills the image in the shape o fhte provided polygon with the specified brush..
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="shape">The shape.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> Fill<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, IShape shape)
          where TColor : struct, IPackedPixel<TPacked>
          where TPacked : struct, IEquatable<TPacked>
        {
            return source.Fill(new SolidBrush<TColor, TPacked>(color), shape);
        }

        /// <summary>
        /// Flood fills the image in the shape of a Linear polygon described by the points
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, Vector2[] points, GraphicsOptions options)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            // using Polygon directly instead of LinearPolygon as its will have less indirection
            return source.Fill(brush, new Polygon(new LinearLineSegment(points)), options);
        }

        /// <summary>
        /// Flood fills the image in the shape of a Linear polygon described by the points
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, IBrush<TColor, TPacked> brush, Vector2[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            // using Polygon directly instead of LinearPolygon as its will have less indirection
            return source.Fill(brush, new Polygon(new LinearLineSegment(points)));
        }

        /// <summary>
        /// Flood fills the image in the shape of a Linear polygon described by the points
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="points">The points.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, Vector2[] points, GraphicsOptions options)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            // using Polygon directly instead of LinearPolygon as its will have less indirection
            return source.Fill(new SolidBrush<TColor, TPacked>(color), new Polygon(new LinearLineSegment(points)), options);
        }

        /// <summary>
        /// Flood fills the image in the shape of a Linear polygon described by the points
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="color">The color.</param>
        /// <param name="points">The points.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> FillPolygon<TColor, TPacked>(this Image<TColor, TPacked> source, TColor color, Vector2[] points)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            // using Polygon directly instead of LinearPolygon as its will have less indirection
            return source.Fill(new SolidBrush<TColor, TPacked>(color), new Polygon(new LinearLineSegment(points)));
        }
    }
}

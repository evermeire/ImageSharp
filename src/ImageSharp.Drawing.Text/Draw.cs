﻿// <copyright file="Draw.cs" company="James Jackson-South">
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
    using Drawing.Pens;
    using Drawing.Processors;
    using Drawing.Shapes;

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
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="options">The options.</param>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Font font, IBrush<TColor, TPacked> brush, IPen<TColor, TPacked> pen, GraphicsOptions options, Vector2 position)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            return source.Process(new TextDrawingProcessor<TColor, TPacked>(text, pen, brush, font.Typeface, font.Size, (int)Math.Round(source.VerticalResolution), position, options));
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Font font, IBrush<TColor, TPacked> brush, Vector2 position)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            return source.DrawString(text, font, brush, null, GraphicsOptions.Default, position);
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="options">The options.</param>
        /// <param name="position">The position.</param>
        /// <returns>The Image</returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Font font, IBrush<TColor, TPacked> brush, GraphicsOptions options, Vector2 position)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            return source.DrawString(text, font, brush, null, options, position);
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Font font, TColor color, Vector2 position)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            return source.DrawString(text, font, new SolidBrush<TColor,TPacked>(color), null, GraphicsOptions.Default, position);
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        /// <param name="options">The options.</param>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Font font, TColor color, GraphicsOptions options, Vector2 position)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            return source.DrawString(text, font, new SolidBrush<TColor, TPacked>(color), null, options, position);
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Font font, IPen<TColor, TPacked> pen, Vector2 position)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            return source.DrawString(text, font, null, pen, GraphicsOptions.Default, position);
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="options">The options.</param>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Font font, IPen<TColor, TPacked> pen, GraphicsOptions options, Vector2 position)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct, IEquatable<TPacked>
        {
            return source.DrawString(text, font, null, pen, options, position);
        }
    }
}
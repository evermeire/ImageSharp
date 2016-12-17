// <copyright file="Draw.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
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
        /// <param name="position">The position.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Vector2 position, Font font, IBrush<TColor, TPacked> brush)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            var shapes = font.GenerateContours(text);

            source.Process(new FillShapeProcessor<TColor, TPacked>(brush, shapes, position, GraphicsOptions.Default));

            return source;
        }

        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="position">The position.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Vector2 position, Font font, TColor color)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            return source.DrawString(text, position, font, new SolidBrush<TColor, TPacked>(color));
        }


        /// <summary>
        /// Draws the outline of the polygon with the provided pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <typeparam name="TPacked">The type of the packed.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="position">The position.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <returns>
        /// The Image
        /// </returns>
        public static Image<TColor, TPacked> DrawString<TColor, TPacked>(this Image<TColor, TPacked> source, string text, Vector2 position, Font font, IPen<TColor, TPacked> pen)
           where TColor : struct, IPackedPixel<TPacked>
           where TPacked : struct
        {
            var shapes = font.GenerateContours(text);

            source.Process(new DrawPathProcessor<TColor, TPacked>(pen, shapes, position, GraphicsOptions.Default));

            return source;
        }
    }
}

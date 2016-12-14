﻿// <copyright file="Pens.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Pens
{
    /// <summary>
    /// Common Pen styles
    /// </summary>
    public partial class Pens
    {

        /// <summary>
        /// Create a solid pen with out any drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen Solid(Color color, float width)
            => new Pen(color, width);


        /// <summary>
        /// Create a solid pen with out any drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen Solid(IBrush<Color, uint> brush, float width)
            => new Pen(brush, width);

        /// <summary>
        /// Create a pen with a 'Dash' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen Dash(Color color, float width)
            => new Pen(Pens<Color, uint>.Dash(color, width));

        /// <summary>
        /// Create a pen with a 'Dash' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen Dash(IBrush<Color, uint> brush, float width)
            => new Pen(Pens<Color, uint>.Dash(brush, width));


        /// <summary>
        /// Create a pen with a 'Dot' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen Dot(Color color, float width)
            => new Pen(Pens<Color, uint>.Dot(color, width));

        /// <summary>
        /// Create a pen with a 'Dot' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen Dot(IBrush<Color, uint> brush, float width)
            => new Pen(Pens<Color, uint>.Dot(brush, width));


        /// <summary>
        /// Create a pen with a 'Dash Dot' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen DashDot(Color color, float width)
            => new Pen(Pens<Color, uint>.DashDot(color, width));

        /// <summary>
        /// Create a pen with a 'Dash Dot' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen DashDot(IBrush<Color, uint> brush, float width)
            => new Pen(Pens<Color, uint>.DashDot(brush, width));

        /// <summary>
        /// Create a pen with a 'Dash Dot Dot' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen DashDotDot(Color color, float width)
            => new Pen(Pens<Color, uint>.DashDotDot(color, width));


        /// <summary>
        /// Create a pen with a 'Dash Dot Dot' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen DashDotDot(IBrush<Color, uint> brush, float width)
            => new Pen(Pens<Color, uint>.DashDotDot(brush, width));
    }

    /// <summary>    /// 
    /// Common Pen styles
    /// </summary>
    /// <typeparam name="TColor">The type of the color.</typeparam>
    /// <typeparam name="TPacked">The type of the packed.</typeparam>
    public partial class Pens<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        internal static readonly float[] dashDotPattern = new[] { 3f, 1f, 1f, 1f };
        internal static readonly float[] dashDotDotPattern = new[] { 3f, 1f, 1f, 1f, 1f, 1f };
        internal static readonly float[] dottedPattern = new[] { 1f, 1f };
        internal static readonly float[] dashedPattern = new[] { 3f, 1f };

        /// <summary>
        /// Create a solid pen with out any drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> Solid(TColor color, float width)
            => new Pen<TColor, TPacked>(color, width);

        /// <summary>
        /// Create a solid pen with out any drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        public static Pen<TColor, TPacked> Solid(IBrush<TColor, TPacked> brush, float width)
            => new Pen<TColor, TPacked>(brush, width);


        /// <summary>
        /// Create a pen with a 'Dash' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> Dash(TColor color, float width)
            => new Pen<TColor, TPacked>(color, width, dashedPattern);

        /// <summary>
        /// Create a pen with a 'Dash' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> Dash(IBrush<TColor, TPacked> brush, float width)
            => new Pen<TColor, TPacked>(brush, width, dashedPattern);


        /// <summary>
        /// Create a pen with a 'Dot' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> Dot(TColor color, float width)
            => new Pen<TColor, TPacked>(color, width, dottedPattern);

        /// <summary>
        /// Create a pen with a 'Dot' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> Dot(IBrush<TColor, TPacked> brush, float width)
            => new Pen<TColor, TPacked>(brush, width, dottedPattern);



        /// <summary>
        /// Create a pen with a 'Dash Dot' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> DashDot(TColor color, float width)
            => new Pen<TColor, TPacked>(color, width, dashDotPattern);

        /// <summary>
        /// Create a pen with a 'Dash Dot' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> DashDot(IBrush<TColor, TPacked> brush, float width)
            => new Pen<TColor, TPacked>(brush, width, dashDotPattern);


        /// <summary>
        /// Create a pen with a 'Dash Dot Dot' drawing patterns
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> DashDotDot(TColor color, float width)
            => new Pen<TColor, TPacked>(color, width, dashDotDotPattern);

        /// <summary>
        /// Create a pen with a 'Dash Dot Dot' drawing patterns
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static Pen<TColor, TPacked> DashDotDot(IBrush<TColor, TPacked> brush, float width)
            => new Pen<TColor, TPacked>(brush, width, dashDotDotPattern);
    }
}

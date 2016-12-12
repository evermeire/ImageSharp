// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processing
{
    using System;

    /// <summary>
    /// interface preresenting a brush
    /// </summary>
    public interface IBrushApplicator<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// Gets a value indicating whether the brush will return colors that may 
        /// or maynet require merging with background.
        /// </summary>
        /// <remarks>
        ///     Will it ever return a color that it not 100% opaque.
        /// </remarks>
        /// <value>
        ///   <c>true</c> if [requires composition]; otherwise, <c>false</c>.
        /// </value>
        bool RequiresComposition { get; }

        /// <summary>
        /// Gets the colors for a block of pixels.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endX">The end x.</param>
        /// <param name="endY">The end y.</param>
        /// <returns></returns>
        TColor[,] GetColor(int startX, int startY, int endX, int endY);

        /// <summary>
        /// Gets the colors for a block of pixels.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <param name="endX">The end x.</param>
        /// <param name="Y">The Y coordinate to get the line of colors for.</param>
        /// <returns></returns>
        TColor[] GetColor(int startX, int endX, int Y);

        /// <summary>
        /// Gets the color for a single pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        TColor GetColor(int x, int y);
    }
}

﻿// <copyright file="ShapeProcessorBases.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using Drawing;
    using ImageSharp.Processors;

    internal abstract class ShapeProcessorBase<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        private const float Epsilon = 0.001f;

        private readonly IBrush fillColor;
        private readonly IShape poly;
        private readonly TargetLayer layer;

        public ShapeProcessorBase(IBrush brush, IShape shape)
        {
            this.poly = shape;
            this.fillColor = brush;
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
        }

        protected abstract float Opacity(float distance);

        protected abstract int DrawPadding { get; }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            var rect = RectangleF.Ceiling(poly.Bounds); //rounds the points out away from the center

            int polyStartY = rect.Y - DrawPadding;
            int polyEndY = rect.Bottom + DrawPadding;
            int startX = rect.X - DrawPadding;
            int endX = rect.Right + DrawPadding;

            // Align start/end positions.
            int minX = Math.Max(0, startX);
            int maxX = Math.Min(source.Width, endX);
            int minY = Math.Max(0, polyStartY);
            int maxY = Math.Min(source.Height, polyEndY);

            // Reset offset if necessary.
            if (minX > 0)
            {
                startX = 0;
            }

            if (minY > 0)
            {
                polyStartY = 0;
            }

            //calculate 
            var applicator = fillColor.CreateApplicator(rect);
            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            {
                Parallel.For(
                minY,
                maxY,
                this.ParallelOptions,
                y =>
                {
                    int offsetY = y - polyStartY;
                    for (int x = minX; x < maxX; x++)
                    {
                        int offsetX = x - startX;

                        var dist = poly.Distance(offsetX, offsetY);
                        var opacity = Opacity(dist);

                        if (opacity > 0)
                        {
                            var color = applicator.GetColor(offsetX, offsetY).ToVector4();

                            Vector4 backgroundColor = sourcePixels[offsetX, offsetY].ToVector4();

                            float a = color.W * opacity;

                            if (Math.Abs(a) < Epsilon)
                            {
                                color = backgroundColor;
                            }
                            else if (a < 1 && a > 0)
                            {
                                color.W = 1;
                                color = Vector4.Lerp(backgroundColor, color, a);
                            }

                            TColor packed = default(TColor);
                            packed.PackFromVector4(color);
                            sourcePixels[offsetX, offsetY] = packed;
                        }
                    }
                });
            }
        }
    }
}
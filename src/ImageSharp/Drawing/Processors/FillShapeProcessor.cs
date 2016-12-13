// <copyright file="ShapeProcessorBases.cs" company="James Jackson-South">
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

    public class FillShapeProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        private const float Epsilon = 0.001f;

        const float antialiasFactor = 1f;
        private const int drawPadding = 1;
        private readonly IBrush<TColor, TPacked> fillColor;
        private readonly IShape poly;

        public FillShapeProcessor(IBrush<TColor, TPacked> brush, IShape shape)
        {
            this.poly = shape;
            this.fillColor = brush;
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
        }

        protected float Opacity(float distance)
        {
            if (distance <= 0)
            {
                return 1;
            }
            else if (distance < antialiasFactor)
            {
                return 1 - (distance / antialiasFactor);
            }
            return 0;
        }

        protected override void OnApply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle)
        {
            var rect = RectangleF.Ceiling(poly.Bounds); //rounds the points out away from the center

            int polyStartY = rect.Y - drawPadding;
            int polyEndY = rect.Bottom + drawPadding;
            int startX = rect.X - drawPadding;
            int endX = rect.Right + drawPadding;

            int minX = Math.Max(sourceRectangle.Left, startX);
            int maxX = Math.Min(sourceRectangle.Right, endX);
            int minY = Math.Max(sourceRectangle.Top, polyStartY);
            int maxY = Math.Min(sourceRectangle.Bottom, polyEndY);

            // Align start/end positions.
            minX = Math.Max(0, minX);
            maxX = Math.Min(source.Width, maxX);
            minY = Math.Max(0, minY);
            maxY = Math.Min(source.Height, maxY);

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
                    
                    Vector2 currentPoint = new Vector2();
                    for (int x = minX; x < maxX; x++)
                    {
                        int offsetX = x - startX;
                        currentPoint.X = offsetX;
                        currentPoint.Y = offsetY;

                        var dist = poly.Distance(offsetX, offsetY);
                        var opacity = Opacity(dist);

                        if (opacity > Epsilon)
                        {
                            int offsetColorX = x - minX;

                            Vector4 backgroundVector = sourcePixels[offsetX, offsetY].ToVector4();
                            Vector4 sourceVector = applicator.GetColor(currentPoint).ToVector4();

                            var finalColor = Vector4BlendTransforms.PremultipliedLerp(backgroundVector, sourceVector, opacity);
                            finalColor.W = backgroundVector.W;

                            TColor packed = default(TColor);
                            packed.PackFromVector4(finalColor);
                            sourcePixels[offsetX, offsetY] = packed;
                        }
                    }
                });
            }
        }
    }
}
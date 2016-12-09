// <copyright file="DrawProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using Brushes;
    using Shapes;


    public class FillShapeProcessor<TColor, TPacked> : ShapeProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        private readonly int drawPadding;
        public FillShapeProcessor(IBrush brush, IShape shape) : base(brush, shape)
        {
            drawPadding = (int)Math.Ceiling(antialiasFactor);
        }
        protected override int DrawPadding => drawPadding;

        const float antialiasFactor = 0.75f;
        protected override float Opacity(float distance)
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
    }


    public class DrawShapeProcessor<TColor, TPacked> : ShapeProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        public DrawShapeProcessor(IBrush brush, float thickness, IShape shape) : base(brush, shape)
        {
            halfThickness = thickness / 2;
            drawPadding = (int)Math.Ceiling(halfThickness + antialiasFactor);
        }

        protected override int DrawPadding => drawPadding;

        const float antialiasFactor = 0.75f;
        private readonly float halfThickness;
        private readonly int drawPadding;

        protected override float Opacity(float distance)
        {
            if (distance < 0)
            {
                distance = distance * -1;
            }

            if (distance <= halfThickness)
            {
                //inside band draw full thickness
                return 1;
            }

            //offset by half distance to get new distance
            if(distance > halfThickness)
            {
                distance = distance - halfThickness;
            }

            if (distance < antialiasFactor)
            {
                return 1 - (distance / antialiasFactor);
            }
            return 0;
        }

    }

    /// <summary>
    /// Combines two images together by blending the pixels.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public abstract class ShapeProcessor<TColor, TPacked> : ImageFilter<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {

        private readonly IBrush fillColor;

        /// <summary>
        /// The shape to apply.
        /// </summary>
        private readonly IShape poly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeProcessor{T,TP}"/> class.
        /// </summary>
        /// <param name="brush">
        /// The brush to apply to currently processing image.
        /// </param>
        public ShapeProcessor(IBrush brush, IShape shape)
        {
            this.poly = shape;
            this.fillColor = brush;

        }

        /// <summary>
        /// Calculates the opactiy basedon the distance the point is from the closest line.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        protected abstract float Opacity(float distance);

        protected abstract int DrawPadding { get; }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            var rect = poly.Bounds;
            // if/when we support antiailiasing we might/will need to expand the bound to account
            
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

            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            {
                using (var applicator = fillColor.CreateApplicator(rect))
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

                            // lets Calculate Distance From Edge

                            if(offsetX == 7 &&  offsetY == 10)
                            {
                                var t = "";
                            }
                            var dist = poly.Distance(offsetX, offsetY);

                            var opacity = Opacity(dist);

                            if (opacity > 0)
                            {
                                var pixelColor = applicator.GetColor(offsetX, offsetY);
                                Vector4 pixelColorVector = pixelColor.ToVector4();

                                if (pixelColor.A < 255)
                                {

                                    opacity = opacity * (pixelColor.A / 255f);
                                }

                                if (opacity < 1)
                                {
                                    Vector4 currentColor = sourcePixels[offsetX, offsetY].ToVector4();
                                    pixelColorVector = Vector4.Lerp(currentColor, pixelColorVector, opacity);
                                }

                                TColor packed = default(TColor);
                                packed.PackFromVector4(pixelColorVector);
                                sourcePixels[offsetX, offsetY] = packed;
                            }
                        }
                    });
                }
            }
        }


    }
}
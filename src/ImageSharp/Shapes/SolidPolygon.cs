// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Shapes
{
    using Brushes;
    using Polygons;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public class SolidPolygon : IVectorGraphic
    {
        public ParallelOptions ParallelOptions { get; set; } = Bootstrapper.Instance.ParallelOptions;

        const float antialiasFactor = 0.75f;

        private readonly IBrush fillColor;

        private Lazy<Rectangle> bounds;
        private ComplexPolygon poly;

        public SolidPolygon(IBrush fillColor, ILineSegment segment) : this(fillColor, new[] { segment })
        {
        }

        public SolidPolygon(IBrush fillColor, IEnumerable<ILineSegment> segments)
            :this(fillColor, new ComplexPolygon(segments))
        {
        }
        
        internal SolidPolygon(IBrush fillColor, ComplexPolygon poly)
        {
            this.poly = poly;
            this.fillColor = fillColor;
        }

        public void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            var sourceRectangle = poly.Bounds;
            // if/when we support antiailiasing we might/will need to expand the bound to account

            int startY = sourceRectangle.Y - 2;
            int endY = sourceRectangle.Bottom + 2;
            int startX = sourceRectangle.X - 2;
            int endX = sourceRectangle.Right + 2;

            // Align start/end positions.
            int minX = Math.Max(0, startX);
            int maxX = Math.Min(source.Width, endX);
            int minY = Math.Max(0, startY);
            int maxY = Math.Min(source.Height, endY);

            // Reset offset if necessary.
            if (minX > 0)
            {
                startX = 0;
            }

            if (minY > 0)
            {
                startY = 0;
            }

            //calculate 

            using (PixelAccessor<TColor, TPacked> sourcePixels = source.Lock())
            {
                using (var applicator = fillColor.CreateApplicator(sourceRectangle))
                {
                    Parallel.For(
                    minY,
                    maxY,
                    this.ParallelOptions,
                    y =>
                    {
                        int offsetY = y - startY;
                        for (int x = minX; x < maxX; x++)
                        {
                            int offsetX = x - startX;

                            // lets Calculate Distance From Edge

                            var dist = poly.Distance(offsetX, offsetY);

                            if (dist <= antialiasFactor)
                            {
                                var pixelColor = applicator.GetColor(offsetX, offsetY);
                                Vector4 pixelColorVector = pixelColor.ToVector4();
                                float opacity = 1;

                                if (pixelColor.A < 255)
                                {

                                    opacity = pixelColor.A / 255f;
                                }

                                if (dist != 0)
                                {
                                    var alpha = 1 - (dist / antialiasFactor);

                                    opacity = opacity * alpha;

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

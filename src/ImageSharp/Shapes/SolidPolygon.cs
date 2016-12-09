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

                            const float antialiasFactor = 0.75f;

                            if (dist <= antialiasFactor)
                            {
                                var packed = fillColor.GetColor(sourcePixels, offsetX, offsetY);
                                if (dist == 0)
                                {
                                    //inside mask full color
                                    sourcePixels[offsetX, offsetY] = packed;
                                }
                                else
                                {
                                    var alpha = 1- (dist / antialiasFactor);

                                    Vector4 color = sourcePixels[offsetX, offsetY].ToVector4();
                                    Vector4 backgroundColor = packed.ToVector4();
                                    color = Vector4.Lerp(color, backgroundColor, alpha > 0 ? alpha : backgroundColor.W);

                                    TColor newPacked = default(TColor);
                                    newPacked.PackFromVector4(color);
                                    sourcePixels[offsetX, offsetY] = newPacked;
                                }
                            }
                        }
                    });
            }
        }
    }
}

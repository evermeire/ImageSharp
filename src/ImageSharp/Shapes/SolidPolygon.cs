// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Shapes
{
    using Brushes;
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
        private SimplePolygon outline;
        private IEnumerable<SimplePolygon> holes;

        public SolidPolygon(IBrush fillColor, ILineSegment segment) : this(fillColor, new[] { segment })
        {
        }

        public SolidPolygon(IBrush fillColor, IEnumerable<ILineSegment> segments)
            :this(fillColor, new SimplePolygon(segments))
        {
        }

        internal SolidPolygon(IBrush fillColor, SimplePolygon outline,params SimplePolygon[] holes) 
            : this(fillColor, outline, (IEnumerable<SimplePolygon>) holes)
        {
            this.outline = outline;
            this.fillColor = fillColor;
        }

        internal SolidPolygon(IBrush fillColor, SimplePolygon outline, IEnumerable<SimplePolygon> holes)
        {
            this.outline = outline;
            this.holes = holes ?? Enumerable.Empty<SimplePolygon>();
            this.fillColor = fillColor;
        }

        public void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            var sourceRectangle = outline.Bounds;
            // if/when we support antiailiasing we might/will need to expand the bound to account

            int startY = sourceRectangle.Y;
            int endY = sourceRectangle.Bottom;
            int startX = sourceRectangle.X;
            int endX = sourceRectangle.Right;

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
                            
                            var dist = outline.Distance(offsetX, offsetY, false);

                            if(dist == 0)
                            {
                                foreach(var hole in holes)
                                {
                                    var distFromHole = hole.Distance(offsetX, offsetY, true);

                                    if(distFromHole != 0)
                                    {
                                        //we are in the hole 
                                        dist = distFromHole;
                                        break;
                                    }
                                }
                            }

                            if (dist < 1)
                            {
                                var packed = fillColor.GetColor(sourcePixels, offsetX, offsetY);
                                if (dist == 0)
                                {
                                    //inside mask full color
                                    sourcePixels[offsetX, offsetY] = packed;
                                }
                                else
                                {
                                    Vector4 color = sourcePixels[offsetX, offsetY].ToVector4();

                                    Vector4 backgroundColor = packed.ToVector4();
                                    var alpha = (1 - dist);

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

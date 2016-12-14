// <copyright file="DrawProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;
    using Drawing;
    using ImageSharp.Processors;
    using Paths;
    using Shapes;
    using Pens;

    internal class DrawPathProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        private const float antialiasFactor = 1f;
        private const int paddingFactor = 1;//needs to been the same or greater than antialiasFactor
        private const float Epsilon = 0.001f;

        private readonly IPen<TColor, TPacked> pen;
        private readonly IPath[] paths;
        private readonly RectangleF region;

        public DrawPathProcessor(IPen<TColor, TPacked> pen, IShape shape)
            :this(pen, shape.ToArray())
        { }

        public DrawPathProcessor(IPen<TColor, TPacked> pen, params IPath[] paths)
        {
            this.paths = paths;
            this.pen = pen;
            this.ParallelOptions.MaxDegreeOfParallelism = 1;

            if (paths.Length != 1)
            {
                var maxX = paths.Max(x => x.Bounds.Right);
                var minX = paths.Min(x => x.Bounds.Left);
                var maxY = paths.Max(x => x.Bounds.Bottom);
                var minY = paths.Min(x => x.Bounds.Top);

                region = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }else
            {
                region = paths[0].Bounds;
            }
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
            var applicator = pen.CreateApplicator(region);
            var rect = RectangleF.Ceiling(applicator.RequiredRegion);

            int polyStartY = rect.Y - paddingFactor;
            int polyEndY = rect.Bottom + paddingFactor;
            int startX = rect.X - paddingFactor;
            int endX = rect.Right + paddingFactor;

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

                        var dist = paths.Select(p => p.Distance(offsetX, offsetY)).OrderBy(p => p.DistanceFromPath).First();
                     
                        var color = applicator.GetColor(dist);
                        
                        var opacity = Opacity(color.DistanceFromElement);

                        if (opacity > Epsilon)
                        {
                            int offsetColorX = x - minX;

                            Vector4 backgroundVector = sourcePixels[offsetX, offsetY].ToVector4();
                            Vector4 sourceVector = color.Color.ToVector4();

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
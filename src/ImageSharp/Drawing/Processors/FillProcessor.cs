// <copyright file="FillProcessor.cs" company="James Jackson-South">
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

    /// <summary>
    /// Using the bursh as a source of pixels colors blends the brush color with source.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class FillProcessor<TColor, TPacked> : ImageFilteringProcessor<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        private const float Epsilon = 0.001f;

        private readonly IBrush brush;

        private readonly TargetLayer sourceLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FillProcessor{TColor, TPacked}"/> class.
        /// </summary>
        /// <param name="brush">The brush to source pixel colors from.</param>
        /// <param name="sourceLayer">
        /// The layer to treat the source as. 
        /// If <see cref="TargetLayer.Foreground"/> then take the <paramref name="brush"/> 
        /// and apply it under the source image, else apply the <paramref name="brush"/> on top 
        /// of the source image
        /// </param>
        public FillProcessor(IBrush brush, TargetLayer sourceLayer = TargetLayer.Background)
        {
            this.sourceLayer = sourceLayer;
            this.brush = brush;
        }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
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
            
            // create a pixcel applicator (this will be important when there is a gradient or image brush)
            var applicator = brush.CreateApplicator(sourceRectangle);

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

                            var color = applicator.GetColor(offsetX, offsetY).ToVector4();
                            Vector4 backgroundColor = sourcePixels[offsetX, offsetY].ToVector4();

                            //we want the brush color under the source image, flip the color order
                            if (this.sourceLayer == TargetLayer.Foreground)
                            {
                                var tmp = color;
                                color = backgroundColor;
                                backgroundColor = tmp;
                            }

                            // based on the alpha of the foreground color shift the background color towards the forgound by the opactiy level
                            // need to consider what opactiy should be like when doing this???
                            // this logic seems to be working for now
                            float a = color.W;

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
                    });
            }
        }
    }
}
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

    /// <summary>
    /// Combines two images together by blending the pixels.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class BrushProcessor<TColor, TPacked> : ImageFilter<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// The brush to apply.
        /// </summary>
        private readonly IBrush brush;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrushProcessor{T,TP}"/> class.
        /// </summary>
        /// <param name="brush">
        /// The brush to apply to currently processing image.
        /// </param>
        public BrushProcessor(IBrush brush)
        {
            this.brush = brush;

            // Don't Parallelize processing
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
        }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            brush.Apply(source, new NoOpMask(sourceRectangle));
        }

        private class NoOpMask : IMask
        {
            public NoOpMask(Rectangle bounds)
            {
                Bounds = bounds;
            }

            public Rectangle Bounds
            {
                get;
            }

            public float Distance(int x, int y)
            {
                return 0;
            }
        }
    }
}
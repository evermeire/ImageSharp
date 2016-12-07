// <copyright file="DrawProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    using Shapes;

    /// <summary>
    /// Combines two images together by blending the pixels.
    /// </summary>
    /// <typeparam name="TColor">The pixel format.</typeparam>
    /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
    public class ShapeProcessor<TColor, TPacked> : ImageFilter<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        /// <summary>
        /// The shape to apply.
        /// </summary>
        private readonly IVectorGraphic shape;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeProcessor{T,TP}"/> class.
        /// </summary>
        /// <param name="brush">
        /// The brush to apply to currently processing image.
        /// </param>
        public ShapeProcessor(IVectorGraphic shape)
        {
            this.shape = shape;

            // Don't Parallelize processing
            this.ParallelOptions.MaxDegreeOfParallelism = 1;
        }

        /// <inheritdoc/>
        protected override void Apply(ImageBase<TColor, TPacked> source, Rectangle sourceRectangle, int startY, int endY)
        {
            shape.Apply(source);
        }
    }
}
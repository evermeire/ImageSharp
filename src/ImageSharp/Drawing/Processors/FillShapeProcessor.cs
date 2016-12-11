// <copyright file="FillShapeProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using Drawing;

    internal class FillShapeProcessor<TColor, TPacked> : ShapeProcessorBase<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        const float antialiasFactor = 1f;
        private readonly int drawPadding;

        public FillShapeProcessor(IBrush brush, IShape shape) : base(brush, shape)
        {
            drawPadding = (int)Math.Ceiling(antialiasFactor);
        }

        protected override int DrawPadding => drawPadding;

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
}
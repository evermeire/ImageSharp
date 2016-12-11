// <copyright file="DrawProcessor.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Processors
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using Drawing;
    
    internal class DrawShapeProcessor<TColor, TPacked> : ShapeProcessorBase<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        public DrawShapeProcessor(IPen pen, IShape shape) : base(pen.Brush, shape)
        {
            halfThickness = pen.Width / 2;
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
            
            //convert distance to opacity for anti-aliasing
            if (distance < antialiasFactor)
            {
                return 1 - (distance / antialiasFactor);
            }
            return 0;
        }
    }    
}
// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushes
{
    using Shapes;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// A brush representing a Solid color fill
    /// </summary>
    /// <seealso cref="ImageSharp.Brushs.IBrush" />
    public class SolidBrush: IBrush
    {
        private readonly Color color;
        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SolidBrush(Color color)
        {
            this.color = color;
        }

        public class SolidBrushApplicator : IBrushApplicator
        {
            private Color color;

            public SolidBrushApplicator(Color color)
            {
                this.color = color;
            }

            public Color GetColor(int x, int y)
            {
                return color;
            }
            public void Dispose()
            {
                //noop
            }
        }

        public IBrushApplicator CreateApplicator(Rectangle region)
        {
            
                return new SolidBrushApplicator(color);
        }
    }
}

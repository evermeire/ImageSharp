// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushs
{

    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    public struct ColorStop
    {

        private float stop;

        /// <summary>
        /// The color
        /// </summary>
        public Color Color;

        /// <summary>
        /// The stop postion as a value between 0 and 1
        /// </summary>
        public float Stop
        {
            get { return stop; }
            set
            {
                Guard.MustBeBetweenOrEqualTo(value, 0, 0, nameof(Stop));
                stop = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorStop"/> struct.
        /// </summary>
        /// <param name="stop">The stop.</param>
        /// <param name="color">The color.</param>
        public ColorStop(float stop, Color color)
        {
            Color = color;

            Guard.MustBeBetweenOrEqualTo(stop, 0, 0, nameof(stop));
            this.stop = stop;
        }
    }
}

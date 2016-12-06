// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushs
{

    using System;
    using System.Numerics;
    using System.Threading.Tasks;

    public struct SimpleLineSegment
    {
        public Point Start;

        public Point End;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLineSegment"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public SimpleLineSegment(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }
}

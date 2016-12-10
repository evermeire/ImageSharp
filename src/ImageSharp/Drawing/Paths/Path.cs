// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Paths
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public class Path : IPath
    {
        private readonly InternalPath innerPath;

        internal Path(params ILineSegment[] segment)
        {
            innerPath = new InternalPath(segment);
        }

        public RectangleF Bounds => innerPath.Bounds;

        float IPath.Distance(int x, int y)
        {
            return innerPath.DistanceFromPath(new Vector2(x, y), false);
        }
    }
}

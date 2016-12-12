// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing
{
    using Processing;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;

    public partial class PatternBrush
    {
        public static PatternBrush Percent10(Color foreColor)
        {
            return Percent10(foreColor, Color.Transparent);
        }
        public static PatternBrush Percent10(Color foreColor, Color backColor)
        {
            return new PatternBrush(foreColor, backColor, new bool[,] {
                { true , false, false, false},
                { false, false, false, false},
                { false, false, true , false},
                { false, false, false, false}
            });
        }

        public static PatternBrush Percent20(Color foreColor)
        {
            return Percent20(foreColor, Color.Transparent);
        }
        public static PatternBrush Percent20(Color foreColor, Color backColor)
        {
            return new PatternBrush(foreColor, backColor, new bool[,] {
                { true , false, false, false},
                { false, false, true , false},
                { true, false, false, false},
                { false, false, true , false}
            });
        }
    }    
}

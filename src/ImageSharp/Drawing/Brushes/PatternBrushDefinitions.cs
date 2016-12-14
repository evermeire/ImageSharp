// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Brushes
{
    using Processing;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;

    public partial class PatternBrush
    {
        // note 2d arrays when configured using initalizer look inverted
        // ---> Y axis
        // ^
        // | X - axis
        // |
        // 
        //

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
                { true , false, true , false},
                { false, false, false, false},
                { false, true , false, true},
                { false, false, false, false}
            });
        }


        public static PatternBrush Horizontal(Color foreColor)
        {
            return Horizontal(foreColor, Color.Transparent);
        }
        public static PatternBrush Horizontal(Color foreColor, Color backColor)
        {
            return new PatternBrush(foreColor, backColor, new bool[,] {
                { false, true, false, false},
                { false, true, false, false},
                { false, true, false, false},
                { false, true, false, false}
            });
        }



        public static PatternBrush Min(Color foreColor)
        {
            return Min(foreColor, Color.Transparent);
        }
        public static PatternBrush Min(Color foreColor, Color backColor)
        {
            return new PatternBrush(foreColor, backColor, new bool[,] {
                { false, false, false, true},
                { false, false, false, true},
                { false, false, false, true},
                { false, false, false, true}
            });
        }


        public static PatternBrush Vertical(Color foreColor)
        {
            return Vertical(foreColor, Color.Transparent);
        }

        public static PatternBrush Vertical(Color foreColor, Color backColor)
        {
            return new PatternBrush(foreColor, backColor, new bool[,] {
                { false, false, false, false},
                { true , true , true , true },
                { false, false, false, false},
                { false, false, false, false}
            });
        }

        public static PatternBrush ForwardDiagnal(Color foreColor)
        {
            return ForwardDiagnal(foreColor, Color.Transparent);
        }

        public static PatternBrush ForwardDiagnal(Color foreColor, Color backColor)
        {
            return new PatternBrush(foreColor, backColor, new bool[,] {
                { true, false, false, false},
                { false, true , false , false },
                { false, false, true, false},
                { false, false, false, true}
            });
        }

        public static PatternBrush BackwardDiagnal(Color foreColor)
        {
            return BackwardDiagnal(foreColor, Color.Transparent);
        }

        public static PatternBrush BackwardDiagnal(Color foreColor, Color backColor)
        {
            return new PatternBrush(foreColor, backColor, new bool[,] {
                { false, false, false, true},
                { false, false, true , false},
                { false, true , false, false},
                { true,  false, false, false}
            });
        }
    }    
}

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

    public partial class PatternBrush : PatternBrush<Color, uint>
    {
        public PatternBrush(Color foreColor, Color backColor, bool[,] pattern)
            : base(foreColor, backColor, pattern)
        {

        }
    }

    public class PatternBrush<TColor, TPacked> : IBrush<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        private readonly TColor foreColor;
        private readonly TColor backColor;
        private readonly bool[,] pattern;


        public PatternBrush(TColor foreColor, TColor backColor, bool[,] pattern)
        {
            this.foreColor = foreColor;
            this.backColor = backColor;
            this.pattern = pattern;
        }

        private class PatternBrushApplicator : BrushApplicatorBase<TColor, TPacked>
        {
            private readonly int xLength;
            private readonly int yLength;
            private readonly bool[,] pattern;
            private readonly TColor backColor = default(TColor);
            private readonly TColor foreColor = default(TColor);

            public PatternBrushApplicator(TColor foreColor, TColor backColor, bool[,] pattern)
            {
                this.pattern = pattern;

                this.xLength = pattern.GetLength(0);
                this.yLength = pattern.GetLength(1);
            }

            public override bool RequiresComposition
            {
                get
                {
                    return true;
                }
            }

            public override TColor GetColor(int x, int y)
            {
                x = x % xLength;
                y = y % yLength;

                if (pattern[x, y])
                {
                    return foreColor;
                }
                else
                {
                    return backColor;
                }
            }
        }

        public IBrushApplicator<TColor, TPacked> CreateApplicator(RectangleF region)
        {
            return new PatternBrushApplicator(this.foreColor, this.backColor, this.pattern);
        }
    }
}
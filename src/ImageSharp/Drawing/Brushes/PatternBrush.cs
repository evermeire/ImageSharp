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
        
    public partial class PatternBrush : IBrush
    {
        private readonly Color foreColor;
        private readonly Color backColor;
        private readonly bool[,] pattern;


        public PatternBrush(Color foreColor, Color backColor, bool[,] pattern)
        {
            this.foreColor = foreColor;
            this.backColor = backColor;
            this.pattern = pattern;
        }

        private class PatternBrushApplicator<TColor, TPacked> : BrushApplicatorBase<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            private readonly int xLength;
            private readonly int yLength;
            private readonly bool hasOpacitySet = false;
            private readonly bool[,] pattern;
            private readonly TColor backColor = default(TColor);
            private readonly TColor foreColor = default(TColor);

            public PatternBrushApplicator(Vector4 foreColor, Vector4 backColor, bool[,] pattern)
            {
                this.foreColor.PackFromVector4(foreColor);
                this.backColor.PackFromVector4(backColor);
                this.pattern = pattern;

                this.xLength = pattern.GetLength(0);
                this.yLength = pattern.GetLength(1);
                this.hasOpacitySet = foreColor.W != 1 || backColor.W != 1;
            }

            public override bool RequiresComposition
            {
                get
                {
                    return hasOpacitySet;
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

        public IBrushApplicator<TColor, TPacked> CreateApplicator<TColor, TPacked>(RectangleF region)
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
        {
            return new PatternBrushApplicator<TColor, TPacked>(this.foreColor.ToVector4(), this.backColor.ToVector4(), this.pattern);
        }
    }
}

// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests.Drawing
{
    using Drawing;
    using ImageSharp.Drawing;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Numerics;
    using Xunit;

    public class FillPatternBrushTests: FileTestBase
    {
        [Fact]
        public void ImageShouldBeFloodFilledWithPercent10()
        {
            string path = CreateOutputDirectory("Fill", "PatternBrush");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Percent10.png"))
            {
                image
                    .Fill(Color.Blue)
                    .Fill(PatternBrush.Percent10(Color.HotPink, Color.LimeGreen))
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[0, 0]);
                Assert.Equal(Color.LimeGreen, sourcePixels[1, 1]);
                Assert.Equal(Color.HotPink, sourcePixels[2, 2]);
                Assert.Equal(Color.LimeGreen, sourcePixels[3, 3]);
            }
        }
        [Fact]
        public void ImageShouldBeFloodFilledWithPercent10Transparent()
        {
            string path = CreateOutputDirectory("Fill", "PatternBrush");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Percent10_Transparent.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .Fill(PatternBrush.Percent10(Color.HotPink))
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[0, 0]);
                Assert.Equal(Color.Blue, sourcePixels[1, 1]);
                Assert.Equal(Color.HotPink, sourcePixels[2, 2]);
                Assert.Equal(Color.Blue, sourcePixels[3, 3]);
            }
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithPercent20()
        {
            string path = CreateOutputDirectory("Fill", "PatternBrush");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Percent20.png"))
            {
                image
                    .Fill(Color.Blue)
                    .Fill(PatternBrush.Percent20(Color.HotPink, Color.LimeGreen))
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[0, 0]);
                Assert.Equal(Color.HotPink, sourcePixels[0, 2]);
                Assert.Equal(Color.HotPink, sourcePixels[2, 1]);
                Assert.Equal(Color.HotPink, sourcePixels[2, 3]);

                Assert.Equal(Color.LimeGreen, sourcePixels[0, 1]);
                Assert.Equal(Color.LimeGreen, sourcePixels[2, 2]);
            }
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithPercent20_transparent()
        {
            string path = CreateOutputDirectory("Fill", "PatternBrush");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Percent20_transparent.png"))
            {
                image
                    .Fill(Color.Blue)
                    .Fill(PatternBrush.Percent20(Color.HotPink))
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[0, 0]);
                Assert.Equal(Color.HotPink, sourcePixels[0, 2]);
                Assert.Equal(Color.HotPink, sourcePixels[2, 1]);
                Assert.Equal(Color.HotPink, sourcePixels[2, 3]);

                Assert.Equal(Color.Blue, sourcePixels[0, 1]);
                Assert.Equal(Color.Blue, sourcePixels[2, 2]);

            }
        }
    }
}

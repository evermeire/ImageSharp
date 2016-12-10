// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests.Drawing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Xunit;
    using Drawing;
    using ImageSharp.Drawing;
    using System.Numerics;

    public class SolidBrushTests : FileTestBase
    {
        [Fact]
        public void ImageShouldApplySolidColorToAllpixels()
        {
            string path = CreateOutputDirectory("Fill");


            Image image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Solid.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .Fill(Brushes.HotPink)
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[9, 9]);
            }
        }

        [Fact]
        public void ImageShouldApplySolidColorToAllpixelsWithOpacity()
        {
            string path = CreateOutputDirectory("SolidBrush");


            Image image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Opacity.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .Fill(new SolidBrush(new Color(Color.HotPink.R, Color.HotPink.G, Color.HotPink.B, 150)))
                    .Save(output);
            }
            var mergedColor = new Color(Vector4.Lerp(Color.Blue.ToVector4(), Color.HotPink.ToVector4(), 150f / 255f));

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(mergedColor, sourcePixels[9, 9]);
            }
        }
    }
}
// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Brushes;
    using Xunit;

    public class SolidBrushTests : FileTestBase
    {
        [Fact]
        public void ImageShouldApplySolidColorToAllpixels()
        {
            string path = CreateOutputDirectory("SolidBrush");

            foreach (TestFile file in Files)
            {
                Image image = file.CreateImage();

                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
                {
                    image
                        .Fill(new SolidBrush(Color.HotPink))
                        .Save(output);
                }
            }

        }

        [Fact]
        public void ImageShouldApplySolidColorToAllpixelsWithOpacity()
        {
            string path = CreateOutputDirectory("SolidBrush", "Opacity");

            foreach (TestFile file in Files)
            {
                Image image = file.CreateImage();

                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
                {
                    image
                        .Fill(new SolidBrush(new Color(Color.HotPink.R, Color.HotPink.G, Color.HotPink.B, 150)))
                        .Save(output);
                }
            }

        }
    }
}

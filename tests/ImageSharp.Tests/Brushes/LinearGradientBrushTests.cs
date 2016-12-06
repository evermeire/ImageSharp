// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Brushs;
    using Xunit;

    public class LinearGradientBrushTests : FileTestBase
    {
        [Fact]
        public void ImageShouldApplySolidColorToAllpixels()
        {
            string path = CreateOutputDirectory("LinearGradientBrush");

            foreach (TestFile file in Files)
            {
                Image image = file.CreateImage();

                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
                {
                    image
                        .Draw(new LinearGradientBrush(Color.HotPink))
                        .Save(output);
                }
            }

        }
    }
}

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
                        .Draw(new SolidBrush(Color.HotPink))
                        .Save(output);
                }
            }

        }
    }
}

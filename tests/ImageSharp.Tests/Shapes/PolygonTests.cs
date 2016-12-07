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
    using Shapes;

    public class PolygonTests : FileTestBase
    {
        [Fact]
        public void ImageShouldBeOverlayedByFilledPolygon()
        {
            string path = CreateOutputDirectory("Polygons");
            var simplePath = new LinearLineSegment(
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                            );
            var brush = new SolidBrush(Color.HotPink);
            var polygon = new Polygon(brush, simplePath);

            foreach (TestFile file in Files)
            {
                Image image = file.CreateImage();

                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
                {
                    image
                        .Draw(polygon)
                        .Save(output);
                }
            }

        }
        [Fact]
        public void ImageShouldBeOverlayedByFilledRectangle()
        {
            string path = CreateOutputDirectory("Polygons", "Rectangle");
            var simplePath = new LinearLineSegment(
                            new Point(10, 10),
                            new Point(200, 10),
                            new Point(200, 150),
                            new Point(10, 150)
                            );
            var brush = new SolidBrush(Color.HotPink);
            var polygon = new Polygon(brush, simplePath);

            foreach (TestFile file in Files)
            {
                Image image = file.CreateImage();

                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
                {
                    image
                        .Draw(polygon)
                        .Save(output);
                }
            }

        }
    }
}

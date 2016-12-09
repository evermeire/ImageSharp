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
    using Shapes.Polygons;

    public class SolidPolygonTests : FileTestBase
    {
        [Fact]
        public void ImageShouldBeOverlayedByFilledPolygon()
        {
            string path = CreateOutputDirectory("SolidPolygons");
            var simplePath = new LinearLineSegment(
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                            );
            var brush = new SolidBrush(Color.HotPink);
            var polygon = new SolidPolygon(brush, simplePath);

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
        public void ImageShouldBeOverlayedByFilledPolygonOpacity()
        {
            string path = CreateOutputDirectory("SolidPolygons", "OpacityBrush");
            var simplePath = new LinearLineSegment(
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                            );
            var color = new Color(Color.HotPink.R, Color.HotPink.G, Color.HotPink.B, 150);
            var brush = new SolidBrush(color);
            var polygon = new SolidPolygon(brush, simplePath);

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
            string path = CreateOutputDirectory("SolidPolygons", "Rectangle");
            var simplePath = new LinearLineSegment(
                            new Point(10, 10),
                            new Point(200, 10),
                            new Point(200, 150),
                            new Point(10, 150)
                            );
            var brush = new SolidBrush(Color.HotPink);
            var polygon = new SolidPolygon(brush, simplePath);

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
        public void ImageShouldBeOverlayedByFilledRectangleWithHole()
        {
            string path = CreateOutputDirectory("SolidPolygons", "Hole");
            
            var outlinePoly = new SimplePolygon(new[] { new LinearLineSegment(
                            new Point(10, 10),
                            new Point(200, 10),
                            new Point(200, 150),
                            new Point(10, 150)
                            ) });

            var hole1 = new SimplePolygon(new[] { new LinearLineSegment(
                            new Point(20, 20),
                            new Point(20, 40),
                            new Point(40, 40),
                            new Point(40, 20)
                            ) })
            { IsHole = true };

            var hole2 = new SimplePolygon(new[] { new LinearLineSegment(
                            new Point(120, 120),
                            new Point(120, 140),
                            new Point(140, 140),
                            new Point(140, 120)
                            ) })
            { IsHole = true };

            var complexPoly = new ComplexPolygon(new[] {
                outlinePoly, hole1, hole2
            });
            var brush = new SolidBrush(Color.HotPink);
            var polygon = new SolidPolygon(brush, complexPoly);

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

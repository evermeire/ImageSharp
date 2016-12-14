﻿// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
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

    public class SolidPolygonTests : FileTestBase
    {
        [Fact]
        public void ImageShouldBeOverlayedByFilledPolygon()
        {
            string path = CreateOutputDirectory("Drawing", "FilledPolygons");
            var simplePath = new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
            };
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Simple.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .FillPolygon(Color.HotPink, simplePath)
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[11, 11]);

                Assert.Equal(Color.HotPink, sourcePixels[200, 150]);

                Assert.Equal(Color.HotPink, sourcePixels[50, 50]);

                Assert.Equal(Color.Blue, sourcePixels[2, 2]);
            }
        }

        [Fact]
        public void ImageShouldBeOverlayedByFilledPolygonOpacity()
        {
            string path = CreateOutputDirectory("Drawing", "FilledPolygons");
            var simplePath = new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
            };
            var color = new Color(Color.HotPink.R, Color.HotPink.G, Color.HotPink.B, 150);
            
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Opacity.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .FillPolygon(color, simplePath)
                    .Save(output);
            }

            //shift background color towards forground color by the opacity amount
            var mergedColor = new Color(Vector4.Lerp(Color.Blue.ToVector4(), Color.HotPink.ToVector4(), 150f / 255f));

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(mergedColor, sourcePixels[11, 11]);

                Assert.Equal(mergedColor, sourcePixels[200, 150]);

                Assert.Equal(mergedColor, sourcePixels[50, 50]);

                Assert.Equal(Color.Blue, sourcePixels[2, 2]);
            }
        }

        [Fact]
        public void ImageShouldBeOverlayedByFilledRectangle()
        {
            string path = CreateOutputDirectory("Drawing", "FilledPolygons");
            var simplePath = new[] {
                            new Point(10, 10),
                            new Point(200, 10),
                            new Point(200, 150),
                            new Point(10, 150)
                            };

            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Rectangle.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .FillPolygon(Color.HotPink, simplePath)
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[11, 11]);

                Assert.Equal(Color.HotPink, sourcePixels[198, 10]);

                Assert.Equal(Color.HotPink, sourcePixels[10, 50]);

                Assert.Equal(Color.HotPink, sourcePixels[50, 50]);

                Assert.Equal(Color.Blue, sourcePixels[2, 2]);
            }
        }
    }
}

﻿// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests.Drawing
{
    using Drawing;
    using ImageSharp.Drawing;
    using ImageSharp.Drawing.Pens;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Numerics;
    using Xunit;

    public class LineTests : FileTestBase
    {


        [Fact]
        public void ImageShouldBeOverlayedByPath()
        {
            string path = CreateOutputDirectory("Drawing", "Lines");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Simple.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawLines(Color.HotPink, 5, new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                    })
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[9, 9]);

                Assert.Equal(Color.HotPink, sourcePixels[199, 149]);

                Assert.Equal(Color.Blue, sourcePixels[50, 50]);
            }

        }


        [Fact]
        public void ImageShouldBeOverlayedByPathDashed()
        {
            string path = CreateOutputDirectory("Drawing", "Lines");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Dashed.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawLines(Pens.Dash(Color.HotPink, 5), new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                    })
                    .Save(output);
            }

        }

        [Fact]
        public void ImageShouldBeOverlayedByPathDotted()
        {
            string path = CreateOutputDirectory("Drawing", "Lines");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Dot.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawLines(Pens.Dot(Color.HotPink, 5), new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                    })
                    .Save(output);
            }
        }

        [Fact]
        public void ImageShouldBeOverlayedByPathDashDot()
        {
            string path = CreateOutputDirectory("Drawing", "Lines");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/DashDot.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawLines(Pens.DashDot(Color.HotPink, 5), new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                    })
                    .Save(output);
            }

        }

        [Fact]
        public void ImageShouldBeOverlayedByPathDashDotDot()
        {
            string path = CreateOutputDirectory("Drawing", "Lines");
            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/DashDotDot.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawLines(Pens.DashDotDot(Color.HotPink, 5), new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                    })
                    .Save(output);
            }
        }

        [Fact]
        public void ImageShouldBeOverlayedPathWithOpacity()
        {
            string path = CreateOutputDirectory("Drawing", "Lines");

            var color = new Color(Color.HotPink.R, Color.HotPink.G, Color.HotPink.B, 150);

            var image = new Image(500, 500);
            

            using (FileStream output = File.OpenWrite($"{path}/Opacity.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawLines(color, 10, new[] {
                            new Point(10, 10),
                            new Point(200, 150),
                            new Point(50, 300)
                    })
                    .Save(output);
            }

            //shift background color towards forground color by the opacity amount
            var mergedColor = new Color(Vector4.Lerp(Color.Blue.ToVector4(), Color.HotPink.ToVector4(), 150f/255f));

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(mergedColor, sourcePixels[9, 9]);

                Assert.Equal(mergedColor, sourcePixels[199, 149]);

                Assert.Equal(Color.Blue, sourcePixels[50, 50]);
            }
        }

        [Fact]
        public void ImageShouldBeOverlayedByPathOutline()
        {
            string path = CreateOutputDirectory("Drawing", "Lines");

            var image = new Image(500, 500);

            using (FileStream output = File.OpenWrite($"{path}/Rectangle.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawLines(Color.HotPink, 10, new[] {
                            new Point(10, 10),
                            new Point(200, 10),
                            new Point(200, 150),
                            new Point(10, 150)
                        })
                    .Save(output);
            }

            using (var sourcePixels = image.Lock())
            {
                Assert.Equal(Color.HotPink, sourcePixels[8, 8]);

                Assert.Equal(Color.HotPink, sourcePixels[198, 10]);

                Assert.Equal(Color.Blue, sourcePixels[10, 50]);

                Assert.Equal(Color.Blue, sourcePixels[50, 50]);
            }
        }
        
    }
}

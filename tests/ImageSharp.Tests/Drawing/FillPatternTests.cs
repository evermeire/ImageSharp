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
        private Image Test(string name, Color background, IBrush brush, Color[,] expectedPattern)
        {
            string path = CreateOutputDirectory("Fill", "PatternBrush");
            var image = new Image(20, 20);
            image
                  .Fill(background)
                  .Fill(brush);

            using (FileStream output = File.OpenWrite($"{path}/{name}.png"))
            {
                image.Save(output);
            }
            using (var sourcePixels = image.Lock())
            {
                // lets pick random spots to start checking
                var r = new Random();
                var xStride = expectedPattern.GetLength(1);
                var yStride = expectedPattern.GetLength(0);
                var offsetX = r.Next(image.Width / xStride) * xStride;
                var offsetY = r.Next(image.Height / yStride) * yStride;
                for (var x = 0; x < xStride; x++)
                {
                    for (var y = 0; y < yStride; y++)
                    {
                        var actualX = x + offsetX;
                        var actualY = y + offsetY;
                        var expected = expectedPattern[y, x]; // inverted pattern
                        var actual = sourcePixels[actualX, actualY];
                        if (expected != actual)
                        {
                            Assert.True(false, $"Expected {expected} but found {actual} at ({actualX},{actualY})");
                        }
                    }
                }
            }
            using (FileStream output = File.OpenWrite($"{path}/{name}x4.png"))
            {
                image.Resize(80,80).Save(output);
            }



            return image;
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithPercent10()
        {
            Test("Percent10", Color.Blue, PatternBrush.Percent10(Color.HotPink, Color.LimeGreen), new Color[,] {
                { Color.HotPink , Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.HotPink , Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.LimeGreen, Color.LimeGreen}
            });            
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithPercent10Transparent()
        {
            Test("Percent10_Transparent", Color.Blue, PatternBrush.Percent10(Color.HotPink), 
            new Color[,] {
                { Color.HotPink , Color.Blue, Color.Blue, Color.Blue},
                { Color.Blue, Color.Blue, Color.Blue, Color.Blue},
                { Color.Blue, Color.Blue, Color.HotPink , Color.Blue},
                { Color.Blue, Color.Blue, Color.Blue, Color.Blue}
            });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithPercent20()
        {
            Test("Percent20", Color.Blue, PatternBrush.Percent20(Color.HotPink, Color.LimeGreen),
           new Color[,] {
                { Color.HotPink , Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.HotPink , Color.LimeGreen},
                { Color.HotPink , Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.HotPink , Color.LimeGreen}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithPercent20_transparent()
        {
            Test("Percent20_Transparent", Color.Blue, PatternBrush.Percent20(Color.HotPink),
           new Color[,] {
                { Color.HotPink , Color.Blue, Color.Blue, Color.Blue},
                { Color.Blue, Color.Blue, Color.HotPink , Color.Blue},
                { Color.HotPink , Color.Blue, Color.Blue, Color.Blue},
                { Color.Blue, Color.Blue, Color.HotPink , Color.Blue}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithHorizontal()
        {
            Test("Horizontal", Color.Blue, PatternBrush.Horizontal(Color.HotPink, Color.LimeGreen),
           new Color[,] {
                { Color.LimeGreen , Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.HotPink, Color.HotPink, Color.HotPink , Color.HotPink},
                { Color.LimeGreen , Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.LimeGreen , Color.LimeGreen}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithHorizontal_transparent()
        {
            Test("Horizontal_Transparent", Color.Blue, PatternBrush.Horizontal(Color.HotPink),
           new Color[,] {
                { Color.Blue , Color.Blue, Color.Blue, Color.Blue},
                { Color.HotPink, Color.HotPink, Color.HotPink , Color.HotPink},
                { Color.Blue , Color.Blue, Color.Blue, Color.Blue},
                { Color.Blue, Color.Blue, Color.Blue , Color.Blue}
           });
        }



        [Fact]
        public void ImageShouldBeFloodFilledWithMin()
        {
            Test("Min", Color.Blue, PatternBrush.Min(Color.HotPink, Color.LimeGreen),
           new Color[,] {
                { Color.LimeGreen , Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen , Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.LimeGreen , Color.LimeGreen},
                { Color.HotPink, Color.HotPink, Color.HotPink , Color.HotPink}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithMin_transparent()
        {
            Test("Min_Transparent", Color.Blue, PatternBrush.Min(Color.HotPink),
           new Color[,] {
                { Color.Blue , Color.Blue, Color.Blue, Color.Blue},
                { Color.Blue , Color.Blue, Color.Blue, Color.Blue},
                { Color.Blue, Color.Blue, Color.Blue , Color.Blue},
                { Color.HotPink, Color.HotPink, Color.HotPink , Color.HotPink},
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithVertical()
        {
            Test("Vertical", Color.Blue, PatternBrush.Vertical(Color.HotPink, Color.LimeGreen),
           new Color[,] {
                { Color.LimeGreen, Color.HotPink, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.HotPink, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.HotPink, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.HotPink, Color.LimeGreen, Color.LimeGreen}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithVertical_transparent()
        {
            Test("Vertical_Transparent", Color.Blue, PatternBrush.Vertical(Color.HotPink),
           new Color[,] {
                { Color.Blue, Color.HotPink, Color.Blue, Color.Blue},
                { Color.Blue, Color.HotPink, Color.Blue, Color.Blue},
                { Color.Blue, Color.HotPink, Color.Blue, Color.Blue},
                { Color.Blue, Color.HotPink, Color.Blue, Color.Blue}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithForwardDiagnal()
        {
            Test("ForwardDiagnal", Color.Blue, PatternBrush.ForwardDiagnal(Color.HotPink, Color.LimeGreen),
           new Color[,] {
                { Color.HotPink, Color.LimeGreen, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.HotPink, Color.LimeGreen, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.HotPink, Color.LimeGreen},
                { Color.LimeGreen, Color.LimeGreen, Color.LimeGreen, Color.HotPink}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithForwardDiagnal_transparent()
        {
            Test("ForwardDiagnal_Transparent", Color.Blue, PatternBrush.ForwardDiagnal(Color.HotPink),
           new Color[,] {
                { Color.HotPink, Color.Blue,    Color.Blue,    Color.Blue},
                { Color.Blue,    Color.HotPink, Color.Blue,    Color.Blue},
                { Color.Blue,    Color.Blue,    Color.HotPink, Color.Blue},
                { Color.Blue,    Color.Blue,    Color.Blue,    Color.HotPink}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithBackwardDiagnal()
        {
            Test("BackwardDiagnal", Color.Blue, PatternBrush.BackwardDiagnal(Color.HotPink, Color.LimeGreen),
           new Color[,] {
                { Color.LimeGreen, Color.LimeGreen, Color.LimeGreen, Color.HotPink},
                { Color.LimeGreen, Color.LimeGreen, Color.HotPink, Color.LimeGreen},
                { Color.LimeGreen, Color.HotPink, Color.LimeGreen, Color.LimeGreen},
                { Color.HotPink, Color.LimeGreen, Color.LimeGreen, Color.LimeGreen}
           });
        }

        [Fact]
        public void ImageShouldBeFloodFilledWithBackwardDiagnal_transparent()
        {
            Test("BackwardDiagnal_Transparent", Color.Blue, PatternBrush.BackwardDiagnal(Color.HotPink),
           new Color[,] {
                { Color.Blue, Color.Blue,    Color.Blue,    Color.HotPink},
                { Color.Blue,    Color.Blue, Color.HotPink,    Color.Blue},
                { Color.Blue,    Color.HotPink,    Color.Blue, Color.Blue},
                { Color.HotPink,    Color.Blue,    Color.Blue,    Color.Blue}
           });
        }

        
    }
}

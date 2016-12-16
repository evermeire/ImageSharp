// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Tests.Drawing
{
    using Drawing;
    using ImageSharp.Drawing;
    using CorePath = ImageSharp.Drawing.Paths.Path;
    using ImageSharp.Drawing.Paths;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Numerics;
    using Xunit;

    public class DrawStringTests : FileTestBase
    {
        [Fact]
        public void ImageShouldBeOverlayedByText()
        {
            string path = CreateOutputDirectory("Drawing", "String");
            var image = new Image(500, 500);

            var fontFile = new TestFont(TestFonts.Ttf.Tahoma);
            var font = fontFile.CreateFont();
            font.Size = 40;
            using (FileStream output = File.OpenWrite($"{path}/Simple.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawString("Hello World", new Vector2(10, 10), font, Color.HotPink)
                    .Save(output);
            }

            //using (var sourcePixels = image.Lock())
            //{
            //    Assert.Equal(Color.HotPink, sourcePixels[9, 9]);

            //    Assert.Equal(Color.HotPink, sourcePixels[199, 149]);

            //    Assert.Equal(Color.Blue, sourcePixels[50, 50]);
            //}

        }
        [Fact]
        public void ImageShouldBeOverlayedByTextNewLine()
        {
            string path = CreateOutputDirectory("Drawing", "String");
            var image = new Image(500, 500);

            var fontFile = new TestFont(TestFonts.Ttf.Tahoma);
            var font = fontFile.CreateFont();
            font.Size = 40;
            using (FileStream output = File.OpenWrite($"{path}/Newline.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawString("Hello\nWorld", new Vector2(10, 10), font, Color.HotPink)
                    .Save(output);
            }

            //using (var sourcePixels = image.Lock())
            //{
            //    Assert.Equal(Color.HotPink, sourcePixels[9, 9]);

            //    Assert.Equal(Color.HotPink, sourcePixels[199, 149]);

            //    Assert.Equal(Color.Blue, sourcePixels[50, 50]);
            //}

        }

        [Fact]
        public void ImageShouldBeOverlayedByTextNewLineAndWithTab()
        {
            string path = CreateOutputDirectory("Drawing", "String");
            var image = new Image(500, 500);

            var fontFile = new TestFont(TestFonts.Ttf.Tahoma);
            var font = fontFile.CreateFont();
            font.Size = 40;
            using (FileStream output = File.OpenWrite($"{path}/WithTab.png"))
            {
                image
                    .BackgroundColor(Color.Blue)
                    .DrawString("Hello World\nHello\tWorld", new Vector2(10, 10), font, Color.HotPink)
                    .Save(output);
            }

            //using (var sourcePixels = image.Lock())
            //{
            //    Assert.Equal(Color.HotPink, sourcePixels[9, 9]);

            //    Assert.Equal(Color.HotPink, sourcePixels[199, 149]);

            //    Assert.Equal(Color.Blue, sourcePixels[50, 50]);
            //}

        }


        //[Fact]
        //public void ImageShouldBeOverlayedPathWithOpacity()
        //{
        //    string path = CreateOutputDirectory("Drawing", "Path");

        //    var color = new Color(Color.HotPink.R, Color.HotPink.G, Color.HotPink.B, 150);


        //    var linerSegemnt = new LinearLineSegment(
        //                    new Vector2(10, 10),
        //                    new Vector2(200, 150),
        //                    new Vector2(50, 300)
        //            );
        //    var bazierSegment = new BezierLineSegment(new Vector2(50, 300),
        //        new Vector2(500, 500),
        //        new Vector2(60, 10),
        //        new Vector2(10, 400));

        //    var p = new CorePath(linerSegemnt, bazierSegment);

        //    var image = new Image(500, 500);


        //    using (FileStream output = File.OpenWrite($"{path}/Opacity.png"))
        //    {
        //        image
        //            .BackgroundColor(Color.Blue)
        //            .DrawPath(color, 10, p)
        //            .Save(output);
        //    }

        //    //shift background color towards forground color by the opacity amount
        //    var mergedColor = new Color(Vector4.Lerp(Color.Blue.ToVector4(), Color.HotPink.ToVector4(), 150f / 255f));

        //    using (var sourcePixels = image.Lock())
        //    {
        //        Assert.Equal(mergedColor, sourcePixels[9, 9]);

        //        Assert.Equal(mergedColor, sourcePixels[199, 149]);

        //        Assert.Equal(Color.Blue, sourcePixels[50, 50]);
        //    }
        //}

    }
}
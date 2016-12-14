﻿// <copyright file="Crop.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Benchmarks
{
    using System.Drawing;
    using System.Drawing.Drawing2D;

    using BenchmarkDotNet.Attributes;
    using CoreImage = ImageSharp.Image;
    using CorePoint = ImageSharp.Point;
    using CoreColor = ImageSharp.Color;
    using System.IO;

    public class FillPolygon
    {
        [Benchmark(Baseline = true, Description = "System.Drawing Fill Polygon")]
        public void DrawSolidPolygonSystemDrawing()
        {
            using (Bitmap destination = new Bitmap(800, 800))
            {

                using (Graphics graphics = Graphics.FromImage(destination))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.FillPolygon(Brushes.HotPink, new[] {
                        new Point(10, 10),
                        new Point(550, 50),
                        new Point(200, 400)
                    });
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    destination.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        [Benchmark(Description = "ImageSharp Fill Polygon")]
        public void DrawSolidPolygonCore()
        {
            CoreImage image = new CoreImage(800, 800);
            image.FillPolygon(CoreColor.HotPink,
                 new[] {
                     new CorePoint(10, 10),
                     new CorePoint(550, 50),
                     new CorePoint(200, 400)
                 }
                );

            using (MemoryStream ms = new MemoryStream())
            {
                image.SaveAsBmp(ms);
            }
        }
    }
}
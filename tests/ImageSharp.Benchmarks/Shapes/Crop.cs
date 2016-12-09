// <copyright file="Crop.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Benchmarks
{
    using System.Drawing;
    using System.Drawing.Drawing2D;

    using BenchmarkDotNet.Attributes;
    using Shapes;
    using CoreImage = ImageSharp.Image;
    using CoreSize = ImageSharp.Size;

    public class SolidSimpleShape
    {
        [Benchmark(Baseline = true, Description = "System.Drawing Draw Solid Polygon")]
        public Size DrawSolidPolygonSystemDrawing()
        {
            using (Bitmap destination = new Bitmap(800, 800))
            {

                using (Graphics graphics = Graphics.FromImage(destination))
                {
                    graphics.FillPolygon(Brushes.HotPink, new[] {
                        new Point(10, 10),
                        new Point(550, 50),
                        new Point(200, 400)
                    });
                }

                return destination.Size;
            }
        }

        [Benchmark(Description = "ImageSharp Draw Solid Polygon")]
        public CoreSize DrawSolidPolygonCore()
        {
            CoreImage image = new CoreImage(800, 800);
            image.FillPolygon(new ImageSharp.Brushes.SolidBrush(ImageSharp.Color.HotPink), 
                 new[] {
                     new ImageSharp.Point(10, 10),
                     new ImageSharp.Point(550, 50),
                     new ImageSharp.Point(200, 400)
                 }
                );
            return new CoreSize(image.Width, image.Height);
        }
    }
}

//// <copyright file="ColorConversionTests.cs" company="James Jackson-South">
//// Copyright (c) James Jackson-South and contributors.
//// Licensed under the Apache License, Version 2.0.
//// </copyright>

//namespace ImageSharp.Tests
//{
//    using System;
//    using System.Diagnostics.CodeAnalysis;
//    using System.IO;
//    using Brushes;
//    using Xunit;
//    using Shapes;
//    using Shapes.Polygons;

//    public class SolidPolygonWithOutlineTests : FileTestBase
//    {
//        [Fact]
//        public void ImageShouldBeOverlayedByFilledPolygon()
//        {
//            string path = CreateOutputDirectory("SolidPolygonsWithOutline");
//            var simplePath = new LinearLineSegment(
//                            new Point(10, 10),
//                            new Point(200, 150),
//                            new Point(50, 300)
//                            );
//            var brush = new SolidBrush(Color.HotPink);
//            var polygon = new SolidPolygon(brush, simplePath);
//            var brushOutline = new SolidBrush(Color.Aqua);
//            var outline = new Polygon(brushOutline, 3, simplePath);

//            foreach (TestFile file in Files)
//            {
//                Image image = file.CreateImage();

//                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
//                {
//                    image
//                        .Draw(polygon)
//                        .Draw(outline)
//                        .Save(output);
//                }
//            }
//        }


//        [Fact]
//        public void ImageShouldBeOverlayedByFilledPolygonOpacity()
//        {
//            string path = CreateOutputDirectory("SolidPolygonsWithOutline", "OpacityBrush");
//            var simplePath = new LinearLineSegment(
//                            new Point(10, 10),
//                            new Point(200, 150),
//                            new Point(50, 300)
//                            );
//            var color = new Color(Color.HotPink.R, Color.HotPink.G, Color.HotPink.B, 150);
//            var brush = new SolidBrush(color);
//            var polygon = new SolidPolygon(brush, simplePath);
//            var brushOutline = new SolidBrush(Color.Aqua);
//            var outline = new Polygon(brushOutline, 3, simplePath);

//            foreach (TestFile file in Files)
//            {
//                Image image = file.CreateImage();

//                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
//                {
//                    image
//                        .Draw(polygon)
//                        .Draw(outline)
//                        .Save(output);
//                }
//            }
//        }

//        [Fact]
//        public void ImageShouldBeOverlayedByFilledRectangle()
//        {
//            string path = CreateOutputDirectory("SolidPolygonsWithOutline", "Rectangle");
//            var simplePath = new LinearLineSegment(
//                            new Point(10, 10),
//                            new Point(200, 10),
//                            new Point(200, 150),
//                            new Point(10, 150)
//                            );
//            var brush = new SolidBrush(Color.HotPink);
//            var polygon = new SolidPolygon(brush, simplePath);
//            var brushOutline = new SolidBrush(Color.Aqua);
//            var outline = new Polygon(brushOutline, 3, simplePath);

//            foreach (TestFile file in Files)
//            {
//                Image image = file.CreateImage();

//                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
//                {
//                    image
//                        .Draw(polygon)
//                        .Draw(outline)
//                        .Save(output);
//                }
//            }

//        }

//        [Fact]
//        public void ImageShouldBeOverlayedByFilledRectangleWithHole()
//        {
//            string path = CreateOutputDirectory("SolidPolygonsWithOutline", "Hole");

//            new ComplexPolygon(
//                new[] {

//                }
//                )

//            var outlinePoly = ;

//            var hole1 = new SimplePolygon(new[] { new LinearLineSegment(
//                            new Point(20, 20),
//                            new Point(20, 40),
//                            new Point(40, 40),
//                            new Point(40, 20)
//                            ) })
//            { IsHole = true; };

//            var hole2 = new SimplePolygon(new[] { new LinearLineSegment(
//                            new Point(120, 120),
//                            new Point(120, 140),
//                            new Point(140, 140),
//                            new Point(140, 120)
//                            ) });

//            var brush = new SolidBrush(Color.HotPink);
//            var polygon = new SolidPolygon(brush, outlinePoly, hole1, hole2);


//            var brushOutline = new SolidBrush(Color.Aqua);
            
//            var outline = new Polygon(brushOutline, 3, );

//            foreach (TestFile file in Files)
//            {
//                Image image = file.CreateImage();

//                using (FileStream output = File.OpenWrite($"{path}/{file.FileName}"))
//                {
//                    image
//                        .Draw(polygon)
//                        .Save(output);
//                }
//            }

//        }
//    }
//}

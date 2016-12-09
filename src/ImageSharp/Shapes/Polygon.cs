// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Shapes
{
    using Brushes;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using Polygons;

    
    public class Polygon : IVectorGraphic
    {
        public ParallelOptions ParallelOptions { get; set; } = Bootstrapper.Instance.ParallelOptions;

        private readonly IBrush fillColor;

        private Lazy<Rectangle> bounds;
        private SimplePolygon outline;
        private IEnumerable<SimplePolygon> holes;
        private SimplePolygon RawPolygon;
        private Lazy<SolidPolygon> solidPolygon;
        private readonly float thickness;

        public Polygon(IBrush fillColor, float thickness, ILineSegment segment) 
            : this(fillColor, thickness, new[] { segment })
        {
        }

        public Polygon(IBrush fillColor, float thickness, IEnumerable<ILineSegment> segments)
        {
            this.fillColor = fillColor;
            this.thickness = thickness / 2;
            RawPolygon = new SimplePolygon(segments); 

            solidPolygon = new Lazy<SolidPolygon>(GenerateSolid);
        }


        Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.Y - ps1.Y;
            float B1 = ps1.X - pe1.X;
            float C1 = A1 * ps1.X + B1 * ps1.Y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.Y - ps2.Y;
            float B2 = ps2.X - pe2.X;
            float C2 = A2 * ps2.X + B2 * ps2.Y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                throw new System.Exception("Lines are parallel");

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }

        private SolidPolygon GenerateSolid()
        {
            var outlinePoints = new List<Vector2>();
            var holePoints = new List<Vector2>();

            const double halfPI = Math.PI / 2;
            for (var i = 0; i < RawPolygon.Corners; i++)
            {
                var prevPoint = RawPolygon[i - 1];
                var nextPoint = RawPolygon[i + 1];
                var currentPoint = RawPolygon[i];


                // lets make a new line 'thickness 'distance from 
                // line joining 'currentPoint' to 'prevPoint'

                var deltaPrev = prevPoint - currentPoint;
                var prevAngleOut = Math.Atan2(deltaPrev.Y, deltaPrev.X) + halfPI;
                var prevAngleIn = prevAngleOut + Math.PI;

                var prevXDistOut = thickness * Math.Cos(prevAngleOut);
                var prevYDistOut = thickness * Math.Sin(prevAngleOut);

                var prevStartOut = new Vector2(
                        (float)(currentPoint.X + prevXDistOut),
                        (float)(currentPoint.Y - prevYDistOut)
                    );
                var prevEndOut = new Vector2(
                       (float)(prevPoint.X + prevXDistOut),
                       (float)(prevPoint.Y - prevYDistOut)
                   );

                var prevXDistIn = thickness * Math.Cos(prevAngleIn);
                var prevYDistIn = thickness * Math.Sin(prevAngleIn);

                var prevStartIn = new Vector2(
                     (float)(currentPoint.X + prevXDistIn),
                     (float)(currentPoint.Y - prevYDistIn)
                 );
                var prevEndIn = new Vector2(
                       (float)(prevPoint.X + prevXDistIn),
                       (float)(prevPoint.Y - prevYDistIn)
                   );


                var deltaNext = nextPoint - currentPoint;
                var nextAngleOut = Math.Atan2(deltaNext.Y, deltaNext.X) + halfPI;
                var nextAngleIn = nextAngleOut + Math.PI;
                var nextXDistOut = thickness * Math.Cos(nextAngleOut);
                var nextYDistOut = thickness * Math.Sin(nextAngleOut);

                var nextStartOut = new Vector2(
                        (float)(currentPoint.X + nextXDistOut),
                        (float)(currentPoint.Y - nextYDistOut)
                    );
                var nextEndOut = new Vector2(
                       (float)(nextPoint.X + nextXDistOut),
                       (float)(nextPoint.Y - nextYDistOut)
                   );
                var nextXDistIn = thickness * Math.Cos(nextAngleIn);
                var nextYDistIn = thickness * Math.Sin(nextAngleIn);
                var nextStartIn = new Vector2(
                    (float)(currentPoint.X + nextXDistIn),
                    (float)(currentPoint.Y - nextYDistIn)
                );
                var nextEndIn = new Vector2(
                       (float)(nextPoint.X + nextXDistIn),
                       (float)(nextPoint.Y - nextYDistIn)
                   );

                var outPoint = LineIntersectionPoint(prevStartOut, prevEndOut, nextStartOut, nextEndOut);
                var inPoint = LineIntersectionPoint(prevStartIn, prevEndIn, nextStartIn, nextEndIn);

                if(RawPolygon.Distance(inPoint) == 0)
                {
                    outlinePoints.Add(outPoint);
                    holePoints.Add(inPoint);
                }else
                {

                    outlinePoints.Add(inPoint);
                    holePoints.Add(outPoint);
                }
            }
            
            return new SolidPolygon(fillColor, new ComplexPolygon(new[] { new SimplePolygon(outlinePoints), new SimplePolygon(holePoints) { IsHole = true } }));
        }


        public void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            solidPolygon.Value.Apply(source);
        }
    }
}

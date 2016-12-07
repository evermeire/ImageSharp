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

        private SolidPolygon GenerateSolid()
        {
            var outlinePoints = new List<Vector2>();
            var holePoints = new List<Vector2>();
            
            for (var i = 0; i< RawPolygon.Corners; i++)
            {
                var prevPoint = RawPolygon[i - 1];
                var nextPoint = RawPolygon[i + 1];
                var currentPoint = RawPolygon[i];

                var deltaPrev = prevPoint - currentPoint ;
                var deltaNext = nextPoint - currentPoint;

                var prevAngle = Math.Atan2(deltaPrev.Y, deltaPrev.X);                
                var nextAngle = Math.Atan2(deltaNext.Y, deltaNext.X);                               
                var angleOut = ((((prevAngle - nextAngle) / 2)+Math.PI) % Math.PI) + nextAngle;
                var angleIn = angleOut + Math.PI;
                
                // this is currently wrong it draws a line thickness/2 distance from 
                // the point instead of perpendicular to the point
                // points need to be calculated at the 90 degree mark and then a 
                // intersection of the lines based on thier parents lines angle
                // needs to be calcualted

                var outPoint = new Vector2(
                        (float)(currentPoint.X - thickness * Math.Cos(angleOut)),
                        (float)(currentPoint.Y - thickness * Math.Sin(angleOut))
                    );                         
                var inPoint = new Vector2(     
                        (float)(currentPoint.X - thickness * Math.Cos(angleIn)),
                        (float)(currentPoint.Y - thickness * Math.Sin(angleIn))
                    );

                outlinePoints.Add(outPoint);
                holePoints.Add(inPoint);
            }
            
            return new SolidPolygon(fillColor, new SimplePolygon(outlinePoints), new SimplePolygon(holePoints));
        }


        public void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            solidPolygon.Value.Apply(source);
        }
    }
}

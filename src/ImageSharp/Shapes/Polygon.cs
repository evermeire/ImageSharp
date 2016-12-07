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

    internal class Polygon : IVectorGraphic, IMask
    {
        private readonly IBrush fillColor;

        private Lazy<Rectangle> bounds;
        private IReadOnlyList<ILineSegment> segments;

        public Polygon(IBrush fillColor, ILineSegment segment) : this(fillColor, new[] { segment })
        {
        }

        public Polygon(IBrush fillColor, IEnumerable<ILineSegment> segments)
        {
            this.segments = new ReadOnlyCollection<ILineSegment>(segments.ToList());

            bounds = new Lazy<Rectangle>(CalculateBounds);
            this.fillColor = fillColor;
        }

        public Rectangle Bounds => bounds.Value;

        float[] constant;
        float[] multiple;
        bool calcualted = false;
        object locker = new object();

        float[] polyY;
        float[] polyX;
        int polyCorners;
        bool calcualtedPoints = false;
        object lockerPoints = new object();

        private void CalculatePoints()
        {
            if (calcualtedPoints) return;
            lock (lockerPoints)
            {
                if (calcualtedPoints) return;

                var points = Simplify(segments).ToArray();
                polyX = points.Select(x => (float)x.X).ToArray();
                polyY = points.Select(x => (float)x.Y).ToArray();
                polyCorners = points.Length;
                calcualtedPoints = true;
            }
        }

        private void CalculateConstants()
        {
            if (calcualted) return;
            lock (locker)
            {
                if (calcualted) return;

                // ensure points are availible
                CalculatePoints();
                
                constant = new float[polyCorners];
                multiple = new float[polyCorners];
                int i, j = polyCorners - 1;

                for (i = 0; i < polyCorners; i++)
                {
                    if (polyY[j] == polyY[i])
                    {
                        constant[i] = polyX[i];
                        multiple[i] = 0;
                    }
                    else
                    {
                        constant[i] = polyX[i] - (polyY[i] * polyX[j]) / (polyY[j] - polyY[i]) + (polyY[i] * polyX[i]) / (polyY[j] - polyY[i]);
                        multiple[i] = (polyX[j] - polyX[i]) / (polyY[j] - polyY[i]);
                    }
                    j = i;
                }



                calcualted = true;
            }
        }

        bool PointInPolygon(int x, int y)
        {
            if(!Bounds.Contains(x, y))
            {
                return false;
            }

            // things we cound do to make this more efficient
            // pre calculate simple regions that are inside the polygo and see if its contained in one of those

            CalculateConstants();
            

            var j = polyCorners - 1;
            bool oddNodes = false;

            for (var i = 0; i < polyCorners; i++)
            {
                if ((polyY[i] < y && polyY[j] >= y
                || polyY[j] < y && polyY[i] >= y))
                {
                    oddNodes ^= (y * multiple[i] + constant[i] < x);
                }
                j = i;
            }

            return oddNodes;
        }

        public float CalculateDistance(int x, int y)
        {
            float distance = int.MaxValue;
            for (var i = 0; i < polyCorners-1; i++)
            {
                var xDist = polyX[i + 1] - polyX[i];
                var yDist = polyY[i + 1] - polyY[i];

                var yDistPoint = polyY[i] - y;
                var xDistPoint = polyX[i] - x;

                var lastDistance = (float)Math.Abs(
                    Math.Abs(xDist * yDistPoint - xDistPoint * yDist)
                    /
                    Math.Sqrt((xDist * xDist) + (yDist * yDist))
                );

                if(lastDistance < distance)
                {
                    distance = lastDistance;
                }
            }

            return distance;
        }

        public float Distance(int x, int y)
        {
            // we will do a nieve point in polygon test here for now 
            // TODO optermise here and actually return a distance
            if (PointInPolygon(x, y))
            {
                //we are on line or inside
                return 0;
            }

            return CalculateDistance(x, y);
        }

        private Rectangle CalculateBounds()
        {
            // ensure points are availible
            CalculatePoints();
            var minX = (int)polyX.Min();
            var maxX = (int)polyX.Max();
            var minY = (int)polyY.Min();
            var maxY = (int)polyY.Max();

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private IEnumerable<Point> Simplify(IEnumerable<ILineSegment> segments)
        {
            //used to deduplicate
            HashSet<Point> pointHash = new HashSet<Point>();

            foreach (var segment in segments)
            {
                var points = segment.Simplify();
                foreach (var p in points)
                {
                    if (!pointHash.Contains(p))
                    {
                        pointHash.Add(p);
                        yield return p;
                    }
                }
            }
        }

        public void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {

            fillColor.Apply(source, this);
        }
    }
}

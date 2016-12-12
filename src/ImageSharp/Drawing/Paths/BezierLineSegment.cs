// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Paths
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    //code for this taken from http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/

    public class BezierLineSegment : ILineSegment
    {
        private const int SEGMENTS_PER_CURVE = 100;
        private const float MINIMUM_SQR_DISTANCE = 0.01f;

        // This corresponds to about 172 degrees, 8 degrees from a traight line
        private const float DIVISION_THRESHOLD = -0.99f;

        private List<Vector2> linePoints;

        private int curveCount; //how many bezier curves in this path?

        public BezierLineSegment(IEnumerable<PointF> points)
            : this(points?.Select(x => x.ToVector2()).ToArray())
        {
        }

        public BezierLineSegment(IEnumerable<Point> points)
            : this(points?.Select(x => x.ToVector2()).ToArray())
        {
        }

        public BezierLineSegment(params PointF[] points)
            : this(points?.Select(x => x.ToVector2()).ToArray())
        {
        }

        internal BezierLineSegment(Vector2 start, Vector2 end)
            : this(new[] { start, end })
        {
        }

        internal BezierLineSegment(Vector2[] points)
        {
            Guard.NotNull(points, nameof(points));
            Guard.MustBeGreaterThanOrEqualTo(points.Count(), 4, nameof(points));

            this.curveCount = (points.Length - 1) / 3;
            this.linePoints = GetDrawingPoints(points);
        }

        public List<Vector2> GetDrawingPoints(Vector2[] controlPoints)
        {

            var maxPoints = (int)Math.Ceiling(SEGMENTS_PER_CURVE * (float)this.curveCount);
            List<Vector2> drawingPoints = new List<Vector2>(maxPoints); //set a default size to be efficient?

            var targetPoint = controlPoints.Length - 3;
            for (int i = 0; i < targetPoint; i += 3)
            {
                Vector2 p0 = controlPoints[i];
                Vector2 p1 = controlPoints[i + 1];
                Vector2 p2 = controlPoints[i + 2];
                Vector2 p3 = controlPoints[i + 3];

                if (i == 0) //only do this for the first end point. When i != 0, this coincides with the end point of the previous segment,
                {
                    drawingPoints.Add(CalculateBezierPoint(0, p0, p1, p2, p3));
                }

                for (int j = 1; j <= SEGMENTS_PER_CURVE; j++)
                {
                    float t = j / (float)SEGMENTS_PER_CURVE;
                    drawingPoints.Add(CalculateBezierPoint(t, p0, p1, p2, p3));
                }
            }

            return drawingPoints;
        }

        private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; //first term

            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }
        
        public IEnumerable<Vector2> AsSimpleLinearPath()
        {
            return linePoints;
        }
    }
}

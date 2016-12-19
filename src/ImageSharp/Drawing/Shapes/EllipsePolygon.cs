// <copyright file="EllipsePolygon.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Drawing.Shapes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using Paths;

    /// <summary>
    /// A way of optermising drawing rectangles.
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.Shapes.IShape" />
    public class EllipsePolygon : IShape, IPath
    {
        private readonly RectangleF rectangle;
        private readonly Vector2 topLeft;
        private readonly Vector2 bottomRight;
        private readonly Vector2[] points;
        private readonly IEnumerable<IPath> pathCollection;
        private readonly float halfLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="EllipsePolygon" /> class.
        /// </summary>
        /// <param name="ellipse">The ellipse.</param>
        public EllipsePolygon(ImageSharp.Ellipse ellipse)
        {
            this.rectangle = new RectangleF(
                    ellipse.X - ellipse.RadiusX,
                    ellipse.Y - ellipse.RadiusY,
                    ellipse.RadiusX * 2,
                    ellipse.RadiusY * 2);


            var axies = new Vector2(Math.Max(ellipse.RadiusX, ellipse.RadiusY));
            var axieTimes3 = axies * 3;

            this.Length = (float)(Math.PI * ((axieTimes3.X + axieTimes3.Y) - Math.Sqrt((axies.X + axieTimes3.Y) * (axies.Y + axieTimes3.X))));

            pathCollection = new[] { this };

            // use to figure out the linear path (might help with other too)
            // http://www.mathopenref.com/coordcirclealgorithm.html
        }

        /// <summary>
        /// Gets the bounding box of this shape.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public RectangleF Bounds => rectangle;

        /// <summary>
        /// Gets a value indicating whether this instance is closed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is closed; otherwise, <c>false</c>.
        /// </value>
        public bool IsClosed => true;

        /// <summary>
        /// Gets the length of the path
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public float Length
        {
            get;
        }

        private PointInfo Distance(Vector2 point, bool getDistanceAwayOnly, out bool isInside)
        {
            // point in rectangle
            // if after its clamped by the extreams its still the same then it must be inside :)
            var clamped = Vector2.Clamp(point, topLeft, bottomRight);
            isInside = (clamped == point);

            //get the absolute distances from the extreams
            var topLeftDist = Vector2.Abs(point - topLeft);
            var bottomRightDist = Vector2.Abs(point - topLeft);

            //get the min components
            var minDists = Vector2.Min(topLeftDist, bottomRightDist);
            //and then the single smallest (dont have to worry about 
            var distanceFromEdge = Math.Min(minDists.X, minDists.Y);

            if (isInside)
            {
                if (!getDistanceAwayOnly)
                {
                    //we need to make clamped the closest point
                    if (topLeft.X + distanceFromEdge == point.X)
                    {
                        // closer to lhf
                        clamped.X = topLeft.X; // y is already the same
                    }
                    else if (topLeft.Y + distanceFromEdge == point.Y)
                    {
                        // closer to top
                        clamped.Y = topLeft.Y; // x is already the same
                    }
                    else if (bottomRight.Y - distanceFromEdge == point.Y)
                    {
                        // closer to bottom
                        clamped.Y = bottomRight.Y; // x is already the same
                    }
                    else if (bottomRight.X - distanceFromEdge == point.X)
                    {
                        // closer to rhs
                        clamped.X = bottomRight.X; // x is already the same
                    }
                }
            }
            else
            {
                // clamped is the point on the path thats closest no matter what
                distanceFromEdge = (clamped - point).Length();
            }

            var distanceAlongEdge = 0f;
            if (!getDistanceAwayOnly)
            {
                // we need to figure out whats the cloests edge now and thus what distance/poitn is closest
                if (topLeft.X == clamped.X)
                {
                    // distance along edge is length minus the amout down we are from the top of the rect
                    distanceAlongEdge = this.Length - (clamped.Y - topLeft.Y);
                }
                else if (topLeft.Y == clamped.Y)
                {
                    distanceAlongEdge = clamped.X - topLeft.X;
                }
                else if (bottomRight.Y == clamped.Y)
                {
                    distanceAlongEdge = (bottomRight.X - clamped.X) + this.halfLength;
                }
                else if (bottomRight.X == clamped.X)
                {
                    distanceAlongEdge = (bottomRight.Y - clamped.Y) + rectangle.Width;
                }
            }
            return new PointInfo
            {
                SearchPoint = point,
                DistanceFromPath = distanceFromEdge,
                ClosestPointOnPath = clamped,
                DistanceAlongPath = distanceAlongEdge
            };
        }

        PointInfo IPath.Distance(Vector2 point)
        {
            bool tmp;// dont care about inside/outside for paths just distance
            return Distance(point, false, out tmp);
        }

        /// <summary>
        /// the distance of the point from the outline of the shape, if the value is negative it is inside the polygon bounds
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        /// Returns the distance from the shape to the point
        /// </returns>
        public float Distance(Vector2 point)
        {
            bool insidePoly;
            var result = Distance(point, true, out insidePoly);
            // invert the distance from path when inside
            return insidePoly ? -result.DistanceFromPath : result.DistanceFromPath;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IPath> GetEnumerator()
        {
            return pathCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return pathCollection.GetEnumerator();
        }

        /// <summary>
        /// Converts the <see cref="ILineSegment" /> into a simple linear path..
        /// </summary>
        /// <returns>
        /// Returns the current <see cref="ILineSegment" /> as simple linear path.
        /// </returns>
        public Vector2[] AsSimpleLinearPath()
        {
            return points;
        }
    }
}
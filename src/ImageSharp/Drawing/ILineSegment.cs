
namespace ImageSharp.Drawing
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public interface ILineSegment : IPath
    {

        PointF Start { get; }

        PointF End { get; }

        IEnumerable<PointF> CrossingPoints(PointF start, PointF end);

        /// <summary>
        /// the distance of the point from the outline of the shape, if the value is negative it is inside the polygon bounds
        /// </summary>
        /// <param name="point">The point</param>
        /// <returns></returns>
        float Distance(PointF point);
    }
}

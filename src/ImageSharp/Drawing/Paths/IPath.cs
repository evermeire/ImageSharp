

namespace ImageSharp.Drawing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    /// <summary>
    /// Represents a logic path that can be drawn
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{ImageSharp.Drawing.ILineSegment}" />
    public interface IPath : ILineSegment
    {
        RectangleF Bounds { get; }

        bool IsClosed { get; }

        float Length { get; }
        
        /// <summary>
        /// a point on the path <paramref name="distance"/> pixels along the path
        /// </summary>
        /// <param name="distance">The point</param>
        /// <returns></returns>
        PointInfo Distance(int x, int y);
    }
}

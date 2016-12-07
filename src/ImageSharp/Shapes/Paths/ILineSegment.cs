
namespace ImageSharp.Shapes
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ILineSegment
    {
        /// <summary>
        /// Simplifies the specified quality.
        /// </summary>
        /// <param name="quality">The quality.</param>
        /// <returns></returns>
        IEnumerable<Point> Simplify();
    }
}


namespace ImageSharp.Drawing
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public interface ILineSegment
    {
        /// <summary>
        /// Simplifies the specified quality.
        /// </summary>
        /// <param name="quality">The quality.</param>
        /// <returns></returns>
        IEnumerable<Vector2> Simplify();
    }
}

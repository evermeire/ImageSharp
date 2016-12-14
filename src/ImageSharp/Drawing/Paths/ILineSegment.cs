
namespace ImageSharp.Drawing.Paths
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;

    public interface ILineSegment
    {
        /// <summary>
        /// Returns trhe current <see cref="ILineSegment"/> implmenetation as a simple linear path.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Vector2> AsSimpleLinearPath();
    }
}

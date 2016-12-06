using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp.Brushs;

namespace ImageSharp.Brushs
{
    public interface ILineSegment
    {
        /// <summary>
        /// Simplifies the specified quality.
        /// </summary>
        /// <param name="quality">The quality.</param>
        /// <returns></returns>
        IEnumerable<SimpleLineSegment> Simplify(float quality);
    }
}

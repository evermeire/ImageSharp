using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageSharp.Drawing.Pens.Processors
{
    /// <summary>
    /// Returns some meta data about the nearest point on a path from a vector 
    /// </summary>
    public struct ColoredPointInfo<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        public TColor Color;
        
        public float DistanceFromElement;
    }
}

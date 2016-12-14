using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageSharp.Drawing.Paths
{
    /// <summary>
    /// Returns some meta data about the nearest point on a path from a vector 
    /// </summary>
    public struct PointInfo
    {
        public Vector2 Point;

        public float DistanceAlongPath;

        public float DistanceFromPath;
    }
}

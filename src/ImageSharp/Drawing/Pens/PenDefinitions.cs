using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSharp.Drawing
{
    public partial class Pen
    {
        public static Pen Solid(Color color, float width) => new Pen(color, width);
        public static Pen Solid(IBrush<Color, uint> brush, float width) => new Pen(brush, width);

        private static readonly float[] dashedPattern = new[] { 3f, 1f };
        public static Pen Dash(Color color, float width) => new Pen(color, width, dashedPattern);
        public static Pen Dash(IBrush<Color, uint> brush, float width) => new Pen(brush, width, dashedPattern);


        private static readonly float[] dottedPattern = new[] { 1f, 1f };
        public static Pen Dot(Color color, float width) => new Pen(color, width, dottedPattern);
        public static Pen Dot(IBrush<Color, uint> brush, float width) => new Pen(brush, width, dottedPattern);



        private static readonly float[] dashDotPattern = new[] { 3f, 1f, 1f, 1f };
        public static Pen DashDot(Color color, float width) => new Pen(color, width, dashDotPattern);
        public static Pen DashDot(IBrush<Color, uint> brush, float width) => new Pen(brush, width, dashDotPattern);


        private static readonly float[] dashDotDotPattern = new[] { 3f, 1f, 1f, 1f, 1f, 1f };
        public static Pen DashDotDot(Color color, float width) => new Pen(color, width, dashDotDotPattern);
        public static Pen DashDotDot(IBrush<Color, uint> brush, float width) => new Pen(brush, width, dashDotDotPattern);
    }
}

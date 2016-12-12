using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSharp.Drawing.Processing
{
    public abstract class BrushApplicatorBase<TColor, TPacked> : IBrushApplicator<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {
        public virtual bool RequiresComposition
        {
            get
            {
                return false;
            }
        }

        public abstract TColor GetColor(int x, int y);

        public virtual TColor[] GetColor(int startX, int endX, int Y)
        {
            var result = new TColor[endX - startX + 1];
            for(var x = startX; x<=endX; x++)
            {
                result[x - startX] = GetColor(x, Y);
            }
            return result;
        }

        public virtual TColor[,] GetColor(int startX, int startY, int endX, int endY)
        {
            var maxX = endX - startX;
            var maxY = endY - startY;
            var colors = new TColor[maxX + 1, maxY + 1];
            for (var x = 0; x <= maxX; x++)
            {
                for (var y = 0; y <= maxY; y++)
                {
                    colors[x, y] = GetColor(x + startX, y + startY);
                }
            }

            return colors;
        }
    }
}

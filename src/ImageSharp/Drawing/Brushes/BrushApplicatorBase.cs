using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSharp.Drawing.Processing
{
    public abstract class BrushApplicatorBase : IBrushApplicator
    {
        public virtual bool RequiresComposition
        {
            get
            {
                return false;
            }
        }

        public abstract Color GetColor(int x, int y);

        public virtual Color[] GetColor(int startX, int endX, int Y)
        {
            var result = new Color[endX - startX + 1];
            for(var x = startX; x<=endX; x++)
            {
                result[x - startX] = GetColor(x, Y);
            }
            return result;
        }

        public virtual Color[,] GetColor(int startX, int startY, int endX, int endY)
        {
            var maxX = endX - startX;
            var maxY = endY - startY;
            var colors = new Color[maxX + 1, maxY + 1];
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

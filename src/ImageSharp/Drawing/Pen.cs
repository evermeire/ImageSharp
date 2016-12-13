using System;
using System.Linq;
using ImageSharp.Drawing.Processing;

namespace ImageSharp.Drawing
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.Pen{ImageSharp.Color, System.UInt32}" />
    public class Pen : Pen<Color, uint>
    {
        public Pen(Color color, float width, float[] pattern) : base(color, width, pattern) { }
        public Pen(Color color, float width) : base(color, width) { }
    }

    public class Pen<TColor, TPacked> : IPen<TColor, TPacked>
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
    {
        private readonly float[] pattern;

        public Pen(TColor color, float width, float[] pattern)
            :this(new SolidBrush<TColor, TPacked>(color), width, pattern)
        {
        }

        public Pen(IBrush<TColor, TPacked> brush, float width, float[] pattern)
        {
            this.Brush = brush;
            this.Width = width;
            this.pattern = pattern;
        }

        public Pen(TColor color, float width)
           : this(new SolidBrush<TColor, TPacked>(color), width)
        {
        }

        public Pen(IBrush<TColor, TPacked> brush, float width)
            :this(brush, width, new[] { 1f })
        {
        }

        public IBrush<TColor, TPacked> Brush { get; }

        public float Width { get; }
        

        private class PenApplicator : IPenApplicator<TColor, TPacked>
        {
            private readonly IBrushApplicator<TColor, TPacked> brush;
            private readonly float halfWidth;
            private readonly float[] pattern;
            private readonly float totalLength;

            public PenApplicator(IBrush<TColor, TPacked> brush, RectangleF region, float width, float[] pattern)
            {
                this.brush = brush.CreateApplicator(region);
                this.halfWidth = width /2;
                this.totalLength = 0;


                this.pattern = new float[pattern.Length+1];
                this.pattern[0] = 0;
                for (var i = 0; i < pattern.Length; i++)
                {
                    totalLength += pattern[i] * width;
                    this.pattern[i+1] = totalLength;
                }
                RequiredRegion = RectangleF.Outset(region, width);
            }

            public RectangleF RequiredRegion
            {
                get;
            }

            public ColoredPointInfo<TColor, TPacked> GetColor(PointInfo info)
            {
                var infoResult = default(ColoredPointInfo<TColor, TPacked>);
                infoResult.DistanceFromElement = float.MaxValue; //is really outside the element

                var length = info.DistanceAlongPath % totalLength;

                // we can treat the DistanceAlongPath and DistanceFromPath as x,y coords for the pattern
                // we need to calcualte the distance from the outside edge of the pattern
                // and set them on the ColoredPointInfo<TColor, TPacked> along with the color


                //we can sepcial case this as a solid pen
                infoResult.Color = brush.GetColor(info.Point);

                float maxDistance = 0;

                if(info.DistanceFromPath < halfWidth)
                {
                    //inside strip
                    maxDistance = 0;
                }
                else
                {
                    maxDistance = info.DistanceFromPath - halfWidth; 
                }

                for (var i = 0; i< pattern.Length -1; i++)
                {
                    var start = pattern[i];
                    var end = pattern[i+1];

                    if (length >= start && length < end)
                    {
                        
                        //in section

                        if(i % 2 == 0)
                        {

                            //solid part return the maxDistance 
                         
                            infoResult.DistanceFromElement = maxDistance;
                            return infoResult;
                        }
                        else
                        {
                            //this is a none solid part
                            var closestEdge = Math.Min(length - start, end - length);                            
                            infoResult.DistanceFromElement = Math.Max(maxDistance, closestEdge);
                            return infoResult;
                        }
                    }
                }

                return infoResult;
            }
        }

        public IPenApplicator<TColor, TPacked> CreateApplicator(RectangleF region)
        {
            return new PenApplicator(this.Brush, region, Width, pattern);
        }
    }
}

namespace ImageSharp.Drawing
{

    public class Pen<TColor, TPacked> : IPen<TColor, TPacked>
        where TColor : struct, IPackedPixel<TPacked>
        where TPacked : struct
    {

        public Pen(TColor color, float width)
            :this(new SolidBrush<TColor, TPacked>(color), width)
        {

        }

        public Pen(IBrush<TColor, TPacked> brush, float width)
        {
            this.Brush = brush;
            this.Width = width;
        }

        public IBrush<TColor, TPacked> Brush { get; }

        public float Width { get; }
        
    }
}

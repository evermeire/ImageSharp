namespace ImageSharp.Drawing
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ImageSharp.Drawing.Pen{ImageSharp.Color, System.UInt32}" />
    public class Pen : Pen<Color, uint>
    {
        public Pen(Color color, float width) : base(color, width) { }
    }

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

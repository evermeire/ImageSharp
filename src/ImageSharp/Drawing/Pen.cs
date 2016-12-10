namespace ImageSharp.Drawing
{

    public class Pen : IPen
    {

        public Pen(Color color, float width)
            :this(new SolidBrush(color), width)
        {

        }

        public Pen(IBrush brush, float width)
        {
            this.Brush = brush;
            this.Width = width;
        }

        public IBrush Brush { get; }

        public float Width { get; }
        
    }
}

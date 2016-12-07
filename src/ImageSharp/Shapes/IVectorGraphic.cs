

namespace ImageSharp.Shapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public interface IVectorGraphic
    {
        void Apply<TColor, TPacked>(IImageBase<TColor, TPacked> source)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct;
    }
}

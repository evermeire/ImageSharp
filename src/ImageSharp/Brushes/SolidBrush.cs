// <copyright file="IImageSampler.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Brushes
{
    using Shapes;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;

    /// <summary>
    /// A brush representing a Solid color fill
    /// </summary>
    /// <seealso cref="ImageSharp.Brushs.IBrush" />
    public class SolidBrush: IBrush
    {
        private readonly Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public SolidBrush(Color color)
        {
            this.color = color;
        }

        Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public TColor GetColor<TColor, TPacked>(PixelAccessor<TColor, TPacked> source, int x, int y)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            // this is a solid color we are going to ignore the X,Y 
            // we pass the source incase we need to deal with opacity
            if (color.A == 255)
            {


                var type = typeof(TColor);
                if (_cache.ContainsKey(type))
                {
                    return (TColor)_cache[type];
                }

                Vector4 backgroundColor = color.ToVector4();
                TColor packed = default(TColor);
                packed.PackFromVector4(backgroundColor);
                lock (_cache)
                {
                    if (_cache.ContainsKey(type))
                    {
                        return (TColor)_cache[type];
                    }
                    _cache.Add(type, packed);
                }

                return packed;
            }
            else
            {
                Vector4 backgroundColor = color.ToVector4();
                TColor packed = default(TColor);
                packed.PackFromVector4(backgroundColor);

                Vector4 currentColor = source[x, y].ToVector4();

                currentColor = Vector4.Lerp(currentColor, backgroundColor, (color.A / (float)255));

                TColor newPacked = default(TColor);
                newPacked.PackFromVector4(currentColor);
                return newPacked;
            }
        }
    }
}

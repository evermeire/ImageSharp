using ImageSharp.Drawing.Shapes;
using NOpenType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ImageSharp.Drawing
{
    /// <summary>
    /// 
    /// </summary>
    public class Font
    {
        private readonly Typeface typeface;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        public Font(Stream fontStream)
        {
            this.typeface = OpenTypeReader.Read(fontStream);
        }

        public Font(Font prototype)
        {
            //clone out the setting in here
            this.typeface = prototype.typeface;
            this.Size = prototype.Size;
            this.EnableKerning = prototype.EnableKerning;
        }

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily => typeface.Name;

        /// <summary>
        /// Gets the font veriant.
        /// </summary>
        /// <value>
        /// The font veriant.
        /// </value>
        public string FontVeriant => typeface.FontSubFamily;

        /// <summary>
        /// Gets or sets the size. This defaults to 10.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public float Size { get; set; } = 10; // as good a size any for a defaut size.
        public float LineHeight { get; set; } = 1.5f; // as good a size any for a defaut size.

        public float TabWidth { get; set; } = 4; // as good a size any for a defaut size.


        /// <summary>
        /// Gets or sets a value indicating whether to enable kerning. This defaults to true.
        /// </summary>
        /// <value>
        /// <c>true</c> if kerning is enabled otherwise <c>false</c>.
        /// </value>
        public bool EnableKerning { get; set; } = true;

        public IShape[] GenerateShapes(string str, Vector2 origin)
        {

            // TODO add support for clipping (complex polygons should help here)
            // TODO add support for wrapping (line heights)

            var initialX = origin.X; // store this for line changes
            var glyphs = new IShape[str.Length];
            
            var glyphPathBuilder = new GlyphPathBuilderPolygons(typeface);

            float scale = typeface.CalculateScale(Size);

            bool enable_kerning = this.EnableKerning;
            ushort prevIdx = 0;
            var j = str.Length;
            float finalLineHeight = LineHeight * Size;
            bool startOfLine = true;

            var spaceIndex = (ushort)typeface.LookupIndex(' ');
            var spaceWidth = typeface.GetAdvanceWidthFromGlyphIndex(spaceIndex) * scale;

            for (int i = 0; i < j; ++i)
            {
                char c = str[i];
                bool doKerning = (enable_kerning && !startOfLine);
                startOfLine = false;

                switch (c)
                {
                    case '\n':
                        origin.Y += finalLineHeight;
                        origin.X = initialX;
                        startOfLine = true;
                        break;
                    case '\r':
                        // ignore '\r's
                        break;
                    case ' ':
                        origin.X += spaceWidth;
                        prevIdx = spaceIndex;
                        break;
                    case '\t':
                        origin.X += spaceWidth * TabWidth;
                        prevIdx = spaceIndex;
                        break;
                    default:
                        var glyIndex = (ushort)typeface.LookupIndex(c);
                        glyphs[i] = glyphPathBuilder.BuildGlyph(glyIndex, Size, origin);

                        //this advWidth in font design unit 
                        float advWidth = typeface.GetAdvanceWidthFromGlyphIndex(glyIndex) * scale;
                        //---------------------------------- 

                        if (doKerning)
                        {
                            //check kerning
                            advWidth += typeface.GetKernDistance(prevIdx, glyIndex) * scale;
                        }
                        origin.X += advWidth;
                        prevIdx = glyIndex;
                        break;
                }
            }

            return glyphs;
        }
    }
}
﻿//Apache2, 2016,  WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NOpenType.Tables
{

    partial class GSUB : TableEntry
    {
        internal abstract class LookupSubTable
        {
            public int Format
            {
                get;
                protected set;
            }
        }


        /// <summary>
        ///  for lookup table type 1, format1
        /// </summary>
        class LkSubTableT1Fmt1 : LookupSubTable
        {
            public LkSubTableT1Fmt1(short coverageOffset, short deltaGlyph)
            {
                this.Format = 1;
                this.CoverateOffset = coverageOffset;
                this.DeltaGlyph = deltaGlyph;
            }
            public short CoverateOffset { get; set; }
            /// <summary>
            /// Add to original GlyphID to get substitute GlyphID
            /// </summary>
            public short DeltaGlyph
            {
                //format1
                get;
                private set;
            }
            public CoverageTable CoverageTable
            {
                get;
                set;
            }
        }
        /// <summary>
        /// for lookup table type 1, format2
        /// </summary>
        class LkSubTableT1Fmt2 : LookupSubTable
        {
            public LkSubTableT1Fmt2(short coverageOffset, ushort[] substitueGlyphs)
            {
                this.Format = 2;
                this.CoverageOffset = coverageOffset;
                this.SubstitueGlyphs = substitueGlyphs;
            }
            public short CoverageOffset { get; set; }
            /// <summary>
            /// It provides an array of output glyph indices (Substitute) explicitly matched to the input glyph indices specified in the Coverage table
            /// </summary>
            public ushort[] SubstitueGlyphs
            {
                get;
                private set;
            }
            public CoverageTable CoverageTable
            {
                get;
                set;
            }
        }



    }
}
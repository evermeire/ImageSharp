﻿//Apache2,  2016,  WinterDev
//https://www.microsoft.com/typography/otspec/base.htm
//BASE - Baseline Table
//The Baseline table (BASE) provides information used to align glyphs of different scripts and sizes in a line of text, 
//whether the glyphs are in the same font or in different fonts.
//To improve text layout, the Baseline table also provides minimum (min) and maximum (max) glyph extent values for each script,
//language system, or feature in a font.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NOpenType.Tables
{

    class BASE : TableEntry
    {
        long baseTableStartAt;
        public override string Name
        {
            get { return "BASE"; }
        }
        protected override void ReadContentFrom(BinaryReader reader)
        {

        }
    }
}
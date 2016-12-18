﻿//Apache2,  2016,  WinterDev  
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NOpenType.Tables
{
    class VerticalDeviceMatrics : TableEntry
    {
        //https://www.microsoft.com/typography/otspec/vdmx.htm
        //VDMX - Vertical Device Metrics 
        //The VDMX table relates to OpenType™ fonts with TrueType outlines.
        //Under Windows, the usWinAscent and usWinDescent values from the 'OS/2' table
        //will be used to determine the maximum black height for a font at any given size.
        //Windows calls this distance the Font Height.
        //Because TrueType instructions can lead to Font Heights that differ from the actual scaled and rounded values,
        //basing the Font Height strictly on the yMax and yMin can result in “lost pixels.” 
        //Windows will clip any pixels that extend above the yMax or below the yMin. 
        //In order to avoid grid fitting the entire font to determine the correct height, the VDMX table has been defined.

        //The VDMX table consists of a header followed by groupings of VDMX records:
        Ratio[] ratios;
        public override string Name
        {
            get { return "VDMX"; }
        }
        protected override void ReadContentFrom(BinaryReader reader)
        {
            //USHORT 	version 	Version number (0 or 1).
            //USHORT 	numRecs 	Number of VDMX groups present
            //USHORT 	numRatios 	Number of aspect ratio groupings
            //Ratio 	ratRange[numRatios] 	Ratio ranges (see below for more info)
            //USHORT 	offset[numRatios] 	Offset from start of this table to the VDMX group for this ratio range.
            //Vdmx 	groups 	The actual VDMX groupings (documented below)
            //Ratio Record Type 	Name 	Description
            //BYTE 	bCharSet 	Character set (see below).
            //BYTE 	xRatio 	Value to use for x-Ratio
            //BYTE 	yStartRatio 	Starting y-Ratio value.
            //BYTE 	yEndRatio 	Ending y-Ratio value.
            ushort version = reader.ReadUInt16();
            ushort numRecs = reader.ReadUInt16();
            ushort numRatios = reader.ReadUInt16();
            ratios = new Ratio[numRatios];
            for (int i = 0; i < numRatios; ++i)
            {
                ratios[i] = new Ratio(
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte());
            }
            short[] offsets = Utils.ReadInt16Array(reader, numRatios);
            //------
            //actual vdmx group
            //TODO: implement this
        }
        struct Ratio
        {
            public readonly byte charset;
            public readonly byte xRatio;
            public readonly byte yStartRatio;
            public readonly byte yEndRatio;
            public Ratio(byte charset, byte xRatio, byte yStartRatio, byte yEndRatio)
            {
                this.charset = charset;
                this.xRatio = xRatio;
                this.yStartRatio = yStartRatio;
                this.yEndRatio = yEndRatio;
            }
            // BYTE 	bCharSet 	Character set (see below).
            //BYTE 	xRatio 	Value to use for x-Ratio
            //BYTE 	yStartRatio 	Starting y-Ratio value.
            //BYTE 	yEndRatio 	Ending y-Ratio value.
        }
    }


}

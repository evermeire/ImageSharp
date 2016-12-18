﻿//Apache2, 2016, WinterDev

using System.IO;
using System.Text;
namespace NOpenType.Tables
{

    //https://www.microsoft.com/typography/otspec/os2.htm
    /// <summary>
    /// OS2 and Windows matrics
    /// </summary>
    class OS2Table : TableEntry
    {
        //USHORT 	version 	0x0005
        //SHORT 	xAvgCharWidth 	 
        //USHORT 	usWeightClass 	 
        //USHORT 	usWidthClass 	 
        //USHORT 	fsType 	 
        public ushort version;
        public short xAvgCharWidth;
        public ushort usWeightClass;
        public ushort usWidthClass;
        public ushort fsType;
        //SHORT 	ySubscriptXSize 	 
        //SHORT 	ySubscriptYSize 	 
        //SHORT 	ySubscriptXOffset 	 
        //SHORT 	ySubscriptYOffset 	 
        //SHORT 	ySuperscriptXSize 	 
        //SHORT 	ySuperscriptYSize 	 
        //SHORT 	ySuperscriptXOffset 	 
        //SHORT 	ySuperscriptYOffset 	 
        //SHORT 	yStrikeoutSize 	 
        //SHORT 	yStrikeoutPosition 	 
        //SHORT 	sFamilyClass 	 
        public short ySubscriptXSize;
        public short ySubscriptYSize;
        public short ySubscriptXOffset;
        public short ySubscriptYOffset;
        public short ySuperscriptXSize;
        public short ySuperscriptYSize;
        public short ySuperscriptXOffset;
        public short ySuperscriptYOffset;
        public short yStrikeoutSize;
        public short yStrikeoutPosition;
        public short sFamilyClass;

        //BYTE 	panose[10] 	 
        public byte[] panose;
        //ULONG 	ulUnicodeRange1 	Bits 0-31
        //ULONG 	ulUnicodeRange2 	Bits 32-63
        //ULONG 	ulUnicodeRange3 	Bits 64-95
        //ULONG 	ulUnicodeRange4 	Bits 96-127
        public uint ulUnicodeRange1;
        public uint ulUnicodeRange2;
        public uint ulUnicodeRange3;
        public uint ulUnicodeRange4;

        //CHAR 	achVendID[4] 	 
        public uint achVendID;
        //USHORT 	fsSelection 	 
        //USHORT 	usFirstCharIndex 	 
        //USHORT 	usLastCharIndex 
        public ushort fsSelection;
        public ushort usFirstCharIndex;
        public ushort usLastCharIndex;
        //SHORT 	sTypoAscender 	 
        //SHORT 	sTypoDescender 	 
        //SHORT 	sTypoLineGap 	 
        public short sTypoAscender;
        public short sTypoDescender;
        public short sTypoLineGap;
        //USHORT 	usWinAscent 	 
        //USHORT 	usWinDescent 	 
        //ULONG 	ulCodePageRange1 	Bits 0-31
        //ULONG 	ulCodePageRange2 	Bits 32-63
        public ushort usWinAscent;
        public ushort usWinDescent;
        public uint ulCodePageRange1;
        public uint ulCodePageRange2;
        //SHORT 	sxHeight 	 
        //SHORT 	sCapHeight 	 
        //USHORT 	usDefaultChar 	 
        //USHORT 	usBreakChar 	 
        //USHORT 	usMaxContext 	 
        //USHORT 	usLowerOpticalPointSize 	 
        //USHORT 	usUpperOpticalPointSize 	 
        public short sxHeight;
        public short sCapHeight;
        public ushort usDefaultChar;
        public ushort usBreakChar;
        public ushort usMaxContext;
        public ushort usLowerOpticalPointSize;
        public ushort usUpperOpticalPointSize;

        public override string Name
        {
            get { return "OS/2"; }
        }

        protected override void ReadContentFrom(BinaryReader reader)
        {
            //USHORT 	version 	0x0005
            //SHORT 	xAvgCharWidth 	 
            //USHORT 	usWeightClass 	 
            //USHORT 	usWidthClass 	 
            //USHORT 	fsType 	  
            switch (this.version = reader.ReadUInt16())
            {
                default: throw new System.NotSupportedException();
                case 0:
                    ReadVersion0(reader);
                    break;
                case 1:
                    ReadVersion1(reader);
                    break;
                case 2:
                    ReadVersion2(reader);
                    break;
                case 3:
                    ReadVersion3(reader);
                    break;
                case 4:
                    ReadVersion4(reader);
                    break;
                case 5:
                    ReadVersion5(reader);
                    break;
            }
        }
        void ReadVersion0(BinaryReader reader)
        {
            //https://www.microsoft.com/typography/otspec/os2ver0.htm
            //USHORT 	version 	0x0000
            //SHORT 	xAvgCharWidth 	 
            //USHORT 	usWeightClass 	 
            //USHORT 	usWidthClass 	 
            //USHORT 	fsType 	 
            this.xAvgCharWidth = reader.ReadInt16();
            this.usWeightClass = reader.ReadUInt16();
            this.usWidthClass = reader.ReadUInt16();
            this.fsType = reader.ReadUInt16();

            //SHORT 	ySubscriptXSize 	 
            //SHORT 	ySubscriptYSize 	 
            //SHORT 	ySubscriptXOffset 	 
            //SHORT 	ySubscriptYOffset 	 
            //SHORT 	ySuperscriptXSize 	 
            //SHORT 	ySuperscriptYSize 	 
            //SHORT 	ySuperscriptXOffset 	 
            //SHORT 	ySuperscriptYOffset 	 
            //SHORT 	yStrikeoutSize 	 
            //SHORT 	yStrikeoutPosition 	 
            //SHORT 	sFamilyClass 	 
            this.ySubscriptXSize = reader.ReadInt16();
            this.ySubscriptYSize = reader.ReadInt16();
            this.ySubscriptXOffset = reader.ReadInt16();
            this.ySubscriptYOffset = reader.ReadInt16();
            this.ySuperscriptXSize = reader.ReadInt16();
            this.ySuperscriptYSize = reader.ReadInt16();
            this.ySuperscriptXOffset = reader.ReadInt16();
            this.ySuperscriptYOffset = reader.ReadInt16();
            this.yStrikeoutSize = reader.ReadInt16();
            this.yStrikeoutPosition = reader.ReadInt16();
            this.sFamilyClass = reader.ReadInt16();
            //BYTE 	panose[10] 	 
            this.panose = reader.ReadBytes(10);
            //ULONG 	ulCharRange[4] 	Bits 0-31
            this.ulUnicodeRange1 = reader.ReadUInt32();
            //CHAR 	achVendID[4] 	 
            this.achVendID = reader.ReadUInt32();
            //USHORT 	fsSelection 	 
            //USHORT 	usFirstCharIndex 	 
            //USHORT 	usLastCharIndex 	 
            this.fsSelection = reader.ReadUInt16();
            this.usFirstCharIndex = reader.ReadUInt16();
            this.usLastCharIndex = reader.ReadUInt16();
            //SHORT 	sTypoAscender 	 
            //SHORT 	sTypoDescender 	 
            //SHORT 	sTypoLineGap 	 
            this.sTypoAscender = reader.ReadInt16();
            this.sTypoDescender = reader.ReadInt16();
            this.sTypoLineGap = reader.ReadInt16();
            //USHORT 	usWinAscent 	 
            //USHORT 	usWinDescent
            this.usWinAscent = reader.ReadUInt16();
            this.usWinDescent = reader.ReadUInt16();
        }
#if DEBUG
        public override string ToString()
        {
            return version + "," + Utils.TagToString(this.achVendID);
        }
#endif
        void ReadVersion1(BinaryReader reader)
        {
            //https://www.microsoft.com/typography/otspec/os2ver1.htm

            //SHORT 	xAvgCharWidth 	 
            //USHORT 	usWeightClass 	 
            //USHORT 	usWidthClass 	 
            //USHORT 	fsType 	 
            this.xAvgCharWidth = reader.ReadInt16();
            this.usWeightClass = reader.ReadUInt16();
            this.usWidthClass = reader.ReadUInt16();
            this.fsType = reader.ReadUInt16();
            //SHORT 	ySubscriptXSize 	 
            //SHORT 	ySubscriptYSize 	 
            //SHORT 	ySubscriptXOffset 	 
            //SHORT 	ySubscriptYOffset 	 
            //SHORT 	ySuperscriptXSize 	 
            //SHORT 	ySuperscriptYSize 	 
            //SHORT 	ySuperscriptXOffset 	 
            //SHORT 	ySuperscriptYOffset 	 
            //SHORT 	yStrikeoutSize 	 
            //SHORT 	yStrikeoutPosition 	 
            //SHORT 	sFamilyClass 	 
            this.ySubscriptXSize = reader.ReadInt16();
            this.ySubscriptYSize = reader.ReadInt16();
            this.ySubscriptXOffset = reader.ReadInt16();
            this.ySubscriptYOffset = reader.ReadInt16();
            this.ySuperscriptXSize = reader.ReadInt16();
            this.ySuperscriptYSize = reader.ReadInt16();
            this.ySuperscriptXOffset = reader.ReadInt16();
            this.ySuperscriptYOffset = reader.ReadInt16();
            this.yStrikeoutSize = reader.ReadInt16();
            this.yStrikeoutPosition = reader.ReadInt16();
            this.sFamilyClass = reader.ReadInt16();

            //BYTE 	panose[10] 	 
            this.panose = reader.ReadBytes(10);
            //ULONG 	ulUnicodeRange1 	Bits 0-31
            //ULONG 	ulUnicodeRange2 	Bits 32-63
            //ULONG 	ulUnicodeRange3 	Bits 64-95
            //ULONG 	ulUnicodeRange4 	Bits 96-127
            this.ulUnicodeRange1 = reader.ReadUInt32();
            this.ulUnicodeRange2 = reader.ReadUInt32();
            this.ulUnicodeRange3 = reader.ReadUInt32();
            this.ulUnicodeRange4 = reader.ReadUInt32();
            //CHAR 	achVendID[4] 	 
            this.achVendID = reader.ReadUInt32();
            //USHORT 	fsSelection 	 
            //USHORT 	usFirstCharIndex 	 
            //USHORT 	usLastCharIndex 	
            this.fsSelection = reader.ReadUInt16();
            this.usFirstCharIndex = reader.ReadUInt16();
            this.usLastCharIndex = reader.ReadUInt16();
            //SHORT 	sTypoAscender 	 
            //SHORT 	sTypoDescender 	 
            //SHORT 	sTypoLineGap 	 
            this.sTypoAscender = reader.ReadInt16();
            this.sTypoDescender = reader.ReadInt16();
            this.sTypoLineGap = reader.ReadInt16();
            //USHORT 	usWinAscent 	 
            //USHORT 	usWinDescent 	 
            //ULONG 	ulCodePageRange1 	Bits 0-31
            //ULONG 	ulCodePageRange2 	Bits 32-63
            this.usWinAscent = reader.ReadUInt16();
            this.usWinDescent = reader.ReadUInt16();
            this.ulCodePageRange1 = reader.ReadUInt32();
            this.ulCodePageRange2 = reader.ReadUInt32();
        }
        void ReadVersion2(BinaryReader reader)
        {
            //https://www.microsoft.com/typography/otspec/os2ver2.htm

            // 
            //SHORT 	xAvgCharWidth 	 
            //USHORT 	usWeightClass 	 
            //USHORT 	usWidthClass 	 
            //USHORT 	fsType 	 
            this.xAvgCharWidth = reader.ReadInt16();
            this.usWeightClass = reader.ReadUInt16();
            this.usWidthClass = reader.ReadUInt16();
            this.fsType = reader.ReadUInt16();
            //SHORT 	ySubscriptXSize 	 
            //SHORT 	ySubscriptYSize 	 
            //SHORT 	ySubscriptXOffset 	 
            //SHORT 	ySubscriptYOffset 	 
            //SHORT 	ySuperscriptXSize 	 
            //SHORT 	ySuperscriptYSize 	 
            //SHORT 	ySuperscriptXOffset 	 
            //SHORT 	ySuperscriptYOffset 	 
            //SHORT 	yStrikeoutSize 	 
            //SHORT 	yStrikeoutPosition 	 
            //SHORT 	sFamilyClass 	 
            this.ySubscriptXSize = reader.ReadInt16();
            this.ySubscriptYSize = reader.ReadInt16();
            this.ySubscriptXOffset = reader.ReadInt16();
            this.ySubscriptYOffset = reader.ReadInt16();
            this.ySuperscriptXSize = reader.ReadInt16();
            this.ySuperscriptYSize = reader.ReadInt16();
            this.ySuperscriptXOffset = reader.ReadInt16();
            this.ySuperscriptYOffset = reader.ReadInt16();
            this.yStrikeoutSize = reader.ReadInt16();
            this.yStrikeoutPosition = reader.ReadInt16();
            this.sFamilyClass = reader.ReadInt16();
            //BYTE 	panose[10] 	 
            this.panose = reader.ReadBytes(10);
            //ULONG 	ulUnicodeRange1 	Bits 0-31
            //ULONG 	ulUnicodeRange2 	Bits 32-63
            //ULONG 	ulUnicodeRange3 	Bits 64-95
            //ULONG 	ulUnicodeRange4 	Bits 96-127
            this.ulUnicodeRange1 = reader.ReadUInt32();
            this.ulUnicodeRange2 = reader.ReadUInt32();
            this.ulUnicodeRange3 = reader.ReadUInt32();
            this.ulUnicodeRange4 = reader.ReadUInt32();
            //CHAR 	achVendID[4] 	 
            this.achVendID = reader.ReadUInt32();
            //USHORT 	fsSelection 	 
            //USHORT 	usFirstCharIndex 	 
            //USHORT 	usLastCharIndex 
            this.fsSelection = reader.ReadUInt16();
            this.usFirstCharIndex = reader.ReadUInt16();
            this.usLastCharIndex = reader.ReadUInt16();
            //SHORT 	sTypoAscender 	 
            //SHORT 	sTypoDescender 	 
            //SHORT 	sTypoLineGap 	 
            this.sTypoAscender = reader.ReadInt16();
            this.sTypoDescender = reader.ReadInt16();
            this.sTypoLineGap = reader.ReadInt16();
            //USHORT 	usWinAscent 	 
            //USHORT 	usWinDescent 	 
            //ULONG 	ulCodePageRange1 	Bits 0-31
            //ULONG 	ulCodePageRange2 	Bits 32-63
            this.usWinAscent = reader.ReadUInt16();
            this.usWinDescent = reader.ReadUInt16();
            this.ulCodePageRange1 = reader.ReadUInt32();
            this.ulCodePageRange2 = reader.ReadUInt32();
            //SHORT 	sxHeight 	 
            //SHORT 	sCapHeight 	 
            //USHORT 	usDefaultChar 	 
            //USHORT 	usBreakChar 	 
            //USHORT 	usMaxContext
            this.sxHeight = reader.ReadInt16();
            this.sCapHeight = reader.ReadInt16();
            this.usDefaultChar = reader.ReadUInt16();
            this.usBreakChar = reader.ReadUInt16();
            this.usMaxContext = reader.ReadUInt16();
        }
        void ReadVersion3(BinaryReader reader)
        {

            //https://www.microsoft.com/typography/otspec/os2ver3.htm
            //            USHORT 	version 	0x0003
            //SHORT 	xAvgCharWidth 	 
            //USHORT 	usWeightClass 	 
            //USHORT 	usWidthClass 	 
            //USHORT 	fsType 	 
            this.xAvgCharWidth = reader.ReadInt16();
            this.usWeightClass = reader.ReadUInt16();
            this.usWidthClass = reader.ReadUInt16();
            this.fsType = reader.ReadUInt16();
            //SHORT 	ySubscriptXSize 	 
            //SHORT 	ySubscriptYSize 	 
            //SHORT 	ySubscriptXOffset 	 
            //SHORT 	ySubscriptYOffset 	 
            //SHORT 	ySuperscriptXSize 	 
            //SHORT 	ySuperscriptYSize 	 
            //SHORT 	ySuperscriptXOffset 	 
            //SHORT 	ySuperscriptYOffset 	 
            //SHORT 	yStrikeoutSize 	 
            //SHORT 	yStrikeoutPosition 	 
            //SHORT 	sFamilyClass 	 
            this.ySubscriptXSize = reader.ReadInt16();
            this.ySubscriptYSize = reader.ReadInt16();
            this.ySubscriptXOffset = reader.ReadInt16();
            this.ySubscriptYOffset = reader.ReadInt16();
            this.ySuperscriptXSize = reader.ReadInt16();
            this.ySuperscriptYSize = reader.ReadInt16();
            this.ySuperscriptXOffset = reader.ReadInt16();
            this.ySuperscriptYOffset = reader.ReadInt16();
            this.yStrikeoutSize = reader.ReadInt16();
            this.yStrikeoutPosition = reader.ReadInt16();
            this.sFamilyClass = reader.ReadInt16();
            //BYTE 	panose[10] 	 
            this.panose = reader.ReadBytes(10);
            //ULONG 	ulUnicodeRange1 	Bits 0-31
            //ULONG 	ulUnicodeRange2 	Bits 32-63
            //ULONG 	ulUnicodeRange3 	Bits 64-95
            //ULONG 	ulUnicodeRange4 	Bits 96-127
            this.ulUnicodeRange1 = reader.ReadUInt32();
            this.ulUnicodeRange2 = reader.ReadUInt32();
            this.ulUnicodeRange3 = reader.ReadUInt32();
            this.ulUnicodeRange4 = reader.ReadUInt32();
            //CHAR 	achVendID[4] 	 
            this.achVendID = reader.ReadUInt32();
            //USHORT 	fsSelection 	 
            //USHORT 	usFirstCharIndex 	 
            //USHORT 	usLastCharIndex 	 
            this.fsSelection = reader.ReadUInt16();
            this.usFirstCharIndex = reader.ReadUInt16();
            this.usLastCharIndex = reader.ReadUInt16();
            //SHORT 	sTypoAscender 	 
            //SHORT 	sTypoDescender 	 
            //SHORT 	sTypoLineGap 
            this.sTypoAscender = reader.ReadInt16();
            this.sTypoDescender = reader.ReadInt16();
            this.sTypoLineGap = reader.ReadInt16();
            //USHORT 	usWinAscent 	 
            //USHORT 	usWinDescent 	 
            //ULONG 	ulCodePageRange1 	Bits 0-31
            //ULONG 	ulCodePageRange2 	Bits 32-63
            this.usWinAscent = reader.ReadUInt16();
            this.usWinDescent = reader.ReadUInt16();
            this.ulCodePageRange1 = reader.ReadUInt32();
            this.ulCodePageRange2 = reader.ReadUInt32();
            //SHORT 	sxHeight 	 
            //SHORT 	sCapHeight 	 
            //USHORT 	usDefaultChar 	 
            //USHORT 	usBreakChar 	 
            //USHORT 	usMaxContext
            this.sxHeight = reader.ReadInt16();
            this.sCapHeight = reader.ReadInt16();
            this.usDefaultChar = reader.ReadUInt16();
            this.usBreakChar = reader.ReadUInt16();
            this.usMaxContext = reader.ReadUInt16();
        }
        void ReadVersion4(BinaryReader reader)
        {
            //https://www.microsoft.com/typography/otspec/os2ver4.htm

            //SHORT 	xAvgCharWidth 	 
            //USHORT 	usWeightClass 	 
            //USHORT 	usWidthClass 	 
            //USHORT 	fsType 	 
            this.xAvgCharWidth = reader.ReadInt16();
            this.usWeightClass = reader.ReadUInt16();
            this.usWidthClass = reader.ReadUInt16();
            this.fsType = reader.ReadUInt16();
            //SHORT 	ySubscriptXSize 	 
            //SHORT 	ySubscriptYSize 	 
            //SHORT 	ySubscriptXOffset 	 
            //SHORT 	ySubscriptYOffset 	 
            //SHORT 	ySuperscriptXSize 	 
            //SHORT 	ySuperscriptYSize 	 
            //SHORT 	ySuperscriptXOffset 	 
            //SHORT 	ySuperscriptYOffset 	 
            //SHORT 	yStrikeoutSize 	 
            //SHORT 	yStrikeoutPosition 	 
            //SHORT 	sFamilyClass 	 
            this.ySubscriptXSize = reader.ReadInt16();
            this.ySubscriptYSize = reader.ReadInt16();
            this.ySubscriptXOffset = reader.ReadInt16();
            this.ySubscriptYOffset = reader.ReadInt16();
            this.ySuperscriptXSize = reader.ReadInt16();
            this.ySuperscriptYSize = reader.ReadInt16();
            this.ySuperscriptXOffset = reader.ReadInt16();
            this.ySuperscriptYOffset = reader.ReadInt16();
            this.yStrikeoutSize = reader.ReadInt16();
            this.yStrikeoutPosition = reader.ReadInt16();
            this.sFamilyClass = reader.ReadInt16();
            //BYTE 	panose[10] 	 
            this.panose = reader.ReadBytes(10);
            //ULONG 	ulUnicodeRange1 	Bits 0-31
            //ULONG 	ulUnicodeRange2 	Bits 32-63
            //ULONG 	ulUnicodeRange3 	Bits 64-95
            //ULONG 	ulUnicodeRange4 	Bits 96-127
            this.ulUnicodeRange1 = reader.ReadUInt32();
            this.ulUnicodeRange2 = reader.ReadUInt32();
            this.ulUnicodeRange3 = reader.ReadUInt32();
            this.ulUnicodeRange4 = reader.ReadUInt32();
            //CHAR 	achVendID[4] 	 
            this.achVendID = reader.ReadUInt32();
            //USHORT 	fsSelection 	 
            //USHORT 	usFirstCharIndex 	 
            //USHORT 	usLastCharIndex 	 
            this.fsSelection = reader.ReadUInt16();
            this.usFirstCharIndex = reader.ReadUInt16();
            this.usLastCharIndex = reader.ReadUInt16();
            //SHORT 	sTypoAscender 	 
            //SHORT 	sTypoDescender 	 
            //SHORT 	sTypoLineGap 	 
            this.sTypoAscender = reader.ReadInt16();
            this.sTypoDescender = reader.ReadInt16();
            this.sTypoLineGap = reader.ReadInt16();
            //USHORT 	usWinAscent 	 
            //USHORT 	usWinDescent 	 
            //ULONG 	ulCodePageRange1 	Bits 0-31
            //ULONG 	ulCodePageRange2 	Bits 32-63
            this.usWinAscent = reader.ReadUInt16();
            this.usWinDescent = reader.ReadUInt16();
            this.ulCodePageRange1 = reader.ReadUInt32();
            this.ulCodePageRange2 = reader.ReadUInt32();
            //SHORT 	sxHeight 	 
            //SHORT 	sCapHeight 	 
            //USHORT 	usDefaultChar 	 
            //USHORT 	usBreakChar 	 
            //USHORT 	usMaxContext
            this.sxHeight = reader.ReadInt16();
            this.sCapHeight = reader.ReadInt16();
            this.usDefaultChar = reader.ReadUInt16();
            this.usBreakChar = reader.ReadUInt16();
            this.usMaxContext = reader.ReadUInt16();
        }

        void ReadVersion5(BinaryReader reader)
        {
            this.xAvgCharWidth = reader.ReadInt16();
            this.usWeightClass = reader.ReadUInt16();
            this.usWidthClass = reader.ReadUInt16();
            this.fsType = reader.ReadUInt16();
            //SHORT 	ySubscriptXSize 	 
            //SHORT 	ySubscriptYSize 	 
            //SHORT 	ySubscriptXOffset 	 
            //SHORT 	ySubscriptYOffset 	 
            //SHORT 	ySuperscriptXSize 	 
            //SHORT 	ySuperscriptYSize 	 
            //SHORT 	ySuperscriptXOffset 	 
            //SHORT 	ySuperscriptYOffset 	 
            //SHORT 	yStrikeoutSize 	 
            //SHORT 	yStrikeoutPosition 	 
            //SHORT 	sFamilyClass 	 
            this.ySubscriptXSize = reader.ReadInt16();
            this.ySubscriptYSize = reader.ReadInt16();
            this.ySubscriptXOffset = reader.ReadInt16();
            this.ySubscriptYOffset = reader.ReadInt16();
            this.ySuperscriptXSize = reader.ReadInt16();
            this.ySuperscriptYSize = reader.ReadInt16();
            this.ySuperscriptXOffset = reader.ReadInt16();
            this.ySuperscriptYOffset = reader.ReadInt16();
            this.yStrikeoutSize = reader.ReadInt16();
            this.yStrikeoutPosition = reader.ReadInt16();
            this.sFamilyClass = reader.ReadInt16();

            //BYTE 	panose[10] 	 
            this.panose = reader.ReadBytes(10);
            //ULONG 	ulUnicodeRange1 	Bits 0-31
            //ULONG 	ulUnicodeRange2 	Bits 32-63
            //ULONG 	ulUnicodeRange3 	Bits 64-95
            //ULONG 	ulUnicodeRange4 	Bits 96-127
            this.ulUnicodeRange1 = reader.ReadUInt32();
            this.ulUnicodeRange2 = reader.ReadUInt32();
            this.ulUnicodeRange3 = reader.ReadUInt32();
            this.ulUnicodeRange4 = reader.ReadUInt32();

            //CHAR 	achVendID[4] 	 
            this.achVendID = reader.ReadUInt32();
            //USHORT 	fsSelection 	 
            //USHORT 	usFirstCharIndex 	 
            //USHORT 	usLastCharIndex 
            this.fsSelection = reader.ReadUInt16();
            this.usFirstCharIndex = reader.ReadUInt16();
            this.usLastCharIndex = reader.ReadUInt16();
            //SHORT 	sTypoAscender 	 
            //SHORT 	sTypoDescender 	 
            //SHORT 	sTypoLineGap 	 
            this.sTypoAscender = reader.ReadInt16();
            this.sTypoDescender = reader.ReadInt16();
            this.sTypoLineGap = reader.ReadInt16();
            //USHORT 	usWinAscent 	 
            //USHORT 	usWinDescent 	 
            //ULONG 	ulCodePageRange1 	Bits 0-31
            //ULONG 	ulCodePageRange2 	Bits 32-63
            this.usWinAscent = reader.ReadUInt16();
            this.usWinDescent = reader.ReadUInt16();
            this.ulCodePageRange1 = reader.ReadUInt32();
            this.ulCodePageRange2 = reader.ReadUInt32();
            //SHORT 	sxHeight 	 
            //SHORT 	sCapHeight 	 
            //USHORT 	usDefaultChar 	 
            //USHORT 	usBreakChar 	 
            //USHORT 	usMaxContext 	 
            this.sxHeight = reader.ReadInt16();
            this.sCapHeight = reader.ReadInt16();
            this.usDefaultChar = reader.ReadUInt16();
            this.usBreakChar = reader.ReadUInt16();
            this.usMaxContext = reader.ReadUInt16();
            //USHORT 	usLowerOpticalPointSize 	 
            //USHORT 	usUpperOpticalPointSize 	 

            this.usLowerOpticalPointSize = reader.ReadUInt16();
            this.usUpperOpticalPointSize = reader.ReadUInt16();
        }
    }
}
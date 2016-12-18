﻿//Apache2, 2014-2016, Samuel Carlsson, WinterDev

using System.Collections.Generic;
using NOpenType.Tables;
namespace NOpenType
{
    public class Typeface
    {
        readonly Bounds _bounds;
        readonly ushort _unitsPerEm;
        readonly Glyph[] _glyphs;
        readonly CharacterMap[] _cmaps;
        readonly HorizontalMetrics _horizontalMetrics;
        readonly NameEntry _nameEntry;        
        readonly OS2Table _os2Table;
        Kern _kern;

        internal Typeface(
            NameEntry nameEntry,
            Bounds bounds,
            ushort unitsPerEm,
            Glyph[] glyphs,
            CharacterMap[] cmaps,
            HorizontalMetrics horizontalMetrics,
            OS2Table os2Table)
        {
            _nameEntry = nameEntry;
            _bounds = bounds;
            _unitsPerEm = unitsPerEm;
            _glyphs = glyphs;
            _cmaps = cmaps;
            _horizontalMetrics = horizontalMetrics;
            _os2Table = os2Table;
        }
        internal Kern KernTable
        {
            get { return _kern; }
            set { this._kern = value; }
        }
        internal Gasp GaspTable
        {
            get;
            set;
        }
        public string Name
        {
            get { return _nameEntry.FontName; }
        }
        public string FontSubFamily
        {
            get { return _nameEntry.FontSubFamily; }
        }
        public int LookupIndex(char character)
        {
            // TODO: What if there are none or several tables?
            return _cmaps[0].CharacterToGlyphIndex(character);
        }

        public Glyph Lookup(char character)
        {
            return _glyphs[LookupIndex(character)];
        }
        public Glyph GetGlyphByIndex(int glyphIndex)
        {
            return _glyphs[glyphIndex];
        }
        public ushort GetAdvanceWidth(char character)
        {
            return _horizontalMetrics.GetAdvanceWidth(LookupIndex(character));
        }
        public ushort GetAdvanceWidthFromGlyphIndex(int glyphIndex)
        {
            return _horizontalMetrics.GetAdvanceWidth(glyphIndex);
        }
        public short GetKernDistance(ushort leftGlyphIndex, ushort rightGlyphIndex)
        {
            return _kern.GetKerningDistance(leftGlyphIndex, rightGlyphIndex);
        }
        public Bounds Bounds { get { return _bounds; } }
        public ushort UnitsPerEm { get { return _unitsPerEm; } }
        public Glyph[] Glyphs { get { return _glyphs; } }


        const int pointsPerInch = 72;
        public float CalculateScale(float sizeInPointUnit, int resolution = 96)
        {
            //  float scale = GlyphPathBuilder.GetFUnitToPixelsScale(size,
            //glyphPathBuilder.Resolution,
            //typeface.UnitsPerEm);
            return ((sizeInPointUnit * resolution) / (pointsPerInch * this.UnitsPerEm));
        }

        GDEF GDEFTable
        {
            get;
            set;
        }
        GSUB GSUBTable
        {
            get;
            set;
        }
        GPOS GPOSTable
        {
            get;
            set;
        }
        BASE BaseTable
        {
            get;
            set;
        }

        //-------------------------------------------------------

        public void Lookup(char[] buffer, List<int> output)
        {
            //do shaping here?
            //1. do look up and substitution 
            int j = buffer.Length;
            for (int i = 0; i < j; ++i)
            {
                output.Add(LookupIndex(buffer[i]));
            }
            //tmp disable here
            //check for glyph substitution
            //this.GSUBTable.CheckSubstitution(output[1]);
        }
        //-------------------------------------------------------
        //experiment
        internal void LoadOpenTypeLayoutInfo(GDEF gdefTable, GSUB gsubTable, GPOS gposTable, BASE baseTable)
        {

            //***
            this.GDEFTable = gdefTable;
            this.GSUBTable = gsubTable;
            this.GPOSTable = gposTable;
            this.BaseTable = baseTable;
            //---------------------------
            //1. fill glyph definition            
            if (gdefTable != null)
            {
                gdefTable.FillGlyphData(this.Glyphs);
            }



        }

    }
}

﻿//Apache2,  2016,  WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NOpenType.Tables
{
    //https://www.microsoft.com/typography/otspec/GPOS.htm

    partial class GPOS : TableEntry
    {
        long gposTableStartAt;
        ScriptList scriptList = new ScriptList();
        FeatureList featureList = new FeatureList();
        List<LookupTable> lookupRecords = new List<LookupTable>();
        public override string Name
        {
            get { return "GPOS"; }
        }
        protected override void ReadContentFrom(BinaryReader reader)
        {
            gposTableStartAt = reader.BaseStream.Position;
            //-------------------------------------------
            // GPOS Header
            //The GPOS table begins with a header that contains a version number for the table. Two versions are defined. 
            //Version 1.0 contains offsets to three tables: ScriptList, FeatureList, and LookupList. 
            //Version 1.1 also includes an offset to a FeatureVariations table.
            //For descriptions of these tables, see the chapter, OpenType Layout Common Table Formats .
            //Example 1 at the end of this chapter shows a GPOS Header table definition.
            //GPOS Header, Version 1.0
            //Value 	Type 	Description
            //USHORT 	MajorVersion 	Major version of the GPOS table, = 1
            //USHORT 	MinorVersion 	Minor version of the GPOS table, = 0
            //Offset 	ScriptList 	Offset to ScriptList table, from beginning of GPOS table
            //Offset 	FeatureList 	Offset to FeatureList table, from beginning of GPOS table
            //Offset 	LookupList 	Offset to LookupList table, from beginning of GPOS table

            //GPOS Header, Version 1.1
            //Value 	Type 	Description
            //USHORT 	MajorVersion 	Major version of the GPOS table, = 1
            //USHORT 	MinorVersion 	Minor version of the GPOS table, = 1
            //Offset 	ScriptList 	Offset to ScriptList table, from beginning of GPOS table
            //Offset 	FeatureList 	Offset to FeatureList table, from beginning of GPOS table
            //Offset 	LookupList 	Offset to LookupList table, from beginning of GPOS table
            //ULONG 	FeatureVariations 	Offset to FeatureVariations table, from beginning of GPOS table (may be NULL) 

            this.MajorVersion = reader.ReadUInt16();
            this.MinorVersion = reader.ReadUInt16();

            ushort scriptListOffset = reader.ReadUInt16();//from beginning of GSUB table
            ushort featureListOffset = reader.ReadUInt16();//from beginning of GSUB table
            ushort lookupListOffset = reader.ReadUInt16();//from beginning of GSUB table
            uint featureVariations = (MinorVersion == 1) ? reader.ReadUInt32() : 0;//from beginning of GSUB table

            //-----------------------
            //1. scriptlist             
            scriptList = ScriptList.CreateFrom(reader, gposTableStartAt + scriptListOffset);
            //-----------------------
            //2. feature list             
            featureList = FeatureList.CreateFrom(reader, gposTableStartAt + featureListOffset);
            //-----------------------
            //3. lookup list

            ReadLookupListTable(reader, gposTableStartAt + lookupListOffset);
            //-----------------------
            //4. feature variations
            if (featureVariations > 0)
            {
                reader.BaseStream.Seek(this.Header.Offset + featureVariations, SeekOrigin.Begin);
                ReadFeaureVariations(reader);
            }

        }
        public ushort MajorVersion { get; private set; }
        public ushort MinorVersion { get; private set; }
        void ReadFeaureVariations(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
        void ReadLookupListTable(BinaryReader reader, long lookupListBeginAt)
        {

            reader.BaseStream.Seek(lookupListBeginAt, SeekOrigin.Begin);
            //

            //https://www.microsoft.com/typography/otspec/chapter2.htm
            //LookupList table
            //Type 	Name 	Description
            //USHORT 	LookupCount 	Number of lookups in this table
            //Offset 	Lookup[LookupCount] 	Array of offsets to Lookup tables-from beginning of LookupList -zero based (first lookup is Lookup index = 0)

            //Lookup Table
            //A Lookup table (Lookup) defines the specific conditions, type, 
            //and results of a substitution or positioning action that is used to implement a feature. 
            //For example, a substitution operation requires a list of target glyph indices to be replaced, 
            //a list of replacement glyph indices, and a description of the type of substitution action.
            //Each Lookup table may contain only one type of information (LookupType),
            //determined by whether the lookup is part of a GSUB or GPOS table. GSUB supports eight LookupTypes, 
            //and GPOS supports nine LookupTypes (for details about LookupTypes, see the GSUB and GPOS chapters of the document).

            //Each LookupType is defined with one or more subtables, 
            //and each subtable definition provides a different representation format.
            //The format is determined by the content of the information required for an operation and by required storage efficiency.
            //When glyph information is best presented in more than one format,
            //a single lookup may contain more than one subtable, as long as all the subtables are the same LookupType. 
            //For example, within a given lookup, a glyph index array format may best represent one set of target glyphs,
            //whereas a glyph index range format may be better for another set of target glyphs.

            //During text processing, a client applies a lookup to each glyph in the string before moving to the next lookup. 
            //A lookup is finished for a glyph after the client makes the substitution/positioning operation.
            //To move to the “next” glyph, the client will typically skip all the glyphs that participated in the lookup operation: glyphs 
            //that were substituted/positioned as well as any other glyphs that formed a context for the operation. However, in the case of pair positioning operations (i.e., kerning), the “next” glyph in a sequence may be the second glyph of the positioned pair (see pair positioning lookup for details).

            //A Lookup table contains a LookupType, specified as an integer, that defines the type of information stored in the lookup.
            //The LookupFlag specifies lookup qualifiers that assist a text-processing client in substituting or positioning glyphs.
            //The SubTableCount specifies the total number of SubTables. 
            //The SubTable array specifies offsets, measured from the beginning of the Lookup table, to each SubTable enumerated in the SubTable array.
            //
            //Lookup table
            //Type 	Name 	Description
            //USHORT 	LookupType 	Different enumerations for GSUB and GPOS
            //USHORT 	LookupFlag 	Lookup qualifiers
            //USHORT 	SubTableCount 	Number of SubTables for this lookup
            //Offset 	SubTable
            //[SubTableCount] 	Array of offsets to SubTables-from beginning of Lookup table
            //unit16 	MarkFilteringSet
            lookupRecords.Clear();
            ushort lookupCount = reader.ReadUInt16();
            short[] lookupTableOffsets = Utils.ReadInt16Array(reader, lookupCount);

            //----------------------------------------------
            //load each sub table
            //https://www.microsoft.com/typography/otspec/chapter2.htm
            for (int i = 0; i < lookupCount; ++i)
            {
                long lookupTablePos = lookupListBeginAt + lookupTableOffsets[i];
                reader.BaseStream.Seek(lookupTablePos, SeekOrigin.Begin);

                ushort lookupType = reader.ReadUInt16();//Each Lookup table may contain only one type of information (LookupType)
                ushort lookupFlags = reader.ReadUInt16();
                ushort subTableCount = reader.ReadUInt16();
                //Each LookupType is defined with one or more subtables, and each subtable definition provides a different representation format
                //
                short[] subTableOffsets = Utils.ReadInt16Array(reader, subTableCount);

                ushort markFilteringSet =
                    ((lookupFlags & 0x0010) == 0x0010) ? reader.ReadUInt16() : (ushort)0;

                lookupRecords.Add(
                    new LookupTable(
                        lookupTablePos,
                        lookupType,
                        lookupFlags,
                        subTableCount,
                        subTableOffsets,//Array of offsets to SubTables-from beginning of Lookup table
                        markFilteringSet));
            }
            //----------------------------------------------
            //read each lookup record content ...
            for (int i = 0; i < lookupCount; ++i)
            {
                LookupTable lookupRecord = lookupRecords[i];
                //set origin
                reader.BaseStream.Seek(lookupListBeginAt + lookupTableOffsets[i], SeekOrigin.Begin);
                lookupRecord.ReadRecordContent(reader);
            }

        }

        abstract class LookupSubTable { }

        /// <summary>
        /// sub table of a lookup list
        /// </summary>
        class LookupTable
        {
            //--------------------------
            long lookupTablePos;
            //--------------------------
            public readonly ushort lookupType;
            public readonly ushort lookupFlags;
            public readonly ushort subTableCount;
            public readonly short[] subTableOffsets;
            public readonly ushort markFilteringSet;
            //--------------------------
            List<LookupSubTable> subTables = new List<LookupSubTable>();
            public LookupTable(
                long lookupTablePos,
                ushort lookupType,
                ushort lookupFlags,
                ushort subTableCount,
                short[] subTableOffsets,
                ushort markFilteringSet
                 )
            {
                this.lookupTablePos = lookupTablePos;
                this.lookupType = lookupType;
                this.lookupFlags = lookupFlags;
                this.subTableCount = subTableCount;
                this.subTableOffsets = subTableOffsets;
                this.markFilteringSet = markFilteringSet;
            }
#if DEBUG
            public override string ToString()
            {
                return lookupType.ToString();
            }
#endif
            public void ReadRecordContent(BinaryReader reader)
            {
                switch (lookupType)
                {
                    default: throw new NotSupportedException();
                    case 1:
                        ReadLookupType1(reader);
                        break;
                    case 2:
                        ReadLookupType2(reader);
                        break;
                    case 3:
                        ReadLookupType3(reader);
                        break;
                    case 4:
                        ReadLookupType4(reader);
                        break;
                    case 5:
                        ReadLookupType5(reader);
                        break;
                    case 6:
                        ReadLookupType6(reader);
                        break;
                    case 7:
                        ReadLookupType7(reader);
                        break;
                    case 8:
                        ReadLookupType8(reader);
                        break;
                    case 9:
                        ReadLookupType9(reader);
                        break;
                }
            }

            class LkSubTableType1 : LookupSubTable
            {
                ValueRecord singleValue;
                ValueRecord[] multiValues;
                public LkSubTableType1(ValueRecord singleValue)
                {
                    this.Format = 1;
                    this.singleValue = singleValue;
                }
                public LkSubTableType1(ValueRecord[] valueRecords)
                {
                    this.Format = 2;
                    this.multiValues = valueRecords;
                }
                public int Format
                {
                    get;
                    private set;
                }
                public CoverageTable CoverageTable { get; set; }
            }
            /// <summary>
            /// Lookup Type 1: Single Adjustment Positioning Subtable
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType1(BinaryReader reader)
            {
                long thisLookupTablePos = reader.BaseStream.Position;
                int j = subTableOffsets.Length;

                for (int i = 0; i < j; ++i)
                {
                    //move to read pos
                    long subTableStartAt = reader.BaseStream.Position + subTableOffsets[i];
                    reader.BaseStream.Seek(subTableStartAt, SeekOrigin.Begin);
                    //-----------------------

                    ushort format = reader.ReadUInt16();

                    switch (format)
                    {
                        default: throw new NotSupportedException();
                        case 1:
                            {
                                //Single Adjustment Positioning: Format 1
                                // USHORT 	PosFormat 	Format identifier-format = 1
                                //Offset 	Coverage 	Offset to Coverage table-from beginning of SinglePos subtable
                                //USHORT 	ValueFormat 	Defines the types of data in the ValueRecord
                                //ValueRecord 	Value 	Defines positioning value(s)-applied to all glyphs in the Coverage table 
                                short coverage = reader.ReadInt16();
                                ushort valueFormat = reader.ReadUInt16();
                                var subTable = new LkSubTableType1(ValueRecord.CreateFrom(reader, valueFormat));
                                //-------                                 
                                subTable.CoverageTable = CoverageTable.CreateFrom(reader, subTableStartAt + coverage);
                                //-------
                                this.subTables.Add(subTable);
                            } break;
                        case 2:
                            {
                                //Single Adjustment Positioning: Format 2
                                //USHORT 	PosFormat 	Format identifier-format = 2
                                //Offset 	Coverage 	Offset to Coverage table-from beginning of SinglePos subtable
                                //USHORT 	ValueFormat 	Defines the types of data in the ValueRecord
                                //USHORT 	ValueCount 	Number of ValueRecords
                                //ValueRecord 	Value
                                //[ValueCount] 	Array of ValueRecords-positioning values applied to glyphs
                                short coverage = reader.ReadInt16();
                                ushort valueFormat = reader.ReadUInt16();
                                ushort valueCount = reader.ReadUInt16();
                                var values = new ValueRecord[valueCount];
                                for (int n = 0; n < valueCount; ++n)
                                {
                                    values[n] = ValueRecord.CreateFrom(reader, valueFormat);
                                }
                                var subTable = new LkSubTableType1(values);
                                //-------

                                subTable.CoverageTable = CoverageTable.CreateFrom(reader, subTableStartAt + coverage);
                                //-------
                                this.subTables.Add(subTable);
                            }
                            break;
                    }
                }
            }


            class LkSubTableType2 : LookupSubTable
            {
                PairSetTable[] pairSetTables;
                public LkSubTableType2(PairSetTable[] pairSetTables)
                {
                    this.pairSetTables = pairSetTables;
                }
                public CoverageTable CoverageTable
                {
                    get;
                    set;
                }
            }
            /// <summary>
            ///  Lookup Type 2: Pair Adjustment Positioning Subtable
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType2(BinaryReader reader)
            {
                //-----------------------------------------------
                // USHORT 	PosFormat 	Format identifier-format = 1
                //Offset 	Coverage 	Offset to Coverage table-from beginning of PairPos subtable-only the first glyph in each pair
                //USHORT 	ValueFormat1 	Defines the types of data in ValueRecord1-for the first glyph in the pair -may be zero (0)
                //USHORT 	ValueFormat2 	Defines the types of data in ValueRecord2-for the second glyph in the pair -may be zero (0)
                //USHORT 	PairSetCount 	Number of PairSet tables
                //Offset 	PairSetOffset
                //[PairSetCount] 	Array of offsets to PairSet tables-from beginning of PairPos subtable-ordered by Coverage Index
                //                PairSet table
                //Value 	Type 	Description
                //USHORT 	PairValueCount 	Number of PairValueRecords
                //struct 	PairValueRecord
                //[PairValueCount] 	Array of PairValueRecords-ordered by GlyphID of the second glyph

                //A PairValueRecord specifies the second glyph in a pair (SecondGlyph) and defines a ValueRecord for each glyph (Value1 and Value2). If ValueFormat1 is set to zero (0) in the PairPos subtable, ValueRecord1 will be empty; similarly, if ValueFormat2 is 0, Value2 will be empty.

                //Example 4 at the end of this chapter shows a PairPosFormat1 subtable that defines two cases of pair kerning.
                //PairValueRecord
                //Value 	Type 	Description
                //GlyphID 	SecondGlyph 	GlyphID of second glyph in the pair-first glyph is listed in the Coverage table
                //ValueRecord 	Value1 	Positioning data for the first glyph in the pair
                //ValueRecord 	Value2 	Positioning data for the second glyph in the pair
                //-----------------------------------------------

                // PairPosFormat2 subtable: Class pair adjustment
                //Value 	Type 	Description
                //USHORT 	PosFormat 	Format identifier-format = 2
                //Offset 	Coverage 	Offset to Coverage table-from beginning of PairPos subtable-for the first glyph of the pair
                //USHORT 	ValueFormat1 	ValueRecord definition-for the first glyph of the pair-may be zero (0)
                //USHORT 	ValueFormat2 	ValueRecord definition-for the second glyph of the pair-may be zero (0)
                //Offset 	ClassDef1 	Offset to ClassDef table-from beginning of PairPos subtable-for the first glyph of the pair
                //Offset 	ClassDef2 	Offset to ClassDef table-from beginning of PairPos subtable-for the second glyph of the pair
                //USHORT 	Class1Count 	Number of classes in ClassDef1 table-includes Class0
                //USHORT 	Class2Count 	Number of classes in ClassDef2 table-includes Class0
                //struct 	Class1Record
                //[Class1Count] 	Array of Class1 records-ordered by Class1

                //Each Class1Record contains an array of Class2Records (Class2Record), which also are ordered by class value. 
                //One Class2Record must be declared for each class in the ClassDef2 table, including Class 0.
                //Class1Record
                //Value 	Type 	Description
                //struct 	Class2Record[Class2Count] 	Array of Class2 records-ordered by Class2

                //A Class2Record consists of two ValueRecords,
                //one for the first glyph in a class pair (Value1) and one for the second glyph (Value2).
                //If the PairPos subtable has a value of zero (0) for ValueFormat1 or ValueFormat2, 
                //the corresponding record (ValueRecord1 or ValueRecord2) will be empty.

                //Example 5 at the end of this chapter demonstrates pair kerning with glyph classes in a PairPosFormat2 subtable.
                //Class2Record
                //Value 	Type 	Description
                //ValueRecord 	Value1 	Positioning for first glyph-empty if ValueFormat1 = 0
                //ValueRecord 	Value2 	Positioning for second glyph-empty if ValueFormat2 = 0
                long thisLookupTablePos = reader.BaseStream.Position;
                int j = subTableOffsets.Length;

                for (int i = 0; i < j; ++i)
                {
                    //move to read pos
                    long subTableStartAt = lookupTablePos + subTableOffsets[i];
                    reader.BaseStream.Seek(subTableStartAt, SeekOrigin.Begin);

                    //-----------------------

                    ushort format = reader.ReadUInt16();

                    switch (format)
                    {
                        default: throw new NotSupportedException();
                        case 1:
                            {
                                short coverage = reader.ReadInt16();
                                ushort value1Format = reader.ReadUInt16();
                                ushort value2Format = reader.ReadUInt16();
                                ushort pairSetCount = reader.ReadUInt16();
                                short[] pairSetOffsetArray = Utils.ReadInt16Array(reader, pairSetCount);
                                PairSetTable[] pairSetTables = new PairSetTable[pairSetCount];
                                for (int n = 0; n < pairSetCount; ++n)
                                {
                                    //read each pair set table
                                    reader.BaseStream.Seek(thisLookupTablePos + pairSetOffsetArray[i], SeekOrigin.Begin);
                                    var pairSetTable = new PairSetTable();
                                    pairSetTable.ReadFrom(reader, value1Format, value2Format);
                                    pairSetTables[n] = pairSetTable;
                                }
                                var subTable = new LkSubTableType2(pairSetTables);
                                //coverage                                 
                                subTable.CoverageTable = CoverageTable.CreateFrom(reader, subTableStartAt + coverage);

                                subTables.Add(subTable);
                            } break;
                        case 2:
                            {


                                short coverage = reader.ReadInt16();
                                ushort value1Format = reader.ReadUInt16();
                                ushort value2Format = reader.ReadUInt16();
                                short classDef1Offset = reader.ReadInt16();
                                short classDef2Offset = reader.ReadInt16();
                                ushort class1Count = reader.ReadUInt16();
                                ushort class2Count = reader.ReadUInt16();


                                throw new NotImplementedException();

                            }
                            break;
                    }


                }
            }

            /// <summary>
            /// Lookup Type 3: Cursive Attachment Positioning Subtable
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType3(BinaryReader reader)
            {
                throw new NotImplementedException();
            }

            class LkSubTableType4 : LookupSubTable
            {
                public LkSubTableType4()
                {
                }
                public CoverageTable MarkCoverageTable { get; set; }
                public CoverageTable BaseCoverageTable { get; set; }
                public BaseArrayTable BaseArrayTable { get; set; }
                public MarkArrayTable MarkArrayTable { get; set; }
            }
            /// <summary>
            /// Lookup Type 4: MarkToBase Attachment Positioning Subtable
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType4(BinaryReader reader)
            {


                //MarkBasePosFormat1 subtable: MarkToBase attachment point
                //Value 	Type 	Description
                //USHORT 	PosFormat 	Format identifier-format = 1
                //Offset 	MarkCoverage 	Offset to MarkCoverage table-from beginning of MarkBasePos subtable
                //Offset 	BaseCoverage 	Offset to BaseCoverage table-from beginning of MarkBasePos subtable
                //USHORT 	ClassCount 	Number of classes defined for marks
                //Offset 	MarkArray 	Offset to MarkArray table-from beginning of MarkBasePos subtable
                //Offset 	BaseArray 	Offset to BaseArray table-from beginning of MarkBasePos subtable

                //The BaseArray table consists of an array (BaseRecord) and count (BaseCount) of BaseRecords. 
                //The array stores the BaseRecords in the same order as the BaseCoverage Index. 
                //Each base glyph in the BaseCoverage table has a BaseRecord.

                //BaseArray table
                //Value 	Type 	Description
                //USHORT 	BaseCount 	Number of BaseRecords
                //struct 	BaseRecord[BaseCount] 	Array of BaseRecords-in order of BaseCoverage Index
                long thisSubTablePos = reader.BaseStream.Position;
                int j = subTableOffsets.Length;
                for (int i = 0; i < j; ++i)
                {
                    //move to read pos
                    long subtableStart = thisSubTablePos + subTableOffsets[i]; //beginning of MarkBasePos subtable ***
                    reader.BaseStream.Seek(subtableStart, SeekOrigin.Begin);

                    //----------------------- 
                    ushort format = reader.ReadUInt16();
                    if (format != 1)
                    {
                        throw new NotSupportedException();
                    }
                    short markCoverageOffset = reader.ReadInt16(); //offset from 
                    short baseCoverageOffset = reader.ReadInt16();
                    ushort classCount = reader.ReadUInt16();
                    short markArrayOffset = reader.ReadInt16();
                    short baseArrayOffset = reader.ReadInt16();

                    //---------------------------------------------------------------------------

                    //read mark array table
                    var lookupType4 = new LkSubTableType4();
                    //---------------------------------------------------------------------------                     
                    lookupType4.MarkCoverageTable = CoverageTable.CreateFrom(reader, subtableStart + markCoverageOffset);
                    //---------------------------------------------------------------------------                    
                    lookupType4.BaseCoverageTable = CoverageTable.CreateFrom(reader, subtableStart + baseCoverageOffset);
                    //---------------------------------------------------------------------------                     
                    lookupType4.MarkArrayTable = MarkArrayTable.CreateFrom(reader, subtableStart + markArrayOffset);
                    //---------------------------------------------------------------------------                     
                    lookupType4.BaseArrayTable = BaseArrayTable.CreateFrom(reader, subtableStart + baseArrayOffset, classCount);
                    //---------------------------------------------------------------------------
                    this.subTables.Add(lookupType4);
                }
            }


            class LkSubTableType5 : LookupSubTable
            {
                public CoverageTable MarkCoverage { get; set; }
                public CoverageTable LigatureCoverage { get; set; }
                public MarkArrayTable MarkArrayTable { get; set; }
                public LigatureArrayTable LigatureArrayTable { get; set; }
            }
            /// <summary>
            /// Lookup Type 5: MarkToLigature Attachment Positioning Subtable
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType5(BinaryReader reader)
            {
                //USHORT 	PosFormat 	Format identifier-format = 1
                //Offset 	MarkCoverage 	Offset to Mark Coverage table-from beginning of MarkLigPos subtable
                //Offset 	LigatureCoverage 	Offset to Ligature Coverage table-from beginning of MarkLigPos subtable
                //USHORT 	ClassCount 	Number of defined mark classes
                //Offset 	MarkArray 	Offset to MarkArray table-from beginning of MarkLigPos subtable
                //Offset 	LigatureArray 	Offset to LigatureArray table-from beginning of MarkLigPos subtable

                long thisLookupTablePos = reader.BaseStream.Position;
                int j = subTableOffsets.Length;

                for (int i = 0; i < j; ++i)
                {

                    long subTableStartAt = lookupTablePos + subTableOffsets[i];
                    reader.BaseStream.Seek(subTableStartAt, SeekOrigin.Begin);
                    //-----------------------

                    ushort format = reader.ReadUInt16();
                    if (format != 1)
                    {
                        throw new NotSupportedException();
                    }
                    short markCoverageOffset = reader.ReadInt16(); //from beginning of MarkLigPos subtable
                    short ligatureCoverageOffset = reader.ReadInt16();
                    ushort classCount = reader.ReadUInt16();
                    short markArrayOffset = reader.ReadInt16();
                    short ligatureArrayOffset = reader.ReadInt16();
                    //-----------------------
                    var subTable = new LkSubTableType5();
                    //-----------------------
                    subTable.MarkCoverage = CoverageTable.CreateFrom(reader, subTableStartAt + markCoverageOffset);
                    //-----------------------
                    subTable.LigatureCoverage = CoverageTable.CreateFrom(reader, subTableStartAt + ligatureCoverageOffset);
                    //-----------------------                     
                    subTable.MarkArrayTable = MarkArrayTable.CreateFrom(reader, subTableStartAt + markArrayOffset);
                    //-----------------------
                    reader.BaseStream.Seek(subTableStartAt + ligatureArrayOffset, SeekOrigin.Begin);
                    var ligatureArrayTable = new LigatureArrayTable();
                    ligatureArrayTable.ReadFrom(reader, classCount);
                    subTable.LigatureArrayTable = ligatureArrayTable;
                    //-----------------------
                    this.subTables.Add(subTable);
                }
            }
            class LkSubTableType6 : LookupSubTable
            {
                public CoverageTable MarkCoverage1 { get; set; }
                public CoverageTable MarkCoverage2 { get; set; }
                public CoverageTable LigatureCoverage { get; set; }
                public MarkArrayTable Mark1ArrayTable { get; set; }
                public Mark2ArrayTable Mark2ArrayTable { get; set; }
            }

            /// <summary>
            /// Lookup Type 6: MarkToMark Attachment Positioning Subtable
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType6(BinaryReader reader)
            {

                //USHORT 	PosFormat 	Format identifier-format = 1
                //Offset 	Mark1Coverage 	Offset to Combining Mark Coverage table-from beginning of MarkMarkPos subtable
                //Offset 	Mark2Coverage 	Offset to Base Mark Coverage table-from beginning of MarkMarkPos subtable
                //USHORT 	ClassCount 	Number of Combining Mark classes defined
                //Offset 	Mark1Array 	Offset to MarkArray table for Mark1-from beginning of MarkMarkPos subtable
                //Offset 	Mark2Array 	Offset to Mark2Array table for Mark2-from beginning of MarkMarkPos subtable

                long thisLookupTablePos = reader.BaseStream.Position;
                int j = subTableOffsets.Length;

                for (int i = 0; i < j; ++i)
                {
                    //move to read pos
                    long subTableStartAt = lookupTablePos + subTableOffsets[i];
                    reader.BaseStream.Seek(subTableStartAt, SeekOrigin.Begin);

                    //-----------------------

                    ushort format = reader.ReadUInt16();
                    if (format != 1)
                    {
                        throw new NotSupportedException();
                    }
                    short mark1CoverageOffset = reader.ReadInt16();
                    short mark2CoverageOffset = reader.ReadInt16();
                    ushort classCount = reader.ReadUInt16();
                    short mark1ArrayOffset = reader.ReadInt16();
                    short mark2ArrayOffset = reader.ReadInt16();
                    //
                    var subTable = new LkSubTableType6();
                    subTable.MarkCoverage1 = CoverageTable.CreateFrom(reader, subTableStartAt + mark1CoverageOffset);
                    subTable.MarkCoverage2 = CoverageTable.CreateFrom(reader, subTableStartAt + mark2CoverageOffset);
                    subTable.Mark1ArrayTable = MarkArrayTable.CreateFrom(reader, subTableStartAt + mark1ArrayOffset);
                    subTable.Mark2ArrayTable = Mark2ArrayTable.CreateFrom(reader, subTableStartAt + mark2ArrayOffset, classCount);


                    this.subTables.Add(subTable);
                }

            }
            /// <summary>
            /// Lookup Type 7: Contextual Positioning Subtables
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType7(BinaryReader reader)
            {

                long thisLookupTablePos = reader.BaseStream.Position;
                int j = subTableOffsets.Length;

                for (int i = 0; i < j; ++i)
                {
                    //move to read pos
                    long subTableStartAt = lookupTablePos + subTableOffsets[i];
                    reader.BaseStream.Seek(subTableStartAt, SeekOrigin.Begin);
                    //-----------------------

                    ushort format = reader.ReadUInt16();
                    switch (format)
                    {
                        default: throw new NotSupportedException();
                        case 1:
                            {
                                //Context Positioning Subtable: Format 1
                                //ContextPosFormat1 subtable: Simple context positioning
                                //Value 	Type 	Description
                                //USHORT 	PosFormat 	Format identifier-format = 1
                                //Offset 	Coverage 	Offset to Coverage table-from beginning of ContextPos subtable
                                //USHORT 	PosRuleSetCount 	Number of PosRuleSet tables
                                //Offset 	PosRuleSet[PosRuleSetCount]
                                //
                                short coverageOffset = reader.ReadInt16();
                                ushort posRuleSetCount = reader.ReadUInt16();
                                short[] posRuleSetOffsets = Utils.ReadInt16Array(reader, posRuleSetCount);

                                LkSubTableType7Fmt1 subTable = new LkSubTableType7Fmt1();
                                subTable.PosRuleSetTables = CreateMultiplePosRuleSetTables(subTableStartAt, posRuleSetOffsets, reader);
                                subTable.CoverageTable = CoverageTable.CreateFrom(reader, subTableStartAt + coverageOffset);

                                //-----------
                                subTables.Add(subTable);
                            } break;
                        case 2:
                            {
                                //Context Positioning Subtable: Format 2
                                //USHORT 	PosFormat 	Format identifier-format = 2
                                //Offset 	Coverage 	Offset to Coverage table-from beginning of ContextPos subtable
                                //Offset 	ClassDef 	Offset to ClassDef table-from beginning of ContextPos subtable
                                //USHORT 	PosClassSetCnt 	Number of PosClassSet tables
                                //Offset 	PosClassSet
                                //[PosClassSetCnt] 	Array of offsets to PosClassSet tables-from beginning of ContextPos subtable-ordered by class-may be NULL
                                short coverageOffset = reader.ReadInt16();
                                short classDefOffset = reader.ReadInt16();
                                ushort posClassSetCount = reader.ReadUInt16();
                                short[] posClassSetOffsets = Utils.ReadInt16Array(reader, posClassSetCount);

                                var subTable = new LkSubTableType7Fmt2();
                                subTable.ClassDefOffset = classDefOffset;
                                //---------- 
                                PosClassSetTable[] posClassSetTables = new PosClassSetTable[posClassSetCount];
                                subTable.PosClassSetTables = posClassSetTables;
                                for (int n = 0; n < posClassSetCount; ++n)
                                {
                                    posClassSetTables[n] = PosClassSetTable.CreateFrom(reader, coverageOffset);
                                }
                                //----------                                  
                                subTable.CoverageTable = CoverageTable.CreateFrom(reader, subTableStartAt + coverageOffset);
                                //---------- 
                                subTables.Add(subTable);
                                //----------
                            } break;
                        case 3:
                            {
                                //ContextPosFormat3 subtable: Coverage-based context glyph positioning
                                //Value 	Type 	Description
                                //USHORT 	PosFormat 	Format identifier-format = 3
                                //USHORT 	GlyphCount 	Number of glyphs in the input sequence
                                //USHORT 	PosCount 	Number of PosLookupRecords
                                //Offset 	Coverage[GlyphCount] 	Array of offsets to Coverage tables-from beginning of ContextPos subtable
                                //struct 	PosLookupRecord[PosCount] Array of positioning lookups-in design order
                                var subTable = new LkSubTableType7Fmt3();
                                ushort glyphCount = reader.ReadUInt16();
                                ushort posCount = reader.ReadUInt16();
                                //read each lookahead record
                                short[] coverageOffsets = Utils.ReadInt16Array(reader, glyphCount);
                                subTable.PosLookupRecords = CreateMultiplePosLookupRecords(reader, posCount);
                                subTable.CoverageTables = CoverageTable.CreateMultipleCoverageTables(subTableStartAt, coverageOffsets, reader);
                                //---------- 
                                subTables.Add(subTable);
                                //----------
                            } break;
                    }
                }
            }



            class LkSubTableType7Fmt1 : LookupSubTable
            {

                public CoverageTable CoverageTable { get; set; }
                public PosRuleSetTable[] PosRuleSetTables { get; set; }
            }

            class LkSubTableType7Fmt2 : LookupSubTable
            {
                public short ClassDefOffset { get; set; }
                public CoverageTable CoverageTable { get; set; }
                public PosClassSetTable[] PosClassSetTables { get; set; }

            }
            class LkSubTableType7Fmt3 : LookupSubTable
            {
                public CoverageTable[] CoverageTables { get; set; }
                public PosLookupRecord[] PosLookupRecords { get; set; }
            }
            //----------------------------------------------------------------
            class LkSubTableType8Fmt1 : LookupSubTable
            {

                public CoverageTable CoverageTable { get; set; }
                public PosRuleSetTable[] PosRuleSetTables { get; set; }
            }

            class LkSubTableType8Fmt2 : LookupSubTable
            {
                short[] chainPosClassSetOffsetArray;
                public LkSubTableType8Fmt2(short[] chainPosClassSetOffsetArray)
                {
                    this.chainPosClassSetOffsetArray = chainPosClassSetOffsetArray;
                }
                public CoverageTable CoverageTable { get; set; }
                public PosClassSetTable[] PosClassSetTables { get; set; }

                public short BacktrackClassDefOffset { get; set; }
                public short InputClassDefOffset { get; set; }
                public short LookaheadClassDefOffset { get; set; }
            }
            class LkSubTableType8Fmt3 : LookupSubTable
            {
                public CoverageTable[] BacktrackCoverages { get; set; }
                public CoverageTable[] InputGlyphCoverages { get; set; }
                public CoverageTable[] LookaheadCoverages { get; set; }
                public PosLookupRecord[] PosLookupRecords { get; set; }

                //Chaining Context Positioning Format 3: Coverage-based Chaining Context Glyph Positioning
                //USHORT 	PosFormat 	Format identifier-format = 3
                //USHORT 	BacktrackGlyphCount 	Number of glyphs in the backtracking sequence
                //Offset 	Coverage[BacktrackGlyphCount] 	Array of offsets to coverage tables in backtracking sequence, in glyph sequence order
                //USHORT 	InputGlyphCount 	Number of glyphs in input sequence
                //Offset 	Coverage[InputGlyphCount] 	Array of offsets to coverage tables in input sequence, in glyph sequence order
                //USHORT 	LookaheadGlyphCount 	Number of glyphs in lookahead sequence
                //Offset 	Coverage[LookaheadGlyphCount] 	Array of offsets to coverage tables in lookahead sequence, in glyph sequence order
                //USHORT 	PosCount 	Number of PosLookupRecords
                //struct 	PosLookupRecord[PosCount] 	Array of PosLookupRecords,in design order

            }

            /// <summary>
            /// LookupType 8: Chaining Contextual Positioning Subtable
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType8(BinaryReader reader)
            {
                long thisLookupTablePos = reader.BaseStream.Position;
                int j = subTableOffsets.Length;

                for (int i = 0; i < j; ++i)
                {
                    //move to read pos
                    long subTableStartAt = lookupTablePos + subTableOffsets[i];
                    reader.BaseStream.Seek(subTableStartAt, SeekOrigin.Begin);
                    //-----------------------

                    ushort format = reader.ReadUInt16();
                    switch (format)
                    {
                        default: throw new NotSupportedException();
                        case 1:
                            {
                                //Chaining Context Positioning Format 1: Simple Chaining Context Glyph Positioning
                                // USHORT 	PosFormat 	Format identifier-format = 1
                                //Offset 	Coverage 	Offset to Coverage table-from beginning of ContextPos subtable
                                //USHORT 	ChainPosRuleSetCount 	Number of ChainPosRuleSet tables
                                //Offset 	ChainPosRuleSet
                                //[ChainPosRuleSetCount] 	Array of offsets to ChainPosRuleSet tables-from beginning of ContextPos subtable-ordered by Coverage Index

                                short coverageOffset = reader.ReadInt16();
                                ushort chainPosRuleSetCount = reader.ReadUInt16();
                                short[] chainPosRuleSetOffsetList = Utils.ReadInt16Array(reader, chainPosRuleSetCount);

                                LkSubTableType8Fmt1 subTable = new LkSubTableType8Fmt1();

                                subTable.PosRuleSetTables = CreateMultiplePosRuleSetTables(subTableStartAt, chainPosRuleSetOffsetList, reader);
                                subTable.CoverageTable = CoverageTable.CreateFrom(reader, subTableStartAt + coverageOffset);
                                //----------

                                subTables.Add(subTable);
                            } break;
                        case 2:
                            {
                                //Chaining Context Positioning Format 2: Class-based Chaining Context Glyph Positioning
                                //USHORT 	PosFormat 	Format identifier-format = 2
                                //Offset 	Coverage 	Offset to Coverage table-from beginning of ChainContextPos subtable
                                //Offset 	BacktrackClassDef 	Offset to ClassDef table containing backtrack sequence context-from beginning of ChainContextPos subtable
                                //Offset 	InputClassDef 	Offset to ClassDef table containing input sequence context-from beginning of ChainContextPos subtable
                                //Offset 	LookaheadClassDef 	Offset to ClassDef table containing lookahead sequence context-from beginning of ChainContextPos subtable
                                //USHORT 	ChainPosClassSetCnt 	Number of ChainPosClassSet tables
                                //Offset 	ChainPosClassSet
                                //[ChainPosClassSetCnt] 	Array of offsets to ChainPosClassSet tables-from beginning of ChainContextPos subtable-ordered by input class-may be NULL

                                short coverageOffset = reader.ReadInt16();
                                short backTrackClassDefOffset = reader.ReadInt16();
                                short inpuClassDefOffset = reader.ReadInt16();
                                short lookadheadClassDefOffset = reader.ReadInt16();
                                ushort chainPosClassSetCnt = reader.ReadUInt16();
                                short[] chainPosClassSetOffsetArray = Utils.ReadInt16Array(reader, chainPosClassSetCnt);

                                LkSubTableType8Fmt2 subTable = new LkSubTableType8Fmt2(chainPosClassSetOffsetArray);
                                subTable.BacktrackClassDefOffset = backTrackClassDefOffset;
                                subTable.InputClassDefOffset = inpuClassDefOffset;
                                subTable.LookaheadClassDefOffset = lookadheadClassDefOffset;
                                //----------
                                PosClassSetTable[] posClassSetTables = new PosClassSetTable[chainPosClassSetCnt];
                                subTable.PosClassSetTables = posClassSetTables;
                                for (int n = 0; n < chainPosClassSetCnt; ++n)
                                {
                                    posClassSetTables[n] = PosClassSetTable.CreateFrom(reader, subTableStartAt + chainPosClassSetOffsetArray[i]);
                                }
                                //----------

                                subTable.CoverageTable = CoverageTable.CreateFrom(reader, subTableStartAt + coverageOffset);
                                //----------  
                                subTables.Add(subTable);
                                //----------
                            } break;
                        case 3:
                            {

                                //Chaining Context Positioning Format 3: Coverage-based Chaining Context Glyph Positioning
                                //USHORT 	PosFormat 	Format identifier-format = 3
                                //USHORT 	BacktrackGlyphCount 	Number of glyphs in the backtracking sequence
                                //Offset 	Coverage[BacktrackGlyphCount] 	Array of offsets to coverage tables in backtracking sequence, in glyph sequence order
                                //USHORT 	InputGlyphCount 	Number of glyphs in input sequence
                                //Offset 	Coverage[InputGlyphCount] 	Array of offsets to coverage tables in input sequence, in glyph sequence order
                                //USHORT 	LookaheadGlyphCount 	Number of glyphs in lookahead sequence
                                //Offset 	Coverage[LookaheadGlyphCount] 	Array of offsets to coverage tables in lookahead sequence, in glyph sequence order
                                //USHORT 	PosCount 	Number of PosLookupRecords
                                //struct 	PosLookupRecord[PosCount] 	Array of PosLookupRecords,in design order

                                var subTable = new LkSubTableType8Fmt3();
                                //
                                ushort backtrackGlyphCount = reader.ReadUInt16();
                                short[] backtrackCoverageOffsets = Utils.ReadInt16Array(reader, backtrackGlyphCount);
                                ushort inputGlyphCount = reader.ReadUInt16();
                                short[] inputGlyphCoverageOffsets = Utils.ReadInt16Array(reader, inputGlyphCount);
                                //
                                ushort lookaheadGlyphCount = reader.ReadUInt16();
                                short[] lookaheadCoverageOffsets = Utils.ReadInt16Array(reader, lookaheadGlyphCount);
                                //
                                ushort posCount = reader.ReadUInt16();
                                subTable.PosLookupRecords = CreateMultiplePosLookupRecords(reader, posCount);
                                //--------------

                                subTable.BacktrackCoverages = CoverageTable.CreateMultipleCoverageTables(subTableStartAt, backtrackCoverageOffsets, reader);
                                subTable.InputGlyphCoverages = CoverageTable.CreateMultipleCoverageTables(subTableStartAt, inputGlyphCoverageOffsets, reader);
                                subTable.LookaheadCoverages = CoverageTable.CreateMultipleCoverageTables(subTableStartAt, lookaheadCoverageOffsets, reader);
                                subTables.Add(subTable);

                            } break;
                    }

                }
            }
            /// <summary>
            /// LookupType 9: Extension Positioning
            /// </summary>
            /// <param name="reader"></param>
            void ReadLookupType9(BinaryReader reader)
            {
                //Console.WriteLine("skip lookup type 9");
            }
        }




    }

}
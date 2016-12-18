﻿//Apache2,  2016,  WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//https://www.microsoft.com/typography/otspec/GPOS.htm

namespace NOpenType.Tables
{
    partial class GPOS
    {

        static PosRuleSetTable[] CreateMultiplePosRuleSetTables(long initPos, short[] offsets, BinaryReader reader)
        {
            int j = offsets.Length;
            PosRuleSetTable[] results = new PosRuleSetTable[j];
            for (int i = 0; i < j; ++i)
            {
                results[i] = PosRuleSetTable.CreateFrom(reader, initPos + offsets[i]);
            }
            return results;
        }

        static PosLookupRecord[] CreateMultiplePosLookupRecords(BinaryReader reader, int count)
        {

            PosLookupRecord[] results = new PosLookupRecord[count];
            for (int n = 0; n < count; ++n)
            {
                results[n] = PosLookupRecord.CreateFrom(reader);
            }
            return results;
        }


        class PairSetTable
        {
            List<PairSet> pairSets = new List<PairSet>();
            public void ReadFrom(BinaryReader reader, ushort v1format, ushort v2format)
            {
                ushort rowCount = reader.ReadUInt16();
                for (int i = 0; i < rowCount; ++i)
                {
                    //GlyphID 	SecondGlyph 	GlyphID of second glyph in the pair-first glyph is listed in the Coverage table
                    //ValueRecord 	Value1 	Positioning data for the first glyph in the pair
                    //ValueRecord 	Value2 	Positioning data for the second glyph in the pair
                    ushort secondGlyp = reader.ReadUInt16();
                    ValueRecord v1 = ValueRecord.CreateFrom(reader, v1format);
                    ValueRecord v2 = ValueRecord.CreateFrom(reader, v2format);
                    PairSet pset = new PairSet(secondGlyp, v1, v2);
                }
            }
        }


        struct PairSet
        {
            public readonly ushort secondGlyph;//GlyphID of second glyph in the pair-first glyph is listed in the Coverage table
            public readonly ValueRecord value1;//Positioning data for the first glyph in the pair
            public readonly ValueRecord value2;//Positioning data for the second glyph in the pair   
            public PairSet(ushort secondGlyph, ValueRecord v1, ValueRecord v2)
            {
                this.secondGlyph = secondGlyph;
                this.value1 = v1;
                this.value2 = v2;
            }
        }


        class ValueRecord
        {
            //ValueRecord (all fields are optional)
            //Value 	Type 	Description
            //SHORT 	XPlacement 	Horizontal adjustment for placement-in design units
            //SHORT 	YPlacement 	Vertical adjustment for placement, in design units
            //SHORT 	XAdvance 	Horizontal adjustment for advance, in design units (only used for horizontal writing)
            //SHORT 	YAdvance 	Vertical adjustment for advance, in design units (only used for vertical writing)
            //Offset 	XPlaDevice 	Offset to Device table (non-variable font) / VariationIndex table (variable font) for horizontal placement, from beginning of PosTable (may be NULL)
            //Offset 	YPlaDevice 	Offset to Device table (non-variable font) / VariationIndex table (variable font) for vertical placement, from beginning of PosTable (may be NULL)
            //Offset 	XAdvDevice 	Offset to Device table (non-variable font) / VariationIndex table (variable font) for horizontal advance, from beginning of PosTable (may be NULL)
            //Offset 	YAdvDevice 	Offset to Device table (non-variable font) / VariationIndex table (variable font) for vertical advance, from beginning of PosTable (may be NULL)

            public short XPlacement;
            public short YPlacement;
            public short XAdvance;
            public short YAdvance;
            public short XPlaDevice;
            public short YPlaDevice;
            public short XAdvDevice;
            public short YAdvDevice;

            public ushort valueFormat;
            public void ReadFrom(BinaryReader reader, ushort valueFormat)
            {
                this.valueFormat = valueFormat;
                if (HasFormat(valueFormat, FMT_XPlacement))
                {
                    this.XPlacement = reader.ReadInt16();
                }
                if (HasFormat(valueFormat, FMT_YPlacement))
                {
                    this.YPlacement = reader.ReadInt16();
                }
                if (HasFormat(valueFormat, FMT_XAdvance))
                {
                    this.XAdvance = reader.ReadInt16();
                }
                if (HasFormat(valueFormat, FMT_YAdvance))
                {
                    this.YAdvance = reader.ReadInt16();
                }
                if (HasFormat(valueFormat, FMT_XPlaDevice))
                {
                    this.XPlaDevice = reader.ReadInt16();
                }
                if (HasFormat(valueFormat, FMT_YPlaDevice))
                {
                    this.YPlaDevice = reader.ReadInt16();
                }
                if (HasFormat(valueFormat, FMT_XAdvDevice))
                {
                    this.XAdvDevice = reader.ReadInt16();
                }
                if (HasFormat(valueFormat, FMT_YAdvDevice))
                {
                    this.YAdvDevice = reader.ReadInt16();
                }
            }
            static bool HasFormat(ushort value, int flags)
            {
                return (value & flags) == flags;
            }
            //Mask 	Name 	Description
            //0x0001 	XPlacement 	Includes horizontal adjustment for placement
            //0x0002 	YPlacement 	Includes vertical adjustment for placement
            //0x0004 	XAdvance 	Includes horizontal adjustment for advance
            //0x0008 	YAdvance 	Includes vertical adjustment for advance
            //0x0010 	XPlaDevice 	Includes Device table (non-variable font) / VariationIndex table (variable font) for horizontal placement
            //0x0020 	YPlaDevice 	Includes Device table (non-variable font) / VariationIndex table (variable font) for vertical placement
            //0x0040 	XAdvDevice 	Includes Device table (non-variable font) / VariationIndex table (variable font) for horizontal advance
            //0x0080 	YAdvDevice 	Includes Device table (non-variable font) / VariationIndex table (variable font) for vertical advance
            //0xFF00 	Reserved 	For future use (set to zero)

            //check bits
            const int FMT_XPlacement = 1;
            const int FMT_YPlacement = 1 << 1;
            const int FMT_XAdvance = 1 << 2;
            const int FMT_YAdvance = 1 << 3;
            const int FMT_XPlaDevice = 1 << 4;
            const int FMT_YPlaDevice = 1 << 5;
            const int FMT_XAdvDevice = 1 << 6;
            const int FMT_YAdvDevice = 1 << 7;

            public static ValueRecord CreateFrom(BinaryReader reader, ushort valueFormat)
            {
                var v = new ValueRecord();
                v.ReadFrom(reader, valueFormat);
                return v;
            }
        }


        class AnchorTable
        {
            //Anchor Table

            //A GPOS table uses anchor points to position one glyph with respect to another.
            //Each glyph defines an anchor point, and the text-processing client attaches the glyphs by aligning their corresponding anchor points.

            //To describe an anchor point, an Anchor table can use one of three formats. 
            //The first format uses design units to specify a location for the anchor point.
            //The other two formats refine the location of the anchor point using contour points (Format 2) or Device tables (Format 3). 
            //In a variable font, the third format uses a VariationIndex table (a variant of a Device table) to 
            //reference variation data for adjustment of the anchor position for the current variation instance, as needed. 

            ushort format;
            public void ReadFrom(BinaryReader reader)
            {
                long anchorTableStartAt = reader.BaseStream.Position;

                switch (this.format = reader.ReadUInt16())
                {
                    default: throw new NotSupportedException();
                    case 1:
                        {
                            // AnchorFormat1 table: Design units only
                            //AnchorFormat1 consists of a format identifier (AnchorFormat) and a pair of design unit coordinates (XCoordinate and YCoordinate)
                            //that specify the location of the anchor point. 
                            //This format has the benefits of small size and simplicity,
                            //but the anchor point cannot be hinted to adjust its position for different device resolutions.
                            //Value 	Type 	Description
                            //USHORT 	AnchorFormat 	Format identifier, = 1
                            //SHORT 	XCoordinate 	Horizontal value, in design units
                            //SHORT 	YCoordinate 	Vertical value, in design units
                            short xcoord = reader.ReadInt16();
                            short ycoord = reader.ReadInt16();

                        }
                        break;
                    case 2:
                        {
                            //Anchor Table: Format 2

                            //Like AnchorFormat1, AnchorFormat2 specifies a format identifier (AnchorFormat) and
                            //a pair of design unit coordinates for the anchor point (Xcoordinate and Ycoordinate).

                            //For fine-tuning the location of the anchor point, AnchorFormat2 also provides an index to a glyph contour point (AnchorPoint) 
                            //that is on the outline of a glyph (AnchorPoint).
                            //Hinting can be used to move the AnchorPoint. In the rendered text,
                            //the AnchorPoint will provide the final positioning data for a given ppem size.

                            //Example 16 at the end of this chapter uses AnchorFormat2.


                            //AnchorFormat2 table: Design units plus contour point
                            //Value 	Type 	Description
                            //USHORT 	AnchorFormat 	Format identifier, = 2
                            //SHORT 	XCoordinate 	Horizontal value, in design units
                            //SHORT 	YCoordinate 	Vertical value, in design units
                            //USHORT 	AnchorPoint 	Index to glyph contour point

                            short xcoord = reader.ReadInt16();
                            short ycoord = reader.ReadInt16();
                            ushort anchorPoint = reader.ReadUInt16();


                        }
                        break;
                    case 3:
                        {

                            //Anchor Table: Format 3

                            //Like AnchorFormat1, AnchorFormat3 specifies a format identifier (AnchorFormat) and 
                            //locates an anchor point (Xcoordinate and Ycoordinate).
                            //And, like AnchorFormat 2, it permits fine adjustments in variable fonts to the coordinate values. 
                            //However, AnchorFormat3 uses Device tables, rather than a contour point, for this adjustment.

                            //With a Device table, a client can adjust the position of the anchor point for any font size and device resolution.
                            //AnchorFormat3 can specify offsets to Device tables for the the X coordinate (XDeviceTable) 
                            //and the Y coordinate (YDeviceTable). 
                            //If only one coordinate requires adjustment, 
                            //the offset to the Device table may be set to NULL for the other coordinate.

                            //In variable fonts, AnchorFormat3 must be used to reference variation data to adjust anchor points for different variation instances,
                            //if needed.
                            //In this case, AnchorFormat3 specifies an offset to a VariationIndex table,
                            //which is a variant of the Device table used for variations.
                            //If no VariationIndex table is used for a particular anchor point X or Y coordinate, 
                            //then that value is used for all variation instances.
                            //While separate VariationIndex table references are required for each value that requires variation,
                            //two or more values that require the same variation-data values can have offsets that point to the same VariationIndex table, and two or more VariationIndex tables can reference the same variation data entries.

                            //Example 17 at the end of the chapter shows an AnchorFormat3 table.


                            //AnchorFormat3 table: Design units plus Device or VariationIndex tables
                            //Value 	Type 	Description
                            //USHORT 	AnchorFormat 	Format identifier, = 3
                            //SHORT 	XCoordinate 	Horizontal value, in design units
                            //SHORT 	YCoordinate 	Vertical value, in design units
                            //Offset 	XDeviceTable 	Offset to Device table (non-variable font) / VariationIndex table (variable font) for X coordinate, from beginning of Anchor table (may be NULL)
                            //Offset 	YDeviceTable 	Offset to Device table (non-variable font) / VariationIndex table (variable font) for Y coordinate, from beginning of Anchor table (may be NULL)

                            short xcoord = reader.ReadInt16();
                            short ycoord = reader.ReadInt16();
                            short xdeviceTableOffset = reader.ReadInt16();
                            short ydeviceTableOffset = reader.ReadInt16();


                        }
                        break;
                }

            }
        }


        class MarkArrayTable
        {
            //Mark Array
            //The MarkArray table defines the class and the anchor point for a mark glyph. 
            //Three GPOS subtables-MarkToBase, MarkToLigature, 
            //and MarkToMark Attachment-use the MarkArray table to specify data for attaching marks.

            //The MarkArray table contains a count of the number of mark records (MarkCount) and an array of those records (MarkRecord).
            //Each mark record defines the class of the mark and an offset to the Anchor table that contains data for the mark.

            //A class value can be 0 (zero), but the MarkRecord must explicitly assign that class value (this differs from the ClassDef table, 
            //in which all glyphs not assigned class values automatically belong to Class 0).
            //The GPOS subtables that refer to MarkArray tables use the class assignments for indexing zero-based arrays that contain data for each mark class.

            // MarkArray table
            //Value 	Type 	Description
            //USHORT 	MarkCount 	Number of MarkRecords
            //struct 	MarkRecord
            //[MarkCount] 	Array of MarkRecords-in Coverage order
            //MarkRecord
            //Value 	Type 	Description
            //USHORT 	Class 	Class defined for this mark
            //Offset 	MarkAnchor 	Offset to Anchor table-from beginning of MarkArray table
            MarkRecord[] records;
            void ReadFrom(BinaryReader reader)
            {
                ushort markCount = reader.ReadUInt16();
                records = new MarkRecord[markCount];
                for (int i = 0; i < markCount; ++i)
                {
                    records[i] = new MarkRecord(
                        reader.ReadUInt16(),
                        reader.ReadInt16());
                }
            }
            public static MarkArrayTable CreateFrom(BinaryReader reader, long beginAt)
            {
                reader.BaseStream.Seek(beginAt, SeekOrigin.Begin);
                //
                var markArrTable = new MarkArrayTable();
                markArrTable.ReadFrom(reader);
                return markArrTable;
            }
        }

        struct MarkRecord
        {
            /// <summary>
            /// Class defined for this mark
            /// </summary>
            public readonly ushort markClass;
            /// <summary>
            /// Offset to Anchor table-from beginning of MarkArray table
            /// </summary>
            public readonly short offset;
            public MarkRecord(ushort markClass, short offset)
            {
                this.markClass = markClass;
                this.offset = offset;
            }
#if DEBUG
            public override string ToString()
            {
                return "class " + markClass + ",offset=" + offset;
            }
#endif
        }

        class Mark2ArrayTable
        {
            ///Mark2Array table
            //Value 	Type 	Description
            //USHORT 	Mark2Count 	Number of Mark2 records
            //struct 	Mark2Record
            //[Mark2Count] 	Array of Mark2 records-in Coverage order

            //Each Mark2Record contains an array of offsets to Anchor tables (Mark2Anchor). The array of zero-based offsets, measured from the beginning of the Mark2Array table, defines the entire set of Mark2 attachment points used to attach Mark1 glyphs to a specific Mark2 glyph. The Anchor tables in the Mark2Anchor array are ordered by Mark1 class value.

            //A Mark2Record declares one Anchor table for each mark class (including Class 0) identified in the MarkRecords of the MarkArray. Each Anchor table specifies one Mark2 attachment point used to attach all the Mark1 glyphs in a particular class to the Mark2 glyph.

            Mark2Record[] mark2Records;
            public static Mark2ArrayTable CreateFrom(BinaryReader reader, long beginAt, ushort classCount)
            {
                reader.BaseStream.Seek(beginAt, SeekOrigin.Begin);
                //---
                var mark2ArrTable = new Mark2ArrayTable();

                ushort mark2Count = reader.ReadUInt16();
                mark2ArrTable.mark2Records = new Mark2Record[mark2Count];
                for (int i = 0; i < mark2Count; ++i)
                {
                    mark2ArrTable.mark2Records[i] = new Mark2Record(
                        Utils.ReadInt16Array(reader, classCount));
                }
                return mark2ArrTable;
            }
        }

        struct Mark2Record
        {
            //Mark2Record
            //Value 	Type 	Description
            //Offset 	Mark2Anchor
            //[ClassCount] 	Array of offsets (one per class) to Anchor tables-from beginning of Mark2Array table-zero-based array
            public readonly short[] offsets;
            public Mark2Record(short[] offsets)
            {
                this.offsets = offsets;
            }
        }


        class BaseArrayTable
        {
            BaseRecord[] records;

            public static BaseArrayTable CreateFrom(BinaryReader reader, long beginAt, ushort classCount)
            {
                reader.BaseStream.Seek(beginAt, SeekOrigin.Begin);
                //---
                var baseArrTable = new BaseArrayTable();

                ushort baseCount = reader.ReadUInt16();
                baseArrTable.records = new BaseRecord[baseCount];
                for (int i = 0; i < baseCount; ++i)
                {
                    baseArrTable.records[i] = new BaseRecord(Utils.ReadInt16Array(reader, classCount));
                }

                return baseArrTable;
            }
        }
        struct BaseRecord
        {
            public short[] offsets;
            public BaseRecord(short[] offsets)
            {
                this.offsets = offsets;
            }

        }


        // LigatureArray table
        //Value 	Type 	Description
        //USHORT 	LigatureCount 	Number of LigatureAttach table offsets
        //Offset 	LigatureAttach
        //[LigatureCount] 	Array of offsets to LigatureAttach tables-from beginning of LigatureArray table-ordered by LigatureCoverage Index

        //Each LigatureAttach table consists of an array (ComponentRecord) and count (ComponentCount) of the component glyphs in a ligature. The array stores the ComponentRecords in the same order as the components in the ligature. The order of the records also corresponds to the writing direction of the text. For text written left to right, the first component is on the left; for text written right to left, the first component is on the right.
        //LigatureAttach table
        //Value 	Type 	Description
        //USHORT 	ComponentCount 	Number of ComponentRecords in this ligature
        //struct 	ComponentRecord[ComponentCount] 	Array of Component records-ordered in writing direction

        //A ComponentRecord, one for each component in the ligature, contains an array of offsets to the Anchor tables that define all the attachment points used to attach marks to the component (LigatureAnchor). For each mark class (including Class 0) identified in the MarkArray records, an Anchor table specifies the point used to attach all the marks in a particular class to the ligature base glyph, relative to the component.

        //In a ComponentRecord, the zero-based LigatureAnchor array lists offsets to Anchor tables by mark class. If a component does not define an attachment point for a particular class of marks, then the offset to the corresponding Anchor table will be NULL.

        //Example 8 at the end of this chapter shows a MarkLisPosFormat1 subtable used to attach mark accents to a ligature glyph in the Arabic script.
        //ComponentRecord
        //Value 	Type 	Description
        //Offset 	LigatureAnchor
        //[ClassCount] 	Array of offsets (one per class) to Anchor tables-from beginning of LigatureAttach table-ordered by class-NULL if a component does not have an attachment for a class-zero-based array
        class LigatureArrayTable
        {
            LigatureAttachTable[] ligatures;
            public void ReadFrom(BinaryReader reader, ushort classCount)
            {
                long startPos = reader.BaseStream.Position;
                ushort ligatureCount = reader.ReadUInt16();
                short[] offsets = Utils.ReadInt16Array(reader, ligatureCount);

                ligatures = new LigatureAttachTable[ligatureCount];

                for (int i = 0; i < ligatureCount; ++i)
                {
                    //each ligature table
                    reader.BaseStream.Seek(startPos + offsets[i], SeekOrigin.Begin);
                    ligatures[i] = LigatureAttachTable.ReadFrom(reader, classCount);
                }
            }
        }
        class LigatureAttachTable
        {
            ComponentRecord[] records;
            public static LigatureAttachTable ReadFrom(BinaryReader reader, ushort classCount)
            {
                LigatureAttachTable table = new LigatureAttachTable();
                ushort componentCount = reader.ReadUInt16();
                ComponentRecord[] componentRecs = new ComponentRecord[componentCount];
                table.records = componentRecs;
                for (int i = 0; i < componentCount; ++i)
                {
                    componentRecs[i] = new ComponentRecord(
                        Utils.ReadInt16Array(reader, classCount));
                }
                return table;
            }

        }
        struct ComponentRecord
        {
            public short[] offsets;
            public ComponentRecord(short[] offsets)
            {
                this.offsets = offsets;
            }

        }

        //------


        struct PosLookupRecord
        {


            //PosLookupRecord
            //Value 	Type 	Description
            //USHORT 	SequenceIndex 	Index to input glyph sequence-first glyph = 0
            //USHORT 	LookupListIndex 	Lookup to apply to that position-zero-based

            public readonly ushort seqIndex;
            public readonly ushort lookupListIndex;
            public PosLookupRecord(ushort seqIndex, ushort lookupListIndex)
            {
                this.seqIndex = seqIndex;
                this.lookupListIndex = lookupListIndex;
            }
            public static PosLookupRecord CreateFrom(BinaryReader reader)
            {
                return new PosLookupRecord(reader.ReadUInt16(), reader.ReadUInt16());
            }
        }


        class PosRuleSetTable
        {

            //PosRuleSet table: All contexts beginning with the same glyph
            // Value 	Type 	Description
            //USHORT 	PosRuleCount 	Number of PosRule tables
            //Offset 	PosRule
            //[PosRuleCount] 	Array of offsets to PosRule tables-from beginning of PosRuleSet-ordered by preference
            //
            //A PosRule table consists of a count of the glyphs to be matched in the input context sequence (GlyphCount), 
            //including the first glyph in the sequence, and an array of glyph indices that describe the context (Input). 
            //The Coverage table specifies the index of the first glyph in the context, and the Input array begins with the second glyph in the context sequence. As a result, the first index position in the array is specified with the number one (1), not zero (0). The Input array lists the indices in the order the corresponding glyphs appear in the text. For text written from right to left, the right-most glyph will be first; conversely, for text written from left to right, the left-most glyph will be first.

            //A PosRule table also contains a count of the positioning operations to be performed on the input glyph sequence (PosCount) and an array of PosLookupRecords (PosLookupRecord). Each record specifies a position in the input glyph sequence and a LookupList index to the positioning lookup to be applied there. The array should list records in design order, or the order the lookups should be applied to the entire glyph sequence.

            //Example 10 at the end of this chapter demonstrates glyph kerning in context with a ContextPosFormat1 subtable.

            PosRuleTable[] posRuleTables;
            void ReadFrom(BinaryReader reader)
            {
                long tableStartAt = reader.BaseStream.Position;
                ushort posRuleCount = reader.ReadUInt16();
                short[] posRuleTableOffsets = Utils.ReadInt16Array(reader, posRuleCount);
                int j = posRuleTableOffsets.Length;
                posRuleTables = new PosRuleTable[posRuleCount];
                for (int i = 0; i < j; ++i)
                {
                    //move to and read
                    reader.BaseStream.Seek(tableStartAt + posRuleTableOffsets[i], SeekOrigin.Begin);
                    var posRuleTable = new PosRuleTable();
                    posRuleTable.ReadFrom(reader);
                    posRuleTables[i] = posRuleTable;

                }
            }

            public static PosRuleSetTable CreateFrom(BinaryReader reader, long beginAt)
            {
                reader.BaseStream.Seek(beginAt, SeekOrigin.Begin);
                //------------
                var posRuleSetTable = new PosRuleSetTable();
                posRuleSetTable.ReadFrom(reader);
                return posRuleSetTable;
            }
        }
        class PosRuleTable
        {

            //PosRule subtable
            //Value 	Type 	Description
            //USHORT 	GlyphCount 	Number of glyphs in the Input glyph sequence
            //USHORT 	PosCount 	Number of PosLookupRecords
            //GlyphID 	Input[GlyphCount - 1]  Array of input GlyphIDs-starting with the second glyph***
            //struct 	PosLookupRecord[PosCount] 	Array of positioning lookups-in design order
            PosLookupRecord[] posLookupRecords;
            ushort[] inputGlyphIds;
            public void ReadFrom(BinaryReader reader)
            {
                ushort glyphCount = reader.ReadUInt16();
                ushort posCount = reader.ReadUInt16();
                inputGlyphIds = Utils.ReadUInt16Array(reader, glyphCount - 1);
                posLookupRecords = CreateMultiplePosLookupRecords(reader, posCount);
            }
        }


        class PosClassSetTable
        {
            // PosClassSet table: All contexts beginning with the same class
            //Value 	Type 	Description
            //USHORT 	PosClassRuleCnt 	Number of PosClassRule tables
            //Offset 	PosClassRule[PosClassRuleCnt] 	Array of offsets to PosClassRule tables-from beginning of PosClassSet-ordered by preference

            //For each context, a PosClassRule table contains a count of the glyph classes in a given context (GlyphCount), including the first class in the context sequence. A class array lists the classes, beginning with the second class, that follow the first class in the context. The first class listed indicates the second position in the context sequence.

            //    Note: Text order depends on the writing direction of the text. For text written from right to left, the right-most glyph will be first. Conversely, for text written from left to right, the left-most glyph will be first.

            //The values specified in the Class array are those defined in the ClassDef table. For example, consider a context consisting of the sequence: Class 2, Class 7, Class 5, Class 0. The Class array will read: Class[0] = 7, Class[1] = 5, and Class[2] = 0. The first class in the sequence, Class 2, is defined by the index into the PosClassSet array of offsets. The total number and sequence of glyph classes listed in the Class array must match the total number and sequence of glyph classes contained in the input context.

            //A PosClassRule also contains a count of the positioning operations to be performed on the context (PosCount) and an array of PosLookupRecords (PosLookupRecord) that supply the positioning data. For each position in the context that requires a positioning operation, a PosLookupRecord specifies a LookupList index and a position in the input glyph class sequence where the lookup is applied. The PosLookupRecord array lists PosLookupRecords in design order, or the order in which lookups are applied to the entire glyph sequence.

            //Example 11 at the end of this chapter demonstrates a ContextPosFormat2 subtable that uses glyph classes to modify accent positions in glyph strings.
            //PosClassRule table: One class context definition
            //Value 	Type 	Description
            //USHORT 	GlyphCount 	Number of glyphs to be matched
            //USHORT 	PosCount 	Number of PosLookupRecords
            //USHORT 	Class[GlyphCount - 1] 	Array of classes-beginning with the second class-to be matched to the input glyph sequence
            //struct 	PosLookupRecord[PosCount] 	Array of positioning lookups-in design order

            PosClassRule[] posClasses;
            void ReadFrom(BinaryReader reader)
            {
                long tableStartAt = reader.BaseStream.Position;
                //
                ushort posClassRuleCnt = reader.ReadUInt16();
                short[] posClassRuleOffsets = Utils.ReadInt16Array(reader, posClassRuleCnt);
                int j = posClassRuleOffsets.Length;

                posClasses = new PosClassRule[posClassRuleCnt];
                for (int i = 0; i < j; ++i)
                {
                    //move to and read                     
                    posClasses[i] = PosClassRule.CreateFrom(reader, tableStartAt = posClassRuleOffsets[i]);
                }
            }

            public static PosClassSetTable CreateFrom(BinaryReader reader, long beginAt)
            {
                reader.BaseStream.Seek(beginAt, SeekOrigin.Begin);
                //--------
                var posClassSetTable = new PosClassSetTable();
                posClassSetTable.ReadFrom(reader);
                return posClassSetTable;
            }
        }
        class PosClassRule
        {
            PosLookupRecord[] posLookupRecords;
            ushort[] inputGlyphIds;

            public static PosClassRule CreateFrom(BinaryReader reader, long beginAt)
            {
                //--------
                reader.BaseStream.Seek(beginAt, SeekOrigin.Begin);
                //--------
                PosClassRule posClassRule = new PosClassRule();
                ushort glyphCount = reader.ReadUInt16();
                ushort posCount = reader.ReadUInt16();
                posClassRule.inputGlyphIds = Utils.ReadUInt16Array(reader, glyphCount - 1);
                posClassRule.posLookupRecords = CreateMultiplePosLookupRecords(reader, posCount);
                return posClassRule;
            }
        }


    }

}
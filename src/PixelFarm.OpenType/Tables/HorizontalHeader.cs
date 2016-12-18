﻿//Apache2, 2014-2016, Samuel Carlsson, WinterDev

using System;
using System.IO;
namespace NOpenType.Tables
{
    class HorizontalHeader : TableEntry
    {

        public HorizontalHeader()
        {
        }
        public override string Name
        {
            get { return "hhea"; }
        }
        protected override void ReadContentFrom(BinaryReader input)
        {
            Version = input.ReadUInt32();
            Ascent = input.ReadInt16();
            Descent = input.ReadInt16();
            LineGap = input.ReadInt16();
            AdvancedWidthMax = input.ReadUInt16();
            MinLeftSideBearing = input.ReadInt16();
            MinRightSideBearing = input.ReadInt16();
            MaxXExtent = input.ReadInt16();
            CaretSlopRise = input.ReadInt16();
            CaretSlopRun = input.ReadInt16();
            Reserved(input.ReadInt16());
            Reserved(input.ReadInt16());
            Reserved(input.ReadInt16());
            Reserved(input.ReadInt16());
            Reserved(input.ReadInt16());
            MatricDataFormat = input.ReadInt16(); // 0
            HorizontalMetricsCount = input.ReadUInt16();
        }
        public uint Version { get; private set; }
        public short Ascent { get; private set; }
        public short Descent { get; private set; }
        public short LineGap { get; private set; }
        public ushort AdvancedWidthMax { get; private set; }
        public short MinLeftSideBearing { get; private set; }
        public short MinRightSideBearing { get; private set; }
        public short MaxXExtent { get; private set; }
        public short CaretSlopRise { get; private set; }
        public short CaretSlopRun { get; private set; }
        public short MatricDataFormat { get; private set; }
        public ushort HorizontalMetricsCount { get; private set; }
        void Reserved(short zero)
        {
            // should be zero
        }
    }
}

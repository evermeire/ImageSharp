﻿//Apache2, 2014-2016, Samuel Carlsson, WinterDev

using System;
using System.IO;
namespace NOpenType.Tables
{
    /// <summary>
    /// this is base class of all 'top' font table
    /// </summary>
    abstract class TableEntry
    {
        public TableEntry()
        {
        }
        public TableHeader Header { get; set; }
        protected abstract void ReadContentFrom(BinaryReader reader);
        public abstract string Name { get; }
        public void LoadDataFrom(BinaryReader reader)
        {
            reader.BaseStream.Seek(this.Header.Offset, SeekOrigin.Begin);
            ReadContentFrom(reader);
        }
        public uint TableLength
        {
            get { return this.Header.Length; }
        }

    }
    class UnreadTableEntry : TableEntry
    {
        public UnreadTableEntry(TableHeader header)
        {
            this.Header = header;
        }
        public override string Name
        {
            get { return this.Header.Tag; }
        }
        protected override void ReadContentFrom(BinaryReader reader)
        {
            //intend ***
            throw new NotImplementedException();
        }
#if DEBUG
        public override string ToString()
        {
            return this.Name;
        }
#endif
    }
}
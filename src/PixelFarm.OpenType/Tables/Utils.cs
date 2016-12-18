﻿//Apache2, 2014-2016, Samuel Carlsson, WinterDev

using System;
using System.Text;
using System.IO;
namespace NOpenType.Tables
{
    static class Utils
    {


        public static string TagToString(uint tag)
        {
            byte[] bytes = BitConverter.GetBytes(tag);
            Array.Reverse(bytes);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
        public static short[] ReadInt16Array(BinaryReader reader, int nRecords)
        {
            short[] arr = new short[nRecords];
            for (int i = 0; i < nRecords; ++i)
            {
                arr[i] = reader.ReadInt16();
            }
            return arr;
        }
        public static ushort[] ReadUInt16Array(BinaryReader reader, int nRecords)
        {
            ushort[] arr = new ushort[nRecords];
            for (int i = 0; i < nRecords; ++i)
            {
                arr[i] = reader.ReadUInt16();
            }
            return arr;
        }
      
    }
}
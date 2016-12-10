﻿// <copyright file="JpegDecoderCore.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Formats
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Performs the jpeg decoding operation.
    /// </summary>
    internal unsafe class JpegDecoderCore : IDisposable
    {
        /// <summary>
        /// The maximum (inclusive) number of bits in a Huffman code.
        /// </summary>
        internal const int MaxCodeLength = 16;

        /// <summary>
        /// The maximum (inclusive) number of codes in a Huffman tree.
        /// </summary>
        internal const int MaxNCodes = 256;

        /// <summary>
        /// The log-2 size of the Huffman decoder's look-up table.
        /// </summary>
        internal const int LutSize = 8;

        /// <summary>
        /// The maximum number of color components
        /// </summary>
        private const int MaxComponents = 4;

        /// <summary>
        /// The maximum number of Huffman table classes
        /// </summary>
        private const int MaxTc = 1;

        /// <summary>
        /// The maximum number of Huffman table identifiers
        /// </summary>
        private const int MaxTh = 3;

        private const int ThRowSize = MaxTh + 1;

        /// <summary>
        /// The maximum number of quantization tables
        /// </summary>
        private const int MaxTq = 3;

        /// <summary>
        /// The DC table index
        /// </summary>
        private const int DcTable = 0;

        /// <summary>
        /// The AC table index
        /// </summary>
        private const int AcTable = 1;

        /// <summary>
        /// Unzig maps from the zigzag ordering to the natural ordering. For example,
        /// unzig[3] is the column and row of the fourth element in zigzag order. The
        /// value is 16, which means first column (16%8 == 0) and third row (16/8 == 2).
        /// </summary>
        private static readonly int[] Unzig =
            {
                0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18, 11, 4, 5, 12, 19, 26, 33,
                40, 48, 41, 34, 27, 20, 13, 6, 7, 14, 21, 28, 35, 42, 49, 56, 57, 50,
                43, 36, 29, 22, 15, 23, 30, 37, 44, 51, 58, 59, 52, 45, 38, 31, 39, 46,
                53, 60, 61, 54, 47, 55, 62, 63,
            };

        /// <summary>
        /// The component array
        /// </summary>
        private readonly Component[] componentArray;

        /// <summary>
        /// Saved state between progressive-mode scans.
        /// </summary>
        private readonly Block8x8F[][] progCoeffs;

        /// <summary>
        /// The huffman trees
        /// </summary>
        private readonly Huffman[] huffmanTrees;

        /// <summary>
        /// Quantization tables, in zigzag order.
        /// </summary>
        private readonly Block8x8F[] quantizationTables;

        /// <summary>
        /// A temporary buffer for holding pixels
        /// </summary>
        private readonly byte[] temp;

        /// <summary>
        /// The byte buffer.
        /// </summary>
        internal Bytes bytes;

        /// <summary>
        /// The image width
        /// </summary>
        private int imageWidth;

        /// <summary>
        /// The image height
        /// </summary>
        private int imageHeight;

        /// <summary>
        /// The number of color components within the image.
        /// </summary>
        private int componentCount;

        /// <summary>
        /// A grayscale image to decode to.
        /// </summary>
        private GrayImage grayImage;

        /// <summary>
        /// The full color image to decode to.
        /// </summary>
        private YCbCrImage ycbcrImage;

        /// <summary>
        /// The input stream.
        /// </summary>
        internal Stream inputStream;

        /// <summary>
        /// Holds the unprocessed bits that have been taken from the byte-stream.
        /// </summary>
        internal Bits bits;

        /// <summary>
        /// The array of keyline pixels in a CMYK image
        /// </summary>
        private byte[] blackPixels;

        /// <summary>
        /// The width in bytes or a single row of keyline pixels in a CMYK image
        /// </summary>
        private int blackStride;

        /// <summary>
        /// The restart interval
        /// </summary>
        private int restartInterval;

        /// <summary>
        /// Whether the image is interlaced (progressive)
        /// </summary>
        private bool isProgressive;

        /// <summary>
        /// Whether the image has a JFIF header
        /// </summary>
        private bool isJfif;

        /// <summary>
        /// Whether the image is in CMYK format with an App14 marker
        /// </summary>
        private bool adobeTransformValid;

        /// <summary>
        /// The App14 marker color-space
        /// </summary>
        private byte adobeTransform;

        /// <summary>
        /// End-of-Band run, specified in section G.1.2.2.
        /// </summary>
        private ushort eobRun;

        /// <summary>
        /// The horizontal resolution. Calculated if the image has a JFIF header.
        /// </summary>
        private short horizontalResolution;

        /// <summary>
        /// The vertical resolution. Calculated if the image has a JFIF header.
        /// </summary>
        private short verticalResolution;

        private int blockIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="JpegDecoderCore"/> class.
        /// </summary>
        public JpegDecoderCore()
        {
            //this.huffmanTrees = new Huffman[MaxTc + 1, MaxTh + 1];
            this.huffmanTrees = new Huffman[(MaxTc + 1) * (MaxTh + 1)];

            this.quantizationTables = new Block8x8F[MaxTq + 1];
            this.temp = new byte[2 * BlockF.BlockSize];
            this.componentArray = new Component[MaxComponents];
            this.progCoeffs = new Block8x8F[MaxComponents][];
            this.bits = new Bits();
            this.bytes = Bytes.Create();

            // TODO: This looks like it could be static.
            for (int i = 0; i < MaxTc + 1; i++)
            {
                for (int j = 0; j < MaxTh + 1; j++)
                {
                    this.huffmanTrees[i * ThRowSize + j].Init(LutSize, MaxNCodes, MaxCodeLength);
                }
            }
        }

        /// <summary>
        /// Decodes the image from the specified this._stream and sets
        /// the data to image.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="image">The image, where the data should be set to.</param>
        /// <param name="stream">The stream, where the image should be.</param>
        /// <param name="configOnly">Whether to decode metadata only.</param>
        public void Decode<TColor, TPacked>(Image<TColor, TPacked> image, Stream stream, bool configOnly)
            where TColor : struct, IPackedPixel<TPacked>
            where TPacked : struct
        {
            this.inputStream = stream;

            // Check for the Start Of Image marker.
            this.ReadFull(this.temp, 0, 2);
            if (this.temp[0] != JpegConstants.Markers.XFF || this.temp[1] != JpegConstants.Markers.SOI)
            {
                throw new ImageFormatException("Missing SOI marker.");
            }

            // Process the remaining segments until the End Of Image marker.
            while (true)
            {
                this.ReadFull(this.temp, 0, 2);
                while (this.temp[0] != 0xff)
                {
                    // Strictly speaking, this is a format error. However, libjpeg is
                    // liberal in what it accepts. As of version 9, next_marker in
                    // jdmarker.c treats this as a warning (JWRN_EXTRANEOUS_DATA) and
                    // continues to decode the stream. Even before next_marker sees
                    // extraneous data, jpeg_fill_bit_buffer in jdhuff.c reads as many
                    // bytes as it can, possibly past the end of a scan's data. It
                    // effectively puts back any markers that it overscanned (e.g. an
                    // "\xff\xd9" EOI marker), but it does not put back non-marker data,
                    // and thus it can silently ignore a small number of extraneous
                    // non-marker bytes before next_marker has a chance to see them (and
                    // print a warning).
                    // We are therefore also liberal in what we accept. Extraneous data
                    // is silently ignore
                    // This is similar to, but not exactly the same as, the restart
                    // mechanism within a scan (the RST[0-7] markers).
                    // Note that extraneous 0xff bytes in e.g. SOS data are escaped as
                    // "\xff\x00", and so are detected a little further down below.
                    this.temp[0] = this.temp[1];
                    this.temp[1] = this.ReadByte();
                }

                byte marker = this.temp[1];
                if (marker == 0)
                {
                    // Treat "\xff\x00" as extraneous data.
                    continue;
                }

                while (marker == 0xff)
                {
                    // Section B.1.1.2 says, "Any marker may optionally be preceded by any
                    // number of fill bytes, which are bytes assigned code X'FF'".
                    marker = this.ReadByte();
                }

                // End Of Image.
                if (marker == JpegConstants.Markers.EOI)
                {
                    break;
                }

                if (marker >= JpegConstants.Markers.RST0 && marker <= JpegConstants.Markers.RST7)
                {
                    // Figures B.2 and B.16 of the specification suggest that restart markers should
                    // only occur between Entropy Coded Segments and not after the final ECS.
                    // However, some encoders may generate incorrect JPEGs with a final restart
                    // marker. That restart marker will be seen here instead of inside the ProcessSOS
                    // method, and is ignored as a harmless error. Restart markers have no extra data,
                    // so we check for this before we read the 16-bit length of the segment.
                    continue;
                }

                // Read the 16-bit length of the segment. The value includes the 2 bytes for the
                // length itself, so we subtract 2 to get the number of remaining bytes.
                this.ReadFull(this.temp, 0, 2);
                int remaining = (this.temp[0] << 8) + this.temp[1] - 2;
                if (remaining < 0)
                {
                    throw new ImageFormatException("Short segment length.");
                }

                switch (marker)
                {
                    case JpegConstants.Markers.SOF0:
                    case JpegConstants.Markers.SOF1:
                    case JpegConstants.Markers.SOF2:
                        this.isProgressive = marker == JpegConstants.Markers.SOF2;
                        this.ProcessStartOfFrameMarker(remaining);
                        if (configOnly && this.isJfif)
                        {
                            return;
                        }

                        break;
                    case JpegConstants.Markers.DHT:
                        if (configOnly)
                        {
                            this.Skip(remaining);
                        }
                        else
                        {
                            this.ProcessDefineHuffmanTablesMarker(remaining);
                        }

                        break;
                    case JpegConstants.Markers.DQT:
                        if (configOnly)
                        {
                            this.Skip(remaining);
                        }
                        else
                        {
                            this.ProcessDqt(remaining);
                        }

                        break;
                    case JpegConstants.Markers.SOS:
                        if (configOnly)
                        {
                            return;
                        }

                        this.ProcessStartOfScan(remaining);
                        break;
                    case JpegConstants.Markers.DRI:
                        if (configOnly)
                        {
                            this.Skip(remaining);
                        }
                        else
                        {
                            this.ProcessDefineRestartIntervalMarker(remaining);
                        }

                        break;
                    case JpegConstants.Markers.APP0:
                        this.ProcessApplicationHeader(remaining);
                        break;
                    case JpegConstants.Markers.APP1:
                        this.ProcessApp1Marker(remaining, image);
                        break;
                    case JpegConstants.Markers.APP14:
                        this.ProcessApp14Marker(remaining);
                        break;
                    default:
                        if ((JpegConstants.Markers.APP0 <= marker && marker <= JpegConstants.Markers.APP15)
                            || marker == JpegConstants.Markers.COM)
                        {
                            this.Skip(remaining);
                        }
                        else if (marker < JpegConstants.Markers.SOF0)
                        {
                            // See Table B.1 "Marker code assignments".
                            throw new ImageFormatException("Unknown marker");
                        }
                        else
                        {
                            throw new ImageFormatException("Unknown marker");
                        }

                        break;
                }
            }

            if (this.grayImage != null)
            {
                this.ConvertFromGrayScale(this.imageWidth, this.imageHeight, image);
            }
            else if (this.ycbcrImage != null)
            {
                if (this.componentCount == 4)
                {
                    this.ConvertFromCmyk(this.imageWidth, this.imageHeight, image);
                    return;
                }

                if (this.componentCount == 3)
                {
                    if (this.IsRGB())
                    {
                        this.ConvertFromRGB(this.imageWidth, this.imageHeight, image);
                        return;
                    }

                    this.ConvertFromYCbCr(this.imageWidth, this.imageHeight, image);
                    return;
                }

                throw new ImageFormatException("JpegDecoder only supports RGB, CMYK and Grayscale color spaces.");
            }
            else
            {
                throw new ImageFormatException("Missing SOS marker.");
            }
        }

        /// <summary>
        /// Processes a Define Huffman Table marker, and initializes a huffman
        /// struct from its contents. Specified in section B.2.4.2.
        /// </summary>
        /// <param name="remaining">The remaining bytes in the segment block.</param>
        private void ProcessDefineHuffmanTablesMarker(int remaining)
        {
            while (remaining > 0)
            {
                if (remaining < 17)
                {
                    throw new ImageFormatException("DHT has wrong length");
                }

                this.ReadFull(this.temp, 0, 17);

                int tc = this.temp[0] >> 4;
                if (tc > MaxTc)
                {
                    throw new ImageFormatException("Bad Tc value");
                }

                int th = this.temp[0] & 0x0f;
                if (th > MaxTh || (!this.isProgressive && (th > 1)))
                {
                    throw new ImageFormatException("Bad Th value");
                }

                this.ProcessDefineHuffmanTablesMarkerLoop(ref this.huffmanTrees[tc * ThRowSize + th], ref remaining);
            }
        }

        private void ProcessDefineHuffmanTablesMarkerLoop(ref Huffman huffman, ref int remaining)
        {
            // Read nCodes and huffman.Valuess (and derive h.Length).
            // nCodes[i] is the number of codes with code length i.
            // h.Length is the total number of codes.
            huffman.Length = 0;

            int[] ncodes = new int[MaxCodeLength];
            for (int i = 0; i < ncodes.Length; i++)
            {
                ncodes[i] = this.temp[i + 1];
                huffman.Length += ncodes[i];
            }

            if (huffman.Length == 0)
            {
                throw new ImageFormatException("Huffman table has zero length");
            }

            if (huffman.Length > MaxNCodes)
            {
                throw new ImageFormatException("Huffman table has excessive length");
            }

            remaining -= huffman.Length + 17;
            if (remaining < 0)
            {
                throw new ImageFormatException("DHT has wrong length");
            }

            this.ReadFull(huffman.Values, 0, huffman.Length);

            // Derive the look-up table.
            for (int i = 0; i < huffman.Lut.Length; i++)
            {
                huffman.Lut[i] = 0;
            }

            uint x = 0, code = 0;

            for (int i = 0; i < LutSize; i++)
            {
                code <<= 1;

                for (int j = 0; j < ncodes[i]; j++)
                {
                    // The codeLength is 1+i, so shift code by 8-(1+i) to
                    // calculate the high bits for every 8-bit sequence
                    // whose codeLength's high bits matches code.
                    // The high 8 bits of lutValue are the encoded value.
                    // The low 8 bits are 1 plus the codeLength.
                    byte base2 = (byte)(code << (7 - i));
                    ushort lutValue = (ushort)((huffman.Values[x] << 8) | (2 + i));

                    for (int k = 0; k < 1 << (7 - i); k++)
                    {
                        huffman.Lut[base2 | k] = lutValue;
                    }

                    code++;
                    x++;
                }
            }

            // Derive minCodes, maxCodes, and indices.
            int c = 0, index = 0;
            for (int i = 0; i < ncodes.Length; i++)
            {
                int nc = ncodes[i];
                if (nc == 0)
                {
                    huffman.MinCodes[i] = -1;
                    huffman.MaxCodes[i] = -1;
                    huffman.Indices[i] = -1;
                }
                else
                {
                    huffman.MinCodes[i] = c;
                    huffman.MaxCodes[i] = c + nc - 1;
                    huffman.Indices[i] = index;
                    c += nc;
                    index += nc;
                }

                c <<= 1;
            }
        }

        /// <summary>
        /// Returns the next Huffman-coded value from the bit-stream, decoded according to the given value.
        /// </summary>
        /// <param name="huffman">The huffman value</param>
        /// <returns>The <see cref="byte"/></returns>
        private byte DecodeHuffman(ref Huffman huffman)
        {
            // Copy stuff to the stack:

            if (huffman.Length == 0)
            {
                throw new ImageFormatException("Uninitialized Huffman table");
            }

            if (this.bits.UnreadBits < 8)
            {
                var errorCode = this.bits.EnsureNBits(8, this);

                if (errorCode == ErrorCodes.NoError)
                {
                    ushort v = huffman.Lut[(this.bits.Accumulator >> (this.bits.UnreadBits - LutSize)) & 0xff];

                    if (v != 0)
                    {
                        byte n = (byte)((v & 0xff) - 1);
                        this.bits.UnreadBits -= n;
                        this.bits.Mask >>= n;
                        return (byte)(v >> 8);
                    }
                }
                else
                {
                    this.UnreadByteStuffedByte();
                }
            }

            int code = 0;
            for (int i = 0; i < MaxCodeLength; i++)
            {
                if (this.bits.UnreadBits == 0)
                {
                    var errorCode = this.bits.EnsureNBits(1, this);
                    if (errorCode != ErrorCodes.NoError)
                    {
                        throw new MissingFF00Exception();
                    }
                }

                if ((this.bits.Accumulator & this.bits.Mask) != 0)
                {
                    code |= 1;
                }

                this.bits.UnreadBits--;
                this.bits.Mask >>= 1;

                if (code <= huffman.MaxCodes[i])
                {
                    return huffman.Values[huffman.Indices[i] + code - huffman.MinCodes[i]];
                }

                code <<= 1;
            }

            throw new ImageFormatException("Bad Huffman code");
        }

        /// <summary>
        /// Decodes a single bit
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        private bool DecodeBit()
        {
            if (this.bits.UnreadBits == 0)
            {
                var errorCode = this.bits.EnsureNBits(1, this);
                if (errorCode != ErrorCodes.NoError)
                {
                    throw new MissingFF00Exception();
                }
            }

            bool ret = (this.bits.Accumulator & this.bits.Mask) != 0;
            this.bits.UnreadBits--;
            this.bits.Mask >>= 1;
            return ret;
        }

        /// <summary>
        /// Decodes the given number of bits
        /// </summary>
        /// <param name="count">The number of bits to decode.</param>
        /// <returns>The <see cref="uint"/></returns>
        private uint DecodeBits(int count)
        {
            if (this.bits.UnreadBits < count)
            {
                var errorCode = this.bits.EnsureNBits(count, this);
                if (errorCode != ErrorCodes.NoError)
                {
                    throw new MissingFF00Exception();
                }
            }

            uint ret = this.bits.Accumulator >> (this.bits.UnreadBits - count);
            ret = (uint)(ret & ((1 << count) - 1));
            this.bits.UnreadBits -= count;
            this.bits.Mask >>= count;
            return ret;
        }

        /// <summary>
        /// Undoes the most recent ReadByteStuffedByte call,
        /// giving a byte of data back from bits to bytes. The Huffman look-up table
        /// requires at least 8 bits for look-up, which means that Huffman decoding can
        /// sometimes overshoot and read one or two too many bytes. Two-byte overshoot
        /// can happen when expecting to read a 0xff 0x00 byte-stuffed byte.
        /// </summary>
        private void UnreadByteStuffedByte()
        {
            this.bytes.I -= this.bytes.UnreadableBytes;
            this.bytes.UnreadableBytes = 0;
            if (this.bits.UnreadBits >= 8)
            {
                this.bits.Accumulator >>= 8;
                this.bits.UnreadBits -= 8;
                this.bits.Mask >>= 8;
            }
        }

        /// <summary>
        /// Returns the next byte, whether buffered or not buffered. It does not care about byte stuffing.
        /// </summary>
        /// <returns>The <see cref="byte"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal byte ReadByte()
        {
            return this.bytes.ReadByte(this.inputStream);
        }

        /// <summary>
        /// Reads exactly length bytes into data. It does not care about byte stuffing.
        /// </summary>
        /// <param name="data">The data to write to.</param>
        /// <param name="offset">The offset in the source buffer</param>
        /// <param name="length">The number of bytes to read</param>
        private void ReadFull(byte[] data, int offset, int length)
        {
            // Unread the overshot bytes, if any.
            if (this.bytes.UnreadableBytes != 0)
            {
                if (this.bits.UnreadBits >= 8)
                {
                    this.UnreadByteStuffedByte();
                }

                this.bytes.UnreadableBytes = 0;
            }

            while (length > 0)
            {
                if (this.bytes.J - this.bytes.I >= length)
                {
                    Array.Copy(this.bytes.Buffer, this.bytes.I, data, offset, length);
                    this.bytes.I += length;
                    length -= length;
                }
                else
                {
                    Array.Copy(this.bytes.Buffer, this.bytes.I, data, offset, this.bytes.J - this.bytes.I);
                    offset += this.bytes.J - this.bytes.I;
                    length -= this.bytes.J - this.bytes.I;
                    this.bytes.I += this.bytes.J - this.bytes.I;

                    this.bytes.Fill(this.inputStream);
                }
            }
        }

        /// <summary>
        /// Skips the next n bytes.
        /// </summary>
        /// <param name="count">The number of bytes to ignore.</param>
        private void Skip(int count)
        {
            // Unread the overshot bytes, if any.
            if (this.bytes.UnreadableBytes != 0)
            {
                if (this.bits.UnreadBits >= 8)
                {
                    this.UnreadByteStuffedByte();
                }

                this.bytes.UnreadableBytes = 0;
            }

            while (true)
            {
                int m = this.bytes.J - this.bytes.I;
                if (m > count)
                {
                    m = count;
                }

                this.bytes.I += m;
                count -= m;
                if (count == 0)
                {
                    break;
                }

                this.bytes.Fill(this.inputStream);
            }
        }

        /// <summary>
        /// Processes the Start of Frame marker.  Specified in section B.2.2.
        /// </summary>
        /// <param name="remaining">The remaining bytes in the segment block.</param>
        private void ProcessStartOfFrameMarker(int remaining)
        {
            if (this.componentCount != 0)
            {
                throw new ImageFormatException("Multiple SOF markers");
            }

            switch (remaining)
            {
                case 6 + (3 * 1): // Grayscale image.
                    this.componentCount = 1;
                    break;
                case 6 + (3 * 3): // YCbCr or RGB image.
                    this.componentCount = 3;
                    break;
                case 6 + (3 * 4): // YCbCrK or CMYK image.
                    this.componentCount = 4;
                    break;
                default:
                    throw new ImageFormatException("Incorrect number of components");
            }

            this.ReadFull(this.temp, 0, remaining);

            // We only support 8-bit precision.
            if (this.temp[0] != 8)
            {
                throw new ImageFormatException("Only 8-Bit precision supported.");
            }

            this.imageHeight = (this.temp[1] << 8) + this.temp[2];
            this.imageWidth = (this.temp[3] << 8) + this.temp[4];
            if (this.temp[5] != this.componentCount)
            {
                throw new ImageFormatException("SOF has wrong length");
            }

            for (int i = 0; i < this.componentCount; i++)
            {
                this.componentArray[i].Identifier = this.temp[6 + (3 * i)];

                // Section B.2.2 states that "the value of C_i shall be different from
                // the values of C_1 through C_(i-1)".
                for (int j = 0; j < i; j++)
                {
                    if (this.componentArray[i].Identifier == this.componentArray[j].Identifier)
                    {
                        throw new ImageFormatException("Repeated component identifier");
                    }
                }

                this.componentArray[i].Selector = this.temp[8 + (3 * i)];
                if (this.componentArray[i].Selector > MaxTq)
                {
                    throw new ImageFormatException("Bad Tq value");
                }

                byte hv = this.temp[7 + (3 * i)];
                int h = hv >> 4;
                int v = hv & 0x0f;
                if (h < 1 || h > 4 || v < 1 || v > 4)
                {
                    throw new ImageFormatException("Unsupported Luma/chroma subsampling ratio");
                }

                if (h == 3 || v == 3)
                {
                    throw new ImageFormatException("Lnsupported subsampling ratio");
                }

                switch (this.componentCount)
                {
                    case 1:

                        // If a JPEG image has only one component, section A.2 says "this data
                        // is non-interleaved by definition" and section A.2.2 says "[in this
                        // case...] the order of data units within a scan shall be left-to-right
                        // and top-to-bottom... regardless of the values of H_1 and V_1". Section
                        // 4.8.2 also says "[for non-interleaved data], the MCU is defined to be
                        // one data unit". Similarly, section A.1.1 explains that it is the ratio
                        // of H_i to max_j(H_j) that matters, and similarly for V. For grayscale
                        // images, H_1 is the maximum H_j for all components j, so that ratio is
                        // always 1. The component's (h, v) is effectively always (1, 1): even if
                        // the nominal (h, v) is (2, 1), a 20x5 image is encoded in three 8x8
                        // MCUs, not two 16x8 MCUs.
                        h = 1;
                        v = 1;
                        break;

                    case 3:

                        // For YCbCr images, we only support 4:4:4, 4:4:0, 4:2:2, 4:2:0,
                        // 4:1:1 or 4:1:0 chroma subsampling ratios. This implies that the
                        // (h, v) values for the Y component are either (1, 1), (1, 2),
                        // (2, 1), (2, 2), (4, 1) or (4, 2), and the Y component's values
                        // must be a multiple of the Cb and Cr component's values. We also
                        // assume that the two chroma components have the same subsampling
                        // ratio.
                        switch (i)
                        {
                            case 0:
                                {
                                    // Y.
                                    // We have already verified, above, that h and v are both
                                    // either 1, 2 or 4, so invalid (h, v) combinations are those
                                    // with v == 4.
                                    if (v == 4)
                                    {
                                        throw new ImageFormatException("Unsupported subsampling ratio");
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    // Cb.
                                    if (this.componentArray[0].HorizontalFactor % h != 0
                                        || this.componentArray[0].VerticalFactor % v != 0)
                                    {
                                        throw new ImageFormatException("Unsupported subsampling ratio");
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    // Cr.
                                    if (this.componentArray[1].HorizontalFactor != h
                                        || this.componentArray[1].VerticalFactor != v)
                                    {
                                        throw new ImageFormatException("Unsupported subsampling ratio");
                                    }

                                    break;
                                }
                        }

                        break;

                    case 4:

                        // For 4-component images (either CMYK or YCbCrK), we only support two
                        // hv vectors: [0x11 0x11 0x11 0x11] and [0x22 0x11 0x11 0x22].
                        // Theoretically, 4-component JPEG images could mix and match hv values
                        // but in practice, those two combinations are the only ones in use,
                        // and it simplifies the applyBlack code below if we can assume that:
                        // - for CMYK, the C and K channels have full samples, and if the M
                        // and Y channels subsample, they subsample both horizontally and
                        // vertically.
                        // - for YCbCrK, the Y and K channels have full samples.
                        switch (i)
                        {
                            case 0:
                                if (hv != 0x11 && hv != 0x22)
                                {
                                    throw new ImageFormatException("Unsupported subsampling ratio");
                                }

                                break;
                            case 1:
                            case 2:
                                if (hv != 0x11)
                                {
                                    throw new ImageFormatException("Unsupported subsampling ratio");
                                }

                                break;
                            case 3:
                                if (this.componentArray[0].HorizontalFactor != h
                                    || this.componentArray[0].VerticalFactor != v)
                                {
                                    throw new ImageFormatException("Unsupported subsampling ratio");
                                }

                                break;
                        }

                        break;
                }

                this.componentArray[i].HorizontalFactor = h;
                this.componentArray[i].VerticalFactor = v;
            }
        }

        /// <summary>
        /// Processes the Define Quantization Marker and tables. Specified in section B.2.4.1.
        /// </summary>
        /// <param name="remaining">The remaining bytes in the segment block.</param>
        /// <exception cref="ImageFormatException">
        /// Thrown if the tables do not match the header
        /// </exception>
        private void ProcessDqt(int remaining)
        {
            while (remaining > 0)
            {
                bool done = false;

                remaining--;
                byte x = this.ReadByte();
                byte tq = (byte)(x & 0x0f);
                if (tq > MaxTq)
                {
                    throw new ImageFormatException("Bad Tq value");
                }

                switch (x >> 4)
                {
                    case 0:
                        if (remaining < BlockF.BlockSize)
                        {
                            done = true;
                            break;
                        }

                        remaining -= BlockF.BlockSize;
                        this.ReadFull(this.temp, 0, BlockF.BlockSize);

                        for (int i = 0; i < BlockF.BlockSize; i++)
                        {
                            this.quantizationTables[tq][i] = this.temp[i];
                        }

                        break;
                    case 1:
                        if (remaining < 2 * BlockF.BlockSize)
                        {
                            done = true;
                            break;
                        }

                        remaining -= 2 * BlockF.BlockSize;
                        this.ReadFull(this.temp, 0, 2 * BlockF.BlockSize);

                        for (int i = 0; i < BlockF.BlockSize; i++)
                        {
                            this.quantizationTables[tq][i] = (this.temp[2 * i] << 8) | this.temp[(2 * i) + 1];
                        }

                        break;
                    default:
                        throw new ImageFormatException("Bad Pq value");
                }

                if (done)
                {
                    break;
                }
            }

            if (remaining != 0)
            {
                throw new ImageFormatException("DQT has wrong length");
            }
        }

        /// <summary>
        /// Processes the DRI (Define Restart Interval Marker) Which specifies the interval between RSTn markers, in macroblocks
        /// </summary>
        /// <param name="remaining">The remaining bytes in the segment block.</param>
        private void ProcessDefineRestartIntervalMarker(int remaining)
        {
            if (remaining != 2)
            {
                throw new ImageFormatException("DRI has wrong length");
            }

            this.ReadFull(this.temp, 0, 2);
            this.restartInterval = ((int)this.temp[0] << 8) + (int)this.temp[1];
        }

        /// <summary>
        /// Processes the application header containing the JFIF identifier plus extra data.
        /// </summary>
        /// <param name="remaining">The remaining bytes in the segment block.</param>
        private void ProcessApplicationHeader(int remaining)
        {
            if (remaining < 5)
            {
                this.Skip(remaining);
                return;
            }

            this.ReadFull(this.temp, 0, 13);
            remaining -= 13;

            // TODO: We should be using constants for this.
            this.isJfif = this.temp[0] == 'J' && this.temp[1] == 'F' && this.temp[2] == 'I' && this.temp[3] == 'F'
                          && this.temp[4] == '\x00';

            if (this.isJfif)
            {
                this.horizontalResolution = (short)(this.temp[9] + (this.temp[10] << 8));
                this.verticalResolution = (short)(this.temp[11] + (this.temp[12] << 8));
            }

            if (remaining > 0)
            {
                this.Skip(remaining);
            }
        }

        /// <summary>
        /// Processes the App1 marker retrieving any stored metadata
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="remaining">The remaining bytes in the segment block.</param>
        /// <param name="image">The image.</param>
        private void ProcessApp1Marker<TColor, TPacked>(int remaining, Image<TColor, TPacked> image)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            if (remaining < 6)
            {
                this.Skip(remaining);
                return;
            }

            byte[] profile = new byte[remaining];
            this.ReadFull(profile, 0, remaining);

            if (profile[0] == 'E' && profile[1] == 'x' && profile[2] == 'i' && profile[3] == 'f' && profile[4] == '\0'
                && profile[5] == '\0')
            {
                image.ExifProfile = new ExifProfile(profile);
            }
        }

        /// <summary>
        /// Processes the "Adobe" APP14 segment stores image encoding information for DCT filters.
        /// This segment may be copied or deleted as a block using the Extra "Adobe" tag, but note that it is not
        /// deleted by default when deleting all metadata because it may affect the appearance of the image.
        /// </summary>
        /// <param name="remaining">The remaining number of bytes in the stream.</param>
        private void ProcessApp14Marker(int remaining)
        {
            if (remaining < 12)
            {
                this.Skip(remaining);
                return;
            }

            this.ReadFull(this.temp, 0, 12);
            remaining -= 12;

            if (this.temp[0] == 'A' && this.temp[1] == 'd' && this.temp[2] == 'o' && this.temp[3] == 'b'
                && this.temp[4] == 'e')
            {
                this.adobeTransformValid = true;
                this.adobeTransform = this.temp[11];
            }

            if (remaining > 0)
            {
                this.Skip(remaining);
            }
        }

        /// <summary>
        /// Converts the image from the original CMYK image pixels.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <param name="image">The image.</param>
        private void ConvertFromCmyk<TColor, TPacked>(int width, int height, Image<TColor, TPacked> image)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            if (!this.adobeTransformValid)
            {
                throw new ImageFormatException(
                          "Unknown color model: 4-component JPEG doesn't have Adobe APP14 metadata");
            }

            // If the 4-component JPEG image isn't explicitly marked as "Unknown (RGB
            // or CMYK)" as per http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/JPEG.html#Adobe
            if (this.adobeTransform != JpegConstants.Adobe.ColorTransformUnknown)
            {
                int scale = this.componentArray[0].HorizontalFactor / this.componentArray[1].HorizontalFactor;

                image.InitPixels(width, height);

                using (PixelAccessor<TColor, TPacked> pixels = image.Lock())
                {
                    // Convert the YCbCr part of the YCbCrK to RGB, invert the RGB to get
                    // CMY, and patch in the original K. The RGB to CMY inversion cancels
                    // out the 'Adobe inversion' described in the applyBlack doc comment
                    // above, so in practice, only the fourth channel (black) is inverted.
                    Parallel.For(
                        0,
                        height,
                        y =>
                            {
                                int yo = this.ycbcrImage.GetRowYOffset(y);
                                int co = this.ycbcrImage.GetRowCOffset(y);

                                for (int x = 0; x < width; x++)
                                {
                                    byte yy = this.ycbcrImage.YChannel[yo + x];
                                    byte cb = this.ycbcrImage.CbChannel[co + (x / scale)];
                                    byte cr = this.ycbcrImage.CrChannel[co + (x / scale)];

                                    TColor packed = default(TColor);
                                    this.PackCmyk<TColor, TPacked>(ref packed, yy, cb, cr, x, y);
                                    pixels[x, y] = packed;
                                }
                            });
                }

                this.AssignResolution(image);
            }
        }

        /// <summary>
        /// Converts the image from the original grayscale image pixels.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>long, float.</example></typeparam>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <param name="image">The image.</param>
        private void ConvertFromGrayScale<TColor, TPacked>(int width, int height, Image<TColor, TPacked> image)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            image.InitPixels(width, height);

            using (PixelAccessor<TColor, TPacked> pixels = image.Lock())
            {
                Parallel.For(
                    0,
                    height,
                    Bootstrapper.Instance.ParallelOptions,
                    y =>
                        {
                            int yoff = this.grayImage.GetRowOffset(y);
                            for (int x = 0; x < width; x++)
                            {
                                byte rgb = this.grayImage.Pixels[yoff + x];

                                TColor packed = default(TColor);
                                packed.PackFromBytes(rgb, rgb, rgb, 255);
                                pixels[x, y] = packed;
                            }
                        });
            }

            this.AssignResolution(image);
        }

        /// <summary>
        /// Converts the image from the original YCbCr image pixels.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <param name="image">The image.</param>
        private void ConvertFromYCbCr<TColor, TPacked>(int width, int height, Image<TColor, TPacked> image)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            int scale = this.componentArray[0].HorizontalFactor / this.componentArray[1].HorizontalFactor;
            image.InitPixels(width, height);

            using (PixelAccessor<TColor, TPacked> pixels = image.Lock())
            {
                Parallel.For(
                    0,
                    height,
                    Bootstrapper.Instance.ParallelOptions,
                    y =>
                        {
                            int yo = this.ycbcrImage.GetRowYOffset(y);
                            int co = this.ycbcrImage.GetRowCOffset(y);

                            for (int x = 0; x < width; x++)
                            {
                                byte yy = this.ycbcrImage.YChannel[yo + x];
                                byte cb = this.ycbcrImage.CbChannel[co + (x / scale)];
                                byte cr = this.ycbcrImage.CrChannel[co + (x / scale)];

                                TColor packed = default(TColor);
                                PackYcbCr<TColor, TPacked>(ref packed, yy, cb, cr);
                                pixels[x, y] = packed;
                            }
                        });
            }

            this.AssignResolution(image);
        }

        /// <summary>
        /// Converts the image from the original RBG image pixels.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="width">The image width.</param>
        /// <param name="height">The height.</param>
        /// <param name="image">The image.</param>
        private void ConvertFromRGB<TColor, TPacked>(int width, int height, Image<TColor, TPacked> image)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            int scale = this.componentArray[0].HorizontalFactor / this.componentArray[1].HorizontalFactor;
            image.InitPixels(width, height);

            using (PixelAccessor<TColor, TPacked> pixels = image.Lock())
            {
                Parallel.For(
                    0,
                    height,
                    Bootstrapper.Instance.ParallelOptions,
                    y =>
                        {
                            int yo = this.ycbcrImage.GetRowYOffset(y);
                            int co = this.ycbcrImage.GetRowCOffset(y);

                            for (int x = 0; x < width; x++)
                            {
                                byte red = this.ycbcrImage.YChannel[yo + x];
                                byte green = this.ycbcrImage.CbChannel[co + (x / scale)];
                                byte blue = this.ycbcrImage.CrChannel[co + (x / scale)];

                                TColor packed = default(TColor);
                                packed.PackFromBytes(red, green, blue, 255);
                                pixels[x, y] = packed;
                            }
                        });
            }

            this.AssignResolution(image);
        }

        /// <summary>
        /// Assigns the horizontal and vertical resolution to the image if it has a JFIF header.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="image">The image to assign the resolution to.</param>
        private void AssignResolution<TColor, TPacked>(Image<TColor, TPacked> image)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            if (this.isJfif && this.horizontalResolution > 0 && this.verticalResolution > 0)
            {
                image.HorizontalResolution = this.horizontalResolution;
                image.VerticalResolution = this.verticalResolution;
            }
        }

        struct StackallocUnzigData
        {
            internal fixed int Data[64];
        }

        /// <summary>
        /// Processes the SOS (Start of scan marker).
        /// </summary>
        /// <remarks>
        /// TODO: This also needs some significant refactoring to follow a more OO format.
        /// </remarks>
        /// <param name="remaining">The remaining bytes in the segment block.</param>
        /// <exception cref="ImageFormatException">
        /// Missing SOF Marker
        /// SOS has wrong length
        /// </exception>
        private void ProcessStartOfScan(int remaining)
        {
            if (this.componentCount == 0)
            {
                throw new ImageFormatException("Missing SOF marker");
            }

            if (remaining < 6 || 4 + (2 * this.componentCount) < remaining || remaining % 2 != 0)
            {
                throw new ImageFormatException("SOS has wrong length");
            }

            this.ReadFull(this.temp, 0, remaining);
            byte scanComponentCount = this.temp[0];

            int scanComponentCountX2 = 2 * scanComponentCount;
            if (remaining != 4 + scanComponentCountX2)
            {
                throw new ImageFormatException("SOS length inconsistent with number of components");
            }

            Scan[] scan = new Scan[MaxComponents];
            int totalHv = 0;

            for (int i = 0; i < scanComponentCount; i++)
            {
                this.ProcessScanImpl(i, ref scan[i], scan, ref totalHv);
            }

            // Section B.2.3 states that if there is more than one component then the
            // total H*V values in a scan must be <= 10.
            if (this.componentCount > 1 && totalHv > 10)
            {
                throw new ImageFormatException("Total sampling factors too large.");
            }

            // zigStart and zigEnd are the spectral selection bounds.
            // ah and al are the successive approximation high and low values.
            // The spec calls these values Ss, Se, Ah and Al.
            // For progressive JPEGs, these are the two more-or-less independent
            // aspects of progression. Spectral selection progression is when not
            // all of a block's 64 DCT coefficients are transmitted in one pass.
            // For example, three passes could transmit coefficient 0 (the DC
            // component), coefficients 1-5, and coefficients 6-63, in zig-zag
            // order. Successive approximation is when not all of the bits of a
            // band of coefficients are transmitted in one pass. For example,
            // three passes could transmit the 6 most significant bits, followed
            // by the second-least significant bit, followed by the least
            // significant bit.
            // For baseline JPEGs, these parameters are hard-coded to 0/63/0/0.
            int zigStart = 0;
            int zigEnd = BlockF.BlockSize - 1;
            int ah = 0;
            int al = 0;

            if (this.isProgressive)
            {
                zigStart = this.temp[1 + scanComponentCountX2];
                zigEnd = this.temp[2 + scanComponentCountX2];
                ah = this.temp[3 + scanComponentCountX2] >> 4;
                al = this.temp[3 + scanComponentCountX2] & 0x0f;

                if ((zigStart == 0 && zigEnd != 0) || zigStart > zigEnd || BlockF.BlockSize <= zigEnd)
                {
                    throw new ImageFormatException("Bad spectral selection bounds");
                }

                if (zigStart != 0 && scanComponentCount != 1)
                {
                    throw new ImageFormatException("Progressive AC coefficients for more than one component");
                }

                if (ah != 0 && ah != al + 1)
                {
                    throw new ImageFormatException("Bad successive approximation values");
                }
            }

            // mxx and myy are the number of MCUs (Minimum Coded Units) in the image.
            int h0 = this.componentArray[0].HorizontalFactor;
            int v0 = this.componentArray[0].VerticalFactor;
            int mxx = (this.imageWidth + (8 * h0) - 1) / (8 * h0);
            int myy = (this.imageHeight + (8 * v0) - 1) / (8 * v0);

            if (this.grayImage == null && this.ycbcrImage == null)
            {
                this.MakeImage(mxx, myy);
            }

            if (this.isProgressive)
            {
                for (int i = 0; i < scanComponentCount; i++)
                {
                    int compIndex = scan[i].Index;
                    if (this.progCoeffs[compIndex] == null)
                    {
                        var size = mxx * myy * this.componentArray[compIndex].HorizontalFactor
                                   * this.componentArray[compIndex].VerticalFactor;

                        this.progCoeffs[compIndex] = new Block8x8F[size];
                    }
                }
            }

            this.bits = new Bits();

            int mcu = 0;
            byte expectedRst = JpegConstants.Markers.RST0;

            // b is the decoded coefficients block, in natural (not zig-zag) order.
            //Block b;
            int[] dc = new int[MaxComponents];

            // bx and by are the location of the current block, in units of 8x8
            // blocks: the third block in the first row has (bx, by) = (2, 0).
            int bx, by, blockCount = 0;

            Block8x8F b = new Block8x8F();
            Block8x8F temp1 = new Block8x8F();
            Block8x8F temp2 = new Block8x8F();

            // Tricky way to copy contents of the Unzig static variable to the stack:
            StackallocUnzigData unzigOnStack = new StackallocUnzigData();
            int* unzigPtr = unzigOnStack.Data;
            Marshal.Copy(Unzig, 0, (IntPtr)unzigPtr, 64);

            for (int my = 0; my < myy; my++)
            {
                for (int mx = 0; mx < mxx; mx++)
                {
                    for (int i = 0; i < scanComponentCount; i++)
                    {
                        int compIndex = scan[i].Index;
                        int hi = this.componentArray[compIndex].HorizontalFactor;
                        int vi = this.componentArray[compIndex].VerticalFactor;

                        for (int j = 0; j < hi * vi; j++)
                        {
                            // The blocks are traversed one MCU at a time. For 4:2:0 chroma
                            // subsampling, there are four Y 8x8 blocks in every 16x16 MCU.
                            // For a baseline 32x16 pixel image, the Y blocks visiting order is:
                            // 0 1 4 5
                            // 2 3 6 7
                            // For progressive images, the interleaved scans (those with component count > 1)
                            // are traversed as above, but non-interleaved scans are traversed left
                            // to right, top to bottom:
                            // 0 1 2 3
                            // 4 5 6 7
                            // Only DC scans (zigStart == 0) can be interleave AC scans must have
                            // only one component.
                            // To further complicate matters, for non-interleaved scans, there is no
                            // data for any blocks that are inside the image at the MCU level but
                            // outside the image at the pixel level. For example, a 24x16 pixel 4:2:0
                            // progressive image consists of two 16x16 MCUs. The interleaved scans
                            // will process 8 Y blocks:
                            // 0 1 4 5
                            // 2 3 6 7
                            // The non-interleaved scans will process only 6 Y blocks:
                            // 0 1 2
                            // 3 4 5
                            if (scanComponentCount != 1)
                            {
                                bx = (hi * mx) + (j % hi);
                                by = (vi * my) + (j / hi);
                            }
                            else
                            {
                                int q = mxx * hi;
                                bx = blockCount % q;
                                by = blockCount / q;
                                blockCount++;
                                if (bx * 8 >= this.imageWidth || by * 8 >= this.imageHeight)
                                {
                                    continue;
                                }
                            }

                            var qtIndex = this.componentArray[compIndex].Selector;

                            // TODO: Find a way to clean up this mess

                            fixed (Block8x8F* qtp = &this.quantizationTables[qtIndex])
                            {
                                if (this.isProgressive)
                                // Load the previous partially decoded coefficients, if applicable.
                                {
                                    this.blockIndex = ((@by * mxx) * hi) + bx;

                                    fixed (Block8x8F* bp = &this.progCoeffs[compIndex][this.blockIndex])
                                    {
                                        this.ProcessBlockImpl(
                                            ah,
                                            bp,
                                            &temp1,
                                            &temp2,
                                            unzigPtr,
                                            scan,
                                            i,
                                            zigStart,
                                            zigEnd,
                                            al,
                                            dc,
                                            compIndex,
                                            @by,
                                            mxx,
                                            hi,
                                            bx,
                                            qtp);
                                    }
                                }
                                else
                                {
                                    b.Clear();
                                    this.ProcessBlockImpl(
                                        ah,
                                        &b,
                                        &temp1,
                                        &temp2,
                                        unzigPtr,
                                        scan,
                                        i,
                                        zigStart,
                                        zigEnd,
                                        al,
                                        dc,
                                        compIndex,
                                        @by,
                                        mxx,
                                        hi,
                                        bx,
                                        qtp);
                                }
                            }
                        }

                        // for j
                    }

                    // for i
                    mcu++;

                    if (this.restartInterval > 0 && mcu % this.restartInterval == 0 && mcu < mxx * myy)
                    {
                        // A more sophisticated decoder could use RST[0-7] markers to resynchronize from corrupt input,
                        // but this one assumes well-formed input, and hence the restart marker follows immediately.
                        this.ReadFull(this.temp, 0, 2);
                        if (this.temp[0] != 0xff || this.temp[1] != expectedRst)
                        {
                            throw new ImageFormatException("Bad RST marker");
                        }

                        expectedRst++;
                        if (expectedRst == JpegConstants.Markers.RST7 + 1)
                        {
                            expectedRst = JpegConstants.Markers.RST0;
                        }

                        // Reset the Huffman decoder.
                        this.bits = new Bits();

                        // Reset the DC components, as per section F.2.1.3.1.
                        dc = new int[MaxComponents];

                        // Reset the progressive decoder state, as per section G.1.2.2.
                        this.eobRun = 0;
                    }
                }

                // for mx
            }

            // for my
        }

        private void ProcessBlockImpl(
            int ah,
            Block8x8F* b,
            Block8x8F* temp1,
            Block8x8F* temp2,
            int* unzigPtr,
            Scan[] scan,
            int i,
            int zigStart,
            int zigEnd,
            int al,
            int[] dc,
            int compIndex,
            int @by,
            int mxx,
            int hi,
            int bx,
            Block8x8F* qt)
        {
            var huffmannIdx = AcTable * ThRowSize + scan[i].AcTableSelector;
            if (ah != 0)
            {
                this.Refine(b, ref this.huffmanTrees[huffmannIdx], unzigPtr, zigStart, zigEnd, 1 << al);
            }
            else
            {
                int zig = zigStart;
                if (zig == 0)
                {
                    zig++;

                    // Decode the DC coefficient, as specified in section F.2.2.1.
                    byte value = this.DecodeHuffman(
                        ref this.huffmanTrees[DcTable * ThRowSize + scan[i].DcTableSelector]);
                    if (value > 16)
                    {
                        throw new ImageFormatException("Excessive DC component");
                    }

                    int deltaDC = this.bits.ReceiveExtend(value, this);
                    dc[compIndex] += deltaDC;

                    // b[0] = dc[compIndex] << al;
                    Block8x8F.SetScalarAt(b, 0, dc[compIndex] << al);
                }

                if (zig <= zigEnd && this.eobRun > 0)
                {
                    this.eobRun--;
                }
                else
                {
                    // Decode the AC coefficients, as specified in section F.2.2.2.
                    // Huffman huffv = ;
                    for (; zig <= zigEnd; zig++)
                    {
                        byte value = this.DecodeHuffman(ref this.huffmanTrees[huffmannIdx]);
                        byte val0 = (byte)(value >> 4);
                        byte val1 = (byte)(value & 0x0f);
                        if (val1 != 0)
                        {
                            zig += val0;
                            if (zig > zigEnd)
                            {
                                break;
                            }

                            int ac = this.bits.ReceiveExtend(val1, this);

                            // b[Unzig[zig]] = ac << al;
                            Block8x8F.SetScalarAt(b, unzigPtr[zig], ac << al);
                        }
                        else
                        {
                            if (val0 != 0x0f)
                            {
                                this.eobRun = (ushort)(1 << val0);
                                if (val0 != 0)
                                {
                                    this.eobRun |= (ushort)this.DecodeBits(val0);
                                }

                                this.eobRun--;
                                break;
                            }

                            zig += 0x0f;
                        }
                    }
                }
            }

            if (this.isProgressive)
            {
                if (zigEnd != BlockF.BlockSize - 1 || al != 0)
                {
                    // We haven't completely decoded this 8x8 block. Save the coefficients.

                    // TODO!!!
                    //throw new NotImplementedException();
                    //this.progCoeffs[compIndex][((@by * mxx) * hi) + bx] = b.Clone();
                    this.progCoeffs[compIndex][((@by * mxx) * hi) + bx] = *b;

                    // At this point, we could execute the rest of the loop body to dequantize and
                    // perform the inverse DCT, to save early stages of a progressive image to the
                    // *image.YCbCr buffers (the whole point of progressive encoding), but in Go,
                    // the jpeg.Decode function does not return until the entire image is decoded,
                    // so we "continue" here to avoid wasted computation.
                    return;
                }
            }

            // Dequantize, perform the inverse DCT and store the block to the image.
            Block8x8F.UnZig(b, qt, unzigPtr);

            b->TransformIDCTInto(ref *temp1, ref *temp2);

            byte[] dst;
            int offset;
            int stride;

            if (this.componentCount == 1)
            {
                dst = this.grayImage.Pixels;
                stride = this.grayImage.Stride;
                offset = this.grayImage.Offset + (8 * ((@by * this.grayImage.Stride) + bx));
            }
            else
            {
                switch (compIndex)
                {
                    case 0:
                        dst = this.ycbcrImage.YChannel;
                        stride = this.ycbcrImage.YStride;
                        offset = this.ycbcrImage.YOffset + (8 * ((@by * this.ycbcrImage.YStride) + bx));
                        break;

                    case 1:
                        dst = this.ycbcrImage.CbChannel;
                        stride = this.ycbcrImage.CStride;
                        offset = this.ycbcrImage.COffset + (8 * ((@by * this.ycbcrImage.CStride) + bx));
                        break;

                    case 2:
                        dst = this.ycbcrImage.CrChannel;
                        stride = this.ycbcrImage.CStride;
                        offset = this.ycbcrImage.COffset + (8 * ((@by * this.ycbcrImage.CStride) + bx));
                        break;

                    case 3:

                        dst = this.blackPixels;
                        stride = this.blackStride;
                        offset = 8 * ((@by * this.blackStride) + bx);
                        break;

                    default:
                        throw new ImageFormatException("Too many components");
                }
            }

            // Level shift by +128, clip to [0, 255], and write to dst.

            temp1->CopyColorsTo(new MutableSpan<byte>(dst, offset), stride, temp2);
        }

        private void ProcessScanImpl(int i, ref Scan currentScan, Scan[] scan, ref int totalHv)
        {
            // Component selector.
            int cs = this.temp[1 + (2 * i)];
            int compIndex = -1;
            for (int j = 0; j < this.componentCount; j++)
            {
                //Component compv = ;
                if (cs == this.componentArray[j].Identifier)
                {
                    compIndex = j;
                }
            }

            if (compIndex < 0)
            {
                throw new ImageFormatException("Unknown component selector");
            }

            currentScan.Index = (byte)compIndex;

            this.ProcessComponentImpl(i, ref currentScan, scan, ref totalHv, ref this.componentArray[compIndex]);
        }

        private void ProcessComponentImpl(
            int i,
            ref Scan currentScan,
            Scan[] scan,
            ref int totalHv,
            ref Component currentComponent)
        {
            // Section B.2.3 states that "the value of Cs_j shall be different from
            // the values of Cs_1 through Cs_(j-1)". Since we have previously
            // verified that a frame's component identifiers (C_i values in section
            // B.2.2) are unique, it suffices to check that the implicit indexes
            // into comp are unique.
            for (int j = 0; j < i; j++)
            {
                if (currentScan.Index == scan[j].Index)
                {
                    throw new ImageFormatException("Repeated component selector");
                }
            }

            totalHv += currentComponent.HorizontalFactor * currentComponent.VerticalFactor;

            currentScan.DcTableSelector = (byte)(this.temp[2 + (2 * i)] >> 4);
            if (currentScan.DcTableSelector > MaxTh)
            {
                throw new ImageFormatException("Bad DC table selector value");
            }

            currentScan.AcTableSelector = (byte)(this.temp[2 + (2 * i)] & 0x0f);
            if (currentScan.AcTableSelector > MaxTh)
            {
                throw new ImageFormatException("Bad AC table selector  value");
            }
        }

        /// <summary>
        /// Decodes a successive approximation refinement block, as specified in section G.1.2.
        /// </summary>
        /// <param name="b">The block of coefficients</param>
        /// <param name="h">The Huffman tree</param>
        /// <param name="unzigPtr"></param>
        /// <param name="zigStart">The zig-zag start index</param>
        /// <param name="zigEnd">The zig-zag end index</param>
        /// <param name="delta">The low transform offset</param>
        private void Refine(Block8x8F* b, ref Huffman h, int* unzigPtr, int zigStart, int zigEnd, int delta)
        {
            // Refining a DC component is trivial.
            if (zigStart == 0)
            {
                if (zigEnd != 0)
                {
                    throw new ImageFormatException("Invalid state for zig DC component");
                }

                bool bit = this.DecodeBit();
                if (bit)
                {
                    int stuff = (int)Block8x8F.GetScalarAt(b, 0);

                    // int stuff = (int)b[0];
                    stuff |= delta;
                    // b[0] = stuff;
                    Block8x8F.SetScalarAt(b, 0, stuff);
                }

                return;
            }

            // Refining AC components is more complicated; see sections G.1.2.2 and G.1.2.3.
            int zig = zigStart;
            if (this.eobRun == 0)
            {
                for (; zig <= zigEnd; zig++)
                {
                    bool done = false;
                    int z = 0;
                    byte val = this.DecodeHuffman(ref h);
                    int val0 = val >> 4;
                    int val1 = val & 0x0f;

                    switch (val1)
                    {
                        case 0:
                            if (val0 != 0x0f)
                            {
                                this.eobRun = (ushort)(1 << val0);
                                if (val0 != 0)
                                {
                                    this.eobRun |= (ushort)this.DecodeBits(val0);
                                }

                                done = true;
                            }

                            break;
                        case 1:
                            z = delta;
                            bool bit = this.DecodeBit();
                            if (!bit)
                            {
                                z = -z;
                            }

                            break;
                        default:
                            throw new ImageFormatException("Unexpected Huffman code");
                    }

                    if (done)
                    {
                        break;
                    }

                    int blah = zig;

                    zig = this.RefineNonZeroes(b, zig, zigEnd, val0, delta);
                    if (zig > zigEnd)
                    {
                        throw new ImageFormatException($"Too many coefficients {zig} > {zigEnd}");
                    }

                    if (z != 0)
                    {
                        //b[Unzig[zig]] = z;
                        Block8x8F.SetScalarAt(b, unzigPtr[zig], z);
                    }
                }
            }

            if (this.eobRun > 0)
            {
                this.eobRun--;
                this.RefineNonZeroes(b, zig, zigEnd, -1, delta);
            }
        }

        /// <summary>
        /// Refines non-zero entries of b in zig-zag order.
        /// If <paramref name="nz"/> >= 0, the first <paramref name="nz"/> zero entries are skipped over.
        /// </summary>
        /// <param name="b">The block of coefficients</param>
        /// <param name="zig">The zig-zag start index</param>
        /// <param name="zigEnd">The zig-zag end index</param>
        /// <param name="nz">The non-zero entry</param>
        /// <param name="delta">The low transform offset</param>
        /// <returns>The <see cref="int"/></returns>
        private int RefineNonZeroes(Block8x8F* b, int zig, int zigEnd, int nz, int delta)
        {
            for (; zig <= zigEnd; zig++)
            {
                int u = Unzig[zig];
                float bu = Block8x8F.GetScalarAt(b, u);

                // TODO: Are the equality comparsions OK with floating point values? Isn't an epsilon value necessary?
                if (bu == 0)
                {
                    if (nz == 0)
                    {
                        break;
                    }

                    nz--;
                    continue;
                }

                bool bit = this.DecodeBit();
                if (!bit)
                {
                    continue;
                }

                if (bu >= 0)
                {
                    //b[u] += delta;
                    Block8x8F.SetScalarAt(b, u, bu + delta);
                }
                else
                {
                    //b[u] -= delta;
                    Block8x8F.SetScalarAt(b, u, bu - delta);
                }
            }

            return zig;
        }

        /// <summary>
        /// Makes the image from the buffer.
        /// </summary>
        /// <param name="mxx">The horizontal MCU count</param>
        /// <param name="myy">The vertical MCU count</param>
        private void MakeImage(int mxx, int myy)
        {
            if (this.componentCount == 1)
            {
                GrayImage gray = new GrayImage(8 * mxx, 8 * myy);
                this.grayImage = gray.Subimage(0, 0, this.imageWidth, this.imageHeight);
            }
            else
            {
                int h0 = this.componentArray[0].HorizontalFactor;
                int v0 = this.componentArray[0].VerticalFactor;
                int horizontalRatio = h0 / this.componentArray[1].HorizontalFactor;
                int verticalRatio = v0 / this.componentArray[1].VerticalFactor;

                YCbCrImage.YCbCrSubsampleRatio ratio = YCbCrImage.YCbCrSubsampleRatio.YCbCrSubsampleRatio444;
                switch ((horizontalRatio << 4) | verticalRatio)
                {
                    case 0x11:
                        ratio = YCbCrImage.YCbCrSubsampleRatio.YCbCrSubsampleRatio444;
                        break;
                    case 0x12:
                        ratio = YCbCrImage.YCbCrSubsampleRatio.YCbCrSubsampleRatio440;
                        break;
                    case 0x21:
                        ratio = YCbCrImage.YCbCrSubsampleRatio.YCbCrSubsampleRatio422;
                        break;
                    case 0x22:
                        ratio = YCbCrImage.YCbCrSubsampleRatio.YCbCrSubsampleRatio420;
                        break;
                    case 0x41:
                        ratio = YCbCrImage.YCbCrSubsampleRatio.YCbCrSubsampleRatio411;
                        break;
                    case 0x42:
                        ratio = YCbCrImage.YCbCrSubsampleRatio.YCbCrSubsampleRatio410;
                        break;
                }

                YCbCrImage ycbcr = new YCbCrImage(8 * h0 * mxx, 8 * v0 * myy, ratio);
                this.ycbcrImage = ycbcr.Subimage(0, 0, this.imageWidth, this.imageHeight);

                if (this.componentCount == 4)
                {
                    int h3 = this.componentArray[3].HorizontalFactor;
                    int v3 = this.componentArray[3].VerticalFactor;
                    this.blackPixels = new byte[8 * h3 * mxx * 8 * v3 * myy];
                    this.blackStride = 8 * h3 * mxx;
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the image in an RGB image.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsRGB()
        {
            if (this.isJfif)
            {
                return false;
            }

            if (this.adobeTransformValid && this.adobeTransform == JpegConstants.Adobe.ColorTransformUnknown)
            {
                // http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/JPEG.html#Adobe
                // says that 0 means Unknown (and in practice RGB) and 1 means YCbCr.
                return true;
            }

            return this.componentArray[0].Identifier == 'R' && this.componentArray[1].Identifier == 'G'
                   && this.componentArray[2].Identifier == 'B';
        }

        /// <summary>
        /// Optimized method to pack bytes to the image from the YCbCr color space.
        /// This is faster than implicit casting as it avoids double packing.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="packed">The packed pixel.</param>
        /// <param name="y">The y luminance component.</param>
        /// <param name="cb">The cb chroma component.</param>
        /// <param name="cr">The cr chroma component.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PackYcbCr<TColor, TPacked>(ref TColor packed, byte y, byte cb, byte cr)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            int ccb = cb - 128;
            int ccr = cr - 128;

            byte r = (byte)(y + (1.402F * ccr)).Clamp(0, 255);
            byte g = (byte)(y - (0.34414F * ccb) - (0.71414F * ccr)).Clamp(0, 255);
            byte b = (byte)(y + (1.772F * ccb)).Clamp(0, 255);

            packed.PackFromBytes(r, g, b, 255);
        }

        /// <summary>
        /// Optimized method to pack bytes to the image from the CMYK color space.
        /// This is faster than implicit casting as it avoids double packing.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <typeparam name="TPacked">The packed format. <example>uint, long, float.</example></typeparam>
        /// <param name="packed">The packed pixel.</param>
        /// <param name="y">The y luminance component.</param>
        /// <param name="cb">The cb chroma component.</param>
        /// <param name="cr">The cr chroma component.</param>
        /// <param name="xx">The x-position within the image.</param>
        /// <param name="yy">The y-position within the image.</param>
        private void PackCmyk<TColor, TPacked>(ref TColor packed, byte y, byte cb, byte cr, int xx, int yy)
            where TColor : struct, IPackedPixel<TPacked> where TPacked : struct
        {
            // TODO: We can speed this up further with Vector4
            int ccb = cb - 128;
            int ccr = cr - 128;

            // First convert from YCbCr to CMY
            float cyan = (y + (1.402F * ccr)).Clamp(0, 255) / 255F;
            float magenta = (y - (0.34414F * ccb) - (0.71414F * ccr)).Clamp(0, 255) / 255F;
            float yellow = (y + (1.772F * ccb)).Clamp(0, 255) / 255F;

            // Get keyline
            float keyline = (255 - this.blackPixels[(yy * this.blackStride) + xx]) / 255F;

            // Convert back to RGB
            byte r = (byte)(((1 - cyan) * (1 - keyline)).Clamp(0, 1) * 255);
            byte g = (byte)(((1 - magenta) * (1 - keyline)).Clamp(0, 1) * 255);
            byte b = (byte)(((1 - yellow) * (1 - keyline)).Clamp(0, 1) * 255);

            packed.PackFromBytes(r, g, b, 255);
        }

        /// <summary>
        /// Represents a component scan
        /// </summary>
        private struct Scan
        {
            /// <summary>
            /// Gets or sets the component index.
            /// </summary>
            public byte Index { get; set; }

            /// <summary>
            /// Gets or sets the DC table selector
            /// </summary>
            public byte DcTableSelector { get; set; }

            /// <summary>
            /// Gets or sets the AC table selector
            /// </summary>
            public byte AcTableSelector { get; set; }
        }

        /// <summary>
        /// ReadByteStuffedByte was throwing exceptions on normal execution path (very inefficent)
        /// It's better tho have an error code for this!
        /// </summary>
        internal enum ErrorCodes
        {
            NoError,
            // ReSharper disable once InconsistentNaming
            MissingFF00
        }

        /// <summary>
        /// The missing ff00 exception.
        /// </summary>
        internal class MissingFF00Exception : Exception
        {
        }

        /// <summary>
        /// The short huffman data exception.
        /// </summary>
        private class ShortHuffmanDataException : Exception
        {
        }

        /// <summary>
        /// The EOF (End of File exception).
        /// Thrown when the decoder encounters an EOF marker without a proceeding EOI (End Of Image) marker
        /// </summary>
        internal class EOFException : Exception
        {
        }

        public void Dispose()
        {
            for (int i = 0; i < this.huffmanTrees.Length; i++)
            {
                this.huffmanTrees[i].Dispose();
            }

            this.bytes.Dispose();
        }
    }
}

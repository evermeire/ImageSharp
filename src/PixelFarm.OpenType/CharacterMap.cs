﻿//Apache2, 2014-2016, Samuel Carlsson, WinterDev

using System;
namespace NOpenType
{
    class CharacterMap
    {
        //https://www.microsoft.com/typography/otspec/cmap.htm

        readonly int _segCount;
        readonly ushort[] _startCode; //Starting character code for each segment
        readonly ushort[] _endCode;//Ending character code for each segment, last = 0xFFFF.      
        readonly ushort[] _idDelta; //Delta for all character codes in segment
        readonly ushort[] _idRangeOffset; //Offset in bytes to glyph indexArray, or 0 (not offset in bytes unit)
        readonly ushort[] _glyphIdArray;

        public CharacterMap(int segCount, ushort[] startCode, ushort[] endCode, ushort[] idDelta, ushort[] idRangeOffset, ushort[] glyphIdArray)
        {
            _segCount = segCount;
            _startCode = startCode;
            _endCode = endCode;
            _idDelta = idDelta;
            _idRangeOffset = idRangeOffset;
            _glyphIdArray = glyphIdArray;
        }

        public ushort PlatformId { get; set; }
        public ushort EncodingId { get; set; }
        public int CharacterToGlyphIndex(UInt32 character)
        {
            return (int)RawCharacterToGlyphIndex(character);
        }

        public uint RawCharacterToGlyphIndex(UInt32 character)
        {
            // TODO: Fast segment lookup using bit operations?
            for (int i = 0; i < _segCount; i++)
            {
                if (_endCode[i] >= character && _startCode[i] <= character)
                {
                    if (_idRangeOffset[i] == 0)
                    {
                        return (character + _idDelta[i]) % 65536; // TODO: bitmask instead?
                    }
                    else
                    {
                        //If the idRangeOffset value for the segment is not 0,
                        //the mapping of character codes relies on glyphIdArray. 
                        //The character code offset from startCode is added to the idRangeOffset value.
                        //This sum is used as an offset from the current location within idRangeOffset itself to index out the correct glyphIdArray value. 
                        //This obscure indexing trick works because glyphIdArray immediately follows idRangeOffset in the font file.
                        //The C expression that yields the glyph index is:

                        //*(idRangeOffset[i]/2 
                        //+ (c - startCount[i]) 
                        //+ &idRangeOffset[i])

                        var offset = _idRangeOffset[i] / 2 + (character - _startCode[i]);
                        // I want to thank Microsoft for this clever pointer trick
                        // TODO: What if the value fetched is inside the _idRangeOffset table?
                        // TODO: e.g. (offset - _idRangeOffset.Length + i < 0)
                        return _glyphIdArray[offset - _idRangeOffset.Length + i];
                    }
                }
            }
            return 0;
        }
    }
}

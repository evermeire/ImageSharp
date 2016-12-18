﻿
//Apache2,  2016,  WinterDev
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace NOpenType.Tables
{



    static class TagsLookup
    {

        static Dictionary<string, TagInfo> registeredScriptTags = new Dictionary<string, TagInfo>();
        static Dictionary<string, TagInfo> registeredFeatureTags = new Dictionary<string, TagInfo>();

        static TagsLookup()
        {
            RegisterScriptTags();
            RegisterFeatureTags();
            RegisterBaselineTags();
        }


        static void RegisterScriptTags()
        {
            //https://www.microsoft.com/typography/otspec/scripttags.htm
            RegisterScriptTag("Adlam", "adlm");
            RegisterScriptTag("Anatolian Hieroglyphs", "hluw");

            RegisterScriptTag("Arabic", "arab");
            RegisterScriptTag("Armenian", "armn");
            RegisterScriptTag("Avestan", "avst");
            //
            RegisterScriptTag("Balinese", "bali");
            RegisterScriptTag("Bamum", "bamu");
            RegisterScriptTag("Bassa Vah ", "bass");
            //
            RegisterScriptTag("Batak", "batk");
            RegisterScriptTag("Bengali", "beng");
            RegisterScriptTag("Bengali v.2", "bng2");
            RegisterScriptTag("Bhaiksuki", "bhks");
            RegisterScriptTag("Brahmi", "brah");
            RegisterScriptTag("Braille", "brai");
            RegisterScriptTag("Buginese", "bugi");
            RegisterScriptTag("Buhid", "buhd");
            RegisterScriptTag("Byzantine Music", "byzm");
            //
            RegisterScriptTag("Canadian Syllabics", "cans");
            RegisterScriptTag("Carian", "cari");
            RegisterScriptTag("Caucasian Albanian", "aghb");
            //
            RegisterScriptTag("Chakma", "cakm");
            RegisterScriptTag("Cham", "cham");
            RegisterScriptTag("Cherokee", "cher");
            RegisterScriptTag("CJK Ideographic", "hani");
            RegisterScriptTag("Coptic", "copt");
            RegisterScriptTag("Cypriot Syllabary", "cprt");
            RegisterScriptTag("Cyrillic", "cyrl");
            //
            RegisterScriptTag("Default", "DFLT");
            RegisterScriptTag("Deseret", "dsrt");
            RegisterScriptTag("Devanagari", "deva");
            RegisterScriptTag("Devanagari v.2", "dev2");
            RegisterScriptTag("Duployan", "dupl");
            //            
            RegisterScriptTag("Egyptian Hieroglyphs", "egyp");
            RegisterScriptTag("Elbasan", "elba");
            RegisterScriptTag("Ethiopic", "ethi");
            // 
            RegisterScriptTag("Georgian", "geor");
            RegisterScriptTag("Glagolitic", "glag");
            RegisterScriptTag("Gothic", "goth");
            RegisterScriptTag("Grantha", "gran");
            RegisterScriptTag("Greek", "grek");
            RegisterScriptTag("Gujarati", "gujr");
            RegisterScriptTag("Gujarati v.2", "gjr2");
            RegisterScriptTag("Gurmukhi", "guru");
            RegisterScriptTag("Gurmukhi v.2", "gur2");
            // 
            RegisterScriptTag("Hangul", "hang");
            RegisterScriptTag("Hangul Jamo", "jamo");
            RegisterScriptTag("Hanunoo", "hano");
            RegisterScriptTag("Hatran", "hatr");
            RegisterScriptTag("Hebrew", "hebr");
            RegisterScriptTag("Hiragana", "kana");
            // 
            RegisterScriptTag("Imperial Aramaic", "armi");
            RegisterScriptTag("Inscriptional Pahlavi", "phli");
            RegisterScriptTag("Inscriptional Parthian", "prti");
            //             	
            RegisterScriptTag("Javanese", "java");
            // 
            RegisterScriptTag("Kaithi", "kthi");
            RegisterScriptTag("Kannada", "knda");
            RegisterScriptTag("Kannada v.2", "knd2");
            RegisterScriptTag("Katakana", "kana");
            RegisterScriptTag("Kayah Li", "kali");
            RegisterScriptTag("Kharosthi", "khar");
            RegisterScriptTag("Khmer", "khmr");
            RegisterScriptTag("Khojki", "khoj");
            RegisterScriptTag("Khudawadi", "sind");
            // 
            RegisterScriptTag("Lao", "lao");
            RegisterScriptTag("Latin", "latn");
            RegisterScriptTag("Lepcha", "lepc");
            RegisterScriptTag("Limbu", "limb");
            RegisterScriptTag("Linear A", "lina");
            RegisterScriptTag("Linear B", "linb");
            RegisterScriptTag("Lisu (Fraser)", "lisu");
            RegisterScriptTag("Lycian", "lyci");
            RegisterScriptTag("Lydian", "lydi");
            // 
            RegisterScriptTag("Mahajani", "mahj");
            RegisterScriptTag("Malayalam", "mlym");
            RegisterScriptTag("Malayalam v.2", "mlm2");
            RegisterScriptTag("Mandaic, Mandaean", "mand");
            RegisterScriptTag("Manichaean", "mani");
            RegisterScriptTag("Marchen", "marc");
            RegisterScriptTag("Mathematical Alphanumeric Symbols", "math");
            RegisterScriptTag("Meitei Mayek (Meithei, Meetei)", "mtei");
            RegisterScriptTag("Mende Kikakui", "mend");
            RegisterScriptTag("Meroitic Cursive", "merc");
            RegisterScriptTag("Meroitic Hieroglyphs", "mero");
            RegisterScriptTag("Miao", "plrd");
            RegisterScriptTag("Modi", "modi");
            RegisterScriptTag("Mongolian", "mong");
            RegisterScriptTag("Mro", "mroo");
            RegisterScriptTag("Multani", "mult");
            RegisterScriptTag("Musical Symbols", "musc");
            RegisterScriptTag("Myanmar", "mymr");
            RegisterScriptTag("Myanmar v.2", "mym2");
            //      
            RegisterScriptTag("Nabataean", "nbat");
            RegisterScriptTag("Newa", "newa");
            RegisterScriptTag("New Tai Lue", "talu");
            RegisterScriptTag("N'Ko", "nko");
            // 
            RegisterScriptTag("Odia (formerly Oriya)", "orya");
            RegisterScriptTag("Odia v.2 (formerly Oriya v.2)", "ory2");
            RegisterScriptTag("Ogham", "ogam");
            RegisterScriptTag("Ol Chiki", "olck");
            RegisterScriptTag("Old Italic", "ital");
            RegisterScriptTag("Old Hungarian", "hung");
            RegisterScriptTag("Old North Arabian", "narb");
            RegisterScriptTag("Old Permic", "perm");
            RegisterScriptTag("Old Persian Cuneiform ", "xpeo");
            RegisterScriptTag("Old South Arabian", "sarb");
            RegisterScriptTag("Old Turkic, Orkhon Runic", "orkh");
            RegisterScriptTag("Osage", "osge");
            RegisterScriptTag("Osmanya", "osma");
            // 
            RegisterScriptTag("Pahawh Hmong", "hmng");
            RegisterScriptTag("Palmyrene", "palm");
            RegisterScriptTag("Pau Cin Hau", "pauc");
            RegisterScriptTag("Phags-pa", "phag");
            RegisterScriptTag("Phoenician ", "phnx");
            RegisterScriptTag("Psalter Pahlavi", "phlp");
            // 
            RegisterScriptTag("Rejang", "rjng");
            RegisterScriptTag("Runic", "runr");
            // 
            RegisterScriptTag("Samaritan", "samr");
            RegisterScriptTag("Saurashtra", "saur");
            RegisterScriptTag("Sharada", "shrd");
            RegisterScriptTag("Shavian", "shaw");
            RegisterScriptTag("Siddham", "sidd");
            RegisterScriptTag("Sign Writing", "sgnw");
            RegisterScriptTag("Sinhala", "sinh");
            RegisterScriptTag("Sora Sompeng", "sora");
            RegisterScriptTag("Sumero-Akkadian Cuneiform", "xsux");
            RegisterScriptTag("Sundanese", "sund");
            RegisterScriptTag("Syloti Nagri", "sylo");
            RegisterScriptTag("Syriac", "syrc");
            //

            RegisterScriptTag("Tagalog", "tglg");
            RegisterScriptTag("Tagbanwa", "tagb");
            RegisterScriptTag("Tai Le", "tale");
            RegisterScriptTag("Tai Tham (Lanna)", "lana");
            RegisterScriptTag("Tai Viet", "tavt");
            RegisterScriptTag("Takri", "takr");
            RegisterScriptTag("Tamil", "taml");
            RegisterScriptTag("Tamil v.2", "tml2");
            RegisterScriptTag("Tangut", "tang");
            RegisterScriptTag("Telugu", "telu");
            RegisterScriptTag("Telugu v.2", "tel2");
            RegisterScriptTag("Thaana", "thaa");
            RegisterScriptTag("Thai", "thai");
            RegisterScriptTag("Tibetan", "tibt");
            RegisterScriptTag("Tifinagh", "tfng");
            RegisterScriptTag("Tirhuta", "tirh");
            //
            RegisterScriptTag("Ugaritic Cuneiform", "ugar");
            //
            RegisterScriptTag("Vai", "vai");
            //
            RegisterScriptTag("Warang Citi", "wara");
            //
            RegisterScriptTag("Yi", "yi");
        }

        static void RegisterFeatureTags()
        {
            //https://www.microsoft.com/typography/otspec/featurelist.htm

            RegisterFeatureTag("aalt", "Access All Alternates");
            RegisterFeatureTag("abvf", "Above-base Forms");
            RegisterFeatureTag("abvm", "Above-base Mark Positioning");
            RegisterFeatureTag("abvs", "Above-base Substitutions");
            RegisterFeatureTag("afrc", "Alternative Fractions");
            RegisterFeatureTag("akhn", "Akhands");
            //
            RegisterFeatureTag("blwf", "Below-base Forms");
            RegisterFeatureTag("blwm", "Below-base Mark Positioning");
            RegisterFeatureTag("blws", "Below-base Substitutions");
            //
            RegisterFeatureTag("calt", "Contextual Alternates");
            RegisterFeatureTag("case", "Case-Sensitive Forms");
            RegisterFeatureTag("ccmp", "Glyph Composition / Decomposition");
            RegisterFeatureTag("cfar", "Conjunct Form After Ro");
            RegisterFeatureTag("cjct", "Conjunct Forms");
            RegisterFeatureTag("clig", "Contextual Ligatures");
            RegisterFeatureTag("cpct", "Centered CJK Punctuation");
            RegisterFeatureTag("cpsp", "Capital Spacing");
            RegisterFeatureTag("cswh", "Contextual Swash");
            RegisterFeatureTag("curs", "Cursive Positioning");
            for (int i = 1; i < 9; ++i)
            {
                RegisterFeatureTag("cv0" + i, "Character Variants" + i);
            }
            for (int i = 10; i < 100; ++i)
            {
                RegisterFeatureTag("cv" + i, "Character Variants" + i);
            }
            RegisterFeatureTag("c2pc", "Petite Capitals From Capitals");
            RegisterFeatureTag("c2sc", "Small Capitals From Capitals");
            //
            RegisterFeatureTag("dist", "Distances");
            RegisterFeatureTag("dlig", "Discretionary Ligatures");
            RegisterFeatureTag("dnom", "Denominators");
            RegisterFeatureTag("dtls", "Dotless Forms");
            //
            RegisterFeatureTag("expt", "Expert Forms");
            //
            RegisterFeatureTag("falt", "Final Glyph on Line Alternates");
            RegisterFeatureTag("fin2", "Terminal Forms #2");
            RegisterFeatureTag("fin3", "Terminal Forms #3");
            RegisterFeatureTag("fina", "Terminal Forms");
            RegisterFeatureTag("flac", "Flattened accent forms");
            RegisterFeatureTag("frac", "Fractions");
            RegisterFeatureTag("fwid", "Full Widths");
            //
            RegisterFeatureTag("half", "Half Forms");
            RegisterFeatureTag("haln", "Halant Forms");
            RegisterFeatureTag("halt", "Alternate Half Widths");
            RegisterFeatureTag("hist", "Historical Forms");
            RegisterFeatureTag("hkna", "Horizontal Kana Alternates");
            RegisterFeatureTag("hlig", "Historical Ligatures");
            RegisterFeatureTag("hngl", "Hangul");
            RegisterFeatureTag("hojo", "Hojo Kanji Forms (JIS X 0212-1990 Kanji Forms)");
            RegisterFeatureTag("hwid", "Half Widths");
            //
            RegisterFeatureTag("init", "Initial Forms");
            RegisterFeatureTag("isol", "Isolated Forms");
            RegisterFeatureTag("ital", "Italics");
            //Italics
            RegisterFeatureTag("jalt", "Justification Alternates");
            RegisterFeatureTag("jp78", "JIS78 Forms");
            RegisterFeatureTag("jp83", "JIS83 Forms");
            RegisterFeatureTag("jp90", "JIS90 Forms");
            RegisterFeatureTag("jp04", "JIS2004 Forms");
            //
            RegisterFeatureTag("kern", "Kerning");
            //
            RegisterFeatureTag("lfbd", "Left Bounds");
            RegisterFeatureTag("liga", "Standard Ligatures");
            RegisterFeatureTag("ljmo", "Leading Jamo Forms");
            RegisterFeatureTag("lnum", "Lining Figures");
            RegisterFeatureTag("locl", "Localized Forms");
            RegisterFeatureTag("ltra", "Left-to-right alternates");
            RegisterFeatureTag("ltrm", "Left-to-right mirrored forms");
            //
            RegisterFeatureTag("mark", "Mark Positioning");
            RegisterFeatureTag("med2", "Medial Forms #2");
            RegisterFeatureTag("medi", "Medial Forms");
            RegisterFeatureTag("mgrk", "Mathematical Greek");
            RegisterFeatureTag("mkmk", "Mark to Mark Positioning");
            RegisterFeatureTag("mset", "Mark Positioning via Substitution");
            //
            RegisterFeatureTag("nalt", "Alternate Annotation Forms");
            RegisterFeatureTag("nlck", "NLC Kanji Forms");
            RegisterFeatureTag("nukt", "Nukta Forms");
            RegisterFeatureTag("numr", "Numerators");
            //
            RegisterFeatureTag("onum", "Oldstyle Figures");
            RegisterFeatureTag("opbd", "Optical Bounds");
            RegisterFeatureTag("ordn", "Ordinals");
            RegisterFeatureTag("ornm", "Ornaments");
            //
            RegisterFeatureTag("palt", "Proportional Alternate Widths");
            RegisterFeatureTag("pcap", "Petite Capitals");
            RegisterFeatureTag("pkna", "Proportional Kana");
            RegisterFeatureTag("pnum", "Proportional Figures");
            RegisterFeatureTag("pref", "Pre-Base Forms");
            RegisterFeatureTag("pres", "Pre-base Substitutions");
            RegisterFeatureTag("pstf", "Post-base Forms");
            RegisterFeatureTag("psts", "Post-base Substitutions");
            RegisterFeatureTag("pwid", "Proportional Widths");
            //
            RegisterFeatureTag("qwid", "Quarter Widths");
            //
            RegisterFeatureTag("rand", "Randomize");
            RegisterFeatureTag("rclt", "Required Contextual Alternates");
            RegisterFeatureTag("rkrf", "Rakar Forms");
            RegisterFeatureTag("rlig", "Required Ligatures");
            RegisterFeatureTag("rphf", "Reph Forms");
            //

            RegisterFeatureTag("rtbd", "Right Bounds");
            RegisterFeatureTag("rtla", "Right-to-left alternates");
            RegisterFeatureTag("rtlm", "Right-to-left mirrored forms");
            RegisterFeatureTag("ruby", "Ruby Notation Forms");
            RegisterFeatureTag("rvrn", "Required Variation Alternates");
            //
            RegisterFeatureTag("salt", "Stylistic Alternates");
            RegisterFeatureTag("sinf", "Scientific Inferiors");
            RegisterFeatureTag("size", "Optical size");
            RegisterFeatureTag("smcp", "Small Capitals");
            RegisterFeatureTag("smpl", "Simplified Forms");
            RegisterFeatureTag("ss01", "Stylistic Set 1");
            RegisterFeatureTag("ss02", "Stylistic Set 2");
            RegisterFeatureTag("ss03", "Stylistic Set 3");
            RegisterFeatureTag("ss04", "Stylistic Set 4");
            RegisterFeatureTag("ss05", "Stylistic Set 5");
            RegisterFeatureTag("ss06", "Stylistic Set 6");
            RegisterFeatureTag("ss07", "Stylistic Set 7");
            RegisterFeatureTag("ss08", "Stylistic Set 8");
            RegisterFeatureTag("ss09", "Stylistic Set 9");
            RegisterFeatureTag("ss10", "Stylistic Set 10");
            RegisterFeatureTag("ss11", "Stylistic Set 11");
            RegisterFeatureTag("ss12", "Stylistic Set 12");
            RegisterFeatureTag("ss13", "Stylistic Set 13");
            RegisterFeatureTag("ss14", "Stylistic Set 14");
            RegisterFeatureTag("ss15", "Stylistic Set 15");
            RegisterFeatureTag("ss16", "Stylistic Set 16");
            RegisterFeatureTag("ss17", "Stylistic Set 17");
            RegisterFeatureTag("ss18", "Stylistic Set 18");
            RegisterFeatureTag("ss19", "Stylistic Set 19");
            RegisterFeatureTag("ss20", "Stylistic Set 20");
            // 
            RegisterFeatureTag("ssty", "Math script style alternates");
            RegisterFeatureTag("stch", "Stretching Glyph Decomposition");
            RegisterFeatureTag("subs", "Subscript");
            RegisterFeatureTag("sups", "Superscript");
            RegisterFeatureTag("swsh", "Swash");
            //
            RegisterFeatureTag("titl", "Titling");
            RegisterFeatureTag("tjmo", "Trailing Jamo Forms");
            RegisterFeatureTag("tnam", "Traditional Name Forms");
            RegisterFeatureTag("tnum", "Tabular Figures");
            RegisterFeatureTag("trad", "Traditional Forms");
            RegisterFeatureTag("twid", "Third Widths");
            //            
            RegisterFeatureTag("unic", "Unicase");

            RegisterFeatureTag("valt", "Alternate Vertical Metrics");
            RegisterFeatureTag("vatu", "Vattu Variants");
            RegisterFeatureTag("vert", "Vertical Writing");
            RegisterFeatureTag("vhal", "Alternate Vertical Half Metrics");
            RegisterFeatureTag("vjmo", "Vowel Jamo Forms");
            RegisterFeatureTag("vkna", "Vertical Kana Alternates");
            RegisterFeatureTag("vkrn", "Vertical Kerning");
            RegisterFeatureTag("vpal", "Proportional Alternate Vertical Metrics");
            RegisterFeatureTag("vrt2", "Vertical Alternates and Rotation");
            RegisterFeatureTag("vrtr", "Vertical Alternates for Rotation");
            //


        }

        static void RegisterBaselineTags()
        {
            //TODO: implement here
        }
        static void RegisterLanguageTags()
        {
            //https://www.microsoft.com/typography/otspec/languagetags.htm

        }
        public static TagInfo GetTagInfo(string shortname)
        {
            TagInfo found;
            registeredScriptTags.TryGetValue(shortname, out found);
            return found;
        }
#if DEBUG
        static void debugCheckShortName(string shortname)
        {
            if (shortname.Length > 4)
            {
                throw new NotSupportedException();
            }
        }
#endif
        static void RegisterScriptTag(string fullname, string shortname)
        {
#if DEBUG
            debugCheckShortName(shortname);
#endif
            if (registeredScriptTags.ContainsKey(shortname))
            {
                //TODO: fix this
                if (shortname == "kana")
                {

                    return;
                }
            }
            registeredScriptTags.Add(shortname, new TagInfo(TagKind.Script, shortname, fullname));
        }
        static void RegisterFeatureTag(string shortname, string fullname)
        {
#if DEBUG
            debugCheckShortName(shortname);
#endif
            registeredFeatureTags.Add(shortname, new TagInfo(TagKind.Feature, shortname, fullname));             
        }
    }

    enum TagKind
    {
        Script,
        Feature,
    }
    class TagInfo
    {

        public string ShortName { get; private set; }
        public string FullName { get; private set; }
        public TagKind Kind { get; private set; }
        public TagInfo(TagKind kind, string shortName, string fullname)
        {
            this.Kind = kind;
            this.FullName = fullname;
            this.ShortName = shortName;
        }

    }
}
﻿//from http://stackoverflow.com/questions/3633000/net-enumerate-winforms-font-styles
// https://www.microsoft.com/Typography/OTSpec/name.htm
//MIT 2016, WinterDev

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace NOpenType
{
    public class InstalledFont
    {

        public InstalledFont(string fontName, string fontSubFamily)
        {
            FontName = fontName;
            FontSubFamily = fontSubFamily;
            FontPath = "";
        }
        public string FontName { get; set; }
        public string FontSubFamily { get; set; }
        public string FontPath { get; set; }

#if DEBUG
        public override string ToString()
        {
            return FontName;
        }
#endif
    }

}
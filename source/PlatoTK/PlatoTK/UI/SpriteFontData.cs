/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System.Collections.Generic;

namespace PlatoTK.UI
{
    internal class SpriteFontData
    {
        public int LineSpacing { get; set; }
        public float Spacing { get; set; }
        public char? DefaultCharacter { get; set; }
        public List<char> Characters { get; set; }
        public Dictionary<char, SpriteFontGlyphData> Glyphs { get; set; }
    }
}

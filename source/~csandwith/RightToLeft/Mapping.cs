/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RightToLeft
{
    public class Mapping
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public int xo;
        public int yo;
        public int xa;
    }
    public class SpriteFontMapping
    {
        public int LineSpacing;
        public float Spacing = -1;
        public char DefaultCharacter;
        public List<char> Characters = new();
        public Dictionary<char, SpriteFont.Glyph> Glyphs = new();
    }
}
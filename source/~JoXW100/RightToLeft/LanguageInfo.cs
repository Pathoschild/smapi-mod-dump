/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;

namespace RightToLeft
{
    public class LanguageInfo
    {
        public string name;
        public string code;
        public char defaultCharacter;
        public int xOffset;
        public bool useXAdvance;
        public int dialogueFontLineSpacing = 50;
        public int smallFontLineSpacing = 33;
        public int tinyFontLineSpacing = 25;
        public float dialogueFontSpacing = -2;
        public float smallFontSpacing = -1;
        public float tinyFontSpacing = 1;
        public ModLanguage metaData;

        public SpriteFont dialogueFont;
        public SpriteFont smallFont;
        public SpriteFont tinyFont;
        public string path;

    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace MobileArcade
{
    class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public Color BackgroundColor { get; set; } = new Color(41, 57, 106);
        public Color PostBackgroundColor { get; set; } = Color.White;
        public int PostMarginX { get; set; } = 16;
        public int PostMarginY { get;  set; } = 16;
        public int PostHeight { get; set; } = 128;
    }
}

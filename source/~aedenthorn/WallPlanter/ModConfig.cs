/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;

namespace WallPlanter
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton ModKey { get; set; } = SButton.LeftShift;
        public SButton UpButton { get; set; } = SButton.Up;
        public SButton DownButton { get; set; } = SButton.Down;
        public int OffsetY { get; set; } = 64;
        public int InnerOffsetY { get; set; } = 0;
    }
}

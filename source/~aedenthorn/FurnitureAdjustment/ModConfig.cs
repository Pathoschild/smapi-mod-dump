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

namespace FurnitureAdjustment
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool MoveCursor { get; set; } = true;

        public SButton RaiseButton { get; set; } = SButton.NumPad8;
        public SButton LowerButton { get; set; } = SButton.NumPad2;
        public SButton LeftButton { get; set; } = SButton.NumPad4;
        public SButton RightButton { get; set; } = SButton.NumPad6;
        public SButton ModKey { get; set; } = SButton.LeftAlt;
        public int ModSpeed { get; set; } = 5;
        public int MoveSpeed { get; set; } = 10;
    }
}

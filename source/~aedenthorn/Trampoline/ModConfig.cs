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

namespace Trampoline
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public string JumpSound { get; set; } = "dwop";
        public SButton ConvertKey { get; set; } = SButton.T;
        public SButton HigherKey { get; set; } = SButton.Up;
        public SButton LowerKey { get; set; } = SButton.Down;
        public SButton SlowerKey { get; set; } = SButton.Left;
        public SButton FasterKey { get; set; } = SButton.Right;
        public float JumpSpeed { get; set; } = 6f;
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace SocialPageOrderRedux
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public KeybindList prevButton { get; set; } = KeybindList.Parse($"{SButton.Down}, {SButton.DPadLeft}");
        public KeybindList nextButton { get; set; } = KeybindList.Parse($"{SButton.Up}, {SButton.DPadRight}");
        public bool UseButton { get; set; } = true;
        public bool UseDropdown { get; set; } = true;
        public bool UseFilter { get; set; } = true;
        public int ButtonOffsetX { get; set; } = 0;
        public int ButtonOffsetY { get; set; } = 0;
        public int DropdownOffsetX { get; set; } = 0;
        public int DropdownOffsetY { get; set; } = 0;
        public int FilterOffsetX { get; set; } = 0;
        public int FilterOffsetY { get; set; } = 0;
    }
}

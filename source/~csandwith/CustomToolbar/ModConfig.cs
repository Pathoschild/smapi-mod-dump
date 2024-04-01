/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/


using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace CustomToolbar
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool ShowWithActiveMenu { get; set; } = false;
        public bool Vertical { get; set; } = false;
        public SButton RotateKey { get; set; } = SButton.MouseMiddle;
        public bool SetPosition { get; set; } = true;
        public string PinnedPosition { get; set; } = "bottom";
        public int PositionX { get; set; } = -1;
        public int PositionY { get; set; } = -1;
        public int MarginX { get; set; } = 64;
        public int MarginY { get; set; } = 64;
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 50;
    }
}

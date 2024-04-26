/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BussinBungus/BungusSDVMods
**
*************************************************/

using StardewModdingAPI;

namespace LostBookMenu
{
    public class ModConfig
    /// Original ///
    /*{
        public bool ModEnabled { get; set; } = true;
        public bool MenuInLibrary { get; set; } = true;
        public SButton MenuKey { get; set; } = SButton.None;
        public bool LegacyTitles { get; set; } = true;
        public int WindowWidth { get; set; } = 1600;
        public int WindowHeight { get; set; } = 900;
        public int GridColumns { get; set; } = 7;
        public int xOffset { get; set; } = -1;
        public int yOffset { get; set; } = 160;
        public float CoverScale { get; set; } = 8;
        public int HorizontalSpace { get; set; } = 96;
        public int VerticalSpace { get; set; } = 116;
    }*/
    /// Smaller ///
    {
        public bool ModEnabled { get; set; } = true;
        public bool MenuInLibrary { get; set; } = true;
        public SButton MenuKey { get; set; } = SButton.None;
        public bool LegacyTitles { get; set; } = false;
        public int WindowWidth { get; set; } = 1050;
        public int WindowHeight { get; set; } = 600;
        public int GridColumns { get; set; } = 7;
        public int xOffset { get; set; } = -1;
        public int yOffset { get; set; } = 160;
        public float CoverScale { get; set; } = 8;
        public int HorizontalSpace { get; set; } = 16;
        public int VerticalSpace { get; set; } = 20;
    }
}

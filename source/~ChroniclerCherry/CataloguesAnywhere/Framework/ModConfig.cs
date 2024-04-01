/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;

namespace CataloguesAnywhere.Framework
{
    class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public SButton ActivateButton { get; set; } = SButton.LeftControl;
        public SButton furnitureButton { get; set; } = SButton.D1;
        public SButton WallpaperButton { get; set; } = SButton.D2;
    }
}

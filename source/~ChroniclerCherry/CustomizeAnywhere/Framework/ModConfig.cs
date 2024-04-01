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

namespace CustomizeAnywhere.Framework
{
    class ModConfig
    {
        public bool canAccessMenusAnywhere { get; set; }
        public SButton ActivateButton { get; set; }
        public SButton customizeButton { get; set; }
        public SButton dyeButton { get; set; }
        public SButton tailoringButton { get; set; }
        public SButton dresserButton { get; set; }
        public bool canTailorWithoutEvent { get; set; }

        public ModConfig()
        {
            this.canAccessMenusAnywhere = true;
            this.ActivateButton = SButton.LeftShift;
            this.customizeButton = SButton.D1;
            this.dyeButton = SButton.D2;
            this.tailoringButton = SButton.D3;
            this.dresserButton = SButton.D4;
            this.canTailorWithoutEvent = false;
        }
    }
}

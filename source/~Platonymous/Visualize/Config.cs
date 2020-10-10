/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;

namespace Visualize
{
    internal class Config
    {
        public string activeProfile { get; set; } = "Platonymous.Original";
        public float saturation { get; set; } = 100;
        public SButton next { get; set; } = SButton.PageDown;
        public SButton previous { get; set; } = SButton.PageUp;
        public SButton satHigher { get; set; } = SButton.NumPad9;
        public SButton satLower { get; set; } = SButton.NumPad6;
        public SButton reset { get; set; } = SButton.NumPad0;
        public int passes { get; set; } = 10;

    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MouseyPounds/stardew-mods
**
*************************************************/

using StardewModdingAPI;

namespace CropColorCombiner
{
    class ModConfig
    {
        public SButton Unify_Color_Keybind { get; set; } = SButton.OemOpenBrackets;
        public bool Unify_Color_For_Flowers { get; set; } = true;
        public bool Unify_Color_For_Everything_Else { get; set; } = false;
        public SButton Reduce_Quality_Keybind { get; set; } = SButton.OemCloseBrackets;
        public bool Reduce_Quality_For_Flowers { get; set; } = true;
        public bool Reduce_Quality_For_Everything_Else { get; set; } = false;

    }
}

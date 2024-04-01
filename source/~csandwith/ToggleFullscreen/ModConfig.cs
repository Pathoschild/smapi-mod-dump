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

namespace ToggleFullScreen
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public KeybindList ToggleButtons { get; set; } = new KeybindList(new Keybind(SButton.LeftAlt, SButton.Enter));
        public int LastWindowedWidth { get; set; } = -1;
        public int LastWindowedHeight { get; set; } = -1;
    }
}

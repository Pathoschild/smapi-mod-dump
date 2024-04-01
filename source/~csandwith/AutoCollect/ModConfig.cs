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

namespace AutoCollect
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public KeybindList ToggleKey { get; set; } = new KeybindList(SButton.NumPad9);
        public int MaxDistance { get; set; } = 1;
    }
}

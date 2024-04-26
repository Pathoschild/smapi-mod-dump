/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jslattery26/stardew_mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ChestPoolingV2
{
    internal class ModConfig
    {
        public ModControlsConfig Keys { get; set; } = new();
    }

    internal class ModControlsConfig
    {
        public KeybindList DisablePoolingToggle { get; set; } = new(SButton.O);
    }
}
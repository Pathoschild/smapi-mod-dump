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

namespace SharedRecipes
{
    internal class ModConfig
    {

        public bool ShareCrafting { get; set; } = true;
        public bool ShareCooking { get; set; } = true;

        public ModControlsConfig Keys { get; set; } = new();
    }

    internal class ModControlsConfig
    {
        public KeybindList SyncRecipes { get; set; } = new(keybinds: [new Keybind(SButton.LeftShift, SButton.S, SButton.R)]);
    }

}
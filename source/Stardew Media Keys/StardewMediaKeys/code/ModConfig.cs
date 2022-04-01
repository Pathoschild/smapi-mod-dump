/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AngeloC3/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace StardewMediaKeys
{
    class ModConfig
    {
        /// <summary>The key used to activate the menu</summary>
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("OemPeriod");

        /// <summary>true if the title is in blue - false if default text color</summary>
        public bool BlueNotDefaultTitle { get; set; } = true;

        /// <summary>Disables the hotkey making this only avaialble for use via Mobile Phone</summary>
        public bool OnlyUsableWithPhone { get; set; } = false;
    }
}

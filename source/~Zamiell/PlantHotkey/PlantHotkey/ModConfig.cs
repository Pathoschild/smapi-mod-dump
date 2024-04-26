/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace PlantHotkey
{
    public sealed class ModConfig
    {
        public KeybindList Hotkey { get; set; } = KeybindList.Parse("LeftControl");
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/PrimmR/Turbo
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace Turbo
{
    /// <summary>Model for config.json.</summary>
    public sealed class ModConfig
    {
        public int clockMode { get; set; } = 0; // 0 Regular, 1 Constant, 2 Frozen

        public KeybindList decrementSpeedKeybind { get; set; } = KeybindList.Parse("OemComma");

        public KeybindList incrementSpeedKeybind { get; set; } = KeybindList.Parse("OemPeriod");

        public KeybindList resetSpeedKeybind { get; set; } = KeybindList.Parse("OemSemicolon");

        public KeybindList cycleClockModeKeybind { get; set; } = KeybindList.Parse("OemQuotes");
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TimeSpeed.Framework
{
    /// <summary>The keyboard bindings used to control the flow of time. See available keys at <a href="https://msdn.microsoft.com/en-us/library/microsoft.xna.framework.input.keys.aspx" />. Set a key to null to disable it.</summary>
    internal class ModControlsConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Freeze or unfreeze time. Freezing time will stay in effect until you unfreeze it; unfreezing time will stay in effect until you enter a new location with time settings.</summary>
        public KeybindList FreezeTime { get; set; } = new(SButton.N);

        /// <summary>Slow down time by one second per 10-game-minutes. Combine with Control to increase by 100 seconds, Shift to increase by 10 seconds, or Alt to increase by 0.1 seconds.</summary>
        public KeybindList IncreaseTickInterval { get; set; } = new(SButton.OemPeriod);

        /// <summary>Speed up time by one second per 10-game-minutes. Combine with Control to decrease by 100 seconds, Shift to decrease by 10 seconds, or Alt to decrease by 0.1 seconds.</summary>
        public KeybindList DecreaseTickInterval { get; set; } = new(SButton.OemComma);

        /// <summary>Reload all values from the config file and apply them immediately. Time will stay frozen if it was frozen via keybind.</summary>
        public KeybindList ReloadConfig { get; set; } = new(SButton.B);
    }
}

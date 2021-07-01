/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TransparencySettings
{
    /// <summary>A class that performs certain tasks when keys/buttons are pressed.</summary>
    public static class InputManager
    {
        /// <summary>True if the "disable transparency" toggle is current active.</summary>
        public static PerScreen<bool> DisableTransparency { get; private set; } = new PerScreen<bool>(() => false); //default to false for each local player
        /// <summary>True if the "full transparency" toggle is current active.</summary>
        public static PerScreen<bool> FullTransparency { get; private set; } = new PerScreen<bool>(() => false); //default to false for each local player

        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Initializes this manager, e.g. by enabling its SMAPI events.</summary>
        public static void Initialize(IModHelper helper, IMonitor monitor)
        {
            Monitor = monitor;

            helper.Events.Input.ButtonsChanged += ButtonsChanged;
        }

        private static void ButtonsChanged(object sender, StardewModdingAPI.Events.ButtonsChangedEventArgs e)
        {
            if (Context.IsWorldReady == false)
                return; //do nothing

            bool full = ModEntry.Config.KeyBindings.FullTransparency.JustPressed(); //if a "full transparency" keybind was just pressed
            bool disable = ModEntry.Config.KeyBindings.DisableTransparency.JustPressed(); //if a "disable transparency" keybind was just pressed

            if (full && disable) //if both keybinds were pressed just pressed in the same tick (e.g. due to partially or fully overlapping bindings)
            {
                if (ModEntry.Config.KeyBindings.DisableTransparency.GetKeybindCurrentlyDown().Buttons.Length > ModEntry.Config.KeyBindings.FullTransparency.GetKeybindCurrentlyDown().Buttons.Length) //if the "disable" keybind uses more buttons than the "full" keybind
                    ToggleDisableTransparency();
                else //if the "disable" keybind uses fewer/equal buttons
                    ToggleFullTransparency();
            }
            else if (full) //if only "full" was pressed this tick
                ToggleFullTransparency();
            else if (disable) //if only "disable" was pressed this tick
                ToggleDisableTransparency();
        }

        private static void ToggleFullTransparency()
        {
            FullTransparency.Value = !FullTransparency.Value; //toggle this setting for the current player
            DisableTransparency.Value = false; //turn the other setting off
            Monitor?.VerboseLog($"Full transparency keybind pressed. Current mode: {(FullTransparency.Value ? "Full" : "Default")}.");
        }

        private static void ToggleDisableTransparency()
        {
            DisableTransparency.Value = !DisableTransparency.Value; //toggle this setting for the current player
            FullTransparency.Value = false; //turn the other setting off
            Monitor?.VerboseLog($"Disable transparency keybind pressed. Current mode: {(DisableTransparency.Value ? "Disable" : "Default")}.");
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace DeluxeJournal.Framework
{
    internal class Config
    {
        /// <summary>Enable to push renewed tasks to the top of the task group instead of the bottom.</summary>
        public bool PushRenewedTasksToTheTop { get; set; } = false;

        /// <summary>Enable to have the "Smart Add" button be the default when creating a task (if applicable).</summary>
        public bool EnableDefaultSmartAdd { get; set; } = true;

        /// <summary>Enable to show an indicator on the journal button when a task is completed.</summary>
        public bool EnableVisualTaskCompleteIndicator { get; set; } = false;

        /// <summary>Show the "Smart Add" info box in the "Add Task" window.</summary>
        public bool ShowSmartAddTip { get; set; } = true;

        /// <summary>Show the help message when the task page is empty.</summary>
        public bool ShowAddTaskHelpMessage { get; set; } = true;

        /// <summary>Toggle between "Net Wealth" and "Total Amount to Pay/Gain" display modes.</summary>
        public bool MoneyViewNetWealth { get; set; } = false;

        /// <summary>Keybind for toggling the visibility of overlays.</summary>
        public KeybindList ToggleOverlaysKeybind { get; set; } = KeybindList.Parse("O");

        /// <summary>Overlay background color hex code (alpha normalized RGB values for blending).</summary>
        public string OverlayBackgroundColor { get; set; } = "00000040";

        /// <summary>The name of the color schema file to load from "assets/data/colors/". Uses the default loading rules if empty.</summary>
        public string TargetColorSchemaFile { get; set; } = string.Empty;

        /// <summary>Save data to the mod configuration file.</summary>
        public void Save()
        {
            if (DeluxeJournalMod.Instance is not DeluxeJournalMod mod)
            {
                throw new InvalidOperationException("Attempted to save config before mod entry.");
            }

            mod.Helper.WriteConfig(this);
        }
    }
}

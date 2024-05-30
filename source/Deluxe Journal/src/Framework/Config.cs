/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Framework
{
    internal class Config
    {
        /// <summary>Enable to push renewed tasks to the top of the task list instead of the bottom.</summary>
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
    }
}

using System;
using StardewValley;
using StardewModdingAPI;

namespace CleanFarm.CleanTasks
{
    /// <summary>The interface for a clean task.</summary>
    interface ICleanTask
    {
        /// <summary>Can this task be run. Usually checks the config settings to see if it's enabled.</summary>
        bool CanRun();

        /// <summary>Runs the clean task.</summary>
        /// <param name="farm">The farm to be cleaned.</param>
        void Run(Farm farm);

        /// <summary>Prints to the console all the items that were removed in a nice format.</summary>
        /// <param name="monitor">The monitor interface to access the log method from.</param>
        void ReportRemovedItems(IMonitor monitor);

        /// <summary>Restores all removed items for debug purposes.</summary>
        /// <param name="farm">The farm to restore the items to.</param>
        /// <returns>The number of items that were restored.</returns>
        int RestoreRemovedItems(Farm farm);
    }
}

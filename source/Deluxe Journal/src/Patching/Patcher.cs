/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;

namespace DeluxeJournal.Patching
{
    internal static class Patcher
    {
        /// <summary>Apply Harmony patches.</summary>
        /// <param name="harmony">Harmony instance.</param>
        /// <param name="monitor">Monitor for error logging.</param>
        /// <param name="patches">The patches to apply.</param>
        public static void Apply(Harmony harmony, IMonitor monitor, params IPatch[] patches)
        {
            foreach (IPatch patch in patches)
            {
                try
                {
                    patch.Apply(harmony);
                }
                catch (Exception ex)
                {
                    monitor.Log($"Failed to apply Harmony patch '{patch.Name}'. See log file for details.", LogLevel.Error);
                    monitor.Log(ex.ToString(), LogLevel.Trace);
                }
            }
        }
    }
}

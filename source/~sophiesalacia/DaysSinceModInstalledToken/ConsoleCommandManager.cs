/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Linq;
using StardewModdingAPI;

namespace DaysSinceModInstalledToken;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {

        Globals.CCHelper.Add("sophie.dsmi.infodump", "Dumps contents of data file to the log. Warning: this will probably be a lot of data, don't use this command unless you need to.", (_, _) =>
        {
            if (!Context.IsWorldReady)
            {
                Log.Warn("This command should only be used in a loaded save.");
                return;
            }

            Log.Info($"Contents of \"data/{DaysSinceModInstalledHelper.CurrentSaveId}/data.json\":" +
                $"\n\t{string.Join("\n\t", DaysSinceModInstalledHelper.ModInstallStrings.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
        });

    }
}

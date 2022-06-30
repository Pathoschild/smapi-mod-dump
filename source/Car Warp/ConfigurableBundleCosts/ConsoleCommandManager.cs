/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Text;
using StardewModdingAPI;
using StardewValley;

namespace ConfigurableBundleCosts;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add("sophie.cbc.dump_bundle_data", "Dumps bundle data for the loaded save", (_, _) =>
            {
                if (!Context.IsWorldReady)
                {
                    Log.Error("Only use this command in a loaded save.");
                    return;
                }

                Dictionary<string, string> bundleData = Game1.netWorldState?.Value.BundleData;
                if (bundleData is null)
                {
                    Log.Info("No bundle data found.");
                    return;
                }

                StringBuilder bundleString = new("\n");

                foreach ((string key, string value) in bundleData)
                {
                    bundleString.AppendLine($"\t{key}: {value}");
                }

                Log.Info(bundleString.ToString());
            }
        );
    }
}

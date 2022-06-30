/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace CarWarp;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add("sophie.cw.warp", "Triggers the Car Warp dialogue", (_, _) =>
            {
                if (Globals.SolidFoundationsApi is null || !Context.IsWorldReady)
                {
                    Log.Error("Unable to trigger warp.");
                    return;
                }

                new CarWarp().Activate();
            }
        );
    }
}

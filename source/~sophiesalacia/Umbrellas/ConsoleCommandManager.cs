/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using static StardewValley.GameLocation;

namespace Umbrellas;

internal class ConsoleCommandManager
{
    internal static bool PerpetualRain = false;
    internal static bool ForceUmbrellas = false;
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add(
            "sophie.umbrellas.perpetualrain",
            "Toggles perpetual rain on or off.",
            TogglePerpetualRain
        );

        Globals.CCHelper.Add(
            "sophie.umbrellas.forceumbrellas",
            "Forces NPCs to carry umbrellas regardless of the setting and weather.",
            ToggleForceUmbrellas
        );

        Globals.CCHelper.Add(
            "sophie.umbrellas.reloaddata",
            "Reloads umbrella data asset.",
            ReloadData
        );
    }

    private static void TogglePerpetualRain(string command, string[] args)
    {
        PerpetualRain = !PerpetualRain;
        EventHookManager.CheckPerpetualRain(null, null);
        Log.Info(PerpetualRain ? "Perpetual rain toggled on." : "Perpetual rain toggled off.");
    }

    private static void ToggleForceUmbrellas(string command, string[] args)
    {
        ForceUmbrellas = !ForceUmbrellas;
        EventHookManager.CheckUmbrellaNeeded(null, null);
        Log.Info(ForceUmbrellas ? "Umbrellas strictly enforced." : "Umbrella mandate lifted.");
    }
    private static void ReloadData(string command, string[] args)
    {
        AssetManager.ReloadData();
        HarmonyPatches.CachedUmbrellaData.Clear();
    }
}

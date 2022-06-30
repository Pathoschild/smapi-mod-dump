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
using StardewValley;
using System;

namespace ConfigurableBundleCosts;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /// <summary>The mod entry point.</summary>
    /// <param name="helper" />
    public override void Entry(IModHelper helper)
    {
        Globals.InitializeGlobals(this);
        Globals.InitializeConfig();
        EventHookManager.InitializeEventHooks();
        ConsoleCommandManager.InitializeConsoleCommands();
        ApplyPatches();
    }

    private void ApplyPatches()
    {
        Monitor.Log(HarmonyPatches.ApplyHarmonyPatches() ? "Patches successfully applied" : "Failed to apply patches");
    }
}

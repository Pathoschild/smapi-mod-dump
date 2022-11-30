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

namespace MoreTokens;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += OnGameLaunched;
    }

    private static void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        HarmonyPatcher.ApplyPatches();

        if (!Globals.InitializeCpApi())
        {
            Log.Warn("Unable to locate ContentPatcher API. Aborting token registration.");
            return;
        }

        TokenManager.RegisterTokens();
    }
}
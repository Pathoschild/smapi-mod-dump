/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using StardewValley;

namespace DialaTarotCSharp;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += OnGameLaunched;
        Globals.EventHelper.Content.AssetRequested += AssetManager.LoadOrEditAssets;
        Globals.EventHelper.GameLoop.DayEnding += (_, _) =>
        {
            Game1.player.modData.Remove("sophie.DialaTarot/ReadingDoneForToday");
        };
    }

    private static void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        HarmonyPatcher.ApplyPatches();
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using StardewModdingAPI.Events;

namespace DaysSinceModInstalledToken;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += Init;
        Globals.EventHelper.GameLoop.SaveLoaded += DaysSinceModInstalledHelper.LoadOrCreateFile;
        Globals.EventHelper.GameLoop.DayEnding += DaysSinceModInstalledHelper.UpdateModInstallStrings;
    }

    private static void Init(object? sender, GameLaunchedEventArgs e)
    {
        Globals.ContentPatcherApi = Globals.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

        if (Globals.ContentPatcherApi is null)
        {
            Log.Error("Failed to initialize Content Patcher API. Aborting initialization.");
            return;
        }

        TokenManager.RegisterToken();

        DaysSinceModInstalledHelper.GetModList();
    }
}

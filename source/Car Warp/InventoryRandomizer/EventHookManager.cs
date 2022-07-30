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

namespace InventoryRandomizer;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.SaveLoaded += ReloadAllObjectData;
        Globals.EventHelper.GameLoop.SaveLoaded += SetUpUtils;
        Globals.EventHelper.Content.AssetReady += ReloadObjectData;
        Globals.EventHelper.GameLoop.OneSecondUpdateTicked += TimeManager.OnOneSecondUpdateTicked;
        Globals.EventHelper.GameLoop.GameLaunched += InitializeGmcmMenu;
    }

    private static void ReloadAllObjectData(object sender, SaveLoadedEventArgs e)
    {
        Log.Info("Reloading object data for all supported assets.");
        AssetManager.ReloadObjectData();

    }

    private static void SetUpUtils(object sender, SaveLoadedEventArgs e)
    {
        // reset timer, chatbox just to be safe
        TimeManager.ResetTimer();
        ChatManager.GetChatBox();

        if (Globals.Config.ChatMessageAlerts)
        {
            ChatManager.DisplayCurrentConfigMessage();
        }
    }

    private static void ReloadObjectData(object sender, AssetReadyEventArgs e)
    {
        if (!AssetManager.AssetIsSupported(e.Name.BaseName))
            return;

        Log.Info($"Reloading data for {e.Name.BaseName}.");
        AssetManager.ReloadObjectData(e.Name.BaseName);
    }

    /// <summary>
    ///     Tries to get and utilize all APIs.
    /// </summary>
    private static void InitializeGmcmMenu(object sender, GameLaunchedEventArgs e)
    {
        if (Globals.InitializeGmcmApi())
        {
            GenericModConfigMenuHelper.BuildConfigMenu();
        }
        else
        {
            Log.Info("Failed to fetch GMCM API, skipping config menu setup.");
        }
    }
}

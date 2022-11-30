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
using StardewModdingAPI.Events;
using StardewValley;

namespace ConfigurableSpecialOrdersUnlock;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        
        Globals.EventHelper.Content.AssetRequested += AssetManager.LoadOrEditAssets;
        Globals.EventHelper.GameLoop.GameLaunched += InitializeGmcmMenu;
        Globals.EventHelper.GameLoop.SaveLoaded += InvalidateCache;
        Globals.EventHelper.GameLoop.DayStarted += AddCutsceneToSeenIfSkipped;
    }

    private static void AddCutsceneToSeenIfSkipped(object sender, DayStartedEventArgs e)
    {
        if (Game1.stats.DaysPlayed < Globals.Config.GetUnlockDaysPlayed())
            return;

        if (!Globals.Config.SkipCutscene || Game1.player.eventsSeen.Contains(15389722))
            return;
            
        Game1.player.eventsSeen.Add(15389722);
        Log.Info("Skipping Special Orders board installation cutscene");
    }

    /// <summary>
    ///     Tries to get and utilize GMCM API.
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

    /// <summary>
    /// Forces invalidation of Data/Events/Town.
    /// This prevents cached values from being used if the player has changed config options.
    /// </summary>
    public static void InvalidateCache(object sender, EventArgs e)
    {
        Globals.GameContent.InvalidateCache("Data/Events/Town");
    }
}

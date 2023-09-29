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
using static StardewValley.GameLocation;

namespace Umbrellas;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.SaveLoaded += AssetManager.InitializeAssets;
        Globals.EventHelper.Content.AssetRequested += AssetManager.LoadAssets;
        Globals.EventHelper.Content.AssetReady += AssetManager.UpdateAssets;
        //Globals.EventHelper.GameLoop.GameLaunched += InitializeGmcmMenu;
        Globals.EventHelper.GameLoop.DayStarted += CheckPerpetualRain;
        Globals.EventHelper.GameLoop.DayStarted += (_, _) => { AssetManager.ReloadData(); };
        Globals.EventHelper.Player.Warped += CheckUmbrellaNeeded;
    }

    internal static void CheckPerpetualRain(object sender, DayStartedEventArgs e)
    {
        if (ConsoleCommandManager.PerpetualRain)
        {
            foreach (LocationContext lc in Enum.GetValues<LocationContext>())
            {
                LocationWeather lw = Game1.netWorldState.Value.GetWeatherForLocation(lc);

                lw.weatherForTomorrow.Value = 1;
                lw.isRaining.Value = true;
                lw.isSnowing.Value = false;
                lw.isDebrisWeather.Value = false;
            }

            Game1.isRaining = true;
            Game1.isSnowing = false;
            Game1.isDebrisWeather = false;
        }
    }
    internal static void CheckUmbrellaNeeded(object sender, WarpedEventArgs e)
    {
        Globals.UmbrellaNeeded = ConsoleCommandManager.ForceUmbrellas || (Game1.currentLocation.IsOutdoors && Game1.isRaining);
    }

    /// <summary>
    /// Tries to get and utilize all APIs.
    /// </summary>
    internal static void InitializeGmcmMenu(object sender, GameLaunchedEventArgs e)
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

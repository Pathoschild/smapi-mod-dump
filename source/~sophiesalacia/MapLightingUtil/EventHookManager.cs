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
using StardewModdingAPI.Events;

namespace MapLightingUtil;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.GameLoop.GameLaunched += Init;
        Globals.EventHelper.GameLoop.SaveLoaded += InitAssets;
        Globals.EventHelper.Content.AssetRequested += LoadAssets;
        Globals.EventHelper.Content.AssetsInvalidated += RefreshAssets;
        Globals.EventHelper.Player.Warped += (_, _) => UpdateLights();
        Globals.EventHelper.GameLoop.TimeChanged += (_, _) => UpdateLights();
        Globals.EventHelper.GameLoop.ReturnedToTitle += CleanUpLightsForExit;
    }

    private static void InitAssets(object sender, SaveLoadedEventArgs e)
    {
        LightHandler.Init();
    }

    private static void Init(object sender, GameLaunchedEventArgs e)
    {
        Globals.LightsAssetName = Globals.GameContent.ParseAssetName(Globals.LightsAssetPath);
    }

    private static void CleanUpLightsForExit(object sender, ReturnedToTitleEventArgs e)
    {
        LightHandler.RemoveAllLights();
    }

    private static void UpdateLights()
    {
        LightHandler.RefreshLights();
    }

    private static void RefreshAssets(object sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Contains(Globals.LightsAssetName))
            LightHandler.RefreshAsset();
    }

    private static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.LightsAssetPath))
            e.LoadFrom(() => new Dictionary<string, LightData>(), AssetLoadPriority.Low);
    }
}

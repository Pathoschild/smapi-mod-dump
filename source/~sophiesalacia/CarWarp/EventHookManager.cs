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
using static SolidFoundations.Framework.Interfaces.Internal.IApi;

namespace CarWarp;

internal class EventHookManager
{
    /// <summary>
    /// Sets up initial event handler.
    /// </summary>
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += LoadAssets;
        Globals.EventHelper.GameLoop.GameLaunched += HookIntoApis;
    }

    private static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo(Globals.WarpLocationsContentPath))
        {
            e.LoadFrom(
                () => new Dictionary<string, WarpLocationModel>(),
                AssetLoadPriority.Medium
            );
        }
    }


    /// <summary>
    /// Tries to get and utilize all APIs.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>")]
    private static void HookIntoApis(object sender, GameLaunchedEventArgs e)
    {
        if (!Globals.InitializeSFApi())
        {
            Log.Error("Failed to fetch SolidFoundations API.");
            return;
        }

        if (!Globals.InitializeCPApi())
        {
            Log.Error("Failed to fetch ContentPatcher API.");
            return;
        }

        if (Globals.InitializeGMCMApi())
        {
            GenericModConfigMenuHelper.BuildConfigMenu();
        }
        else
        {
            Log.Info("Failed to fetch GMCM API, skipping config menu setup.");
        }

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "Configuration", () =>
            {
                return new[] {
                    Globals.Config.Configuration.ToLower() switch
                    {
                        "right" =>  "sophie\\CarWarp\\overlay-steering-wheel-right",
                        "left" =>   "sophie\\CarWarp\\overlay-steering-wheel-left",
                        "none" =>   "sophie\\CarWarp\\overlay-no-steering-wheel",
                        "empty" =>  "sophie\\CarWarp\\overlay-no-dashboard",
                        _ =>        "sophie\\CarWarp\\overlay-steering-wheel-right"
                    }
                };
            }
        );

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "SeasonalOverlay", () =>
            {
                return new[] { Globals.Config.SeasonalOverlay.ToString() };
            }
        );

        Globals.SolidFoundationsApi.BroadcastSpecialActionTriggered += OnBroadcastTriggered;
    }

    /// <summary>
    /// Intercepts the Broadcast message from skell's Car, triggering the warp dialogue.
    /// </summary>
    private static void OnBroadcastTriggered(object sender, BroadcastEventArgs e)
    {
        if (e.BuildingId is "skellady.SF.cars_Car")
        {
            // pass car to CarWarp and initiate activation
            new CarWarp(e.Building).Activate();
        }
    }
}

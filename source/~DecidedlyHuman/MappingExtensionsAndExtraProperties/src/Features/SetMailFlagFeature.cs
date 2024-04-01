/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Diagnostics;
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Functionality;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using xTile.ObjectModel;

namespace MappingExtensionsAndExtraProperties.Features;

public class SetMailFlagFeature : Feature
{
    public sealed override string FeatureId { get; init; }
    public sealed override Harmony HarmonyPatcher { get; init; }
    public sealed override bool AffectsCursorIcon { get; init; }
    public sealed override int CursorId { get; init; }
    private static TilePropertyHandler tileProperties;
    private static Logger logger;
    public sealed override bool Enabled
    {
        get => enabled;
        internal set => enabled = value;
    }
    private static bool enabled;

    public SetMailFlagFeature(Harmony harmony, string id, Logger logger, TilePropertyHandler tilePropertyHandler)
    {
        this.Enabled = false;
        this.HarmonyPatcher = harmony;
        this.FeatureId = id;
        SetMailFlagFeature.logger = logger;
        SetMailFlagFeature.tileProperties = tilePropertyHandler;
        this.AffectsCursorIcon = false;
        this.CursorId = 5;
    }

    public override void Enable()
    {
        try
        {
            this.HarmonyPatcher.Patch(
                AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
                postfix: new HarmonyMethod(typeof(SetMailFlagFeature),
                    nameof(SetMailFlagFeature.GameLocation_CheckAction_Postfix)));
        }
        catch (Exception e)
        {
            logger.Exception(e);
        }

        this.Enabled = true;
    }

    public override void Disable()
    {
        this.Enabled = false;
    }

    public override void RegisterCallbacks() {}

    public override bool ShouldChangeCursor(GameLocation location, int tileX, int tileY, out int cursorId)
    {
        cursorId = default;

        return false;
    }

    public static void GameLocation_CheckAction_Postfix(GameLocation __instance, Location tileLocation,
        xTile.Dimensions.Rectangle viewport, Farmer who)
    {
        if (!enabled)
            return;

#if DEBUG
        Stopwatch timer = new Stopwatch();
        timer.Start();
#endif
        // Consider removing this try/catch.
        try
        {
            // First, pull our tile co-ordinates from the location.
            int tileX = tileLocation.X;
            int tileY = tileLocation.Y;

            // Check for the DHSetMailFlag property on a given tile.
            if (tileProperties.TryGetBuildingProperty(tileX, tileY, __instance, SetMailFlag.PropertyKey,
                    out PropertyValue dhSetMailFlagProperty))
            {
                MailFlag.SetMailFlag(dhSetMailFlagProperty, logger);
            }
        }
        catch (Exception e)
        {
            logger.Error("Caught exception handling GameLocation.checkAction in a postfix. Details follow:");
            logger.Exception(e);
        }
#if DEBUG
        timer.Stop();

        logger.Log($"Took {timer.ElapsedMilliseconds} to process in CheckAction patch.", LogLevel.Info);
#endif
    }
}

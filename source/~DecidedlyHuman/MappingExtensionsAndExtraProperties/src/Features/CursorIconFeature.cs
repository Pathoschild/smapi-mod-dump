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
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.ObjectModel;

namespace MappingExtensionsAndExtraProperties.Features;

public class CursorIconFeature : Feature
{
    public override string FeatureId { get; init; }
    public override Harmony HarmonyPatcher { get; init; }
    public sealed override bool AffectsCursorIcon { get; init; }
    public sealed override int CursorId { get; init; }
    private static string[] tilePropertiesControlled;
    public sealed override bool Enabled
    {
        get => enabled;
        internal set => enabled = value;
    }
    private static bool enabled;
    private static TilePropertyHandler tileProperties;
    private static Logger logger;

    public CursorIconFeature(Harmony harmony, string id, Logger logger, TilePropertyHandler tilePropertyHandler)
    {
        this.Enabled = false;
        this.HarmonyPatcher = harmony;
        this.FeatureId = id;
        CursorIconFeature.logger = logger;
        CursorIconFeature.tileProperties = tilePropertyHandler;
        this.AffectsCursorIcon = false;
        this.CursorId = default;
    }

    public override void Enable()
    {
        try
        {
            this.HarmonyPatcher.Patch(
                AccessTools.Method(typeof(Game1), nameof(Game1.drawMouseCursor)),
                prefix: new HarmonyMethod(typeof(CursorIconFeature), nameof(CursorIconFeature.Game1_drawMouseCursor_Prefix)));
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

    public static void Game1_drawMouseCursor_Prefix(Game1 __instance)
    {
        int tileX = (int)Game1.currentCursorTile.X;
        int tileY = (int)Game1.currentCursorTile.Y;

        if (FeatureManager.TryGetCursorIdForTile(Game1.currentLocation, tileX, tileY, out int id))
        {
            Game1.mouseCursor = id;
        }
    }
}

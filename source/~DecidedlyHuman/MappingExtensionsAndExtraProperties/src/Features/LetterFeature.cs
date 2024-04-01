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
using MappingExtensionsAndExtraProperties.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using xTile.ObjectModel;

namespace MappingExtensionsAndExtraProperties.Features;

public class LetterFeature : Feature
{
    public override Harmony HarmonyPatcher { get; init; }
    public sealed override bool AffectsCursorIcon { get; init; }
    public sealed override int CursorId { get; init; }
    private string[] tilePropertiesControlled = [
        "MEEP_Letter"];
    public sealed override bool Enabled
    {
        get => enabled;
        internal set => enabled = value;
    }
    private static bool enabled;

    public override string FeatureId { get; init; }
    private static TilePropertyHandler tileProperties;
    private static Logger logger;

    public LetterFeature(Harmony harmony, string id, Logger logger, TilePropertyHandler tilePropertyHandler)
    {
        this.Enabled = false;
        this.HarmonyPatcher = harmony;
        this.FeatureId = id;
        LetterFeature.logger = logger;
        LetterFeature.tileProperties = tilePropertyHandler;
        this.AffectsCursorIcon = true;
        this.CursorId = 5;

        GameLocation.RegisterTileAction("MEEP_Letter", this.DoLetter);
    }

    private bool DoLetter(GameLocation location, string[] args, Farmer player, Point tile)
    {
        if (!enabled)
            return false;

        // Consider removing this try/catch.
        try
        {
            int tileX = tile.X;
            int tileY = tile.Y;
            string combinedArgs = args.Join(delimiter: " ");

            Letter.DoLetter(location, combinedArgs, tileX, tileY, logger);

            return true;
        }
        catch (Exception e)
        {
            logger.Error("Caught exception handling MEEP_Letter Action property. Details follow:");
            logger.Exception(e);
        }

        return false;
    }

    public override void Enable()
    {
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

        if (!enabled)
            return false;

        for (int i = 0; i < this.tilePropertiesControlled.Length; i++)
        {
            if (tileProperties.TryGetBackProperty(tileX, tileY, Game1.currentLocation, this.tilePropertiesControlled[i],
                    out PropertyValue _))
            {
                cursorId = this.CursorId;
                return true;
            }
        }

        return false;
    }
}

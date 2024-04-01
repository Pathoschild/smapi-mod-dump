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
using System.Collections.Generic;
using System.Diagnostics;
using DecidedlyShared.Logging;
using DecidedlyShared.Ui;
using DecidedlyShared.Utilities;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Functionality;
using MappingExtensionsAndExtraProperties.Models.EventArgs;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using MappingExtensionsAndExtraProperties.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using xTile.ObjectModel;

namespace MappingExtensionsAndExtraProperties.Features;

public class CloseupInteractionFeature : Feature
{
    public sealed override Harmony HarmonyPatcher { get; init; }
    public sealed override bool AffectsCursorIcon { get; init; }
    public sealed override int CursorId { get; init; }
    private string[] tilePropertiesControlled = [
        "MEEP_CloseupInteraction_Image",
        "MEEP_CloseupInteraction_Image_1",
        "MEEP_CloseupInteraction_Text",
        "MEEP_CloseupInteraction_Sound"];

    public sealed override bool Enabled
    {
        get => enabled;
        internal set => enabled = value;
    }
    private static bool enabled;

    public sealed override string FeatureId { get; init; }
    private static TilePropertyHandler tileProperties;
    private static Properties propertyUtils;
    private static Logger logger;

    public CloseupInteractionFeature(Harmony harmony, string id, Logger logger, TilePropertyHandler tilePropertyHandler, Properties propertyUtils)
    {
        this.Enabled = false;
        this.HarmonyPatcher = harmony;
        this.FeatureId = id;
        CloseupInteractionFeature.logger = logger;
        CloseupInteractionFeature.tileProperties = tilePropertyHandler;
        CloseupInteractionFeature.propertyUtils = propertyUtils;
        this.AffectsCursorIcon = true;
        this.CursorId = 5;

        GameLocation.RegisterTileAction("MEEP_CloseupInteraction_Image", this.DoCloseupInteraction);
        GameLocation.RegisterTileAction("MEEP_CloseupInteractionReel", this.DoCloseupReel);
    }

    private bool DoCloseupReel(GameLocation location, string[] propertyArgs, Farmer player, Point tile)
    {
        if (!enabled)
            return false;

        if (propertyUtils.TryGetInteractionReel(tile.X, tile.Y, location,
                CloseupInteractionImage.PropertyKey,
                out List<MenuPage> pages))
        {
            string cueName = "bigSelect";

            // Now we check for a sound interaction property.
            if (tileProperties.TryGetBackProperty(tile.X, tile.Y, location, CloseupInteractionSound.PropertyKey,
                    out PropertyValue closeupSoundProperty))
            {
                if (Parsers.TryParse(closeupSoundProperty.ToString(),
                        out CloseupInteractionSound parsedSoundProperty))
                {
                    cueName = parsedSoundProperty.CueName;
                }
            }

            CloseupInteraction.DoCloseupReel(pages, logger, cueName);

            return true;
        }

        return false;
    }

    private bool DoCloseupInteraction(GameLocation location, string[] propertyArgs, Farmer player, Point tile)
    {
        if (!enabled)
            return false;

        string joinedArgs = propertyArgs.Join(delimiter: " ");
        CloseupInteraction.DoCloseupInteraction(location, tile.X, tile.Y, joinedArgs, logger);

        return true;
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

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
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace FestivalWarpProperty;

// TODO: This needs heavy, heavy modularisation and tidiness passes. This is quick initial release territory.
public class Patches
{
    private static Logger logger;

    public static void InitialiseLogger(Logger logger)
    {
        Patches.logger = logger;
    }

    public static void GameLocation_CheckAction_Postfix(GameLocation __instance, Location tileLocation,
        xTile.Dimensions.Rectangle viewport, Farmer who)
    {
        try
        {
            int tileX = tileLocation.X;
            int tileY = tileLocation.Y;
            Tile backLayerTile = __instance.Map.GetLayer("Back")
                .PickTile(new Location(tileX * 64, tileY * 64), viewport.Size);
            Tile buildingsLayerTile = __instance.Map.GetLayer("Buildings")
                .PickTile(new Location(tileX * 64, tileY * 64), viewport.Size);


            if (backLayerTile != null && backLayerTile
                    .Properties.TryGetValue("FestivalWarpMagicThing", out PropertyValue backLayerValue))
            {
                DoWarp(backLayerValue, who);
            }
            else if (buildingsLayerTile != null && buildingsLayerTile
                         .Properties.TryGetValue("FestivalWarpMagicThing", out PropertyValue buildingsLayerValue))
            {
                DoWarp(buildingsLayerValue, who);
            }
        }
        catch (Exception e)
        {
            logger.Log($"Failed in {nameof(GameLocation_CheckAction_Postfix)}.");
            logger.Exception(e);
        }
    }

    private static void DoWarp(PropertyValue value, Farmer farmer)
    {
        if (!TryParseCoords(value, out (int x, int y) coords))
        {
            // If the parsing failed, we log our error, and return.
            return;
        }

        // The coordinates were parsed successfully, so we do our warp.
        Game1.playSound("doorCreak");
        Game1.globalFadeToBlack(() => { farmer.Position = new Vector2(coords.x * 64, coords.y * 64); });
    }

    private static bool TryParseCoords(PropertyValue value, out (int x, int y) coords)
    {
        coords.x = 0;
        coords.y = 0;

        // First, we split our tile property with our separator, space.
        string[] splitProperty = value.ToString().Split(" ");

        // Since we're expecting only an X and Y coordinate, any length other than 2 is invalid.
        if (splitProperty.Length != 2)
            return false;
        else
        {
            // We have the correct number of parameters, so we parse them to int.
            if (!int.TryParse(splitProperty[0], out int x))
                return false;

            if (!int.TryParse(splitProperty[1], out int y))
                return false;

            // Neither of them returned false, so we assign our coords tuple's values, and return true.
            coords.x = x;
            coords.y = y;

            return true;
        }
    }

    // private static void AfterFade()
    // {
    //     who.Position = new Vector2(xCoord * 64, yCoord * 64);
    // }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework;
using AchtuurCore.Utility;
using HarmonyLib;
using JunimoBeacon.Patches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Characters;
using System.Collections.Generic;
using System.Linq;

namespace JunimoBeacon;
internal class PlaceBeaconOverlay : Overlay
{
    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {

        // Draw last known crop coord
        Debug.DebugOnlyExecute(() =>
        {
            DrawLastKnownCropPoint(spriteBatch);
            DrawJunimoHarvesters(spriteBatch);
        });


        if (Game1.player.ActiveObject is null
            || Game1.player.ActiveObject.ParentSheetIndex != JunimoBeacon.ID
            || !TypeChecker.isType<StardewValley.Farm>(Game1.player.currentLocation))
            return;

        Vector2 size = Vector2.One * tileSize;

        /// Draw tiles around held beacon
        Vector2 cursorTile = Game1.currentCursorTile;
        bool beaconWouldBeInRange = ModEntry.Instance.IsInRangeOfAnyGroup(cursorTile);

        Color beacon_color = (beaconWouldBeInRange)
            ? Color.Lime
            : ColorHelper.MultiplyColor(Color.Green, 0.7f);

        IEnumerable<Vector2> tiles = Tiles.GetVisibleTiles(expand: 1)
            .Intersect(JunimoBeacon.GetBeaconRangeTiles(cursorTile, expand: 0));

        DrawTiles(spriteBatch, tiles, beacon_color);


        /// Draw tiles around groups range
        foreach (JunimoGroup group in ModEntry.Instance.JunimoGroups)
        {
            Color hut_range_color = (group.IsInRange(cursorTile))
                ? group.Color
                : ColorHelper.MultiplyColor(group.Color, 0.85f);


            IEnumerable<Vector2> groupTiles = group.GetTiles().Intersect(Tiles.GetVisibleTiles());
            DrawTiles(spriteBatch, groupTiles, hut_range_color);
        }

        /// Draw beacons that are not in range of anything
        IEnumerable<JunimoBeacon> groupBeacons = ModEntry.Instance.JunimoGroups.SelectMany(group => group.Beacons);

        // get tiles that are in range of beacons that are not in range of a hut
        IEnumerable<Vector2> out_of_range_beacon_tiles = JunimoBeacon.GetBeaconsOnFarm()
            .Where(b => !groupBeacons.Contains(b))
            .SelectMany(b => b.GetTilesInRange());

        Color out_of_range_color = Color.ForestGreen * 0.75f;
        DrawTiles(spriteBatch, out_of_range_beacon_tiles, out_of_range_color);
    }


    private static void DrawLastKnownCropPoint(SpriteBatch spriteBatch)
    {
        DrawTile(spriteBatch, MatureCropsWithinRadiusPatcher.lastKnownCropLocationTile, Color.OrangeRed);
    }

    private static void DrawJunimoHarvesters(SpriteBatch spriteBatch)
    {
        foreach (JunimoHarvester junimo in MatureCropsWithinRadiusPatcher.JunimoHarvesters)
        {
            NetColor color = (NetColor)AccessTools.Field(typeof(JunimoHarvester), "color").GetValue(junimo);

            if (junimo.controller is not null)
            {
                DrawPathfinding(spriteBatch, junimo.controller, color.Value);
            }
        }
    }

    protected static void DrawPathfinding(SpriteBatch spriteBatch, PathFindController pathfindcontroller, Color? color = null, Color? pathColor = null)
    {
        IEnumerable<Point> pathPoints = pathfindcontroller.pathToEndPoint.AsEnumerable();

        if (pathPoints is null || pathPoints.Count() == 0)
            return;

        // Draw start point
        DrawPoint(spriteBatch, pathPoints.First(), color, null);
        // Draw end point
        DrawPoint(spriteBatch, pathPoints.Last(), color, null);

        // Loop through middle part and draw path
        pathColor ??= color * 0.75f;
        foreach (Point point in pathPoints.Skip(1).SkipLast(1))
        {
            DrawPoint(spriteBatch, point, color: pathColor, tileSizePercentage: 0.5f);
        }
    }
}

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
    public static Texture2D GreenPlacementTile { get; set; }
    public static Texture2D PlacementTile { get; set; }
    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {

        // Draw last known crop coord
        Debug.DebugOnlyExecute(() => {
            DrawLastKnownCropPoint(spriteBatch);
            DrawJunimoHarvesters(spriteBatch);
        });


        if (Game1.player.ActiveObject is null || Game1.player.ActiveObject.ParentSheetIndex != JunimoBeacon.ID)
            return;

        // Draw tiles around held beacon
        Vector2 cursorTile = Game1.currentCursorTile;
        bool beaconWouldBeInRange = ModEntry.Instance.IsInRangeOfAnyGroup(cursorTile);

        Color beacon_color = (beaconWouldBeInRange)
            ? Color.White
            : Color.Yellow * 0.7f;

        Color hut_range_color = (beaconWouldBeInRange)
            ? Color.ForestGreen
            : Color.DarkGreen * 0.85f;

        beacon_color.A = 255;
        hut_range_color.A = 255;

        IEnumerable<Vector2> tiles = Tiles.GetVisibleTiles(expand: 1)
            .Intersect(JunimoBeacon.GetBeaconRangeTiles(cursorTile, expand: 0));

        foreach (Vector2 tile in tiles)
        {
            Vector2 screenCoords = Tiles.GetTileScreenCoords(tile);
            spriteBatch.DrawTexture(GreenPlacementTile, screenCoords, new Vector2(tileSize, tileSize), beacon_color);
        }


        // Draw tiles around junimo huts
        IEnumerable<Vector2> HutTiles = ModEntry.Instance.JunimoGroups
            .SelectMany(group => group.GetTiles())
            .Intersect(Tiles.GetVisibleTiles());

        foreach (Vector2 tile in HutTiles)
        {
            Vector2 screenCoords = Tiles.GetTileScreenCoords(tile);
            Vector2 size = Vector2.One * tileSize;
            spriteBatch.DrawTexture(GreenPlacementTile, screenCoords, size, hut_range_color);
        }
    }

    private static void DrawLastKnownCropPoint(SpriteBatch spriteBatch)
    {
        Vector2 lastKnownCropCoords = Tiles.GetTileScreenCoords(MatureCropsWithinRadiusPatcher.lastKnownCropLocationTile);
        Vector2 size = Vector2.One * Game1.tileSize;
        spriteBatch.DrawTexture(PlacementTile, lastKnownCropCoords, size, Color.OrangeRed);
    }

    private static void DrawJunimoHarvesters(SpriteBatch spriteBatch)
    {
        Vector2 size = Vector2.One * Game1.tileSize;
        foreach(JunimoHarvester junimo in MatureCropsWithinRadiusPatcher.JunimoHarvesters)
        {
            NetColor color = (NetColor) AccessTools.Field(typeof(JunimoHarvester), "color").GetValue(junimo);

            Vector2 location = junimo.getTileLocation();
            Vector2 locationCoords = Tiles.GetTileScreenCoords(location);
            spriteBatch.DrawTexture(PlacementTile, locationCoords, size, color.Value);

            if (junimo.controller is not null)
            {
                Vector2 target = junimo.controller.endPoint.ToVector2();
                Vector2 targetCoords = Tiles.GetTileScreenCoords(target);
                spriteBatch.DrawTexture(PlacementTile, targetCoords, size, color.Value);
            }

        }
    }

    public static void LoadPlacementTileTexture()
    {
        // Full asset is five 64x64 pixel tiles in a row, we only want the leftmost one of these tiles
        Texture2D fullAsset = ModEntry.Instance.Helper.GameContent.Load<Texture2D>("LooseSprites/buildingPlacementTiles");

        // Get color data of entire asset
        Color[] fullAssetColors = new Color[fullAsset.Width * fullAsset.Height];
        fullAsset.GetData<Color>(fullAssetColors);

        // Copy only leftmost tile to smaller array
        Color[] sliceAssetColors = new Color[64 * fullAsset.Height];
        Color[] grayScaleAssetColors = new Color[64 * fullAsset.Height];

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                sliceAssetColors[x + y * 64] = fullAssetColors[x + y * fullAsset.Width];
                grayScaleAssetColors[x + y * 64] = fullAssetColors[x + y * fullAsset.Width].ToGrayScale();
            }
        }

        GreenPlacementTile = new Texture2D(Game1.graphics.GraphicsDevice, 64, 64);
        GreenPlacementTile.SetData<Color>(sliceAssetColors);

        PlacementTile = new Texture2D(Game1.graphics.GraphicsDevice, 64, 64);
        PlacementTile.SetData<Color>(grayScaleAssetColors);
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerExpShare;

internal class TileShareRangeOverlay : AchtuurCore.Framework.Overlay
{
    /// <summary>
    /// Color used to indicate sharing range. Blueish green color
    /// </summary>
    private Color RangeColor = new Color(0f, 0.9f, 0.4f);

    /// <summary>
    /// Color used to color a tile an in-exp-sharing range farmer is on. Purplish color
    /// </summary>
    private Color FarmerInRangeColor = new Color(0.7f, 0.1f, 0.95f);

    public TileShareRangeOverlay()
    {
        this.Enabled = false;
    }

    /// <inheritdoc/>
    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {
        float color_fac = ModEntry.Instance.Config.OverlayOpacity;
        Vector2 currentTile = Game1.player.Tile;

        IEnumerable<Farmer> nearbyFarmers = ModEntry.GetNearbyPlayers();

        List<Vector2> nearbyFarmerTile = nearbyFarmers.Select(f => f.Tile).ToList();

        // In tile mode, draw range of tiles
        if (ModEntry.Instance.Config.ExpShareType == ExpShareType.Tile)
        {
            int radius = ModEntry.Instance.Config.NearbyPlayerTileRange;
            // only get tiles at radius = radius
            List<Vector2> ExpShareRadius = Game1.player.GetTilesInRadius(radius, min_radius: radius).ToList();

            // Loop through ring at radius = config.NearbyPlayerTileRange
            // Only draw rows from left to right
            foreach (Vector2 tile in ExpShareRadius)
            {
                // Get tile color (blueish green)
                Color color = RangeColor * color_fac;
                Vector2 screenCoord = Tiles.GetTileScreenCoords(tile);

                // Draw only once per row, so tiles on the right side of the row's perimeter should only have their border drawn
                if (tile.X > currentTile.X || ExpShareRadius.Contains(new Vector2(tile.X - 1, tile.Y)))
                {
                    DrawEdgeBordersToOutside(spriteBatch, tile, color * (1f / color_fac), ExpShareRadius, currentTile);
                    continue;
                }

                // Mirror X location of tile to the opposite side of player
                float inv_tileX = currentTile.X + Math.Abs(currentTile.X - tile.X);
                float row_length = Math.Abs(inv_tileX - tile.X + 1);


                // Draw tile and border
                spriteBatch.DrawRect(screenCoord.X, screenCoord.Y, new Vector2(tileSize * row_length, tileSize), color);
                //spriteBatch.DrawLine(screenCoord.X, screenCoord.Y, new Vector2(tileSize, tileSize), color);

                DrawEdgeBordersToOutside(spriteBatch, tile, color * (1f / color_fac), ExpShareRadius, currentTile);
            }
        }
        // Map/Global mode, draw black overlay over map to indicate that something is happening
        else
        {
            Rectangle visibleArea = Tiles.GetVisibleArea(expand: 1);
            Color color = RangeColor * color_fac;
            float screenX = visibleArea.X * Game1.tileSize - Game1.viewport.X;
            float screenY = visibleArea.Y * Game1.tileSize - Game1.viewport.Y;

            spriteBatch.DrawRect(screenX, screenY, tileSize * new Vector2(visibleArea.Width, visibleArea.Height), color);
        }


        // Draw tiles that nearby farmers are on last, so that they go on top of previously drawn stuff
        foreach (Vector2 tile in nearbyFarmerTile)
        {
            float screenX = tile.X * Game1.tileSize - Game1.viewport.X;
            float screenY = tile.Y * Game1.tileSize - Game1.viewport.Y;
            Color color = FarmerInRangeColor * color_fac;
            spriteBatch.DrawRect(screenX + tileGap, screenY + tileGap, new Vector2(tileSize - tileGap * 2, tileSize - tileGap * 2), color);
            DrawEdgeBorders(spriteBatch, tile, color * (1f / color_fac));
        }
    }
}

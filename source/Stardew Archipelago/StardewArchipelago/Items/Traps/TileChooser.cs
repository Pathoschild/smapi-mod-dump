/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;

namespace StardewArchipelago.Items.Traps
{
    public class TileChooser
    {
        private const int MAX_RETRIES = 20;

        public Vector2 GetRandomTileInbounds(GameLocation area)
        {
            var tile = area.getRandomTile();
            var tilePoint = Utility.Vector2ToPoint(tile);
            var tileLocation = new Location(tilePoint.X, tilePoint.Y);
            while (area.isTileOccupied(tile) || area.isWaterTile(tilePoint.X, tilePoint.Y) ||
                   !area.isTileLocationTotallyClearAndPlaceable(tile) ||
                   !area.isTileLocationOpenIgnoreFrontLayers(tileLocation))
            {
                tile = area.getRandomTile();
                tilePoint = Utility.Vector2ToPoint(tile);
                tileLocation = new Location(tilePoint.X, tilePoint.Y);
            }

            return tile;
        }
        public Vector2 GetRandomTileInboundsOffScreen(GameLocation area)
        {
            var numberRetries = MAX_RETRIES;
            var spawnPosition = GetRandomTileInbounds(area);
            while (numberRetries > 0 && Utility.isOnScreen(Utility.Vector2ToPoint(spawnPosition), 64, area))
            {
                numberRetries--;
                spawnPosition = GetRandomTileInbounds(area);
            }
            return spawnPosition;
        }
    }
}

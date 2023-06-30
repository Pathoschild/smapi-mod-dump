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
using StardewArchipelago.Extensions;
using StardewArchipelago.Items.Traps;
using StardewValley;

namespace StardewArchipelago.GameModifications
{
    public class PlayerUnstucker
    {
        private TileChooser _tileChooser;

        public PlayerUnstucker(TileChooser tileChooser)
        {
            _tileChooser = tileChooser;
        }

        public bool Unstuck()
        {
            var player = Game1.player;
            var map = player.currentLocation;
            var tiles = new List<Point>();
            for (var x = 0; x < map.Map.Layers[0].LayerWidth; x++)
            {
                for (var y = 0; y < map.Map.Layers[0].LayerHeight; y++)
                {
                    tiles.Add(new Point(x, y));
                }
            }

            tiles = tiles.OrderBy(x => x.GetTotalDistance(player.getTileLocationPoint())).ToList();

            foreach (var tile in tiles)
            {
                if (_tileChooser.CanPathFindToAnyWarp(map, tile))
                {
                    player.setTileLocation(new Vector2(tile.X, tile.Y));
                    return true;
                }
            }

            return false;
        }
    }
}

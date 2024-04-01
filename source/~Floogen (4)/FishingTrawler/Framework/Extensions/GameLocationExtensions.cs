/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using StardewValley.Objects;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;
using StardewValley.Extensions;

namespace FishingTrawler.Framework.Extensions
{
    public static class GameLocationExtensions
    {
        public static bool isTileOccupiedForPlacement(this GameLocation location, Vector2 tileLocation, Object toPlace = null)
        {
            return location.CanItemBePlacedHere(tileLocation, toPlace != null && toPlace.isPassable());
        }

        public static bool isTileOccupied(this GameLocation location, Vector2 tileLocation, string characterToIgnore = "", bool ignoreAllCharacters = false)
        {
            CollisionMask mask = ignoreAllCharacters ? CollisionMask.All & ~CollisionMask.Characters & ~CollisionMask.Farmers : CollisionMask.All;
            return location.IsTileOccupiedBy(tileLocation, mask);
        }

        public static bool isTileOccupiedIgnoreFloors(this GameLocation location, Vector2 tileLocation, string characterToIgnore = "")
        {
            return location.IsTileOccupiedBy(tileLocation, CollisionMask.Buildings | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.Characters | CollisionMask.TerrainFeatures, ignorePassables: CollisionMask.Flooring);
        }

        public static bool isTileLocationOpenIgnoreFrontLayers(this GameLocation location, Location tile)
        {
            return location.map.RequireLayer("Buildings").Tiles[tile.X, tile.Y] == null && !location.isWaterTile(tile.X, tile.Y);
        }

        public static bool isTileLocationTotallyClearAndPlaceable(this GameLocation location, int x, int y)
        {
            return location.isTileLocationTotallyClearAndPlaceable(new Vector2(x, y));
        }

        public static bool isTileLocationTotallyClearAndPlaceableIgnoreFloors(this GameLocation location, Vector2 v)
        {
            return location.isTileOnMap(v) && !location.isTileOccupiedIgnoreFloors(v) && location.isTilePassable(new Location((int)v.X, (int)v.Y), Game1.viewport) && location.isTilePlaceable(v);
        }

        public static bool isTileLocationTotallyClearAndPlaceable(this GameLocation location, Vector2 v)
        {
            Vector2 pixel = new Vector2((v.X * Game1.tileSize) + Game1.tileSize / 2, (v.Y * Game1.tileSize) + Game1.tileSize / 2);
            foreach (Furniture f in location.furniture)
            {
                if (f.furniture_type != Furniture.rug && !f.isPassable() && f.GetBoundingBox().Contains((int)pixel.X, (int)pixel.Y) && !f.AllowPlacementOnThisTile((int)v.X, (int)v.Y))
                    return false;
            }

            return location.isTileOnMap(v) && !location.isTileOccupied(v) && location.isTilePassable(new Location((int)v.X, (int)v.Y), Game1.viewport) && location.isTilePlaceable(v);
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace MouseMoveMode
{
    class Util
    {
        public static bool debugPassable { get; private set; } = false;
        private static bool debugPassableVebose = false;
        private static HashSet<Vector2> cacheCantPassable = new HashSet<Vector2>();
        private static List<DrawableNode> nonPassableNodes = new List<DrawableNode>();

        public static void flushCache()
        {
            if (cacheCantPassable.Count != 0)
            {
                cacheCantPassable.Clear();
            }
            if (Util.debugPassable)
            {
                if (nonPassableNodes.Count != 0)
                {
                    nonPassableNodes.Clear();
                }
            }
        }

        public static void drawPassable(SpriteBatch b)
        {
            if (!Util.debugPassable)
                return;
            foreach (var node in nonPassableNodes)
            {
                node.draw(b, color: Color.Red);
            }
        }

        /**
         * @brief This use tile (a scale down 1/64 from game true position)
         *
         * @param x tile value in X-axis
         * @param y tile value in Y-axis
         * @return if the current tile is passable
         */
        public static bool isTilePassable(float x, float y, bool useBetter = true)
        {
            return _isTilePassable(new Vector2(x, y), useBetter);
        }

        /**
         * @brief This use tile (a scale down 1/64 from game true position)
         *
         * @param tile value
         * @param useBetter (Optional) use new different way to check isTilePassable
         * @return if the current tile is passable
         */
        public static bool isTilePassable(Vector2 tile, bool useBetter = true)
        {
            return _isTilePassable(tile, useBetter);
        }

        /**
         * @brief Function routing
         */
        private static bool _isTilePassable(Vector2 tile, bool useBetter)
        {
            if (useBetter)
                return _isTilePassableOld(tile);
            else
                return _isTilePassableNew(tile);
        }

        /**
         * @brief VonLoewe implementation for isTilePassable, fixeds as some time building tiles is passable
         * Now actually there is way more passable tile here tbh
         */
        private static bool _isTilePassableNew(Vector2 tile)
        {
            var l = Game1.player.currentLocation;
            var building = l.getBuildingAt(tile);
            if (building is not null)
            {
                if (!building.isTilePassable(tile))
                {
                    if (Util.debugPassable)
                        nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
                    if (Util.debugPassableVebose)
                        ModEntry.getMonitor().Log("Found unpassable building " + building + " at tile " + tile, LogLevel.Info);
                    return false;
                }
            }

            const CollisionMask collisionMask = CollisionMask.Furniture | CollisionMask.Objects |
                                    CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
            if (l.isTilePassable(tile) && !l.IsTileOccupiedBy(tile, collisionMask))
                return true;
            nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
            return false;
        }

        /**
         * @brief My old implementation
         */
        private static bool _isTilePassableOld(Vector2 tile)
        {
            if (cacheCantPassable.Contains(tile))
            {
                return false;
            }

            GameLocation gl = Game1.player.currentLocation;
            if (!gl.isTilePassable(tile))
            {
                if (Util.debugPassableVebose)
                    ModEntry.getMonitor().Log("Found unpassable tile from current location at " + tile, LogLevel.Info);
                cacheCantPassable.Add(tile);
                nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
                return false;
            }

            var building = gl.getBuildingAt(tile);
            if (building is not null)
            {
                if (!building.isTilePassable(tile))
                {
                    cacheCantPassable.Add(tile);
                    if (Util.debugPassable)
                        nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
                    if (Util.debugPassableVebose)
                        ModEntry.getMonitor().Log("Found unpassable building " + building + " at tile " + tile, LogLevel.Info);
                    return false;
                }
            }

            foreach (var items in gl.terrainFeatures)
            {
                if (!items.ContainsKey(tile))
                {
                    continue;
                }
                if (items[tile].isPassable())
                {
                    continue;
                }
                //Tree can be cutdown, thus caching should not be consider, unless we flush every path-find
                if (Util.debugPassableVebose)
                    ModEntry.getMonitor().Log("Found unpassable terrain feature " + items[tile], LogLevel.Info);
                cacheCantPassable.Add(tile);
                if (Util.debugPassable)
                    nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
                return false;
            }

            var largerTerrainFeature = gl.getLargeTerrainFeatureAt((int)tile.X, (int)tile.Y);
            if (largerTerrainFeature is not null)
            {
                if (!largerTerrainFeature.isPassable())
                {
                    //this.Monitor.Log("Found unpassable large terrain feature " + item + " at " + tile, LogLevel.Info);
                    cacheCantPassable.Add(tile);
                    if (Util.debugPassable)
                        nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
                    return false;
                }
            }

            if (gl.Objects.ContainsKey(tile))
            {
                var item = gl.Objects[tile];
                if (!item.isPassable())
                {
                    // Object like stone etc should also consider breakable, thus should not be cache
                    if (Util.debugPassableVebose)
                        ModEntry.getMonitor().Log("Found unpassable object" + item, LogLevel.Info);
                    cacheCantPassable.Add(tile);
                    if (Util.debugPassable)
                        nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
                    return false;
                }
            }

            var funiture = gl.GetFurnitureAt(tile);
            if (funiture is not null)
            {
                // Some how bed is not passable, player can't go into bed using
                // path finding
                if (funiture is BedFurniture)
                {
                    var bed = (BedFurniture)funiture;
                    if (bed.bedType != BedFurniture.BedType.Child)
                    {
                        var bedSpot = bed.TileLocation;
                        if (tile.X <= bedSpot.X + bed.getTilesWide() & tile.X >= bedSpot.X & tile.Y > bedSpot.Y & tile.Y < bedSpot.Y + 2)
                        {
                            if (debugPassableVebose)
                                ModEntry.getMonitor().Log("Found unpassable furniture" + funiture, LogLevel.Info);
                            return true;
                        }
                    }
                }
                if (!funiture.isPassable())
                {
                    // Object like stone etc should also consider breakable, thus should not be cache
                    if (Util.debugPassableVebose)
                        ModEntry.getMonitor().Log("Found unpassable furniture" + funiture, LogLevel.Info);
                    cacheCantPassable.Add(tile);
                    if (Util.debugPassable)
                        nonPassableNodes.Add(new DrawableNode(Util.toBoxPosition(tile)));
                    return false;
                }
            }

            return true;
        }

        /**
         * @brief Convert a tile position (128x128) to true game position, which is a multiple of 64 compare to original value
         *
         * @param tile position vector
         * @param paddingX (Optional) default to be the middle of the tile
         * @param paddingY (Optional) default to be the middle of the tile
         * @return true positional vector
         */
        public static Vector2 toPosition(Vector2 tile, float paddingX = 32f, float paddingY = 32f)
        {
            return new Vector2(tile.X * 64 + paddingX, tile.Y * 64 + paddingY);
        }

        /**
         * @brief Convert a tile position (128x128) to true game position, which is a multiple of 64 compare to original value
         *
         * @param tile position vector
         * @return true positional vector
         */
        public static Rectangle toBoxPosition(Vector2 tile)
        {
            return new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64);
        }

        /**
         * @brief Convert a in-game position to a 128x128 tile map, which is a downscale of 64 times compare to original value
         *
         * @param position in true value
         * @return down scale tile position
         */
        public static Vector2 toTile(Vector2 position)
        {
            return new Vector2((float)Math.Truncate(position.X / 64f), (float)Math.Truncate(position.Y / 64f));
        }

        /**
         * @brief Convert a float fragtion tile to int
         *
         * @param position in true value
         * @return down scale tile position
         */
        public static Vector2 fixFragtionTile(Vector2 tile)
        {
            // Some time player tiles does not match
            var microTileAlter = 0.00f;
            return new Vector2((float)Math.Truncate(tile.X), (float)Math.Truncate(tile.Y - microTileAlter));
        }

        /**
         * @brief Convert a in-game rectangle into a list of tile on 128x128 block map
         *
         * @param box usually a boundingBox or renderBox of in-game object
         * @return list of tile corresponed to the input
         */
        public static List<Vector2> toTiles(Rectangle box)
        {
            var res = new List<Vector2>();
            var x = (int)Math.Round(box.X / 64f);
            var y = (int)Math.Round(box.Y / 64f);
            var w = (int)Math.Round(box.Width / 64f);
            var h = (int)Math.Round(box.Height / 64f);
            for (int i = x; i < x + w; i++)
                for (int j = y; j < y + h; j++)
                {
                    res.Add(new Vector2(i, j));
                }
            return res;
        }
    }

    // It's from the game source, don't change this enum
    public enum FaceDirection
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }
}

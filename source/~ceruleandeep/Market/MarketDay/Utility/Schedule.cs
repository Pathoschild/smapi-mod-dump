/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MarketDay.Utility
{
    public static class Schedule
    {
        public static Stack<Point> findPathForNPCSchedules(
            Point startPoint,
            Point endPoint,
            GameLocation location,
            int limit)
        {
            PriorityQueue priorityQueue = new PriorityQueue();
            HashSet<int> intSet = new HashSet<int>();
            int num = 0;
            priorityQueue.Enqueue(new PathNode(startPoint.X, startPoint.Y, (byte) 0, (PathNode) null),
                Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
            PathNode pathNode1 = (PathNode) priorityQueue.Peek();
            int layerWidth = location.map.Layers[0].LayerWidth;
            int layerHeight = location.map.Layers[0].LayerHeight;
            while (!priorityQueue.IsEmpty())
            {
                PathNode pathNode2 = priorityQueue.Dequeue();
                if (pathNode2.x == endPoint.X && pathNode2.y == endPoint.Y)
                    return PathFindController.reconstructPath(pathNode2);
                intSet.Add(pathNode2.id);
                for (int index = 0; index < 4; ++index)
                {
                    int x = pathNode2.x + (int) Directions[index, 0];
                    int y = pathNode2.y + (int) Directions[index, 1];
                    int hash = PathNode.ComputeHash(x, y);
                    if (!intSet.Contains(hash))
                    {
                        PathNode p = new PathNode(x, y, pathNode2);
                        p.g = (byte) ((uint) pathNode2.g + 1U);
                        if (p.x == endPoint.X && p.y == endPoint.Y || p.x >= 0 && p.y >= 0 &&
                            (p.x < layerWidth && p.y < layerHeight) &&
                            !isPositionImpassableForNPCSchedule(location, p.x, p.y))
                        {
                            int priority = (int) p.g +
                                           getPreferenceValueForTerrainType(location, p.x, p.y) +
                                           (Math.Abs(endPoint.X - p.x) + Math.Abs(endPoint.Y - p.y) +
                                            (p.x == pathNode2.x && p.x == pathNode1.x ||
                                             p.y == pathNode2.y && p.y == pathNode1.y
                                                ? -2
                                                : 0));
                            if (!priorityQueue.Contains(p, priority))
                                priorityQueue.Enqueue(p, priority);
                        }
                    }
                }

                pathNode1 = pathNode2;
                ++num;
                if (num >= limit)
                    return (Stack<Point>) null;
            }

            return (Stack<Point>) null;
        }

        private static readonly sbyte[,] Directions = new sbyte[4, 2]
        {
            {
                (sbyte) -1,
                (sbyte) 0
            },
            {
                (sbyte) 1,
                (sbyte) 0
            },
            {
                (sbyte) 0,
                (sbyte) 1
            },
            {
                (sbyte) 0,
                (sbyte) -1
            }
        };

        private static bool isPositionImpassableForNPCSchedule(GameLocation loc, int x, int y)
        {
            Tile tile = loc.Map.GetLayer("Buildings").Tiles[x, y];
            if (tile != null && tile.TileIndex != -1)
            {
                PropertyValue propertyValue = (PropertyValue) null;
                tile.TileIndexProperties.TryGetValue("Action", out propertyValue);
                if (propertyValue == null)
                    tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    string str = propertyValue.ToString();
                    if (str.StartsWith("LockedDoorWarp") || !str.Contains("Door") && !str.Contains("Passable"))
                        return true;
                }
                else if (loc.doesTileHaveProperty(x, y, "Passable", "Buildings") == null &&
                         loc.doesTileHaveProperty(x, y, "NPCPassable", "Buildings") == null)
                    return true;
            }

            if (loc.doesTileHaveProperty(x, y, "NoPath", "Back") != null)
                return true;
            foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) loc.warps)
            {
                if (warp.X == x && warp.Y == y)
                    return true;
            }

            return loc.isTerrainFeatureAt(x, y);
        }

        private static int getPreferenceValueForTerrainType(GameLocation l, int x, int y)
        {
            string str = l.doesTileHaveProperty(x, y, "Type", "Back");
            if (str != null)
            {
                string lower = str.ToLower();
                if (lower == "stone")
                    return -7;
                if (lower == "wood")
                    return -4;
                if (lower == "dirt")
                    return -2;
                if (lower == "grass")
                    return -1;
            }

            return 0;
        }
    }
}
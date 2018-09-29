using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

using xTile.Dimensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SFarmer = StardewValley.Farmer;

namespace PetEnhancements
{
    class PathFindingUtil
    {
        private static byte[,] weight;

        public static Vector2 getTargetPosition(Vector2 tile)
        {
            return new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);
        }

        public static List<Vector2> FindPathToFarmer(Pet pet, SFarmer player)
        {
            updateWeights();
            var start = new Vector2((int)Math.Round(pet.position.X / Game1.tileSize), (int)Math.Round(pet.position.Y / Game1.tileSize));
            var end = player.getTileLocation();


            var closedSet = new List<Vector2>();
            var openSet = new List<Vector2> { start };
            var cameFrom = new Dictionary<Vector2, Vector2>();
            var currentDistance = new Dictionary<Vector2, double>();
            var predictedDistance = new Dictionary<Vector2, double>();

            currentDistance.Add(start, 0);
            predictedDistance.Add(start, 0 + performHeuristic(start, end));

            while (openSet.Count > 0)
            {
                var current = (from p in openSet orderby predictedDistance[p] ascending select p).First();

                if (current.X == end.X && current.Y == end.Y)
                {
                    return reconstructPath(cameFrom, end);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in getOpenAdjacentTiles(player.currentLocation, current))
                {
                    var tempCurrentDistance = currentDistance[current] + 1;

                    if (closedSet.Contains(neighbor) && tempCurrentDistance >= currentDistance[neighbor])
                    {
                        continue;
                    }

                    if (canClearTile(neighbor) || neighbor == end)
                    {
                        if (cameFrom.Keys.Contains(neighbor))
                        {
                            cameFrom[neighbor] = current;
                        }
                        else
                        {
                            cameFrom.Add(neighbor, current);
                        }

                        currentDistance[neighbor] = tempCurrentDistance;
                        predictedDistance[neighbor] = currentDistance[neighbor] + performHeuristic(neighbor, end);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            // unable to figure out a path, abort.
            throw new Exception(
                string.Format(
                    "unable to find a path.",
                    start.X, start.Y,
                    end.X, end.Y
                )
            );
        }

        private static double performHeuristic(Vector2 start, Vector2 end)
        {
            return Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));
        }

        private static List<Vector2> getOpenAdjacentTiles(GameLocation location, Vector2 tile)
        {
            var tiles = new List<Vector2>();
            foreach (var adjTile in Utility.getAdjacentTileLocations(tile))
            {
                var validX = adjTile.X >= 0 && adjTile.X < weight.GetLength(0);
                var validY = adjTile.Y >= 0 && adjTile.Y < weight.GetLength(1);
                if (validX && validY && weight[(int)adjTile.X, (int)adjTile.Y] == 0)
                {
                    tiles.Add(adjTile);
                }
            }

            return tiles;
        }

        private static List<Vector2> reconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
        {
            if (!cameFrom.Keys.Contains(current))
            {
                return new List<Vector2>();
            }

            var path = reconstructPath(cameFrom, cameFrom[current]);
            path.Add(current);
            return path;
        }

        private static void updateWeights()
        {
            var location = Game1.player.currentLocation;

            var xTiles = location.map.DisplayWidth / Game1.tileSize;
            var yTiles = location.map.DisplayHeight / Game1.tileSize;

            weight = new byte[xTiles, yTiles];

            for (int i = 0; i < xTiles; i++)
            {
                for (int h = 0; h < yTiles; h++)
                {
                    if (!isPassableTile(location, i, h))
                    {
                        weight[i, h] = 1;
                    }
                }
            }
        }

        private static bool canClearTile(Vector2 tile)
        {
            bool ret = false;
            var columnToCheck = tile.X + 1;
            try
            {
                if (columnToCheck < weight.GetLength(0))
                {
                    ret = weight[(int)columnToCheck, (int)tile.Y] == 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("DEBUG --> Failed to check tile at X position " + columnToCheck + ": " + e.Message);
            }
            return ret;
        }

        private static bool isPassableTile(GameLocation location, int x, int y)
        {
            var tile = new Vector2(x, y);

            Location displayLoc = new Location(x * Game1.tileSize, y * Game1.tileSize);

            StardewValley.Object obj = null;
            location.objects.TryGetValue(tile, out obj);

            TerrainFeature terrainObj = null;
            location.terrainFeatures.TryGetValue(tile, out terrainObj);

            Building building = null;
            if (location is BuildableGameLocation)
            {
                building = ((BuildableGameLocation)location).getBuildingAt(tile);
            }

            return location.isPointPassable(displayLoc, Game1.viewport)
                   && building == null
                   && (obj == null || obj.isPassable())
                   && (terrainObj == null || terrainObj.isPassable());
        }
    }
}

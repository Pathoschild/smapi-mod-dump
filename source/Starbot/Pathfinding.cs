using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbot.Pathfinder
{
    public class Location
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Location Parent;
        public bool Preferable = false;
    }

    public class Pathfinder
    {
        public static List<Tuple<int,int>> FindPath(GameLocation location, int startX, int startY, int targetX, int targetY, int cutoff = -1)
        {
            Location current = null;
            Location start = new Location { X = startX, Y = startY };
            Location target = new Location { X = targetX, Y = targetY };
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;

            // start by adding the original position to the open list  
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the square with the lowest F score  
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add to closed, remove from open
                closedList.Add(current);
                openList.Remove(current);

                // if closed contains destination, we're done
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null) break;

                // if closed has exceed cutoff, break out and fail
                if (cutoff > 0 && closedList.Count > cutoff)
                {
                    //Mod.instance.Monitor.Log("Breaking out of pathfinding, cutoff exceeded");
                    return null;
                }

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, location, openList);
                g = current.G + 1;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if closed, ignore 
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in open
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute score, set parent  
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.Preferable, adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to open
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score lower
                        // if yes update the parent because it means it's a better path  
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            //make sure path is complete
            if (current == null) return null;
            if (current.X != targetX || current.Y != targetY)
            {
                //Mod.instance.Monitor.Log("No path available.", StardewModdingAPI.LogLevel.Warn);
                return null;
            }

            // if path exists, let's pack it up for return
            var returnPath = new List<Tuple<int, int>>();
            while (current != null)
            {
                returnPath.Add(new Tuple<int, int>(current.X, current.Y));
                current = current.Parent;
            }
            returnPath.Reverse();
            return returnPath;
        }

        static List<Location> GetWalkableAdjacentSquares(int x, int y, GameLocation map, List<Location> openList)
        {
            List<Location> list = new List<Location>();

            if (IsPassable(map, x, y - 1))
            {
                Location node = openList.Find(l => l.X == x && l.Y == y - 1);
                if (node == null) list.Add(new Location() { Preferable = IsPreferableWalkingSurface(map, x, y), X = x, Y = y - 1 });
                else list.Add(node);
            }

            if (IsPassable(map, x, y + 1))
            {
                Location node = openList.Find(l => l.X == x && l.Y == y + 1);
                if (node == null) list.Add(new Location() { Preferable = IsPreferableWalkingSurface(map, x, y), X = x, Y = y + 1 });
                else list.Add(node);
            }

            if (IsPassable(map, x - 1, y))
            {
                Location node = openList.Find(l => l.X == x - 1 && l.Y == y);
                if (node == null) list.Add(new Location() { Preferable = IsPreferableWalkingSurface(map, x, y), X = x - 1, Y = y });
                else list.Add(node);
            }

            if (IsPassable(map, x + 1, y))
            {
                Location node = openList.Find(l => l.X == x + 1 && l.Y == y);
                if (node == null) list.Add(new Location() { Preferable = IsPreferableWalkingSurface(map, x, y), X = x + 1, Y = y });
                else list.Add(node);
            }

            return list;
        }

        static bool IsPreferableWalkingSurface(GameLocation location, int x, int y)
        {
            //todo, make roads more desireable
            return false;
        }

        static bool IsPassable(GameLocation location, int x, int y)
        {
            var v = new Vector2(x, y);
            bool isWarp = false;
            foreach(var w in location.warps)
            {
                if (w.X == x && w.Y == y) isWarp = true;
            }
            bool isOnMap = location.isTileOnMap(v);
            bool isOccupied = location.isTileOccupiedIgnoreFloors(v, "");
            bool isPassable = location.isTilePassable(new xTile.Dimensions.Location((int)x, (int)y), Game1.viewport);
            //check for bigresourceclumps on the farm
            if(location is Farm)
            {
                var fff = location as Farm;
                foreach(var brc in fff.largeTerrainFeatures)
                {
                    var r = brc.getBoundingBox();
                    var xx = x;
                    var yy = y;
                    if (xx > r.X && xx < r.X + r.Width && yy > r.Y && yy < r.Y + r.Height) return false;
                }
            }
            if (location is StardewValley.Locations.BuildableGameLocation)
            {
                var bgl = location as StardewValley.Locations.BuildableGameLocation;
                foreach (var b in bgl.buildings)
                {
                    if (!b.isTilePassable(v)) return false;
                }
            }
            if(location is StardewValley.Locations.BuildableGameLocation || location is Farm)
            {
                //more aggressive test. doesn't like floors
                if (location.isCollidingPosition(new Rectangle((x * 64) + 2, (y * 64) + 2, 60, 60), Game1.viewport, true, 0, false, null, false, false, true)) return false;
            }
            return (isWarp || (isOnMap && !isOccupied && isPassable)); //warps must be passable even off-map
        }

        static int ComputeHScore(bool preferable, int x, int y, int targetX, int targetY)
        {
            return (Math.Abs(targetX - x) + Math.Abs(targetY - y)) - (preferable ? 1 : 0);
        }
    }
}
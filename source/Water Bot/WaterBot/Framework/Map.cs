/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andyruwruw/stardew-valley-water-bot
**
*************************************************/

using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System;
using System.Linq;
using BotFramework;
using xTile.Tiles;
using Microsoft.Xna.Framework.Graphics;

namespace WaterBot.Framework
{
    public delegate void console(string message);

    /// <summary>
    /// Contains map data and relevant path finding / grouping algorithms.
    /// </summary>
    class Map
    {
        public List<List<Tile>> map;

        public List<Tile> waterableTiles;

        public List<Group> groupings;

        public int width;

        public int height;

        private Tuple<int, int, Func<int, int, bool>>[] directions;

        private Tuple<int, int, Func<int, int, bool>>[] diagonals;

        public Map()
        {
            this.directions = new Tuple<int, int, Func<int, int, bool>>[] {
                new Tuple<int, int, Func<int, int, bool>>(1, 0, (int y, int x) => y < this.height),
                new Tuple<int, int, Func<int, int, bool>>(-1, 0, (int y, int x) => y >= 0),
                new Tuple<int, int, Func<int, int, bool>>(0, 1, (int y, int x) => x < this.width),
                new Tuple<int, int, Func<int, int, bool>>(0, -1, (int y, int x) => x >= 0),
            };
            this.diagonals = new Tuple<int, int, Func<int, int, bool>>[] {
                new Tuple<int, int, Func<int, int, bool>>(1, 1, (int y, int x) => x < this.width && y < this.height),
                new Tuple<int, int, Func<int, int, bool>>(-1, -1, (int y, int x) => x >= 0 && y >= 0),
                new Tuple<int, int, Func<int, int, bool>>(1, -1, (int y, int x) => x >= 0 && y < this.height),
                new Tuple<int, int, Func<int, int, bool>>(-1, 1, (int y, int x) => x < this.width && y >= 0),
            };
        }

        /// <summary>
        /// Converts coords to tile.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        /// <param name="width">Number of tiles horizontally.</param>
        /// <param name="height">Number of tiles vertically.</param>
        public static Point convertCoords(int x, int y, int width, int height)
        {
            return new Point(y, x);
        }

        /// <summary>
        /// Whether tile is passable.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        public static bool tileIsPassable(int x, int y)
        {
            return Game1.currentLocation.isCollidingPosition(new Rectangle(y * 64 + 1, x * 64 + 1, 62, 62), Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);
        }

        /// <summary>
        /// Whether tile can refill watering can.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        public static bool tileIsRefillable(int x, int y)
        {
            return Game1.currentLocation.CanRefillWateringCanOnTile(y, x);
        }

        /// <summary>
        /// Whether tile needs watering.
        /// </summary>
        /// 
        /// <param name="x">X of tile.</param>
        /// <param name="y">Y of tile.</param>
        public static bool tileNeedsWatering(int x, int y)
        {
            TerrainFeature feature;
            Vector2 index = new Vector2(x, y);

            return Game1.currentLocation.isTileHoeDirt(index) &&
                Game1.currentLocation.GetHoeDirtAtTile(index) != null &&
                Game1.currentLocation.terrainFeatures.TryGetValue(index, out feature) &&
                ((Game1.currentLocation.terrainFeatures[index] as HoeDirt).state.Value == 0) &&
                ((Game1.currentLocation.terrainFeatures[index] as HoeDirt).crop != null) &&
                (!(Game1.currentLocation.terrainFeatures[index] as HoeDirt).crop.dead) &&
                (((Game1.currentLocation.terrainFeatures[index] as HoeDirt).crop.fullyGrown &&
                (Game1.currentLocation.terrainFeatures[index] as HoeDirt).crop.dayOfCurrentPhase > 0) ||
                ((Game1.currentLocation.terrainFeatures[index] as HoeDirt).crop.currentPhase < (Game1.currentLocation.terrainFeatures[index] as HoeDirt).crop.phaseDays.Count - 1) ||
                (Game1.currentLocation.terrainFeatures[index] as HoeDirt).crop.RegrowsAfterHarvest());
        }

        /// <summary>
        /// Loads farm data into array.
        /// </summary>
        /// 
        /// <param name="location">Farm location instance.</param>
        public void loadMap()
        {
            this.height = Game1.currentLocation.map.Layers[0].LayerHeight;
            this.width = Game1.currentLocation.map.Layers[0].LayerWidth;

            List<List<Tile>> map = new List<List<Tile>>();
            List<Tile> waterableTiles = new List<Tile>();

            for (int y = 0; y < this.height; y++)
            {
                List<Tile> row = new List<Tile>();

                for (int x = 0; x < this.width; x++)
                {
                    Point converted = Map.convertCoords(x, y, this.width, this.height);
                    bool needsWatering = Map.tileNeedsWatering(converted.Y, converted.X);

                    Tile tile = new Tile(
                        x,
                        y,
                        Map.tileIsPassable(converted.X, converted.Y),
                        Map.tileIsRefillable(converted.X, converted.Y),
                        needsWatering
                    );

                    if (needsWatering)
                    {
                        waterableTiles.Add(tile);
                    }
                    row.Add(tile);
                }

                map.Add(row);
            }

            this.map = map;
            this.waterableTiles = waterableTiles;
        }

        /// <summary>
        /// Returns a list of adjacent tiles.
        /// </summary>
        /// 
        /// <param name="tile">Tile to find neighbors of.</param>
        public List<Tile> getNeighbors(Tile tile, bool checkBlock = false, bool checkWaterable = false)
        {
            List<Tile> list = new List<Tile>();

            if (tile.x > 0)
            {
                Tile neighbor = this.map[tile.y][tile.x - 1];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }
            if (tile.y > 0)
            {
                Tile neighbor = this.map[tile.y - 1][tile.x];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }
            if (tile.x < this.width - 1)
            {
                Tile neighbor = this.map[tile.y][tile.x + 1];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }
            if (tile.y < this.height - 1)
            {
                Tile neighbor = this.map[tile.y + 1][tile.x];
                if ((!checkBlock || !neighbor.block) && (!checkWaterable || neighbor.waterable))
                {
                    list.Add(neighbor);
                }
            }

            return list;
        }

        /// <summary>
        /// Find all groupings of waterable tiles
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public void findGroupings(console console)
        {
            this.groupings = new List<Group>();

            foreach (Tile tile in this.waterableTiles)
            {
                bool grouped = false;

                if (tile.visited)
                {
                    continue;
                }

                int index = this.groupings.Count;

                if (tile.x <= this.width - 1)
                {
                    Tile neighbor = this.map[tile.y][tile.x + 1];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        Group group = new Group(index);
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                }
                if (tile.y <= this.height - 1 && !grouped)
                {
                    Tile neighbor = this.map[tile.y + 1][tile.x];
                    if (neighbor.waterable)
                    {
                        grouped = true;
                        Group group = new Group(index);
                        this.populateGroup(group, tile);
                        this.groupings.Add(group);
                    }
                }
                if (!grouped)
                {
                    Group solo = new Group(index);

                    solo.Add(tile);
                    tile.visited = true;

                    this.groupings.Add(solo);
                }
            }
        }

        /// <summary>
        /// Recursively find connected waterable tiles (depth-first search)
        /// </summary>
        /// 
        /// <param name="group">List of tiles so far.</param>
        /// <param name="tile">New tile added.</param>
        public void populateGroup(Group group, Tile tile)
        {
            // Add the tile and mark it as visted
            group.Add(tile);
            tile.visited = true;

            // Check edge cases
            bool leftBorder = tile.x == 0;
            bool rightBorder = tile.x == this.width - 1;
            bool topBorder = tile.y == 0;
            bool bottomBorder = tile.y == this.height - 1;

            // Recursively check neighbors
            if (!leftBorder)
            {
                Tile neighbor = this.map[tile.y][tile.x - 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!rightBorder)
            {
                Tile neighbor = this.map[tile.y][tile.x + 1];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!topBorder)
            {
                Tile neighbor = this.map[tile.y - 1][tile.x];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
            if (!bottomBorder)
            {
                Tile neighbor = this.map[tile.y + 1][tile.x];
                if (neighbor.waterable && neighbor.visited == false)
                {
                    this.populateGroup(group, neighbor);
                }
            }
        }

        public Tile findClosestWalkableTile(Tile center)
        {
            foreach (List<Tile> row in this.map)
            {
                foreach (Tile tile in row)
                {
                    tile.walkableCheck = false;
                }
            }

            List<Tile> queue = new List<Tile>();
            queue.Add(center);

            while (queue.Count > 0)
            {
                Tile current = queue[0];
                queue.RemoveAt(0);

                if (current.walkableCheck)
                {
                    continue;
                }

                current.walkableCheck = true;

                if (!current.block)
                {
                    return current;
                } else
                {
                    foreach (Tuple<int, int, Func<int, int, bool>> direction in this.directions.Concat(this.diagonals))
                    {
                        if (direction.Item3(current.y + direction.Item1, current.x + direction.Item2))
                        {
                            if (!this.map[current.y + direction.Item1][current.x + direction.Item2].walkableCheck)
                            {
                                queue.Add(this.map[current.y + direction.Item1][current.x + direction.Item2]);
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Runs A* on map to find shortest path from start to end.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        /// <param name="start">Point to start at.</param>
        /// <param name="end">Point to end at.</param>
        public Tuple<List<Tile>, int> walkablePathBetweenPoints(console console, Point start, Point end)
        {
            List<Tile> openSet = new List<Tile>();
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            Dictionary<Tile, int> gScore = new Dictionary<Tile, int>();
            Dictionary<Tile, int> fScore = new Dictionary<Tile, int>();

            Tile startTile = this.map[start.Y][start.X];

            openSet.Add(startTile);

            gScore.Add(startTile, 0);
            fScore.Add(startTile, startTile.distanceTo(end));

            while (openSet.Count > 0)
            {
                Tile current = openSet[0];
                int smallest = int.MaxValue;

                foreach (Tile tile in openSet)
                {
                    if (fScore[tile] < smallest)
                    {
                        smallest = fScore[tile];
                        current = tile;
                    }
                }

                if (current.Equals(end))
                {
                    List<Tile> path = new List<Tile>();
                    while (cameFrom.Keys.Contains(current))
                    {
                        current = cameFrom[current];
                        path.Add(current);
                    }
                    return new Tuple<List<Tile>, int>(path, path.Count);
                }

                openSet.Remove(current);

                List<Tile> neighbors = getNeighbors(current, true, false);

                foreach (Tile neighbor in neighbors)
                {
                    if (!gScore.Keys.Contains(neighbor))
                    {
                        gScore.Add(neighbor, int.MaxValue);
                    }

                    int tenativeGScore = gScore[current] + 1;
                    if (tenativeGScore < gScore[neighbor])
                    {
                        if (!cameFrom.Keys.Contains(neighbor))
                        {
                            cameFrom.Add(neighbor, null);
                        }
                        if (!fScore.Keys.Contains(neighbor))
                        {
                            fScore.Add(neighbor, int.MaxValue);
                        }

                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tenativeGScore;
                        fScore[neighbor] = tenativeGScore + neighbor.distanceTo(end);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            return new Tuple<List<Tile>, int>(null, int.MaxValue);
        }

        /// <summary>
        /// Uses A* to find cost between different groupings.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public int[,] generateCostMatrix(console console)
        {
            foreach (Group group in this.groupings)
            {
                if (group.Count() == 0)
                {
                    this.groupings.Remove(group);
                }
            }

            int nodes = this.groupings.Count + 1;

            int[,] costMatrix = new int[nodes, nodes];

            // From
            for (int i = 0; i < nodes; i++)
            {
                // To
                for (int j = 0; j < nodes; j++)
                {
                    if (i == j)
                    {
                        costMatrix[i, j] = -1;
                    }
                    else
                    {
                        if (costMatrix[j, i] > 0)
                        {
                            costMatrix[i, j] = costMatrix[j, i];
                        }
                        else if (j == 0 || i == 0)
                        {
                            Point start = new Point(Game1.player.TilePoint.X, Game1.player.TilePoint.Y);
                            Point end = this.groupings[i == 0 ? j - 1 : i - 1].Centroid(this);

                            Tuple<List<Tile>, int> path = this.walkablePathBetweenPoints(console, start, end);

                            costMatrix[i, j] = path.Item2;
                        }
                        else
                        {
                            Point start = this.groupings[i - 1].Centroid(this);
                            Point end = this.groupings[j - 1].Centroid(this);

                            Tuple<List<Tile>, int> path = this.walkablePathBetweenPoints(console, start, end);

                            costMatrix[i, j] = path.Item2;
                        }
                    }
                }
            }

            List<int> safeGroups = new List<int>();
            List<int> deleteGroups = new List<int>();

            for (int i = 0; i < nodes; i++)
            {
                if (costMatrix[0, i] == int.MaxValue)
                {
                    deleteGroups.Add(i);
                }
                else
                {
                    safeGroups.Add(i);
                }
            }

            deleteGroups.Sort();

            for (int i = deleteGroups.Count - 1; i >= 0; i--)
            {
                this.groupings.RemoveAt(deleteGroups[i] - 1);
            }

            int[,] reachableCostMatrix = new int[safeGroups.Count, safeGroups.Count];

            for (int i = 0; i < safeGroups.Count; i++)
            {
                for (int j = 0; j < safeGroups.Count; j++)
                {
                    reachableCostMatrix[i, j] = costMatrix[safeGroups[i], safeGroups[j]];
                    reachableCostMatrix[j, i] = costMatrix[safeGroups[j], safeGroups[i]];
                }
            }

            return reachableCostMatrix;
        }  

        /// <summary>
        /// Runs TSP Greedy to find best path through all groups
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public List<Group> findGroupPath(console console)
        {
            try
            {
                if (this.groupings.Count == 1)
                {
                    return this.groupings;
                }

                List<Group> path = new List<Group>();

                int[,] costMatrix = this.generateCostMatrix(console);

                int counter = 0;
                int j = 0;
                int i = 0;
                int min = int.MaxValue;

                List<int> visitedRouteList = new List<int>();

                visitedRouteList.Add(0);
                int[] route = new int[costMatrix.Length];

                while (i < costMatrix.GetLength(0) && j < costMatrix.GetLength(1))
                {
                    if (counter >= costMatrix.GetLength(0) - 1)
                    {
                        break;
                    }

                    if (j != i && !(visitedRouteList.Contains(j)))
                    {
                        if (costMatrix[i, j] < min)
                        {
                            min = costMatrix[i, j];
                            route[counter] = j + 1;
                        }
                    }
                    j++;

                    if (j == costMatrix.GetLength(0))
                    {
                        min = int.MaxValue;
                        visitedRouteList.Add(route[counter] - 1);

                        j = 0;
                        i = route[counter] - 1;
                        counter++;
                    }
                }

                foreach (int index in visitedRouteList)
                {
                    if (index != 0)
                    {
                        path.Add(this.groupings[index - 1]);
                    }
                }

                return path;
            } catch (IndexOutOfRangeException e)
            {
                console("WaterBot encountered an error and will return a sub-optimal path.");
                return this.groupings;
            } catch (Exception e)
            {
                console("WaterBot encountered an unknown error and will return a sub-optimal path.");
                return this.groupings;
            }
        }

        /// <summary>
        /// Finds path in group through adjacent tiles
        /// </summary>
        /// 
        /// <param name="group">Group of crops to find path through.</param>
        public List<ActionableTile> findFillPath(Group group, console console)
        {
            // Start a new path of actionable tiles
            // Actionable tiles are a standing place, and nearby tiles to water
            List<ActionableTile> path = new List<ActionableTile>();

            // Reset the visited values of each tile in group.
            foreach (Tile tile in group.getList())
            {
                this.map[tile.y][tile.x].reset();
            }

            // Queue for depth first search
            List<Tile> stack = new List<Tile>();
            stack.Add(group.findClosestTile(Game1.player.TilePoint.X, Game1.player.TilePoint.Y).Item1);

            bool keepGoing;
            do
            {
                keepGoing = false;

                while (stack.Count > 0)
                {
                    Tile current = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);

                    if (current.visited)
                    {
                        continue;
                    }
                    else if (current.watered)
                    {
                        current.visited = true;

                        foreach (Tuple<int, int, Func<int, int, bool>> direction in this.directions)
                        {
                            if (direction.Item3(current.y + direction.Item1, current.x + direction.Item2))
                            {
                                Tile neighbor = this.map[current.y + direction.Item1][current.x + direction.Item2];
                                if (!neighbor.visited && group.Contains(neighbor))
                                {
                                    stack.Add(neighbor);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Start a new action
                        ActionableTile actionable = new ActionableTile(ActionableTile.Action.Water);
                        current.visited = true;

                        if (current.block)
                        {
                            int score = 0;
                            Tile bestOption = null;

                            // If you can stand on adjacents, do it.
                            foreach (Tuple<int, int, Func<int, int, bool>> direction in this.directions.Concat(this.diagonals))
                            {
                                if (direction.Item3(current.y + direction.Item1, current.x + direction.Item2))
                                {
                                    Tile neighbor = this.map[current.y + direction.Item1][current.x + direction.Item2];
                                    if (neighbor.block)
                                    {
                                        continue;
                                    }

                                    int neighborScore = 0;

                                    if (group.Contains(neighbor)) neighborScore += 5;
                                    if (neighbor.waterable) neighborScore += 5;
                                    if (!neighbor.watered) neighborScore += 5;
                                    if (!neighbor.visited) neighborScore += 5;

                                    if (neighborScore > score)
                                    {
                                        bestOption = neighbor;
                                    }
                                }
                            }

                            if (bestOption == null)
                            {
                                continue;
                            }

                            // Set possible standing point.
                            actionable.setStand(bestOption.getPoint());
                        }
                        else
                        {
                            // Set this as standing position
                            actionable.setStand(current.getPoint());
                        }

                        // Water here
                        if (current.getPoint() == actionable.getStand())
                        {
                            actionable.pushExecuteOn(current.getPoint());
                            current.watered = true;
                        }
                        else if (this.map[actionable.getStand().Y][actionable.getStand().X].waterable && !this.map[actionable.getStand().Y][actionable.getStand().X].watered)
                        {
                            actionable.pushExecuteOn(actionable.getStand());
                            this.map[actionable.getStand().Y][actionable.getStand().X].watered = true;
                        }

                        // If you can water adjacents, do it.
                        foreach (Tuple<int, int, Func<int, int, bool>> direction in this.directions)
                        {
                            if (direction.Item3(actionable.getStand().Y + direction.Item1, actionable.getStand().X + direction.Item2))
                            {
                                Tile neighbor = this.map[actionable.getStand().Y + direction.Item1][actionable.getStand().X + direction.Item2];
                                if (neighbor.waterable && !neighbor.watered)
                                {
                                    actionable.pushExecuteOn(neighbor.getPoint());
                                    neighbor.watered = true;
                                }
                                if (!neighbor.visited && group.Contains(neighbor))
                                {
                                    stack.Add(neighbor);
                                }
                            }
                        }

                        // If you can water diagonals, do it.
                        foreach (Tuple<int, int, Func<int, int, bool>> direction in this.diagonals)
                        {
                            if (direction.Item3(actionable.getStand().Y + direction.Item1, actionable.getStand().X + direction.Item2))
                            {
                                Tile neighbor = this.map[actionable.getStand().Y + direction.Item1][actionable.getStand().X + direction.Item2];
                                if (neighbor.waterable && !neighbor.watered)
                                {
                                    actionable.pushExecuteOn(neighbor.getPoint());
                                    neighbor.watered = true;
                                }
                            }
                        }

                        path.Add(actionable);
                    }
                }

                foreach (Tile tile in group.getList())
                {
                    if (!tile.visited)
                    {
                        stack.Add(tile);
                        keepGoing = true;
                        break;
                    }
                }
            } while (keepGoing);

            return path;
        }

        /// <summary>
        /// Finds the closest refill location
        /// </summary>
        public ActionableTile getClosestRefill(Tile start, console console)
        {
            foreach (List<Tile> row in this.map)
            {
                foreach (Tile tile in row)
                {
                    tile.waterCheck = false;
                }
            }

            List<Tuple<Tile, Tile>> queue = new List<Tuple<Tile, Tile>>(); // Current, Last
            queue.Add(new Tuple<Tile, Tile>(start, null));

            Tile current = null;
            Tile last = null;

            while (queue.Count > 0)
            {
                last = queue[0].Item2;
                current = queue[0].Item1;
                queue.RemoveAt(0);

                if (current.waterCheck)
                {
                    continue;
                }

                current.waterCheck = true;

                if (current.water)
                {
                    List<Point> actions = new List<Point>();
                    actions.Add(current.getPoint());

                    if (current.block)
                    {
                        return new ActionableTile(last.getPoint(), actions, ActionableTile.Action.Refill);
                    }
                    else
                    {
                        return new ActionableTile(current.getPoint(), actions, ActionableTile.Action.Refill);
                    }
                }

                if (current.block && current != start)
                {
                    continue;
                }

                foreach (Tuple<int, int, Func<int, int, bool>> direction in this.directions)
                {
                    if (direction.Item3(current.y + direction.Item1, current.x + direction.Item2))
                    {
                        if (!this.map[current.y + direction.Item1][current.x + direction.Item2].block || this.map[current.y + direction.Item1][current.x + direction.Item2].water)
                        {
                            queue.Add(new Tuple<Tile, Tile>(this.map[current.y + direction.Item1][current.x + direction.Item2], current));
                        }
                    }
                }
            }

            return null;
        }
    }
}

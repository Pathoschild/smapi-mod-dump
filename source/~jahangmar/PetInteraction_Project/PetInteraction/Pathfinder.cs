//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using CostMap = System.Collections.Generic.Dictionary<Microsoft.Xna.Framework.Vector2, int>;
using Node = Microsoft.Xna.Framework.Vector2;
using NodeMap = System.Collections.Generic.Dictionary<Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2>;


namespace PetInteraction
{
    public class PathFinder
    {

        private static readonly Dictionary<GameLocation, HashSet<Node>> cachedPassableTiles = new Dictionary<GameLocation, HashSet<Node>>();

        public static Queue<Vector2> CalculatePath(Pet pet, Vector2 dest)
        {
            Vector2 src = new Vector2((int)System.Math.Round(pet.position.X / Game1.tileSize), (int)System.Math.Round(pet.position.Y / Game1.tileSize));
            dest = new Vector2((int)dest.X, (int)dest.Y);

            if (ModEntry.debug())
                ModEntry.Log($"Trying to find path from {src} to {dest}", LogLevel.Trace);


            List<Vector2> addedTiles = Utility.getAdjacentTileLocations(dest);

            Queue<Vector2> path = new Queue<Node>();

            int loop = 0;
            Queue<Node> lookatTiles = new Queue<Node>(addedTiles);
            while (loop++ < 4)
            {
                Node curr = lookatTiles.Dequeue();
                if (IsPassable(curr, pet))
                {
                    dest = curr;
                    path = PathFinder.FindPath(src, dest, pet);
                    if (path.Count > 0)
                        break;
                }
                foreach (Node adj in Utility.getAdjacentTileLocations(curr))
                {
                    if (!addedTiles.Contains(adj))
                    {
                        lookatTiles.Enqueue(adj);
                        addedTiles.Add(adj);
                    }
                }
            }

            if (ModEntry.debug())
            {
                ModEntry.Log("Path: ", LogLevel.Trace);
                foreach (Vector2 v in path)
                    ModEntry.Log(v.ToString(), LogLevel.Trace);
            }
            return new Queue<Vector2>(path.Take(path.Count - 1));
        }

        private static void AddCachedPassableTile(Node tile)
        {
            if (cachedPassableTiles.ContainsKey(Game1.currentLocation))
                cachedPassableTiles[Game1.currentLocation].Add(tile);
            else
                cachedPassableTiles.Add(Game1.currentLocation, new HashSet<Node>() { tile });
        }

        public static void RemoveCachedPassableTile(Node tile)
        {
            if (cachedPassableTiles.ContainsKey(Game1.currentLocation))
                cachedPassableTiles[Game1.currentLocation].Remove(tile);
        }

        private static bool IsCachedPassableTile(Node tile)
        {
            return cachedPassableTiles.ContainsKey(Game1.currentLocation) && cachedPassableTiles[Game1.currentLocation].Contains(tile);
        }

        public static void ResetCachedTiles()
        {
            foreach (GameLocation location in Game1.locations)
                ResetCachedTiles(location);
        }

        public static void ResetCachedTiles(GameLocation location)
        {
            if (cachedPassableTiles.ContainsKey(location))
                cachedPassableTiles[location].Clear();
            else
                cachedPassableTiles.Add(location, new HashSet<Node>());
        }

        private static Queue<Vector2> FindPath(Vector2 source, Vector2 destination, Pet pet)
        {
            return AStar(source, destination, pet);
        }

        private static Queue<Vector2> ReconstructPath(NodeMap cameFrom, Node current)
        {
            List<Node> total_path = new List<Node>();
            total_path.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                total_path.Insert(0, current);
            }
            return new Queue<Node>(total_path);
        }

        private static Queue<Vector2> AStar(Node source, Node dest, Pet pet)
        {
            NodeMap cameFrom = new NodeMap();

            CostMap g = new CostMap();
            g.Add(source, 0);

            CostMap f = new CostMap();
            f.Add(source, Heur(source, dest));

            List<Node> openSet = new List<Node>()
            {
                source
            };

            int loops = 0;

            while (openSet.Count > 0 && loops++ < 100)
            {
                Node current = openSet[0];
                if (current == dest)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);

                foreach (Node neighbor in GetPassableNeighbors(current, pet))
                {
                    int tentative_g = GetCost(g, current) + 1;

                    if (tentative_g < GetCost(g, neighbor))
                    {
                        Add(cameFrom, neighbor, current);
                        Add(g, neighbor, tentative_g);
                        Add(f, neighbor, tentative_g + Heur(neighbor, dest));
                        if (!openSet.Contains(neighbor))
                            AddSorted(openSet, neighbor, f);
                    }
                }
            }
            return new Queue<Vector2>();
        }

        private static void AddSorted(List<Node> list, Node node, CostMap f)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (GetCost(f, node) < GetCost(f, list[i]))
                {
                    list.Insert(i, node);
                    return;
                }
            }
            list.Insert(list.Count, node);
        }


        private static int GetCost(CostMap map, Node n) => map.ContainsKey(n) ? map[n] : int.MaxValue;

        private static void Add<T>(Dictionary<Node, T> dic, Node key, T value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }

        /// <summary>
        /// Heuristic for the A* algorithm. Manhattan distance.
        /// </summary>
        private static int Heur(Node n, Node dest)
        {
            //own experiments and
            //Masilo Mapaila: "EFFICIENT PATH FINDING FOR TILE-BASED 2D GAMES", Cape Town, South Africa
            //show that this is better than euclidian distance for this application
            return (int) (System.Math.Abs(n.X - dest.X) + System.Math.Abs(n.Y - dest.Y));
        }

        public static List<Node> GetPassableNeighbors(Node n, Pet pet)
        {
            List<Node> adjacents = StardewValley.Utility.getAdjacentTileLocations(n);
            if (ModEntry.debug())
                foreach (Node adj in adjacents)
                {
                    if (!ModEntry.NonPassables.Contains(adj) && !IsPassable(adj, pet))
                        ModEntry.NonPassables.Add(adj);
                    if (!ModEntry.Passables.Contains(adj) && IsPassable(adj, pet))
                        ModEntry.Passables.Add(adj);
                }

            List<Node> passables = adjacents.FindAll((Node node) => IsPassable(node, pet));
            return passables;
        }

        public static bool IsPassable(Node tile, Pet pet)
        {
            if (IsCachedPassableTile(tile))
                return true;
            else if (IsPassableSingle(tile, pet) && IsPassableSingle(new Node(tile.X + 1, tile.Y), pet))
            {
                AddCachedPassableTile(tile);
                return true;
            }
            return false;
        }

        public static bool IsPassableSingle(Node tile, Pet pet)
        {
            Rectangle rect = new Rectangle((int)tile.X * Game1.tileSize + 1, (int)tile.Y * Game1.tileSize + 1, Game1.tileSize - 2, Game1.tileSize - 2);

            //Vector2 plPos = Game1.player.Position;
            //Game1.player.position.Set(new Vector2(-100, -100));
            bool result = !(Game1.currentLocation.isCollidingPosition(rect, Game1.viewport, false, 0, false, pet, true, false, false));
            //Game1.player.position.Set(plPos);

            return result;
        }

    }
}

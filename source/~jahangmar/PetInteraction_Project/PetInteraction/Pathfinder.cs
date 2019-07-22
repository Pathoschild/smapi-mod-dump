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

using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Characters;

using StardewModdingAPI;

using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Node = Microsoft.Xna.Framework.Vector2;
using Path = System.Collections.Generic.Queue<Microsoft.Xna.Framework.Vector2>;
using CostMap = System.Collections.Generic.Dictionary<Microsoft.Xna.Framework.Vector2, int>;
using NodeMap = System.Collections.Generic.Dictionary<Microsoft.Xna.Framework.Vector2, Microsoft.Xna.Framework.Vector2>;


namespace PetInteraction
{
    public class PathFinder
    {
        public static Queue<Vector2> CalculatePath(Pet pet, Vector2 dest)
        {
            Vector2 src = new Vector2((int)System.Math.Round(pet.position.X / Game1.tileSize), (int)System.Math.Round(pet.position.Y / Game1.tileSize));
            dest = new Vector2((int)dest.X, (int)dest.Y);

            if (ModEntry.debug())
                ModEntry.Log($"Trying to find path from {src} to {dest}", LogLevel.Trace);


            List<Vector2> alts = Utility.getAdjacentTileLocations(dest);
            alts.Add(dest);
            alts.Add(dest + new Vector2(-1, -1));
            alts.Add(dest + new Vector2(1, 1));
            alts.Add(dest + new Vector2(-1, 1));
            alts.Add(dest + new Vector2(1, -1));
            foreach (Vector2 alt in alts)
            {
                if (PathFinder.IsPassable(alt))
                {
                    dest = alt;
                    break;
                }
            }

            Queue<Vector2> path = PathFinder.FindPath(src, dest);

            if (ModEntry.debug())
            {
                ModEntry.Log("Path: ", LogLevel.Trace);
                foreach (Vector2 v in path)
                    ModEntry.Log(v.ToString(), LogLevel.Trace);
            }
            return new Queue<Vector2>(path.Take(path.Count - 1));
        }

        private static Queue<Vector2> FindPath(Vector2 source, Vector2 destination)
        {
            return AStar(source, destination);
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

        private static Queue<Vector2> AStar(Node source, Node dest)
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

                foreach (Node neighbor in GetPassableNeighbors(current))
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

        public static List<Node> GetPassableNeighbors(Node n)
        {
            List<Node> adjacents = StardewValley.Utility.getAdjacentTileLocations(n);
            if (ModEntry.debug())
                foreach (Node adj in adjacents)
                {
                    if (!ModEntry.NonPassables.Contains(adj) && !IsPassable(adj))
                        ModEntry.NonPassables.Add(adj);
                    if (!ModEntry.Passables.Contains(adj) && IsPassable(adj))
                        ModEntry.Passables.Add(adj);
                }
            return adjacents.FindAll((Node node) => IsPassable(node));
        }

        public static bool IsPassable(Node tile)
        {
            return MapSpecificTiles(tile) || IsPassableSingle(tile) && IsPassableSingle(new Node(tile.X + 1, tile.Y));
        }

        public static bool IsPassableSingle(Node tile, bool checkCharacters = true)
        {

            Rectangle tileRect = new Rectangle((int)tile.X*Game1.tileSize, (int)tile.Y*Game1.tileSize, Game1.tileSize, Game1.tileSize);

            if (checkCharacters && Game1.player.GetBoundingBox().Intersects(tileRect))
                return false;

            GameLocation location = Game1.currentLocation;
            Object obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);

            TerrainFeature tf;
            location.terrainFeatures.TryGetValue(tile, out tf);

            foreach (LargeTerrainFeature ltf in location.largeTerrainFeatures)
            {
                if (ltf.getBoundingBox().Intersects(tileRect))
                {
                    return false;
                }
            }

            Building building = null;
            if (location is BuildableGameLocation bgl)
            {
                building = bgl.getBuildingAt(tile);
            }

            if (checkCharacters)
            {
                foreach (Character c in location.characters)
                {
                    if (!(c is Pet) && c.GetBoundingBox().Intersects(tileRect))
                    {
                        return false;
                    }
                }
            }

            return location.isTilePassable(new xTile.Dimensions.Location((int)tile.X, (int)tile.Y), Game1.viewport)
                && ((obj == null) || obj.isPassable())
                && ((tf == null) || (tf.isPassable()))
                && ((building == null) || (building.isTilePassable(tile)));
        }

        private static bool MapSpecificTiles(Node tile)
        {
            GameLocation location = Game1.currentLocation;
            HashSet<Node> passables = null;
            if (location is Town)
            {
                passables = new HashSet<Node>()
                {
                    //left stairs
                    new Node(20, 42),
                    new Node(20, 43),
                    new Node(20, 44),
                    new Node(20, 45),
                    new Node(20, 46),

                    //right stairs
                    new Node(48, 42),
                    new Node(48, 43),
                    new Node(48, 44),
                    new Node(48, 45),
                    new Node(48, 46),
                    new Node(48, 47),
                    new Node(48, 48),
                    new Node(48, 49),
                    new Node(48, 50),
                    new Node(48, 51),

                    //small bridge top-right
                    new Node(91, 11),
                    new Node(92, 11),
                    new Node(93, 11),
                    new Node(94, 11),
                    new Node(95, 11),
                    new Node(96, 11),

                    /* 
                     {X:78 Y:36}
                     {X:80 Y:35}
                     {X:82 Y:34}
                     {X:84 Y:33}
                     {X:70 Y:39}
                     {X:67 Y:49}
                     {X:67 Y:48}
                     {X:67 Y:47}
                     {X:67 Y:46}
                     {X:67 Y:45}
                     {X:67 Y:44}
                     {X:67 Y:43}
                     {X:67 Y:42}
                     {X:67 Y:41}
                     */
                };
            }
            else if (location is Mountain)
            {
                passables = new HashSet<Node>()
                {
                    //bridge near mine entrance
                new Node(45, 7),
                new Node(46, 7),
                new Node(47, 7),
                new Node(48, 7),
                new Node(49, 7),
                };
            }
            else if (location is Beach || location is BeachNightMarket)
            {
                passables = new HashSet<Node>()
                {
                    new Node(57, 13),
                    new Node(58, 13),
                    new Node(59, 13),
                    new Node(60, 13),
                    new Node(61, 13),
                };
            }
            else if (location is Forest)
            {
                passables = new HashSet<Node>()
                { 
                    //clock-wise bridges
                    new Node(76, 49),
                    new Node(77, 49),
                    new Node(78, 49),
                    new Node(79, 49),
                    new Node(80, 49),
                    new Node(81, 49),
                    new Node(82, 49),

                    new Node(87, 52),
                    new Node(87, 53),
                    new Node(87, 54),
                    new Node(87, 55),
                    new Node(87, 56),

                    new Node(61, 70),
                    new Node(62, 70),
                    new Node(63, 70),
                    new Node(64, 70),
                    new Node(65, 70),

                    new Node(40, 79),
                    new Node(40, 80),
                    new Node(40, 81),
                    new Node(40, 82),

                    new Node(37, 85),
                    new Node(37, 86),
                    new Node(37, 87),
                };
            }

            return (passables != null) && passables.Contains(tile);
        }
    }
}

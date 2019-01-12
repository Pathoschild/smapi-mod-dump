using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace FloodEventsTesting
{
    public class FloodEventsTesting : Mod
    {
        protected Dictionary<GameLocation,Dictionary<int,List<Point>>> FloodedTiles;
        protected int CurrentFloodDepth;
        protected int TotalFloodDepth = 4;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            FloodedTiles = new Dictionary<GameLocation, Dictionary<int, List<Point>>>();
            CurrentFloodDepth = TotalFloodDepth;
            //do something here, I suppose.
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        public static List<Point> GenerateFloodMap(GameLocation location, int depth, Func<GameLocation, int, int, bool> IsBlockedTile=null)
        {
            // Variables
            int[,] map = new int[location.map.Layers[0].LayerWidth, location.map.Layers[0].LayerHeight];
            int mx = location.map.Layers[0].LayerWidth, my = location.map.Layers[0].LayerHeight;
            // Recursive flood method
            void PopulateFromOrigin(int x, int y, int level = 0)
            {
                if (level > 0 && map[x, y] == int.MaxValue && (level > depth || IsBlockedTile?.Invoke(location, x, y) == true))
                    map[x, y] = -1;
                else
                {
                    map[x, y] = level;
                    for (int ox = -1; ox < 2; ox++)
                    for (int oy = -1; oy < 2; oy++)
                        if (ox >= 0 && ox < mx && oy >= 0 && oy < my && map[x, y] > level)
                            PopulateFromOrigin(x + ox, y + oy, level + 1);
                }
            }
            // Clear map
            for (int x = 0; x < mx; x++)
            for (int y = 0; y < my; y++)
                map[x, y] = int.MaxValue;
            // Populate map
            for (int x = 0; x < mx; x++)
            for (int y = 0; y < my; y++)
            {
                if (location.waterTiles[x, y])
                    PopulateFromOrigin(x, y, 0);
            }

            // copy flooded tiles
            var output = new List<Point>();
            for (int x = 0; x < mx; x++)
            for (int y = 0; y < my; y++)
            {
                int height = map[x, y];
                if (height < 0 || height == int.MaxValue)
                    continue;
                output.Add(new Point(x, y));
            }
            // return flooded tiles
            return output;
        }

        public bool StopTheWater(GameLocation loc, int x, int y)
        {
            if (loc.objects.ContainsKey(new Vector2(x,y)) && loc.objects[new Vector2(x,y)] is Fence sFence && (sFence.whichType.Value != Fence.wood || sFence.whichType.Value != Fence.steel))
            { 
                return true;
            }
            return false;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.IsMultipleOf(4))
                return;

            CreateFlood();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {

            if (e.Button == SButton.MouseMiddle) {
                Game1.getFarm().waterTiles[Game1.player.getTileX(), Game1.player.getTileY()] = true;
            }
        }

        private void CreateFlood()
        {
            Monitor.Log("Initiating flood calculations");
            foreach (GameLocation loc in Game1.locations)
            {
                if (loc is Farm)
                {
                    if (!(loc.waterTiles is null))
                    {
                        FloodedTiles.Add(loc, new Dictionary<int, List<Point>>()
                        {
                            {1, GenerateFloodMap(loc, 1, StopTheWater)},
                            {2, GenerateFloodMap(loc, 2, StopTheWater)},
                            {3, GenerateFloodMap(loc, 3, StopTheWater)},
                            {4, GenerateFloodMap(loc, 4, StopTheWater)},
                            {0, new List<Point>()}
                        });
                    }
                }
            }

            foreach (var kvp in FloodedTiles)
            {
                foreach (var k in kvp.Value)
                {
                    Console.Write($"{k.Key} list:");
                    for (int i = 0; i < k.Value.Count; i++)
                    {
                        Console.Write($" ({k.Value[i].X},{k.Value[i].Y})");
                    }
                    Console.WriteLine();
                }
            }

            CurrentFloodDepth = 4;

            Monitor.Log("Flooding maps");
            //flood the maps

            foreach (var kvp in FloodedTiles)
            {
                GameLocation loc = kvp.Key;
                var fPoints = kvp.Value[CurrentFloodDepth];
                
                for (int i = 0; i <  loc.waterTiles.GetLength(0); i++)
                {
                    for (int j = 0; j <  loc.waterTiles.GetLength(1); j++)
                    {
                        if (fPoints.Contains(new Point(i, j)))
                        {
                            Console.WriteLine($"Flipping tile [{i},{j}] to true");
                            loc.waterTiles[i, j] = true;
                            loc.map.GetLayer("Back").Tiles[i, j].Properties.Add("Water", "T");
                            
                            Vector2 currTile = new Vector2(i*16, j*16);

                            if (loc.terrainFeatures.ContainsKey(currTile) && (loc.terrainFeatures[currTile] is HoeDirt ||
                                                                              loc.terrainFeatures[currTile] is Grass ||
                                                                              (loc.terrainFeatures[currTile] is Tree t &&
                                                                               (t.growthStage.Value <= 3 ||
                                                                                t.health.Value <= 18f)) ||
                                                                              (loc.terrainFeatures[
                                                                                   currTile] is FruitTree tf &&
                                                                               (tf.growthStage.Value <= 3 ||
                                                                                tf.health.Value <= 18f))))
                            {
                                loc.terrainFeatures.Remove(currTile);
                            }

                            //now onto objects!!!
                            if (loc.objects.ContainsKey(currTile) &&
                                loc.objects[currTile] is StardewValley.Object fObj &&
                                (fObj.bigCraftable.Value || fObj.IsSpawnedObject) && !(fObj.canBePlacedInWater()) &&
                                !(fObj is Fence ff && (ff.whichType.Value != 2 || ff.whichType.Value != 5)))
                            {
                                loc.objects.Remove(currTile);
                            }
                        }

                    }
                }
            }
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            Console.WriteLine($"Current Flood Depth is {CurrentFloodDepth}");
            if (CurrentFloodDepth <= 0)
                return;

            Random r = new Random();
            //recede the water, spawn junk
            List<int> junkItems = new List<int>(){168,169,170,171,172,167,388,390,372,393};
            CurrentFloodDepth -= 1;
            foreach (var kvp in FloodedTiles)
            {
                GameLocation loc = kvp.Key;
                var rPoints = kvp.Value[CurrentFloodDepth+1];
                var fPoints = kvp.Value[CurrentFloodDepth];
                
                for (int i = 0; i <  loc.waterTiles.GetLength(0); i++)
                {
                    for (int j = 0; j <  loc.waterTiles.GetLength(1); j++)
                    {
                        if (rPoints.Contains(new Point(i, j)) && !fPoints.Contains(new Point(i, j)))
                        {
                            Console.WriteLine($"Flipping tile [{i},{j}] to false");
                            loc.waterTiles[i, j] = false;
                            loc.map.GetLayer("Back").Tiles[i, j].Properties.Remove("Water");
                            if (!loc.objects.ContainsKey(new Vector2(i,j)))
                                loc.objects.Add(new Vector2(i,j),new SObject(Vector2.Zero, junkItems[r.Next(0, junkItems.Count)],false));
                        }

                        if (fPoints.Contains(new Point(i, j)))
                        {
                            Console.WriteLine($"Flipping tile [{i},{j}] to true");
                            loc.waterTiles[i, j] = true;
                        }
                    }
                }
            }
        }
    }
}

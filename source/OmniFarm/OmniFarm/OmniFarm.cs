using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace OmniFarm
{
    public class OmniFarm : Mod
    {
        public class OmniFarmConfig
        {
            private List<Vector2> mineLocations = new List<Vector2>();
            private List<Vector2> grassLocations = new List<Vector2>();

            public List<Tuple<Vector2, Vector2>> mineAreas { get; set; } = new List<Tuple<Vector2, Vector2>>();
            public List<Tuple<Vector2, Vector2>> grassAreas { get; set; } = new List<Tuple<Vector2, Vector2>>();

            public List<Vector2> stumpLocations { get; set; } = new List<Vector2>();
            public List<Vector2> hollowLogLocations { get; set; } = new List<Vector2>();
            public List<Vector2> meteoriteLocations { get; set; } = new List<Vector2>();
            public List<Vector2> boulderLocations { get; set; } = new List<Vector2>();
            public List<Vector2> largeRockLocations { get; set; } = new List<Vector2>();

            public double oreChance { get; set; } = 0.05;
            public double gemChance { get; set; } = 0.01;

            public Vector2 WarpFromForest { get; set; } = new Vector2(32, 117);
            public Vector2 WarpFromBackWood { get; set; } = new Vector2(-1, -1);
            public Vector2 WarpFromBusStop { get; set; } = new Vector2(-1, -1);

            public OmniFarmConfig() { }

            public OmniFarmConfig Default()
            {
                //mine
                mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(89, 3), new Vector2(96, 7)));
                mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(97, 4), new Vector2(115, 10)));
                mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(91, 8), new Vector2(96, 8)));
                mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(92, 9), new Vector2(96, 9)));
                mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(93, 10), new Vector2(96, 10)));

                //grass
                grassAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(99, 73), new Vector2(115, 84)));
                grassAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(99, 96), new Vector2(115, 108)));

                //stump
                List<Vector2> stumpTemp = new List<Vector2>();
                AddVector2Grid(new Vector2(7, 24), new Vector2(7, 24), ref stumpTemp);
                AddVector2Grid(new Vector2(9, 26), new Vector2(9, 26), ref stumpTemp);
                AddVector2Grid(new Vector2(13, 27), new Vector2(13, 27), ref stumpTemp);
                stumpLocations = stumpTemp;

                //hollow log
                List<Vector2> hollowLogTemp = new List<Vector2>();
                AddVector2Grid(new Vector2(3, 23), new Vector2(3, 23), ref hollowLogTemp);
                AddVector2Grid(new Vector2(4, 26), new Vector2(4, 26), ref hollowLogTemp);
                AddVector2Grid(new Vector2(18, 28), new Vector2(18, 28), ref hollowLogTemp);
                hollowLogLocations = hollowLogTemp;

                //meterorite

                //boulder

                //large rock

                return this;
            }

            public List<Vector2> getMineLocations()
            {
                mineLocations.Clear();
                foreach (Tuple<Vector2, Vector2> T in mineAreas)
                {
                    AddVector2Grid(T.Item1, T.Item2, ref mineLocations);
                }
                return mineLocations;
            }

            public List<Vector2> getGrassLocations()
            {
                grassLocations.Clear();
                foreach (Tuple<Vector2, Vector2> T in grassAreas)
                {
                    AddVector2Grid(T.Item1, T.Item2, ref grassLocations);
                }
                return grassLocations;
            }
        }

        static public OmniFarmConfig ModConfig;
        public override void Entry(IModHelper helper)
        {
            ModConfig = helper.ReadJsonFile<OmniFarmConfig>("config.json");
            if (ModConfig == null)
            {
                ModConfig = helper.ReadConfig<OmniFarmConfig>().Default();
                helper.WriteConfig<OmniFarmConfig>(ModConfig);
            }

            StardewModdingAPI.Events.TimeEvents.AfterDayStarted += AfterDayStarted;
            
            /*
            StardewModdingAPI.Events.MineEvents.MineLevelChanged += (q, e) =>
            {
                if (ModConfig == null)
                    return;

                if ((Game1.currentLocation is MineShaft) == false)
                    return;

                List<Vector2> grassLocations = new List<Vector2>();
                AddVector2Grid(new Vector2(0, 0), new Vector2(50, 50), ref grassLocations);
                foreach (Vector2 tile in grassLocations)
                {
                    StardewValley.Object check;
                    if (Game1.currentLocation.objects.TryGetValue(tile, out check))
                    {
                        Log.Debug(check.name);
                        Log.Debug(check.bigCraftable);
                        Log.Debug(check.isOn);
                        Log.Debug(check.canBeGrabbed);
                        Log.Debug(check.canBeSetDown);
                        Log.Debug(check.parentSheetIndex);
                        Log.Debug(check.getHealth());
                        Log.Debug(check.fragility);
                        Log.Debug(check.type);
                        Log.Debug(check.GetType());
                    }
                }
            };
            */
        }

        static void AfterDayStarted(object sender, EventArgs e)
        {
            ChangeWarpPoints();

            if (ModConfig == null)
                return;

            foreach (GameLocation GL in Game1.locations)
            {
                if (GL is Farm)
                {
                    Farm ourFarm = (Farm)GL;
                    foreach (Vector2 tile in ModConfig.stumpLocations)
                    {
                        ourFarm.terrainFeatures.Remove(tile);
                        ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.stumpIndex, 2, 2, tile);
                    }

                    foreach (Vector2 tile in ModConfig.hollowLogLocations)
                    {
                        ourFarm.terrainFeatures.Remove(tile);
                        ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.hollowLogIndex, 2, 2, tile);
                    }

                    foreach (Vector2 tile in ModConfig.meteoriteLocations)
                    {
                        ourFarm.terrainFeatures.Remove(tile);
                        ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.meteoriteIndex, 2, 2, tile);
                    }

                    foreach (Vector2 tile in ModConfig.boulderLocations)
                    {
                        ourFarm.terrainFeatures.Remove(tile);
                        ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.boulderIndex, 2, 2, tile);
                    }

                    foreach (Vector2 tile in ModConfig.largeRockLocations)
                    {
                        ourFarm.terrainFeatures.Remove(tile);
                        ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.mineRock1Index, 2, 2, tile);
                    }
                    
                    //grass
                    if (Game1.IsWinter == false)
                        foreach (Vector2 tile in ModConfig.getGrassLocations())
                        {
                            ourFarm.terrainFeatures.Remove(tile);
                            ourFarm.terrainFeatures.Add(tile, new Grass(Grass.springGrass, 4));
                        }
                    
                    //mine
                    Random randomGen = new Random();
                    foreach (Vector2 tile in ModConfig.getMineLocations())
                    {
                        if (ourFarm.isObjectAt((int)tile.X, (int)tile.Y))
                            continue;

                        //calculate ore spawn
                        if (Game1.player.hasSkullKey)
                        {
                            //5% chance of spawn ore
                            if (randomGen.NextDouble() < ModConfig.oreChance)
                            {
                                addRandomOre(ref ourFarm, ref randomGen, 4, tile);
                                continue;
                            }
                        }
                        else
                        {
                            //check mine level
                            if (Game1.player.deepestMineLevel > 80) //gold level
                            {
                                if (randomGen.NextDouble() < ModConfig.oreChance)
                                {
                                    addRandomOre(ref ourFarm, ref randomGen, 3, tile);
                                    continue;
                                }
                            }
                            else if (Game1.player.deepestMineLevel > 40) //iron level
                            {
                                if (randomGen.NextDouble() < ModConfig.oreChance)
                                {
                                    addRandomOre(ref ourFarm, ref randomGen, 2, tile);
                                    continue;
                                }
                            }
                            else
                            {
                                if (randomGen.NextDouble() < ModConfig.oreChance)
                                {
                                    addRandomOre(ref ourFarm, ref randomGen, 1, tile);
                                    continue;
                                }
                            }
                        }

                        //if ore doesnt spawn then calculate gem spawn
                        //1% to spawn gem
                        if (randomGen.NextDouble() < ModConfig.gemChance)
                        {
                            //0.1% chance of getting mystic stone
                            if (Game1.player.hasSkullKey)
                                if (randomGen.Next(0, 100) < 1)
                                {
                                    ourFarm.setObject(tile, createOre("mysticStone", tile));
                                    continue;
                                }
                                else
                                if (randomGen.Next(0, 500) < 1)
                                {
                                    ourFarm.setObject(tile, createOre("mysticStone", tile));
                                    continue;
                                }

                            switch (randomGen.Next(0, 100) % 8)
                            {
                                case 0: ourFarm.setObject(tile, createOre("gemStone", tile)); break;
                                case 1: ourFarm.setObject(tile, createOre("diamond", tile)); break;
                                case 2: ourFarm.setObject(tile, createOre("ruby", tile)); break;
                                case 3: ourFarm.setObject(tile, createOre("jade", tile)); break;
                                case 4: ourFarm.setObject(tile, createOre("amethyst", tile)); break;
                                case 5: ourFarm.setObject(tile, createOre("topaz", tile)); break;
                                case 6: ourFarm.setObject(tile, createOre("emerald", tile)); break;
                                case 7: ourFarm.setObject(tile, createOre("aquamarine", tile)); break;
                                default: break;
                            }
                            continue;
                        }
                    }
                }
            }
        }
        
        static void ChangeWarpPoints()
        {
            foreach (GameLocation GL in Game1.locations)
            {
                if (ModConfig.WarpFromForest.X != -1)
                {
                    if (GL is Forest)
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)ModConfig.WarpFromForest.X;
                                w.TargetY = (int)ModConfig.WarpFromForest.Y;
                            }
                        }
                    }
                }
                    
                if (ModConfig.WarpFromBackWood.X != -1)
                {
                    if (GL.Name.ToLower().Contains("backwood"))
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)ModConfig.WarpFromBackWood.X;
                                w.TargetY = (int)ModConfig.WarpFromBackWood.Y;
                            }
                        }
                    }
                }

                if (ModConfig.WarpFromBusStop.X != -1)
                {
                    if (GL.Name.ToLower().Contains("busstop"))
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)ModConfig.WarpFromBusStop.X;
                                w.TargetY = (int)ModConfig.WarpFromBusStop.Y;
                            }
                        }
                    }
                }
            }
        }
        
        static void addRandomOre(ref Farm input, ref Random randomGen, int highestOreLevel, Vector2 tileLocation)
        {
            switch (randomGen.Next(0, 100) % highestOreLevel)
            {
                case 0: input.setObject(tileLocation, createOre("copperStone", tileLocation)); break;
                case 1: input.setObject(tileLocation, createOre("ironStone", tileLocation)); break;
                case 2: input.setObject(tileLocation, createOre("goldStone", tileLocation)); break;
                case 3: input.setObject(tileLocation, createOre("iridiumStone", tileLocation)); break;
                default: break;
            }
        }

        static StardewValley.Object createOre(string oreName, Vector2 tileLocation)
        {
            switch (oreName)
            {
                case "mysticStone": return new StardewValley.Object(tileLocation, 46, "Stone", true, false, false, false);
                case "gemStone": return new StardewValley.Object(tileLocation, (Game1.random.Next(7) + 1) * 2, "Stone", true, false, false, false);
                case "diamond": return new StardewValley.Object(tileLocation, 2, "Stone", true, false, false, false);
                case "ruby": return new StardewValley.Object(tileLocation, 4, "Stone", true, false, false, false);
                case "jade": return new StardewValley.Object(tileLocation, 6, "Stone", true, false, false, false);
                case "amethyst": return new StardewValley.Object(tileLocation, 8, "Stone", true, false, false, false);
                case "topaz": return new StardewValley.Object(tileLocation, 10, "Stone", true, false, false, false);
                case "emerald": return new StardewValley.Object(tileLocation, 12, "Stone", true, false, false, false);
                case "aquamarine": return new StardewValley.Object(tileLocation, 14, "Stone", true, false, false, false);
                case "iridiumStone": return new StardewValley.Object(tileLocation, 765, 1);
                case "goldStone": return new StardewValley.Object(tileLocation, 764, 1);
                case "ironStone": return new StardewValley.Object(tileLocation, 290, 1);
                case "copperStone": return new StardewValley.Object(tileLocation, 751, 1);
                default: return null;
            }
        }
        
        static void ClearKey(ref SerializableDictionary<Vector2, TerrainFeature> input, Vector2 KeyToClear)
        {
            if (input.Remove(KeyToClear))
            {
                ClearKey(ref input, KeyToClear);
            }
        }

        static void AddVector2Grid(Vector2 TopLeftTile, Vector2 BottomRightTile, ref List<Vector2> grid)
        {
            if (TopLeftTile == BottomRightTile)
            {
                grid.Add(TopLeftTile);
                return;
            }

            int i = (int)TopLeftTile.X;
            while (i <= (int)BottomRightTile.X)
            {
                int j = (int)TopLeftTile.Y;
                while (j <= (int)BottomRightTile.Y)
                {
                    grid.Add(new Vector2(i, j));
                    j++;
                }
                i++;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace OmniFarm
{
    public class OmniFarm : Mod, IAssetLoader
    {
        /*********
        ** Fields
        *********/
        private OmniFarmConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<OmniFarmConfig>();

            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return
                asset.AssetNameEquals(@"Maps\Farm_Combat")
                || (Config.useOptionalCave && asset.AssetNameEquals(@"Maps\FarmCave"));
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(@"Maps\Farm_Combat"))
                return this.Helper.Content.Load<T>(@"assets\Farm_Combat.tbin");
            if (asset.AssetNameEquals(@"Maps\FarmCave"))
                return this.Helper.Content.Load<T>(@"assets\FarmCave.tbin");
            throw new NotSupportedException($"Unknown asset {asset.AssetName}");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, EventArgs e)
        {
            if (Game1.whichFarm == Farm.combat_layout)
            {
                ChangeWarpPoints();

                foreach (GameLocation GL in Game1.locations)
                {
                    if (GL is Farm ourFarm)
                    {
                        foreach (Vector2 tile in Config.stumpLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.stumpIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in Config.hollowLogLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.hollowLogIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in Config.meteoriteLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.meteoriteIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in Config.boulderLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.boulderIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in Config.largeRockLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.mineRock1Index, 2, 2, tile);
                        }

                        //grass
                        if (Game1.IsWinter == false)
                            foreach (Vector2 tile in Config.getGrassLocations())
                            {
                                if (ourFarm.terrainFeatures.TryGetValue(tile, out TerrainFeature check))
                                {
                                    if (check is Grass grass)
                                        grass.numberOfWeeds.Value = Config.GrassGrowth_1forsparse_4forFull;
                                }
                                else
                                    ourFarm.terrainFeatures.Add(tile, new Grass(Grass.springGrass, Config.GrassGrowth_1forsparse_4forFull));
                            }

                        //mine
                        Random randomGen = new Random();
                        foreach (Vector2 tile in Config.getMineLocations())
                        {
                            if (ourFarm.isObjectAt((int)tile.X, (int)tile.Y))
                                continue;

                            //calculate ore spawn
                            if (Game1.player.hasSkullKey)
                            {
                                //5% chance of spawn ore
                                if (randomGen.NextDouble() < Config.oreChance)
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
                                    if (randomGen.NextDouble() < Config.oreChance)
                                    {
                                        addRandomOre(ref ourFarm, ref randomGen, 3, tile);
                                        continue;
                                    }
                                }
                                else if (Game1.player.deepestMineLevel > 40) //iron level
                                {
                                    if (randomGen.NextDouble() < Config.oreChance)
                                    {
                                        addRandomOre(ref ourFarm, ref randomGen, 2, tile);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (randomGen.NextDouble() < Config.oreChance)
                                    {
                                        addRandomOre(ref ourFarm, ref randomGen, 1, tile);
                                        continue;
                                    }
                                }
                            }

                            //if ore doesn't spawn then calculate gem spawn
                            //1% to spawn gem
                            if (randomGen.NextDouble() < Config.gemChance)
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
                                }
                                continue;
                            }
                        }
                    }
                }
            }
        }

        private void ChangeWarpPoints()
        {
            foreach (GameLocation GL in Game1.locations)
            {
                if (Config.WarpFromForest.X != -1)
                {
                    if (GL is Forest)
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)Config.WarpFromForest.X;
                                w.TargetY = (int)Config.WarpFromForest.Y;
                            }
                        }
                    }
                }

                if (Config.WarpFromBackWood.X != -1)
                {
                    if (GL.Name.ToLower().Contains("backwood"))
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)Config.WarpFromBackWood.X;
                                w.TargetY = (int)Config.WarpFromBackWood.Y;
                            }
                        }
                    }
                }

                if (Config.WarpFromBusStop.X != -1)
                {
                    if (GL.Name.ToLower().Contains("busstop"))
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)Config.WarpFromBusStop.X;
                                w.TargetY = (int)Config.WarpFromBusStop.Y;
                            }
                        }
                    }
                }
            }
        }

        private void addRandomOre(ref Farm input, ref Random randomGen, int highestOreLevel, Vector2 tileLocation)
        {
            switch (randomGen.Next(0, 100) % highestOreLevel)
            {
                case 0: input.setObject(tileLocation, createOre("copperStone", tileLocation)); break;
                case 1: input.setObject(tileLocation, createOre("ironStone", tileLocation)); break;
                case 2: input.setObject(tileLocation, createOre("goldStone", tileLocation)); break;
                case 3: input.setObject(tileLocation, createOre("iridiumStone", tileLocation)); break;
            }
        }

        private SObject createOre(string oreName, Vector2 tileLocation)
        {
            switch (oreName)
            {
                case "mysticStone": return new SObject(tileLocation, 46, "Stone", true, false, false, false);
                case "gemStone": return new SObject(tileLocation, (Game1.random.Next(7) + 1) * 2, "Stone", true, false, false, false);
                case "diamond": return new SObject(tileLocation, 2, "Stone", true, false, false, false);
                case "ruby": return new SObject(tileLocation, 4, "Stone", true, false, false, false);
                case "jade": return new SObject(tileLocation, 6, "Stone", true, false, false, false);
                case "amethyst": return new SObject(tileLocation, 8, "Stone", true, false, false, false);
                case "topaz": return new SObject(tileLocation, 10, "Stone", true, false, false, false);
                case "emerald": return new SObject(tileLocation, 12, "Stone", true, false, false, false);
                case "aquamarine": return new SObject(tileLocation, 14, "Stone", true, false, false, false);
                case "iridiumStone": return new SObject(tileLocation, 765, 1);
                case "goldStone": return new SObject(tileLocation, 764, 1);
                case "ironStone": return new SObject(tileLocation, 290, 1);
                case "copperStone": return new SObject(tileLocation, 751, 1);
                default: return null;
            }
        }

        private void ClearResourceClump(IList<ResourceClump> input, Vector2 tile)
        {
            for (int i = 0; i < input.Count; i++)
            {
                ResourceClump RC = input[i];
                if (RC.tile.Value == tile)
                {
                    input.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

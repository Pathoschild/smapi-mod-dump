using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        private static partial class ObjectSpawner
        {
            /// <summary>Generates ore in the game based on the current player's config settings.</summary>
            public static void OreGeneration()
            {
                foreach (FarmData data in Utility.FarmDataList)
                {
                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"Starting ore generation for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"Starting ore generation for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }

                    if (data.Config.OreSpawnEnabled)
                    {
                        Utility.Monitor.Log("Ore spawn is enabled. Starting generation process...", LogLevel.Trace);
                    }
                    else
                    {
                        Utility.Monitor.Log("Ore spawn is disabled.", LogLevel.Trace);
                        continue;
                    }

                    foreach (OreSpawnArea area in data.Config.Ore_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking ore settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate the map name for the area
                        if (Game1.getLocationFromName(area.MapName) == null)
                        {
                            Utility.Monitor.Log($"Issue: No map named \"{area.MapName}\" could be found. No ore will be spawned there.", LogLevel.Info);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area ({area.MapName}). Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("All extra conditions met. Generating list of valid tiles...", LogLevel.Trace);

                        List<Vector2> validTiles = Utility.GenerateTileList(area, data.Save, data.Config.QuarryTileIndex, data.Config.Ore_Spawn_Settings.CustomTileIndex, false); //calculate a list of valid tiles for ore in this area

                        Utility.Monitor.Log($"Number of valid tiles: {validTiles.Count}. Deciding how many items to spawn...", LogLevel.Trace);

                        //calculate how much ore to spawn today
                        int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, data.Config.Ore_Spawn_Settings.PercentExtraSpawnsPerMiningLevel, Utility.Skills.Mining);

                        Utility.Monitor.Log($"Items to spawn: {spawnCount}. Determining spawn chances for ore...", LogLevel.Trace);

                        //figure out which config section to use (if the spawn area's data is null, use the "global" data instead)
                        Dictionary<string, int> skillReq = area.MiningLevelRequired ?? data.Config.Ore_Spawn_Settings.MiningLevelRequired;
                        Dictionary<string, int> startChance = area.StartingSpawnChance ?? data.Config.Ore_Spawn_Settings.StartingSpawnChance;
                        Dictionary<string, int> tenChance = area.LevelTenSpawnChance ?? data.Config.Ore_Spawn_Settings.LevelTenSpawnChance;
                        //also use the "global" data if the area data is non-null but empty (which can happen accidentally when the json file is manually edited)
                        if (skillReq.Count < 1)
                        {
                            skillReq = data.Config.Ore_Spawn_Settings.MiningLevelRequired;
                        }
                        if (startChance.Count < 1)
                        {
                            startChance = data.Config.Ore_Spawn_Settings.StartingSpawnChance;
                        }
                        if (tenChance.Count < 1)
                        {
                            tenChance = data.Config.Ore_Spawn_Settings.LevelTenSpawnChance;
                        }

                        //calculate the final spawn chance for each type of ore
                        Dictionary<string, int> oreChances = Utility.AdjustedSpawnChances(Utility.Skills.Mining, skillReq, startChance, tenChance);

                        if (oreChances.Count < 1) //if there's no chance of spawning any ore for some reason, just stop working on this area now
                        {
                            Utility.Monitor.Log("No chance of spawning any ore. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log($"Spawn chances complete. Beginning spawn process...", LogLevel.Trace);

                        //begin to spawn ore
                        int randomIndex;
                        Vector2 randomTile;
                        int randomOreNum;
                        while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for ore & still ore to be spawned
                        {
                            //this section spawns 1 ore at a random valid location

                            spawnCount--; //reduce by 1, since one will be spawned
                            randomIndex = Utility.RNG.Next(validTiles.Count); //get the array index for a random tile
                            randomTile = validTiles[randomIndex]; //get the tile's x,y coordinates
                            validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be obstructed now

                            int totalWeight = 0; //the upper limit for the random number that picks ore type (i.e. the sum of all ore chances)
                            foreach (KeyValuePair<string, int> ore in oreChances)
                            {
                                totalWeight += ore.Value; //sum up all the ore chances
                            }
                            randomOreNum = Utility.RNG.Next(totalWeight); //generate random number from 0 to [totalWeight - 1]
                            foreach (KeyValuePair<string, int> ore in oreChances)
                            {
                                if (randomOreNum < ore.Value) //this ore "wins"
                                {
                                    int? oreID = Utility.SpawnOre(ore.Key, Game1.getLocationFromName(area.MapName), randomTile); //spawn ore & get its index ID

                                    if (oreID != null && area.DaysUntilSpawnsExpire != null) //if oreID exists & if this area assigns expiration dates to ore
                                    {
                                        SavedObject saved = new SavedObject(area.MapName, randomTile, SavedObject.ObjectType.Ore, oreID.Value, ore.Key, area.DaysUntilSpawnsExpire); //create a record of the newly spawned ore
                                        data.Save.SavedObjects.Add(saved); //add it to the save file with the area's expiration setting
                                    }

                                    break;
                                }
                                else //this ore "loses"
                                {
                                    randomOreNum -= ore.Value; //subtract this ore's chance from the random number before moving to the next one
                                }
                            }
                        }

                        Utility.Monitor.Log($"Ore spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Ore spawn complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Ore spawn complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Ore spawn process complete.", LogLevel.Trace);
            }
        }
    }
}

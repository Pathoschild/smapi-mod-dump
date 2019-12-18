using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        private static partial class Generation
        {
            /// <summary>Generates monsters in the game based on the current player's config settings.</summary>
            public static void MonsterGeneration()
            {
                foreach (FarmData data in Utility.FarmDataList)
                {
                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"Starting monster generation for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"Starting monster generation for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }

                    if (data.Config.Monster_Spawn_Settings != null) //if this config contains monster settings
                    {
                        if (data.Config.MonsterSpawnEnabled)
                        {
                            Utility.Monitor.Log("Monster generation is enabled. Starting generation process...", LogLevel.Trace);
                        }
                        else
                        {
                            Utility.Monitor.Log($"Monster generation is disabled for this {(data.Pack == null ? "file" : "content pack")}.", LogLevel.Trace);
                            continue;
                        }
                    }
                    else //if this config's monster settings are null
                    {
                        Utility.Monitor.Log($"This {(data.Pack == null ? "file" : "content pack")}'s monster spawn settings are blank.", LogLevel.Trace);
                        continue;
                    }

                    foreach (MonsterSpawnArea area in data.Config.Monster_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking monster settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate the map name for the area
                        List<GameLocation> locations = Utility.GetAllLocationsFromName(area.MapName); //get all locations for this map name
                        if (locations.Count == 0) //if no locations were found
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. Monsters won't be spawned there.", LogLevel.Trace);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log($"All extra conditions met. Validating list of monster types...", LogLevel.Trace);

                        //validate the provided monster types
                        List<MonsterType> validMonsterTypes = Utility.ValidateMonsterTypes(area.MonsterTypes, area.UniqueAreaID);

                        if (validMonsterTypes.Count <= 0)
                        {
                            Utility.Monitor.Log($"No monster types were curently valid for this area. Skipping to the next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log($"Monster type validation complete. Beginning generation process...", LogLevel.Trace);

                        for (int x = 0; x < locations.Count; x++) //for each location matching this area's map name
                        {
                            //calculate how many monsters to spawn today
                            int spawnCount = Utility.RNG.Next(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay + 1); //random number from min to max

                            if (locations.Count > 1) //if this area targets multiple locations
                            {
                                Utility.Monitor.Log($"Potential spawns at {locations[x].Name} #{x + 1}: {spawnCount}.", LogLevel.Trace);
                            }
                            else //if this area only targets one location
                            {
                                Utility.Monitor.Log($"Potential spawns at {locations[x].Name}: {spawnCount}.", LogLevel.Trace);
                            }

                            List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned

                            //begin to generate monsters
                            while (spawnCount > 0) //while more monsters should be spawned
                            {
                                spawnCount--;

                                //get the total spawn weight of valid monster types
                                int totalWeight = 0;
                                foreach (MonsterType type in validMonsterTypes) //for each valid monster type
                                {
                                    if (type.Settings.ContainsKey("SpawnWeight")) //if a custom spawn weight was provided
                                    {
                                        totalWeight += Convert.ToInt32(type.Settings["SpawnWeight"]);
                                    }
                                    else //if no spawn weight was provided
                                    {
                                        totalWeight += 1;
                                    }
                                }

                                //select a random monster using spawn weights
                                MonsterType randomMonster = null;
                                int random = Utility.RNG.Next(0, totalWeight); //get a random integer from 0 to (totalWeight - 1)

                                for (int m = 0; m < validMonsterTypes.Count; m++) //for each valid monster type
                                {
                                    int spawnWeight = 1; //default to 1
                                    if (validMonsterTypes[m].Settings.ContainsKey("SpawnWeight")) //if a spawn weight was provided
                                    {
                                        spawnWeight = Convert.ToInt32(validMonsterTypes[m].Settings["SpawnWeight"]); //use it
                                    }

                                    if (random < spawnWeight) //if this monster type is selected
                                    {
                                        randomMonster = Utility.Clone(validMonsterTypes[m]); //get the selected monster type (cloned for later use as a unique instance)
                                        break;
                                    }
                                    else //if this monster type is not selected
                                    {
                                        random -= spawnWeight; //subtract this item's weight from the random number
                                    }
                                }

                                //create a saved object representing this spawn (with a "blank" tile location)
                                SavedObject saved = new SavedObject()
                                {
                                    MapName = locations[x].uniqueName.Value ?? locations[x].Name,
                                    Type = SavedObject.ObjectType.Monster,
                                    DaysUntilExpire = area.DaysUntilSpawnsExpire ?? 1,
                                    MonType = randomMonster
                                };
                                spawns.Add(saved); //add it to the list
                            }

                            Utility.PopulateTimedSpawnList(spawns, data, area); //process the listed spawns and add them to Utility.TimedSpawns
                        }

                        Utility.Monitor.Log($"Monster spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Monster generation complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Monster generation complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Monster generation process complete.", LogLevel.Trace);
            }
        }
    }
}

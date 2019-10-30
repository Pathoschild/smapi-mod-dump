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
        private static partial class Generation
        {
            /// <summary>Generates large objects (e.g. stumps and logs) in the game based on the current player's config settings.</summary>
            public static void LargeObjectGeneration()
            {
                foreach (FarmData data in Utility.FarmDataList)
                {
                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"Starting large object generation for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"Starting large object generation for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }

                    if (data.Config.Large_Object_Spawn_Settings != null) //if this config contains large object settings
                    {
                        if (data.Config.LargeObjectSpawnEnabled)
                        {
                            Utility.Monitor.Log("Large object generation is enabled. Starting generation process...", LogLevel.Trace);
                        }
                        else
                        {
                            Utility.Monitor.Log($"Large object generation is disabled for this {(data.Pack == null ? "file" : "content pack")}.", LogLevel.Trace);
                            continue;
                        }
                    }
                    else //if this config's large object settings are null
                    {
                        Utility.Monitor.Log($"This {(data.Pack == null ? "file" : "content pack")}'s large object spawn settings are blank.", LogLevel.Trace);
                        continue;
                    }

                    foreach (LargeObjectSpawnArea area in data.Config.Large_Object_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking large object settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate map name for the area
                        if (!(Game1.getLocationFromName(area.MapName) is Farm loc)) //if the provided map is not a farm (or is null)
                        {
                            //non-farm maps generally don't support resource clumps (a.k.a. large objects) properly, so display an error message and skip this area
                            Utility.Monitor.Log($"The map named \"{area.MapName}\" could not be found OR is not a \"farm\" map type. Large objects won't be spawned there.", LogLevel.Trace);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("All extra conditions met. Checking map's support for large objects...", LogLevel.Trace);

                        Utility.Monitor.Log("Current map supports large objects. Checking the Find Existing Objects setting...", LogLevel.Trace);

                        List<int> objectIDs = Utility.GetLargeObjectIDs(area.ObjectTypes); //get a list of index numbers for relevant object types in this area

                        //find the locations any existing objects (of the listed types)
                        if (area.FindExistingObjectLocations == true) //if enabled 
                        {
                            if (data.Save.ExistingObjectLocations.ContainsKey(area.UniqueAreaID)) //if this config+farm already has a list of existing objects (even if it's blank)
                            {
                                Utility.Monitor.Log("Find Existing Objects enabled. Using save file data from a previous search.", LogLevel.Trace);
                            }
                            else //if this config+farm hasn't been checked for existing objects yet 
                            {
                                Utility.Monitor.Log("Find Existing Objects enabled. Finding...", LogLevel.Trace);

                                List<string> existingObjects = new List<string>(); //any new object location strings to be added to area.IncludeAreas

                                foreach (ResourceClump clump in loc.resourceClumps) //go through the map's set of resource clumps (stumps, logs, etc)
                                {
                                    bool validObjectType = false; //whether the current object is listed in this area's config
                                    foreach (int ID in objectIDs) //check the list of valid index numbers for this area
                                    {
                                        if (clump.parentSheetIndex.Value == ID)
                                        {
                                            validObjectType = true; //this clump's ID matches one of the listed object IDs
                                            break;
                                        }
                                    }
                                    if (validObjectType == false) //if this clump isn't listed in the config
                                    {
                                        continue; //skip to the next clump
                                    }

                                    string newInclude = $"{clump.tile.X},{clump.tile.Y};{clump.tile.X},{clump.tile.Y}"; //generate an include string for this tile
                                    bool alreadyListed = false; //whether newInclude is already listed in area.IncludeAreas

                                    foreach (string include in area.IncludeCoordinates) //check each existing include string
                                    {
                                        if (include == newInclude)
                                        {
                                            alreadyListed = true; //this tile is already specifically listed
                                            break;
                                        }
                                    }

                                    if (!alreadyListed) //if this object isn't already specifically listed in the include areas
                                    {
                                        existingObjects.Add(newInclude); //add the string to the list of new include strings
                                    }
                                }

                                Utility.Monitor.Log($"Existing objects found: {existingObjects.Count}.", LogLevel.Trace);

                                data.Save.ExistingObjectLocations.Add(area.UniqueAreaID, existingObjects.ToArray()); //add the new strings to the save data for the current config+farm
                            }
                        }
                        else
                        {
                            Utility.Monitor.Log("Find Existing Objects disabled. Skipping.", LogLevel.Trace);
                        }

                        //calculate how many objects to spawn today
                        int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, area.PercentExtraSpawnsPerSkillLevel, (Utility.Skills)Enum.Parse(typeof(Utility.Skills), area.RelatedSkill, true));

                        Utility.Monitor.Log($"Calculating potential spawns: {spawnCount}. Beginning generation process...", LogLevel.Trace);

                        List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned

                        //begin to generate objects
                        while (spawnCount > 0) //while more objects should be spawned
                        {
                            spawnCount--;

                            int randomObject = objectIDs[Utility.RNG.Next(objectIDs.Count)]; //get a random object ID to spawn

                            //create a saved object representing this spawn (with a "blank" tile location)
                            SavedObject saved = new SavedObject(area.MapName, new Vector2(), SavedObject.ObjectType.LargeObject, randomObject, null, area.DaysUntilSpawnsExpire);
                            spawns.Add(saved); //add it to the list
                        }

                        Utility.PopulateTimedSpawnList(spawns, data, area); //process the listed spawns and add them to Utility.TimedSpawns

                        Utility.Monitor.Log($"Large object generation complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Large object generation complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Large object generation complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Large object generation process complete.", LogLevel.Trace);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
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

                        //validate the map name for the area
                        List<GameLocation> locations = Utility.GetAllLocationsFromName(area.MapName); //get all locations for this map name
                        if (locations.Count == 0) //if no locations were found
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. Forage won't be spawned there.", LogLevel.Trace);
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
                        if (area.FindExistingObjectLocations == true && locations.Count == 1) //if enabled & exactly one location was found (TODO: rework or fix this system to support multiple locations per area)
                        {
                            if (data.Save.ExistingObjectLocations.ContainsKey(area.UniqueAreaID)) //if this area already has a list of existing objects (even if it's blank)
                            {
                                Utility.Monitor.Log("Find Existing Objects enabled. Using save file data from a previous search.", LogLevel.Trace);
                            }
                            else //if this config+farm hasn't been checked for existing objects yet 
                            {
                                Utility.Monitor.Log("Find Existing Objects enabled. Finding...", LogLevel.Trace);

                                List<string> existingObjects = new List<string>(); //any new object location strings to be added to area.IncludeAreas

                                IEnumerable<TerrainFeature> resourceClumps = null; //a list of large objects at this location
                                if (locations[0] is Farm farm)
                                {
                                    resourceClumps = farm.resourceClumps.ToList(); //use the farm's clump list
                                }
                                else if (locations[0] is MineShaft mine)
                                {
                                    resourceClumps = mine.resourceClumps.ToList(); //use the mine's clump list
                                }
                                else
                                {
                                    resourceClumps = locations[0].largeTerrainFeatures.OfType<LargeResourceClump>(); //use this location's large resource clump list
                                }

                                foreach (TerrainFeature clump in resourceClumps) //for each of this location's large objects
                                {
                                    string newInclude = "";

                                    bool validObjectID = false; //whether this clump's ID is listed in this area's config
                                    foreach (int ID in objectIDs) //for each valid object ID for this area
                                    {
                                        if (clump is ResourceClump smallClump && smallClump.parentSheetIndex.Value == ID) //if this clump's ID matches one of the listed object IDs
                                        {
                                            validObjectID = true;
                                            newInclude = $"{smallClump.tile.X},{smallClump.tile.Y};{smallClump.tile.X},{smallClump.tile.Y}"; //generate an include string for this clump's tile
                                            break;
                                        }
                                        else if (clump is LargeResourceClump largeClump && largeClump.Clump.Value.parentSheetIndex.Value == ID) //if this large clump's ID matches one of the listed object IDs
                                        {
                                            validObjectID = true;
                                            newInclude = $"{largeClump.Clump.Value.tile.X},{largeClump.Clump.Value.tile.Y};{largeClump.Clump.Value.tile.X},{largeClump.Clump.Value.tile.Y}"; //generate an include string for this large clump's tile
                                            break;
                                        }
                                    }
                                    if (validObjectID == false) //if this clump's ID isn't listed in the config
                                    {
                                        continue; //skip to the next clump
                                    }
                                    
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
                        else //if this setting is disabled or multiple maps prevent enabling it
                        {
                            if (!area.FindExistingObjectLocations) //if this setting is disabled
                            {
                                Utility.Monitor.Log("Find Existing Objects disabled. Skipping.", LogLevel.Trace);
                            }
                            else //if this was caused by map limitations
                            {
                                Utility.Monitor.Log("Find Existing Objects does not currently support map names that target multiple locations (e.g. building types). The setting will be ignored.", LogLevel.Debug);
                                Utility.Monitor.Log($"Affected area: {area.UniqueAreaID}", LogLevel.Debug);
                                Utility.Monitor.Log($"Map name: {area.MapName}", LogLevel.Debug);
                            }
                        }

                        Utility.Monitor.Log($"Beginning generation process...", LogLevel.Trace);

                        for (int x = 0; x < locations.Count; x++) //for each location matching this area's map name
                        {
                            //calculate how many objects to spawn today
                            int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, area.PercentExtraSpawnsPerSkillLevel, (Utility.Skills)Enum.Parse(typeof(Utility.Skills), area.RelatedSkill, true));

                            if (locations.Count > 1) //if this area targets multiple locations
                            {
                                Utility.Monitor.Log($"Potential spawns at {locations[x].Name} #{x + 1}: {spawnCount}.", LogLevel.Trace);
                            }
                            else //if this area only targets one location
                            {
                                Utility.Monitor.Log($"Potential spawns at {locations[x].Name}: {spawnCount}.", LogLevel.Trace);
                            }

                            //begin to generate large objects
                            List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned
                            while (spawnCount > 0) //while more objects should be spawned
                            {
                                spawnCount--;

                                int randomObject = objectIDs[Utility.RNG.Next(objectIDs.Count)]; //get a random object ID to spawn

                                //create a saved object representing this spawn (with a "blank" tile location)
                                SavedObject saved = new SavedObject(area.MapName, new Vector2(), SavedObject.ObjectType.LargeObject, randomObject, null, area.DaysUntilSpawnsExpire ?? 0);
                                spawns.Add(saved); //add it to the list
                            }

                            Utility.PopulateTimedSpawnList(spawns, data, area); //process the listed spawns and add them to Utility.TimedSpawns
                        }

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

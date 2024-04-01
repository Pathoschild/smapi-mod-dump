/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

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
                        List<string> locations = Utility.GetAllLocationsFromName(area.MapName); //get all locations for this map name
                        if (locations.Count == 0) //if no locations were found
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. Forage won't be spawned there.", LogLevel.Trace);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save, data.Pack?.Manifest) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("All extra conditions met. Checking map's support for large objects...", LogLevel.Trace);

                        Utility.Monitor.Log("Current map supports large objects. Checking the Find Existing Objects setting...", LogLevel.Trace);

                        List<string> objectIDs = Utility.GetLargeObjectIDs(area.ObjectTypes, area.UniqueAreaID); //get a list of index numbers for this area's object types

                        if (objectIDs.Count <= 0)
                        {
                            Utility.Monitor.Log($"Large object list contained no valid object types. Skipping to the next large object area...", LogLevel.Trace);
                            continue;
                        }

                        //find the locations any existing objects (of the listed types)
                        if (area.FindExistingObjectLocations == true //if enabled
                            && locations.Count == 1 //AND only one location was found (building interiors not currently supported)
                            && !locations[0].StartsWith("UndergroundMine", StringComparison.OrdinalIgnoreCase) //AND it's not a mine level
                            && !locations[0].StartsWith("VolcanoDungeon", StringComparison.OrdinalIgnoreCase) //AND it's not a volcano level
                            )
                        {
                            if (data.Save.ExistingObjectLocations.ContainsKey(area.UniqueAreaID)) //if this area already has a list of existing objects (even if it's blank)
                            {
                                Utility.Monitor.Log("Find Existing Objects enabled. Using save file data from a previous search.", LogLevel.Trace);
                            }
                            else //if this config+farm hasn't been checked for existing objects yet 
                            {
                                Utility.Monitor.Log("Find Existing Objects enabled. Finding...", LogLevel.Trace);

                                HashSet<int> idsToFind = new HashSet<int>(); //integer IDs for large objects spawned by this area (note: this intentionally excludes giant crops and other subclasses)
                                foreach (string id in objectIDs)
                                {
                                    if (int.TryParse(id, out var intID))
                                        idsToFind.Add(intID);
                                }

                                List<string> existingObjects = new List<string>(); //any new object location strings to be added to area.IncludeAreas

                                foreach (ResourceClump clump in Game1.getLocationFromName(locations[0]).resourceClumps) //for each large object at this location
                                {
                                    if (idsToFind.Contains(clump.parentSheetIndex.Value)) //if its ID matches one spawned by this area
                                    {
                                        existingObjects.Add($"{clump.Tile.X},{clump.Tile.Y};{clump.Tile.X},{clump.Tile.Y}"); //create a string for its tile ("x,y;x,y") and add it to the list
                                    }
                                }

                                Utility.Monitor.Log($"Existing objects saved as spawn locations for this area: {existingObjects.Count}.", LogLevel.Trace);

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
                                Utility.Monitor.Log("Find Existing Objects does not currently support this map type. The setting will be ignored.", LogLevel.Debug);
                                Utility.Monitor.Log($"Affected area: {area.UniqueAreaID}", LogLevel.Debug);
                                Utility.Monitor.Log($"Map name: {area.MapName}", LogLevel.Debug);
                            }
                        }

                        Utility.Monitor.Log($"Beginning generation process...", LogLevel.Trace);

                        for (int x = 0; x < locations.Count; x++) //for each location matching this area's map name
                        {
                            //calculate how many objects to spawn today
                            int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, area.PercentExtraSpawnsPerSkillLevel, (Utility.Skills)Enum.Parse(typeof(Utility.Skills), area.RelatedSkill, true));

                            Utility.Monitor.Log($"Potential spawns at {locations[x]}: {spawnCount}.", LogLevel.Trace);

                            //begin to generate large objects
                            List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned
                            while (spawnCount > 0) //while more objects should be spawned
                            {
                                spawnCount--;

                                string randomObject = objectIDs[Utility.RNG.Next(objectIDs.Count)]; //get a random object ID to spawn

                                SavedObject saved = new SavedObject() //create a saved object representing this spawn (with a "blank" tile location)
                                {
                                    MapName = locations[x],
                                    Type = SavedObject.ObjectType.LargeObject,
                                    ID = randomObject,
                                    DaysUntilExpire = area.DaysUntilSpawnsExpire ?? 0
                                };
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

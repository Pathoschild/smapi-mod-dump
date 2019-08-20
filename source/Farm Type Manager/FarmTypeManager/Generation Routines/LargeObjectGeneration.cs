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

                    if (data.Config.LargeObjectSpawnEnabled)
                    {
                        Utility.Monitor.Log("Large object spawn is enabled. Starting generation process...", LogLevel.Trace);
                    }
                    else
                    {
                        Utility.Monitor.Log("Large object spawn is disabled.", LogLevel.Trace);
                        continue;
                    }

                    foreach (LargeObjectSpawnArea area in data.Config.Large_Object_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking large object settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate the map name for the area
                        if (Game1.getLocationFromName(area.MapName) == null)
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. Large objects won't be spawned there.", LogLevel.Info);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("All extra conditions met. Checking map's support for large objects...", LogLevel.Trace);

                        Farm loc = Game1.getLocationFromName(area.MapName) as Farm; //variable for the current location being worked on (NOTE: null if the current location isn't a "farm" map)
                        if (loc == null) //if this area isn't a "farm" map, there's usually no built-in support for resource clumps (i.e. large objects), so display an error message and skip this area
                        {
                            Utility.Monitor.Log($"Large objects cannot be spawned in the \"{area.MapName}\" map. Only \"farm\" map types are currently supported.", LogLevel.Info);
                            continue;
                        }

                        Utility.Monitor.Log("Current map supports large objects. Generating list of valid tiles...", LogLevel.Trace);

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

                                    foreach (string include in area.IncludeAreas) //check each existing include string
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

                        List<Vector2> validTiles = Utility.GenerateTileList(area, data.Save, data.Config.QuarryTileIndex, data.Config.Large_Object_Spawn_Settings.CustomTileIndex, true); //calculate a list of valid tiles for large objects in this area

                        Utility.Monitor.Log($"Number of valid tiles: {validTiles.Count}. Deciding how many items to spawn...", LogLevel.Trace);

                        //calculate how many objects to spawn today
                        int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, area.PercentExtraSpawnsPerSkillLevel, (Utility.Skills)Enum.Parse(typeof(Utility.Skills), area.RelatedSkill, true));

                        Utility.Monitor.Log($"Items to spawn: {spawnCount}. Beginning spawn process...", LogLevel.Trace);

                        //begin to spawn objects
                        while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for objects & still objects to be spawned
                        {
                            //this section spawns 1 large object at a random valid location

                            spawnCount--; //reduce by 1, since one will be spawned

                            int randomIndex;
                            Vector2 randomTile;
                            bool tileConfirmed = false; //false until a valid large (2x2) object location is confirmed
                            do
                            {
                                randomIndex = Utility.RNG.Next(validTiles.Count); //get the array index for a random valid tile
                                randomTile = validTiles[randomIndex]; //get the random tile's x,y coordinates
                                validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be invalidated now
                                tileConfirmed = Utility.IsTileValid(area, randomTile, true); //is the tile still valid for large objects?
                            } while (validTiles.Count > 0 && !tileConfirmed);

                            if (!tileConfirmed) { break; } //if no more valid tiles could be found, stop trying to spawn things in this area

                            int randomObject = objectIDs[Utility.RNG.Next(objectIDs.Count)]; //get a random object ID to spawn

                            Utility.SpawnLargeObject(randomObject, loc, randomTile);

                            if (area.DaysUntilSpawnsExpire != null) //if this area assigns expiration dates to objects
                            {
                                SavedObject saved = new SavedObject(area.MapName, randomTile, SavedObject.ObjectType.LargeObject, randomObject, null, area.DaysUntilSpawnsExpire); //create a record of the newly spawned object
                                data.Save.SavedObjects.Add(saved); //add it to the save file with the area's expiration setting
                            }
                        }

                        Utility.Monitor.Log($"Large object spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Large object spawn complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Large object spawn complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Large object spawn process complete.", LogLevel.Trace);
            }
        }
    }
}

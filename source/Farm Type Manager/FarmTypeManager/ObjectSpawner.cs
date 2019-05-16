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
        private static class ObjectSpawner
        {
            /// <summary>Generates forageable items in the game based on the current player's config settings.</summary>
            public static void ForageGeneration()
            {
                foreach (FarmData data in Utility.FarmDataList)
                {
                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"Starting forage generation for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"Starting forage generation for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }

                    if (data.Config.ForageSpawnEnabled)
                    {
                        Utility.Monitor.Log("Forage spawn is enabled. Starting generation process...", LogLevel.Trace);
                    }
                    else
                    {
                        Utility.Monitor.Log("Forage spawn is disabled for this file.", LogLevel.Trace);
                        continue;
                    }

                    foreach (ForageSpawnArea area in data.Config.Forage_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking forage settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate the map name for the area
                        if (Game1.getLocationFromName(area.MapName) == null)
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. No forage will be spawned there.", LogLevel.Info);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("All extra conditions met. Generating list of valid tiles...", LogLevel.Trace);

                        List<Vector2> validTiles = Utility.GenerateTileList(area, data.Save, data.Config.QuarryTileIndex, data.Config.Forage_Spawn_Settings.CustomTileIndex, false); //calculate a list of valid tiles for forage in this area

                        Utility.Monitor.Log($"Number of valid tiles: {validTiles.Count}. Deciding how many items to spawn...", LogLevel.Trace);

                        //calculate how much forage to spawn today
                        int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, data.Config.Forage_Spawn_Settings.PercentExtraSpawnsPerForagingLevel, Utility.Skills.Foraging);

                        Utility.Monitor.Log($"Items to spawn: {spawnCount}. Retrieving list of forage types...", LogLevel.Trace);

                        object[] forageList = null; //the list of forage types to use for this area today
                        List<int> forageIDs = new List<int>(); //the list of valid item IDs parsed from the forage list

                        switch (Game1.currentSeason)
                        {
                            case "spring":
                                if (area.SpringItemIndex != null) //if there's an "override" list set for this area
                                {
                                    if (area.SpringItemIndex.Length > 0) //if the override includes any items
                                    {
                                        forageList = area.SpringItemIndex; //get the override index list for this area
                                    }
                                    //if an area index exists but is empty, *do not* use the main index; users may want to disable spawns in this season
                                }
                                else if (data.Config.Forage_Spawn_Settings.SpringItemIndex.Length > 0) //if no "override" list exists and the main index list includes any items
                                {
                                    forageList = data.Config.Forage_Spawn_Settings.SpringItemIndex; //get the main index list
                                }
                                break;
                            case "summer":
                                if (area.SummerItemIndex != null)
                                {
                                    if (area.SummerItemIndex.Length > 0)
                                    {
                                        forageList = area.SummerItemIndex;
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.SummerItemIndex.Length > 0)
                                {
                                    forageList = data.Config.Forage_Spawn_Settings.SummerItemIndex;
                                }
                                break;
                            case "fall":
                                if (area.FallItemIndex != null)
                                {
                                    if (area.FallItemIndex.Length > 0)
                                    {
                                        forageList = area.FallItemIndex;
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.FallItemIndex.Length > 0)
                                {
                                    forageList = data.Config.Forage_Spawn_Settings.FallItemIndex;
                                }
                                break;
                            case "winter":
                                if (area.WinterItemIndex != null)
                                {
                                    if (area.WinterItemIndex.Length > 0)
                                    {
                                        forageList = area.WinterItemIndex;
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.WinterItemIndex.Length > 0)
                                {
                                    forageList = data.Config.Forage_Spawn_Settings.WinterItemIndex;
                                }
                                break;
                        }

                        if (forageList == null) //no valid forage list was selected
                        {
                            Utility.Monitor.Log($"No forage list selected. This generally means the {Game1.currentSeason}IndexList was empty. Skipping to the next forage area...", LogLevel.Trace);
                            continue;
                        }

                        //a list was selected, so parse the list into valid forage IDs
                        foreach (object forage in forageList)
                        {
                            if (forage is long) //the forage is a valid number
                            {
                                forageIDs.Add(Convert.ToInt32(forage));
                            }
                            else if (forage is string) //the forage is a string (i.e. the name of an item)
                            {
                                Utility.Monitor.Log($"Found a name in the selected forage list. Parsing into object ID...", LogLevel.Trace);
                                string forageName = (string)forage; //cast it as a name
                                bool foundMatchingItem = false;

                                foreach (KeyValuePair<int, string> item in Game1.objectInformation) //for each item in the game's object list
                                {
                                    if (forageName.Equals(item.Value.Split('/')[0], StringComparison.OrdinalIgnoreCase)) //if the forage name matches the current item's name (note: first part of the dictionary value, separated from other settings by '/')
                                    {
                                        forageIDs.Add(item.Key); //add the item's ID (which is the dictionary key)
                                        foundMatchingItem = true;
                                        Utility.Monitor.Log($"Index parsed from \"{forageName}\" into ID: {item.Key}", LogLevel.Trace);
                                    }
                                }

                                if (foundMatchingItem == false) //no matching item name could be found
                                {
                                    Utility.Monitor.Log($"An area's {Game1.currentSeason}ItemIndex list contains a name that did not match any items.", LogLevel.Info);
                                    Utility.Monitor.Log($"Area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Info);
                                    Utility.Monitor.Log($"Item name: \"{forageName}\"", LogLevel.Info);
                                }
                            }
                            else //the forage doesn't match any known types
                            {
                                Utility.Monitor.Log($"An area's {Game1.currentSeason}ItemIndex list contains an unrecognized setting.", LogLevel.Info);
                                Utility.Monitor.Log($"Area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Info);
                                Utility.Monitor.Log($"This generally means the list contains a typo. The affected forage item(s) will be skipped.", LogLevel.Info);
                            }
                        }

                        if (forageIDs.Count <= 0) //no valid items were added to the list
                        {
                            Utility.Monitor.Log($"Forage list selected, but contained no valid forage items. Skipping to the next forage area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log($"Forage types found: {forageIDs.Count}. Beginning spawn process...", LogLevel.Trace);

                        //begin to spawn forage; each loop should spawn 1 random forage object on a random valid tile
                        while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for forage & still forage to be spawned
                        {
                            spawnCount--; //reduce by 1, since one will be spawned
                            int randomIndex = Utility.RNG.Next(validTiles.Count); //get the array index for a random valid tile
                            Vector2 randomTile = validTiles[randomIndex]; //get the random tile's x,y coordinates
                            validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be obstructed now

                            int randomForage = forageIDs[Utility.RNG.Next(forageIDs.Count)]; //pick a random forage ID from the list

                            Utility.Monitor.Log($"Attempting to spawn forage. Location: {randomTile.X},{randomTile.Y} ({area.MapName}).", LogLevel.Trace);
                            //this method call is based on code from SDV's DayUpdate() in Farm.cs, as of SDV 1.3.27
                            Game1.getLocationFromName(area.MapName).dropObject(new StardewValley.Object(randomTile, randomForage, (string)null, false, true, false, true), randomTile * 64f, Game1.viewport, true, (Farmer)null);
                        }

                        Utility.Monitor.Log($"Forage spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Forage spawn complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Forage spawn complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Forage spawn process complete.", LogLevel.Trace);
                Utility.Monitor.Log("", LogLevel.Trace);
            }

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
                            if (data.Save.ExistingObjectsFound == false) //if this hasn't been done yet for the current config+farm
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
                                data.Save.ExistingObjectsFound = true; //record that this process has already been done
                            }
                            else //this config+farm already has a list of existing objects (if any)
                            {
                                Utility.Monitor.Log("Find Existing Objects enabled. Using save file data from a previous search.", LogLevel.Trace);
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

                            Utility.Monitor.Log($"Attempting to spawn large object. Location: {randomTile.X},{randomTile.Y} ({area.MapName}).", LogLevel.Trace);
                            loc.addResourceClumpAndRemoveUnderlyingTerrain(objectIDs[Utility.RNG.Next(objectIDs.Count)], 2, 2, randomTile); //generate an object using the list of valid index numbers
                        }

                        Utility.Monitor.Log($"Large object spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                        Utility.Monitor.Log("", LogLevel.Trace);
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
                                    Utility.SpawnOre(ore.Key, area.MapName, randomTile);
                                    break;
                                }
                                else //this ore "loses"
                                {
                                    randomOreNum -= ore.Value; //subtract this ore's chance from the random number before moving to the next one
                                }
                            }

                            Utility.Monitor.Log($"Attempting to spawn ore. Location: {randomTile.X},{randomTile.Y} ({area.MapName}).", LogLevel.Trace);
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

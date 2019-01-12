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
        static class ObjectSpawner
        {
            
            /// <summary>Generates foraged items in the game based on the current player's config settings.</summary>
            public static void ForageGeneration()
            {
                if (Utility.Config.ForageSpawnEnabled != true) { return; } //if forage spawn is disabled, don't do anything

                Random rng = new Random(); //DEVNOTE: "Game1.random" exists, but causes some odd spawn behavior; using this for now...

                foreach (SpawnArea area in Utility.Config.Forage_Spawn_Settings.Areas)
                {
                    //validate the map name for the area
                    if (Game1.getLocationFromName(area.MapName) == null)
                    {
                        Utility.Monitor.Log($"Issue: No map named \"{area.MapName}\" could be found. No forage will be spawned there.", LogLevel.Info);
                        continue;
                    }

                    List<Vector2> validTiles = Utility.GenerateTileList(area, Utility.Config.Forage_Spawn_Settings.CustomTileIndex, false); //calculate a list of valid tiles for forage in this area

                    //calculate how much forage to spawn today
                    int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, Utility.Config.Forage_Spawn_Settings.PercentExtraSpawnsPerForagingLevel, Utility.Skills.Foraging);

                    //begin to spawn forage
                    while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for forage & still forage to be spawned
                    {
                        //this section spawns 1 forage object at a random valid location

                        spawnCount--; //reduce by 1, since one will be spawned
                        int randomIndex = rng.Next(validTiles.Count); //get the array index for a random valid tile
                        Vector2 randomTile = validTiles[randomIndex]; //get the random tile's x,y coordinates
                        validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be obstructed now

                        int randomForageType = -1; //will stay at -1 if the current season has no forage items listed, or be set to a random item's index number
                        switch (Game1.currentSeason)
                        {
                            case "spring":
                                if (Utility.Config.Forage_Spawn_Settings.SpringItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.SpringItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.SpringItemIndex.Length)]; //get a random index from the spring list
                                }
                                break;
                            case "summer":
                                if (Utility.Config.Forage_Spawn_Settings.SummerItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.SummerItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.SummerItemIndex.Length)];
                                }
                                break;
                            case "fall":
                                if (Utility.Config.Forage_Spawn_Settings.FallItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.FallItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.FallItemIndex.Length)];
                                }
                                break;
                            case "winter":
                                if (Utility.Config.Forage_Spawn_Settings.WinterItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.WinterItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.WinterItemIndex.Length)];
                                }
                                break;
                        }
                        if (randomForageType != -1) //if the forage type seems valid
                        {
                            //this method call is based on code from SDV's DayUpdate() in Farm.cs, as of SDV 1.3.27
                            Game1.getLocationFromName(area.MapName).dropObject(new StardewValley.Object(randomTile, randomForageType, (string)null, false, true, false, true), randomTile * 64f, Game1.viewport, true, (Farmer)null);
                        }
                    }
                }
            }

            /// <summary>Generates large objects (e.g. stumps and logs) in the game based on the current player's config settings.</summary>
            public static void LargeObjectGeneration()
            {
                if (Utility.Config.LargeObjectSpawnEnabled != true) { return; } //if large object spawn is disabled, don't do anything

                Random rng = new Random(); //DEVNOTE: "Game1.random" exists, but causes some odd spawn behavior; using this for now...

                foreach (LargeObjectSpawnArea area in Utility.Config.Large_Object_Spawn_Settings.Areas)
                {
                    //validate the map name for the area
                    if (Game1.getLocationFromName(area.MapName) == null)
                    {
                        Utility.Monitor.Log($"Issue: No map named \"{area.MapName}\" could be found. Large objects won't be spawned there.", LogLevel.Info);
                        continue;
                    }

                    Farm loc = Game1.getLocationFromName(area.MapName) as Farm; //variable for the current location being worked on (NOTE: null if the current location isn't a "farm" map)
                    if (loc == null) //if this area isn't a "farm" map, there's usually no built-in support for resource clumps (i.e. large objects), so display an error message and skip this area
                    {
                        Utility.Monitor.Log($"Issue: Large objects cannot be spawned in the \"{area.MapName}\" map. Only \"farm\" map types are currently supported.", LogLevel.Info);
                        continue;
                    }

                    List<int> objectIDs = Utility.GetLargeObjectIDs(area.ObjectTypes); //get a list of index numbers for relevant object types in this area

                    if (area.FindExistingObjectLocations == true) //if enabled, ensure that any existing objects are added to the include area list
                    {
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

                        if (existingObjects.Count > 0) //if any existing objects need to be included
                        {
                            area.IncludeAreas = area.IncludeAreas.Concat(existingObjects).ToArray(); //add the new include strings to the end of the existing set
                        }

                        area.FindExistingObjectLocations = false; //disable this process so it doesn't happen every day (using it repeatedly while spawning new objects would fill the whole map over time...)
                    }

                    List<Vector2> validTiles = Utility.GenerateTileList(area, Utility.Config.Large_Object_Spawn_Settings.CustomTileIndex, true); //calculate a list of valid tiles for large objects in this area

                    //calculate how many objects to spawn today
                    int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, area.PercentExtraSpawnsPerSkillLevel, (Utility.Skills)Enum.Parse(typeof(Utility.Skills), area.RelatedSkill, true));
                    
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
                            randomIndex = rng.Next(validTiles.Count); //get the array index for a random valid tile
                            randomTile = validTiles[randomIndex]; //get the random tile's x,y coordinates
                            validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be invalidated now
                            tileConfirmed = Utility.IsTileValid(area, randomTile, true); //is the tile still valid for large objects?
                        } while (validTiles.Count > 0 && !tileConfirmed);

                        if (!tileConfirmed) { break; } //if no more valid tiles could be found, stop trying to spawn things in this area

                        loc.addResourceClumpAndRemoveUnderlyingTerrain(objectIDs[rng.Next(objectIDs.Count)], 2, 2, randomTile); //generate an object using the list of valid index numbers
                    }
                }
            }
            
            /// <summary>Generates ore in the game based on the current player's config settings.</summary>
            public static void OreGeneration()
            {
                if (Utility.Config.OreSpawnEnabled != true) { return; } //if ore spawn is disabled, don't do anything

                Random rng = new Random(); //DEVNOTE: "Game1.random" exists, but causes some odd spawn behavior; using this for now...

                foreach (OreSpawnArea area in Utility.Config.Ore_Spawn_Settings.Areas)
                {
                    //validate the map name for the area
                    if (Game1.getLocationFromName(area.MapName) == null)
                    {
                        Utility.Monitor.Log($"Issue: No map named \"{area.MapName}\" could be found. No ore will be spawned there.", LogLevel.Info);
                        continue;
                    }

                    List<Vector2> validTiles = Utility.GenerateTileList(area, Utility.Config.Ore_Spawn_Settings.CustomTileIndex, false); //calculate a list of valid tiles for ore in this area

                    //calculate how much ore to spawn today
                    int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, Utility.Config.Ore_Spawn_Settings.PercentExtraSpawnsPerMiningLevel, Utility.Skills.Mining);

                    //figure out which config section to use (if the spawn area's data is null, use the "global" data instead)
                    Dictionary<string, int> skillReq = area.MiningLevelRequired ?? Utility.Config.Ore_Spawn_Settings.MiningLevelRequired;
                    Dictionary<string, int> startChance = area.StartingSpawnChance ?? Utility.Config.Ore_Spawn_Settings.StartingSpawnChance;
                    Dictionary<string, int> tenChance = area.LevelTenSpawnChance ?? Utility.Config.Ore_Spawn_Settings.LevelTenSpawnChance;
                    //also use the "global" data if the area data is non-null but empty (which can happen accidentally when the json file is manually edited)
                    if (skillReq.Count < 1)
                    {
                        skillReq = Utility.Config.Ore_Spawn_Settings.MiningLevelRequired;
                    }
                    if (startChance.Count < 1)
                    {
                        startChance = Utility.Config.Ore_Spawn_Settings.StartingSpawnChance;
                    }
                    if (tenChance.Count < 1)
                    {
                        tenChance = Utility.Config.Ore_Spawn_Settings.LevelTenSpawnChance;
                    }

                    //calculate the final spawn chance for each type of ore
                    Dictionary<string, int> oreChances = Utility.AdjustedSpawnChances(Utility.Skills.Mining, skillReq, startChance, tenChance);
                    
                    if (oreChances.Count < 1) { continue; } //if there's no chance of spawning any ore for some reason, just stop working on this area now

                    //begin to spawn ore
                    int randomIndex;
                    Vector2 randomTile;
                    int randomOreNum;
                    while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for ore & still ore to be spawned
                    {
                        //this section spawns 1 ore at a random valid location

                        spawnCount--; //reduce by 1, since one will be spawned
                        randomIndex = rng.Next(validTiles.Count); //get the array index for a random tile
                        randomTile = validTiles[randomIndex]; //get the tile's x,y coordinates
                        validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be obstructed now

                        int totalWeight = 0; //the upper limit for the random number that picks ore type (i.e. the sum of all ore chances)
                        foreach (KeyValuePair<string, int> ore in oreChances)
                        {
                            totalWeight += ore.Value; //sum up all the ore chances
                        }
                        randomOreNum = rng.Next(totalWeight); //generate random number from 0 to [totalWeight - 1]
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
                    }
                }
            }
        }
    }
}

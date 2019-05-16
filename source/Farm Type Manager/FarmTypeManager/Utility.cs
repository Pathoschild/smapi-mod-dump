using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static class Utility
        {
            /// <summary>Generates a list of all valid tiles for object spawning in the provided SpawnArea.</summary>
            /// <param name="area">A SpawnArea listing an in-game map name and the valid regions/terrain within it that may be valid spawn points.</param>
            /// <param name="quarryTileIndex">The list of quarry tile indices for this spawn process.</param>
            /// <param name="customTileIndex">The list of custom tile indices for this spawn process.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A completed list of all valid tile coordinates for this spawn process in this SpawnArea.</returns>
            public static List<Vector2> GenerateTileList(SpawnArea area, InternalSaveData save, int[] quarryTileIndex, int[] customTileIndex, bool isLarge)
            {
                List<Vector2> validTiles = new List<Vector2>(); //list of all open, valid tiles for new spawns on the current map

                foreach (string type in area.AutoSpawnTerrainTypes) //loop to auto-detect valid tiles based on various types of terrain
                {
                    if (type.Equals("quarry", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "quarry" tile index list
                    {
                        validTiles.AddRange(Utility.GetTilesByIndex(area, quarryTileIndex, isLarge));
                    }
                    else if (type.Equals("custom", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "custom" tile index list
                    {
                        validTiles.AddRange(Utility.GetTilesByIndex(area, customTileIndex, isLarge));
                    }
                    else  //add any tiles with properties matching "type" (e.g. tiles with the "Diggable" property, "Grass" type, etc; if the "type" is "All", this will just add every valid tile)
                    {
                        validTiles.AddRange(Utility.GetTilesByProperty(area, type, isLarge));
                    }
                }
                foreach (string include in area.IncludeAreas) //check for valid tiles in each "include" zone for the area
                {
                    validTiles.AddRange(Utility.GetTilesByVectorString(area, include, isLarge));
                }

                if (area is LargeObjectSpawnArea objArea && objArea.FindExistingObjectLocations) //if this area is the large object type and "find existing objects" is enabled
                {
                    foreach (string include in save.ExistingObjectLocations[area.UniqueAreaID]) //check each saved "include" string for the area
                    {
                        validTiles.AddRange(Utility.GetTilesByVectorString(area, include, isLarge));
                    }
                }

                validTiles = validTiles.Distinct().ToList(); //remove any duplicate tiles from the list

                foreach (string exclude in area.ExcludeAreas) //check for valid tiles in each "exclude" zone for the area (validity isn't technically relevant here, but simpler to code, and tiles' validity cannot currently change during this process)
                {
                    List<Vector2> excludedTiles = Utility.GetTilesByVectorString(area, exclude, isLarge); //get list of valid tiles in the excluded area
                    validTiles.RemoveAll(excludedTiles.Contains); //remove any previously valid tiles that match the excluded area
                }

                return validTiles;
            }

            /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on tile index, e.g. tiles using a specific dirt texture).</summary>
            /// <param name="area">The SpawnArea describing the current area and its settings.</param>
            /// <param name="tileIndices">A list of integers representing spritesheet tile indices. Tiles with any matching index will be checked for object spawning.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByIndex(SpawnArea area, int[] tileIndices, bool isLarge)
            {
                GameLocation loc = Game1.getLocationFromName(area.MapName); //variable for the current location being worked on
                List<Vector2> validTiles = new List<Vector2>(); //will contain x,y coordinates for tiles that are open & valid for new object placement

                //the following loops should populate a list of valid, open tiles for spawning
                int currentTileIndex;
                for (int y = 0; y < (loc.Map.DisplayHeight / Game1.tileSize); y++)
                {
                    for (int x = 0; x < (loc.Map.DisplayWidth / Game1.tileSize); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                    {
                        Vector2 tile = new Vector2(x, y);
                        currentTileIndex = loc.getTileIndexAt(x, y, "Back"); //get the tile index of the current tile
                        foreach (int index in tileIndices)
                        {
                            if (currentTileIndex == index) //if the current tile matches one of the tile indices
                            {
                                if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                                {
                                    validTiles.Add(tile); //add to list of valid spawn tiles
                                    break; //skip the rest of the indices to avoid adding this tile multiple times
                                }
                            }
                        }
                    }
                }
                return validTiles;
            }

            /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on tile properties, e.g. the "grass" type).</summary>
            /// <param name="area">The SpawnArea describing the current area and its settings.</param>
            /// <param name="type">A string representing the tile property to match, or a special term used for some additional checks.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByProperty(SpawnArea area, string type, bool isLarge)
            {
                GameLocation loc = Game1.getLocationFromName(area.MapName); //variable for the current location being worked on
                List<Vector2> validTiles = new List<Vector2>(); //will contain x,y coordinates for tiles that are open & valid for new object placement

                //the following loops should populate a list of valid, open tiles for spawning
                for (int y = 0; y < (loc.Map.DisplayHeight / Game1.tileSize); y++)
                {
                    for (int x = 0; x < (loc.Map.DisplayWidth / Game1.tileSize); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                    {
                        Vector2 tile = new Vector2(x, y);
                        if (type.Equals("all", StringComparison.OrdinalIgnoreCase)) //if the "property" to be matched is "All" (a special exception)
                        {

                            //add any clear tiles, regardless of properties
                            if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                            {
                                validTiles.Add(tile); //add to list of valid spawn tiles
                            }
                        }
                        if (type.Equals("diggable", StringComparison.OrdinalIgnoreCase)) //if the tile's "Diggable" property matches (case-insensitive)
                        {
                            if (loc.doesTileHaveProperty(x, y, "Diggable", "Back") == "T") //NOTE: the string "T" means "true" for several tile property checks
                            {
                                if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                                {
                                    validTiles.Add(tile); //add to list of valid spawn tiles
                                }
                            }
                        }
                        else //assumed to be checking for a specific value in the tile's "Type" property, e.g. "Grass" or "Dirt"
                        {
                            string currentType = loc.doesTileHaveProperty(x, y, "Type", "Back") ?? ""; //NOTE: this sets itself to a blank (not null) string to avoid null errors when comparing it

                            if (currentType.Equals(type, StringComparison.OrdinalIgnoreCase)) //if the tile's "Type" property matches (case-insensitive)
                            {
                                if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                                {
                                    validTiles.Add(tile); //add to list of valid spawn tiles
                                }
                            }
                        }
                    }
                }
                return validTiles;
            }

            /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on a string describing two vectors).</summary>
            /// <param name="area">The SpawnArea describing the current area and its settings.</param>
            /// <param name="vectorString">A string describing two vectors. Parsed into vectors and used to find a rectangular area.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByVectorString(SpawnArea area, string vectorString, bool isLarge)
            {
                GameLocation loc = Game1.getLocationFromName(area.MapName); //variable for the current location being worked on
                List<Vector2> validTiles = new List<Vector2>(); //x,y coordinates for tiles that are open & valid for new object placement
                List<Tuple<Vector2, Vector2>> vectorPairs = new List<Tuple<Vector2, Vector2>>(); //pairs of x,y coordinates representing areas on the map (to be scanned for valid tiles)

                //parse the "raw" string representing two coordinates into actual numbers, populating "vectorPairs"
                string[] xyxy = vectorString.Split(new char[] { ',', '/', ';' }); //split the string into separate strings based on various delimiter symbols
                if (xyxy.Length != 4) //if "xyxy" didn't split into the right number of strings, it's probably formatted poorly
                {
                    Monitor.Log($"Issue: This include/exclude area for the {area.MapName} map isn't formatted correctly: \"{vectorString}\"", LogLevel.Info);
                }
                else
                {
                    int[] numbers = new int[4]; //this section will convert "xyxy" into four numbers and store them here
                    bool success = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (Int32.TryParse(xyxy[i].Trim(), out numbers[i]) != true) //attempts to store each "xyxy" string as an integer in "numbers"; returns false if it failed
                        {
                            success = false;
                        }
                    }

                    if (success) //everything was successfully parsed, apparently
                    {
                        //convert the numbers to a pair of vectors and add them to the list
                        vectorPairs.Add(new Tuple<Vector2, Vector2>(new Vector2(numbers[0], numbers[1]), new Vector2(numbers[2], numbers[3])));
                    }
                    else
                    {
                        Monitor.Log($"Issue: This include/exclude area for the {area.MapName} map isn't formatted correctly: \"{vectorString}\"", LogLevel.Info);
                    }
                }

                //check the area marked by "vectorPairs" for valid, open tiles and populate "validTiles" with them
                foreach (Tuple<Vector2, Vector2> pair in vectorPairs)
                {
                    for (int y = (int)Math.Min(pair.Item1.Y, pair.Item2.Y); y <= (int)Math.Max(pair.Item1.Y, pair.Item2.Y); y++) //use the lower Y first, then the higher Y; should define the area regardless of which corners/order the user wrote down
                    {
                        for (int x = (int)Math.Min(pair.Item1.X, pair.Item2.X); x <= (int)Math.Max(pair.Item1.X, pair.Item2.X); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                        {
                            Vector2 tile = new Vector2(x, y);
                            if (IsTileValid(area, new Vector2(x, y), isLarge)) //if the tile is clear of any obstructions
                            {
                                validTiles.Add(tile); //add to list of valid spawn tiles
                            }
                        }
                    }
                }

                return validTiles;
            }

            /// <summary>Determines whether a specific tile on a map is valid for object placement, using any necessary checks from Stardew's native methods.</summary>
            /// <param name="area">The SpawnArea describing the current area and its settings.</param>
            /// <param name="tile">The tile to be validated for object placement (for a large object, this is effectively its upper left corner).</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>Whether the provided tile is valid for the given area and object size, based on the area's StrictTileChecking setting.</returns>
            public static bool IsTileValid(SpawnArea area, Vector2 tile, bool isLarge)
            {
                GameLocation loc = Game1.getLocationFromName(area.MapName); //variable for the current location being worked on 
                bool valid = false;


                if (area.StrictTileChecking.Equals("off", StringComparison.OrdinalIgnoreCase) || area.StrictTileChecking.Equals("none", StringComparison.OrdinalIgnoreCase)) //no validation at all
                {
                    valid = true;
                }
                else if (area.StrictTileChecking.Equals("low", StringComparison.OrdinalIgnoreCase)) //low-strictness validation
                {
                    if (isLarge) //2x2 tile validation
                    {
                        //if all the necessary tiles for a 2x2 object are *not* blocked by other objects
                        if (!loc.isObjectAtTile((int)tile.X, (int)tile.Y) && !loc.isObjectAtTile((int)tile.X + 1, (int)tile.Y) && !loc.isObjectAtTile((int)tile.X, (int)tile.Y + 1) && !loc.isObjectAtTile((int)tile.X + 1, (int)tile.Y + 1))
                        {
                            valid = true;
                        }
                    }
                    else //single tile validation
                    {
                        if (!loc.isObjectAtTile((int)tile.X, (int)tile.Y)) //if the tile is *not* blocked by another object
                        {
                            valid = true;
                        }
                    }
                }
                else if (area.StrictTileChecking.Equals("medium", StringComparison.OrdinalIgnoreCase)) //medium-strictness validation
                {
                    if (isLarge) //2x2 tile validation
                    {
                        //if all the necessary tiles for a 2x2 object are *not* occupied
                        if (!loc.isTileOccupiedForPlacement(tile) && !loc.isTileOccupiedForPlacement(new Vector2(tile.X + 1, tile.Y)) && !loc.isTileOccupiedForPlacement(new Vector2(tile.X, tile.Y + 1)) && !loc.isTileOccupiedForPlacement(new Vector2(tile.X + 1, tile.Y + 1)))
                        {
                            valid = true;
                        }
                    }
                    else //single tile validation
                    {
                        if (!loc.isTileOccupiedForPlacement(tile)) //if the tile is *not* occupied
                        {
                            valid = true;
                        }
                    }
                }
                else //default to "high"-strictness validation
                {
                    if (isLarge) //2x2 tile validation
                    {
                        //if all the necessary tiles for a 2x2 object are *not* occupied
                        if (loc.isTileLocationTotallyClearAndPlaceable(tile) && loc.isTileLocationTotallyClearAndPlaceable(new Vector2(tile.X + 1, tile.Y)) && loc.isTileLocationTotallyClearAndPlaceable(new Vector2(tile.X, tile.Y + 1)) && loc.isTileLocationTotallyClearAndPlaceable(new Vector2(tile.X + 1, tile.Y + 1)))
                        {
                            valid = true;
                        }
                    }
                    else //single tile validation
                    {
                        if (loc.isTileLocationTotallyClearAndPlaceable(tile)) //if the tile is *not* occupied
                        {
                            valid = true;
                        }
                    }
                }

                return valid;
            }

            /// <summary>Generates ore and places it on the specified map and tile.</summary>
            /// <param name="oreName">A string representing the name of the ore type to be spawned, e.g. "</param>
            /// <param name="mapName">The name of the GameLocation where the ore should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static void SpawnOre(string oreName, string mapName, Vector2 tile)
            {
                StardewValley.Object ore = null; //ore object, to be spawned into the world later
                switch (oreName.ToLower()) //avoid any casing issues in method calls by making this lower-case
                {
                    case "stone":
                        ore = new StardewValley.Object(tile, 668 + (RNG.Next(2) * 2), 1); //either of the two random stones spawned in the vanilla hilltop quarry
                        ore.MinutesUntilReady = 2; //durability, i.e. number of hits with basic pickaxe required to break the ore (each pickaxe level being +1 damage)
                        break;
                    case "geode":
                        ore = new StardewValley.Object(tile, 75, 1); //"regular" geode rock, as spawned on vanilla hilltop quarries 
                        ore.MinutesUntilReady = 3;
                        break;
                    case "frozengeode":
                        ore = new StardewValley.Object(tile, 76, 1); //frozen geode rock
                        ore.MinutesUntilReady = 5;
                        break;
                    case "magmageode":
                        ore = new StardewValley.Object(tile, 77, 1); //magma geode rock
                        ore.MinutesUntilReady = 8; //TODO: replace this guess w/ actual vanilla durability
                        break;
                    case "gem":
                        ore = new StardewValley.Object(tile, (RNG.Next(7) + 1) * 2, "Stone", true, false, false, false); //any of the possible gem rocks
                        ore.MinutesUntilReady = 5; //based on "gemstone" durability, but applies to every type for simplicity's sake
                        break;
                    case "copper":
                        ore = new StardewValley.Object(tile, 751, 1); //copper ore
                        ore.MinutesUntilReady = 3;
                        break;
                    case "iron":
                        ore = new StardewValley.Object(tile, 290, 1); //iron ore
                        ore.MinutesUntilReady = 4;
                        break;
                    case "gold":
                        ore = new StardewValley.Object(tile, 764, 1); //gold ore
                        ore.MinutesUntilReady = 8;
                        break;
                    case "iridium":
                        ore = new StardewValley.Object(tile, 765, 1); //iridium ore
                        ore.MinutesUntilReady = 16; //TODO: confirm this is still the case (it's based on SDV 1.11 code)
                        break;
                    case "mystic":
                        ore = new StardewValley.Object(tile, 46, "Stone", true, false, false, false); //mystic ore, i.e. high-end cavern rock with iridium + gold
                        ore.MinutesUntilReady = 16; //TODO: replace this guess w/ actual vanilla durability
                        break;
                    default: break;
                }

                if (ore != null)
                {
                    GameLocation loc = Game1.getLocationFromName(mapName);
                    loc.setObject(tile, ore); //actually spawn the ore object into the world
                }
                else
                {
                    Utility.Monitor.Log($"The ore to be spawned (\"{oreName}\") doesn't match any known ore types. Make sure that name isn't misspelled in your player config file.", LogLevel.Info);
                }

                return;
            }

            /// <summary>Produces a dictionary containing the final, adjusted spawn chance of each object in the provided dictionaries. (Part of the convoluted object spawning process for ore.)</summary>
            /// <param name="skill">The player skill that affects spawn chances (e.g. Mining for ore spawn chances).</param>
            /// <param name="levelRequired">A dictionary of object names and the skill level required to spawn them.</param>
            /// <param name="startChances">A dictionary of object names and their weighted chances to spawn at their lowest required skill level (e.g. chance of spawning stone if you're level 0).</param>
            /// <param name="maxChances">A dictionary of object names and their weighted chances to spawn at skill level 10.</param>
            /// <returns></returns>
            public static Dictionary<string, int> AdjustedSpawnChances(Utility.Skills skill, Dictionary<string, int> levelRequired, Dictionary<string, int> startChances, Dictionary<string, int> maxChances)
            {
                Dictionary<string, int> adjustedChances = new Dictionary<string, int>();

                int skillLevel = 0; //highest skill level among all existing farmers, not just the host
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    skillLevel = Math.Max(skillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                }

                foreach (KeyValuePair<string, int> objType in levelRequired)
                {
                    int chance = 0; //chance of spawning this object
                    if (objType.Value > skillLevel)
                    {
                        //skill is too low to spawn this object; leave it at 0%
                    }
                    else if (objType.Value == skillLevel)
                    {
                        chance = startChances[objType.Key]; //skill is the minimum required; use the starting chance
                    }
                    else if (skillLevel >= 10)
                    {
                        chance = maxChances[objType.Key]; //level 10 skill; use the max level chance
                    }
                    else //skill is somewhere in between "starting" and "level 10", so do math to set the chance somewhere in between them (i forgot the term for this kind of averaging, sry)
                    {
                        int count = 0;
                        long chanceMath = 0; //used in case the chances are very large numbers for some reason
                        for (int x = objType.Value; x < 10; x++) //loop from [minimum skill level for this object] to [max level - 1], for vague math reasons
                        {
                            if (skillLevel > x)
                            {
                                chanceMath += maxChances[objType.Key]; //add level 10 chance
                            }
                            else
                            {
                                chanceMath += startChances[objType.Key]; //add starting chance
                            }
                            count++;
                        }
                        chanceMath = (long)Math.Round((double)chanceMath / (double)count); //divide to get the average
                        chance = Convert.ToInt32(chanceMath); //convert back to a reasonable number range once the math is done
                    }

                    if (chance > 0) //don't bother adding any objects with 0% or negative spawn chance
                    {
                        adjustedChances.Add(objType.Key, chance); //add the object name & chance to the list of adjusted chances
                    }
                }

                return adjustedChances;
            }

            /// <summary>Calculates the final number of objects to spawn today in the current spawning process, based on config settings and player levels in a relevant skill.</summary>
            /// <param name="min">Minimum number of objects to spawn today (before skill multiplier).</param>
            /// <param name="max">Maximum number of objects to spawn today (before skill multiplier).</param>
            /// <param name="percent">Additive multiplier for each of the player's levels in the relevant skill (e.g. 10 would represent +10% objects per level).</param>
            /// <param name="skill">Enumerator for the skill on which the "percent" additive multiplier is based.</param>
            /// <returns>The final number of objects to spawn today in the current spawning process.</returns>
            public static int AdjustedSpawnCount(int min, int max, int percent, Utility.Skills skill)
            {
                int spawnCount = RNG.Next(min, max + 1); //random number from min to max (higher number is exclusive, so +1 to adjust for it)

                //calculate skill multiplier bonus
                double skillMultiplier = percent;
                skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. default config is "10" (10% per level) so it converts to "0.1"
                int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                }
                skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. with default config: "1.0" at level 0, "1.7" at level 7, etc

                //calculate final forage amount
                skillMultiplier *= spawnCount; //multiply the initial random spawn count by the skill multiplier
                spawnCount = (int)skillMultiplier; //store the integer portion of the current multiplied value (e.g. this is "1" if the multiplier is "1.7")
                double remainder = skillMultiplier - (int)skillMultiplier; //store the decimal portion of the multiplied value (e.g. this is "0.7" if the multiplier is "1.7")

                if (RNG.NextDouble() < remainder) //use remainder as a % chance to spawn one extra object (e.g. if the final count would be "1.7", there's a 70% chance of spawning 2 objects)
                {
                    spawnCount++;
                }

                return spawnCount;
            }

            /// <summary>Generate an array of index numbers for large objects (a.k.a. resource clumps) based on an array of names. Duplicates are allowed; invalid entries are discarded.</summary>
            /// <param name="names">A list of names representing large objects (e.g. "Stump", "boulders").</param>
            /// <returns>An array of index numbers for large object spawning purposes.</returns>
            public static List<int> GetLargeObjectIDs(string[] names)
            {
                List<int> IDs = new List<int>(); //a list of index numbers to be returned

                foreach (string name in names)
                {
                    //for each valid name, add the game's internal ID for that large object (a.k.a. resource clump)
                    switch (name.ToLower())
                    {
                        case "stump":
                        case "stumps":
                            IDs.Add(600);
                            break;
                        case "log":
                        case "logs":
                            IDs.Add(602);
                            break;
                        case "boulder":
                        case "boulders":
                            IDs.Add(672);
                            break;
                        case "meteor":
                        case "meteors":
                        case "meteorite":
                        case "meteorites":
                            IDs.Add(622);
                            break;
                        case "minerock1":
                        case "mine rock 1":
                            IDs.Add(752);
                            break;
                        case "minerock2":
                        case "mine rock 2":
                            IDs.Add(754);
                            break;
                        case "minerock3":
                        case "mine rock 3":
                            IDs.Add(756);
                            break;
                        case "minerock4":
                        case "mine rock 4":
                            IDs.Add(758);
                            break;
                        default: //"name" isn't recognized as any existing object names
                            int parsed;
                            if (int.TryParse(name, out parsed)) //if the string seems to be a valid integer, save it to "parsed" and add it to the list
                            {
                                IDs.Add(parsed);
                            }
                            break;
                    }
                }

                return IDs;
            }

            /// <summary>Checks whether objects should be spawned in a given SpawnArea based on its ExtraConditions settings.</summary>
            /// <param name="area">The SpawnArea to be checked.</param>
            /// <param name="save">The mod's save data for the current farm and config file.</param>
            /// <returns>True if objects are allowed to spawn. False if any extra conditions should prevent spawning.</returns>
            public static bool CheckExtraConditions(SpawnArea area, InternalSaveData save)
            {
                Monitor.Log($"Checking extra conditions for this area...", LogLevel.Trace);

                //check years
                if (area.ExtraConditions.Years != null && area.ExtraConditions.Years.Length > 0)
                {
                    Monitor.Log("Years condition(s) found. Checking...", LogLevel.Trace);

                    bool validYear = false;

                    foreach (string year in area.ExtraConditions.Years)
                    {
                        try //watch for errors related to string parsing
                        {
                            if (year.Equals("All", StringComparison.OrdinalIgnoreCase) || year.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                            {
                                validYear = true;
                                break; //skip the rest of the "year" checks
                            }
                            else if (year.Contains("+")) //contains a plus, so parse it as a single year & any years after it, e.g. "2+"
                            {
                                string[] split = year.Split('+'); //split into separate strings around the plus symbol
                                int minYear = Int32.Parse(split[0].Trim()); //get the number to the left of the plus (trim whitespace) 

                                if (minYear <= Game1.year) //if the current year is within the specified range
                                {
                                    validYear = true;
                                    break; //skip the rest of the "year" checks
                                }
                            }
                            else if (year.Contains("-")) //contains a delimiter, so parse it as a range of years, e.g. "1-10"
                            {
                                string[] split = year.Split('-'); //split into separate strings for each delimiter
                                int minYear = Int32.Parse(split[0].Trim()); //get the first number (trim whitespace)
                                int maxYear = Int32.Parse(split[1].Trim()); //get the second number (trim whitespace)

                                if (minYear <= Game1.year && maxYear >= Game1.year) //if the current year is within the specified range
                                {
                                    validYear = true;
                                    break; //skip the rest of the "year" checks
                                }
                            }
                            else //parse as a single year, e.g. "1"
                            {
                                int yearNum = Int32.Parse(year.Trim()); //convert to a number

                                if (yearNum == Game1.year) //if it matches the current year
                                {
                                    validYear = true;
                                    break; //skip the rest of the "year" checks
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log($"Issue: This part of the extra condition \"Years\" for the {area.MapName} map isn't formatted correctly: \"{year}\"", LogLevel.Info);
                        }
                    }

                    if (validYear)
                    {
                        Monitor.Log("The current year matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("The current year did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false;
                    }
                }

                //check seasons
                if (area.ExtraConditions.Seasons != null && area.ExtraConditions.Seasons.Length > 0)
                {
                    Monitor.Log("Seasons condition(s) found. Checking...", LogLevel.Trace);

                    bool validSeason = false;

                    foreach (string season in area.ExtraConditions.Seasons)
                    {
                        if (season.Equals("All", StringComparison.OrdinalIgnoreCase) || season.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validSeason = true;
                            break; //skip the rest of the "season" checks
                        }
                        else if (season.Equals(Game1.currentSeason, StringComparison.OrdinalIgnoreCase)) //if the current season is listed
                        {
                            validSeason = true;
                            break; //skip the rest of the "season" checks
                        }
                    }

                    if (validSeason)
                    {
                        Monitor.Log("The current season matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("The current season did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check days
                if (area.ExtraConditions.Days != null && area.ExtraConditions.Days.Length > 0)
                {
                    Monitor.Log("Days condition(s) found. Checking...", LogLevel.Trace);

                    bool validDay = false;

                    foreach (string day in area.ExtraConditions.Days)
                    {
                        try //watch for errors related to string parsing
                        {
                            if (day.Equals("All", StringComparison.OrdinalIgnoreCase) || day.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                            {
                                validDay = true;
                                break; //skip the rest of the "day" checks
                            }
                            else if (day.Contains("+")) //contains a plus, so parse it as a single day & any days after it, e.g. "2+"
                            {
                                string[] split = day.Split('+'); //split into separate strings around the plus symbol
                                int minDay = Int32.Parse(split[0].Trim()); //get the number to the left of the plus (trim whitespace) 

                                if (minDay <= Game1.dayOfMonth) //if the current day is within the specified range
                                {
                                    validDay = true;
                                    break; //skip the rest of the "day" checks
                                }
                            }
                            else if (day.Contains("-")) //contains a delimiter, so parse it as a range of dates, e.g. "1-10"
                            {
                                string[] split = day.Split('-'); //split into separate strings for each delimiter
                                int minDay = Int32.Parse(split[0].Trim()); //get the first number (trim whitespace)
                                int maxDay = Int32.Parse(split[1].Trim()); //get the second number (trim whitespace)

                                if (minDay <= Game1.dayOfMonth && maxDay >= Game1.dayOfMonth) //if the current day is within the specified range
                                {
                                    validDay = true;
                                    break; //skip the rest of the "day" checks
                                }
                            }
                            else //parse as a single date, e.g. "1" or "25"
                            {
                                int dayNum = Int32.Parse(day.Trim()); //convert to a number

                                if (dayNum == Game1.dayOfMonth) //if it matches the current day
                                {
                                    validDay = true;
                                    break; //skip the rest of the "day" checks
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log($"Issue: This part of the extra condition \"Days\" for the {area.MapName} map isn't formatted correctly: \"{day}\"", LogLevel.Info);
                        }
                    }

                    if (validDay)
                    {
                        Monitor.Log("The current day matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("The current day did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check yesterday's weather
                if (area.ExtraConditions.WeatherYesterday != null && area.ExtraConditions.WeatherYesterday.Length > 0)
                {
                    Monitor.Log("Yesterday's Weather condition(s) found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherYesterday) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of these checks
                        }

                        switch (save.WeatherForYesterday) //compare to yesterday's weather
                        {
                            case Utility.Weather.Sunny:
                            case Utility.Weather.Festival: //festival and wedding = sunny, as far as this mod is concerned
                            case Utility.Weather.Wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }

                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of these checks
                        }
                    }


                    if (validWeather)
                    {
                        Monitor.Log("Yesterday's weather matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Yesterday's weather did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check today's weather
                if (area.ExtraConditions.WeatherToday != null && area.ExtraConditions.WeatherToday.Length > 0)
                {
                    Monitor.Log("Today's Weather condition(s) found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherToday) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of these checks
                        }

                        switch (Utility.WeatherForToday()) //compare to today's weather
                        {
                            case Utility.Weather.Sunny:
                            case Utility.Weather.Festival: //festival and wedding = sunny, as far as this mod is concerned
                            case Utility.Weather.Wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case Utility.Weather.Snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }
                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of these checks
                        }
                    }

                    if (validWeather)
                    {
                        Monitor.Log("Today's weather matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Today's weather did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check tomorrow's weather
                if (area.ExtraConditions.WeatherTomorrow != null && area.ExtraConditions.WeatherTomorrow.Length > 0)
                {
                    Monitor.Log("Tomorrow's Weather condition(s) found. Checking...", LogLevel.Trace);

                    bool validWeather = false;

                    foreach (string weather in area.ExtraConditions.WeatherTomorrow) //for each listed weather name
                    {
                        if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validWeather = true;
                            break; //skip the rest of these checks
                        }

                        switch (Game1.weatherForTomorrow) //compare to tomorrow's weather
                        {
                            case (int)Utility.Weather.Sunny:
                            case (int)Utility.Weather.Festival: //festival and wedding = sunny, as far as this mod is concerned
                            case (int)Utility.Weather.Wedding:
                                if (weather.Equals("Sun", StringComparison.OrdinalIgnoreCase) || weather.Equals("Sunny", StringComparison.OrdinalIgnoreCase) || weather.Equals("Clear", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Rain:
                                if (weather.Equals("Rain", StringComparison.OrdinalIgnoreCase) || weather.Equals("Rainy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Raining", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Debris:
                                if (weather.Equals("Wind", StringComparison.OrdinalIgnoreCase) || weather.Equals("Windy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Debris", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Lightning:
                                if (weather.Equals("Storm", StringComparison.OrdinalIgnoreCase) || weather.Equals("Stormy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Storming", StringComparison.OrdinalIgnoreCase) || weather.Equals("Lightning", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                            case (int)Utility.Weather.Snow:
                                if (weather.Equals("Snow", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowy", StringComparison.OrdinalIgnoreCase) || weather.Equals("Snowing", StringComparison.OrdinalIgnoreCase))
                                {
                                    validWeather = true;
                                }
                                break;
                        }

                        if (validWeather == true) //if a valid weather condition was listed
                        {
                            break; //skip the rest of these checks
                        }
                    }

                    if (validWeather)
                    {
                        Monitor.Log("Tomorrow's weather matched a setting. Spawn allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Tomorrow's weather did NOT match any settings. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                //check number of spawns
                //NOTE: it's important that this is the last condition checked, because otherwise it might count down while not actually spawning (i.e. while blocked by another condition
                if (area.ExtraConditions.LimitedNumberOfSpawns != null)
                {
                    Monitor.Log("Limited Number Of Spawns condition found. Checking...", LogLevel.Trace);
                    if (area.ExtraConditions.LimitedNumberOfSpawns > 0) //if there's at least one spawn day for this area
                    {
                        //if save data already exists for this area
                        if (save.LNOSCounter.ContainsKey(area.UniqueAreaID))
                        {
                            Monitor.Log("Sava data found for this area; checking spawn days counter...", LogLevel.Trace);
                            //if there's still at least one spawn day remaining
                            if ((area.ExtraConditions.LimitedNumberOfSpawns - save.LNOSCounter[area.UniqueAreaID]) > 0)
                            {
                            Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns - save.LNOSCounter[area.UniqueAreaID]}. Spawn allowed.", LogLevel.Trace);
                            save.LNOSCounter[area.UniqueAreaID]++; //increment (NOTE: this change needs to be saved at the end of the day)
                            }
                            else //no spawn days remaining
                            {
                                Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns - save.LNOSCounter[area.UniqueAreaID]}. Spawn disabled.", LogLevel.Trace);
                                return false; //prevent spawning
                            }
                        }
                        else //no save file exists for this area; behave as if LNOSCounter == 0
                        {
                            Monitor.Log("No save data found for this area; creating new counter.", LogLevel.Trace);
                            save.LNOSCounter.Add(area.UniqueAreaID, 1); //new counter for this area, starting at 1
                            Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns}. Spawn allowed.", LogLevel.Trace);
                        }
                    }
                    else //no spawns remaining
                    {
                        Monitor.Log($"Spawns remaining (including today): {area.ExtraConditions.LimitedNumberOfSpawns}. Spawn disabled.", LogLevel.Trace);
                        return false; //prevent spawning
                    }
                }

                return true; //all extra conditions allow for spawning
            }

            /// <summary>Checks whether a config file should be used with the currently loaded farm.</summary>
            /// <param name="config">The FarmConfig to be checked.</param>
            /// <returns>True if the file should be used with the current farm; false otherwise.</returns>
            public static bool CheckFileConditions(FarmConfig config, IContentPack pack, IModHelper helper)
            {
                Monitor.Log("Checking file conditions...", LogLevel.Trace);

                //check "reset main data folder" flag
                //NOTE: it's preferable to do this as the first step; it's intended to be a one-off cleaning process, rather than a conditional effect
                if (config.File_Conditions.ResetMainDataFolder && MConfig.EnableContentPackFileChanges) //if "reset" is true and file changes are enabled
                {
                    if (pack != null) //if this is part of a content pack
                    {
                        //attempt to load the content pack's global save data
                        ContentPackSaveData packSave = null;
                        try
                        {
                            packSave = pack.ReadJsonFile<ContentPackSaveData>(Path.Combine("data", "ContentPackSaveData.save")); //load the content pack's global save data (null if it doesn't exist)
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Warning: This content pack's save data could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"Affected file: data/ContentPackSaveData.save", LogLevel.Warn);
                            Monitor.Log($"Please delete the file and/or contact the mod's developer.", LogLevel.Warn);
                            Monitor.Log($"The content pack will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                            Monitor.Log($"----------", LogLevel.Warn);
                            Monitor.Log($"{ex.Message}", LogLevel.Warn);
                            return false; //disable this content pack's config, since it may require this process to function
                        }

                        if (packSave == null) //no global save data exists for this content pack
                        {
                            packSave = new ContentPackSaveData();
                        }

                        if (!packSave.MainDataFolderReset) //if this content pack has NOT reset the main data folder yet
                        {
                            Monitor.Log($"ResetMainDataFolder requested by content pack: {pack.Manifest.Name}", LogLevel.Debug);
                            string dataPath = Path.Combine(helper.DirectoryPath, "data"); //the path to this mod's data folder
                            DirectoryInfo dataFolder = new DirectoryInfo(dataPath); //an object representing this mod's data directory

                            if (dataFolder.Exists) //the data folder exists
                            {
                                Monitor.Log("Attempting to archive data folder...", LogLevel.Trace);
                                try
                                {
                                    string archivePath = Path.Combine(helper.DirectoryPath, "data", "archive", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                                    DirectoryInfo archiveFolder = Directory.CreateDirectory(archivePath); //create a timestamped archive folder
                                    foreach (FileInfo file in dataFolder.GetFiles()) //for each file in dataFolder
                                    {
                                        file.MoveTo(Path.Combine(archiveFolder.FullName, file.Name)); //move each file to archiveFolder
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Monitor.Log($"Warning: This content pack attempted to archive Farm Type Manager's data folder but failed: {pack.Manifest.Name}", LogLevel.Warn);
                                    Monitor.Log($"Please report this issue to Farm Type Manager's developer. This might also be fixed by manually removing your FarmTypeManager/data/ files.", LogLevel.Warn);
                                    Monitor.Log($"The content pack will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                                    Monitor.Log($"----------", LogLevel.Warn);
                                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                                    return false; //disable this content pack's config, since it may require this process to function
                                }
                            }
                            else //the data folder doesn't exist
                            {
                                Monitor.Log("Data folder not found; assuming it was deleted or not yet generated.", LogLevel.Trace);
                            }

                            packSave.MainDataFolderReset = true; //update save data
                        }

                        pack.WriteJsonFile(Path.Combine("data", "ContentPackSaveData.save"), packSave); //update the content pack's global save data file
                        Monitor.Log("Data folder archive successful.", LogLevel.Trace);
                    }
                    else //if this is NOT part of a content pack
                    {
                        Monitor.Log("This farm's config file has ResetMainDataFolder = true, but this setting only works for content packs.", LogLevel.Info);
                    }
                }

                //check farm type
                if (config.File_Conditions.FarmTypes != null && config.File_Conditions.FarmTypes.Length > 0)
                {
                    Monitor.Log("Farm type condition(s) found. Checking...", LogLevel.Trace);

                    bool validType = false;

                    foreach (string type in config.File_Conditions.FarmTypes) //for each listed farm type
                    {
                        if (type.Equals("All", StringComparison.OrdinalIgnoreCase) || type.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validType = true;
                            break; //skip the rest of these checks
                        }

                        switch (Game1.whichFarm) //compare to the current farm type
                        {
                            case (int)Utility.FarmTypes.Standard:
                                if (type.Equals("Standard", StringComparison.OrdinalIgnoreCase) || type.Equals("Default", StringComparison.OrdinalIgnoreCase) || type.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                                {
                                    validType = true;
                                }
                                break;
                            case (int)Utility.FarmTypes.Riverland:
                                if (type.Equals("Riverland", StringComparison.OrdinalIgnoreCase) || type.Equals("Fishing", StringComparison.OrdinalIgnoreCase) || type.Equals("Fish", StringComparison.OrdinalIgnoreCase))
                                {
                                    validType = true;
                                }
                                break;
                            case (int)Utility.FarmTypes.Forest:
                                if (type.Equals("Forest", StringComparison.OrdinalIgnoreCase) || type.Equals("Foraging", StringComparison.OrdinalIgnoreCase) || type.Equals("Forage", StringComparison.OrdinalIgnoreCase) || type.Equals("Woodland", StringComparison.OrdinalIgnoreCase))
                                {
                                    validType = true;
                                }
                                break;
                            case (int)Utility.FarmTypes.Hilltop:
                                if (type.Equals("Hill-top", StringComparison.OrdinalIgnoreCase) || type.Equals("Hilltop", StringComparison.OrdinalIgnoreCase) || type.Equals("Mining", StringComparison.OrdinalIgnoreCase) || type.Equals("Mine", StringComparison.OrdinalIgnoreCase))
                                {
                                    validType = true;
                                }
                                break;
                            case (int)Utility.FarmTypes.Wilderness:
                                if (type.Equals("Wilderness", StringComparison.OrdinalIgnoreCase) || type.Equals("Combat", StringComparison.OrdinalIgnoreCase) || type.Equals("Monster", StringComparison.OrdinalIgnoreCase))
                                {
                                    validType = true;
                                }
                                break;
                        }

                        if (validType) //if a valid weather condition was listed
                        {
                            break; //skip the rest of these checks
                        }
                    }

                    if (validType) //if a valid farm type was listed
                    {
                        Monitor.Log("Farm type matched a setting. File allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Farm type did NOT match any settings. File disabled.", LogLevel.Trace);
                        return false; //prevent config use
                    }
                }

                //check farmer name
                if (config.File_Conditions.FarmerNames != null && config.File_Conditions.FarmerNames.Length > 0)
                {
                    Monitor.Log("Farmer name condition(s) found. Checking...", LogLevel.Trace);

                    bool validName = false;

                    foreach (string name in config.File_Conditions.FarmerNames) //for each listed name
                    {
                        if (name.Equals(Game1.player.Name, StringComparison.OrdinalIgnoreCase)) //if the name matches the current player's
                        {
                            validName = true;
                            break; //skip the rest of these checks
                        }
                    }

                    if (validName) //if a valid farmer name was listed
                    {
                        Monitor.Log("Farmer name matched a setting. File allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Farmer name did NOT match any settings. File disabled.", LogLevel.Trace);
                        return false; //prevent config use
                    }
                }

                //check save file names (technically the save folder name)
                if (config.File_Conditions.SaveFileNames != null && config.File_Conditions.SaveFileNames.Length > 0)
                {
                    Monitor.Log("Save file name condition(s) found. Checking...", LogLevel.Trace);

                    bool validSave = false;

                    foreach (string saveName in config.File_Conditions.SaveFileNames) //for each listed save name
                    {
                        if (saveName.Equals(Constants.SaveFolderName, StringComparison.OrdinalIgnoreCase)) //if the name matches the current player's save folder name
                        {
                            validSave = true;
                            break; //skip the rest of these checks
                        }
                    }

                    if (validSave) //if a valid save name was listed
                    {
                        Monitor.Log("Save file name matched a setting. File allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Save file name did NOT match any settings. File disabled.", LogLevel.Trace);
                        return false; //prevent config use
                    }
                }

                return true; //all checks were successful; config should be used
            }

            /// <summary>Loads all available data files for the current farm into FarmDataList. Checks the mod's data folder and any relevant content packs.</summary>
            /// <param name="helper">SMAPI interface, used here to access files.</param>
            /// <returns>True if the files loaded successfully; false otherwise.</returns>
            public static void LoadFarmData(IModHelper helper)
            {
                Monitor.Log("Beginning file loading process...", LogLevel.Trace);

                //clear any existing farm data
                FarmDataList = new List<FarmData>();

                FarmConfig config; //temp for the current config as it's loaded
                InternalSaveData save; //temp for the current save as it's loaded

                if (MConfig.EnableContentPacks) //if content packs are enabled
                {
                    //load data from each relevant content pack
                    foreach (IContentPack pack in helper.ContentPacks.GetOwned())
                    {
                        Monitor.Log($"Loading files from content pack: {pack.Manifest.Name}", LogLevel.Trace);

                        //clear each temp object
                        config = null;
                        save = null;

                        //attempt to load the farm config from this pack
                        try
                        {
                            config = pack.ReadJsonFile<FarmConfig>($"content.json"); //load the content pack's farm config (null if it doesn't exist)
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Warning: This content pack could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"Please edit the content.json file or reinstall the content pack. The auto-generated error message is displayed below:", LogLevel.Warn);
                            Monitor.Log($"----------", LogLevel.Warn);
                            Monitor.Log($"{ex.Message}", LogLevel.Warn);
                            continue; //skip to the next content pack
                        }

                        if (config == null) //no config file found for this farm
                        {
                            Monitor.Log($"Warning: The content.json file for this content pack could not be found: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"Please reinstall the content pack. If you are its author, please create a config file named content.json in the pack's main folder (not the /data/ folder).", LogLevel.Warn);
                            continue; //skip to the next content pack
                        }

                        //attempt to load the save data for this pack and specific farm
                        try
                        {
                            save = pack.ReadJsonFile<InternalSaveData>($"data/{Constants.SaveFolderName}_SaveData.save"); //load the content pack's save data for this farm (null if it doesn't exist)
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Warning: Your farm's save data for this content pack could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"This file will need to be edited or deleted: data/{Constants.SaveFolderName}_SaveData.save", LogLevel.Warn);
                            Monitor.Log($"The content pack will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                            Monitor.Log($"----------", LogLevel.Warn);
                            Monitor.Log($"{ex.Message}", LogLevel.Warn);
                            continue; //skip to the next content pack
                        }

                        if (save == null) //no save file found for this farm
                        {
                            save = new InternalSaveData(); //load the (built-in) default save settings
                        }

                        ValidateFarmData(config, pack); //validate certain data in the current file before using it

                        pack.WriteJsonFile($"content.json", config); //update the content pack's config file
                        pack.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), save); //create or update the content pack's save file for the current farm

                        if (CheckFileConditions(config, pack, helper)) //check file conditions; only use the current data if this returns true
                        {
                            FarmDataList.Add(new FarmData(config, save, pack)); //add the config, save, and content pack to the farm data list
                            Monitor.Log("Content pack loaded successfully.", LogLevel.Trace);
                        }
                    }

                    Monitor.Log("All available content packs checked.", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Content packs disabled in config.json. Skipping to local files...", LogLevel.Trace);
                }

                //clear each temp object
                config = null;
                save = null;

                Monitor.Log("Loading files from FarmTypeManager/data", LogLevel.Trace);

                //attempt to load the farm config from this mod's data folder
                //NOTE: this should always be done *after* content packs, because it will end the loading process if an error occurs
                try
                {
                    config = helper.Data.ReadJsonFile<FarmConfig>(Path.Combine("data", $"{Constants.SaveFolderName}.json")); //load the current save's config file (null if it doesn't exist)
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Warning: This file could not be parsed correctly: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Warn);
                    Monitor.Log($"Please edit the file, or delete it and reload your farm to generate a new config file.", LogLevel.Warn);
                    Monitor.Log($"Your config file will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                    Monitor.Log($"----------", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return; //end this process without adding this set of farm data
                }

                if (config == null) //no config file found for this farm
                {
                    //attempt to load the default.json config file
                    try
                    {
                        config = helper.Data.ReadJsonFile<FarmConfig>(Path.Combine("data", "default.json")); //load the default.json config file (null if it doesn't exist)
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Warning: This file could not be parsed correctly: FarmTypeManager/data/default.json", LogLevel.Warn);
                        Monitor.Log($"Please edit the file, or delete it and reload your farm to generate a new default config file.", LogLevel.Warn);
                        Monitor.Log($"Your config file will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                        Monitor.Log($"----------", LogLevel.Warn);
                        Monitor.Log($"{ex.Message}", LogLevel.Warn);
                        return; //end this process without adding this set of farm data
                    }

                    if (config == null) //no default.json config file
                    {
                        config = new FarmConfig(); //load the (built-in) default config settings
                    }

                    ValidateFarmData(config, null); //validate certain data in the current file before using it

                    helper.Data.WriteJsonFile(Path.Combine("data", "default.json"), config); //create or update the default.json config file
                }

                //attempt to load the save data for this farm
                try
                {
                    save = helper.Data.ReadJsonFile<InternalSaveData>(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save")); //load the mod's save data for this farm (null if it doesn't exist)
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Warning: This file could not be parsed correctly: FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save", LogLevel.Warn);
                    Monitor.Log($"Please edit the file, or delete it and reload your farm to generate a new savedata file.", LogLevel.Warn);
                    Monitor.Log($"Your config file will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                    Monitor.Log($"----------", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return; //end this process without adding this set of farm data
                }

                if (save == null) //no save file found for this farm
                {
                    save = new InternalSaveData(); //load the (built-in) default save settings
                }

                ValidateFarmData(config, null); //validate certain data in the current file before using it

                helper.Data.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}.json"), config); //create or update the config file for the current farm
                helper.Data.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), save); //create or update this config's save file for the current farm

                if (CheckFileConditions(config, null, helper)) //check file conditions; only use the current data if this returns true
                {
                    FarmDataList.Add(new FarmData(config, save, null)); //add the config, save, and a *null* content pack to the farm data list
                    Monitor.Log("FarmTypeManager/data farm data loaded successfully.", LogLevel.Trace);
                }
            }

            /// <summary>Validates a single instance of farm data, correcting certain settings automatically.</summary>
            /// <param name="config">The contents of a single config file to be validated.</param>
            /// <param name="pack">The content pack associated with this config data; null if the file was from this mod's own folders.</param>
            public static void ValidateFarmData(FarmConfig config, IContentPack pack)
            {
                if (pack != null)
                {
                    Monitor.Log($"Validating data from content pack: {pack.Manifest.Name}", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Validating data from FarmTypeManager/data", LogLevel.Trace);
                }

                List<SpawnArea[]> allAreas = new List<SpawnArea[]>(); //a unified list of each "Areas" array in this config file
                allAreas.Add(config.Forage_Spawn_Settings.Areas);
                allAreas.Add(config.Large_Object_Spawn_Settings.Areas);
                allAreas.Add(config.Ore_Spawn_Settings.Areas);

                HashSet<string> IDs = new HashSet<string>(); //a record of all unique IDs encountered during this process

                Monitor.Log("Checking for duplicate UniqueAreaIDs...", LogLevel.Trace);

                //erase any duplicate IDs and record the others in the "IDs" hashset
                foreach (SpawnArea[] areas in allAreas) //for each "Areas" array in allAreas
                {
                    foreach (SpawnArea area in areas) //for each area in the current array
                    {
                        if (String.IsNullOrWhiteSpace(area.UniqueAreaID) || area.UniqueAreaID.ToLower() == "null") //if the area ID is null, blank, or the string "null" (to account for user confusion)
                        {
                            continue; //this name will be replaced later, so ignore it for now
                        }

                        if (IDs.Contains(area.UniqueAreaID)) //if this area's ID was already encountered
                        {
                            Monitor.Log($"Duplicate UniqueAreaID found: \"{area.UniqueAreaID}\" will be renamed.", LogLevel.Info);
                            if (pack != null) //if this config is from a content pack
                            {
                                Monitor.Log($"Content pack: {pack.Manifest.Name}", LogLevel.Info);
                                Monitor.Log($"If this happens after updating another mod, it might cause certain conditions (such as one-time-only spawns) to reset in that area.", LogLevel.Info);
                            }

                            area.UniqueAreaID = ""; //erase this area's ID, marking it for replacement
                        }
                        else //if this ID is unique so far
                        {
                            IDs.Add(area.UniqueAreaID); //add the area to the ID set
                        }
                    }
                }

                Monitor.Log("Assigning new UniqueAreaIDs to any blanks or duplicates...", LogLevel.Trace);

                string newName; //temp storage for a new ID while it's created/tested
                int newNumber; //temp storage for the numeric part of a new ID

                //create new IDs for any empty ones
                foreach (SpawnArea[] areas in allAreas) //for each "Areas" array in allAreas
                {
                    foreach (SpawnArea area in areas) //for each area in the current array
                    {
                        if (String.IsNullOrWhiteSpace(area.UniqueAreaID) || area.UniqueAreaID.ToLower() == "null") //if the area ID is null, blank, or the string "null" (to account for user confusion)
                        {
                            //create a new name, based on which type of area this is
                            newName = area.MapName;
                            if (area is ForageSpawnArea) { newName += " forage area "; }
                            else if (area is LargeObjectSpawnArea) { newName += " large object area "; }
                            else if (area is OreSpawnArea) { newName += " ore area "; } 
                            else { newName += " area "; }

                            newNumber = 1;

                            while (IDs.Contains(newName + newNumber)) //if this ID wouldn't be unique
                            {
                                newNumber++; //increment and try again
                            }

                            area.UniqueAreaID = newName + newNumber; //apply the new unique ID
                            Monitor.Log($"New UniqueAreaID assigned: {area.UniqueAreaID}", LogLevel.Trace);
                        }

                        IDs.Add(area.UniqueAreaID); //the ID is finalized, so add it to the set of encountered IDs
                    }
                }

                Monitor.Log("Checking for valid min/max spawn settings...", LogLevel.Trace);
                foreach (SpawnArea[] areas in allAreas) //for each "Areas" array in allAreas
                {
                    foreach (SpawnArea area in areas) //for each area in the current array
                    {
                        if (area.MinimumSpawnsPerDay > area.MaximumSpawnsPerDay) //if max spawns > min spawns
                        {
                            //swap the two numbers
                            int temp = area.MinimumSpawnsPerDay;
                            area.MinimumSpawnsPerDay = area.MaximumSpawnsPerDay;
                            area.MaximumSpawnsPerDay = temp;

                            Monitor.Log($"Min > max spawns in this area: \"{area.UniqueAreaID}\" ({area.MapName}). Numbers swapped.", LogLevel.Trace);
                        }
                    }
                }

                if (pack != null)
                {
                    Monitor.Log($"Validation complete for content pack: {pack.Manifest.Name}", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Validation complete for FarmTypeManager/data", LogLevel.Trace);
                }
                return;
            }

            /// <summary>Encapsulates IMonitor.Log() for this mod's static classes. Must be given an IMonitor in the ModEntry class to produce output.</summary>
            public static class Monitor
            {
                private static IMonitor monitor;

                public static IMonitor IMonitor
                {
                    set
                    {
                        monitor = value;
                    }
                }

                /// <summary>Log a message for the player or developer.</summary>
                /// <param name="message">The message to log.</param>
                /// <param name="level">The log severity level.</param>
                public static void Log(string message, LogLevel level = LogLevel.Debug)
                {
                    if (monitor != null)
                    {
                        monitor.Log(message, level);
                    }
                }
            }

            /// <summary>Parses today's weather from several booleans into a "Weather" enum.</summary>
            /// <returns>A "Weather" enum describing today's weather.</returns>
            public static Weather WeatherForToday()
            {
                if (Game1.isLightning)
                    return Weather.Lightning; //note this has to be completed before "isRaining", because both are true during storms
                else if (Game1.isRaining)
                    return Weather.Rain;
                else if (Game1.isSnowing)
                    return Weather.Snow;
                else if (Game1.isDebrisWeather)
                    return Weather.Debris;
                else
                    return Weather.Sunny;
            }

            /// <summary>A list of all config data for the current farm, related save data, and content pack (if applicable).</summary>
            public static List<FarmData> FarmDataList = new List<FarmData>();

            /// <summary>The global settings for this mod. Should be set during mod startup.</summary>
            public static ModConfig MConfig { get; set; }

            /// <summary>Random number generator shared throughout the mod. Initialized automatically.</summary>
            public static Random RNG { get; } = new Random();

            /// <summary>Enumerated list of farm types, in the order used by Stardew's internal code (e.g. Farm.cs)</summary>
            public enum FarmTypes { Standard, Riverland, Forest, Hilltop, Wilderness }

            /// <summary>Enumerated list of player skills, in the order used by Stardew's internal code (e.g. Farmer.cs).</summary>
            public enum Skills { Farming, Fishing, Foraging, Mining, Combat, Luck }

            /// <summary>Enumerated list of weather condition types, in the order used by Stardew's internal code (e.g. Game1.cs)</summary>
            public enum Weather { Sunny, Rain, Debris, Lightning, Festival, Snow, Wedding }
        }
    }
}

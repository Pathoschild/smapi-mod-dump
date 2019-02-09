using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
    public static class Utility
    {
        /// <summary>Generates a list of all valid tiles for object spawning in the provided SpawnArea.</summary>
        /// <param name="area">A SpawnArea listing an in-game map name and the valid regions/terrain within it that may be valid spawn points.</param>
        /// <param name="customTileIndex">The list of custom tile indices for this spawn process (e.g. forage or ore generation). Found in the relevant section of Utility.Config.</param>
        /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
        /// <returns>A completed list of all valid tile coordinates for this spawn process in this SpawnArea.</returns>
        public static List<Vector2> GenerateTileList(SpawnArea area, int[] customTileIndex, bool isLarge)
        {
            List<Vector2> validTiles = new List<Vector2>(); //list of all open, valid tiles for new spawns on the current map

            foreach (string type in area.AutoSpawnTerrainTypes) //loop to auto-detect valid tiles based on various types of terrain
            {
                if (type.Equals("quarry", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "quarry" tile index list
                {
                    validTiles.AddRange(Utility.GetTilesByIndex(area, Utility.Config.QuarryTileIndex, isLarge));
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
            Random rng = new Random();
            StardewValley.Object ore = null; //ore object, to be spawned into the world later
            switch (oreName.ToLower()) //avoid any casing issues in method calls by making this lower-case
            {
                case "stone":
                    ore = new StardewValley.Object(tile, 668 + (rng.Next(2) * 2), 1); //either of the two random stones spawned in the vanilla hilltop quarry
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
                    ore = new StardewValley.Object(tile, (rng.Next(7) + 1) * 2, "Stone", true, false, false, false); //any of the possible gem rocks
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
            Random rng = new Random(); //DEVNOTE: "Game1.random" exists, but causes some odd spawn behavior; using this for now...
            int spawnCount = rng.Next(min, max + 1); //random number from min to max (higher number is exclusive, so +1 to adjust for it)

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

            if (rng.NextDouble() < remainder) //use remainder as a % chance to spawn one extra object (e.g. if the final count would be "1.7", there's a 70% chance of spawning 2 objects)
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
        /// <returns>True if objects are allowed to spawn. False if any extra conditions should prevent spawning.</returns>
        public static bool CheckExtraConditions(SpawnArea area)
        {
            //check years
            if (area.ExtraConditions.Years != null && area.ExtraConditions.Years.Length > 0)
            {
                bool validYear = false;

                foreach (string year in area.ExtraConditions.Years)
                {
                    try //watch for errors related to string parsing
                    {
                        if (year.Equals("All", StringComparison.OrdinalIgnoreCase) || year.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                        {
                            validYear = true;
                            break; //skip the rest of the "day" checks
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
                    catch (Exception ex)
                    {
                        Monitor.Log($"Issue: This part of the extra condition \"Years\" for the {area.MapName} map isn't formatted correctly: \"{year}\"", LogLevel.Info);
                    }
                }

                    if (validYear != true)
                {
                    return false;
                }
            }

            //check seasons
            if (area.ExtraConditions.Seasons != null && area.ExtraConditions.Seasons.Length > 0)
            {
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

                if (validSeason != true) //if no valid listing for the current season was found
                {
                    return false; //prevent spawning
                }
            }

            //check days
            if (area.ExtraConditions.Days != null && area.ExtraConditions.Days.Length > 0)
            {
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
                    catch (Exception ex)
                    {
                        Monitor.Log($"Issue: This part of the extra condition \"Days\" for the {area.MapName} map isn't formatted correctly: \"{day}\"", LogLevel.Info);
                    }
                }

                if (validDay != true) //if no valid listing for the current day was found
                {
                    return false; //prevent spawning
                }
            }

            //check yesterday's weather
            if (area.ExtraConditions.WeatherYesterday != null && area.ExtraConditions.WeatherYesterday.Length > 0)
            {
                bool validWeather = false;

                foreach (string weather in area.ExtraConditions.WeatherYesterday) //for each listed weather name
                {
                    if (weather.Equals("All", StringComparison.OrdinalIgnoreCase) || weather.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if "all" or "any" is listed
                    {
                        validWeather = true;
                        break; //skip the rest of these checks
                    }

                    switch (Config.Internal_Save_Data.WeatherForYesterday) //compare to yesterday's weather
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

                if (validWeather != true) //if no valid listing for yesterday's weather was found
                {
                    return false; //prevent spawning
                }
            }

            //check today's weather
            if (area.ExtraConditions.WeatherToday != null && area.ExtraConditions.WeatherToday.Length > 0)
            {
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

                if (validWeather != true) //if no valid listing for today's weather was found
                {
                    return false; //prevent spawning
                }
            }

            //check tomorrow's weather
            if (area.ExtraConditions.WeatherTomorrow != null && area.ExtraConditions.WeatherTomorrow.Length > 0)
            {
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

                if (validWeather != true) //if no valid listing for yesterday's weather was found
                {
                    return false; //prevent spawning
                }
            }


            //check number of spawns (NOTE: it's important that this is the last condition checked, because otherwise it might count down while not actually spawning (blocked by another condition)
            if (area.ExtraConditions.LimitedNumberOfSpawns != null)
            {
                if (area.ExtraConditions.LimitedNumberOfSpawns > 0) //still has spawns remaining
                {
                    area.ExtraConditions.LimitedNumberOfSpawns--; //decrement (note that it's necessary to save this update to the config file; this is done elsewhere)
                }
                else //no spawns remaining
                {
                    return false; //prevent spawning
                }
            }

            return true; //all extra conditions allow for spawning
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

        /// <summary>Data contained in the per-character configuration file, including various mod settings.</summary>
        public static FarmConfig Config { get; set; } = null;

        /// <summary>Enumerated list of player skills, in the order used by Stardew's internal code (e.g. Farmer.cs).</summary>
        public enum Skills { Farming, Fishing, Foraging, Mining, Combat, Luck }

        /// <summary>Enumerated list of weather condition types, in the order used by Stardew's internal code (e.g. Game1.cs)</summary>
        public enum Weather { Sunny, Rain, Debris, Lightning, Festival, Snow, Wedding }
    }
}

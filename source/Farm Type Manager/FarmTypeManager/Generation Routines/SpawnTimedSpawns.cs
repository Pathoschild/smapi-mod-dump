using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        private static partial class Generation
        {
            /// <summary>Spawns provided objects where their spawn times match the provided time, then removes them from the provided list.</summary>
            /// <param name="timedSpawns">A list of timed spawn objects to be spawned. Spawned objects will be removed from this list.</param>
            /// <param name="time">An in-game time value. If provided, only objects with matching SpawnTime values will be spawned.</param>
            public static void SpawnTimedSpawns(List<List<TimedSpawn>> timedSpawns, StardewTime? time = null)
            {
                Utility.Monitor.VerboseLog($"Spawning objects set to appear at time: {time?.Time.ToString() ?? "(any)"}...");

                int spawnedTotal = 0; //tracks the number of objects spawned during this process
                bool filter(TimedSpawn spawn) => spawn.SavedObject.SpawnTime == time; //define a filter that is true when a TimedSpawn matches the provided time

                for (int x = timedSpawns.Count - 1; x >= 0; x--) //for each list of spawns (looping backward for removal purposes)
                {
                    //NOTE: the spawns in each "spawns" list must share the same SpawnArea for optimization reasons
                    List<TimedSpawn> spawns = null; //refer to the current list as "spawns"
                    int spawnedByThisList = 0; //tracks the number of objects spawned from this list

                    if (time.HasValue == false) //if no time was provided
                    {
                        spawns = timedSpawns[x]; //don't filter the list
                        timedSpawns[x].RemoveAt(x); //remove the original list
                    }
                    else //if a time was provided
                    {
                        spawns = timedSpawns[x].Where(filter).ToList(); //get a list of spawns with matching times
                        timedSpawns[x].RemoveAll(filter); //remove the matching spawns from the original list

                        if (timedSpawns[x].Count <= 0) //if the original list is now empty
                        {
                            timedSpawns.RemoveAt(x); //remove it
                        }
                    }

                    if (spawns.Count <= 0) //if nothing in the list had a matching time
                    {
                        continue; //skip to the next list
                    }

                    GameLocation location = Game1.getLocationFromName(spawns[0].SavedObject.MapName); //get the location matching this object's map name (NOTE: do not use the area name, which may be different)

                    //validate the "only spawn if a player is present" setting
                    if (spawns[0].SpawnArea.SpawnTiming.OnlySpawnIfAPlayerIsPresent)
                    {
                        FarmerCollection farmers = Game1.getOnlineFarmers(); //get all active players

                        bool playerIsPresent = false;
                        foreach (Farmer farmer in farmers)
                        {
                            if (farmer.currentLocation == location) //if this farmer is at the current location
                            {
                                playerIsPresent = true;
                                break;
                            }
                        }

                        if (!playerIsPresent) //if no players are present
                        {
                            Utility.Monitor.VerboseLog($"Skipping spawns for this location because no players are present: {spawns[0].SpawnArea.UniqueAreaID} ({spawns[0].SpawnArea.MapName})");
                            continue; //skip to the next list
                        }
                    }

                    bool isLarge = spawns[0].SavedObject.Type == SavedObject.ObjectType.LargeObject; //whether these spawns are large (2x2 tiles); checked via the first spawn in the list
                    int[] customTiles = { }; //the set of custom tiles to use (to be selected based on the spawn object's type)
                    int? monstersAtLocation = null; //the number of existing monsters at a location (used to optionally limit monster spawns)

                    switch (spawns[0].SavedObject.Type)
                    {
                        case SavedObject.ObjectType.Forage:
                            customTiles = spawns[0].FarmData.Config.Forage_Spawn_Settings.CustomTileIndex;
                            break;
                        case SavedObject.ObjectType.LargeObject:
                            customTiles = spawns[0].FarmData.Config.Large_Object_Spawn_Settings.CustomTileIndex;
                            break;
                        case SavedObject.ObjectType.Ore:
                            customTiles = spawns[0].FarmData.Config.Ore_Spawn_Settings.CustomTileIndex;
                            break;
                        case SavedObject.ObjectType.Monster:
                            customTiles = spawns[0].FarmData.Config.Monster_Spawn_Settings.CustomTileIndex;

                            if (Utility.MConfig.MonsterLimitPerLocation.HasValue) //if a per-location monster limit was provided
                            {
                                monstersAtLocation = location.characters.Count(character => character is Monster); //get the number of monsters at this location
                            }
                            break;
                    }

                    //generate a new list of valid tiles for this spawn area
                    List<Vector2> tiles = Utility.GenerateTileList(spawns[0].SpawnArea, location, spawns[0].FarmData.Save, spawns[0].FarmData.Config.QuarryTileIndex, customTiles);

                    for (int y = spawns.Count - 1; y >= 0; y--) //for each object to be spawned (looping backward for removal purposes)
                    {
                        if (Utility.MConfig.MonsterLimitPerLocation.HasValue && Utility.MConfig.MonsterLimitPerLocation <= monstersAtLocation) //if this location has reached a provided monster limit
                        {
                            break; //skip the rest of this spawn list
                        }

                        Vector2? chosenTile = null;
                        while (tiles.Count > 0 && !chosenTile.HasValue) //while potential tiles exist & a valid tile has not been chosen yet
                        {
                            int randomIndex = Utility.RNG.Next(tiles.Count); //get the array index for a random valid tile
                            if (Utility.IsTileValid(location, tiles[randomIndex], isLarge, spawns[y].SpawnArea.StrictTileChecking)) //if this tile is valid
                            {
                                chosenTile = tiles[randomIndex]; //choose this tile
                            }
                            tiles.RemoveAt(randomIndex); //remove the tile from the list
                        }

                        if (!chosenTile.HasValue) //if no remaining tiles were valid
                        {
                            break; //skip the rest of this spawn list
                        }

                        spawns[y].SavedObject.Tile = chosenTile.Value; //apply the random tile to this spawn  

                        //spawn the object based on its type
                        switch (spawns[y].SavedObject.Type)
                        {
                            case SavedObject.ObjectType.Forage:
                                Utility.SpawnForage(spawns[y].SavedObject.ID.Value, location, spawns[y].SavedObject.Tile); //spawn forage
                                break;
                            case SavedObject.ObjectType.LargeObject:
                                Utility.SpawnLargeObject(spawns[y].SavedObject.ID.Value, (Farm)location, spawns[y].SavedObject.Tile); //spawn large object
                                break;
                            case SavedObject.ObjectType.Ore:
                                int? oreID = Utility.SpawnOre(spawns[y].SavedObject.Name, location, spawns[y].SavedObject.Tile); //spawn ore and get its ID if successful
                                if (oreID.HasValue) //if the ore spawned successfully (i.e. generated an ID)
                                {
                                    spawns[y].SavedObject.ID = oreID.Value; //record this spawn's ID
                                }
                                break;
                            case SavedObject.ObjectType.Monster:
                                int? monID = Utility.SpawnMonster(spawns[y].SavedObject.MonType, location, spawns[y].SavedObject.Tile, spawns[y].SpawnArea.UniqueAreaID); //spawn monster and get its ID if successful
                                if (monID.HasValue) //if the monster spawned successfully (i.e. generated an ID)
                                {
                                    spawns[y].SavedObject.ID = monID.Value; //record this spawn's ID
                                    if (monstersAtLocation.HasValue) //if the monster counter is being used
                                    {
                                        monstersAtLocation++; //increment monster counter
                                    }
                                }
                                break;
                        }

                        if (spawns[y].SavedObject.ID.HasValue) //if this object spawned successfully
                        {
                            //increment the spawn trackers
                            spawnedTotal++;
                            spawnedByThisList++;

                            if (spawns[y].SavedObject.DaysUntilExpire.HasValue) //if this object has an expiration date
                            {
                                SavedObject saved = Utility.Clone(spawns[y].SavedObject); //clone this object to avoid any accidental modification
                                spawns[y].FarmData.Save.SavedObjects.Add(saved); //add the spawn to the relevant save data
                            }
                        }
                    }

                    if (spawns[0].SpawnArea.SpawnTiming.SpawnSound != null && spawns[0].SpawnArea.SpawnTiming.SpawnSound.Trim() != "") //if this area has a SpawnSound setting
                    {
                        if (spawnedByThisList > 0) //if anything was spawned by this list
                        {
                            try
                            {
                                Game1.soundBank.GetCue(spawns[0].SpawnArea.SpawnTiming.SpawnSound); //test whether this sound exists by retrieving it from the game's soundbank
                                //NOTE: the "playSound" method below can produce errors that aren't caught locally, so the above loading test is required
                                location.playSound(spawns[0].SpawnArea.SpawnTiming.SpawnSound); //play this area's SpawnSound
                            }
                            catch //if an exception is thrown while retrieving the sound
                            {
                                //ignore it; an alert should already have been generated by prior processes
                            }                            
                        }
                    }
                }
                Utility.Monitor.VerboseLog($"Spawn process complete for time: {time?.Time.ToString() ?? "(any)"}. Objects spawned: {spawnedTotal}.");
            }
        }
    }
}
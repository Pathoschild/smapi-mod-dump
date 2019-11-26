using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
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
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Check each saved object with an expiration setting, respawning them if they were removed overnight (e.g. by the weekly forage removal process).</summary>
            /// <param name="save">The save data to the checked.</param>
            public static void ReplaceProtectedSpawnsOvernight(InternalSaveData save)
            {
                int missing = 0; //# of objects missing
                int blocked = 0; //# of objects that could not respawn due to blocked locations
                int respawned = 0; //# of objects respawned
                int unloaded = 0; //# of objects skipped due to missing (unloaded) or invalid map names
                foreach (SavedObject saved in save.SavedObjects)
                {
                    if (saved.DaysUntilExpire == null) //if the object's expiration setting is null
                    {
                        continue; //skip to the next object
                    }

                    if (saved.Type == SavedObject.ObjectType.Monster) //if this is a monster
                    {
                        missing++; //increment missing tracker (note: monsters should always be removed overnight)

                        GameLocation location = Game1.getLocationFromName(saved.MapName); //get the object's location

                        if (location == null) //if the map wasn't found
                        {
                            unloaded++; //increment unloaded tracker
                            continue; //skip to the next object
                        }

                        //this mod should remove all of its monsters overnight, so respawn this monster without checking for its existence
                        int? newID = SpawnMonster(saved.MonType, location, saved.Tile, "[No Area ID: Respawning previously saved monster.]"); //respawn the monster and get its new ID (null if spawn failed)
                        if (newID.HasValue) //if a monster ID was generated
                        {
                            saved.ID = newID.Value; //update this monster's saved ID
                            respawned++; //increment respawn tracker
                        }
                        else //if spawn failed (presumably due to obstructions)
                        {
                            blocked++; //increment obstruction tracker
                        }
                    }
                    else if (saved.Type == SavedObject.ObjectType.LargeObject) //if this is a large object
                    {
                        GameLocation location = Game1.getLocationFromName(saved.MapName); //get the object's location

                        if (location == null) //if the map wasn't found
                        {
                            unloaded++; //increment unloaded tracker
                            continue; //skip to the next object
                        }

                        IEnumerable<TerrainFeature> resourceClumps = null; //a list of large objects at this location
                        if (location is Farm farm)
                        {
                            resourceClumps = farm.resourceClumps.ToList(); //use the farm's clump list
                        }
                        else if (location is MineShaft mine)
                        {
                            resourceClumps = mine.resourceClumps.ToList(); //use the mine's clump list
                        }
                        else
                        {
                            resourceClumps = location.largeTerrainFeatures.OfType<LargeResourceClump>(); //use this location's large resource clump list
                        }

                        bool stillExists = false; //does this large object still exist?

                        foreach (TerrainFeature clump in resourceClumps) //for each of this location's large objects
                        {
                            if (clump is ResourceClump smallClump)
                            {
                                if (smallClump.tile.X == saved.Tile.X && smallClump.tile.Y == saved.Tile.Y && smallClump.parentSheetIndex.Value == saved.ID) //if this clump's location & ID match the saved object
                                {
                                    stillExists = true;
                                    break; //stop searching the clump list
                                }
                            }
                            else if (clump is LargeResourceClump largeClump)
                            {
                                if (largeClump.Clump.Value.tile.X == saved.Tile.X && largeClump.Clump.Value.tile.Y == saved.Tile.Y && largeClump.Clump.Value.parentSheetIndex.Value == saved.ID) //if this clump's location & ID match the saved object
                                {
                                    stillExists = true;
                                    break; //stop searching the clump list
                                }
                            }
                        }

                        if (!stillExists) //if the object no longer exists
                        {
                            missing++; //increment missing tracker

                            if (IsTileValid(location, saved.Tile, true, "High")) //if the object's tile is valid for large object placement (defaulting to "high" strictness)
                            {
                                SpawnLargeObject(saved.ID.Value, location, saved.Tile); //respawn the object
                                respawned++; //increment respawn tracker
                            }
                            else //if the object's tile is invalid
                            {
                                blocked++; //increment obstruction tracker
                            }
                        }
                    }
                    else //if this is forage or ore
                    {
                        GameLocation location = Game1.getLocationFromName(saved.MapName); //get the object's location

                        if (location == null) //if the map wasn't found
                        {
                            unloaded++; //increment unloaded tracker
                            continue; //skip to the next object
                        }

                        StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location

                        if (realObject == null) //if the object no longer exists
                        {
                            missing++; //increment missing object tracker

                            if (!location.isTileOccupiedForPlacement(saved.Tile)) //if the object's tile is not occupied
                            {
                                if (saved.Type == SavedObject.ObjectType.Forage) //if this is forage
                                {
                                    SpawnForage(saved.ID.Value, location, saved.Tile); //respawn it
                                }
                                else //if this is ore
                                {
                                    SpawnOre(saved.Name, location, saved.Tile); //respawn it
                                }
                                respawned++; //increment respawn tracker
                            }
                            else //if the object's tile is occupied
                            {
                                blocked++; //increment obstruction tracker
                            }
                        }
                    }
                }

                Monitor.VerboseLog($"Missing objects: {missing}. Respawned: {respawned}. Not respawned due to obstructions: {blocked}. Skipped due to missing maps: {unloaded}.");
            }
        }
    }
}
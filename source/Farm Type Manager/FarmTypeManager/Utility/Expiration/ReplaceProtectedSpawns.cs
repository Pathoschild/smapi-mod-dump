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
            /// <summary>Check each saved object with an expiration setting, respawning them if they were removed after being saved (e.g. by the weekly forage removal process).</summary>
            /// <param name="save">The save data to the checked.</param>
            public static void ReplaceProtectedSpawns(InternalSaveData save)
            {
                int missing = 0; //# of objects missing
                int blocked = 0; //# of objects that could not respawn due to blocked locations
                int respawned = 0; //# of objects respawned
                int unloaded = 0; //# of objects skipped due to missing (unloaded) or invalid map names
                int uninstalled = 0; //# of objects skipped due to missing object data, generally caused by removed mods

                foreach (SavedObject saved in save.SavedObjects)
                {
                    if (saved.DaysUntilExpire == null) //if the object's expiration setting is null
                    {
                        continue; //skip to the next object
                    }

                    GameLocation location = Game1.getLocationFromName(saved.MapName); //get the object's location

                    if (location == null) //if the map wasn't found
                    {
                        unloaded++; //increment unloaded tracker
                        continue; //skip to the next object
                    }

                    if (saved.Type == SavedObject.ObjectType.Monster) //if this is a monster
                    {
                        missing++; //increment missing tracker (note: monsters should always be removed overnight)

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

                            if (IsTileValid(location, saved.Tile, saved.Size, "High")) //if the object's tile is valid for large object placement (defaulting to "high" strictness)
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
                    else if (saved.Type == SavedObject.ObjectType.Item) //if this is a forage item
                    {
                        missing++; //increment missing tracker (note: items should always be removed overnight)

                        //this mod should remove all of its forage items overnight, so respawn this item without checking for its existence
                        if (IsTileValid(location, saved.Tile, new Point(1, 1), "Medium")) //if the item's tile is clear enough to respawn
                        {
                            //update this item's ID, in case it changed due to other mods
                            string[] categoryAndName = saved.Name.Split(':');
                            int? newID = GetItemID(categoryAndName[0], categoryAndName[1]);

                            if (newID.HasValue) //if a new ID was successfully generated
                            {
                                respawned++; //increment respawn tracker
                                saved.ID = newID; //save the new ID
                                SpawnForage(saved, location, saved.Tile); //respawn the item
                            }
                            else //if a new ID could not be generated
                            {
                                uninstalled++; //increment uninstalled mod tracker
                                Monitor.Log($"Failed to generated a new ID for a saved forage item overnight. Item name: {saved.Name}", LogLevel.Trace);
                            }
                        }
                        else //if this object's tile is obstructed
                        {
                            blocked++; //increment obstruction tracker
                        }
                    }
                    else if (saved.Type == SavedObject.ObjectType.Container) //if this is a container
                    {
                        missing++; //increment missing tracker (note: chests should always be removed overnight)

                        //this mod should remove all of its containers overnight, so respawn this container without checking for its existence
                        if (IsTileValid(location, saved.Tile, new Point(1, 1), "Medium")) //if the container's tile is clear enough to respawn
                        {
                            respawned++; //increment respawn tracker
                            SpawnForage(saved, location, saved.Tile); //respawn the container
                        }
                        else //if this object's tile is obstructed
                        {
                            blocked++; //increment obstruction tracker
                        }
                    }
                    else //if this is forage or ore
                    {
                        StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location

                        if (realObject == null) //if the object no longer exists
                        {
                            missing++; //increment missing object tracker

                            if (IsTileValid(location, saved.Tile, new Point(1, 1), "Medium")) //if the object's tile is clear enough to respawn
                            {
                                if (saved.Type == SavedObject.ObjectType.Object) //if this is a forage object
                                {
                                    if (saved.Name != null) //if this forage was originally assigned a name
                                    {
                                        saved.ID = GetItemID("object", saved.Name); //update this forage's ID, in case it changed due to other mods
                                    }

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
                if (uninstalled > 0) //if any objects could not respawn due to missing mod data
                {
                    Monitor.Log($"{uninstalled} objects could not be respawned overnight due to missing item mods.", LogLevel.Debug);
                }
            }
        }
    }
}
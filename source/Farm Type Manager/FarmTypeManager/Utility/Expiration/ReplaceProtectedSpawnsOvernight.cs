using System;
using System.Collections.Generic;
using System.IO;
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

                    if (saved.Type == SavedObject.ObjectType.LargeObject) //if this is a large object
                    {
                        Farm farm = Game1.getLocationFromName(saved.MapName) as Farm; //get the specified location & treat it as a farm (null otherwise)

                        if (farm == null) //if this isn't a valid map
                        {
                            unloaded++; //increment unloaded tracker
                            //note: don't remove the object's save data; this might be a temporary issue, e.g. a map that didn't load correctly
                            continue; //skip to the next object
                        }

                        bool stillExists = false; //does this large object still exist?

                        //WARNING: this section accesses SDV "Net" objects; it does not edit them directly, but should be suspected if inconsistent errors occur
                        foreach (ResourceClump clump in farm.resourceClumps) //for each clump (a.k.a. large object) on this map
                        {
                            if (clump.tile.X == saved.Tile.X && clump.tile.Y == saved.Tile.Y && clump.parentSheetIndex.Value == saved.ID) //if this clump's location & ID match the saved object
                            {
                                stillExists = true;
                                break; //skip the rest of these clumps
                            }
                        }
                        //end of WARNING

                        if (!stillExists) //if the object no longer exists
                        {
                            missing++; //increment missing tracker

                            //if the object's tiles are not unoccupied
                            if (!farm.isTileOccupiedForPlacement(saved.Tile) && !farm.isTileOccupiedForPlacement(new Vector2(saved.Tile.X + 1, saved.Tile.Y)) && !farm.isTileOccupiedForPlacement(new Vector2(saved.Tile.X, saved.Tile.Y + 1)) && !farm.isTileOccupiedForPlacement(new Vector2(saved.Tile.X + 1, saved.Tile.Y + 1)))
                            {
                                SpawnLargeObject(saved.ID, farm, saved.Tile); //respawn it
                                respawned++; //increment respawn tracker
                            }
                            else //the tiles are occupied
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
                            //note: don't remove the object's save data; this might be a temporary issue, e.g. a map that didn't load correctly
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
                                    SpawnForage(saved.ID, location, saved.Tile); //respawn it
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
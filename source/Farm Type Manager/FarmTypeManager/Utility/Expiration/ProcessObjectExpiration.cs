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
            /// <summary>Check each saved object's expiration data, updating counters & removing missing/expired objects from the data and game.</summary>
            /// <param name="save">The save data to the checked.</param>
            public static void ProcessObjectExpiration(InternalSaveData save)
            {
                Monitor.Log($"Updating save data for missing/expired objects...", LogLevel.Trace);
                List<SavedObject> objectsToRemove = new List<SavedObject>(); //objects to remove from saved data after processing (note: do not remove them while looping through them)

                foreach (SavedObject saved in save.SavedObjects) //for each saved object & expiration countdown
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

                        if (stillExists) //if the object still exists
                        {
                            if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                            {
                                Monitor.Log($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).", LogLevel.Trace);
                                farm.removeEverythingExceptCharactersFromThisTile((int)saved.Tile.X, (int)saved.Tile.Y); //remove the object from the game
                                objectsToRemove.Add(saved); //mark object for removal from save
                            }
                            else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                            {
                                saved.DaysUntilExpire--; //decrease counter by 1
                            }
                        }
                        else //if the object no longer exists
                        {
                            objectsToRemove.Add(saved); //mark object for removal from save
                        }
                    }
                    else //if this is forage or ore
                    {
                        GameLocation location = Game1.getLocationFromName(saved.MapName); //get the object's location

                        if (location == null) //if the map wasn't found
                        {
                            //note: don't remove the object's save data; this might be a temporary issue, e.g. a map that didn't load correctly
                            continue; //skip to the next object
                        }

                        StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location

                        if (realObject != null && realObject.ParentSheetIndex == saved.ID) //if an object exists in the saved location & matches the saved object's ID
                        {
                            if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                            {
                                Monitor.Log($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).", LogLevel.Trace);
                                location.removeObject(saved.Tile, false); //remove the object from the game
                                objectsToRemove.Add(saved); //mark object for removal from save
                            }
                            else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                            {
                                saved.DaysUntilExpire--; //decrease counter by 1
                            }

                        }
                        else //if the object no longer exists
                        {
                            objectsToRemove.Add(saved); //mark object for removal from save
                        }
                    }

                }

                Monitor.Log($"Expiration check complete. Clearing {objectsToRemove.Count} missing/expired objects from save data.", LogLevel.Trace);
                foreach (SavedObject saved in objectsToRemove) //for each object that should be removed from the save data
                {
                    save.SavedObjects.Remove(saved); //remove it
                }
            }
        }
    }
}
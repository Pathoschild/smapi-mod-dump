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
using StardewValley.Monsters;

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
                List<SavedObject> objectsToRemove = new List<SavedObject>(); //objects to remove from saved data after processing (note: do not remove them while looping through them)

                foreach (SavedObject saved in save.SavedObjects) //for each saved object & expiration countdown
                {
                    if (saved.DaysUntilExpire == null && saved.Type != SavedObject.ObjectType.Monster) //if the object's expiration setting is null & it's not a monster
                    {
                        Monitor.VerboseLog($"Removing object data saved with a null expiration setting. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                        objectsToRemove.Add(saved); //mark this for removal from save
                        continue; //skip to the next object
                    }

                    if (saved.Type == SavedObject.ObjectType.Monster) //if this is a monster
                    {
                        GameLocation location = Game1.getLocationFromName(saved.MapName); //get the monster's location

                        if (location == null) //if this isn't a valid map
                        {
                            Monitor.VerboseLog($"Removing object data saved for a missing location. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark this for removal from save
                            continue; //skip to the next object
                        }

                        bool stillExists = false; //does this monster still exist?

                        for (int x = location.characters.Count - 1; x >= 0; x--) //for each character at this location (looping backward for removal purposes)
                        {
                            if (location.characters[x] is Monster monster && monster.id == saved.ID) //if this is a monster with an ID that matches the saved ID
                            {
                                stillExists = true;
                                if (saved.DaysUntilExpire == 1 || saved.DaysUntilExpire == null) //if this should expire tonight (including monsters generated without expiration settings)
                                {
                                    Monitor.VerboseLog($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                                    objectsToRemove.Add(saved); //mark this for removal from save
                                }
                                else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                                {
                                    saved.DaysUntilExpire--; //decrease counter by 1
                                }

                                if (saved.MonType != null && saved.MonType.Settings.ContainsKey("PersistentHP") && (bool)saved.MonType.Settings["PersistentHP"]) //if the PersistentHP setting is enabled for this monster
                                {
                                    saved.MonType.Settings["CurrentHP"] = monster.Health; //save this monster's current HP
                                }

                                location.characters.RemoveAt(x); //remove this monster from the location (NOTE: this must be done even for unexpired monsters to avoid SDV save errors)
                                break; //stop searching the character list
                            }
                        }

                        if (!stillExists) //if this monster no longer exists
                        {
                            objectsToRemove.Add(saved); //mark this for removal from save
                        }
                    }
                    else if (saved.Type == SavedObject.ObjectType.LargeObject) //if this is a large object
                    {
                        GameLocation location = Game1.getLocationFromName(saved.MapName); //get the specified location

                        if (location == null) //if this isn't a valid map
                        {
                            Monitor.VerboseLog($"Removing object data saved for a missing location. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark this for removal from save
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

                        TerrainFeature existingObject = null; //the in-game object, if it currently exists

                        foreach (TerrainFeature clump in resourceClumps) //for each of this location's large objects
                        {
                            if (clump is ResourceClump smallClump)
                            {
                                if (smallClump.tile.X == saved.Tile.X && smallClump.tile.Y == saved.Tile.Y && smallClump.parentSheetIndex.Value == saved.ID) //if this clump's location & ID match the saved object
                                {
                                    existingObject = smallClump;
                                    break; //stop searching the clump list
                                }
                            }
                            else if (clump is LargeResourceClump largeClump)
                            {
                                if (largeClump.Clump.Value.tile.X == saved.Tile.X && largeClump.Clump.Value.tile.Y == saved.Tile.Y && largeClump.Clump.Value.parentSheetIndex.Value == saved.ID) //if this clump's location & ID match the saved object
                                {
                                    existingObject = largeClump;
                                    break; //stop searching the clump list
                                }
                            }
                        }

                        if (existingObject != null) //if the object still exists
                        {
                            if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                            {
                                Monitor.VerboseLog($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");

                                if (existingObject is ResourceClump clump)
                                {
                                    if (location is Farm farmLoc)
                                    {
                                        farmLoc.resourceClumps.Remove(clump); //remove this object from the farm's resource clumps list
                                    }
                                    else if (location is MineShaft mineLoc)
                                    {
                                        mineLoc.resourceClumps.Remove(clump); //remove this object from the mine's resource clumps list
                                    }
                                }

                                objectsToRemove.Add(saved); //mark object for removal from save
                            }
                            else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                            {
                                saved.DaysUntilExpire--; //decrease counter by 1
                            }

                            if (existingObject is LargeResourceClump largeClump)
                            {
                                location.largeTerrainFeatures.Remove(largeClump); //remove this object from the large terrain features list (NOTE: this must be done even for unexpired LargeResourceClumps to avoid SDV save errors)
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
                            Monitor.VerboseLog($"Removing object data saved for a missing location. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark this for removal from save
                            continue; //skip to the next object
                        }

                        StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location

                        if (realObject != null && realObject.ParentSheetIndex == saved.ID) //if an object exists in the saved location & matches the saved object's ID
                        {
                            if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                            {
                                Monitor.VerboseLog($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                realObject.CanBeGrabbed = true; //workaround for certain objects being ignored by the removeObject method
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

                Monitor.Log($"Expiration check complete. Clearing {objectsToRemove.Count} missing/expired objects.", LogLevel.Trace);
                foreach (SavedObject saved in objectsToRemove) //for each object that should be removed from the save data
                {
                    save.SavedObjects.Remove(saved); //remove it
                }
            }
        }
    }
}
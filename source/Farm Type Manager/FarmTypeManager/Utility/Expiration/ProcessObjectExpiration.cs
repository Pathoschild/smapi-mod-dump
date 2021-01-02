/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

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
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Check each saved object's expiration data, updating counters & removing missing/expired objects from the data and custom classes from the game world.</summary>
            /// <param name="save">The save data to the checked.</param>
            /// <param name="endOfDay">If false, expiration dates will be ignored. Used to temporarily remove custom classes during the day.</param>
            public static void ProcessObjectExpiration(InternalSaveData save, bool endOfDay = true)
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

                    if (saved.DaysUntilExpire.HasValue //if this saved object has an expiration setting
                        && saved.MapName.StartsWith("UndergroundMine", StringComparison.OrdinalIgnoreCase)) //AND if this saved object was in a mine level (i.e. temporary location)
                    {
                        saved.DaysUntilExpire = 1; //force this object to expire below
                    }

                    GameLocation location = Game1.getLocationFromName(saved.MapName); //get the saved object's location

                    if (location == null) //if this isn't a valid map
                    {
                        Monitor.VerboseLog($"Removing object data saved for a missing location. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                        objectsToRemove.Add(saved); //mark this for removal from save
                        continue; //skip to the next object
                    }

                    if (saved.Type == SavedObject.ObjectType.Monster) //if this is a monster
                    {
                        bool stillExists = false; //does this monster still exist?

                        for (int x = location.characters.Count - 1; x >= 0; x--) //for each character at this location (looping backward for removal purposes)
                        {
                            if (location.characters[x] is Monster monster && monster.id == saved.ID) //if this is a monster with an ID that matches the saved ID
                            {
                                stillExists = true;
                                if (endOfDay) //if expirations should be processed
                                {
                                    if (saved.DaysUntilExpire == 1 || saved.DaysUntilExpire == null) //if this should expire tonight (including monsters generated without expiration settings)
                                    {
                                        Monitor.VerboseLog($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                                        objectsToRemove.Add(saved); //mark this for removal from save
                                    }
                                    else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                                    {
                                        saved.DaysUntilExpire--; //decrease counter by 1
                                    }
                                }

                                if (saved.MonType != null && saved.MonType.Settings.ContainsKey("PersistentHP") && (bool)saved.MonType.Settings["PersistentHP"]) //if the PersistentHP setting is enabled for this monster
                                {
                                    saved.MonType.Settings["CurrentHP"] = monster.Health; //save this monster's current HP
                                }

                                location.characters.RemoveAt(x); //remove this monster from the location, regardless of expiration
                                break; //stop searching the character list
                            }
                        }

                        if (!stillExists) //if this monster no longer exists
                        {
                            Monitor.VerboseLog($"Removing missing object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark this for removal from save
                        }
                    }
                    else if (saved.Type == SavedObject.ObjectType.ResourceClump) //if this is a resource clump
                    {
                        IEnumerable<TerrainFeature> resourceClumps = null; //a list of resource clumps at this location
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
                            if (endOfDay) //if expirations should be processed
                            {
                                if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                                {
                                    Monitor.VerboseLog($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");

                                    if (existingObject is ResourceClump clump) //if this is NOT a custom class that always needs removal
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
                            }

                            if (existingObject is LargeResourceClump largeClump) //if this is a custom class that always needs removal
                            {
                                location.largeTerrainFeatures.Remove(largeClump); //remove this object from the large terrain features list (NOTE: this must be done even for unexpired LargeResourceClumps to avoid SDV save errors)
                            }
                        }
                        else //if the object no longer exists
                        {
                            Monitor.VerboseLog($"Removing missing object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark object for removal from save
                        }
                    }
                    else if (saved.Type == SavedObject.ObjectType.Item) //if this is a forage item, i.e. "debris" containing an item
                    {
                        bool stillExists = false; //does this item still exist?

                        //if a PlacedItem terrain feature exists at the saved tile & contains an item with a matching name
                        if (location.terrainFeatures.ContainsKey(saved.Tile) && location.terrainFeatures[saved.Tile] is PlacedItem placedItem && placedItem.Item?.ParentSheetIndex == saved.ID.Value)
                        {
                            stillExists = true;
                            location.terrainFeatures.Remove(saved.Tile); //remove this placed item, regardless of expiration

                            if (endOfDay) //if expirations should be processed
                            {
                                if (saved.DaysUntilExpire == 1 || saved.DaysUntilExpire == null) //if this should expire tonight
                                {
                                    Monitor.VerboseLog($"Removing expired object. Type: {saved.Type.ToString()}. Name: {placedItem.Item?.Name}. Location: {saved.MapName}.");
                                    objectsToRemove.Add(saved); //mark this for removal from save
                                }
                                else if (saved.DaysUntilExpire > 1) //if this should expire, but not tonight
                                {
                                    saved.DaysUntilExpire--; //decrease counter by 1
                                }
                            }
                        }

                        if (!stillExists) //if this item no longer exists
                        {
                            Monitor.VerboseLog($"Removing missing object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark this for removal from save
                        }
                    }
                    else if (saved.Type == SavedObject.ObjectType.Container) //if this is a container
                    {
                        StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location
                        
                        if (realObject != null) //if an object exists in the saved location
                        {
                            bool sameContainerCategory = false;
                            switch (saved.ConfigItem?.Category.ToLower()) //compare the saved object's category to this object's class
                            {
                                case "barrel":
                                case "barrels":
                                case "breakable":
                                case "breakables":
                                case "crate":
                                case "crates":
                                    if (realObject is BreakableContainerFTM)
                                    {
                                        sameContainerCategory = true;
                                    }
                                    break;
                                case "buried":
                                case "burieditem":
                                case "burieditems":
                                case "buried item":
                                case "buried items":
                                    if (realObject is BuriedItems)
                                    {
                                        sameContainerCategory = true;
                                    }
                                    break;
                                case "chest":
                                case "chests":
                                    if (realObject is Chest)
                                    {
                                        sameContainerCategory = true;
                                    }
                                    break;
                            }
                            
                            if (sameContainerCategory) //if the real object matches the saved object's category
                            {
                                if (realObject is Chest chest) //if this is a chest
                                {
                                    while (chest.items.Count < saved.ConfigItem?.Contents.Count) //while this chest has less items than the saved object's "contents"
                                    {
                                        saved.ConfigItem.Contents.RemoveAt(0); //remove a missing item from the ConfigItem's contents (note: chests output the item at index 0 when used)
                                    }
                                }
                                
                                realObject.CanBeGrabbed = true; //workaround for certain objects being ignored by the removeObject method
                                location.removeObject(saved.Tile, false); //remove this container from the location, regardless of expiration

                                if (endOfDay) //if expirations should be processed
                                {
                                    if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                                    {
                                        Monitor.VerboseLog($"Removing expired container. Type: {saved.Type.ToString()}. Category: {saved.ConfigItem?.Category}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");

                                        objectsToRemove.Add(saved); //mark object for removal from save
                                    }
                                    else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                                    {
                                        saved.DaysUntilExpire--; //decrease counter by 1
                                    }
                                }
                            }
                            else //if the real object does NOT match the saved object's category
                            {
                                Monitor.VerboseLog($"Removing missing object. Type: {saved.Type.ToString()}. Category: {saved.ConfigItem?.Category}. Location: {saved.MapName}.");
                                objectsToRemove.Add(saved); //mark object for removal from save
                            }
                        }
                        else //if the object no longer exists
                        {
                            Monitor.VerboseLog($"Removing missing object. Type: {saved.Type.ToString()}. Category: {saved.ConfigItem?.Category}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark object for removal from save
                        }
                    }
                    else //if this is a StardewValley.Object (e.g. forage or ore)
                    {
                        StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location

                        if (realObject != null && realObject.ParentSheetIndex == saved.ID) //if an object exists in the saved location & matches the saved object's ID
                        {
                            if (endOfDay) //if expirations should be processed
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
                        }
                        else //if the object no longer exists
                        {
                            Monitor.VerboseLog($"Removing missing object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark object for removal from save
                        }
                    }

                }

                Monitor.Log($"Object check complete. Removing {objectsToRemove.Count} missing/expired objects from save data.", LogLevel.Trace);
                foreach (SavedObject saved in objectsToRemove) //for each object that should be removed from the save data
                {
                    save.SavedObjects.Remove(saved); //remove it
                }

                MonsterTracker.Clear(); //clear all monster IDs and related data, since they've all been removed
            }
        }
    }
}
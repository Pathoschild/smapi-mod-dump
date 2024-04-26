/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

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

                    //if this saved object has an expiration setting, AND the target map is a known temporary location
                    if (saved.DaysUntilExpire.HasValue
                        && (saved.MapName.StartsWith("UndergroundMine", StringComparison.OrdinalIgnoreCase) //mine level
                        || saved.MapName.StartsWith("VolcanoDungeon", StringComparison.OrdinalIgnoreCase))) //volcano level
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
                            if (location.characters[x] is Monster monster && int.TryParse(saved.ID?.ToString(), out int monsterID) && monster.id == monsterID) //if this is a monster with an ID that matches the saved ID
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
                        ResourceClump existingObject = null; //the in-game object, if it currently exists

                        string largeObjectStringID = saved.ID?.ToString();
                        foreach (ResourceClump clump in location.resourceClumps) //for each of this location's large objects
                        {
                            if (clump.Tile.X == saved.Tile.X && clump.Tile.Y == saved.Tile.Y) //if its tile location matches
                            {
                                if (clump is GiantCrop crop)
                                {
                                    if (crop.Id == largeObjectStringID) //if this is a crop and the ID matches
                                    {
                                        existingObject = clump;
                                        break;
                                    }
                                }
                                else if (Utility.ItemExtensionsAPI?.IsClump(largeObjectStringID) == true) //if IE is installed
                                {
                                    if (clump.modData.TryGetValue("mistyspring.ItemExtensions/CustomClumpId", out string itemExtensionsClumpID) && largeObjectStringID == itemExtensionsClumpID) //if this is an IE clump and the ID matches
                                    {
                                        existingObject = clump;
                                        break;
                                    }
                                }
                                else if (largeObjectStringID == (clump.parentSheetIndex.Value.ToString() ?? "")) //if this is NOT any other kind of clump, and the index matches
                                {
                                    existingObject = clump;
                                    break;
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

                                    location.resourceClumps.Remove(existingObject); //remove this object from the farm's resource clumps list

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
                    else if (saved.Type == SavedObject.ObjectType.Item) //if this is a non-standard forage item (PlacedItem, furniture, etc)
                    {
                        bool stillExists = false; //does this item still exist?

                        switch (saved.ConfigItem?.Category.ToLower())
                        {
                            case "(bc)":
                            case "bc":
                            case "bigcraftable":
                            case "bigcraftables":
                            case "big craftable":
                            case "big craftables":
                                //if a big craftable exists at the saved tile with a matching ID
                                if (location.Objects.TryGetValue(saved.Tile, out StardewValley.Object realObject) && realObject.bigCraftable.Value == true && realObject.ItemId == saved.StringID)
                                {
                                    stillExists = true;

                                    if (saved.ConfigItem?.CanBePickedUp == false) //if this object was flagged as "cannot be picked up"
                                        realObject.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag (in case of mod removal overnight, etc; it should be re-enabled by another method after save)

                                    if (endOfDay) //if expirations should be processed
                                    {
                                        if (saved.DaysUntilExpire == 1) //if the BC should expire tonight
                                        {
                                            Monitor.VerboseLog($"Removing expired object. Type: Big Craftable. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                            realObject.CanBeGrabbed = true; //allow removeObject to handle certain objects that would otherwise be ignored
                                            realObject.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag if applicable
                                            location.removeObject(saved.Tile, false); //remove the object from the game
                                            objectsToRemove.Add(saved); //mark object for removal from save
                                        }
                                        else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                                        {
                                            saved.DaysUntilExpire--; //decrease counter by 1
                                        }
                                    }
                                }
                                break;
                            case "fence":
                            case "fences":
                            case "gate":
                            case "gates":
                                //if a fence exists at the saved tile with a matching ID
                                if (location.Objects.TryGetValue(saved.Tile, out realObject) && realObject is Fence realFence && realFence.ItemId == saved.StringID)
                                {
                                    stillExists = true;

                                    if (saved.ConfigItem?.CanBePickedUp == false) //if this object was flagged as "cannot be picked up"
                                        realFence.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag (in case of mod removal overnight, etc; it should be re-enabled by another method after save)

                                    if (endOfDay) //if expirations should be processed
                                    {
                                        if (saved.DaysUntilExpire == 1) //if the fence should expire tonight
                                        {
                                            Monitor.VerboseLog($"Removing expired object. Type: Fence. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                            realFence.CanBeGrabbed = true; //allow removeObject to handle certain objects that would otherwise be ignored
                                            realFence.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag if applicable
                                            location.removeObject(saved.Tile, false); //remove the object from the game
                                            objectsToRemove.Add(saved); //mark object for removal from save
                                        }
                                        else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                                        {
                                            saved.DaysUntilExpire--; //decrease counter by 1
                                        }
                                    }
                                }
                                break;
                            case "(f)":
                            case "f":
                            case "furniture":
                                foreach (Furniture realFurniture in location.furniture)
                                {
                                    if (realFurniture.TileLocation.Equals(saved.Tile) && realFurniture.ItemId.Equals(saved.StringID, StringComparison.Ordinal)) //if furniture exists with a matching tile and ID
                                    {
                                        stillExists = true;

                                        if (endOfDay) //if expirations should be processed
                                        {
                                            if (saved.DaysUntilExpire == 1) //if this should expire tonight
                                            {
                                                Monitor.VerboseLog($"Removing expired object. Type: Furniture. Name: {realFurniture.Name}. Location: {saved.MapName}.");
                                                location.furniture.Remove(realFurniture); //remove the furniture from the game
                                                objectsToRemove.Add(saved); //mark this for removal from save
                                            }
                                            else if (saved.DaysUntilExpire > 1) //if this should expire, but not tonight
                                            {
                                                saved.DaysUntilExpire--; //decrease counter by 1
                                            }
                                        }

                                        break; //stop checking furniture after finding a match
                                    }
                                }
                                break;
                            default:
                                //if a PlacedItem exists at the saved tile & contains an item with a matching ID
                                if (location.terrainFeatures.ContainsKey(saved.Tile) && location.terrainFeatures[saved.Tile] is PlacedItem placedItem && placedItem.Item?.ItemId == saved.StringID)
                                {
                                    stillExists = true;
                                    location.terrainFeatures.Remove(saved.Tile); //remove this placed item, regardless of expiration
                                    placedItem.Item = null; //clear the reference to the contained item

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
                                break;
                        }

                        if (!stillExists) //if this item no longer exists
                        {
                            Monitor.VerboseLog($"Removing missing object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.MapName}.");
                            objectsToRemove.Add(saved); //mark this for removal from save
                        }
                    }
                    else if (saved.Type == SavedObject.ObjectType.Container) //if this is a container
                    {
                        if (location.Objects.TryGetValue(saved.Tile, out StardewValley.Object realObject)) //if an object exists in the saved location
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
                                    while (chest.Items.Count < saved.ConfigItem?.Contents.Count) //while this chest has less items than the saved object's "contents"
                                    {
                                        saved.ConfigItem.Contents.RemoveAt(0); //remove a missing item from the ConfigItem's contents (note: chests output the item at index 0 when used)
                                    }
                                }

                                realObject.CanBeGrabbed = true; //allow removeObject to handle certain objects that would otherwise be ignored
                                realObject.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag if applicable
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
                    else if (saved.Type == SavedObject.ObjectType.DGA) //if this is a DGA item
                    {
                        if //if a matching PlacedItem exists here
                        (
                            location.terrainFeatures.ContainsKey(saved.Tile) //if this tile has a features
                            && location.terrainFeatures[saved.Tile] is PlacedItem placedItem //and it's a placed item
                            && placedItem.Item != null //and it isn't empty
                            && DGAItemAPI?.GetDGAItemId(placedItem.Item) == saved.Name //and the contained item matches the saved name (according to DGA's API)
                        )
                        {
                            location.terrainFeatures.Remove(saved.Tile); //remove this placed item, regardless of expiration
                            placedItem.Item = null; //clear the reference to the contained item

                            if (endOfDay) //if expirations should be processed
                            {
                                if (saved.DaysUntilExpire == 1 || saved.DaysUntilExpire == null) //if this should expire tonight
                                {
                                    Monitor.VerboseLog($"Removing expired object. Type: DGA item. Name: {saved.Name}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                    objectsToRemove.Add(saved); //mark this for removal from save
                                }
                                else if (saved.DaysUntilExpire > 1) //if this should expire, but not tonight
                                {
                                    saved.DaysUntilExpire--; //decrease counter by 1
                                }
                            }
                        }
                        else if (location.GetFurnitureAt(saved.Tile) is Furniture realFurniture && DGAItemAPI?.GetDGAItemId(realFurniture) == saved.Name) //if matching furniture exists here
                        {
                            if (endOfDay) //if expirations should be processed
                            {
                                if (saved.DaysUntilExpire == 1) //if this should expire tonight
                                {
                                    Monitor.VerboseLog($"Removing expired object. Type: DGA furniture. Name: {saved.Name}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                    location.furniture.Remove(realFurniture); //remove this furniture
                                    objectsToRemove.Add(saved); //mark this for removal from save
                                }
                                else if (saved.DaysUntilExpire > 1) //if this should expire, but not tonight
                                {
                                    saved.DaysUntilExpire--; //decrease counter by 1
                                }
                            }
                        }
                        else //if a matching PlacedItem or furniture does not exist, check for a normal object
                        {
                            if (location.Objects.TryGetValue(saved.Tile, out StardewValley.Object realObject) && DGAItemAPI?.GetDGAItemId(realObject) == saved.Name) //if an object exists in the saved location & matches the saved object (according to DGA's API)
                            {
                                if (saved.ConfigItem?.CanBePickedUp == false) //if this object was flagged as "cannot be picked up"
                                    realObject.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag (in case of mod removal overnight, etc; it should be re-enabled by another method after save)

                                if (endOfDay) //if expirations should be processed
                                {
                                    if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                                    {
                                        Monitor.VerboseLog($"Removing expired object. Type: DGA object. Name: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                        realObject.CanBeGrabbed = true; //allow removeObject to handle certain objects that would otherwise be ignored
                                        realObject.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag if applicable
                                        location.removeObject(saved.Tile, false); //remove the object from the game
                                        objectsToRemove.Add(saved); //mark object for removal from save
                                    }
                                    else if (saved.DaysUntilExpire > 1) //if the object should expire, but not tonight
                                    {
                                        saved.DaysUntilExpire--; //decrease counter by 1
                                    }
                                }
                            }
                            else //if this tile does NOT contain this DGA item in any form
                            {
                                Monitor.VerboseLog($"Removing missing object. Type: DGA item. Name: {saved.Name}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                objectsToRemove.Add(saved); //mark object for removal from save
                            }
                        }
                    }
                    else //if this is a StardewValley.Object (e.g. forage or ore)
                    {
                        if (location.Objects.TryGetValue(saved.Tile, out StardewValley.Object realObject) && realObject.bigCraftable.Value == false && realObject.ItemId == saved.StringID) //if an object exists in the saved location & matches the saved object's ID
                        {
                            if (saved.ConfigItem?.CanBePickedUp == false) //if this object was flagged as "cannot be picked up"
                                realObject.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag (in case of mod removal overnight, etc; it should be re-enabled by another method after save)

                            if (endOfDay) //if expirations should be processed
                            {
                                if (saved.DaysUntilExpire == 1) //if the object should expire tonight
                                {
                                    Monitor.VerboseLog($"Removing expired object. Type: {saved.Type.ToString()}. ID: {saved.ID}. Location: {saved.Tile.X},{saved.Tile.Y} ({saved.MapName}).");
                                    realObject.CanBeGrabbed = true; //allow removeObject to handle certain objects that would otherwise be ignored
                                    realObject.Fragility = StardewValley.Object.fragility_Removable; //disable "indestructible" flag if applicable
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
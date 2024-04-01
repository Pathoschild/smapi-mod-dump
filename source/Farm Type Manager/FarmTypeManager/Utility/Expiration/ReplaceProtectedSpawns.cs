/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
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
            /// <summary>Check each saved object with an expiration setting and respawn it if it disappeared after saving (e.g. due to automatic cleanup or not being serialized).</summary>
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

                        //if this monster's type is no longer valid (NOTE: this may also be necessary to update the MTF monster ID cache; consider refactoring that)
                        if (ValidateMonsterTypes(new List<MonsterType>() { saved.MonType }, "[No Area ID: Respawning previously saved monster.]")?.Count <= 0)
                        {
                            uninstalled++; //increment uninstall tracker
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
                        bool stillExists = false; //does this large object still exist?

                        string largeObjectStringID = saved.ID?.ToString();
                        foreach (ResourceClump clump in location.resourceClumps) //for each of this location's large objects
                        {
                            if (clump.Tile.X == saved.Tile.X && clump.Tile.Y == saved.Tile.Y) //if its tile location matches
                            {
                                if (clump is GiantCrop crop)
                                {
                                    if (crop.Id == largeObjectStringID) //if this is a crop and the ID matches
                                    {
                                        stillExists = true;
                                        break;
                                    }
                                }
                                else if (Utility.ItemExtensionsAPI?.IsClump(largeObjectStringID) == true) //if IE is installed
                                {
                                    if (clump.modData.TryGetValue("mistyspring.ItemExtensions/CustomClumpId", out string itemExtensionsClumpID) && largeObjectStringID == itemExtensionsClumpID) //if this is an IE clump and the ID matches
                                    {
                                        stillExists = true;
                                        break;
                                    }
                                }
                                else if (largeObjectStringID == (clump.parentSheetIndex.Value.ToString() ?? "")) //if this is NOT any other kind of clump, and the index matches
                                {
                                    stillExists = true;
                                    break;
                                }
                            }
                        }

                        if (!stillExists) //if the object no longer exists
                        {
                            missing++; //increment missing tracker

                            if (IsTileValid(location, saved.Tile, saved.Size, "High")) //if the object's tile is valid for large object placement (defaulting to "high" strictness)
                            {
                                SpawnLargeObject(saved.ID?.ToString(), location, saved.Tile); //respawn the object
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
                        switch (saved.ConfigItem?.Category.ToLower()) //check category to determine how to replace this item
                        {
                            case "(bc)":
                            case "bc":
                            case "bigcraftable":
                            case "bigcraftables":
                            case "big craftable":
                            case "big craftables":
                                StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location

                                if (realObject == null) //if the object no longer exists
                                {
                                    missing++; //increment missing object tracker

                                    if (IsTileValid(location, saved.Tile, new Point(1, 1), "Medium")) //if the object's tile is clear enough to respawn
                                    {
                                        saved.ID = GetItemID(saved.ConfigItem.Category, saved.ConfigItem.Name); //try to regenerate this item's ID from its config data
                                        if (saved.ID != null) //if a valid ID was found for this object
                                        {
                                            respawned++; //increment respawn tracker
                                            SpawnForage(saved, location, saved.Tile); //respawn it
                                        }
                                        else
                                        {
                                            uninstalled++; //increment uninstalled mod tracker
                                            Monitor.LogOnce($"Couldn't find a valid ID for a previously saved big craftable. Name: {saved.Name}", LogLevel.Trace);
                                        }
                                    }
                                    else //if the object's tile is occupied
                                    {
                                        blocked++; //increment obstruction tracker
                                    }
                                }
                                else if (saved.ConfigItem?.CanBePickedUp == false) //if this object was flagged as "cannot be picked up"
                                    realObject.Fragility = StardewValley.Object.fragility_Indestructable; //re-enable "indestructible" flag (should be disabled before save)

                                break;

                            case "(f)":
                            case "f":
                            case "furniture": //if this has the furniture category
                                bool stillExists = false;
                                foreach (Furniture realFurniture in location.furniture)
                                {
                                    if (realFurniture.TileLocation.Equals(saved.Tile) && realFurniture.ItemId.Equals(saved.StringID, StringComparison.Ordinal)) //if furniture exists with a matching tile and ID
                                    {
                                        stillExists = true;
                                        break; //stop checking furniture after finding a match
                                    }
                                }

                                if (!stillExists) //if the furniture no longer exists
                                {
                                    missing++; //increment missing tracker

                                    saved.ID = GetItemID(saved.ConfigItem.Category, saved.ConfigItem.Name); //try to regenerate this item's ID from its config data
                                    //note: furniture can overlap and should be more persistent than most objects, so tile validity is not checked here
                                    if (saved.ID != null) //if a valid ID was found for this object
                                    {
                                        respawned++; //increment respawn tracker
                                        SpawnForage(saved, location, saved.Tile); //respawn it
                                    }
                                    else
                                    {
                                        uninstalled++; //increment uninstalled mod tracker
                                        Monitor.LogOnce($"Couldn't find a valid ID for a previously saved furniture item. Name: {saved.Name}", LogLevel.Trace);
                                    }
                                }
                                break;

                            default: //if this is any other kind of item (and thus a PlacedItem)
                                missing++; //increment missing tracker (PlacedItem should always be removed overnight)

                                //assume that this must have been removed overnight; respawn the item without checking for its existence
                                if (!location.terrainFeatures.ContainsKey(saved.Tile) && IsTileValid(location, saved.Tile, new Point(1, 1), "Medium")) //if the item's tile is clear enough to respawn
                                {
                                    saved.ID = GetItemID(saved.ConfigItem.Category, saved.ConfigItem.Name); //try to regenerate this item's ID from its config data
                                    if (saved.ID != null) //if a valid ID was found for this object
                                    {
                                        respawned++; //increment respawn tracker
                                        SpawnForage(saved, location, saved.Tile); //respawn it (note: furniture exists in a list, so a tile validity check isn't required)
                                    }
                                    else
                                    {
                                        uninstalled++; //increment uninstalled mod tracker
                                        Monitor.LogOnce($"Couldn't find a valid ID for a previously saved forage item. Name: {saved.Name}", LogLevel.Trace);
                                    }
                                }
                                else //if this object's tile is obstructed
                                {
                                    blocked++; //increment obstruction tracker
                                }
                                break;
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
                    else if (saved.Type == SavedObject.ObjectType.DGA) //if this is a DGA item
                    {
                        StardewValley.Object realObject = location.getObjectAtTile((int)saved.Tile.X, (int)saved.Tile.Y); //get the object at the saved location (if any)
                        Furniture realFurniture = location.GetFurnitureAt(saved.Tile); //get the furniture at the saved location (if any)
                        bool featureExists = location.terrainFeatures.TryGetValue(saved.Tile, out TerrainFeature realFeature); //try to get a terrain feature at this location

                        if (DGAItemAPI == null) //if DGA isn't available
                        {
                            uninstalled++; //increment uninstalled mod tracker
                            Monitor.LogOnce($"The interface for Dynamic Game Assets (DGA) is unavailable, so a DGA item couldn't be respawned from save data.", LogLevel.Trace);
                        }
                        else if ((realObject == null || DGAItemAPI.GetDGAItemId(realObject) != saved.Name) //if a matching DGA object is NOT here
                                && (realFurniture == null || DGAItemAPI.GetDGAItemId(realFurniture) != saved.Name) //AND a matching DGA furniture is NOT here
                                && (!featureExists || realFeature is not PlacedItem placed || placed.Item == null || DGAItemAPI.GetDGAItemId(placed.Item) != saved.Name)) //AND a matching DGA item is NOT here
                        {
                            missing++; //increment missing object tracker

                            if (IsTileValid(location, saved.Tile, new Point(1, 1), "Medium")) //if the item's tile is clear enough to respawn
                            {
                                respawned++; //increment respawn tracker
                                SpawnForage(saved, location, saved.Tile); //respawn the DGA item
                            }
                            else //if the object's tile is obstructed
                            {
                                blocked++; //increment obstruction tracker
                            }
                        }
                        else if (realObject != null && realFurniture == null && saved.ConfigItem?.CanBePickedUp == false) //if this is a non-furniture object flagged as "cannot be picked up"
                            realObject.Fragility = StardewValley.Object.fragility_Indestructable; //re-enable "indestructible" flag (should be disabled before save)
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
                                        //update this forage's ID, in case it changed due to other mods

                                        if (saved.Name.Contains(':')) //if this is "category:name"
                                        {
                                            string[] categoryAndName = saved.Name.Split(':');
                                            saved.ID = GetItemID(categoryAndName[0], categoryAndName[1]);
                                        }
                                        else //if this is just an object name
                                            saved.ID = GetItemID("object", saved.Name);
                                    }

                                    if (saved.ID != null) //if a valid ID was found for this object
                                    {
                                        respawned++; //increment respawn tracker
                                        SpawnForage(saved, location, saved.Tile); //respawn it
                                    }
                                    else
                                    {
                                        uninstalled++; //increment uninstalled mod tracker
                                        Monitor.LogOnce($"Couldn't find a valid ID for a previously saved forage object. Object name: {saved.Name}", LogLevel.Trace);
                                    }
                                }
                                else //if this is ore
                                {
                                    respawned++; //increment respawn tracker
                                    SpawnOre(saved.Name, location, saved.Tile); //respawn it
                                }
                            }
                            else //if the object's tile is occupied
                            {
                                blocked++; //increment obstruction tracker
                            }
                        }
                        else if (saved.ConfigItem?.CanBePickedUp == false) //if this object was flagged as "cannot be picked up"
                            realObject.Fragility = StardewValley.Object.fragility_Indestructable; //re-enable "indestructible" flag (should be disabled before save)
                    }
                }

                Monitor.Log($"Missing objects: {missing}. Respawned: {respawned}. Not respawned due to obstructions: {blocked}. Skipped due to missing maps: {unloaded}. Skipped due to missing item types: {uninstalled}.", LogLevel.Trace);
            }
        }
    }
}
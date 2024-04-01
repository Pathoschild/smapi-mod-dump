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
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates an object from an index and places it on the specified map and tile.</summary>
            /// <param name="index">The parent sheet index (a.k.a. object ID) of the object type to spawn.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="indestructible">True if this forage should spawn as an "indestructible" item that can't be picked up (regardless of its normal behavior).</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnForage(string index, GameLocation location, Vector2 tile, bool indestructible = false)
            {
                StardewValley.Object forageObj = new StardewValley.Object(index, 1)
                {
                    Location = location,
                    TileLocation = tile
                };

                if (indestructible) //if this should NOT be picked up or destroyed by players
                {
                    forageObj.Fragility = StardewValley.Object.fragility_Indestructable;
                    forageObj.modData[Utility.ModDataKeys.CanBePickedUp] = "false";
                }
                else if (CanBePickedUp(index)) //if this ID is normally allowed to be picked up
                {
                    forageObj.IsSpawnedObject = true; //allow "normal" forage behavior if applicable (including allowing players to pick it up)
                }

                int? durability = GetDefaultDurability(index); //try to get this item's default durability
                if (durability.HasValue) //if a default exists
                    forageObj.MinutesUntilReady = durability.Value; //use it

                Monitor.VerboseLog($"Spawning forage object. Name: {forageObj.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                return location.objects.TryAdd(tile, forageObj); //attempt to add the object and return success/failure
            }

            /// <summary>Generates an item from a saved object and places it on the specified map and tile.</summary>
            /// <param name="forage">The SavedObject containing this forage's information.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnForage(SavedObject forage, GameLocation location, Vector2 tile)
            {
                if (forage.Type == SavedObject.ObjectType.Object) //if this is a basic object
                {
                    bool indestructible;
                    if (forage.ConfigItem?.CanBePickedUp == false) //if this setting was provided AND is false
                        indestructible = true; //make this item "indestructible" to forcibly prevent pickup
                    else
                        indestructible = false; //use normal behavior

                    return SpawnForage(forage.StringID, location, tile, indestructible); //call the object ID version of this method
                }
                else if (forage.Type == SavedObject.ObjectType.Container) //if this is a container
                {
                    Item container = CreateItem(forage, tile); //create the container to be spawned

                    if (container == null || !(container is StardewValley.Object)) //if the container couldn't be created or isn't a StardewValley.Object
                    {
                        Monitor.Log("The SpawnForage method failed to generate a container. This may be caused by a problem with this mod's logic. Please report this to the developer if possible.", LogLevel.Warn);
                        Monitor.Log($"Container type: {forage.Name}", LogLevel.Warn);
                        Monitor.Log($"Item ID: {forage.ID}", LogLevel.Warn);
                        return false;
                    }

                    if (location.objects.ContainsKey(tile)) //if this tile is already occupied in the object dictionary
                    {
                        Monitor.VerboseLog("Tile is already occupied by an object. Skipping container spawn.");
                    }

                    Monitor.VerboseLog($"Spawning container. Name: {container.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    location.objects.Add(tile, (StardewValley.Object)container); //add the container to the location's object array
                    return true;
                }
                else if (forage.Type == SavedObject.ObjectType.DGA) //if this is a DGA item
                {
                    return SpawnDGAItem(forage, location, tile);
                }
                else //assume ObjectType.Item
                {
                    Item forageItem = CreateItem(forage, tile); //create the item to be spawned

                    if (forageItem == null) //if the item couldn't be created
                        return false; //do nothing (log messages should be handled by the creation process)

                    if (forageItem is StardewValley.Object bc && bc.bigCraftable.Value) //if this item is a big craftable
                    {
                        if (forage.ConfigItem?.CanBePickedUp == false)
                        {
                            //disable pickup if applicable
                            bc.Fragility = StardewValley.Object.fragility_Indestructable;
                            bc.modData[Utility.ModDataKeys.CanBePickedUp] = "false";
                        }

                        Monitor.VerboseLog($"Spawning big craftable. Name: {forageItem.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        location.objects.Add(tile, bc);
                        return true;
                    }
                    else if (forageItem is Furniture furniture)
                    {
                        if (forage.ConfigItem?.CanBePickedUp == false)
                        {
                            //disable pickup if applicable (note: fragility has no effect on furniture, so this relies on a Harmony patch and the flag below)
                            furniture.modData[Utility.ModDataKeys.CanBePickedUp] = "false";
                        }

                        Monitor.VerboseLog($"Spawning furniture. Name: {forageItem.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        location.furniture.Add(furniture);
                        return true;
                    }
                    else //handle this as a PlacedItem
                    {
                        if (location.terrainFeatures.ContainsKey(tile)) //if a terrain feature already exists on this tile
                            return false; //fail to spawn

                        PlacedItem placed = new PlacedItem(forageItem); //create a terrainfeature containing the item

                        if (forage.ConfigItem?.CanBePickedUp == false)
                        {
                            //disable pickup if applicable (note: apply this to the PlacedItem, not the actual item it contains)
                            placed.modData[Utility.ModDataKeys.CanBePickedUp] = "false";
                        }

                        Monitor.VerboseLog($"Spawning forage item. Name: {forageItem.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        location.terrainFeatures.Add(tile, placed); //add the placed item to this location
                        return true;
                    }
                }
            }

            /// <summary>Attempts to generate and place an item using the <see cref="Utility.DGAItemAPI"/> interface.</summary>
            /// <param name="forage">The SavedObject containing this forage's information.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            /// <returns>True if the item spawned successfully; false otherwise.</returns>
            private static bool SpawnDGAItem(SavedObject forage, GameLocation location, Vector2 tile)
            {
                try
                {
                    Item itemDGA = CreateItem(forage, tile);

                    if (itemDGA is Furniture furnitureDGA) //if the resulting item is furniture
                    {
                        if (forage.ConfigItem?.CanBePickedUp == false)
                        {
                            //disable pickup if applicable (note: fragility has no effect on furniture, so this relies on a Harmony patch and the flag below)
                            furnitureDGA.modData[Utility.ModDataKeys.CanBePickedUp] = "false";
                        }

                        Monitor.VerboseLog($"Spawning DGA forage furniture. Name: {forage.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        furnitureDGA.TileLocation = tile;
                        Rectangle originalBoundingBox = furnitureDGA.boundingBox.Value; //get "original" bounding box
                        furnitureDGA.boundingBox.Value = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, originalBoundingBox.Width, originalBoundingBox.Height); //adjust for tile position
                        furnitureDGA.updateDrawPosition();
                        location.furniture.Add(furnitureDGA); //add the furniture to this location
                        return true;
                    }
                    else if (itemDGA is StardewValley.Object objectDGA) //if the resulting item is a SDV object (i.e. can be treated like normal forage)
                    {
                        if (forage.ConfigItem?.CanBePickedUp == false)
                        {
                            //disable pickup if applicable
                            objectDGA.Fragility = StardewValley.Object.fragility_Indestructable;
                            objectDGA.modData[Utility.ModDataKeys.CanBePickedUp] = "false";
                        }

                        Monitor.VerboseLog($"Spawning DGA forage object. Name: {forage.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        objectDGA.IsSpawnedObject = true;
                        return location.Objects.TryAdd(tile, objectDGA); //attempt to place the object and return success/failure
                    }
                    else if (itemDGA != null) //if the resulting item is any other type of Item (i.e. can be treated as a PlacedItem)
                    {
                        if (location.terrainFeatures.ContainsKey(tile)) //if a terrain feature already exists on this tile
                            return false; //fail to spawn

                        PlacedItem placed = new PlacedItem(itemDGA); //create a terrainfeature containing the item

                        if (forage.ConfigItem?.CanBePickedUp == false)
                        {
                            //disable pickup if applicable (note: apply this to the PlacedItem, not the actual item it contains)
                            placed.modData[Utility.ModDataKeys.CanBePickedUp] = "false";
                        }

                        Monitor.VerboseLog($"Spawning DGA forage item. Name: {forage.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        location.terrainFeatures.Add(tile, placed); //add the placed item to this location
                        return true;
                    }

                    //if the item was null or not an Item class
                    return false;
                }
                catch (Exception ex)
                {
                    Monitor.Log($"An error occurred while spawning a Dynamic Game Assets (DGA) item.", LogLevel.Warn);
                    Monitor.Log($"Item name: \"{forage.Name}\"", LogLevel.Warn);
                    Monitor.Log($"The affected item will be skipped. The auto-generated error message has been added to the log.", LogLevel.Warn);
                    Monitor.Log($"----------", LogLevel.Trace);
                    Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
                    return false;
                }
            }
        }
    }
}
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
            /// <summary>Generates a object from an index and places it on the specified map and tile.</summary>
            /// <param name="index">The parent sheet index (a.k.a. object ID) of the object type to spawn.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnForage(int index, GameLocation location, Vector2 tile)
            {
                StardewValley.Object forageObj;

                if (CanBePickedUp(index)) //if this object can be picked up
                {
                    forageObj = new StardewValley.Object(tile, index, null, false, true, false, true); //generate the object (use the constructor that allows pickup)

                    Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    return location.dropObject(forageObj, tile * 64f, Game1.viewport, true, null); //attempt to place the object and return success/failure
                }
                else //if this object CANNOT be picked up
                {
                    forageObj = new StardewValley.Object(tile, index, 1); //generate the object (use the constructor that prevents pickup)
                    int? durability = GetDefaultDurability(index); //try to get this item's default durability
                    if (durability.HasValue) //if a default exists
                        forageObj.MinutesUntilReady = durability.Value; //use it

                    Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    location.objects.Add(tile, forageObj); //add the object directly to the objects list
                    return true;
                }
            }

            /// <summary>Generates a item from a saved object and places it on the specified map and tile.</summary>
            /// <param name="forage">The SavedObject containing this forage's information.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnForage(SavedObject forage, GameLocation location, Vector2 tile)
            {
                if (forage.Type == SavedObject.ObjectType.Object) //if this is a basic object
                {
                    return SpawnForage(forage.ID.Value, location, tile); //call the object ID version of this method
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

                    Monitor.VerboseLog($"Spawning container. Type: {container.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    location.objects.Add(tile, (StardewValley.Object)container); //add the container to the location's object array
                    return true;
                }
                else if (forage.Type == SavedObject.ObjectType.DGA) //if this is a DGA item
                {
                    return SpawnDGAItem(forage, location, tile);
                }
                else //if this is an item
                {
                    if (location.terrainFeatures.ContainsKey(tile)) //if a terrain feature already exists on this tile
                        return false; //fail to spawn

                    Item forageItem = CreateItem(forage, tile); //create the item to be spawned

                    if (forageItem == null) //if the item couldn't be created
                    {
                        Monitor.Log("The SpawnForage method failed to generate an item. This may be caused by a problem with this mod's logic. Please report this to the developer if possible.", LogLevel.Warn);
                        Monitor.Log($"Item name: {forage.Name}", LogLevel.Warn);
                        Monitor.Log($"Item ID: {forage.ID}", LogLevel.Warn);
                        return false;
                    }

                    Monitor.VerboseLog($"Spawning forage item. Type: {forageItem.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    PlacedItem placed = new PlacedItem(tile, forageItem); //create a terrainfeature containing the item
                    location.terrainFeatures.Add(tile, placed); //add the placed item to this location
                    return true;
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
                    object rawDGA = DGAItemAPI.SpawnDGAItem(forage.Name); //try to create this item with DGA's API
                    if (rawDGA is Furniture furnitureDGA) //if the resulting item is furniture
                    {
                        Monitor.VerboseLog($"Spawning DGA forage furniture. Name: {forage.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        furnitureDGA.TileLocation = tile;
                        Rectangle originalBoundingBox = furnitureDGA.boundingBox.Value; //get "original" bounding box
                        furnitureDGA.boundingBox.Value = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, originalBoundingBox.Width, originalBoundingBox.Height); //adjust for tile position
                        furnitureDGA.updateDrawPosition();
                        location.furniture.Add(furnitureDGA); //add the furniture to this location
                        return true;
                    }
                    else if (rawDGA is StardewValley.Object objectDGA) //if the resulting item is a SDV object (i.e. can be treated like normal forage)
                    {
                        Monitor.VerboseLog($"Spawning DGA forage object. Name: {forage.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        objectDGA.IsSpawnedObject = true;
                        return location.dropObject(objectDGA, tile * 64f, Game1.viewport, true, null); //attempt to place the object and return success/failure
                    }
                    else if (rawDGA is Item itemDGA) //if the resulting item is any other type of Item (i.e. can be treated as a PlacedItem)
                    {
                        if (location.terrainFeatures.ContainsKey(tile)) //if a terrain feature already exists on this tile
                            return false; //fail to spawn

                        Monitor.VerboseLog($"Spawning DGA forage item. Name: {forage.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        PlacedItem placed = new PlacedItem(tile, itemDGA); //create a terrainfeature containing the item
                        location.terrainFeatures.Add(tile, placed); //add the placed item to this location
                        return true;
                    }
                    else if (rawDGA != null) //if DGA spawned an item, but it isn't a recognized type
                    {
                        Monitor.Log("Dynamic Game Assets (DGA) created an item, but FTM doesn't recognize its type. This may be caused by the item or a problem with FTM's logic.", LogLevel.Warn);
                        Monitor.Log($"Item name: {forage.Name}", LogLevel.Warn);
                        Monitor.Log($"Item type (C# code): {rawDGA.GetType()?.Name ?? "null"}", LogLevel.Warn);
                        return false;
                    }
                    else //if DGA did not spawn an item
                    {
                        Monitor.Log("The SpawnForage method failed to generate a Dynamic Game Assets (DGA) item. This may be caused by a problem with this mod's logic. Please report this to FTM's developer if possible.", LogLevel.Warn);
                        Monitor.Log($"Item name: {forage.Name}", LogLevel.Warn);
                        return false;
                    }
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
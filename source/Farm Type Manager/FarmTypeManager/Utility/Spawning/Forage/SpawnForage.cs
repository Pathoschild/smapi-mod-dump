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
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

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

                switch (index) //if this object ID cannot normally be picked up
                {
                    case 0: //weeds
                    case 2: //stone
                    case 4: //
                    case 75: //stone
                    case 76: //
                    case 77: //
                    case 294: //twig
                    case 295: //
                    case 313: //weeds
                    case 314: //
                    case 315: //
                    case 316: //
                    case 317: //
                    case 318: //
                    case 319: //ice crystals (called "weeds" in the object data)
                    case 320: //
                    case 321: //
                    case 343: //stone
                    case 450: //
                    case 452: //weeds
                    case 668: //stone
                    case 670: //
                    case 674: //weeds
                    case 675: //
                    case 751: //stone
                    case 760: //
                    case 762: //
                    case 764: //
                    case 765: //
                    case 784: //weeds
                    case 792: //weeds (forest farm, spring version)
                    case 793: //forest farm weed (forest farm, summer version)
                    case 794: //forest farm weed (forest farm, fall version)
                    case 590: //artifact dig spot

                        forageObj = new StardewValley.Object(tile, index, 1); //use an alternative constructor
                        Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        location.objects.Add(tile, forageObj); //add the object directly to the objects list
                        return true;
                }

                //no special case for this object ID was found, so use the typical spawn method
                forageObj = new StardewValley.Object(tile, index, null, false, true, false, true); //generate the forage object
                Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                return location.dropObject(forageObj, tile * 64f, Game1.viewport, true, null); //attempt to place the object and return success/failure
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

                    Monitor.VerboseLog($"Spawning forage item. Type: {forageItem.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    PlacedItem placed = new PlacedItem(tile, forageItem); //create a terrainfeature containing the item
                    location.terrainFeatures.Add(tile, placed); //add the placed item to this location
                    return true;
                }
            }
        }
    }
}
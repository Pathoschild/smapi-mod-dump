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
                StardewValley.Object forageObj = new StardewValley.Object(tile, index, null, false, true, false, true); //generate the forage object
                Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                location.dropObject(forageObj, tile * 64f, Game1.viewport, true, null); //place the forage at the location
                return true;
            }

            /// <summary>Generates a item from a saved object and places it on the specified map and tile.</summary>
            /// <param name="name">A raw string value describing the item to spawn, e.g. "pizza" or "hat:sombrero".</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnForage(SavedObject forage, GameLocation location, Vector2 tile)
            {
                if (forage.Type == SavedObject.ObjectType.Object)
                {
                    return SpawnForage(forage.ID.Value, location, tile); //call the object ID version of this method
                }

                if (forage.Type == SavedObject.ObjectType.Item)
                {
                    int? spawnChance = forage.ConfigItem?.PercentChanceToSpawn; //get this item's spawn chance, if provided
                    if (spawnChance.HasValue && spawnChance.Value < RNG.Next(100)) //if this item "fails" its chance to spawn
                    {
                        Monitor.VerboseLog($"Skipping forage item due to spawn chance. Type: {forage.Name}. Location: {tile.X},{tile.Y} ({location.Name}).");
                        return false;
                    }

                    Item forageItem = CreateItem(forage); //create the item to be spawned

                    if (forageItem == null) //if no item could be created
                    {
                        Monitor.Log("The SpawnForage method failed to generate an item. This may be caused by a problem with this mod's logic. Please report this to the developer if possible.", LogLevel.Warn);
                        Monitor.Log($"Item name: {forage.Name}", LogLevel.Warn);
                        Monitor.Log($"Item ID: {forage.ID}", LogLevel.Warn);
                        return false;
                    }

                    Monitor.VerboseLog($"Spawning forage item. Type: {forageItem.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");

                    Vector2 pixel = new Vector2((int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize); //get the "pixel" location of the item, rather than the "tile" location
                    Debris itemDebris = new Debris(-2, 1, pixel, pixel, 0.1f) //create "debris" to contain the forage item
                    {
                        item = forageItem
                    };
                    itemDebris.Chunks[0].bounces = 3; //prevent the debris bouncing when spawned by increasing its "number of bounces so far" counter
                    location.debris.Add(itemDebris); //place the debris at the the location
                    return true;
                }

                return false; //TODO: error message for unsupported forage types (should be unreachable, however)
            }
        }
    }
}
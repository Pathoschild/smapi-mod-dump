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
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Console command. Removes any of several item types. Intended for removal of spawned items, especially those with <see cref="ConfigItem.CanBePickedUp"/> set to false.</summary>
        ///<remarks>
        /// Format: "remove_items [permanent | tile]"
        /// Examples: "remove_items", "remove_items permanent", "remove_items 10 20"
        /// </remarks>
        private void RemoveItems(string command, string[] args)
        {
            if (!Context.IsWorldReady) { return; } //if the player isn't in a fully loaded game yet, ignore this command

            GameLocation location = Game1.player.currentLocation;
            if (location == null) { return; }

            Vector2? tile = null;
            bool permanent = false;

            //parse args
            if (args.Length > 0)
            {
                if (args[0].StartsWith("perm", System.StringComparison.OrdinalIgnoreCase)) //if this is the "permanent" arg
                    permanent = true;
                else if (int.TryParse(args[0], out int x)) //if this the "X" arg
                {
                    if (args.Length > 1)
                    {
                        if (int.TryParse(args[1], out int y)) //if this the "Y" arg
                        {
                            tile = new Vector2(x, y);
                        }
                        else
                        {
                            Utility.Monitor.Log($"Failed to recognize argument: \"{args[1]}\". Type \"help {command}\" for formatting instructions.", LogLevel.Info);
                            return;
                        }
                    }
                    else
                    {
                        Utility.Monitor.Log($"Failed to recognize argument: found \"X\" but not \"Y\". Type \"help {command}\" for formatting instructions.", LogLevel.Info);
                        return;
                    }
                }
                else
                {
                    Utility.Monitor.Log($"Failed to recognize argument: \"{args[0]}\". Type \"help {command}\" for formatting instructions.", LogLevel.Info);
                    return;
                }
            }

            if (permanent) //if all "permanent" FTM-spawned items should be removed from this location
            {
                Utility.Monitor.Log($"Removing permanent items spawned by FTM from current location ({location.Name})...", LogLevel.Info);
                RemovePermanentItemsFromLocation(location);
            }
            else
            {
                if (tile == null) //if a tile was not specified by args
                {
                    //get the tile that the player is currently facing
                    tile = Game1.player.Tile;
                    switch (Game1.player.FacingDirection)
                    {
                        default: //unknown values
                        case 0: //up
                            tile = new Vector2(tile.Value.X, tile.Value.Y - 1); //the tile above the player
                            break;
                        case 1: //right
                            tile = new Vector2(tile.Value.X + 1, tile.Value.Y); //the tile right of the player
                            break;
                        case 2: //down
                            tile = new Vector2(tile.Value.X, tile.Value.Y + 1); //the tile below the player
                            break;
                        case 3: //left
                            tile = new Vector2(tile.Value.X - 1, tile.Value.Y); //the tile left of the player
                            break;
                    }
                }

                Utility.Monitor.Log($"Removing an item from tile {tile.Value.X}, {tile.Value.Y} at current location ({location.Name})...", LogLevel.Info);
                RemoveItemsFromTile(location, tile.Value);
            }

            Utility.Monitor.Log($"Removal complete. Any removed items are listed above.", LogLevel.Info);
        }

        /// <summary>Removes one item of any supported type from a specific tile.</summary>
        /// <param name="location">The location of the tile.</param>
        /// <param name="tile">The tile from which to remove items.</param>
        private void RemoveItemsFromTile(GameLocation location, Vector2 tile)
        {
            //check for objects (including big craftables)
            if (location.Objects.TryGetValue(tile, out StardewValley.Object obj))
            {
                Utility.Monitor.Log($"- Object removed. Location: {location.Name}. Tile: {tile.X}, {tile.Y}. Name: {obj.DisplayName ?? obj.Name}", LogLevel.Debug);
                location.Objects.Remove(tile);
                return;
            }

            //check for furniture
            if (location.GetFurnitureAt(tile) is Furniture furniture)
            {
                Utility.Monitor.Log($"- Furniture removed. Location: {location.Name}. Tile: {furniture.TileLocation.X}, {furniture.TileLocation.Y}. Name: {furniture.DisplayName ?? furniture.Name}.", LogLevel.Debug);
                location.furniture.Remove(furniture);
                return;
            }

            //check for resource clumps (including giant crops)
            ResourceClump clumpToRemove = null;
            Rectangle boundingBox = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64); //get the pixel boundaries of this tile
            foreach (ResourceClump clump in location.resourceClumps)
            {
                if (clump.getBoundingBox().Intersects(boundingBox)) //if this resource clump intersects with the tile
                {
                    clumpToRemove = clump;
                    break; //skip the rest of the clumps
                }
            }
            if (clumpToRemove != null)
            {
                Utility.Monitor.Log($"- Resource clump removed. Location: {location.Name}. Tile: {tile.X}, {tile.Y}. Type: {clumpToRemove.GetType()?.ToString()}", LogLevel.Debug);
                location.resourceClumps.Remove(clumpToRemove);
                return;
            }

            //check for terrain features
            if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature feature))
            {
                Utility.Monitor.Log($"- Terrain feature removed. Location: {location.Name}. Tile: {tile.X}, {tile.Y}. Type: {feature.GetType()?.ToString()}", LogLevel.Debug);
                location.terrainFeatures.Remove(tile);
                return;
            }
        }

        /// <summary>Removes all items spawned by FTM with the <see cref="Utility.ModDataKeys.CanBePickedUp"/> set to false.</summary>
        /// <param name="location">The location from which to remove items.</param>
        private void RemovePermanentItemsFromLocation(GameLocation location)
        {
            List<Vector2> objectsToRemove = new List<Vector2>();

            //find and remove objects
            foreach (var entry in location.Objects.Pairs)
            {
                if (entry.Value.modData.TryGetValue(Utility.ModDataKeys.CanBePickedUp, out var data) && data.StartsWith("f", StringComparison.OrdinalIgnoreCase)) //if this is flagged as "cannot be picked up"
                {
                    entry.Value.Fragility = StardewValley.Object.fragility_Removable; //make it destructible (in case removal fails in some way)
                    objectsToRemove.Add(entry.Key); //remove it after this loop
                }
            }

            foreach (Vector2 key in objectsToRemove)
            {
                Utility.Monitor.Log($"- Object removed. Location: {location.Name}. Tile: {key.X}, {key.Y}.", LogLevel.Debug);
                location.Objects.Remove(key);
            }

            List<Furniture> furnitureToRemove = new List<Furniture>();

            //find and remove furniture
            foreach (var furniture in location.furniture)
            {
                if (furniture.modData.TryGetValue(Utility.ModDataKeys.CanBePickedUp, out var data) && data.StartsWith("f", StringComparison.OrdinalIgnoreCase)) //if this is flagged as "cannot be picked up"
                {
                    furnitureToRemove.Add(furniture);
                }
            }

            foreach (Furniture furniture in furnitureToRemove)
            {
                Utility.Monitor.Log($"- Furniture removed. Location: {location.Name}. Tile: {furniture.TileLocation.X}, {furniture.TileLocation.Y}.", LogLevel.Debug);
                location.furniture.Remove(furniture);
            }

            List<Vector2> featuresToRemove = new List<Vector2>();

            //find and remove PlacedItem terrain features
            foreach (var entry in location.terrainFeatures.Pairs)
            {
                if (entry.Value is PlacedItem placed && placed.modData.TryGetValue(Utility.ModDataKeys.CanBePickedUp, out var data) && data.StartsWith("f", StringComparison.OrdinalIgnoreCase)) //if this is a PlacedItem flagged as "cannot be picked up"
                {
                    featuresToRemove.Add(entry.Key);
                }
            }

            foreach (Vector2 key in featuresToRemove)
            {
                Utility.Monitor.Log($"- Item removed. Location: {location.Name}. Tile: {key.X}, {key.Y}.", LogLevel.Debug);
                location.terrainFeatures.Remove(key);
            }

            return;
        }
    }
}
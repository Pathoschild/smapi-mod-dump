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
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Determines whether a specific tile on a map is valid for object placement, using any necessary checks from Stardew's native methods.</summary>
            /// <param name="location">The game location to be checked.</param>
            /// <param name="tile">The tile to be validated for object placement (for a large object, this is effectively its upper left corner).</param>
            /// <param name="size">A point representing the size of this object in tiles.</param>
            /// <returns>Whether the provided tile is valid for the given area and object size, based on the area's StrictTileChecking setting.</returns>
            public static bool IsTileValid(GameLocation location, Vector2 tile, Point size, string strictTileChecking)
            {
                bool valid = true; //whether the provided tile is valid with the given parameters

                List<Vector2> tilesToCheck = new List<Vector2>(); //a list of tiles that need to be valid (based on spawn object size)

                for (int x = 0; x < size.X; x++)
                {
                    for (int y = 0; y < size.Y; y++)
                    {
                        tilesToCheck.Add(new Vector2(tile.X + x, tile.Y + y));
                    }
                }

                if (strictTileChecking.Equals("none", StringComparison.OrdinalIgnoreCase)) //no validation at all
                {
                    valid = true;
                }
                else if (strictTileChecking.Equals("low", StringComparison.OrdinalIgnoreCase)) //low-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.isObjectAtTile((int)t.X, (int)t.Y)) //if this tile is blocked by another object
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }
                else if (strictTileChecking.Equals("medium", StringComparison.OrdinalIgnoreCase)) //medium-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.isTileOccupiedForPlacement(t)) //if this tile is occupied
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }
                else if (strictTileChecking.Equals("high", StringComparison.OrdinalIgnoreCase)) //high-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (!location.isTileLocationTotallyClearAndPlaceable(t) || !IsTileClearOfDebrisItems(location, t)) //if the tile is *not* totally clear OR contains debris items
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }
                else //max-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        string noSpawn = location.doesTileHaveProperty((int)t.X, (int)t.Y, "NoSpawn", "Back"); //get the "NoSpawn" property for this tile

                        if ((noSpawn != null && noSpawn != "") || !location.isTileLocationTotallyClearAndPlaceable(t) || !IsTileClearOfDebrisItems(location, t)) //if noSpawn is *not* empty OR if the tile is *not* totally clear OR contains debris items
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }

                return valid;
            }

            private static bool IsTileClearOfDebrisItems(GameLocation location, Vector2 tile)
            {
                foreach (Debris debris in location.debris) //for each debris at this location
                {
                    if (debris.item != null && debris.Chunks.Count > 0) //if this debris contains an item
                    {
                        Vector2 debrisTile = new Vector2((int)(debris.Chunks[0].position.X / Game1.tileSize) + 1, (int)(debris.Chunks[0].position.Y / Game1.tileSize) + 1);

                        if (debrisTile == tile) //if this debris's position matches the provided tile
                        {
                            return false; //the tile is NOT clear
                        }
                    }
                }

                return true; //the tile is clear
            }
        }
    }
}
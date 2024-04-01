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
            /// <summary>Determines whether a specific tile on a map is valid for object placement.</summary>
            /// <param name="location">The game location to check.</param>
            /// <param name="tile">The tile to validate for object placement. If the object is larger than 1 tile, this is the top left corner.</param>
            /// <param name="size">A point representing the object's size in tiles (horizontal, vertical).</param>
            /// <returns>True if the tile is currently valid for object placement.</returns>
            public static bool IsTileValid(GameLocation location, Vector2 tile, Point size, string strictTileChecking)
            {
                if (strictTileChecking.Equals("none", StringComparison.OrdinalIgnoreCase)) //no validation required
                    return true;

                List<Vector2> tilesToCheck = new List<Vector2>(); //a list of tiles that need to be valid (based on spawn object size)

                for (int x = 0; x < size.X; x++)
                {
                    for (int y = 0; y < size.Y; y++)
                    {
                        tilesToCheck.Add(new Vector2(tile.X + x, tile.Y + y));
                    }
                }

                if (strictTileChecking.Equals("low", StringComparison.OrdinalIgnoreCase)) //low-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.isObjectAtTile((int)t.X, (int)t.Y)) //if this tile is blocked by an object
                        {
                            return false;
                        }
                    }
                }
                else if (strictTileChecking.Equals("medium", StringComparison.OrdinalIgnoreCase)) //medium-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.IsTileOccupiedBy(t)) //if this tile is occupied
                        {
                            return false;
                        }
                    }
                }
                else if (strictTileChecking.Equals("high", StringComparison.OrdinalIgnoreCase)) //high-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.IsTileOccupiedBy(t) || !location.CanItemBePlacedHere(t)) //if the tile is occupied OR not clear for placement
                        {
                            return false;
                        }
                    }
                }
                else //max-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.IsNoSpawnTile(t) || location.IsTileOccupiedBy(t) || !location.CanItemBePlacedHere(t)) //if this tile has "NoSpawn", is *not* totally clear
                        {
                            return false;
                        }
                    }
                }

                return true; //all relevant tests passed, so the tile is valid
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

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
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>Whether the provided tile is valid for the given area and object size, based on the area's StrictTileChecking setting.</returns>
            public static bool IsTileValid(GameLocation location, Vector2 tile, bool isLarge, string strictTileChecking)
            {
                bool valid = false;

                if (strictTileChecking.Equals("off", StringComparison.OrdinalIgnoreCase) || strictTileChecking.Equals("none", StringComparison.OrdinalIgnoreCase)) //no validation at all
                {
                    valid = true;
                }
                else if (strictTileChecking.Equals("low", StringComparison.OrdinalIgnoreCase)) //low-strictness validation
                {
                    if (isLarge) //2x2 tile validation
                    {
                        //if all the necessary tiles for a 2x2 object are *not* blocked by other objects
                        if (!location.isObjectAtTile((int)tile.X, (int)tile.Y) && !location.isObjectAtTile((int)tile.X + 1, (int)tile.Y) && !location.isObjectAtTile((int)tile.X, (int)tile.Y + 1) && !location.isObjectAtTile((int)tile.X + 1, (int)tile.Y + 1))
                        {
                            valid = true;
                        }
                    }
                    else //single tile validation
                    {
                        if (!location.isObjectAtTile((int)tile.X, (int)tile.Y)) //if the tile is *not* blocked by another object
                        {
                            valid = true;
                        }
                    }
                }
                else if (strictTileChecking.Equals("medium", StringComparison.OrdinalIgnoreCase)) //medium-strictness validation
                {
                    if (isLarge) //2x2 tile validation
                    {
                        //if all the necessary tiles for a 2x2 object are *not* occupied
                        if (!location.isTileOccupiedForPlacement(tile) && !location.isTileOccupiedForPlacement(new Vector2(tile.X + 1, tile.Y)) && !location.isTileOccupiedForPlacement(new Vector2(tile.X, tile.Y + 1)) && !location.isTileOccupiedForPlacement(new Vector2(tile.X + 1, tile.Y + 1)))
                        {
                            valid = true;
                        }
                    }
                    else //single tile validation
                    {
                        if (!location.isTileOccupiedForPlacement(tile)) //if the tile is *not* occupied
                        {
                            valid = true;
                        }
                    }
                }
                else //default to "high"-strictness validation
                {
                    if (isLarge) //2x2 tile validation
                    {
                        //if all the necessary tiles for a 2x2 object are *not* occupied
                        if (location.isTileLocationTotallyClearAndPlaceable(tile) && location.isTileLocationTotallyClearAndPlaceable(new Vector2(tile.X + 1, tile.Y)) && location.isTileLocationTotallyClearAndPlaceable(new Vector2(tile.X, tile.Y + 1)) && location.isTileLocationTotallyClearAndPlaceable(new Vector2(tile.X + 1, tile.Y + 1)))
                        {
                            valid = true;
                        }
                    }
                    else //single tile validation
                    {
                        if (location.isTileLocationTotallyClearAndPlaceable(tile)) //if the tile is *not* occupied
                        {
                            valid = true;
                        }
                    }
                }

                return valid;
            }
        }
    }
}
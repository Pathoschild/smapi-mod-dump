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
                bool valid = true; //whether the provided tile is valid with the given parameters

                List<Vector2> tilesToCheck; //a list of tiles that need to be valid (based on spawn object size)

                if (isLarge) //if the object to be spawned is 2x2 tiles in size
                {
                    //list a 2x2 set of tiles with "tile" as the top left corner
                    tilesToCheck = new List<Vector2> {
                        tile,
                        new Vector2((int)tile.X + 1, (int)tile.Y),
                        new Vector2((int)tile.X, (int)tile.Y + 1),
                        new Vector2((int)tile.X + 1, (int)tile.Y + 1)
                    };
                }
                else //if the object is 1x1
                {
                    tilesToCheck = new List<Vector2> { tile }; //list only "tile"
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
                        if (!location.isTileLocationTotallyClearAndPlaceable(t)) //if the tile is *not* totally clear
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

                        if ((noSpawn != null && noSpawn != "") || !location.isTileLocationTotallyClearAndPlaceable(t)) //if noSpawn is *not* empty OR if the tile is *not* totally clear
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }

                return valid;
            }
        }
    }
}
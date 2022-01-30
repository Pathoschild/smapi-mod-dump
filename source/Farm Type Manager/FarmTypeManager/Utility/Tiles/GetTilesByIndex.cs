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
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Produces a list of x/y coordinates for object spawning at a location (based on tile index, e.g. tiles using a specific dirt texture).</summary>
            /// <param name="location">The game location to be checked.</param>
            /// <param name="tileIndices">A list of integers representing spritesheet tile indices. Tiles with any matching index will be checked for object spawning.</param>
            /// <returns>A list of Vector2, each representing a tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByIndex(GameLocation location, int[] tileIndices)
            {
                List<Vector2> tiles = new List<Vector2>(); //will contain x,y coordinates for new object placement

                //get the total size of the current map
                int mapX = location.Map.DisplayWidth / Game1.tileSize;
                int mapY = location.Map.DisplayHeight / Game1.tileSize;

                //the following loops should populate a list of tiles for spawning
                int currentTileIndex;
                for (int y = 0; y < mapY; y++) //for each Y value of the curent map
                {
                    for (int x = 0; x < mapX; x++) //for each X value of the current map
                    {
                        currentTileIndex = location.getTileIndexAt(x, y, "Back"); //get the tile index of the current tile
                        foreach (int index in tileIndices)
                        {
                            if (currentTileIndex == index) //if the current tile matches one of the tile indices
                            {
                                tiles.Add(new Vector2(x, y)); //add this tile to the list
                                break; //skip the rest of the indices to avoid adding this tile multiple times
                            }
                        }
                    }
                }
                return tiles;
            }
        }
    }
}
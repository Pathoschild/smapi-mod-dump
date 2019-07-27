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
            /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on tile index, e.g. tiles using a specific dirt texture).</summary>
            /// <param name="area">The SpawnArea describing the current area and its settings.</param>
            /// <param name="tileIndices">A list of integers representing spritesheet tile indices. Tiles with any matching index will be checked for object spawning.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByIndex(SpawnArea area, int[] tileIndices, bool isLarge)
            {
                GameLocation loc = Game1.getLocationFromName(area.MapName); //variable for the current location being worked on
                List<Vector2> validTiles = new List<Vector2>(); //will contain x,y coordinates for tiles that are open & valid for new object placement

                //the following loops should populate a list of valid, open tiles for spawning
                int currentTileIndex;
                for (int y = 0; y < (loc.Map.DisplayHeight / Game1.tileSize); y++)
                {
                    for (int x = 0; x < (loc.Map.DisplayWidth / Game1.tileSize); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                    {
                        Vector2 tile = new Vector2(x, y);
                        currentTileIndex = loc.getTileIndexAt(x, y, "Back"); //get the tile index of the current tile
                        foreach (int index in tileIndices)
                        {
                            if (currentTileIndex == index) //if the current tile matches one of the tile indices
                            {
                                if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                                {
                                    validTiles.Add(tile); //add to list of valid spawn tiles
                                    break; //skip the rest of the indices to avoid adding this tile multiple times
                                }
                            }
                        }
                    }
                }
                return validTiles;
            }
        }
    }
}
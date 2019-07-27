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
            /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on tile properties, e.g. the "grass" type).</summary>
            /// <param name="area">The SpawnArea describing the current area and its settings.</param>
            /// <param name="type">A string representing the tile property to match, or a special term used for some additional checks.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByProperty(SpawnArea area, string type, bool isLarge)
            {
                GameLocation loc = Game1.getLocationFromName(area.MapName); //variable for the current location being worked on
                List<Vector2> validTiles = new List<Vector2>(); //will contain x,y coordinates for tiles that are open & valid for new object placement

                //the following loops should populate a list of valid, open tiles for spawning
                for (int y = 0; y < (loc.Map.DisplayHeight / Game1.tileSize); y++)
                {
                    for (int x = 0; x < (loc.Map.DisplayWidth / Game1.tileSize); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                    {
                        Vector2 tile = new Vector2(x, y);
                        if (type.Equals("all", StringComparison.OrdinalIgnoreCase)) //if the "property" to be matched is "All" (a special exception)
                        {

                            //add any clear tiles, regardless of properties
                            if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                            {
                                validTiles.Add(tile); //add to list of valid spawn tiles
                            }
                        }
                        if (type.Equals("diggable", StringComparison.OrdinalIgnoreCase)) //if the tile's "Diggable" property matches (case-insensitive)
                        {
                            if (loc.doesTileHaveProperty(x, y, "Diggable", "Back") == "T") //NOTE: the string "T" means "true" for several tile property checks
                            {
                                if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                                {
                                    validTiles.Add(tile); //add to list of valid spawn tiles
                                }
                            }
                        }
                        else //assumed to be checking for a specific value in the tile's "Type" property, e.g. "Grass" or "Dirt"
                        {
                            string currentType = loc.doesTileHaveProperty(x, y, "Type", "Back") ?? ""; //NOTE: this sets itself to a blank (not null) string to avoid null errors when comparing it

                            if (currentType.Equals(type, StringComparison.OrdinalIgnoreCase)) //if the tile's "Type" property matches (case-insensitive)
                            {
                                if (IsTileValid(area, tile, isLarge)) //if the tile is clear of any obstructions
                                {
                                    validTiles.Add(tile); //add to list of valid spawn tiles
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
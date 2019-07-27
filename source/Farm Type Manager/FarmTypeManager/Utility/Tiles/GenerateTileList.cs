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
            /// <summary>Generates a list of all valid tiles for object spawning in the provided SpawnArea.</summary>
            /// <param name="area">A SpawnArea listing an in-game map name and the valid regions/terrain within it that may be valid spawn points.</param>
            /// <param name="quarryTileIndex">The list of quarry tile indices for this spawn process.</param>
            /// <param name="customTileIndex">The list of custom tile indices for this spawn process.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A completed list of all valid tile coordinates for this spawn process in this SpawnArea.</returns>
            public static List<Vector2> GenerateTileList(SpawnArea area, InternalSaveData save, int[] quarryTileIndex, int[] customTileIndex, bool isLarge)
            {
                List<Vector2> validTiles = new List<Vector2>(); //list of all open, valid tiles for new spawns on the current map

                foreach (string type in area.AutoSpawnTerrainTypes) //loop to auto-detect valid tiles based on various types of terrain
                {
                    if (type.Equals("quarry", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "quarry" tile index list
                    {
                        validTiles.AddRange(Utility.GetTilesByIndex(area, quarryTileIndex, isLarge));
                    }
                    else if (type.Equals("custom", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "custom" tile index list
                    {
                        validTiles.AddRange(Utility.GetTilesByIndex(area, customTileIndex, isLarge));
                    }
                    else  //add any tiles with properties matching "type" (e.g. tiles with the "Diggable" property, "Grass" type, etc; if the "type" is "All", this will just add every valid tile)
                    {
                        validTiles.AddRange(Utility.GetTilesByProperty(area, type, isLarge));
                    }
                }
                foreach (string include in area.IncludeAreas) //check for valid tiles in each "include" zone for the area
                {
                    validTiles.AddRange(Utility.GetTilesByVectorString(area, include, isLarge));
                }

                if (area is LargeObjectSpawnArea objArea && objArea.FindExistingObjectLocations) //if this area is the large object type and "find existing objects" is enabled
                {
                    foreach (string include in save.ExistingObjectLocations[area.UniqueAreaID]) //check each saved "include" string for the area
                    {
                        validTiles.AddRange(Utility.GetTilesByVectorString(area, include, isLarge));
                    }
                }

                validTiles = validTiles.Distinct().ToList(); //remove any duplicate tiles from the list

                foreach (string exclude in area.ExcludeAreas) //check for valid tiles in each "exclude" zone for the area (validity isn't technically relevant here, but simpler to code, and tiles' validity cannot currently change during this process)
                {
                    List<Vector2> excludedTiles = Utility.GetTilesByVectorString(area, exclude, isLarge); //get list of valid tiles in the excluded area
                    validTiles.RemoveAll(excludedTiles.Contains); //remove any previously valid tiles that match the excluded area
                }

                return validTiles;
            }
        }
    }
}
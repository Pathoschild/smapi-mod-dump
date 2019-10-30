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
            /// <summary>Generates a large object and places it on the specified map and tile.</summary>
            /// <param name="index">The parent sheet index (a.k.a. object ID) of the object type to spawn.</param>
            /// <param name="location">The Farm where the large object should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static void SpawnLargeObject(int index, Farm location, Vector2 tile)
            {
                Monitor.VerboseLog($"Spawning large object. ID: {index}. Location: {tile.X},{tile.Y} ({location.Name}).");
                location.addResourceClumpAndRemoveUnderlyingTerrain(index, 2, 2, tile); //generate an object using the list of valid index numbers
            }
        }
    }
}
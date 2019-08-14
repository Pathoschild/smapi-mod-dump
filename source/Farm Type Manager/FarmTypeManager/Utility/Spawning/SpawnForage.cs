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
            /// <summary>Generates forage and places it on the specified map and tile.</summary>
            /// <param name="index">The parent sheet index (a.k.a. object ID) of the object type to spawn.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static void SpawnForage(int index, GameLocation location, Vector2 tile)
            {
                StardewValley.Object forageObj = new StardewValley.Object(tile, index, (string)null, false, true, false, true); //generate the forage object
                Monitor.VerboseLog($"Spawning forage. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                location.dropObject(forageObj, tile * 64f, Game1.viewport, true, (Farmer)null); //place the forage at the location
            }
        }
    }
}
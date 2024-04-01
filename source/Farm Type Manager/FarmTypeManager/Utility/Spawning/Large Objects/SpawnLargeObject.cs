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
            /// <param name="location">The GameLocation where the large object should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnLargeObject(string index, GameLocation location, Vector2 tile)
            {
                Monitor.VerboseLog($"Spawning large object. ID: {index}. Location: {tile.X},{tile.Y} ({location.Name}).");

                switch (index)
                {
                    //if this is a known, basic resource clump
                    case "600":
                    case "602":
                    case "622":
                    case "672":
                    case "752":
                    case "754":
                    case "756":
                    case "758":
                        location.resourceClumps.Add(new ResourceClump(int.Parse(index), 2, 2, tile));
                        return true;
                    //if this is one of the "green rain" clumps added in SDV 1.6
                    case "44":
                    case "46":
                        location.resourceClumps.Add(new ResourceClump(int.Parse(index), 2, 2, tile, 4, "TileSheets\\Objects_2")); //spawn with the correct health and spritesheet
                        return true;
                    //if this is any other known resource clump that uses "TileSheets/Objects_2"
                    case "148":
                        location.resourceClumps.Add(new ResourceClump(int.Parse(index), 2, 2, tile, null, "TileSheets\\Objects_2")); //spawn with the correct spritesheet
                        return true;

                        //this is NOT a known basic clump
                }

                if (Utility.ItemExtensionsAPI != null && Utility.ItemExtensionsAPI.IsClump(index)) //if this is an IE clump
                {
                    bool spawned = Utility.ItemExtensionsAPI.TrySpawnClump(index, tile, location, out string error);

                    if (!spawned && error != null) //if an error caused spawning to fail
                    {
                        Utility.Monitor.LogOnce($"Item Extensions encounted an error while trying to spawn a clump (a.k.a. large object). Clump ID: \"{index}\". Error message: \"{error}\".", LogLevel.Info);
                    }

                    return spawned; //true if IE says the clump was spawned successfully
                }

                //assume this is a giant crop ID
                location.resourceClumps.Add(new GiantCrop(index.ToString(), tile));
                return true;
            }
        }
    }
}
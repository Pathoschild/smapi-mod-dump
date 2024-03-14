/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BarkingUpTheRightTree.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="GameLocation"/> class.</summary>
    internal static class GameLocationPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="GameLocation.loadObjects()"/> method.</summary>
        /// <param name="__instance">The <see cref="GameLocation"/> instance being patched.</param>
        /// <returns><see langword="true"/>, meaning the original method will get ran.</returns>
        /// <remarks>This is used to load any custom trees that have been placed using tile data on a map.</remarks>
        internal static bool LoadObjectsPrefix(GameLocation __instance)
        {
            // loop through each tile and look for tiles with tree properties
            for (int x = 0; x < __instance.Map.Layers[0].LayerWidth; x++)
                for (int y = 0; y < __instance.Map.Layers[0].LayerHeight; y++)
                {
                    // check if the tile has the "Tree" property
                    var treeName = __instance.doesTileHaveProperty(x, y, "Tree", "Back");
                    if (treeName == null)
                        continue;

                    // ensure tree has been loaded and get required data
                    if (!ModEntry.Instance.Api.GetRawTreeByName(treeName, out var rawTree))
                    {
                        ModEntry.Instance.Monitor.Log($"No tree with the name: {treeName} could be found. (Will not be planted on map)", LogLevel.Warn);
                        continue;
                    }

                    // place tree
                    var tileLocation = new Vector2(x, y);
                    if (!__instance.terrainFeatures.ContainsKey(tileLocation) && !__instance.objects.ContainsKey(tileLocation))
                    {
                        var tree = new Tree(rawTree.Id, 5);
                        tree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillBarkHarvest"] = "0";
                        tree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillNextShakeProducts"] = JsonConvert.SerializeObject(new int[rawTree.Data.ShakingProducts.Count]);
                        if (__instance.doesTileHaveProperty(x, y, "NonChoppable", "Back") != null)
                            tree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/nonChoppable"] = string.Empty; // the value is unused as only the presence of the key is checked to see if the tree is choppable
                        __instance.terrainFeatures.Add(tileLocation, tree);
                    }
                }

            return true;
        }
    }
}

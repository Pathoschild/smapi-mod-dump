/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using stardew_access.Utils;
using StardewValley;
using System.Reflection;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace stardew_access.Patches
{
    internal class TileMapPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            // Example: using Method to directly patch the setter
            MethodInfo? setterMethod = typeof(TileArray).GetMethod("set_Item", [typeof(int), typeof(int), typeof(Tile)]);

            if (setterMethod != null)
            {
                var postfix = new HarmonyMethod(typeof(TileMapPatch), nameof(TileArraySetterPatch));
                harmony.Patch(setterMethod!, postfix: postfix);
            }
            else
            {
                Log.Error("Could not find the setter method to patch. Exiting patching sequence.");
            }
        }

        /// <summary>
        /// Postfix for xTile.Map constructor.
        /// </summary>
        /// <param name="id">The ID of the map.</param>
        private static void TileArraySetterPatch(int x, int y, Tile value, TileArray __instance)
        {
            TileUtils.UpdateTile((x, y), value, __instance);  
        }
    }
}

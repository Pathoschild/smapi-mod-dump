/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    internal class GameLocationDrawWaterPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(GameLocation), "resetLocalState");

        public static void Postfix(GameLocation __instance)
        {
            if (__instance is Mine)
                __instance.warps.Clear();
            else if (__instance is MineShaft)
                return;

            bool foundAnyWater = false;
            __instance.waterTiles = new bool[__instance.map.Layers[0].LayerWidth, __instance.map.Layers[0].LayerHeight];
            __instance.waterColor.Value = new Color(50, 100, 200) * 0.5f;
            for (int y = 0; y < __instance.map.GetLayer("Buildings").LayerHeight; y++)
            {
                for (int x = 0; x < __instance.map.GetLayer("Buildings").LayerWidth; x++)
                {
                    if (__instance.doesTileHaveProperty(x, y, "Water", "Back") is not null || __instance.doesTileHaveProperty(x, y, "Water", "BackBack") is not null)
                    {
                        foundAnyWater = true;
                        __instance.waterTiles[x, y] = true;
                    }
                }
            }

            if (!foundAnyWater)
                __instance.waterTiles = null;
        }
    }
}

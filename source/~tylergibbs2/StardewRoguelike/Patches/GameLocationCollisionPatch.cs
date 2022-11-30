/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(
        typeof(GameLocation),
        "isCollidingPosition",
        new Type[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) }
    )]
    internal class GameLocationCollisionPatch
    {
        public static void Postfix(GameLocation __instance, ref bool __result, Rectangle position, xTile.Dimensions.Rectangle viewport, Character character)
        {
            foreach (Vector2 tileLocation in __instance.terrainFeatures.Keys)
            {
                TerrainFeature tf = __instance.terrainFeatures[tileLocation];
                if (tf.isPassable())
                    continue;

                if (tf.getBoundingBox(tileLocation).Intersects(position))
                {
                    __result = true;
                    return;
                }
            }

            if (__instance is MineShaft mine && character is Farmer farmer)
            {
                bool result = ChallengeFloor.CheckForCollision(mine, position, farmer);
                if (result)
                    __result = result;
            }
        }
    }
}

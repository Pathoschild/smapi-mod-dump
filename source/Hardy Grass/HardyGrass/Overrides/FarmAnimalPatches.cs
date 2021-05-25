/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiscipleOfEris/HardyGrass
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using Harmony;

namespace HardyGrass
{
    [HarmonyPatch(typeof(FarmAnimal))]
    [HarmonyPatch("grassEndPointFunction", new Type[] {typeof(PathNode), typeof(Point), typeof(GameLocation), typeof(Character)})]
    public class FarmAnimal_grassEndPointFunction_Patch
    {
        public static bool Prefix(ref bool __result, PathNode currentPoint, Point endPoint, GameLocation location, Character c)
        {
            __result = false;

            Vector2 tileLocation = new Vector2(currentPoint.x, currentPoint.y);
            if (location.terrainFeatures.TryGetValue(tileLocation, out var t) && t is Grass grass && (int)grass.numberOfWeeds > 0 && !FarmAnimal.reservedGrass.Contains(grass))
            {
                FarmAnimal.reservedGrass.Add(grass);
                if (c is FarmAnimal)
                {
                    (c as FarmAnimal).foundGrass = grass;
                }
                __result = true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(FarmAnimal))]
    [HarmonyPatch("Eat", new Type[] { typeof(GameLocation) })]
    public class FarmAnimal_Eat_Patch
    {
        public static bool Prefix(FarmAnimal __instance, GameLocation location)
        {
            Vector2 tilePosition = new Vector2(__instance.GetBoundingBox().Center.X / 64, __instance.GetBoundingBox().Center.Y / 64);
            __instance.isEating.Value = true;
            int weedsToEat = __instance.isCoopDweller() ? 2 : 4;
            int fullnessPerWeed = (byte.MaxValue + 1) / weedsToEat;
            int weedsEaten = weedsToEat;
            if (location.terrainFeatures.ContainsKey(tilePosition) && location.terrainFeatures[tilePosition] is Grass grass && (int)grass.numberOfWeeds > 0)
            {
                if (ModEntry.config.fixAnimalsEating)
                {
                    weedsToEat = (byte.MaxValue + 1 - (byte)__instance.fullness) / fullnessPerWeed;
                    weedsEaten = Math.Min((int)grass.numberOfWeeds, weedsToEat);
                }

                if (grass.reduceBy(weedsEaten, tilePosition, location.Equals(Game1.currentLocation)))
                {
                    location.terrainFeatures.Remove(tilePosition);
                }
            }
            __instance.Sprite.loop = false;
            __instance.fullness.Value = (weedsEaten == weedsToEat) ? byte.MaxValue : (byte)(__instance.fullness + (weedsEaten * fullnessPerWeed));

            if ((byte)__instance.fullness >= byte.MaxValue && (int)__instance.moodMessage != 5 && (int)__instance.moodMessage != 6 && !Game1.isRaining)
            {
                __instance.happiness.Value = byte.MaxValue;
                __instance.friendshipTowardFarmer.Value = Math.Min(1000, (int)__instance.friendshipTowardFarmer + 8);
            }

            return false;
        }
    }
}
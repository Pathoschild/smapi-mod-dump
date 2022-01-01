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
using HarmonyLib;

namespace HardyGrass
{
    public class FarmAnimalPatches
    {
        public static void ApplyPatches(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(StardewValley.FarmAnimal.grassEndPointFunction)),
                prefix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.grassEndPointFunction_Prefix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(StardewValley.FarmAnimal.Eat)),
                prefix: new HarmonyMethod(typeof(FarmAnimalPatches), nameof(FarmAnimalPatches.Eat_Prefix)));
        }

        public static bool grassEndPointFunction_Prefix(ref bool __result, PathNode currentPoint, Point endPoint, GameLocation location, Character c)
        {
            __result = false;

            Vector2 tileLocation = new Vector2(currentPoint.x, currentPoint.y);
            if (location.terrainFeatures.TryGetValue(tileLocation, out var t) && t is Grass grass && grass.numberOfWeeds.Value > 0 && !FarmAnimal.reservedGrass.Contains(grass))
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

        public static bool Eat_Prefix(FarmAnimal __instance, GameLocation location)
        {
            Vector2 tilePosition = new Vector2(__instance.GetBoundingBox().Center.X / 64, __instance.GetBoundingBox().Center.Y / 64);
            __instance.isEating.Value = true;
            int weedsToEat = __instance.isCoopDweller() ? 2 : 4;
            int fullnessPerWeed = (byte.MaxValue + 1) / weedsToEat;
            int weedsEaten = weedsToEat;

            if (location.terrainFeatures.ContainsKey(tilePosition) && location.terrainFeatures[tilePosition] is Grass grass && grass.numberOfWeeds.Value > 0)
            {
                if (ModEntry.config.fixAnimalsEating)
                {
                    weedsToEat = (byte.MaxValue + 1 - __instance.fullness.Value) / fullnessPerWeed;
                    weedsEaten = Math.Min(grass.numberOfWeeds.Value, weedsToEat);
                }

                if (grass.reduceBy(weedsEaten, tilePosition, location.Equals(Game1.currentLocation)))
                {
                    location.terrainFeatures.Remove(tilePosition);
                }
            }
            __instance.Sprite.loop = false;
            __instance.fullness.Value = (weedsEaten == weedsToEat) ? byte.MaxValue : (byte)(__instance.fullness.Value + (weedsEaten * fullnessPerWeed));

            if (__instance.fullness.Value >= byte.MaxValue && __instance.moodMessage.Value != 5 && __instance.moodMessage.Value != 6 && !Game1.isRaining)
            {
                __instance.happiness.Value = byte.MaxValue;
                __instance.friendshipTowardFarmer.Value = Math.Min(1000, __instance.friendshipTowardFarmer.Value + 8);
            }

            return false;
        }
    }
}
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
    public static class ObjectPatches
    {
        public static void ApplyPatches(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.placementAction_Prefix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPlaceable)),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.isPlaceable_Postfix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPassable)),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.isPassable_Postfix)));
        }

        public static bool placementAction_Prefix(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            if (__instance.bigCraftable.Value || __instance is StardewValley.Objects.Furniture || (__instance.ParentSheetIndex != ModEntry.GrassStarterObjectId && __instance.ParentSheetIndex != ModEntry.QuickGrassStarterObjectId))
            {
                return true;
            }

            Vector2 placementTile = new Vector2(x / 64, y / 64);

            if (location.objects.ContainsKey(placementTile) || location.terrainFeatures.ContainsKey(placementTile))
            {
                __result = false;
                return false;
            }

            Grass grass = new Grass(1, ModEntry.config.shortGrassStarters ? 0 : 4);
            if (__instance.ParentSheetIndex == ModEntry.QuickGrassStarterObjectId)
            {
                grass.modData.Add(ModEntry.IsQuickModDataKey, ModEntry.IsQuickModDataValue);
            }
            location.terrainFeatures.Add(placementTile, grass);
            location.playSound("dirtyHit");

            __result = true;
            return false;
        }

        public static void isPlaceable_Postfix(StardewValley.Object __instance, ref bool __result)
        {
            if (!__instance.bigCraftable.Value && __instance.ParentSheetIndex == ModEntry.QuickGrassStarterObjectId)
            {
                __result = true;
                return;
            }
        }

        public static void isPassable_Postfix(StardewValley.Object __instance, ref bool __result)
        {
            if (!__instance.bigCraftable.Value && __instance.ParentSheetIndex == ModEntry.QuickGrassStarterObjectId)
            {
                __result = true;
                return;
            }
        }
    }
}
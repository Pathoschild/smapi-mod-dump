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
    [HarmonyPatch(typeof(StardewValley.Object))]
    [HarmonyPatch("placementAction", new Type[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) })]
    public class Object_placementAction_Patch
    {
        public static bool Prefix(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            if (__instance.bigCraftable || __instance is StardewValley.Objects.Furniture || (__instance.ParentSheetIndex != ModEntry.GrassStarterObjectId && __instance.ParentSheetIndex != ModEntry.QuickGrassStarterObjectId))
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
    }

    [HarmonyPatch(typeof(StardewValley.Object))]
    [HarmonyPatch("isPlaceable", new Type[] { })]
    public class Object_isPlaceable_Patch
    {
        public static void Postfix(StardewValley.Object __instance, ref bool __result)
        {
            if (!__instance.bigCraftable && __instance.ParentSheetIndex == ModEntry.QuickGrassStarterObjectId)
            {
                __result = true;
                return;
            }
        }
    }

    [HarmonyPatch(typeof(StardewValley.Object))]
    [HarmonyPatch("isPassable", new Type[] { })]
    public class Object_isPassable_Patch
    {
        public static void Postfix(StardewValley.Object __instance, ref bool __result)
        {
            if (!__instance.bigCraftable && __instance.ParentSheetIndex == ModEntry.QuickGrassStarterObjectId)
            {
                __result = true;
                return;
            }
        }
    }
}
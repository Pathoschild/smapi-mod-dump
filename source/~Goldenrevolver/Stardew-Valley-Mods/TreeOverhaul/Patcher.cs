/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Collections.Generic;

namespace TreeOverhaul
{
    internal class Patcher
    {
        private static TreeOverhaul mod;

        public static void PatchAll(TreeOverhaul treeOverhaul)
        {
            mod = treeOverhaul;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Tree), nameof(Tree.GetMaxSizeHere)),
               postfix: new HarmonyMethod(typeof(Patcher), nameof(GetMaxSizeHere_Post)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Tree), nameof(Tree.IsGrowthBlockedByNearbyTree)),
               postfix: new HarmonyMethod(typeof(Patcher), nameof(IsGrowthBlockedByNearbyTree_Post)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
               prefix: new HarmonyMethod(typeof(Patcher), nameof(PerformToolAction_Tree_Pre)));

            harmony.Patch(
               original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.performToolAction)),
               prefix: new HarmonyMethod(typeof(Patcher), nameof(PerformToolAction_FruitTree_Pre)));
        }

        public static void GetMaxSizeHere_Post(Tree __instance, ref int __result)
        {
            if (mod.Config.StopShadeSaplingGrowth && __instance.growthStage.Value == 0 && __result == 4)
            {
                __result = 0;
            }
        }

        // custom version of Tree.IsGrowthBlockedByNearbyTree that ignores stumps and respects fruit trees
        public static void IsGrowthBlockedByNearbyTree_Post(Tree __instance, ref bool __result)
        {
            GameLocation location = __instance.Location;
            Vector2 tile = __instance.Tile;
            var growthRect = new Rectangle((int)((tile.X - 1f) * 64f), (int)((tile.Y - 1f) * 64f), 192, 192);

            foreach (KeyValuePair<Vector2, TerrainFeature> other in location.terrainFeatures.Pairs)
            {
                if (other.Key != tile)
                {
                    if (other.Value is Tree otherTree && otherTree.growthStage.Value >= 5 && otherTree.getBoundingBox().Intersects(growthRect))
                    {
                        if (!otherTree.stump.Value || !mod.Config.GrowthIgnoresStumps)
                        {
                            __result = true;
                            return;
                        }
                    }
                    else if (mod.Config.GrowthRespectsFruitTrees && other.Value is FruitTree otherFruitTree && otherFruitTree.daysUntilMature.Value <= 0 && otherFruitTree.getBoundingBox().Intersects(growthRect))
                    {
                        if (!otherFruitTree.stump.Value || !mod.Config.GrowthIgnoresStumps)
                        {
                            __result = true;
                            return;
                        }
                    }
                }
            }

            __result = false;
            return;
        }

        public static bool PerformToolAction_Tree_Pre(Tree __instance, Tool t, int explosion, Vector2 tileLocation)
        {
            if (mod.Config.SaveSprouts <= 0)
            {
                return true;
            }

            if (__instance.tapped.Value)
            {
                return true;
            }
            if (__instance.health.Value <= -99f)
            {
                return true;
            }

            if (__instance.growthStage.Value is 1 or 2)
            {
                if (explosion > 0)
                {
                    return true;
                }
                if (t is Pickaxe or Hoe)
                {
                    return false;
                }
                if (t is MeleeWeapon)
                {
                    if (t.isScythe())
                    {
                        return !(mod.Config.SaveSprouts >= 2);
                    }
                    else
                    {
                        return !(mod.Config.SaveSprouts >= 3);
                    }
                }
            }

            return true;
        }

        public static bool PerformToolAction_FruitTree_Pre(FruitTree __instance, Tool t, int explosion, Vector2 tileLocation)
        {
            if (mod.Config.SaveSprouts <= 0)
            {
                return true;
            }

            if (__instance.health.Value <= -99f)
            {
                return true;
            }
            if (t is MeleeWeapon)
            {
                return true;
            }

            if (__instance.growthStage.Value is 1 or 2)
            {
                if (explosion > 0)
                {
                    return true;
                }
                if (t is Pickaxe or Hoe)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
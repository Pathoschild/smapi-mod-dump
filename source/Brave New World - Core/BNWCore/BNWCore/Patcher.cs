/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using BNWCore.Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using Object = StardewValley.Object;

namespace BNWCore.Patches
{
    internal static class Patcher
    {
        private static IModHelper helper;
        private static Harmony harmony;
        internal static void Patch(IModHelper helper)
        {
            Patcher.helper = helper;
            harmony = new(helper.ModRegistry.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.placementAction)),
                prefix: new(typeof(SObjectPatches), nameof(SObjectPatches.placementActionPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.canBePlacedInWater)),
                prefix: new(typeof(SObjectPatches), nameof(SObjectPatches.canBePlacedInWaterPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.canBePlacedHere)),
                prefix: new(typeof(SObjectPatches), nameof(SObjectPatches.canBePlacedHerePrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.isPlaceable)),
                prefix: new(typeof(SObjectPatches), nameof(SObjectPatches.isPlaceablePrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows)),
                prefix: new HarmonyMethod(typeof(BNWCoreAddCrownPatches), nameof(BNWCoreAddCrownPatches.Farm_AddCrows))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                prefix: new HarmonyMethod(typeof(BNWCoreToolsPatches), nameof(BNWCoreToolsPatches.Tree_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.performToolAction)),
                prefix: new HarmonyMethod(typeof(BNWCoreToolsPatches), nameof(BNWCoreToolsPatches.FruitTree_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Pickaxe), nameof(Pickaxe.DoFunction)),
                prefix: new HarmonyMethod(typeof(BNWCoreToolsPatches), nameof(BNWCoreToolsPatches.Pickaxe_DoFunction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ResourceClump), nameof(ResourceClump.performToolAction)),
                prefix: new HarmonyMethod(typeof(BNWCoreToolsPatches), nameof(BNWCoreToolsPatches.ResourceClump_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), "tilesAffected"),
                postfix: new HarmonyMethod(typeof(BNWCoreToolsPatches), nameof(BNWCoreToolsPatches.Tool_TilesAffected_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(ShopPatches), nameof(ShopPatches.GameLocation_performAction_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.openShopMenu)),
               prefix: new HarmonyMethod(typeof(ShopPatches), nameof(ShopPatches.GameLocation_openShopMenu_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.performToolAction)),
               prefix: new HarmonyMethod(typeof(SkillPatches), nameof(SkillPatches.GiveWateringExp))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
                prefix: new HarmonyMethod(typeof(BombPatches), nameof(BombPatches.Explode_Prefix))
            );
            harmony.Patch(
              original: AccessTools.Method(typeof(Farm), "resetLocalState"),
              postfix: new HarmonyMethod(typeof(RobinSchedulesPatches), nameof(RobinSchedulesPatches.Farm_resetLocalState_Postfix))
           );
            harmony.PatchAll();
        }
    }
}

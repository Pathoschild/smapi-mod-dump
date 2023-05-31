/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace BNWCore
{
    public static class Patcher
    {
        private static Harmony harmony;
        internal static void Patch(IModHelper helper)
        {
            harmony = new(ModEntry.ModHelper.ModRegistry.ModID);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
                prefix: new HarmonyMethod(typeof(Bomb_Patches), nameof(Bomb_Patches.Explode_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows)),
               prefix: new HarmonyMethod(typeof(Remove_Crows_Event_Patches), nameof(Remove_Crows_Event_Patches.Farm_AddCrows))
           );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                prefix: new HarmonyMethod(typeof(Tools_Patches), nameof(Tools_Patches.Tree_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.performToolAction)),
                prefix: new HarmonyMethod(typeof(Tools_Patches), nameof(Tools_Patches.FruitTree_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Pickaxe), nameof(Pickaxe.DoFunction)),
                prefix: new HarmonyMethod(typeof(Tools_Patches), nameof(Tools_Patches.Pickaxe_DoFunction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ResourceClump), nameof(ResourceClump.performToolAction)),
                prefix: new HarmonyMethod(typeof(Tools_Patches), nameof(Tools_Patches.ResourceClump_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), "tilesAffected"),
                postfix: new HarmonyMethod(typeof(Tools_Patches), nameof(Tools_Patches.Tool_TilesAffected_Postfix))
            );
            harmony.Patch(
              original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.performToolAction)),
              prefix: new HarmonyMethod(typeof(Get_Watering_XP_Patches), nameof(Get_Watering_XP_Patches.GiveWateringExp))
           );
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
              original: AccessTools.Method(typeof(Farm), "resetLocalState"),
              postfix: new HarmonyMethod(typeof(Building_System_Changes), nameof(Building_System_Changes.Farm_resetLocalState_Postfix))
           );
            harmony.PatchAll();
        }
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using HarmonyLib;

using MoreFertilizers.Framework;

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.FruitTreePatches;

[HarmonyPatch(typeof(FruitTree))]
internal static class FruitTreePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(FruitTree.dayUpdate))]
    private static void PostfixDayUpdate(FruitTree __instance)
    {
        if (!__instance.stump.Value && __instance.fruitsOnTree.Value <= 0 && __instance.growthStage.Value == FruitTree.treeStage
            && __instance.modData?.GetBool(CanPlaceHandler.EverlastingFruitTreeFertilizer) == true)
        {
            __instance.fruitsOnTree.Value = 1;
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using GrowableGiantCrops.Framework.InventoryModels;

using HarmonyLib;

using StardewValley.Menus;

namespace GrowableGiantCrops.HarmonyPatches.Niceties;

/// <summary>
/// Holds patches against Qi's shop stock.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class UtilityShopPatcher
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Utility.GetQiChallengeRewardStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PostfixQiGemShop(Dictionary<ISalable, int[]> __result)
    {
        InventoryTree tree = new(TreeIndexes.Mushroom, 1, 5);
        __result.Add(tree, new[] { 0, ShopMenu.infiniteStock, 858, 15 });
    }
}

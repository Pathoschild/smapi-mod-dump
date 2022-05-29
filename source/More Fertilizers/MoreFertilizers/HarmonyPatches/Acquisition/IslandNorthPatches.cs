/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using HarmonyLib;
using StardewValley.Locations;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Patches against IslandNorth to put the fertilizer into the shop.
/// </summary>
[HarmonyPatch(typeof(IslandNorth))]
internal static class IslandNorthPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(IslandNorth.getIslandMerchantTradeStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void PostfixShop(Dictionary<ISalable, int[]> __result)
    {
        try
        {
            if (!Utility.hasFinishedJojaRoute() && ModEntry.DeluxeFishFoodID != -1
                && Utility.getFishCaughtPercent(Game1.player) > 0.5f)
            { // 851 - magma cap.
                __result.TryAdd(new SObject(ModEntry.DeluxeFishFoodID, 1), new[] { 0, int.MaxValue, 851, 5 });
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors adding stock to Island North!\n\n{ex}", LogLevel.Error);
        }
    }
}
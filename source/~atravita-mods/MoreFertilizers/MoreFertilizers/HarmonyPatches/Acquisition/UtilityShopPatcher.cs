/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;

using StardewValley.Menus;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Holds patches against Utility for shops.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class UtilityShopPatcher
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Utility.getShopStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixGetShopStock(bool Pierres, ref List<Item> __result)
    {
        if (Pierres)
        {
            try
            {
                if (ModEntry.LuckyFertilizerID != -1
                    && !(Game1.year == 1 && Game1.currentSeason.Equals("spring", StringComparison.OrdinalIgnoreCase))
                    && Game1.player.team.AverageDailyLuck() > 0.07)
                {
                    __result.Add(new SObject(ModEntry.LuckyFertilizerID, 15, isRecipe: false, price: Game1.year == 1 ? 100 : 150));
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Failed in adding to Pierre's stock!{ex}", LogLevel.Error);
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Utility.getQiShopStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixGetCasinoShop(Dictionary<ISalable, int[]> __result)
    {
        try
        {
            if (ModEntry.LuckyFertilizerID != -1 && Game1.player.team.AverageDailyLuck() > 0.05)
            {
                __result.Add(new SObject(ModEntry.LuckyFertilizerID, 1), new[] { 500, ShopMenu.infiniteStock });
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in adding to casino's stock!{ex}", LogLevel.Error);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Utility.getJojaStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixJojaStock(Dictionary<ISalable, int[]> __result)
    {
        try
        {
            if (ModEntry.SecretJojaFertilizerID != -1 && Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja")
                && Game1.player.stats.IndividualMoneyEarned > 1_000_000 && Game1.random.NextDouble() < 0.15)
            {
                __result.Add(new SObject(ModEntry.SecretJojaFertilizerID, 1), new[] { 500, 2 });
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to add Secret Joja Fertilizer to JojaMart.\n\n{ex}", LogLevel.Error);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Utility.GetQiChallengeRewardStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention.")]
    private static void PostfixQiGemShop(Dictionary<ISalable, int[]> __result)
    {
        if (ModEntry.EverlastingFertilizerID != -1)
        {
            SObject obj = new(ModEntry.EverlastingFertilizerID, 1);
            __result.Add(obj, new[] { 0, ShopMenu.infiniteStock, 858, 1 });
        }
    }
}
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
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Patches against Locations.SeedShop.
/// </summary>
[HarmonyPatch(typeof(SeedShop))]
internal static class SeedShopPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SeedShop.shopStock))]
    private static void PostfixSeedShop(ref Dictionary<ISalable, int[]> __result)
    {
        try
        {
            if (ModEntry.LuckyFertilizerID != -1
                && !(Game1.year == 1 && Game1.currentSeason.Equals("spring", StringComparison.OrdinalIgnoreCase))
                && Game1.player.team.AverageDailyLuck() > 0.07)
            {
                __result.Add(new SObject(Vector2.Zero, ModEntry.LuckyFertilizerID, 1), new[] { Game1.year == 1 ? 100 : 150, int.MaxValue });
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in adding to seedShop!{ex}", LogLevel.Error);
        }
    }
}
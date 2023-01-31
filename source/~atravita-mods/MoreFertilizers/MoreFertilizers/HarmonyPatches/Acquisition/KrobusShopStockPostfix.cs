/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Caching;
using AtraShared.Utils;

using HarmonyLib;

using StardewModdingAPI.Utilities;

using StardewValley.Locations;
using StardewValley.Menus;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Postfix to add things to Krobus's shop.
/// </summary>
[HarmonyPatch(typeof(Sewer))]
internal static class KrobusShopStockPostfix
{
    private static readonly TickCache<bool> HasGottenPrismaticFertilizer = new(static () => FarmerHelpers.HasAnyFarmerRecievedFlag($"museumCollectedRewardO_{ModEntry.PrismaticFertilizerID}_1"));

    [HarmonyPatch(nameof(Sewer.getShadowShopStock))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(ref Dictionary<ISalable, int[]> __result)
    {
        if (ModEntry.PaddyCropFertilizerID != -1)
        {
            __result.TryAdd(new SObject(ModEntry.PaddyCropFertilizerID, 1), new[] { 40, ShopMenu.infiniteStock });
        }
        if (ModEntry.WisdomFertilizerID != -1 && Game1.currentSeason is "spring" or "fall")
        {
            __result.TryAdd(new SObject(ModEntry.WisdomFertilizerID, 1), new[] { 100, ShopMenu.infiniteStock });
        }
        if (ModEntry.MiraculousBeveragesID != -1 && Game1.year > 2 && Utility.getCookedRecipesPercent() > 0.5f)
        {
            __result.TryAdd(new SObject(ModEntry.MiraculousBeveragesID, 1), new[] { 250, ShopMenu.infiniteStock });
        }
        if (ModEntry.RadioactiveFertilizerID != -1 && Game1.year > 2 && Game1.player.hasMagicInk)
        {
            __result.TryAdd(new SObject(ModEntry.RadioactiveFertilizerID, 1), new[] { 250, ShopMenu.infiniteStock });
        }
        if (ModEntry.PrismaticFertilizerID != -1 && HasGottenPrismaticFertilizer.GetValue())
        {
            __result.TryAdd(new SObject(ModEntry.PrismaticFertilizerID, 1), new[] { 100, ShopMenu.infiniteStock });
        }
    }
}
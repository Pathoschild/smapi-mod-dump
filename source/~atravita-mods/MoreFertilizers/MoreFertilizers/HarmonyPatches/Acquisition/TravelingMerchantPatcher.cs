/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraShared.Caching;

using HarmonyLib;

using MoreFertilizers.Framework;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

#warning - check for multiplayer compat here.

/// <summary>
/// Applies patches against the traveling merchant.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class TravelingMerchantPatcher
{
    private static readonly TickCache<bool> HasPlayerUnlockedBountiful = new(() => Game1.MasterPlayer.mailReceived.Contains(AssetEditor.BOUNTIFUL_BUSH_UNLOCK));

    [UsedImplicitly]
    [HarmonyPatch("generateLocalTravelingMerchantStock")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(Dictionary<ISalable, int[]> __result, int seed)
    {
        Random random = new(seed);
        random.PreWarm();
        if (ModEntry.SecretJojaFertilizerID != -1 && Game1.player.DailyLuck > 0.5 && random.NextDouble() < 0.05)
        {
            __result.Add(new SObject(ModEntry.SecretJojaFertilizerID, 1), new[] { (int)(2500 * Game1.player.difficultyModifier), 1 });
        }

        if (ModEntry.BountifulBushID != -1 && Game1.currentSeason is "spring" or "fall" && HasPlayerUnlockedBountiful.GetValue())
        {
            __result.Add(new SObject(ModEntry.BountifulBushID, 1), new[] { 200, random.Next(1, 3) });
        }
        else if (ModEntry.WisdomFertilizerID != -1 && Game1.currentSeason is "summer" or "winter")
        {
            __result.Add(new SObject(ModEntry.WisdomFertilizerID, 1), new[] { 100, random.Next(1, 3) });
        }
    }
}

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
using StardewValley.Tools;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Postfixes Pan.getPanItems to add these fertilizers.
/// </summary>
[HarmonyPatch(typeof(Pan))]
internal static class PanGetPanItemsPostfix
{
    [HarmonyPriority(Priority.VeryLow)]
    [HarmonyPatch(nameof(Pan.getPanItems))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(GameLocation location, ref List<Item> __result)
    {
        if (location is Town && Game1.random.NextDouble() < 0.05 && ModEntry.LuckyFertilizerID != -1)
        {
            __result.Add(new SObject(ModEntry.LuckyFertilizerID, 5));
        }
        else if (location is IslandLocation && Game1.random.NextDouble() < 0.05 && ModEntry.OrganicFertilizerID != -1)
        {
            __result.Add(new SObject(ModEntry.OrganicFertilizerID, 5));
        }
        else if (location is Sewer && Game1.random.NextDouble() < 0.05 && ModEntry.DeluxeJojaFertilizerID != -1)
        {
            __result.Add(new SObject(ModEntry.DeluxeJojaFertilizerID, 5));
        }
    }
}
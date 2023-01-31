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
using StardewValley.Locations;
using StardewValley.Tools;

namespace MoreFertilizers.HarmonyPatches.Acquisition;

/// <summary>
/// Postfixes Pan.getPanItems to add these fertilizers.
/// </summary>
[HarmonyPatch(typeof(Pan))]
internal static class PanGetPanItemsPostfix
{
    [HarmonyPriority(Priority.VeryLow)] // behind other panning mods
    [HarmonyPatch(nameof(Pan.getPanItems))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(GameLocation location, ref List<Item> __result)
    {
        if (Game1.random.NextDouble() > 0.05)
        {
            return;
        }

        if (location is Town && ModEntry.LuckyFertilizerID != -1)
        {
            __result.Add(new SObject(ModEntry.LuckyFertilizerID, 5));
        }
        else if (location is Farm && ModEntry.WisdomFertilizerID != -1 && Game1.player.FarmingLevel > 4)
        {
            __result.Add(new SObject(ModEntry.WisdomFertilizerID, 5));
        }
        else if (location is IslandLocation && ModEntry.OrganicFertilizerID != -1)
        {
            __result.Add(new SObject(ModEntry.OrganicFertilizerID, 5));
        }
        else if (location is Sewer)
        {
            if (ModEntry.SecretJojaFertilizerID != -1 && Game1.random.NextDouble() < 0.1
                && Utility.hasFinishedJojaRoute())
            {
                __result.Add(new SObject(ModEntry.SecretJojaFertilizerID, 2));
            }
            if (ModEntry.DeluxeJojaFertilizerID != -1)
            {
                __result.Add(new SObject(ModEntry.DeluxeJojaFertilizerID, 5));
            }
        }
        else if (location is BugLand && ModEntry.BountifulFertilizerID != -1)
        {
            __result.Add(new SObject(ModEntry.BountifulFertilizerID, 5));
        }
    }
}
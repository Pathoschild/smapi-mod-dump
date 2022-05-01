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
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches;

/// <summary>
/// Patches against hoedirt.
/// </summary>
[HarmonyPatch(typeof(HoeDirt))]
internal static class HoeDirtPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(HoeDirt.canPlantThisSeedHere))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixCanBePlanted(int objectIndex, bool isFertilizer, ref bool __result)
    {
        if (isFertilizer && ModEntry.SpecialFertilizerIDs.Contains(objectIndex))
        {
            __result = false;
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(HoeDirt.plant))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixPlant(int index, bool isFertilizer, ref bool __result)
    {
        if (isFertilizer && ModEntry.SpecialFertilizerIDs.Contains(index))
        {
            __result = false;
            return false;
        }
        return true;
    }
}
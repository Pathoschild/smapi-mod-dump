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
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.OrganicFertilizer;

/// <summary>
/// Handles organic seeds for indoor pots.
/// </summary>
[HarmonyPatch(typeof(IndoorPot))]
internal static class IndoorPotPlacement
{
    [HarmonyPatch(nameof(IndoorPot.performObjectDropInAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(IndoorPot __instance, Item? dropInItem, bool probe)
    {
        if (probe)
        {
            return;
        }
        if (dropInItem?.modData?.GetBool(CanPlaceHandler.Organic) == true
            && __instance.hoeDirt?.Value?.fertilizer?.Value is HoeDirt.noFertilizer
            && ModEntry.OrganicFertilizerID != -1)
        {
            __instance.hoeDirt.Value.fertilizer.Value = ModEntry.OrganicFertilizerID;
        }
    }
}
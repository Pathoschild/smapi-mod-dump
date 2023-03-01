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

namespace GrowableGiantCrops.HarmonyPatches.GrassPatches;

/// <summary>
/// Adds patches against Item.
/// </summary>
[HarmonyPatch(typeof(Item))]
internal static class ItemPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Item.canStackWith))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static bool PrefixCanStackWith(Item __instance, ISalable other, ref bool __result)
    {
        if (other is null || __instance.ParentSheetIndex != SObjectPatches.GrassStarterIndex)
        {
            return true;
        }

        try
        {
            if (other is SObject otherItem)
            {
                string? myData = null;
                string? otherData = null;
                _ = __instance.modData?.TryGetValue(SObjectPatches.ModDataKey, out myData);
                _ = otherItem.modData?.TryGetValue(SObjectPatches.ModDataKey, out otherData);
                if (myData != otherData
                    || (myData is not null && ModEntry.Config.PreserveModData && !__instance.modData.ModDataMatches(otherItem.modData)))
                {
                    __result = false;
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in overwriting stacking behavior:\n\n{ex}", LogLevel.Error);
        }

        return true;
    }
}

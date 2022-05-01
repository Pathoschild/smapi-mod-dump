/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using AtraShared.Utils.Extensions;
using HarmonyLib;
using MoreFertilizers.Framework;

namespace MoreFertilizers.HarmonyPatches;

/// <summary>
/// Holds patches against SObject.
/// </summary>
[HarmonyPatch(typeof(SObject))]
internal static class SObjectPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SObject.performObjectDropInAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void PostfixDropInAction(SObject __instance, Item dropInItem, bool probe)
    {
        if (!probe)
        {
            try
            {
                if (dropInItem.modData?.GetBool(CanPlaceHandler.Organic) == true)
                {
                    __instance.heldObject.Value.modData?.SetBool(CanPlaceHandler.Organic, true);
                    __instance.heldObject.Value.Name += " (Organic)";
                    __instance.heldObject.Value.MarkContextTagsDirty();
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Failed in setting organic for {dropInItem.Name}\n\n{ex}", LogLevel.Error);
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SObject.HighlightFertilizers))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixHighlightFertilizers(Item i, ref bool __result)
    {
        try
        {
            if (i is SObject obj && !obj.bigCraftable.Value && obj.ParentSheetIndex != -1 && ModEntry.SpecialFertilizerIDs.Contains(obj.ParentSheetIndex))
            {
                __result = false;
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.LogOnce($"Mod failed while adjusting highlighting fertilizers for enricher!\n\n{ex}", LogLevel.Error);
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch("loadDisplayName")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void PostfixLoadDisplayName(SObject __instance, ref string __result)
    {
        if (__instance.modData?.GetBool(CanPlaceHandler.Organic) == true)
        {
            __result += ' ' + I18n.Organic();
        }
    }
}
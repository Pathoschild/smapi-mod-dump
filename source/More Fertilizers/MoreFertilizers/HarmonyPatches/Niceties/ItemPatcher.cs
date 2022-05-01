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

namespace MoreFertilizers.HarmonyPatches.Niceties;

/// <summary>
/// Adds a context tag for organic produce.
/// </summary>
[HarmonyPatch(typeof(Item))]
internal static class ItemPatcher
{
    [HarmonyPatch("_PopulateContextTags")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static void Postfix(Item __instance, HashSet<string> tags)
    {
        try
        {
            if (__instance.modData?.GetBool(CanPlaceHandler.Organic) == true)
            {
                // Re-add in the usual name-based key, since I've adjusted the name.
                tags.Add("item_" + __instance.SanitizeContextTag(__instance.Name[..^10]));
                tags.Add("atravita_morefertilizers_organic");
            }
            else if (__instance.modData?.GetBool(CanPlaceHandler.Joja) == true)
            {
                tags.Add("atravita_morefertilizers_joja");
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in adding context tags!\n\n{ex}", LogLevel.Error);
        }
    }
}
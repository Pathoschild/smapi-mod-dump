/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/TrashDoesNotConsumeBait
**
*************************************************/

using AtraShared.Utils.Extensions;
using HarmonyLib;
using StardewValley.Objects;

namespace TrashDoesNotConsumeBait.HarmonyPatches;

/// <summary>
/// Patches on CrabPot to restore bait if the object was trash.
/// </summary>
[HarmonyPatch(typeof(CrabPot))]
internal static class CrabPotPatches
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(nameof(CrabPot.checkForAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void Prefix(CrabPot __instance, bool justCheckingForActivity, out SObject? __state)
    {
        if (!justCheckingForActivity && ModEntry.Config.CrabPotTrashDoesNotEatBait && __instance.heldObject.Value?.IsTrashItem() == true)
        {
            __state = __instance.bait.Value;
        }
        else
        {
            __state = null;
        }
    }

    [HarmonyPatch(nameof(CrabPot.checkForAction))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void Postfix(CrabPot __instance, bool justCheckingForActivity, SObject? __state)
    {
        if (!justCheckingForActivity && __state is not null && __instance.bait.Value is null)
        {
            __instance.bait.Value = __state;
        }
    }
}
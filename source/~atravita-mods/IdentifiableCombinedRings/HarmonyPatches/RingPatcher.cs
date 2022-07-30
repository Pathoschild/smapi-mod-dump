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
using StardewValley.Objects;

namespace IdentifiableCombinedRings.HarmonyPatches;

/// <summary>
/// Class to hold patches on the Ring class.
/// </summary>
[HarmonyPatch(typeof(Ring))]
internal class RingPatcher
{
    /// <summary>
    /// Patches the DisplayName method.
    /// </summary>
    /// <param name="__instance">Combined ring to check.</param>
    /// <param name="__result">Output (the Display Name).</param>
    [HarmonyPatch(nameof(Ring.DisplayName), MethodType.Getter)]
    [HarmonyPostfix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification ="Harmony convention")]
    public static void PostfixGetDisplayName(Ring __instance, ref string __result)
    {
        if (__instance is CombinedRing combinedRing)
        {
            if (combinedRing.combinedRings.Count > 2 || combinedRing.combinedRings[0] is CombinedRing || combinedRing.combinedRings[0] is CombinedRing)
            {
                __result += I18n.Many();
                return;
            }

            __result = string.Join(" & ", combinedRing.combinedRings.Select((Ring ring) => ring.DisplayName));
        }
    }
}
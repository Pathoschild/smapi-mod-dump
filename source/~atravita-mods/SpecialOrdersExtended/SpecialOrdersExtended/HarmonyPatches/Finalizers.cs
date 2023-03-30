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

namespace SpecialOrdersExtended.HarmonyPatches;

/// <summary>
/// Holds the finalizers for this project.
/// </summary>
[HarmonyPatch(typeof(SpecialOrder))]
internal class Finalizers
{
    /// <summary>
    /// Finalizes GetSpecialOrder to return null of there's an error.
    /// </summary>
    /// <param name="key">Key of the special order.</param>
    /// <param name="__result">The parsed special order, set to null to remove.</param>
    /// <param name="__exception">The observed exception.</param>
    /// <returns>null to suppress the error.</returns>
    [HarmonyFinalizer]
    [HarmonyPatch(nameof(SpecialOrder.GetSpecialOrder))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    public static Exception? FinalizeGetSpecialOrder(string key, ref SpecialOrder? __result, Exception? __exception)
    {
        if (__exception is not null)
        {
            ModEntry.ModMonitor.Log($"Detected invalid special order {key}\n\n{__exception}", LogLevel.Error);
            __result = null;
        }
        return null;
    }
}
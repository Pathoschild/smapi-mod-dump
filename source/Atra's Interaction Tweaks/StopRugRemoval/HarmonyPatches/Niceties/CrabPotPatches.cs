/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using HarmonyLib;
using StardewValley.Objects;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Holds patches against crab pots.
/// </summary>
[HarmonyPatch(typeof(CrabPot))]
internal static class CrabPotPatches
{
    /// <summary>
    /// Surpresses drawing crabpots when an event is up.
    /// </summary>
    /// <returns>True to continue (and draw), false to surpress drawing.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CrabPot.draw))]
    private static bool PrefixDraw()
        => !ModEntry.Config.HideCrabPots || !(Game1.eventUp || Game1.isFestival()) || !ModEntry.Config.Enabled;
}
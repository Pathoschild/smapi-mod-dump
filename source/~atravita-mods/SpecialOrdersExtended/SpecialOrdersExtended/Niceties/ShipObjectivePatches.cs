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

namespace SpecialOrdersExtended.Niceties;

/// <summary>
/// Handles patches against the ShipObjective.
/// </summary>
[HarmonyPatch(typeof(ShipObjective))]
internal static class ShipObjectivePatches
{
    /// <summary>
    /// Skip null items for OnShipped.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>true to continue to the rest of the function, false otherwise.</returns>
    [HarmonyPatch(nameof(ShipObjective.OnItemShipped))]
    private static bool Prefix(Item item)
        => item is not null;
}

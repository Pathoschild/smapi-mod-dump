/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using GrowableGiantCrops.Framework;

using HarmonyLib;

namespace GrowableGiantCrops.HarmonyPatches.ToolPatches;

/// <summary>
/// Patches on farmer.
/// </summary>
[HarmonyPatch(typeof(Farmer))]
internal static class FarmerPatches
{
    /// <summary>
    /// Disable the tool swipe for the shovel.
    /// </summary>
    /// <param name="who">Farmer.</param>
    /// <returns>false if the current tool is the shoveltool to prevent the swipes from showing up.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Farmer.showToolSwipeEffect))]
    private static bool PrefixToolSwipe(Farmer who) => who.CurrentTool is not ShovelTool;
}

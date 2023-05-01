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

namespace CameraPan.HarmonyPatches;

/// <summary>
/// Patches against GameLocation.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class GameLocationPatches
{
    [HarmonyPatch(nameof(GameLocation.startEvent))]
    private static void Prefix()
    {
        ModEntry.Reset();
        ModEntry.SnapOnNextTick = true;
    }
}

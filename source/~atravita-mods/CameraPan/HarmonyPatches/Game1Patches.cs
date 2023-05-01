/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using HarmonyLib;

namespace CameraPan.HarmonyPatches;

/// <summary>
/// Holds patches against Game1.
/// </summary>
[HarmonyPatch(typeof(Game1))]
internal static class Game1Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Game1.afterFadeReturnViewportToPlayer))]
    private static void PostfixAfterFade() => ResetAndSnap();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Game1.globalFadeToClear))]
    private static void PrefixFadeToClear() => ResetAndSnap();

    [HarmonyPostfix]
    [HarmonyPatch("onFadeToBlackComplete")]
    private static void PostfixFadeToBlack() => ResetAndSnap();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Game1.moveViewportTo))]
    private static void PrefixMoveViewport(ref Game1.afterFadeFunction? reachedTarget)
    {
        if (reachedTarget is null)
        {
            reachedTarget = ModEntry.ZeroOffset;
        }
        else
        {
            reachedTarget += ModEntry.ZeroOffset;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ResetAndSnap()
    {
        ModEntry.SnapOnNextTick = true;
        ModEntry.Reset();
    }
}

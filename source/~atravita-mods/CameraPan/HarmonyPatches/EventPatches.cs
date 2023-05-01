/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;

using HarmonyLib;

namespace CameraPan.HarmonyPatches;

/// <summary>
/// Patches on events.
/// </summary>
[HarmonyPatch]
internal static class EventPatches
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Game1), nameof(Game1.eventFinished));
        yield return AccessTools.Method(typeof(Event), nameof(Event.endBehaviors));
    }

    [HarmonyPostfix]
    private static void PostfixEventEnd()
    {
        DelayedAction.functionAfterDelay(
            () =>
            {
                var pos = Game1.player.getStandingXY();
                ModEntry.Reset();
                ModEntry.SnapOnNextTick = true;
            },
            25);
    }
}

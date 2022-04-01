/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

#if DEBUG
using AtraShared.Utils.Extensions;
using HarmonyLib;

namespace StopRugRemoval.HarmonyPatches.Niceties;

[HarmonyPatch(typeof(NPC))]
internal static class SayHiToPatch
{
    [HarmonyPatch(nameof(NPC.sayHiTo))]
    private static void Postfix(NPC __instance, Character c)
    {
        ModEntry.ModMonitor.DebugLog($"{__instance.Name} trying to say hi to {c.Name}", LogLevel.Alert);
    }
}
#endif
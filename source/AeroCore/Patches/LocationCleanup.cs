/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;

namespace AeroCore.Patches
{
    [HarmonyPatch(typeof(GameLocation))]
    internal class LocationCleanup
    {
        internal static event Action<GameLocation> Cleanup;

        [HarmonyPatch(nameof(GameLocation.cleanupBeforePlayerExit))]
        [HarmonyPostfix]
        internal static void AfterCleanup(GameLocation __instance)
            => Cleanup?.Invoke(__instance);
    }
}

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
    [HarmonyPatch]
    internal class MenuCleanup
    {
        [HarmonyPatch(typeof(Game1), nameof(Game1.exitActiveMenu))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        internal static void Prefix()
        {
            if (Game1.activeClickableMenu is IDisposable d)
                d.Dispose();
        }
    }
}

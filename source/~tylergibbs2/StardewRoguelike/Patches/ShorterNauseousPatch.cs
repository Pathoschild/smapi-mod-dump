/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Buff), MethodType.Constructor, new Type[] { typeof(int) })]
    internal class ShorterNauseousPatch
    {
        public static void Postfix(Buff __instance, int which)
        {
            if (which == 25 && __instance.millisecondsDuration >= 10 * 1000)
                __instance.millisecondsDuration = 10 * 1000;
        }
    }
}

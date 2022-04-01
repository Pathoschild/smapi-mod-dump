/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;

namespace HDPortraits.Patches
{
    [HarmonyPatch(typeof(Event))]
    internal class EventPatch
    {
        [HarmonyPatch("command_changePortrait")]
        [HarmonyPostfix]
        public static void ChangePortraitPostfix(string[] split)
        {
            PortraitDrawPatch.EventOverrides.Value.Add(split[1], split[2]);
        }

        [HarmonyPatch("cleanup")]
        [HarmonyPostfix]
        public static void CleanupPostfix()
        {
            PortraitDrawPatch.EventOverrides.Value.Clear();
        }
    }
}

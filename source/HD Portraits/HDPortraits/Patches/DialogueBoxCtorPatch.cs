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
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HDPortraits.Patches
{
    [HarmonyPatch]
    class DialogueBoxCtorPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (var ctor in AccessTools.GetDeclaredConstructors(typeof(DialogueBox)))
                yield return ctor;
        }
        public static void Postfix(DialogueBox __instance)
        {
            DialoguePatch.Init(__instance);
        }
    }
}

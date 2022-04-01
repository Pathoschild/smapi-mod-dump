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
    [HarmonyPatch(typeof(GameLocation))]
    internal class GameLocationPatch
    {
        [HarmonyPatch("answerDialogueAction")]
        [HarmonyPrefix]
        public static void answerDialogueActionPrefix(string questionAndAnswer, GameLocation __instance)
        {
            if(questionAndAnswer.StartsWith("telephone_"))
                PortraitDrawPatch.overrideName.Value = "AnsweringMachine";
            else
                PortraitDrawPatch.overrideName.Value = null;
        }
    }
}

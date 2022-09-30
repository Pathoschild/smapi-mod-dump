/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(GameLocation), "answerDialogueAction")]
    internal class GameLocationAnswerDialogue
    {
        public static void Postfix(GameLocation __instance, ref bool __result, string questionAndAnswer, string[] questionParams)
        {
            if (Roguelike.AnswerDialogueAction(__instance, questionAndAnswer, questionParams))
                __result = true;
            else if (ChallengeFloor.AnswerDialogueAction(__instance as MineShaft, questionAndAnswer, questionParams))
                __result = true;
            else if (Merchant.AnswerDialogueAction(__instance as MineShaft, questionAndAnswer, questionParams))
                __result = true;
        }
    }
}

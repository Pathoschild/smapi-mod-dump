/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using HarmonyLib;
using StardewValley;

namespace TarotEvent;

[HarmonyPatch]
class HarmonyPatches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.performAction))]
    [HarmonyPrefix]
    public static bool performAction_Prefix(string action, Farmer who)
    {
        if (action != "DialaTarot" || !who.IsLocalPlayer)
            return true;

        GameLocation currentLoc = Game1.currentLocation;
        currentLoc.createQuestionDialogue("Would you like to have a tarot reading done?", currentLoc.createYesNoResponses(), "tarotReading");
        return false;
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogueAction))]
    [HarmonyPrefix]
    public static bool answerDialogueAction_Prefix(string questionAndAnswer)
    {
        if (questionAndAnswer != "tarotReading_Yes")
            return true;

        GameLocation currentLoc = Game1.currentLocation;
        currentLoc.startEvent(new Event("none/-100 -100/farmer -100 -100 0/globalFadeToClear/skippable/bgColor 0 0 0/ambientLight 0 0 0/changeToTemporaryMap TestTarot/viewport 14 8 true/pause 1200/message \"The Ace of Cups: Signals the start of something beautiful when it comes to new relationships.\"/pause 500/message \"The Sun: There is happiness, celebration, and fulfillment in this relationship.\"/pause 500/message \"The Lovers: Signals pure love and harmony between you and your partner.\"/pause 1000/globalFade/viewport -999 -999/end"));
        return false;
    }
}

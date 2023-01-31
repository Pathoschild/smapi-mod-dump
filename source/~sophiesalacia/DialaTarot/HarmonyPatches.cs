/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;

namespace DialaTarotCSharp;

[HarmonyPatch]
class HarmonyPatches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.performAction))]
    [HarmonyPrefix]
    public static bool performAction_Prefix(string action, Farmer who, Location tileLocation)
    {
        if (action != "DialaTarot" || !who.IsLocalPlayer)
            return true;

        GameLocation currentLoc = Game1.currentLocation;

        #if DEBUG
            currentLoc.createQuestionDialogue("Would you like to have a tarot reading done?",
            currentLoc.createYesNoResponses(), "tarotReading");
            return false;
        #endif

        // if diala is in the current location and near enough to the tile location
        if (currentLoc.characters.Where(npc => npc.Name == "Diala").TakeWhile(npc => Vector2.Distance(npc.getTileLocation(), new Vector2(tileLocation.X, tileLocation.Y)) > 3f).Any())
        {
            // if you've already done a reading today, reject with specific message
            if (who.modData.ContainsKey("sophie.DialaTarot/ReadingDoneForToday"))
            {
                Game1.drawObjectDialogue("You've already had a reading done today. Come back another time.");
                return false;
            }

            // if you have seen the necessary event
            if (who.eventsSeen.Contains(20031411))
            {
                currentLoc.createQuestionDialogue("Would you like to have a tarot reading done?",
                    currentLoc.createYesNoResponses(), "tarotReading");
                return false;
            }

            // otherwise generic rejection dialogue
            Game1.drawObjectDialogue("Diala is busy.");
            return false;
        }

        // if diala is not on the map or near the tile location
        Game1.drawObjectDialogue("Come back when Diala is here.");
        return false;

    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogueAction))]
    [HarmonyPrefix]
    public static bool answerDialogueAction_Prefix(string questionAndAnswer)
    {
        if (questionAndAnswer != "tarotReading_Yes")
            return true;

        Game1.player.modData["sophie.DialaTarot/ReadingDoneForToday"] = "true";

        Game1.activeClickableMenu = null;
        GameLocation currentLoc = Game1.currentLocation;

        string eventString = Game1.content.Load<Dictionary<string, string>>("sophie.DialaTarot/Event")["Event"];

        currentLoc.startEvent(new Event(eventString));
        return false;
    }

    [HarmonyPatch(typeof(Event), nameof(Event.command_cutscene))]
    [HarmonyPrefix]
    public static bool command_cutscene_Prefix(Event __instance, string[] split)
    {
        // if custom event script is active, skip prefix and run original code
        if (__instance.currentCustomEventScript != null)
        {
            return true;
        }
        if (split[1] == "DialaTarot")
        {
            __instance.currentCustomEventScript = new EventScriptDialaTarot();
        }

        return true;
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Reflection;
using BirbCore.Attributes;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData.BigCraftables;

namespace GameboyArcade;

class Utilities
{
    public static void ShowArcadeMenu(string minigameId, string arcadeName)
    {
        Response[] arcadeOptions = new Response[2]
        {
            new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play")),
            new Response("Exit", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave")),
        };
        Game1.currentLocation.createQuestionDialogue($"== {arcadeName} ==", arcadeOptions, $"drbirbdev.GameboyArcade {minigameId}");
    }
}

[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogueAction))]
class GameLocation_AnswerDialogueAction
{
    internal static void Postfix(string questionAndAnswer, string[] questionParams, ref bool __result)
    {
        try
        {
            if (questionAndAnswer == "drbirbdev.GameboyArcade_Play")
            {
                if (questionParams is null || questionParams.Length < 2)
                {
                    Log.Error("drbirbdev.ArcadeGame_Play dialogueKey requires minigame id parameter");
                    return;
                }

                Content content = null;

                content = ModEntry.GetGame(questionParams[1]);

                if (content is null)
                {
                    Log.Error($"drbirbdev.ArcadeGame_Play dialogueKey had unknown minigame id parameter {questionParams.Join(delimiter: ",")}");
                    return;
                }

                GameboyMinigame.LoadGame(content);

                __result = true;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

[HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.checkForAction))]
class Object_CheckForAction
{
    internal static bool Prefix(Farmer who, bool justCheckingForActivity, StardewValley.Object __instance, ref bool __result)
    {
        try
        {
            if (ItemRegistry.GetData(__instance.QualifiedItemId).RawData is not BigCraftableData data)
            {
                return true;
            }

            if (data.CustomFields == null)
            {
                return true;
            }
            if (!data.CustomFields.TryGetValue("drbirbdev.GameboyArcade_GameID", out string gameId))
            {
                return true;
            }

            if (justCheckingForActivity)
            {
                __result = true;
                return false;
            }
            Content content = ModEntry.GetGame(gameId);
            Utilities.ShowArcadeMenu(content.UniqueID, content.Name);
            return false;

        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }
        return true;
    }
}


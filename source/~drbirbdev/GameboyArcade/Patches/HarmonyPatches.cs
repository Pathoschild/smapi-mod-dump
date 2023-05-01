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
using BirbShared;
using HarmonyLib;
using StardewValley;

namespace GameboyArcade
{
    class Utilities
    {
        public static void ShowArcadeMenu(string minigameId, string arcadeName) {
            Response[] arcadeOptions = new Response[2]
            {
                new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play")),
                new Response("Exit", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave")),
            };
            Game1.currentLocation.createQuestionDialogue($"== {arcadeName} ==", arcadeOptions, $"drbirbdev.GameboyArcade {minigameId}");
        }
    }

    [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.checkForAction))]
    class Object_CheckForAction
    {
        internal static void Postfix(Farmer who, bool justCheckingForActivity, ref bool __result, StardewValley.Object __instance)
        {
            try
            {
                // TODO: check if BigCraftable is any of the loaded content packs
                if (ModEntry.DynamicGameAssets is null)
                {
                    return;
                }
                string dgaId = ModEntry.DynamicGameAssets.GetDGAItemId(__instance);
                if (dgaId is not null && ModEntry.BigCraftableIDMap.ContainsKey(dgaId))
                {
                    if (justCheckingForActivity)
                    {
                        __result = true;
                        return;
                    }
                    string minigameId = ModEntry.BigCraftableIDMap[dgaId];

                    Utilities.ShowArcadeMenu(minigameId, __instance.DisplayName);
                }

            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogueAction))]
    class GameLocation_AnswerDialogueAction
    {
        internal static void Postfix(string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (!__result && questionAndAnswer == "drbirbdev.GameboyArcade_Play")
                {
                    if (questionParams is null || questionParams.Length < 2)
                    {
                        Log.Error("drbirbdev.ArcadeGame_Play dialogueKey requires minigame id parameter");
                        return;
                    }
                    if (!ModEntry.LoadedContentPacks.TryGetValue(questionParams[1], out Content content)) {
                        Log.Error($"drbirbdev.ArcadeGame_Play dialogueKey had unknown minigame id parameter {questionParams[0]}");
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

    [HarmonyPatch(typeof(Event), nameof(Event.command_cutscene))]
    class Event_Command_Cutscene
    {
        internal static void Postfix(string[] split, Event __instance)
        {
            try
            {
                if (Game1.currentMinigame != null)
                {
                    return;
                }
                if (ModEntry.LoadedContentPacks.ContainsKey(split[1]))
                {
                    Content content = ModEntry.LoadedContentPacks[split[1]];
                    if (!content.EnableEvents)
                    {
                        Log.Error($"Event is attempting to use minigame {content.UniqueID} in a cutscene, but that content pack has disallowed this.");
                        return;
                    }

                    GameboyMinigame.LoadGame(content, true);
                    __instance.CurrentCommand++;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.performAction))]
    class GameLocation_PerformAction
    {
        internal static void Postfix(string action, Farmer who)
        {
            try
            {
                if (action != null && who.IsLocalPlayer && action.StartsWith("drbirbdev.GameboyArcade "))
                {
                    string[] actionParams = action.Split(' ');
                    if (actionParams.Length < 2)
                    {
                        Log.Error($"TileLocation is attempting to play a Gameboy Arcade minigame without specifying which game.");
                        return;
                    }
                    if (!ModEntry.LoadedContentPacks.ContainsKey(actionParams[1]))
                    {
                        Log.Error($"TileLocation is attempting to play a non-existent Gameboy Arcade minigame.");
                        return;
                    }
                    Content content = ModEntry.LoadedContentPacks[actionParams[1]];
                    Utilities.ShowArcadeMenu(content.UniqueID, content.Name);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

}

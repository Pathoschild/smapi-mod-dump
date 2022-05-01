/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using GingerIslandMainlandAdjustments.AssetManagers;
using GingerIslandMainlandAdjustments.MultiplayerHandler;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace GingerIslandMainlandAdjustments.Niceties;

/// <summary>
/// Class that handles patches against GameLocation...to handle the phone.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal static class PhoneHandler
{
    /// <summary>
    /// Prefix that lets me inject Pam into the phone menu.
    /// </summary>
    /// <param name="answerChoices">Responses.</param>
    /// <param name="dialogKey">Question key, used to keep track of which question set.</param>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameLocation.createQuestionDialogue), new Type[] { typeof(string), typeof(Response[]), typeof(string) })]
    private static void PrefixQuestionDialogue(ref Response[] answerChoices, string dialogKey)
    {
        if (dialogKey.Equals("telephone", StringComparison.OrdinalIgnoreCase)
            && Game1.player.mailReceived.Contains(AssetEditor.PAMMAILKEY)
            && Game1.getCharacterFromName("Pam") is NPC pam // omit if Pam inexplicably vanished.
            && answerChoices.Any((Response r) => r.responseKey.Equals("Carpenter", StringComparison.OrdinalIgnoreCase)))
        {
            List<Response> responseList = new() { new Response("PamBus", pam.displayName) };
            responseList.AddRange(answerChoices);
            answerChoices = responseList.ToArray();
        }
    }

    /// <summary>
    /// Postfixing answerDialogueAction to handle Pam's phone calls.
    /// </summary>
    /// <param name="__instance">Location we're calling from.</param>
    /// <param name="questionAndAnswer">questionAndAnswer.</param>
    /// <param name="__result">Result.</param>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameLocation.answerDialogueAction), new Type[] { typeof(string), typeof(string[]) })]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixAnswerDialogueAction(GameLocation __instance, string questionAndAnswer, ref bool __result)
    {
        if (questionAndAnswer.Equals("telephone_PamBus", StringComparison.OrdinalIgnoreCase))
        {
            Globals.ReflectionHelper.GetMethod(__instance, "playShopPhoneNumberSounds").Invoke(questionAndAnswer);
            Game1.player.freezePause = 4950;
            DelayedAction.functionAfterDelay(
            () =>
            {
                try
                {
                    Game1.playSound("bigSelect");
                    if (Game1.getCharacterFromName("Pam") is not NPC pam)
                    {
                        Globals.ModMonitor.Log($"Pam cannot be found, ending phone call.", LogLevel.Warn);
                        return;
                    }
                    if (Game1.timeOfDay > 2200)
                    {
                        Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Bus_Late"));
                        return;
                    }
                    if (Game1.timeOfDay < 900)
                    {
                        if (Game1.IsVisitingIslandToday(pam.Name))
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString($"Strings\\Characters:Pam_Island_{Game1.random.Next(1, 4)}"));
                        }
                        else if (Utility.IsHospitalVisitDay(pam.Name))
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Doctor"));
                        }
                        else if (MultiplayerSharedState.PamsSchedule is null)
                        {
                            Globals.ModMonitor.Log("Something very odd has happened. Pam's dayScheduleName is null", LogLevel.Debug);
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Other"));
                        }
                        else if (MultiplayerSharedState.PamsSchedule.Contains("BusStop 11 10"))
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString($"Strings\\Characters:Pam_Bus_{Game1.random.Next(1, 4)}"));
                        }
                        else
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Other"));
                        }
                    }
                    else
                    {
                        if (Game1.IsVisitingIslandToday(pam.Name))
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Voicemail_Island"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
                        }
                        else if (Utility.IsHospitalVisitDay(pam.Name))
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Voicemail_Doctor"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
                        }
                        else if (MultiplayerSharedState.PamsSchedule is null)
                        {
                            Globals.ModMonitor.Log("Something very odd has happened. Pam's dayScheduleName is not found?", LogLevel.Debug);
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Voicemail_Other"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
                        }
                        else if (MultiplayerSharedState.PamsSchedule.Contains("BusStop 11 10"))
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Voicemail_Bus"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
                        }
                        else
                        {
                            Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Voicemail_Other"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"));
                        }
                    }
                    __instance.answerDialogueAction("HangUp", Array.Empty<string>());
                }
                catch (Exception ex)
                {
                    Globals.ModMonitor.Log($"Error handling Pam's phone call {ex}", LogLevel.Error);
                }
            },
            4950);
            __result = true;
        }
    }
}
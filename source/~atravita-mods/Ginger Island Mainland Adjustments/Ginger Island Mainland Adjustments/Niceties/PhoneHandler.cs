/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;

using AtraCore.Framework.Caches;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using GingerIslandMainlandAdjustments.AssetManagers;
using GingerIslandMainlandAdjustments.MultiplayerHandler;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace GingerIslandMainlandAdjustments.Niceties;

/// <summary>
/// Class that handles patches against GameLocation...to handle the phone.
/// </summary>
[HarmonyPatch]
internal static class PhoneHandler
{
    /// <summary>
    /// Injects Pam into the phone menu if necessary.
    /// </summary>
    private static void AdjustQuestionDialogue(List<Response> answerChoices)
    {
        // omit if Pam inexplicably vanished.
        if (Game1.player.mailReceived.Contains(AssetEditor.PAMMAILKEY)
            && NPCCache.GetByVillagerName("Pam") is NPC pam)
        {
            answerChoices.Add(new Response("PamBus", pam.displayName));
        }
    }

    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(typeof(Game1), nameof(Game1.ShowTelephoneMenu))]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, Globals.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                (OpCodes.Newobj, typeof(List<Response>).GetCachedConstructor(ReflectionCache.FlagTypes.InstanceFlags)),
                SpecialCodeInstructionCases.StLoc,
            })
            .Advance(1);

            CodeInstruction? ldloc = helper.CurrentInstruction.ToLdLoc();

            helper.Advance(1)
            .GetLabels(out IList<Label>? labelsToMove)
            .Insert(new CodeInstruction[]
            {
                ldloc,
                new(OpCodes.Call, typeof(PhoneHandler).GetCachedMethod(nameof(AdjustQuestionDialogue), ReflectionCache.FlagTypes.StaticFlags)),
            }, withLabels: labelsToMove);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Mod crashed while transpiling {original.FullDescription()}:\n\n{ex}", LogLevel.Error);
            original.Snitch(Globals.ModMonitor);
        }
        return null;
    }

    /// <summary>
    /// Postfixing answerDialogueAction to handle Pam's phone calls.
    /// </summary>
    /// <param name="__instance">Location we're calling from.</param>
    /// <param name="questionAndAnswer">questionAndAnswer.</param>
    /// <param name="__result">Result.</param>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.answerDialogueAction), new Type[] { typeof(string), typeof(string[]) })]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixAnswerDialogueAction(GameLocation __instance, string questionAndAnswer, ref bool __result)
    {
        if (questionAndAnswer.Equals("telephone_PamBus", StringComparison.OrdinalIgnoreCase))
        {
            Globals.ReflectionHelper.GetMethod(__instance, "playShopPhoneNumberSounds", required: false)?.Invoke(questionAndAnswer);
            Game1.player.freezePause = GameLocation.PHONE_RING_DURATION;
            DelayedAction.functionAfterDelay(
            () =>
            {
                try
                {
                    Game1.playSound(GameLocation.PHONE_PICKUP_SOUND);
                    if (NPCCache.GetByVillagerName("Pam") is not NPC pam)
                    {
                        Globals.ModMonitor.Log($"Pam cannot be found, ending phone call.", LogLevel.Warn);
                        return;
                    }
                    else if (Game1.timeOfDay > 2200)
                    {
                        Game1.drawDialogue(pam, Game1.content.LoadString("Strings\\Characters:Pam_Bus_Late"));
                        return;
                    }
                    else if (Game1.timeOfDay < 900)
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
                    __instance.answerDialogueAction(GameLocation.PHONE_HANGUP_SOUND, Array.Empty<string>());
                }
                catch (Exception ex)
                {
                    Globals.ModMonitor.Log($"Error handling Pam's phone call {ex}", LogLevel.Error);
                }
            },
            GameLocation.PHONE_RING_DURATION);
            __result = true;
        }
    }
}
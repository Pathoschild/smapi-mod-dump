/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.GameModifications
{
    public class MultiSleep
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private Harmony _harmony;

        public static int DaysToSkip = 0;
        private static int _multiSleepPrice = -1;

        public MultiSleep(IMonitor monitor, IModHelper modHelper, Harmony harmony)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _harmony = harmony;
            DaysToSkip = 0;
        }

        public void InjectMultiSleepOption(SlotData slotData)
        {
            if (!slotData.EnableMultiSleep)
            {
                return;
            }

            _multiSleepPrice = slotData.MultiSleepCostPerDay;

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction)),
                prefix: new HarmonyMethod(typeof(MultiSleep), nameof(MultiSleep.PerformTouchAction_Sleep_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(MultiSleep), nameof(MultiSleep.AnswerDialogueAction_SleepMany_Prefix))
            );
        }

        public static bool PerformTouchAction_Sleep_Prefix(GameLocation __instance, string fullActionString, Vector2 playerStandingPosition)
        {
            try
            {
                var actionStringFirstWord = fullActionString.Split(' ')[0];

                if (Game1.eventUp || actionStringFirstWord != "Sleep" || Game1.newDay || !Game1.shouldTimePass() || !Game1.player.hasMoved || Game1.player.passedOut)
                {
                    return true; // run original logic
                }

                var possibleResponses = new Response[3]
                {
                    new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes")).SetHotKey(Keys.Y),
                    new Response("Many", "Sleep for multiple days").SetHotKey(Keys.None),
                    new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape),
                };
                
                __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"), possibleResponses, "Sleep", null);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PerformTouchAction_Sleep_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool AnswerDialogueAction_SleepMany_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (questionAndAnswer != "Sleep_Many")
                {
                    return true; // run original logic
                }

                var multiSleepMessage =
                    "How many days do you wish to sleep for?\n(Warning: Sleeping saves the game, this action cannot be undone)";
                Game1.activeClickableMenu = new MultiSleepSelectionMenu(multiSleepMessage, (value, price, who) => SleepMany(__instance, value), minValue: 1, maxValue: 112, defaultNumber: 7, price: _multiSleepPrice);
                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_SleepMany_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static void SleepMany(GameLocation instance, int numberOfDays)
        {
            DaysToSkip = numberOfDays - 1;
            var totalPrice = 0;
            if (_multiSleepPrice > 0)
            {
                totalPrice = _multiSleepPrice * DaysToSkip;
            }

            if (Game1.player.Money < totalPrice)
            {
                return;
            }

            Game1.player.Money -= totalPrice;

            var startSleepMethod = _modHelper.Reflection.GetMethod(instance, "startSleep");
            startSleepMethod.Invoke();
        }
    }
}

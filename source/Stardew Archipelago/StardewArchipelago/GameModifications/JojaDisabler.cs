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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.GameModifications
{
    public class JojaDisabler
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private Harmony _harmony;

        public JojaDisabler(IMonitor monitor, IModHelper modHelper, Harmony harmony)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _harmony = harmony;
        }

        public void DisableJojaMembership()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(JojaMart), nameof(JojaMart.answerDialogue)),
                prefix: new HarmonyMethod(typeof(JojaDisabler), nameof(AnswerDialogue_JojaMembershipPurchase_Prefix))
            );
        }

        // public override bool answerDialogue(Response answer)
        public static bool AnswerDialogue_JojaMembershipPurchase_Prefix(JojaMart __instance, Response answer, ref bool __result)
        {
            try
            {
                if (__instance.lastQuestionKey == null || __instance.afterQuestion != null)
                {
                    return true; // run original logic
                }

                var response = ArgUtility.SplitBySpaceAndGet(__instance.lastQuestionKey, 0) + "_" + answer.responseKey;
                if (!response.Equals("JojaSignUp_Yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true; // run original logic
                }

                const int membershipPrice = 5000;
                if (Game1.player.Money >= membershipPrice)
                {
                    const string competingBrandText = "I see you are already a member of a competing brand... \"Archipelago\"? Here at JojaMart, we believe in complete commitment to our superior services. Please cancel that membership and come back afterwards.";
                    Game1.drawObjectDialogue(competingBrandText);
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
                }

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogue_JojaMembershipPurchase_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace AnimalHusbandryMod.common
{
    internal class TvOverrides
    {
        [HarmonyPriority(Priority.First)]
        public static bool createQuestionDialogue(string question, ref Response[] answerChoices)
        {
            if (question == Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13120"))
            {
                List<Response> answerChoicesList = new List<Response>(answerChoices);
                TvController.GetChannelsWithEpisodeToday().ForEach(c =>
                {
                    answerChoicesList.Insert(answerChoicesList.Count - 1, new Response(c.GetName, c.GetDisplayName));
                });
                answerChoices = answerChoicesList.ToArray();
            }
            return true;
        }

        /// <summary>
        /// Changed from a patch to a private method to avoid conflict with PyTK
        /// </summary>
        private static void selectChannel(TV __instance, Farmer who, string answer)
        {
            Channel channel = TvController.GetChannel(answer);
            if (channel != null)
            {
                DataLoader.Helper.Reflection.GetField<int>(__instance, "currentChannel").SetValue(-1);
                DataLoader.Helper.Reflection.GetField<TemporaryAnimatedSprite>(__instance, "screen").SetValue(
                    new TemporaryAnimatedSprite(channel.GetScreenTextureName, channel.GetScreenSourceRectangle, 150f, 2, 999999, __instance.getScreenPosition(), flicker: false, flipped: false, (float)(__instance.boundingBox.Value.Bottom - 1) / 10000f + 1E-05f, 0f, Color.White, __instance.getScreenSizeModifier(), 0f, 0f, 0f)
                );
                Game1.multipleDialogues(channel.GetEpisodesText());
                Game1.afterDialogues = __instance.turnOffTV;
            }
        }

        public static void checkForAction_postfix(TV __instance)
        {
            Game1.currentLocation.afterQuestion = (GameLocation.afterQuestionBehavior) Delegate.Combine(Game1.currentLocation.afterQuestion, new GameLocation.afterQuestionBehavior( (Farmer who, string which) => selectChannel(__instance, who,which)));
        }
    }
}

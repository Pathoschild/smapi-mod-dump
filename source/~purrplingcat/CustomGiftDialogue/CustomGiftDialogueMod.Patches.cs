/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace CustomGiftDialogue
{
    public sealed partial class CustomGiftDialogueMod
    {
        private static void PATCH__After_chooseSecretSantaGift(Event __instance, Item i)
        {
            if (i is SObject o)
            {
                NPC actorByName = __instance.getActorByName(secretSantaRecipient);

                if (GiftDialogueHelper.FetchGiftReaction(actorByName, o, out string giftDialogue, "_SecretSanta"))
                {
                    GiftDialogueHelper.CancelCurrentDialogue(actorByName);
                    Game1.drawDialogue(actorByName, giftDialogue);
                }
            }

            // Clear secret santa recipient name after object received
            secretSantaRecipient = null;
        }

        private static void PATCH__After_receiveGift(NPC __instance, SObject o)
        {
            try
            {
                string bdaySuffix = __instance.isBirthday(Game1.currentSeason, Game1.dayOfMonth) ? "_Birthday" : "";

                if (GiftDialogueHelper.FetchGiftReaction(__instance, o, out string giftDialogue, bdaySuffix))
                {
                    GiftDialogueHelper.CancelCurrentDialogue(__instance);
                    Game1.drawDialogue(__instance, giftDialogue);
                }
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"An error occurred during handle custom gift reaction dialogue: {ex.Message}", LogLevel.Error);
                ModMonitor.Log(ex.ToString());
            }
        }

        [HarmonyBefore("purrplingcat.npcadventure")]
        private static bool PATCH__Before_checkAction(NPC __instance, ref bool __result, Farmer who)
        {
            if (!Config.EnableNpcGifts)
                return true;

            if (Game1.eventUp || __instance.IsInvisible || __instance.isSleeping.Value || !who.CanMove)
                return true;

            if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift())
                return true;

            if (Game1.dialogueUp)
                return true;

            if (_npcGiftManager.NpcGiftStates.ContainsKey(__instance.Name))
            {
                if (_npcGiftManager.TryGiveGift(__instance, Game1.player))
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }

        private static void PATCH__Before_chooseSecretSantaGift(Event __instance)
        {
            // Because original method clears secretSantaRecipient on Event class instance, we must save it by side
            secretSantaRecipient = __instance.secretSantaRecipient.Name;
        }

        private static bool PATCH__Before_hasTemporaryMessageAvailable(NPC __instance, ref bool __result)
        {
            if (_npcGiftManager.NpcGiftStates.TryGetValue(__instance.Name, out var giftState) && giftState == NpcGiftManager.GiftState.CAN_GIVE_GIFT)
            {
                __result = true;
                return false;
            }
            return true;
        }

        private static bool PATCH__Before_loadCurrentDialogue(NPC __instance, ref Stack<Dialogue> __result)
        {
            int heartLevel = Game1.player.friendshipData.TryGetValue(__instance.Name, out Friendship friends)
                ? (friends.Points / 250)
                : 0;
            Random r = new(
                (int)(Game1.stats.DaysPlayed * 77)
                + (int)Game1.uniqueIDForThisGame / 2 + 2
                + (int)__instance.DefaultPosition.X * 77
                + (int)__instance.DefaultPosition.Y * 777
            );

            if (r.NextDouble() < Config.RevealDialogueChance && heartLevel >= Config.RevealDialogueMinHeartLevel)
            {
                if (__instance.Dialogue != null && GiftDialogueHelper.GetRevealDialogue(__instance, out string dialogue))
                {
                    __result = new Stack<Dialogue>();
                    __result.Push(new Dialogue(dialogue, __instance));
                    __instance.updatedDialogueYet = true;

                    return false;
                }
            }

            return true;
        }
    }
}
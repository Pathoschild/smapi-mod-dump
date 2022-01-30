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
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace CustomGiftDialogue
{
    /// <summary>The mod entry point.</summary>
    public class CustomGiftDialogueMod : Mod
    {
        internal static IMonitor ModMonitor { get; private set; }
        internal static IReflectionHelper Reflection { get; private set; }
        internal static CustomGiftDialogueConfig Config { get; private set; }
        private static string secretSantaRecipient;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Reflection = helper.Reflection;
            Config = helper.ReadConfig<CustomGiftDialogueConfig>();
            ModMonitor = this.Monitor;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                postfix: new HarmonyMethod(this.GetType(), nameof(PATCH__After_receiveGift))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "loadCurrentDialogue"),
                prefix: new HarmonyMethod(this.GetType(), nameof(PATCH__Before_loadCurrentDialogue))
            );

            if (Config.CustomSecretSantaDialogues)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Event), nameof(Event.chooseSecretSantaGift)),
                    prefix: new HarmonyMethod(this.GetType(), nameof(PATCH__Before_chooseSecretSantaGift)),
                    postfix: new HarmonyMethod(this.GetType(), nameof(PATCH__After_chooseSecretSantaGift))
                );
            }

            helper.ConsoleCommands.Add("cdgu_reveal", "cdgu_reveal <npc> [<aboutNpc>] - Test reveal dialogue", this.CommandReveal);
        }

        private void CommandReveal(string name, string[] args)
        {
            if (args.Length < 1) return;

            NPC npc = Game1.getCharacterFromName(args[0]);

            if (npc == null)
            {
                this.Monitor.Log($"Unknown NPC name: ${args[0]}", LogLevel.Error);
                return;
            }

            if (GiftDialogueHelper.GetRevealDialogue(npc, out string dialogue, args.Length > 1 ? args[1] : null))
            {
                npc.CurrentDialogue.Push(new Dialogue(dialogue, npc));
                this.Monitor.Log($"A reveal dialogue was spawned for {npc.Name}", LogLevel.Info);
                return;
            }

            this.Monitor.Log($"No reveal dialogue defined for npc {npc.Name}", LogLevel.Alert);
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

        private static void PATCH__Before_chooseSecretSantaGift(Event __instance)
        {
            // Because original method clears secretSantaRecipient on Event class instance, we must save it by side
            secretSantaRecipient = __instance.secretSantaRecipient.Name;
        }

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
    }
}

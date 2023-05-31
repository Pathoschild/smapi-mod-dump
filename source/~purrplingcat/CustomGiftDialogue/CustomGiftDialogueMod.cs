/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using CustomGiftDialogue.Data;
using HarmonyLib;
using PurrplingCore.Dialogues;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace CustomGiftDialogue
{
    /// <summary>The mod entry point.</summary>
    public sealed partial class CustomGiftDialogueMod : Mod
    {
        internal static IMonitor ModMonitor => Instance.Monitor;
        internal static IReflectionHelper Reflection { get; private set; }
        internal static CustomGiftDialogueConfig Config { get; private set; }
        internal static CustomGiftDialogueMod Instance { get; private set; }
        private static string secretSantaRecipient;
        private static NpcGiftManager _npcGiftManager;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Reflection = helper.Reflection;
            Config = helper.ReadConfig<CustomGiftDialogueConfig>();
            _npcGiftManager = new NpcGiftManager(helper.Events, helper.Translation);

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift)),
                postfix: new HarmonyMethod(GetType(), nameof(PATCH__After_receiveGift))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "loadCurrentDialogue"),
                prefix: new HarmonyMethod(GetType(), nameof(PATCH__Before_loadCurrentDialogue))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(GetType(), nameof(PATCH__Before_checkAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.hasTemporaryMessageAvailable)),
                prefix: new HarmonyMethod(GetType(), nameof(PATCH__Before_hasTemporaryMessageAvailable))
            );

            if (Config.CustomSecretSantaDialogues)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Event), nameof(Event.chooseSecretSantaGift)),
                    prefix: new HarmonyMethod(GetType(), nameof(PATCH__Before_chooseSecretSantaGift)),
                    postfix: new HarmonyMethod(GetType(), nameof(PATCH__After_chooseSecretSantaGift))
                );
            }

            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.ConsoleCommands.Add("cdgu_reveal", "cdgu_reveal <npc> [<aboutNpc>] - Test reveal dialogue", this.CommandReveal);
        }

        /// <summary>
        /// Provide NPC farmer greeting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            Farmer f = Game1.player;
            Character c = Utility.isThereAFarmerOrCharacterWithinDistance(f.getTileLocation(), Config.NpcGreetDistance, f.currentLocation);

            if (Game1.player.friendshipData.ContainsKey(c.Name) && c is NPC npc && npc.isVillager())
            {
                if (Game1.random.NextDouble() > Config.NpcGreetChance) { return; }

                if (GiftDialogueHelper.TryGetFriendshipDialogue(npc, "farmerGreeting_Ambient", out string dialogue))
                {
                    npc.showTextAboveHead(dialogue);
                }
            }
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

            Monitor.Log($"No reveal dialogue defined for npc {npc.Name}", LogLevel.Alert);
        }
    }
}

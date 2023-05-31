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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PurrplingCore.Dialogues;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace CustomGiftDialogue
{
    internal class NpcGiftManager
    {
        public enum GiftState
        {
            NO_GIFT_GIVEN,
            CAN_GIVE_GIFT,
            GIFT_GIVEN,
        }

        private readonly PerScreen<Dictionary<string, GiftState>> _npcGiftStates = new(() => new());
        private readonly PerScreen<Dictionary<string, NpcGiftData.GiftData>> _npcGifts = new(() => new());
        private readonly ITranslationHelper _translation;

        private static string NpcGiftDataAssetName => $"Mods/{CustomGiftDialogueMod.Instance.ModManifest.UniqueID}/NpcGiftData";

        public Dictionary<string, GiftState> NpcGiftStates
        {
            get => _npcGiftStates.Value;
            set => _npcGiftStates.Value = value;
        }

        public Dictionary<string, NpcGiftData.GiftData> NpcGifts
        {
            get => _npcGifts.Value;
            set => _npcGifts.Value = value;
        }

        public NpcGiftManager(IModEvents events, ITranslationHelper translation)
        {
            _translation = translation;
            events.Content.AssetRequested += OnAssetRequested;
            events.GameLoop.DayStarted += OnDayStarted;
            events.GameLoop.TimeChanged += OnTimeChanged;
            events.Player.Warped += OnPlayerWarped;
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e) => CheckNpcGifts();

        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            var keys = NpcGifts.Keys.ToArray();

            // Cleanup all not actual npc gifts
            foreach (var key in keys)
            {
                if (NpcGifts.TryGetValue(key, out var giftToGive) && !CheckGiftConditions(key, giftToGive))
                {
                    NpcGiftStates[key] = GiftState.NO_GIFT_GIVEN;
                    NpcGifts.Remove(key);
                }
            }

            CheckNpcGifts();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            NpcGiftStates.Clear();
            CheckNpcGifts();
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(NpcGiftDataAssetName))
            {
                // Load empty JSON file by creating a dictionary instance
                e.LoadFrom(() => new Dictionary<string, NpcGiftData>(), AssetLoadPriority.Low);
            }
        }

        public void CheckNpcGifts()
        {
            if (!CustomGiftDialogueMod.Config.EnableNpcGifts || !Context.IsWorldReady) { return; }

            Dictionary<string, string> dispositions = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");

            foreach (Character ch in Game1.player.currentLocation.getCharacters())
            {
                if (ch is NPC npc and not Horse && dispositions.ContainsKey(ch.Name))
                {
                    CheckGiftToGive(npc);
                }
            }
        }

        public static NpcGiftData LoadGiftData(NPC npc)
        {
            var giftDataDict = Game1.content.Load<Dictionary<string, NpcGiftData>>(NpcGiftDataAssetName);

            if (giftDataDict.TryGetValue(npc.Name, out NpcGiftData giftData))
            {
                return giftData;
            }

            return null;
        }

        public static NpcGiftData.GiftData FetchGift(NpcGiftData data, NPC npc)
        {
            return (from giftData in data.Gifts
                    where CheckGiftConditions(npc.Name, giftData)
                    select giftData).FirstOrDefault();
        }

        private static bool CheckGiftConditions(string npcName, NpcGiftData.GiftData giftData)
        {
            var conditions = giftData.Condition.Replace("@npc", npcName);

            return string.IsNullOrEmpty(giftData.Condition) || GameStateQuery.CheckConditions(conditions);
        }

        public static int PickGiftItemId(string giftItemsDescription)
        {
            string[] gifts = giftItemsDescription.Split(',');

            return Convert.ToInt32(gifts[Game1.random.Next(0, gifts.Length)].Trim());
        }

        public void CheckGiftToGive(NPC npc)
        {
            if (NpcGiftStates.TryGetValue(npc.Name, out var state) && state == GiftState.GIFT_GIVEN)
            {
                return;
            }

            if (NpcGifts.TryGetValue(npc.Name, out var currentGift) && CheckGiftConditions(npc.Name, currentGift))
            {
                return;
            }

            NpcGiftStates[npc.Name] = GiftState.NO_GIFT_GIVEN;
            NpcGifts.Remove(npc.Name);

            var giftData = LoadGiftData(npc);

            if (giftData != null)
            {
                var giftToGive = FetchGift(giftData, npc);

                if (giftToGive != null && Game1.random.NextDouble() < giftData.GiveChance)
                {
                    NpcGiftStates[npc.Name] = GiftState.CAN_GIVE_GIFT;
                    NpcGifts[npc.Name] = giftToGive;
                }
            }
        }

        public bool TryGiveGift(NPC giver, Farmer receiver)
        {
            if (NpcGiftStates[giver.Name] != GiftState.CAN_GIVE_GIFT)
            {
                return false;
            }

            if (!NpcGifts.TryGetValue(giver.Name, out var giftData))
            {
                NpcGiftStates[giver.Name] = GiftState.NO_GIFT_GIVEN;
                return false;
            }

            var gift = new SObject(Vector2.Zero, PickGiftItemId(giftData.Items), null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);

            if (receiver.addItemToInventoryBool(gift, true)) {
                string dialogue = _translation.Get("npcGiveGift", new { Who = receiver.displayName });

                if (DialogueHelper.GetRawDialogue(giver.Dialogue, giftData.DialogueKey, out KeyValuePair<string, string> giftDialogue))
                {
                    dialogue = giftDialogue.Value;
                }

                NpcGiftStates[giver.Name] = GiftState.GIFT_GIVEN;
                NpcGifts.Remove(giver.Name);
                receiver.showCarrying();
                Game1.drawDialogue(giver, dialogue);

                return true; 
            }

            return false;
        }
    }
}

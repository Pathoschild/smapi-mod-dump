/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SpaceCore.Events;
using static TricksAndTreats.Globals;

namespace TricksAndTreats
{
    internal static class Treats
    {
        public const int loved_pts = 160;
        public const int neutral_pts = 90;
        public const int hated_pts = -80;

        internal static void Initialize(IMod ModInstance)
        {

            Helper.Events.GameLoop.DayStarted += CheckCandyGivers;
            SpaceEvents.BeforeGiftGiven += GiveTreat;
        }

        [EventPriority(EventPriority.Low)]
        private static void CheckCandyGivers(object sender, DayStartedEventArgs e)
        {
            foreach (KeyValuePair<string, Celebrant> entry in NPCData)
            {
                if (entry.Value.Roles.Contains("candygiver"))
                {
                    NPC npc = Game1.getCharacterFromName(entry.Key);
                    if (!npc.Dialogue.ContainsKey(TreatCT))
                    {
                        npc.Dialogue.Add(TreatCT, Helper.Translation.Get("generic.give_candy"));
                    }
                    var TreatsToGive = entry.Value.TreatsToGive;
                    Random random = new();
                    int gift = (int)TreatData[TreatsToGive[random.Next(TreatsToGive.Length)]].ObjectId;
                    npc.Dialogue[TreatCT] = npc.Dialogue[TreatCT] + $" [{gift}]";
                    Log.Trace($"TaT: NPC {entry.Key} will give treat ID {gift}");
                }
            }
        }

        private static void GiveTreat(object sender, EventArgsBeforeReceiveObject e)
        {
            if (e.Gift.Name != "TaT.mystery-treat" && !TreatData.ContainsKey(e.Gift.Name))
                return;

            e.Cancel = true;

            if (!NPCData.ContainsKey(e.Npc.Name))
                return;

            if (!(Game1.currentSeason == "fall" && Game1.dayOfMonth == 27))
            {
                if (e.Gift.Name == "TaT.mystery-treat" || TreatData[e.Gift.Name].HalloweenOnly)
                {
                    Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("info.not_halloween"));
                    return;
                }
            }

            if (!NPCData[e.Npc.Name].Roles.Contains("candytaker"))
            {
                Utils.Speak(e.Npc, "not_candytaker", clear: false);
                return;
            }

            StardewValley.Object gift = e.Gift;
            Farmer gifter = Game1.player;
            NPC giftee = e.Npc;

            if ((bool)NPCData[giftee.Name].ReceivedGift)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("info.already_given"));
                return;
            }

            int score = 0;
            if (!gifter.modData.ContainsKey(ScoreKey))
                gifter.modData.Add(ScoreKey, "0");
            else score = int.Parse(gifter.modData[ScoreKey]);
            string response_key;
            bool play_trick = false;
            int gift_taste = e.Gift.Name == "TaT.mystery-treat" ? GetTreatTaste(giftee.Name, gift.modData[MysteryKey]) : GetTreatTaste(giftee.Name, gift.Name);
            switch (gift_taste)
            {
                case NPC.gift_taste_like:
                case NPC.gift_taste_love:
                    score += Config.LovedGiftVal;
                    gifter.changeFriendship(loved_pts, giftee);
                    response_key = "loved_treat";
                    break;
                case NPC.gift_taste_dislike:
                case NPC.gift_taste_hate:
                    score += Config.HatedGiftVal;
                    gifter.changeFriendship(hated_pts, giftee);
                    response_key = "hated_treat";
                    if (NPCData[giftee.Name].Roles.Contains("trickster"))
                        play_trick = true;
                    break;
                default:
                    score += Config.NeutralGiftVal;
                    gifter.changeFriendship(neutral_pts, giftee);
                    response_key = "neutral_treat";
                    break;
            }
            gifter.modData[ScoreKey] = score.ToString();
            NPCData[giftee.Name].ReceivedGift = true;
            gifter.reduceActiveItemByOne();
            gifter.currentLocation.localSound("give_gift");
            if (play_trick)
                Tricks.SmallTrick(giftee);
            else
                Utils.Speak(giftee, response_key, clear: false);
        }

        private static int GetTreatTaste(string npc, string item)
        {
            var my_data = NPCData[npc];

            if (my_data.HatedTreats.ToList().Contains(item))
                return NPC.gift_taste_hate;
            if (my_data.NeutralTreats.ToList().Contains(item))
                return NPC.gift_taste_neutral;
            if (my_data.LovedTreats.ToList().Contains(item))
                return NPC.gift_taste_love;

            if (TreatData[item].Universal is not null)
            {
                switch (TreatData[item].Universal.ToLower())
                {
                    case "hate":
                        return NPC.gift_taste_hate;
                    case "neutral":
                        return NPC.gift_taste_neutral;
                    case "love":
                        return NPC.gift_taste_love;
                }
            }

            foreach (string flavor in TreatData[item].Flavors.ToList())
            {
                if (my_data.HatedTreats.ToList().Contains(flavor))
                    return NPC.gift_taste_hate;
                if (my_data.NeutralTreats.ToList().Contains(flavor))
                    return NPC.gift_taste_neutral;
                if (my_data.LovedTreats.ToList().Contains(flavor))
                    return NPC.gift_taste_love;
            }

            return NPC.gift_taste_neutral;
        }
    }
}

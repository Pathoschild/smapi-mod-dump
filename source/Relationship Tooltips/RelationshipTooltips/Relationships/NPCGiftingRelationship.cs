/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/RelationshipTooltips
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookcase.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;

namespace RelationshipTooltips.Relationships
{
    public class NPCGiftingRelationship : NPCRelationship, IPerSaveSerializable
    {
        public enum GiftResponse
        {
            Unknown = -3, QuestItem = -2, Null = -1, Hate = NPC.gift_taste_hate, Dislike = NPC.gift_taste_dislike,
            Neutral = NPC.gift_taste_neutral, Like = NPC.gift_taste_like, Love = NPC.gift_taste_love
        }

        protected GiftSaveInfo GiftLog { get; private set; }
        public bool Display { get; private set; }
        public bool Record { get; private set; }
        public bool KnowsAll { get; private set; }

        public NPCGiftingRelationship(ModConfig config, IMonitor monitor) : base(config, monitor)
        {
            GiftLog = null;
            Display = config.displayGiftInfo;
            Record = config.recordGiftInfo;
            KnowsAll = config.playerKnowsAllGifts;
            Bookcase.Events.BookcaseEvents.NPCReceiveGiftPre.Add(OnNPCReceiveGift);
        }
        public void OnNPCReceiveGift(NPCReceiveGiftEvent args)
        {
            if(args.Giver == Game1.player)
            {
                GiftLog.AddGift(args.Target, args.Gift);
            }
        }
        public override int Priority => -10000;
        public override Func<Character, Item, bool> ConditionsMet => (c, i) => { return Display && i != null && !c.IsMonster && (c.GetType() == typeof(NPC)) && ((NPC)c).isVillager() && i.canBeGivenAsGift(); };
        #region Serialization
        private string GiftInfoPath
        {
            get
            {
                return $"{Constants.CurrentSavePath}{Path.PathSeparator}{Constants.SaveFolderName}_RelationshipTooltips_GiftInfo.json";
            }
        }
        Action<IModHelper> IPerSaveSerializable.SaveData => OnSave;
        private void OnSave(IModHelper helper)
        {
            if (Record)
            {
                helper.WriteJsonFile(GiftInfoPath, GiftLog);
                Monitor.Log($"Saved {GiftInfoPath}");
            }
            else
            {
                Monitor.Log("Session data not saved, recordedGiftInfo flag set to " + Record);
            }
        }
        Action<IModHelper> IPerSaveSerializable.LoadData => OnLoad;

        private void OnLoad(IModHelper helper)
        {
            if (Record)
            {
                GiftLog = helper.ReadJsonFile<GiftSaveInfo>(GiftInfoPath) ?? new GiftSaveInfo();
                Monitor.Log($"Loaded {GiftInfoPath}");
            }
            else
                GiftLog = new GiftSaveInfo();
        }
        #endregion

        #region Tooltip
        public override string GetHeaderText<T>(string currentHeader, T character, Item item = null)
        {
            return base.GetHeaderText(currentHeader, character, item);
        }
        public override string GetDisplayText<T>(string currentDisplay, T character, Item item = null)
        {
            NPC selectedNPC = character as NPC ?? throw new ArgumentNullException("character", "Cannot display information about gifts for null Character");
            string npcRelationship = base.GetDisplayText(currentDisplay, character, item);
            GiftResponse response = (GiftResponse)selectedNPC.getGiftTasteForThisItem(item);
            if (Game1.player.friendshipData.TryGetValue(selectedNPC.Name, out Friendship friendship))
            {
                if(IsItemQuestGift(selectedNPC, item))
                {
                    response = GiftResponse.QuestItem;
                }
                else if (!KnowsAll)
                {
                    if (!Record)
                    {
                        return npcRelationship;//This is an ambiguous case, at this point we don't know what the user wants and just return the base implementation
                    }
                    else
                    {
                        if ((friendship.Points / NPC.friendshipPointsPerHeartLevel) < Config.heartLevelToKnowAllGifts)
                        {
                            if (!GiftLog.PlayerHasGifted(selectedNPC.Name, item.Name))
                            {
                                response = GiftResponse.Unknown;
                            }
                        }
                    }
                }
                npcRelationship += Environment.NewLine;
                npcRelationship = GetGiftResponse(npcRelationship, response);
                npcRelationship += AddGiftMiscInfo(friendship);
                return npcRelationship;
            }
            return npcRelationship;
        }
        private string AddGiftMiscInfo(Friendship friendship)
        {
            if (friendship.GiftsThisWeek >= NPC.maxGiftsPerWeek)
                return Environment.NewLine + Config.maxGiftsGivenThisWeek;
            if (friendship.GiftsToday > 0)
                return Environment.NewLine + Config.givenGiftAlreadyToday;
            if (friendship.GiftsThisWeek == 1)
                return Environment.NewLine + Config.singleGiftLeftThisWeek;
            return "";
        }
        /// <summary>
        /// Is the gift being given part of an item delivery quest?
        /// </summary>
        /// <param name="target">NPC to be gifted</param>
        /// <param name="heldItem">Item currently held (must be a gift)</param>
        /// <returns>If the item is part of a quest delivery - true, else false</returns>
        public bool IsItemQuestGift(NPC target, Item heldItem)
        {
            if (target == null || heldItem == null)
                return false;
            foreach (Quest q in Game1.player.questLog)
            {
                if (q.questType == Quest.type_itemDelivery)
                {
                    ItemDeliveryQuest quest = q as ItemDeliveryQuest;
                    if (quest == null)
                        break;
                    if (quest.target?.Value == target.Name && quest.item.Value == heldItem.ParentSheetIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private string GetGiftResponse(string npcRelationship, GiftResponse response)
        {
            npcRelationship += Config.gift + ": "; ;
            switch (response)
            {
                case GiftResponse.Love:
                    {
                        return npcRelationship + Config.giftLoves;
                    }
                case GiftResponse.Like:
                    {
                        return npcRelationship + Config.giftLikes;
                    }
                case GiftResponse.Neutral:
                    {
                        return npcRelationship + Config.giftNeutral;
                    }
                case GiftResponse.Dislike:
                    {
                        return npcRelationship + Config.giftDislikes;
                    }
                case GiftResponse.Hate:
                    {
                        return npcRelationship + Config.giftHates;
                    }
                case GiftResponse.QuestItem:
                    {
                        return npcRelationship + Config.giftQuestItem;
                    }
                case GiftResponse.Unknown:
                    {
                        return npcRelationship + Config.giftUnknown;
                    }
                case GiftResponse.Null:
                    {
                        return npcRelationship + "null";
                    }
                default:
                    {
                        return npcRelationship + $"Unmapped Gift Response '{response.ToString()}'(Report this, likely a version mismatch)";
                    }
            }
        }
        #endregion
    }
}

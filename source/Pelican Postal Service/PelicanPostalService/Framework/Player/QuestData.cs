using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;

namespace PelicanPostalService.Framework.Player
{
    class QuestData
    {
        public bool ClintEmilyQuestReady { get; private set; }
        public Quest CurrentQuest { get; private set; }
        public int FriendshipPoints { get; private set; }
        public static IMonitor Monitor;
        private int goldReward;
      
        public QuestData(Object item, NPC who)
        {
            CurrentQuest = FindOneByCriteria(item, who);
        }

        public Quest FindOneByCriteria(Object item, NPC who)
        {
            foreach (Quest quest in Game1.player.questLog)
            {
                bool deliveryQuest = quest.questType.Value == Quest.type_itemDelivery;
                bool isComplete = quest.completed.Value;
                bool fishingQuest = quest.questType.Value == Quest.type_fishing;
                bool lostItemQuest = quest.questType.Value == Quest.type_harvest;
                bool resourceCollectionQuest = quest.questType.Value == Quest.type_resource;

                if (!isComplete && (deliveryQuest || fishingQuest || lostItemQuest || resourceCollectionQuest))
                {
                    Monitor.Log("Scanning criteria for quest " + quest.id.Value.ToString() + " " + quest.questTitle);

                    if (deliveryQuest)
                    {
                        Quest matchingQuest = ValidateDeliveryQuest(item, who, quest);
                        if (matchingQuest != null)
                        {
                            CurrentQuest = matchingQuest;
                            return CurrentQuest;
                        }
                    }

                    //if (resourceCollectionQuest)
                    //{
                    //    Quest matchingQuest = ValidateResourceCollectionQuest(item, who, quest);
                    //    if (matchingQuest != null)
                    //    {
                    //        CurrentQuest = matchingQuest;
                    //        return CurrentQuest;
                    //    }
                    //}

                    if (lostItemQuest)
                    {
                        Quest matchingQuest = ValidateLostItemQuest(item, who, quest);
                        if (matchingQuest != null)
                        {
                            CurrentQuest = matchingQuest;
                            return CurrentQuest;
                        }
                    }
                }
            }

            return null;
        }

        public void Update()
        {
            if (goldReward > 0)
            {
                CurrentQuest.completed.Value = true;
                CurrentQuest.moneyReward.Value = goldReward;
            }
            
            CurrentQuest.questComplete();
            Game1.addHUDMessage(new HUDMessage("Journal Updated", 2));
            ++Game1.stats.QuestsCompleted;

            // Game data is read-only unless accessed directly
        }

        private Quest ValidateDeliveryQuest(Object item, NPC who, Quest quest)
        {
            ItemDeliveryQuest deliveryQuest = (ItemDeliveryQuest) quest;
            bool dailyQuest = quest.dailyQuest.Value;
            bool clintEmilyQuest = quest.id.Value == 110;
            bool questItemReady = deliveryQuest.item.Value == item.ParentSheetIndex;

            // For "Clint's Attempt", the quest giver is not the recipient
            // Workaround required to update Clint's affection instead of Emily
            if (clintEmilyQuest && questItemReady)
            {
                ClintEmilyQuestReady = true;
                FriendshipPoints = 250;
                return quest;
            }
            else if (deliveryQuest.target.Value.Equals(who.displayName) && questItemReady)
            {
                if (dailyQuest)
                {
                    goldReward = item.Price * 3;
                    FriendshipPoints = 150;
                    return quest;
                }
                else
                {
                    goldReward = deliveryQuest.moneyReward.Value;
                    FriendshipPoints = 250;
                    return quest;
                }
            }

            return null;
        }

        private Quest ValidateLostItemQuest(Object item, NPC who, Quest quest)
        {
            // Robin's Lost Axe
            if (quest.id.Value == 100 && who.displayName.Equals("Robin") && item.DisplayName.Equals("Lost Axe"))
            {
                goldReward = 250;
                FriendshipPoints = 250;
                return quest;
            }

            // Mayor's "Shorts"
            if (quest.id.Value == 102 && who.displayName.Equals("Lewis") && item.DisplayName.Equals("Lucky Purple Shorts"))
            {
                goldReward = 750;
                FriendshipPoints = 250;
                return quest;
            }

            // Blackberry Basket
            if (quest.id.Value == 107 && who.displayName.Equals("Linus") && item.DisplayName.Equals("Berry Basket"))
            {
                FriendshipPoints = 250;
                return quest;
            }

            return null;
        }

        //private Quest ValidateResourceCollectionQuest(Object item, NPC who, Quest quest)
        //{
        //    ResourceCollectionQuest collectionQuest = (ResourceCollectionQuest)quest;
        //    return collectionQuest.checkIfComplete(who, -1, -1, item) ? quest : null;
        //}
    }
}
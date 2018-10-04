using Project.Framework.Player.Friendship;
using Project.Framework.Player.Items;
using Project.Logging;
using StardewValley;
using StardewValley.Quests;

namespace Project.Framework.Player.Quests
{
    public class QuestHandler : Debug
    {
        public bool PreventNextGift { get; private set; }
        private readonly bool lazyItemChecking;
        private bool onSearchSuccess;

        public QuestHandler(bool strictInspection)
        {
            lazyItemChecking = strictInspection;
        }

        public bool FindOneAndUpdate(FriendshipHandler friendshipInfo, ItemHandler itemInfo)
        {
            foreach (Quest quest in Game1.player.questLog)
            {
                if (!quest.completed.Value)
                {
                    bool isValidType = ParseOneByTypeFlag(quest);
                    if (isValidType)
                    {
                        switch (quest.questType.Value)
                        {
                            case 3:
                                UpdateDeliveryQuest(friendshipInfo, itemInfo, quest);
                                break;
                            case 7:
                                UpdateFishingQuest(friendshipInfo, itemInfo, quest);
                                break;
                            case 9:
                                UpdateLostItemQuest(friendshipInfo, itemInfo, quest);
                                break;
                            case 10:
                                UpdateCollectionQuest(friendshipInfo, itemInfo, quest);
                                break;
                        }

                        if (onSearchSuccess)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void AlertInsufficientAmount(string recipient, int denominator)
        {
            string s = string.Format("Not enough material for {0} ({1} needed)", recipient, denominator);
            Game1.addHUDMessage(new HUDMessage(s, 3));
        }

        private bool ParseOneByObjective(int activeObjectId, int requestObjectId)
        {
            return activeObjectId == requestObjectId ? true : false;
        }

        private bool ParseOneByRecipient(string menuSelection, string questGiver)
        {
            return menuSelection.Equals(questGiver) ? true : false;
        }

        private bool ParseOneByTypeFlag(Quest quest)
        {
            bool collectionQuest = quest.questType.Value == Quest.type_resource;
            bool deliveryQuest = quest.questType.Value == Quest.type_itemDelivery;
            bool fishingQuest = quest.questType.Value == Quest.type_fishing;
            bool lostItemQuest = quest.questType.Value == Quest.type_harvest;

            return deliveryQuest || fishingQuest || lostItemQuest || collectionQuest ? true : false;
        }
        
        private void Update(Quest quest, int gold)
        {
            if (gold > 0)
            {
                quest.completed.Value = true;
                quest.moneyReward.Value = gold;
            }

            quest.questComplete();
            ++Game1.stats.QuestsCompleted;
            Game1.addHUDMessage(new HUDMessage("Journal Updated", 2));

            onSearchSuccess = true;
        }

        private void UpdateCollectionQuest(FriendshipHandler friendshipInfo, ItemHandler itemInfo, Quest quest)
        {
            ResourceCollectionQuest request = (ResourceCollectionQuest) quest;
            bool isValidObjective = ParseOneByObjective(itemInfo.Item.ParentSheetIndex, request.deliveryItem.Value.ParentSheetIndex);
            bool isValidRecipient = ParseOneByRecipient(friendshipInfo.Who.displayName, request.target.Value);
            
            if (isValidObjective && isValidRecipient)
            {
                int numberItemsOwed = lazyItemChecking ? request.number.Value : request.number.Value - request.numberCollected.Value;
                bool isValidAmount = Game1.player.hasItemInInventory(itemInfo.Item.ParentSheetIndex, numberItemsOwed);

                if (isValidAmount)
                {
                    itemInfo.DeductItems = request.target.Value.Equals("Robin") ? request.number.Value : 0;
                    friendshipInfo.Update(0, true, null);
                    Update(quest, request.reward.Value);
                }
                else
                {
                    itemInfo.DeductItems = 0;
                    PreventNextGift = true;
                    AlertInsufficientAmount(request.target.Value.ToString(), numberItemsOwed);
                }
            }
        }

        private void UpdateDeliveryQuest(FriendshipHandler friendshipInfo, ItemHandler itemInfo, Quest quest)
        {
            ItemDeliveryQuest request = (ItemDeliveryQuest) quest;
            bool isValidObjective = ParseOneByObjective(itemInfo.Item.ParentSheetIndex, request.item.Value);
            bool isValidRecipient = ParseOneByRecipient(friendshipInfo.Who.displayName, request.target.Value);
            
            // Clint's Attempt
            if (request.id.Value == 110 && isValidObjective)
            {
                friendshipInfo.Update(250, true, "Clint");
                Update(quest, 0);
            }
            else if (isValidRecipient && isValidObjective) 
            {
                // Robin's Request
                if (request.id.Value == 113)
                {
                    bool isValidAmount = Game1.player.hasItemInInventory(itemInfo.Item.ParentSheetIndex, 10);
                    if (isValidAmount)
                    {
                        itemInfo.DeductItems = 10;
                        friendshipInfo.Update(250, true, null);
                        Update(quest, request.moneyReward.Value);
                    }
                    else
                    {
                        itemInfo.DeductItems = 0;
                        PreventNextGift = true;
                        AlertInsufficientAmount(request.target.Value, 10);
                    }
                }
                else if (quest.dailyQuest.Value)
                {
                    friendshipInfo.Update(150, true, null);
                    Update(quest, itemInfo.Item.Price * 3);    
                }
                else
                {
                    friendshipInfo.Update(250, true, null);
                    Update(quest, request.moneyReward.Value);
                }
            }
        }

        private void UpdateFishingQuest(FriendshipHandler friendshipInfo, ItemHandler itemInfo, Quest quest)
        {
            FishingQuest request = (FishingQuest) quest;
            bool isValidObjective = ParseOneByObjective(itemInfo.Item.ParentSheetIndex, request.whichFish.Value);
            bool isValidRecipient = ParseOneByRecipient(friendshipInfo.Who.displayName, request.target.Value);

            if (isValidObjective && isValidRecipient)
            {
                int numberItemsOwed = lazyItemChecking ? request.numberToFish.Value : request.numberToFish.Value - request.numberFished.Value;
                bool isValidAmount = Game1.player.hasItemInInventory(itemInfo.Item.ParentSheetIndex, numberItemsOwed);

                if (isValidAmount)
                {
                    itemInfo.DeductItems = 0;
                    friendshipInfo.Update(0, true, null);
                    Update(quest, request.reward.Value);
                }
                else
                {
                    itemInfo.DeductItems = 0;
                    PreventNextGift = true;
                    AlertInsufficientAmount(request.target.Value.ToString(), numberItemsOwed);
                }
            }
        }

        private void UpdateLostItemQuest(FriendshipHandler friendshipInfo, ItemHandler itemInfo, Quest quest)
        {
            if (quest.id.Value == 100 && friendshipInfo.Who.displayName.Equals("Robin") && itemInfo.Item.DisplayName.Equals("Lost Axe"))
            {
                // Robin's Lost Axe
                friendshipInfo.Update(250, true, null);
                Update(quest, 250);
            }
            else if (quest.id.Value == 102 && friendshipInfo.Who.displayName.Equals("Lewis") && itemInfo.Item.DisplayName.Equals("Lucky Purple Shorts"))
            {
                // Mayor's "Shorts"
                friendshipInfo.Update(250, true, null);
                Update(quest, 750);
            }
            else if (quest.id.Value == 107 && friendshipInfo.Who.displayName.Equals("Linus") && itemInfo.Item.DisplayName.Equals("Berry Basket"))
            {
                // Blackberry Basket
                friendshipInfo.Update(250, true, null);
                Update(quest, 0);
            }
        }
    }
}
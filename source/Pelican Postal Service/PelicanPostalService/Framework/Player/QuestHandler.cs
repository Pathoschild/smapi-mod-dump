using Project.Framework.Player.Friendship;
using Project.Framework.Player.Items;
using Project.Logging;
using StardewValley;
using StardewValley.Quests;

namespace Project.Framework.Player.Quests
{
    public class QuestHandler : Debug
    {
        public bool OnSearchSuccess { get; private set; }
        public bool PreventNextGift { get; private set; }
        private readonly bool lazyItemChecking;

        public QuestHandler(bool lazyItemChecking)
        {
            this.lazyItemChecking = lazyItemChecking;
        }

        public void FindOneAndUpdate(FriendshipHandler friendshipDetails, ItemHandler itemDetails)
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
                                UpdateDeliveryQuest(friendshipDetails, itemDetails, quest);
                                break;
                            case 7:
                                UpdateFishingQuest(friendshipDetails, itemDetails, quest);
                                break;
                            case 9:
                                UpdateLostItemQuest(friendshipDetails, itemDetails, quest);
                                break;
                            case 10:
                                UpdateCollectionQuest(friendshipDetails, itemDetails, quest);
                                break;
                        }
                    }
                }
            }
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
            PreventNextGift = true;

            if (gold > 0)
            {
                quest.completed.Value = true;
                quest.moneyReward.Value = gold;
            }

            quest.questComplete();
            ++Game1.stats.QuestsCompleted;
            Game1.addHUDMessage(new HUDMessage("Journal Updated", 2));
        }

        private void UpdateCollectionQuest(FriendshipHandler friendshipDetails, ItemHandler itemDetails, Quest quest)
        {
            ResourceCollectionQuest request = (ResourceCollectionQuest) quest;
            bool isValidObjective = ParseOneByObjective(itemDetails.Item.ParentSheetIndex, request.deliveryItem.Value.ParentSheetIndex);
            bool isValidRecipient = ParseOneByRecipient(friendshipDetails.Who.displayName, request.target.Value);

            if (isValidObjective && isValidRecipient)
            {
                OnSearchSuccess = true;
                int numberItemsOwed = lazyItemChecking ? request.number.Value : request.number.Value - request.numberCollected.Value;
                bool isValidAmount = Game1.player.hasItemInInventory(itemDetails.Item.ParentSheetIndex, numberItemsOwed);

                if (isValidAmount)
                {
                    // Remove only when quest giver is Robin
                    if (request.target.Value.Equals("Robin"))
                    {
                        itemDetails.RemoveFromInventory(request.number.Value);
                    }

                    friendshipDetails.Update(0, true, null);
                    Update(quest, request.reward.Value);
                }
                else
                {
                    // Remove none and warn
                    WarnAmountLow(request.target.Value.ToString(), numberItemsOwed);
                }
            }
        }

        private void UpdateDeliveryQuest(FriendshipHandler friendshipDetails, ItemHandler itemDetails, Quest quest)
        {
            ItemDeliveryQuest request = (ItemDeliveryQuest)quest;
            bool isValidObjective = ParseOneByObjective(itemDetails.Item.ParentSheetIndex, request.item.Value);
            bool isValidRecipient = ParseOneByRecipient(friendshipDetails.Who.displayName, request.target.Value);

            // Clint's Attempt
            if (request.id.Value == 110 && isValidObjective && friendshipDetails.Who.displayName.Equals("Emily"))
            {
                OnSearchSuccess = true;
                itemDetails.RemoveFromInventory(1);
                friendshipDetails.Update(250, true, "Clint");
                Update(quest, 0);
            }
            else if (isValidRecipient && isValidObjective)
            {
                OnSearchSuccess = true;

                // Robin's Request
                if (request.id.Value == 113)
                {
                    bool isValidAmount = Game1.player.hasItemInInventory(itemDetails.Item.ParentSheetIndex, 10);
                    if (isValidAmount)
                    {
                        itemDetails.RemoveFromInventory(10);
                        friendshipDetails.Update(250, true, null);
                        Update(quest, request.moneyReward.Value);
                    }
                    else
                    {
                        // Remove none and warn
                        WarnAmountLow(request.target.Value, 10);
                    }
                }
                else if (quest.dailyQuest.Value)
                {
                    itemDetails.RemoveFromInventory(1);
                    friendshipDetails.Update(150, true, null);
                    Update(quest, itemDetails.Item.Price * 3);
                }
                else
                {
                    itemDetails.RemoveFromInventory(1);
                    friendshipDetails.Update(250, true, null);
                    Update(quest, request.moneyReward.Value);
                }
            }
        }

        private void UpdateFishingQuest(FriendshipHandler friendshipDetails, ItemHandler itemDetails, Quest quest)
        {
            FishingQuest request = (FishingQuest)quest;
            bool isValidObjective = ParseOneByObjective(itemDetails.Item.ParentSheetIndex, request.whichFish.Value);
            bool isValidRecipient = ParseOneByRecipient(friendshipDetails.Who.displayName, request.target.Value);

            if (isValidObjective && isValidRecipient)
            {
                OnSearchSuccess = true;

                int numberItemsOwed = lazyItemChecking ? request.numberToFish.Value : request.numberToFish.Value - request.numberFished.Value;
                bool isValidAmount = Game1.player.hasItemInInventory(itemDetails.Item.ParentSheetIndex, numberItemsOwed);

                if (isValidAmount)
                {
                    // Remove none
                    friendshipDetails.Update(0, true, null);
                    Update(quest, request.reward.Value);
                }
                else
                {
                    // Remove none and warn
                    WarnAmountLow(request.target.Value.ToString(), numberItemsOwed);
                }
            }
        }

        private void UpdateLostItemQuest(FriendshipHandler friendshipDetails, ItemHandler itemDetails, Quest quest)
        {
            if (quest.id.Value == 100 && friendshipDetails.Who.displayName.Equals("Robin") && itemDetails.Item.DisplayName.Equals("Lost Axe"))
            {
                OnSearchSuccess = true;

                // Robin's Lost Axe
                itemDetails.RemoveFromInventory(1);
                friendshipDetails.Update(250, true, null);
                Update(quest, 250);
            }
            else if (quest.id.Value == 102 && friendshipDetails.Who.displayName.Equals("Lewis") && itemDetails.Item.DisplayName.Equals("Lucky Purple Shorts"))
            {
                OnSearchSuccess = true;

                // Mayor's "Shorts"
                itemDetails.RemoveFromInventory(1);
                friendshipDetails.Update(250, true, null);
                Update(quest, 750);
            }
            else if (quest.id.Value == 107 && friendshipDetails.Who.displayName.Equals("Linus") && itemDetails.Item.DisplayName.Equals("Berry Basket"))
            {
                OnSearchSuccess = true;

                // Blackberry Basket
                itemDetails.RemoveFromInventory(1);
                friendshipDetails.Update(250, true, null);
                Update(quest, 0);
            }
        }

        private void WarnAmountLow(string recipient, int denominator)
        {
            PreventNextGift = true;
            string s = string.Format("Not enough material for {0} ({1} needed)", recipient, denominator);
            Game1.addHUDMessage(new HUDMessage(s, 3));
        }
    }
}
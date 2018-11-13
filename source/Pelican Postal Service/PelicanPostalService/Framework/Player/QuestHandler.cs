using Pelican.Friendship;
using Pelican.Items;
using Pelican.Menus;
using StardewValley;
using StardewValley.Quests;

namespace Pelican.Quests
{
    public class QuestHandler : Meta
    {
        private PostalService menuHandler;

        public QuestHandler(PostalService menuHandler)
        {
            this.menuHandler = menuHandler;
        }

        public void FindOneAndUpdate(NpcHandler npcHandler, ItemHandler itemHandler)
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
                                UpdateDeliveryQuest(npcHandler, itemHandler, quest);
                                break;
                            case 7:
                                UpdateFishingQuest(npcHandler, itemHandler, quest);
                                break;
                            case 9:
                                UpdateLostItemQuest(npcHandler, itemHandler, quest);
                                break;
                            case 10:
                                UpdateCollectionQuest(npcHandler, itemHandler, quest);
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
            menuHandler.UseDefaultAction = false;

            if (gold > 0)
            {
                quest.completed.Value = true;
                quest.moneyReward.Value = gold;
            }

            quest.questComplete();
            ++Game1.stats.QuestsCompleted;

            string i18n = Lang.Get("questComplete");
            Game1.addHUDMessage(new HUDMessage(i18n, 2));
        }

        private void UpdateCollectionQuest(NpcHandler npcHandler, ItemHandler itemHandler, Quest quest)
        {
            ResourceCollectionQuest request = (ResourceCollectionQuest)quest;
            bool isValidObjective = ParseOneByObjective(itemHandler.Item.ParentSheetIndex, request.deliveryItem.Value.ParentSheetIndex);
            bool isValidRecipient = ParseOneByRecipient(npcHandler.Target.Name, request.target.Value);

            if (isValidObjective && isValidRecipient)
            {
                int numberItemsOwed = Config.LazyItemChecking ? request.number.Value : request.number.Value - request.numberCollected.Value;
                bool isValidAmount = Game1.player.hasItemInInventory(itemHandler.Item.ParentSheetIndex, numberItemsOwed);

                if (isValidAmount)
                {
                    // Remove only when quest giver is Robin
                    if (request.target.Value.Equals("Robin"))
                    {
                        itemHandler.RemoveFromInventory(request.number.Value);
                    }

                    npcHandler.Update(0, true, null);
                    Update(quest, request.reward.Value);
                }
                else
                {
                    // Remove none and warn
                    WarnAmountLow(request.target.Value.ToString(), numberItemsOwed);
                }
            }
        }

        private void UpdateDeliveryQuest(NpcHandler npcHandler, ItemHandler itemHandler, Quest quest)
        {
            ItemDeliveryQuest request = (ItemDeliveryQuest)quest;
            bool isValidObjective = ParseOneByObjective(itemHandler.Item.ParentSheetIndex, request.item.Value);
            bool isValidRecipient = ParseOneByRecipient(npcHandler.Target.Name, request.target.Value);

            // Clint's Attempt
            if (request.id.Value == 110 && isValidObjective && npcHandler.Target.Name.Equals("Emily"))
            {
                itemHandler.RemoveFromInventory(1);
                npcHandler.Update(250, true, "Clint");
                Update(quest, 0);
            }
            else if (isValidRecipient && isValidObjective)
            {
                // Robin's Request
                if (request.id.Value == 113)
                {
                    bool isValidAmount = Game1.player.hasItemInInventory(itemHandler.Item.ParentSheetIndex, 10);
                    if (isValidAmount)
                    {
                        itemHandler.RemoveFromInventory(10);
                        npcHandler.Update(250, true, null);
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
                    itemHandler.RemoveFromInventory(1);
                    npcHandler.Update(150, true, null);
                    Update(quest, itemHandler.Item.Price * 3);
                }
                else
                {
                    itemHandler.RemoveFromInventory(1);
                    npcHandler.Update(250, true, null);
                    Update(quest, request.moneyReward.Value);
                }
            }
        }

        private void UpdateFishingQuest(NpcHandler npcHandler, ItemHandler itemHandler, Quest quest)
        {
            FishingQuest request = (FishingQuest)quest;
            bool isValidObjective = ParseOneByObjective(itemHandler.Item.ParentSheetIndex, request.whichFish.Value);
            bool isValidRecipient = ParseOneByRecipient(npcHandler.Target.Name, request.target.Value);

            if (isValidObjective && isValidRecipient)
            {
                int numberItemsOwed = Config.LazyItemChecking ? request.numberToFish.Value : request.numberToFish.Value - request.numberFished.Value;
                bool isValidAmount = Game1.player.hasItemInInventory(itemHandler.Item.ParentSheetIndex, numberItemsOwed);

                if (isValidAmount)
                {
                    // Remove none
                    npcHandler.Update(0, true, null);
                    Update(quest, request.reward.Value);
                }
                else
                {
                    // Remove none and warn
                    WarnAmountLow(request.target.Value.ToString(), numberItemsOwed);
                }
            }
        }

        private void UpdateLostItemQuest(NpcHandler npcHandler, ItemHandler itemHandler, Quest quest)
        {
            if (quest.id.Value == 100 && npcHandler.Target.Name.Equals("Robin") && itemHandler.Item.Name.Equals("Lost Axe"))
            {
                // Robin's Lost Axe
                itemHandler.RemoveFromInventory(1);
                npcHandler.Update(250, true, null);
                Update(quest, 250);
            }
            else if (quest.id.Value == 102 && npcHandler.Target.Name.Equals("Lewis") && itemHandler.Item.Name.Equals("Lucky Purple Shorts"))
            {
                // Mayor's "Shorts"
                itemHandler.RemoveFromInventory(1);
                npcHandler.Update(250, true, null);
                Update(quest, 750);
            }
            else if (quest.id.Value == 107 && npcHandler.Target.Name.Equals("Linus") && itemHandler.Item.Name.Equals("Berry Basket"))
            {
                // Blackberry Basket
                itemHandler.RemoveFromInventory(1);
                npcHandler.Update(250, true, null);
                Update(quest, 0);
            }
        }

        private void WarnAmountLow(string target, int amount)
        {
            menuHandler.UseDefaultAction = false;
            string i18n = Lang.Get("questAmountLow", new { name = target, amt = amount });
            Game1.addHUDMessage(new HUDMessage(i18n, 3));
        }
    }
}
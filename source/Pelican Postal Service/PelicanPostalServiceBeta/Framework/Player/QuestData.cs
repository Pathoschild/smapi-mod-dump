using StardewValley;
using StardewValley.Quests;

namespace PelicanPostalService.Framework.Player
{
    class QuestData
    {
        public Quest Quest { get; private set; }
        public int Reward { get; private set; }
        public int Points { get; private set; }

        public QuestData(Object item, NPC who)
        {
            Quest = FindOneByCriteria(item, who);
        }

        public Quest FindOneByCriteria(Object item, NPC who)
        {
            foreach (Quest quest in Game1.player.questLog)
            {
                if (!quest.completed.Value)
                {
                    bool typeItemDelivery = quest.questType.Value == Quest.type_itemDelivery;
                    bool typeHarvest = quest.questType.Value == Quest.type_harvest;

                    if (typeHarvest)
                    {
                        // Robin's Lost Axe
                        if (quest.id.Value == 100 && who.displayName.Equals("Robin") && item.DisplayName.Equals("Lost Axe"))
                        {
                            Reward = 250;
                            Points = 250;
                            return quest;
                        }
                        // Mayor's "Shorts"
                        else if (quest.id.Value == 102 && who.displayName.Equals("Lewis") && item.DisplayName.Equals("Lucky Purple Shorts"))
                        {
                            Reward = 750;
                            Points = 250;
                            return quest;
                        }
                        // Blackberry Basket
                        else if (quest.id.Value == 107 && who.displayName.Equals("Linus") && item.DisplayName.Equals("Berry Basket"))
                        {
                            Points = 250;
                            return quest;
                        }
                    }
                    else if (typeItemDelivery)
                    {
                        ItemDeliveryQuest deliveryQuest = (ItemDeliveryQuest)quest;
                        if (deliveryQuest.target.Value.Equals(who.displayName) && deliveryQuest.item.Value == item.ParentSheetIndex)
                        {
                            if (quest.dailyQuest.Value)
                            {
                                Reward = item.Price * 3;
                                Points = 150;
                                return quest;
                            }
                            else
                            {
                                Reward = deliveryQuest.moneyReward.Value;
                                Points = 250;
                                return quest;
                            }

                        }
                    }
                }
            }
            return null;
        }

        public void ResolveOne(Object item)
        {
            if (Quest == null)
            {
                throw new System.Exception("[QuestData.cs] Unable to resolve quest: The class variable \"quest\" contains the wrong type");
            }

            Quest.completed.Value = true;
            Quest.reloadDescription();
            Quest.moneyReward.Value = Reward;
            Quest.questComplete();

            // [!] Game data is read-only unless accessed directly
            ++Game1.stats.QuestsCompleted;
            Game1.addHUDMessage(new HUDMessage("Journal Updated", 2));
        }
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValleyTodo.Helpers;
using StardewValleyTodo.Tracker;

namespace StardewValleyTodo.Controllers {
    public class QuestLogController {
        private readonly InventoryTracker _inventoryTracker;

        public QuestLogController(InventoryTracker inventoryTracker) {
            _inventoryTracker = inventoryTracker;
        }

        public void ProcessInput(QuestLog menu) {
            var questPage = Reflect.GetPrivate<int>(menu, "questPage");

            if (questPage == -1) {
                // Quest is not selected
                return;
            }

            var quest = Reflect.GetPrivate<IQuest>(menu, "_shownQuest");
            TrackableQuest trackableQuest;

            switch (quest) {
                case FishingQuest fishingQuest:
                    trackableQuest = TrackFishingQuest(fishingQuest);
                    break;
                case ItemDeliveryQuest itemDeliveryQuest:
                    trackableQuest = TrackItemDeliveryQuest(itemDeliveryQuest);
                    break;
                case ItemHarvestQuest itemHarvestQuest:
                    trackableQuest = TrackItemHarvestQuest(itemHarvestQuest);
                    break;
                case LostItemQuest lostItemQuest:
                    trackableQuest = TrackLostItemQuest(lostItemQuest);
                    break;
                case ResourceCollectionQuest resourceCollectionQuest:
                    trackableQuest = TrackResourceCollectionQuest(resourceCollectionQuest);
                    break;
                case SlayMonsterQuest slayMonsterQuest:
                    trackableQuest = TrackSlayMonsterQuest(slayMonsterQuest);
                    break;
                case SpecialOrder specialOrder:
                    trackableQuest = TrackSpecialOrder(specialOrder);
                    break;
                case Quest basicQuest:
                    trackableQuest = TrackBasicQuest(basicQuest);
                    break;

                default:
                    throw new NotImplementedException($"Unsupported quest type: {quest.GetType().Name}");
            }

            if (trackableQuest != null) {
                _inventoryTracker.Toggle(trackableQuest);
            }
        }

        private TrackableQuest TrackSpecialOrder(SpecialOrder specialOrder) {
            var name = $"{specialOrder.requester.Value}: Special Order";

            var items = new List<TrackableItemBase>();
            foreach (var objective in specialOrder.objectives) {
                if (!objective.ShouldShowProgress()) {
                    continue;
                }

                var item = new TrackableDynamicItem(objective.GetDescription(), () => objective.GetMaxCount(),
                    () => objective.GetCount());
                items.Add(item);
            }

            return new TrackableQuest(name, items, specialOrder);
        }

        private TrackableItemBase CreateCountableItem(string key, int amount) {
            if (key.Contains("-")) {
                throw new NotImplementedException("Can not track categories");
            }

            var info = Game1.objectData[key];
            var itemName = LocalizedStringLoader.Load(info.DisplayName);

            return new CountableItem(key, itemName, amount);
        }

        private string FormatQuestName(string title, string npcName) {
            return $"{npcName}: {title}";
        }

        private TrackableQuest TrackItemDeliveryQuest(ItemDeliveryQuest quest) {
            var name = FormatQuestName(quest.questTitle, quest.target.Value);
            var key = ObjectKey.Parse(quest.ItemId.Value);
            var questItem = CreateCountableItem(key, quest.number.Value);

            return new TrackableQuest(name, questItem, quest);
        }

        private TrackableQuest TrackFishingQuest(FishingQuest quest) {
            var name = FormatQuestName(quest.questTitle, quest.target.Value);
            var key = ObjectKey.Parse(quest.ItemId.Value);
            var questItem = CreateCountableItem(key, quest.numberToFish.Value);

            return new TrackableQuest(name, questItem, quest);
        }

        private TrackableQuest TrackItemHarvestQuest(ItemHarvestQuest quest) {
            var name = quest.questTitle;
            var key = ObjectKey.Parse(quest.ItemId.Value);
            var questItem = CreateCountableItem(key, quest.Number.Value);

            return new TrackableQuest(name, questItem, quest);
        }

        private TrackableQuest TrackLostItemQuest(LostItemQuest quest) {
            var name = FormatQuestName(quest.questTitle, quest.npcName.Value);
            var key = ObjectKey.Parse(quest.ItemId.Value);
            var questItem = CreateCountableItem(key, 1);

            return new TrackableQuest(name, questItem, quest);
        }

        private TrackableQuest TrackResourceCollectionQuest(ResourceCollectionQuest quest) {
            var name = FormatQuestName(quest.questTitle, quest.target.Value);
            var key = ObjectKey.Parse(quest.ItemId.Value);
            var questItem = CreateCountableItem(key, quest.number.Value);

            return new TrackableQuest(name, questItem, quest);
        }

        private TrackableQuest TrackSlayMonsterQuest(SlayMonsterQuest quest) {
            var name = FormatQuestName(quest.questTitle, quest.target.Value);
            var questItem = new TrackableDynamicItem(quest.monsterName.Value,
                () => quest.numberToKill.Value, () => quest.numberKilled.Value);
            return new TrackableQuest(name, questItem, quest);
        }

        private TrackableQuest TrackBasicQuest(Quest quest) {
            var name = quest.questTitle;
            var questItem =
                new TrackableDynamicItem(quest.currentObjective, () => 1, () => quest.completed.Value ? 1 : 0, false);
            return new TrackableQuest(name, questItem, quest);
        }
    }
}

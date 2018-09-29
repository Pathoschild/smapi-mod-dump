using PelicanPostalService.Framework.Player;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace PelicanPostalService.Framework.Menu
{
    public class PostalService
    {
        private readonly ActiveItem activeItem;
        private readonly bool allowQuestSubmissions;
        private QuestData questData;

        public PostalService(ActiveItem item, bool questSubmissions)
        {
            allowQuestSubmissions = questSubmissions;
            activeItem = item;
        }

        public void Open(IClickableMenu currentMenu, Tool currentTool)
        {
            if (ValidateActiveItem() && currentMenu == null && currentTool == null)
            {
                List<string> options = FriendshipData.Find();
                Game1.activeClickableMenu = options != null ? new ChooseFromListMenu(options, OnSelectOption, false) : null;
            }
        }

        private bool FindQuestByCriteria(FriendshipData friendshipData)
        {
            questData = new QuestData(activeItem.Data, friendshipData.Who);
            return questData.CurrentQuest != null ? true : false;
        }

        private void OnSelectOption(string name)
        {
            FriendshipData friendshipData = new FriendshipData(name);

            if (friendshipData.Who != null)
            {
                bool isStrictQuestItem = activeItem.Data.DisplayName.Equals("Lost Axe") || activeItem.Data.DisplayName.Equals("Lucky Purple Shorts") || activeItem.Data.DisplayName.Equals("Berry Basket");
                if (isStrictQuestItem && !allowQuestSubmissions)
                {
                    return;
                }
                
                bool foundMatchingQuest = FindQuestByCriteria(friendshipData);
                if (foundMatchingQuest && allowQuestSubmissions)
                {
                    if (questData.ClintEmilyQuestReady)
                    {
                        UpdateAndExit(friendshipData, questData.FriendshipPoints, true, "Clint");
                    }
                    else
                    {
                        UpdateAndExit(friendshipData, questData.FriendshipPoints, true);                    
                    }
                }
                else if (friendshipData.CanReceiveGiftToday())
                {
                    int points = activeItem.GiftTasteRating(friendshipData);
                    UpdateAndExit(friendshipData, points);
                }
            }
        }

        private void UpdateAndExit(FriendshipData friendshipData, int points, bool quest = false, string name = null)
        {
            if (quest)
            {
                questData.Update();
            }

            friendshipData.Update(points, quest, name);
            Game1.player.reduceActiveItemByOne();
            Game1.activeClickableMenu.exitThisMenu();
        }

        private bool ValidateActiveItem()
        {
            if (activeItem.Data == null)
            {
                return false;
            }

            bool strictMarriageItem = activeItem.Data.DisplayName.Equals("Bouquet") || activeItem.Data.DisplayName.Equals("Mermaid's Pendant") || activeItem.Data.DisplayName.Equals("Wedding Ring");
            return activeItem.Data.canBeGivenAsGift() && !strictMarriageItem ? true : false;
        }
    }
}
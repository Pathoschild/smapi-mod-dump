using PelicanPostalService.Framework.Player;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace PelicanPostalService.Framework.Menu
{
    public class PostalService
    {
        private enum Status { CLEAR, REJECTED }
        private readonly ActiveItem activeItem;
        private readonly bool allowQuestSubmissions;
        private bool discoveredQuest;
        private QuestData questData;

        public PostalService(ActiveItem item, bool questSubmissions)
        {
            allowQuestSubmissions = questSubmissions;
            activeItem = item;
        }

        public void Open(IClickableMenu currentMenu, Tool currentTool)
        {
            if (GetProcessStatus() == Status.CLEAR && currentMenu == null && currentTool == null)
            {
                List<string> options = FriendshipData.Find();
                Game1.activeClickableMenu = options != null ? new ChooseFromListMenu(options, OnSelectOption, false) : null;
            }
        }

        private void FindQuestByCriteria(FriendshipData friendshipData)
        {
            questData = new QuestData(activeItem.Data, friendshipData.Who);
            discoveredQuest = questData.Quest != null ? true : false;
        }

        private Status GetProcessStatus()
        {
            if (activeItem.Data == null)
            {
                return Status.REJECTED;
            }

            bool isStrictMarriageItem = activeItem.Data.DisplayName.Equals("Bouquet") || activeItem.Data.DisplayName.Equals("Mermaid's Pendant") || activeItem.Data.DisplayName.Equals("Wedding Ring");
            if (isStrictMarriageItem)
            {
                return Status.REJECTED;
            }

            return activeItem.Data.canBeGivenAsGift() ? Status.CLEAR : Status.REJECTED;
        }

        private void OnSelectOption(string name)
        {
            FriendshipData friendshipData = new FriendshipData(name);

            if (friendshipData.Who != null)
            {
                FindQuestByCriteria(friendshipData);
                bool isStrictQuestItem = activeItem.Data.DisplayName.Equals("Lost Axe") || activeItem.Data.DisplayName.Equals("Lucky Purple Shorts") || activeItem.Data.DisplayName.Equals("Berry Basket");

                if (isStrictQuestItem && !allowQuestSubmissions)
                {
                    return;
                }
                else if (discoveredQuest && allowQuestSubmissions)
                {
                    questData.ResolveOne(activeItem.Data);
                    UpdateAndExit(friendshipData, questData.Points, true);
                }
                else if (friendshipData.CanReceiveGiftToday())
                {
                    int points = activeItem.GiftTasteRating(friendshipData);
                    UpdateAndExit(friendshipData, points);
                }
            }
        }

        private void UpdateAndExit(FriendshipData friendshipData, int points, bool quest = false)
        {
            friendshipData.Update(points, activeItem.Data, quest);
            Game1.player.reduceActiveItemByOne();
            Game1.activeClickableMenu.exitThisMenu();
        }
    }
}
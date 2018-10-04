using Project.Config;
using Project.Framework.Player.Friendship;
using Project.Framework.Player.Items;
using Project.Framework.Player.Quests;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace Project.Framework.Menus
{
    public class PostalService
    {
        private readonly ItemHandler itemInfo;
        private readonly bool allowQuestSubmissions;
        private readonly bool lazyItemChecking;

        public PostalService(ItemHandler itemHandler, ModConfig settings)
        {
            itemInfo = itemHandler;
            allowQuestSubmissions = settings.AllowQuestSubmissions;
            lazyItemChecking = settings.LazyItemChecking;
        }

        public void Open(IClickableMenu currentMenu, Tool currentTool)
        {
            bool isValidItem = IsValidItem(itemInfo.Item);
            if (isValidItem && currentMenu == null && currentTool == null)
            {
                List<string> options = FriendshipHandler.Find();
                Game1.activeClickableMenu = options != null ? new ChooseFromListMenu(options, OnSelectOption, false) : null;
            }
        }

        private bool IsValidItem(Object item)
        {
            if (item == null)
            {
                return false;
            }

            bool isStrictQuestItem = item.DisplayName.Equals("Lost Axe") || item.DisplayName.Equals("Lucky Purple Shorts") || item.DisplayName.Equals("Berry Basket");
            bool strictMarriageItem = item.DisplayName.Equals("Bouquet") || item.DisplayName.Equals("Mermaid's Pendant") || item.DisplayName.Equals("Wedding Ring");
            return item.canBeGivenAsGift() && !strictMarriageItem && !(isStrictQuestItem && !allowQuestSubmissions) ? true : false;
        }

        private void OnSelectOption(string recipient)
        {
            FriendshipHandler friendshipInfo = new FriendshipHandler(recipient);
            if (friendshipInfo.Who != null)
            {
                QuestHandler questInfo = null;
                bool isValidQuest = false;

                if (allowQuestSubmissions)
                {
                    questInfo = new QuestHandler(lazyItemChecking);
                    isValidQuest = questInfo.FindOneAndUpdate(friendshipInfo, itemInfo);
                    itemInfo.RemoveFromInventory(itemInfo.Item.ParentSheetIndex);
                }

                if (!isValidQuest && friendshipInfo.CanReceiveGiftToday() && !questInfo.PreventNextGift)
                {
                    int rating = itemInfo.GiftTasteRating(friendshipInfo);
                    friendshipInfo.Update(rating, false, null);
                    itemInfo.RemoveFromInventory(itemInfo.Item.ParentSheetIndex);
                }

                Game1.activeClickableMenu.exitThisMenu();
            }
        }
    }
}
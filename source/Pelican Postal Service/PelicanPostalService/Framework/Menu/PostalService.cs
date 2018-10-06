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
        private readonly bool allowQuestSubmissions;
        private readonly ItemHandler itemDetails;
        private readonly bool lazyItemChecking;

        public PostalService(ItemHandler itemDetails, ModConfig settings)
        {
            this.itemDetails = itemDetails;
            allowQuestSubmissions = settings.AllowQuestSubmissions;
            lazyItemChecking = settings.LazyItemChecking;
        }

        public void Open()
        {
            bool isValidItem = ProcessItemAndReport(itemDetails.Item);
            if (isValidItem && Game1.activeClickableMenu == null && Game1.player.CurrentTool == null)
            {
                List<string> options = FriendshipHandler.FindKnownNPCs();
                Game1.activeClickableMenu = options != null ? new ChooseFromListMenu(options, OnSelectOption, false) : null;
            }
        }

        private void Exit()
        {
            Game1.activeClickableMenu.exitThisMenu();
        }

        private void OnSelectOption(string target)
        {
            FriendshipHandler friendshipDetails = new FriendshipHandler(target);
            if (friendshipDetails.Who != null)
            {
                if (allowQuestSubmissions)
                {
                    QuestHandler questDetails = new QuestHandler(lazyItemChecking);
                    questDetails.FindOneAndUpdate(friendshipDetails, itemDetails);

                    if (!questDetails.PreventNextGift)
                    {
                        ProcessGift(friendshipDetails);
                    }
                }
                else
                {
                    ProcessGift(friendshipDetails);
                }

                Exit();
            }
        }

        private void ProcessGift(FriendshipHandler friendshipDetails)
        {
            if (friendshipDetails.CanReceiveGiftToday())
            {
                int rating = itemDetails.GiftTasteRating(friendshipDetails);
                friendshipDetails.Update(rating, false, null);
                itemDetails.RemoveFromInventory(1);
            }
        }

        private bool ProcessItemAndReport(Object item)
        {
            if (item == null)
            {
                return false;
            }

            bool isStrictQuestItem = item.DisplayName.Equals("Lost Axe") || item.DisplayName.Equals("Lucky Purple Shorts") || item.DisplayName.Equals("Berry Basket");
            bool isStrictMarriageItem = item.DisplayName.Equals("Bouquet") || item.DisplayName.Equals("Mermaid's Pendant") || item.DisplayName.Equals("Wedding Ring");
            return item.canBeGivenAsGift() && !isStrictMarriageItem && !(isStrictQuestItem && !allowQuestSubmissions) ? true : false;
        }
    }
}
using Project.Framework.Player.Friendship;
using Project.Logging;
using StardewValley;

namespace Project.Framework.Player.Items
{
    public class ItemHandler : Debug
    {
        public Object Item { get; private set; }
        
        public ItemHandler(Object item)
        {
            Item = item;
        }

        public int GiftTasteRating(FriendshipHandler friendshipDetails)
        {
            if (Item == null)
            {
                return 0;
            }

            GiftTaste giftTaste = new GiftTaste(Item, friendshipDetails);
            return giftTaste.Rating;
        }

        public void RemoveFromInventory(int amount)
        {
            if (amount > 1)
            {
                // Removes first matching item in inventory, ignoring quality
                Game1.player.removeItemsFromInventory(Item.ParentSheetIndex, amount);
            }
            else
            {
                Game1.player.reduceActiveItemByOne();
            }
        }
        
        private class GiftTaste
        {
            public int Rating { get; private set; }

            public GiftTaste(Object item, FriendshipHandler friendshipDetails)
            {
                Rating = (int) (RateByRecipient(friendshipDetails.Who, item) * RateByCurrentDate(friendshipDetails.IsBirthday) * RateByQuality(item.Quality));
            }

            private int RateByCurrentDate(bool isBirthday)
            {
                if (Game1.currentSeason.Equals("winter") && Game1.dayOfMonth == 25)
                {
                    return 5;
                }
                else if (isBirthday)
                {
                    return 8;
                }
                return 1;
            }

            private float RateByQuality(int quality)
            {
                // Normal: 0, Silver: 1, Gold: 2, Iridium: 4
                switch (quality)
                {
                    case 1:
                        return 1.1f;
                    case 2:
                        return 1.25f;
                    case 4:
                        return 1.5f;
                    default:
                        return 1f;
                }
            }

            private int RateByRecipient(NPC who, Object item)
            {
                int flag = who.getGiftTasteForThisItem(item);
                switch (flag)
                {
                    case 0:
                        return 80;
                    case 2:
                        return 45;
                    case 4:
                        return -20;
                    case 6:
                        return -40;
                    case 8:
                        return 20;
                    default:
                        return 0;
                }
            }
        }        
    }
}
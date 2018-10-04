using Project.Framework.Player.Friendship;
using StardewValley;

namespace Project.Framework.Player.Items
{
    public class ItemHandler
    {
        public Object Item { get; private set; }
        public int DeductItems { get; set; }
        
        public ItemHandler(Object item)
        {
            Item = item ?? null;
            DeductItems = 1;
        }

        public int GiftTasteRating(FriendshipHandler friendshipInfo)
        {
            if (Item == null)
            {
                return 0;
            }

            GiftTaste giftTaste = new GiftTaste(Item, friendshipInfo);
            return giftTaste.Rating;
        }

        public void RemoveFromInventory(int id)
        {
            Game1.player.removeItemsFromInventory(id, DeductItems);
        }
        
        private class GiftTaste
        {
            public int Rating { get; private set; }

            public GiftTaste(Object item, FriendshipHandler friendshipInfo)
            {
                Rating = (int) (RateByRecipient(item, friendshipInfo) * RateByCurrentDate(friendshipInfo) * RateByQuality(item));
            }

            private int RateByCurrentDate(FriendshipHandler friendshipInfo)
            {
                if (Game1.currentSeason.Equals("winter") && Game1.dayOfMonth == 25)
                {
                    return 5;
                }
                else if (friendshipInfo.IsBirthday)
                {
                    return 8;
                }
                return 1;
            }

            private float RateByQuality(Object item)
            {
                switch (item.Quality)
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

            private int RateByRecipient(Object item, FriendshipHandler friendshipInfo)
            {
                int flag = friendshipInfo.Who.getGiftTasteForThisItem(item);
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
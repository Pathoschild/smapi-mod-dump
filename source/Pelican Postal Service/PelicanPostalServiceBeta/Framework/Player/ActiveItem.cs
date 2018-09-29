using StardewValley;

namespace PelicanPostalService.Framework.Player
{
    public class ActiveItem
    {
        public Object Data { get; private set; }

        public ActiveItem(Object item)
        {
            Data = item ?? null;
        }

        public int GiftTasteRating(FriendshipData friendshipData)
        {
            if (Data == null)
            {
                return 0;
            }

            GiftTaste giftTaste = new GiftTaste(Data, friendshipData);
            return giftTaste.Rating;
        }

        private class GiftTaste
        {
            public int Rating { get; private set; }

            public GiftTaste(Object item, FriendshipData friendshipData)
            {
                Rating = (int)(GiftTasteByNPC(item, friendshipData) * GiftTasteByWorldDate(friendshipData) * GiftTasteByItemQuality(item));
            }

            private float GiftTasteByItemQuality(Object item)
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

            private int GiftTasteByNPC(Object item, FriendshipData friendshipData)
            {
                int flag = friendshipData.Who.getGiftTasteForThisItem(item);
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

            private int GiftTasteByWorldDate(FriendshipData friendshipData)
            {
                if (Game1.currentSeason.Equals("winter") && Game1.dayOfMonth == 25)
                {
                    return 5;
                }
                else if (friendshipData.IsBirthday)
                {
                    return 8;
                }
                return 1;
            }
        }
    }
}
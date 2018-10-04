using StardewValley;
using System.Collections.Generic;

namespace Project.Framework.Player.Friendship
{
    public class FriendshipHandler
    {
        public bool IsBirthday { get; private set; }
        public NPC Who { get; private set; }
        private readonly int giftsThisDay;
        private readonly int giftsThisWeek;
        private readonly bool isSpouse;

        public FriendshipHandler(string recipient)
        {
            Who = Game1.getCharacterFromName(recipient, true);
            IsBirthday = Game1.currentSeason.Equals(Who.Birthday_Season) && Game1.dayOfMonth == Who.Birthday_Day;
            giftsThisDay = Game1.player.friendshipData[recipient].GiftsToday;
            giftsThisWeek = Game1.player.friendshipData[recipient].GiftsThisWeek;
            isSpouse = Game1.player.friendshipData[recipient].IsMarried();
        }

        public bool CanReceiveGiftToday()
        {
            bool normalScenario = giftsThisDay == 0 && giftsThisWeek < 2;
            bool birthdayScenario = giftsThisDay == 0 && giftsThisWeek == 2 && IsBirthday;
            bool marriageScenario = giftsThisDay == 0 && isSpouse;

            return normalScenario || birthdayScenario || marriageScenario ? true : false;
        }

        public static List<string> Find()
        {
            HashSet<string> table = new HashSet<string>();
            foreach (string key in Game1.player.friendshipData.Keys)
            {
                if (Game1.player.friendshipData.ContainsKey(key))
                {
                    table.Add(key);
                }
            }

            return table.Count > 0 ? Sort(table) : null;
        }
        
        public void Update(int points, bool quest, string otherRecipient)
        {
            string who = otherRecipient ?? Who.displayName;
            Game1.player.friendshipData[who].Points += points;

            if (quest == false)
            {
                ++Game1.player.friendshipData[who].GiftsToday;
                ++Game1.player.friendshipData[who].GiftsThisWeek;
                Game1.player.friendshipData[who].LastGiftDate = Game1.Date;
                Game1.addHUDMessage(new HUDMessage("Item sent", 2));
            }
        }

        private static List<string> Sort(HashSet<string> table)
        {
            List<string> list = new List<string>();
            foreach (string key in table)
            {
                list.Add(key);
            }

            list.Sort();
            return list;
        }
    }
}
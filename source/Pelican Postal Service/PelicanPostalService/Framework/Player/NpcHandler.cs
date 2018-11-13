using StardewValley;
using System.Collections.Generic;

namespace Pelican.Friendship
{
    public class NpcHandler : Meta
    {
        public static Dictionary<string, string> Dictionary { get; private set; }
        public NPC Target { get; private set; }

        public NpcHandler(string target)
        {
            Target = FindOneByName(target);
        }

        public static bool CanReceiveGiftToday(string target)
        {
            int giftsThisDay = Game1.player.friendshipData[target].GiftsToday;
            int giftsThisWeek = Game1.player.friendshipData[target].GiftsThisWeek;
            bool isSpouse = Game1.player.friendshipData[target].IsMarried();

            bool normal = giftsThisDay == 0 && giftsThisWeek < 2;
            bool birthday = giftsThisDay == 0 && giftsThisWeek == 2 && IsBirthday(target);
            bool married = giftsThisDay == 0 && isSpouse;

            return normal || birthday || married ? true : false;
        }

        public static List<string> FindKnownNPCs()
        {
            Dictionary = new Dictionary<string, string>();
            List<string> options = new List<string>();

            foreach (string key in Game1.player.friendshipData.Keys)
            {
                if (Game1.player.friendshipData.ContainsKey(key))
                {
                    // Dictionary added for compatibility with mods and foreign language users
                    if (Meta.Config.AllowQuestSubmissions || CanReceiveGiftToday(key))
                    {
                        string displayName = FindOneByName(key).displayName;
                        Dictionary.Add(displayName, key);
                        options.Add(displayName);
                    }
                }
            }

            options.Sort();
            return options;
        }

        public static bool IsBirthday(string target)
        {
            NPC who = FindOneByName(target);
            return Game1.currentSeason.Equals(who.Birthday_Season) && Game1.dayOfMonth == who.Birthday_Day;
        }

        public void Update(int points, bool quest, string otherRecipient)
        {
            string target = otherRecipient ?? Target.Name;
            Game1.player.friendshipData[target].Points += points;

            if (!quest)
            {
                string i18n = Lang.Get("giftComplete", new { name = target });
                Game1.addHUDMessage(new HUDMessage(i18n, 2));

                ++Game1.player.friendshipData[target].GiftsToday;
                ++Game1.player.friendshipData[target].GiftsThisWeek;
                Game1.player.friendshipData[target].LastGiftDate = Game1.Date;
            }
        }

        private static NPC FindOneByName(string target)
        {
            return Game1.getCharacterFromName(target, true);
        }
    }
}
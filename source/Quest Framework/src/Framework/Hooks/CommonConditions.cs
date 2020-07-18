using PurrplingCore;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Hooks
{
    internal class CommonConditions
    {
        public static Dictionary<string, Func<string, bool>> GetConditions()
        {
            return new Dictionary<string, Func<string, bool>>()
            {
                ["Weather"] = (valueToCheck) => GetCurrentWeatherName() == valueToCheck,
                ["Date"] = (valueToCheck) => SDate.Now() == Utils.ParseDate(valueToCheck),
                ["Days"] = (valueToCheck) => Utility.parseStringToIntArray(valueToCheck).Any(d => d == SDate.Now().Day),
                ["Seasons"] = (valueToCheck) => valueToCheck.Split(' ').Any(s => s == SDate.Now().Season),
                ["DaysOfWeek"] = (valueToCheck) => valueToCheck.Split(' ').Any(
                        d => d.ToLower() == SDate.Now().DayOfWeek.ToString().ToLower()),
                ["Friendship"] = (valueToCheck) => CheckFriendshipCondition(valueToCheck),
                ["MailReceived"] = (valueToCheck) => CheckReceivedMailCondition(valueToCheck),
                ["EventSeen"] = (valueToCheck) => CheckEventSeenCondition(valueToCheck),
                ["MailNotReceived"] = (valueToCheck) => CheckReceivedMailCondition(valueToCheck, not: true),
                ["EventNotSeen"] = (valueToCheck) => CheckEventSeenCondition(valueToCheck, not: true),
                ["MinDaysPlayed"] = (valueToCheck) => Game1.Date.TotalDays >= Convert.ToInt32(valueToCheck),
                ["MaxDaysPlayed"] = (valueToCheck) => Game1.Date.TotalDays <= Convert.ToInt32(valueToCheck),
                ["DaysPlayed"] = (valueToCheck) => Game1.Date.TotalDays == Convert.ToInt32(valueToCheck),
                ["IsPlayerMarried"] = (valueToCheck) => ParseBool(valueToCheck) == Game1.player.isMarried(),
            };
        }

        public static bool CheckEventSeenCondition(string valueToCheck, bool not = false)
        {
            int[] events = Utility.parseStringToIntArray(valueToCheck);
            bool flag = true;

            if (events.Length < 1)
                return false;

            foreach (var ev in events)
            {
                flag &= Game1.player.eventsSeen.Contains(ev);
            }

            return not ? !flag : flag;
        }

        public static bool CheckReceivedMailCondition(string valueToCheck, bool not = false)
        {
            string[] mails = valueToCheck.Split(' ');
            bool flag = true;

            if (mails.Length < 1)
                return false;

            foreach (string mail in mails)
            {
                flag &= Game1.player.mailReceived.Contains(mail);
            }

            return not ? !flag : flag;
        }

        public static bool CheckFriendshipCondition(string friendshipDefinition)
        {
            string[] fships = friendshipDefinition.Split(' ');
            bool flag = true;

            if (fships.Length < 2)
                return false;

            for (int i = 0; i < fships.Length; i += 2)
            {
                flag &= Game1.player.getFriendshipHeartLevelForNPC(fships[i])
                    == Convert.ToInt32(fships[i + 1]);
            }

            return flag;
        }

        private static bool ParseBool(string str)
        {
            var truthyVals = new string[] { "true", "yes", "1", "on", "enabled" };
            var falsyVals = new string[] { "false", "no", "0", "off", "disabled" };
            str = str.ToLower();

            if (truthyVals.Contains(str))
                return true;

            if (falsyVals.Contains(str))
                return false;

            throw new InvalidCastException($"Unable to convert `{str}` to boolean.");
        }

        private static string GetCurrentWeatherName()
        {
            if (Game1.isRaining)
                return "rainy";
            if (Game1.isSnowing)
                return "snowy";
            if (Game1.isLightning)
                return "stormy";
            if (Game1.isDebrisWeather)
                return "cloudy";

            return "sunny";
        }
    }
}

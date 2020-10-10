/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using PurrplingCore;
using QuestFramework.Extensions;
using QuestFramework.Framework.Stats;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework.Hooks
{
    internal class CommonConditions
    {
        internal protected static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;
        public static Dictionary<string, Func<string, CustomQuest, bool>> GetConditions()
        {
            return new Dictionary<string, Func<string, CustomQuest, bool>>()
            {
                ["Weather"] = (valueToCheck, _) => GetCurrentWeatherName() == valueToCheck.ToLower(),
                ["Date"] = (valueToCheck, _) => SDate.Now() == Utils.ParseDate(valueToCheck),
                ["Days"] = (valueToCheck, _) => Utility.parseStringToIntArray(valueToCheck).Any(d => d == SDate.Now().Day),
                ["Seasons"] = (valueToCheck, _) => valueToCheck.ToLower().Split(' ').Any(s => s == SDate.Now().Season),
                ["DaysOfWeek"] = (valueToCheck, _) => valueToCheck.Split(' ').Any(
                        d => d.ToLower() == SDate.Now().DayOfWeek.ToString().ToLower()),
                ["Friendship"] = (valueToCheck, _) => DeprecatedCondition(() => CheckFriendshipLevel(valueToCheck), "Friendship", "FriendshipLevel"),
                ["FriendshipLevel"] = (valueToCheck, _) => CheckFriendshipLevel(valueToCheck), // Emily 7
                ["FriendshipStatus"] = (valueToCheck, _) => CheckFriendshipStatus(valueToCheck), // Shane Dating
                ["MailReceived"] = (valueToCheck, _) => CheckReceivedMailCondition(valueToCheck),
                ["EventSeen"] = (valueToCheck, _) => CheckEventSeenCondition(valueToCheck),
                ["MinDaysPlayed"] = (valueToCheck, _) => Game1.Date.TotalDays >= Convert.ToInt32(valueToCheck),
                ["MaxDaysPlayed"] = (valueToCheck, _) => Game1.Date.TotalDays <= Convert.ToInt32(valueToCheck),
                ["DaysPlayed"] = (valueToCheck, _) => Game1.Date.TotalDays == Convert.ToInt32(valueToCheck),
                ["IsPlayerMarried"] = (valueToCheck, _) => ParseBool(valueToCheck) == Game1.player.isMarried(),
                ["QuestAcceptedInPeriod"] = (valueToCheck, managedQuest) => IsQuestAcceptedInPeriod(valueToCheck, managedQuest),
                ["QuestAcceptedDate"] = (valueToCheck, managedQuest) => IsQuestAcceptedDate(Utils.ParseDate(valueToCheck), managedQuest),
                ["QuestCompletedDate"] = (valueToCheck, managedQuest) => IsQuestCompletedDate(Utils.ParseDate(valueToCheck), managedQuest),
                ["QuestAcceptedToday"] = (valueToCheck, managedQuest) => IsQuestAcceptedDate(SDate.Now(), managedQuest) == ParseBool(valueToCheck),
                ["QuestCompletedToday"] = (valueToCheck, managedQuest) => IsQuestCompletedDate(SDate.Now(), managedQuest) == ParseBool(valueToCheck),
                ["QuestNeverAccepted"] = (valueToCheck, managedQuest) => managedQuest.IsNeverAccepted() == ParseBool(valueToCheck),
                ["QuestNeverCompleted"] = (valueToCheck, managedQuest) => managedQuest.IsNeverCompleted() == ParseBool(valueToCheck),
                ["KnownCraftingRecipe"] = (valueToCheck, _) => Game1.player.craftingRecipes.ContainsKey(valueToCheck),
                ["KnownCookingRecipe"] = (valueToCheck, _) => Game1.player.cookingRecipes.ContainsKey(valueToCheck),
                ["IsCommunityCenterCompleted"] = (valueToCheck, _) => ParseBool(valueToCheck) == Game1.player.hasCompletedCommunityCenter(),
                ["BuildingConstructed"] = (valueToCheck, _) => CheckBuilding(valueToCheck), // Barn
                ["SkillLevel"] = (valueToCheck, _) => CheckSkillLevel(valueToCheck), // Farming 1 Foraging 2
                ["HasMod"] = (valueToCheck, _) => CheckHasModCondition(valueToCheck),
                ["Random"] = (valueToCheck, _) => Game1.random.NextDouble() < Convert.ToDouble(valueToCheck) / 100, // Chance is in %
                ["EPU"] = (valueToCheck, _) => CheckEpuCondition(valueToCheck), // For compatibility with EPU conditions
            };
        }

        private static bool CheckHasModCondition(string valueToCheck)
        {
            foreach (string modUid in valueToCheck.Split(' '))
            {
                Monitor.VerboseLog($"Checking if mod `{modUid}` is loaded ...");
                if (!QuestFrameworkMod.Instance.Helper.ModRegistry.IsLoaded(modUid))
                {
                    Monitor.VerboseLog($"Mod `{modUid}` is not loaded! Returns false");
                    return false;
                }
            }

            return true;
        }

        private static bool CheckEpuCondition(string valueToCheck)
        {
            var epu = QuestFrameworkMod.Instance.Bridge.EPU;

            if (epu == null)
            {
                Monitor.Log("Unable to check EPU condition. Expanded Preconditions Utility is not loaded. Do you have this mod installed in mods folder?", LogLevel.Error);
                return false;
            }

            return epu.CheckConditions(valueToCheck);
        }

        private static bool IsQuestAcceptedDate(SDate dateToCheck, CustomQuest managedQuest)
        {
            return GetQuestStats(managedQuest).LastAccepted == dateToCheck;
        }

        private static bool IsQuestCompletedDate(SDate dateToCheck, CustomQuest managedQuest)
        {
            return GetQuestStats(managedQuest).LastCompleted == dateToCheck;
        }

        private static bool IsQuestAcceptedInPeriod(string valueToCheck, CustomQuest managedQuest)
        {
            if (!Context.IsWorldReady)
                return false;
            var parts = valueToCheck.Split(' ');
            var acceptDate = GetQuestStats(managedQuest).LastAccepted;
            SDate now = SDate.Now();

            Monitor.VerboseLog(
                $"Checking quest accept date `{acceptDate}` matches current `{now}` by `{valueToCheck}`");

            if (parts.Length < 1 || acceptDate == null || (parts.Contains("today") && acceptDate != now))
                return false;

            bool flag = true;
            foreach (string part in parts)
            {
                string[] period = part.Split('=');
                string type = period.ElementAtOrDefault(0);
                string value = period.ElementAtOrDefault(1);

                switch (type)
                {
                    case "y":
                    case "year":
                        flag &= acceptDate.Year == (
                            int.TryParse(value, out var year)
                                ? year
                                : now.Year
                            );
                        break;
                    case "s":
                    case "season":
                        flag &= acceptDate.Season == (value ?? now.Season);
                        break;
                    case "wd":
                    case "weekday":
                        flag &= acceptDate.DayOfWeek == (
                            Enum.TryParse<DayOfWeek>(value, out var dayOfWeek)
                                ? dayOfWeek
                                : now.DayOfWeek
                            );
                        break;
                    case "d":
                    case "day":
                        flag &= acceptDate.Day == (
                            int.TryParse(value, out var day)
                                ? day
                                : now.Day
                            );
                        break;
                    default:
                        flag &= false;
                        break;
                }
            }

            return flag;
        }

        private static QuestStatSummary GetQuestStats(CustomQuest managedQuest)
        {
            return QuestFrameworkMod.Instance
                .StatsManager
                .GetStats(Game1.player.UniqueMultiplayerID)
                .GetQuestStatSummary(managedQuest.GetFullName());
        }

        public static bool CheckEventSeenCondition(string valueToCheck)
        {
            int[] events = Utility.parseStringToIntArray(valueToCheck);
            bool flag = true;

            if (events.Length < 1)
                return false;

            foreach (var ev in events)
            {
                flag &= Game1.player.eventsSeen.Contains(ev);
                Monitor.VerboseLog($"Checked if event `{ev}` was seen. Current flag: {flag}");
            }

            return flag;
        }

        public static bool CheckReceivedMailCondition(string valueToCheck)
        {
            string[] mails = valueToCheck.Split(' ');
            bool flag = true;

            if (mails.Length < 1)
                return false;

            foreach (string mail in mails)
            {
                flag &= Game1.player.mailReceived.Contains(mail);
                Monitor.VerboseLog($"Checked if mail letter `{mail}` was received. Current flag: {flag}");
            }

            return flag;
        }

        public static bool CheckFriendshipLevel(string friendshipLevel)
        {
            string[] friendshipLevelParts = friendshipLevel.Split(' ');
            bool flag = true;

            if (friendshipLevelParts.Length % 2 != 0)
                return false;

            if (friendshipLevelParts.Length < 2)
                return false;

            for (int i = 0; i < friendshipLevelParts.Length; i += 2)
            {
                string whoLevel = friendshipLevelParts[i];
                int currentFriendshipLevel = Game1.player.getFriendshipHeartLevelForNPC(whoLevel);
                int expectedFriendshipLevel = Convert.ToInt32(friendshipLevelParts[i + 1]);

                flag &= currentFriendshipLevel >= expectedFriendshipLevel;

                if (Monitor.IsVerbose)
                    Monitor.Log(
                        $"Checked friendship level for `{whoLevel}`, " +
                        $"current level: {currentFriendshipLevel}, " +
                        $"expected: {expectedFriendshipLevel}, " +
                        $"current flag: {flag}");
            }

            return flag;
        }

        public static bool CheckFriendshipStatus(string friendshipStatus) 
        {
            string[] friendshipStatusParts = friendshipStatus.Split(' ');
            bool flag = true;

            if (friendshipStatusParts.Length % 2 != 0)
                return false;

            if (friendshipStatusParts.Length < 2)
                return false;

            for (int i = 0; i < friendshipStatusParts.Length; i += 2)
            {
                string whoStatus = friendshipStatusParts[i];
                if (!Game1.player.friendshipData.ContainsKey(whoStatus))
                {
                    flag &= false;
                    continue;
                };
                string currentStatus = Game1.player.friendshipData[whoStatus].Status.ToString();
                string expectedStatus = friendshipStatusParts[i + 1];

                flag &= currentStatus == expectedStatus;

                if (Monitor.IsVerbose)
                    Monitor.Log(
                        $"Checked friendship status for `{whoStatus}`, " +
                        $"current status: {currentStatus}, " +
                        $"expected status: {expectedStatus}, " +
                        $"current flag: {flag}");
            }
            
            return flag;
        }

        public static bool CheckSkillLevel(string skillLevel) 
        {
            string[] skillLevelParts = skillLevel.Split(' ');
            bool flag = true;

            if (skillLevelParts.Length < 2)
                return false;

            for (int i = 0; i < skillLevelParts.Length; i += 2)
            {
                string[] skillList = { "Farming", "Fishing", "Foraging", "Mining", "Combat", "Luck" };
                string skillName = skillLevelParts[i];
                int skillId = Array.IndexOf(skillList, skillLevelParts[i]);
                int currentSkillLevel = Game1.player.getEffectiveSkillLevel(skillId);
                int expectedSkillLevel = Convert.ToInt32(skillLevelParts[i + 1]);

                flag &= currentSkillLevel >= expectedSkillLevel;

                if (Monitor.IsVerbose)
                    Monitor.Log(
                        $"Checked skill level for `{skillName}` " +
                        $"with skill id: {skillId}, " +
                        $"current level: {currentSkillLevel}, " +
                        $"expected level: {expectedSkillLevel}, " +
                        $"current flag: {flag}");
            }

            return flag;
        }

        public static bool CheckBuilding(string checkedBuilding)
        {
            string[] building = checkedBuilding.Split(' ');
            bool flag = true;

            for (int i = 0; i < building.Length; i += 1)
            {
                string buildingName = building[i].Replace('_', ' ');
                bool isBuildingConstructed = Game1.getFarm().isBuildingConstructed(buildingName);
                flag &= isBuildingConstructed;

                if (Monitor.IsVerbose)
                    Monitor.Log(
                        $"Checked `{buildingName}` is built in farm" +
                        $"current flag: {flag}");
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

        private static bool DeprecatedCondition(Func<bool> conditionCallback, string oldConditionName, string newConditionName)
        {
            Monitor.Log($"`{oldConditionName}` is deprecated, use `{newConditionName}` instead", LogLevel.Warn);
            return conditionCallback();
        }
    }
}

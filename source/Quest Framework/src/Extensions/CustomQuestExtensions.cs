/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Quests;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Extensions
{
    public static class CustomQuestExtensions
    {
        /// <summary>
        /// Complete this managed quest if this quest is accepted in player's quest log
        /// </summary>
        /// <param name="customQuest"></param>
        public static void Complete(this CustomQuest customQuest)
        {
            if (Context.IsWorldReady && Game1.player.hasQuest(customQuest.id))
                Game1.player.completeQuest(customQuest.id);
        }

        /// <summary>
        /// Accept this managed quest and add them to player's quest log if this quest not already accepted in the log.
        /// </summary>
        /// <param name="customQuest"></param>
        public static void Accept(this CustomQuest customQuest)
        {
            if (Context.IsWorldReady)
                Game1.player.addQuest(customQuest.id);
        }

        /// <summary>
        /// Remove this managed quest from player's quest log without completion.
        /// </summary>
        /// <param name="customQuest"></param>
        public static void RemoveFromQuestLog(this CustomQuest customQuest)
        {
            if (Context.IsWorldReady)
                Game1.player.removeQuest(customQuest.id);
        }

        /// <summary>
        /// Get connected vanilla quest with this managed quest in quest log
        /// </summary>
        /// <param name="customQuest"></param>
        /// <returns>null if this quest is not in quest log, otherwise the connected vanilla quest</returns>
        public static Quest GetInQuestLog(this CustomQuest customQuest)
        {
            if (!Context.IsWorldReady)
                return null;

            return Game1.player.questLog
                .Where(q => q.id.Value == customQuest.id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Check if this quest is included in player's quest log.
        /// </summary>
        /// <param name="customQuest"></param>
        /// <returns></returns>
        public static bool IsInQuestLog(this CustomQuest customQuest)
        {
            return Context.IsWorldReady && Game1.player.hasQuest(customQuest.id);
        }

        public static bool IsNeverAccepted(this CustomQuest customQuest)
        {
            if (!Context.IsWorldReady)
                return false;

            var stats = QuestFrameworkMod.Instance.
                StatsManager
                .GetStats(Game1.player.UniqueMultiplayerID);

            return stats.GetQuestStatSummary(customQuest.GetFullName()).LastAccepted == null;
        }

        public static bool IsNeverCompleted(this CustomQuest customQuest)
        {
            if (!Context.IsWorldReady)
                return false;

            var stats = QuestFrameworkMod.Instance.
                StatsManager
                .GetStats(Game1.player.UniqueMultiplayerID);

            return stats.GetQuestStatSummary(customQuest.GetFullName()).LastCompleted == null;
        }

        public static bool CheckGlobalConditions(this CustomQuest customQuest, Dictionary<string, string> conditions, IEnumerable<string> ignore = null, bool ignoreUnknown = false)
        {
            return QuestFrameworkMod.Instance.ConditionManager.CheckConditions(conditions, customQuest, ignore, ignoreUnknown);
        }

        public static bool CheckGlobalCondition(this CustomQuest customQuest, string condition, string value, bool ignoreUnknown = false)
        {
            return QuestFrameworkMod.Instance.ConditionManager.CheckCondition(condition, value, customQuest, ignoreUnknown);
        }
    }
}

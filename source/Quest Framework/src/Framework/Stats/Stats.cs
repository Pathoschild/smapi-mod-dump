using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework.Stats
{
    internal class Stats
    {
        public List<QuestStat> CompletedQuests { get; set; } = new List<QuestStat>();
        public List<QuestStat> AcceptedQuests { get; set; } = new List<QuestStat>();
        public List<QuestStat> RemovedQuests { get; set; } = new List<QuestStat>();

        public QuestStatSummary GetQuestStatSummary(string questName)
        {
            bool statPredicate(QuestStat stat) => stat.FullQuestName == questName;

            var completions = this.CompletedQuests.Where(statPredicate);
            var acceptals = this.AcceptedQuests.Where(statPredicate);
            var removals = this.RemovedQuests.Where(statPredicate);

            return new QuestStatSummary
            {
                QuestName = questName,
                AcceptalCount = acceptals.Count(),
                CompletionCount = completions.Count(),
                RemovalCount = removals.Count(),
                LastAccepted = GetLastDate(acceptals),
                LastCompleted = GetLastDate(completions),
                LastRemoved = GetLastDate(removals),
            };
        }

        private static SDate GetLastDate(IEnumerable<QuestStat> questStats)
        {
            return questStats.LastOrDefault()?.Date;
        }
    }
}

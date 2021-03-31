/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

namespace QuestFramework.Framework.Stats
{
    internal class QuestStat
    {
        public int DaysSinceStart { get; set; }
        public string FullQuestName { get; set; }

        public QuestStat(string fullQuestName, int daysSinceStart)
        {
            this.DaysSinceStart = daysSinceStart;
            this.FullQuestName = fullQuestName;
        }

        public QuestStat() { }

        [JsonIgnore]
        public SDate Date
        {
            get => SDate.FromDaysSinceStart(this.DaysSinceStart);
            set => this.DaysSinceStart = value?.DaysSinceStart ?? 0;
        }
    }
}

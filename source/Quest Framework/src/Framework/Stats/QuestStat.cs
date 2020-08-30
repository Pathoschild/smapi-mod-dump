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

using StardewModdingAPI.Utilities;

namespace QuestFramework.Framework.Stats
{
    internal class QuestStatSummary
    {
        public string QuestName { get; set; }
        public SDate LastAccepted { get; set; }
        public SDate LastCompleted { get; set; }
        public SDate LastRemoved { get; set; }

        public int AcceptalCount { get; set; }
        public int CompletionCount { get; set; }
        public int RemovalCount { get; set; }
    }
}

using QuestFramework.Framework.Store;
using System.Collections.Generic;


namespace QuestFramework.Framework.Networking
{
    class InitalMessage
    {
        public InitalMessage(Dictionary<string, int> questIds, QuestStateStoreData store, Dictionary<long, Stats.Stats> questStats)
        {
            this.QuestIdList = questIds;
            this.InitalStore = store;
            this.QuestStats = questStats;
        }

        public Dictionary <string, int> QuestIdList { get; set; }
        public QuestStateStoreData InitalStore { get; set; }
        public Dictionary<long, Stats.Stats> QuestStats { get; set; }
    }
}

using QuestFramework.Framework.Store;
using System.Collections.Generic;


namespace QuestFramework.Framework.Networking
{
    class InitalMessage
    {
        public InitalMessage(Dictionary<string, int> questIds, QuestStateStoreData store)
        {
            this.QuestIdList = questIds;
            this.InitalStore = store;
        }

        public Dictionary <string, int> QuestIdList { get; set; }
        public QuestStateStoreData InitalStore { get; set; }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

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

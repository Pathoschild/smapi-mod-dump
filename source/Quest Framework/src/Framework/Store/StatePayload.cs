using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Store
{
    internal class StatePayload
    {
        public StatePayload() { }

        public StatePayload(string questName, long farmerId, JToken stateData)
        {
            this.QuestName = questName;
            this.FarmerId = farmerId;
            this.StateData = stateData;
        }

        public long FarmerId { get; set; }
        public string QuestName { get; set; }
        public JToken StateData { get; set; }
    }
}

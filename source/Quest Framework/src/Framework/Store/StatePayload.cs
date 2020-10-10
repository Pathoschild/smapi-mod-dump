/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

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

        public StatePayload(string questName, long farmerId, JObject stateData)
        {
            this.QuestName = questName;
            this.FarmerId = farmerId;
            this.StateData = stateData;
        }

        public long FarmerId { get; set; }
        public string QuestName { get; set; }
        public JObject StateData { get; set; }
    }
}

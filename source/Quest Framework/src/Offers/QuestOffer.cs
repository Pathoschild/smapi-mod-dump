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
using Newtonsoft.Json.Linq;
using QuestFramework.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Offers
{
    /// <summary>
    /// Quest source definition (offer)
    /// </summary>
    public class QuestOffer
    {
        public string QuestName { get; set; }
        public string OfferedBy { get; set; }
        public Dictionary<string, string> When { get; set; }
        public bool OnlyMainPlayer { get; set; }

        [JsonIgnore]
        public Func<CustomQuest, bool> ConditionFunc { get; set; }

        public QuestOffer<TAttributes> AsOfferWithDetails<TAttributes>()
        {
            if (this is QuestOffer<JObject> jsonSchedule)
            {
                return jsonSchedule.OfferDetails != null
                    ? JToken.FromObject(jsonSchedule).ToObject<QuestOffer<TAttributes>>()
                    : null;
            }

            return this as QuestOffer<TAttributes>;
        }

        internal bool CheckAdditionalCondition(CustomQuest context)
        {
            if (this.ConditionFunc != null)
            {
                return this.ConditionFunc(context);
            }

            return true;
        }
    }

    public class QuestOffer<TDetails> : QuestOffer
    {
        public TDetails OfferDetails { get; set; }
    }
}

using Newtonsoft.Json.Linq;
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
    }

    public class QuestOffer<TDetails> : QuestOffer
    {
        public TDetails OfferDetails { get; set; }
    }
}

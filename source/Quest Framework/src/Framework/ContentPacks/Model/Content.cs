using Newtonsoft.Json.Linq;
using QuestFramework.Offers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.ContentPacks.Model
{
    class Content
    {
        public ISemanticVersion Format { get; set; }

        public List<QuestData> Quests { get; set; }
        public List<QuestOffer<JObject>> Offers { get; set; } = new List<QuestOffer<JObject>>();

        internal IContentPack owner { get; set; }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestFramework.Hooks;
using QuestFramework.Quests;
using System.Collections.Generic;

namespace QuestFramework.Framework.ContentPacks.Model
{
    class QuestData
    {
        public string Name { get; set; }
        public QuestType Type { get; set; } = QuestType.Basic;
        public int CustomTypeId { get; set; } = -1;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public List<string> NextQuests { get; set; }
        public int DaysLeft { get; set; }
        public int Reward { get; set; }
        public string RewardDescription { get; set; }
        public bool Cancelable { get; set; }
        public string ReactionText { get; set; }
        public JToken Trigger { get; set; }
        public List<Hook> Hooks { get; set; }

        [JsonExtensionData]
        public JObject ExtendedData { get; set; }

        public void PopulateExtendedData(CustomQuest customQuest)
        {
            if (this.ExtendedData != null)
                JsonConvert.PopulateObject(this.ExtendedData.ToString(), customQuest);
        }
    }
}

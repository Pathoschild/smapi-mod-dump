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
using QuestFramework.Hooks;
using QuestFramework.Quests;
using QuestFramework.Structures;
using StardewModdingAPI;
using System.Collections.Generic;

namespace QuestFramework.Framework.ContentPacks.Model
{
    internal class QuestData : ITranslatable<QuestData>
    {
        public string Name { get; set; }
        public string Type { get; set; } = QuestType.Basic.ToString();
        public int CustomTypeId { get; set; } = -1;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public List<string> NextQuests { get; set; }
        public int DaysLeft { get; set; }
        public JToken Reward { get; set; }
        public RewardType RewardType { get; set; } = RewardType.Money;
        public int RewardAmount { get; set; }
        public string RewardDescription { get; set; }
        public bool Cancelable { get; set; }
        public string ReactionText { get; set; }
        public JToken Trigger { get; set; }
        public string Texture { get; set; }
        public QuestLogColors Colors { get; set; }
        public List<Hook> Hooks { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public Dictionary<string, int> FriendshipGain { get; set; }
        public ConversationTopicData ConversationTopic { get; set; }

        [JsonExtensionData]
        public JObject ExtendedData { get; set; }
        public void PopulateExtendedData(CustomQuest customQuest)
        {
            if (this.ExtendedData != null)
                JsonConvert.PopulateObject(this.ExtendedData.ToString(), customQuest);
        }

        public QuestData Translate(ITranslationHelper translation)
        {
            return TranslationUtils.Translate(translation, this);
        }
    }
}

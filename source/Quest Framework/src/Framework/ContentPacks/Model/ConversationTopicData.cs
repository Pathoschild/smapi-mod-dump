/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

namespace QuestFramework.Framework.ContentPacks.Model
{
    internal class ConversationTopicData
    {
        public string AddWhenQuestAccepted { get; set; }
        public string AddWhenQuestRemoved { get; set; }
        public string AddWhenQuestCompleted { get; set; }
        public string RemoveWhenQuestCompleted { get; set; }
        public string RemoveWhenQuestRemoved { get; set; }
        public string RemoveWhenQuestAccepted { get; set; }
    }
}
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

namespace QuestFramework.Quests
{
    public class CustomQuestObjective
    {
        public CustomQuestObjective(string tag, string text)
        {
            this.Tag = tag;
            this.Text = text;
        }

        public string Tag { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { get; set; }
    }
}
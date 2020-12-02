/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;


namespace QuestFramework.Framework.Helpers
{
    internal class ConversationTopicHelper
    {
        internal protected static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;
        static public void AddConversationTopic(string addConversationTopicInput)
        {
            string[] convTopicToAddParts = addConversationTopicInput.Split(' ');

            if (convTopicToAddParts.Length % 2 == 0 && convTopicToAddParts.Length >= 2)
            {
                for (int i = 0; i < convTopicToAddParts.Length; i += 2)
                {
                    string convTopicToAdd = convTopicToAddParts[i];
                    int convTopicCompletedDaysActive = Convert.ToInt32(convTopicToAddParts[i + 1]);

                    if (Game1.player.activeDialogueEvents.ContainsKey(convTopicToAdd))
                        continue;

                    Game1.player.activeDialogueEvents.Add(convTopicToAdd, convTopicCompletedDaysActive);
                    Monitor.Log($"Added conversation topic with the key: `{convTopicToAdd}`" +
                        $"the conversation topic will be active for `{convTopicCompletedDaysActive}` days");
                }
            }

        }
        static public void RemoveConversationTopic(string removeConversationTopicInput)
        {
            foreach (string convTopicToRemove in removeConversationTopicInput.Split(' ') )
            {
                if (!Game1.player.activeDialogueEvents.ContainsKey(convTopicToRemove))
                    continue;

                Game1.player.activeDialogueEvents.Remove(convTopicToRemove);
                Monitor.Log($"Removed conversation topic with the key: `{convTopicToRemove}`");        
            }
        }
    }
}

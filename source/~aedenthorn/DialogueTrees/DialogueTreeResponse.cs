/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace DialogueTrees
{
    public class DialogueTreeResponse
    {
        public DialogueTree lastTopic;
        public DialogueTree nextTopic;
        public Dictionary<string, string> topicResponses = new Dictionary<string, string>();
        public NPC npc;
        public string responseID;

        public DialogueTreeResponse(DialogueTree _lastTopic, DialogueTree _nextTopic, NPC n, string responseID)
        {
            lastTopic = _lastTopic;
            nextTopic = _nextTopic;
            topicResponses[lastTopic.topicID] = responseID;
            npc = n;
        }
    }
}
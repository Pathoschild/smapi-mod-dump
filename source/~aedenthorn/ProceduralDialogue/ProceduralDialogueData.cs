/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace ProceduralDialogue
{
    public class ProceduralDialogueData
    {
        public List<ProceduralDialogue> dialogues = new List<ProceduralDialogue>();
        public Dictionary<string, string> topicNames = new Dictionary<string, string>();
        public Dictionary<string, string> playerQuestions = new Dictionary<string, string>();
        public Dictionary<string, string> playerResponses = new Dictionary<string, string>();
        public Dictionary<string, string> UIStrings = new Dictionary<string, string>();
    }
    public class ProceduralDialogue
    {
        public string topicID;
        public List<string> questionIDs;
        public List<string> responseIDs;
    }
}
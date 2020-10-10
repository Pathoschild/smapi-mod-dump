/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace QuestFramework.Framework.Stats
{
    internal class QuestStatSummary
    {
        public string QuestName { get; set; }
        public SDate LastAccepted { get; set; }
        public SDate LastCompleted { get; set; }
        public SDate LastRemoved { get; set; }

        public int AcceptalCount { get; set; }
        public int CompletionCount { get; set; }
        public int RemovalCount { get; set; }
    }
}

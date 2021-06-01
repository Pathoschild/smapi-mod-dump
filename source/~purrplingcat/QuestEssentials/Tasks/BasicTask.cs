/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestEssentials.Messages;

namespace QuestEssentials.Tasks
{
    class BasicTask : QuestTask
    {
        public string Trigger { get; set; }

        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (!this.IsCompleted() && this.Trigger == message.Trigger && this.IsWhenMatched())
            {
                this.IncrementCount(1);

                return true;
            }

            return false;
        }
    }
}

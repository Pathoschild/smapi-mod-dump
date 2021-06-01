/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestEssentials.Framework;
using QuestEssentials.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Tasks
{
    public class FishTask : QuestTask<FishTask.FishData>
    {
        public struct FishData
        {
            public string AcceptedContextTags { get; set; }
        }

        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (this.IsCompleted())
                return false;

            if (message is FishMessage fishMessage)
            {
                if (fishMessage.Fish == null || this.Data.AcceptedContextTags == null || !this.IsWhenMatched())
                    return false;

                if (Helper.CheckItemContextTags(fishMessage.Fish, this.Data.AcceptedContextTags))
                {
                    this.IncrementCount(fishMessage.Fish.Stack);

                    return true;
                }
            }

            return false;
        }
    }
}

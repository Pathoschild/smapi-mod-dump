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
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Tasks
{
    public class CraftTask : QuestTask<CraftTask.CraftData>
    {
        public struct CraftData
        {
            public string AcceptedContextTags { get; set; }
        }
        
        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (this.IsCompleted())
                return false;

            if (message is VanillaCompletionMessage args && args.CompletionType == Quest.type_crafting)
            {
                if (args.Item == null || this.Data.AcceptedContextTags == null || !this.IsWhenMatched())
                    return false;

                if (Helper.CheckItemContextTags(args.Item, this.Data.AcceptedContextTags))
                {
                    this.IncrementCount(args.Item.Stack);

                    return true;
                }
            }

            return false;
        }
    }
}

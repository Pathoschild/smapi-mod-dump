/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestFramework.Quests;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Messages
{
    public class VanillaCompletionMessage : StoryMessage, ICompletionArgs
    {
        public VanillaCompletionMessage(ICompletionArgs fromArgs) : base("VanillaCompletion")
        {
            if (fromArgs == null)
            {
                throw new ArgumentNullException(nameof(fromArgs));
            }

            this.Npc = fromArgs.Npc;
            this.Number1 = fromArgs.Number1;
            this.Number2 = fromArgs.Number2;
            this.Item = fromArgs.Item;
            this.String = fromArgs.String;
            this.CompletionType = fromArgs.CompletionType;
        }

        public NPC Npc { get; }

        public int Number1 { get; }

        public int Number2 { get; }

        public Item Item { get; }

        public string String { get; }

        public int CompletionType { get; }

        public string Name => "StardewValley.Quest:CompletionArgs";
    }
}

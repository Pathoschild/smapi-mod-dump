/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Messages
{
    public class CraftMessage : StoryMessage
    {
        public Item Item { get; }
        public bool Cooking { get; }

        public CraftMessage(Item item, bool cooking) : base("Craft")
        {
            this.Item = item;
            this.Cooking = cooking;
        }
    }
}

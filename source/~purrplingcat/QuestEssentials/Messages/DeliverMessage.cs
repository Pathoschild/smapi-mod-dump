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
    public class DeliverMessage : StoryMessage
    {
        public Farmer Who { get; }
        public NPC Npc { get; }
        public Item Item { get; }

        public DeliverMessage(Farmer who, NPC npc, Item item) : base("Deliver")
        {
            this.Who = who;
            this.Npc = npc;
            this.Item = item;
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Messages;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Messages
{
    internal class TalkMessage : ITalkMessage
    {
        public Farmer Farmer { get; }
        public NPC Npc { get; }

        public TalkMessage(Farmer farmer, NPC npc)
        {
            this.Farmer = farmer;
            this.Npc = npc;
        }
    }
}

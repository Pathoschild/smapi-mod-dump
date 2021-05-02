/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StardustCore.Events.Preconditions.NPCSpecific
{
    public class FriendshipPointsRequired:EventPrecondition
    {
        public NPC npc;
        public int amount;


        public FriendshipPointsRequired()
        {

        }

        public FriendshipPointsRequired(NPC NPC, int Amount)
        {
            this.npc = NPC;
            this.amount = Amount;
        }

        public override string ToString()
        {
            return this.precondition_FriendshipRequired();
        }

        /// <summary>
        /// Gets the amount of friedship points required for this event to occur.
        /// </summary>
        /// <returns></returns>
        public string precondition_FriendshipRequired()
        {
            StringBuilder b = new StringBuilder();
            b.Append("f ");
            b.Append(this.npc.Name);
            b.Append(" ");
            b.Append(this.amount.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            return Game1.player.friendshipData[this.npc.Name].Points >= this.amount;
        }
    }
}

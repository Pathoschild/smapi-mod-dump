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
    public class HeartsRequired:EventPrecondition
    {
        public NPC npc;
        public int amount;


        public HeartsRequired()
        {

        }

        public HeartsRequired(NPC NPC, int Amount)
        {
            this.npc = NPC;
            this.amount = Amount;
        }

        public override string ToString()
        {
            return this.precondition_HeartsRequired();
        }

        /// <summary>
        /// Gets the amount of hearts required for this event to occur.
        /// </summary>
        /// <returns></returns>
        public string precondition_HeartsRequired()
        {
            StringBuilder b = new StringBuilder();
            b.Append("f ");
            b.Append(this.npc.Name);
            b.Append(" ");
            int hearts = this.amount * 250;
            b.Append(hearts.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            int hearts = Game1.player.friendshipData[this.npc.Name].Points / 250;
            return  hearts >= this.amount;
        }
    }
}

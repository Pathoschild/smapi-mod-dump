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
    public class NotMarriedTo : EventPrecondition
    {

        public NPC npc;

        public NotMarriedTo()
        {

        }

        public NotMarriedTo(NPC npc)
        {
            this.npc = npc;
        }

        public override string ToString()
        {
            return this.precondition_playerNotMarriedToThisNPC();
        }

        /// <summary>
        /// Current player is not married to that NPC.
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public string precondition_playerNotMarriedToThisNPC()
        {
            StringBuilder b = new StringBuilder();
            b.Append("o ");
            b.Append(this.npc.Name);
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            if (Game1.player.getSpouse() == null) return true;
            if (Game1.player.getSpouse() == this.npc) return false;
            return true;
        }

    }
}

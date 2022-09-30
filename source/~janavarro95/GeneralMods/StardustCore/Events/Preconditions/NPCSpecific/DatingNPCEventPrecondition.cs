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

namespace Omegasis.StardustCore.Events.Preconditions.NPCSpecific
{
    public class DatingNPCEventPrecondition:EventPrecondition
    {
        public const string EventPreconditionId = "StardustCore.Events.Preconditions.NPCSpecific.DatingNPC";

        public string datingNpc;
        public DatingNPCEventPrecondition()
        {

        }

        public DatingNPCEventPrecondition(string datingNpcName)
        {
            this.datingNpc = datingNpcName;
        }

        public DatingNPCEventPrecondition(NPC npc)
        {
            this.datingNpc = npc.Name;
        }

        public override string ToString()
        {
            return EventPreconditionId + " " + this.datingNpc;
        }

        public override bool meetsCondition()
        {
            if (Game1.player.friendshipData.ContainsKey(this.datingNpc)){
                return Game1.player.friendshipData[this.datingNpc].IsDating();
            }
            else
            {
                return false;
            }
        }
    }
}

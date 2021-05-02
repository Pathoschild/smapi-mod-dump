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

namespace StardustCore.Events.Preconditions.PlayerSpecific
{
    public class NotDatingAnyone: EventPrecondition
    {
        public NotDatingAnyone()
        {

        }

        public override string ToString()
        {
            return "Omegasis.EventFramework.Precondition.NotDatingAnyone";
        }


        public override bool meetsCondition()
        {
            foreach(GameLocation loc in Game1.locations)
            {
                foreach (NPC npc in loc.getCharacters())
                {
                    if (Game1.player.friendshipData[npc.Name].IsDating()) return false;
                }
            }
            return true;
        }

    }
}

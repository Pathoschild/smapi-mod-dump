using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace CustomKissingMod.Api
{
    public class CustomKissingModApi : ICustomKissingModApi
    {
        public bool CanKissNpc(Farmer who, NPC npc)
        {
            return NPCOverrides.CanKissNpc(who, npc);
        }

        public bool HasRequiredFriendshipToKiss(Farmer who, NPC npc)
        {
            return NPCOverrides.HasRequiredFriendshipToKiss(who, npc);
        }
    }
}

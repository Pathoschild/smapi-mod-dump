using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace CustomKissingMod.Api
{
    public interface ICustomKissingModApi
    {
        bool CanKissNpc(Farmer who, NPC npc);

        bool HasRequiredFriendshipToKiss(Farmer who, NPC npc);
    }
}

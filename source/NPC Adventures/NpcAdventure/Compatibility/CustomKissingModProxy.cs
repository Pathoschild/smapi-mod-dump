using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Compatibility
{
    /// <summary>
    /// Proxy to Custom Kissing Mod by Digus for keep compatibility with this mod.
    /// </summary>
    internal class CustomKissingModProxy : ICustomKissingModApi
    {
        private readonly ICustomKissingModApi api;

        public CustomKissingModProxy(IModRegistry registry)
        {
            this.api = registry.GetApi<ICustomKissingModApi>("Digus.CustomKissingMod");
        }
        public bool CanKissNpc(Farmer who, NPC npc)
        {
            if (this.api != null)
            {
                return this.api.CanKissNpc(who, npc);
            }

            return false;
        }

        public bool HasRequiredFriendshipToKiss(Farmer who, NPC npc)
        {
            if (this.api != null)
            {
                return this.api.HasRequiredFriendshipToKiss(who, npc);
            }

            return false;
        }
    }

    public interface ICustomKissingModApi
    {
        bool CanKissNpc(Farmer who, NPC npc);
        bool HasRequiredFriendshipToKiss(Farmer who, NPC npc);
    }
}

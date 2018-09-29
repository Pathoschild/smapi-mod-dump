using Netcode;
using StardewValley;
using System.Collections.Generic;

namespace SelfServiceShop
{
    static class Extensions
    {
        public static NPC Find(this NetCollection<NPC> npcs, string name)
        {
            foreach (NPC npc in npcs)
            {
                if (npc.Name == name)
                    return npc;
            }

            return null;
        }

        public static GameLocation Find(this IList<GameLocation> locations, string name)
        {
            foreach (GameLocation location in locations)
            {
                if (location.Name == name)
                    return location;
            }
            return null;
        }
    }
}

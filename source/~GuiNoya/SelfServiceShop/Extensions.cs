/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GuiNoya/SVMods
**
*************************************************/

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

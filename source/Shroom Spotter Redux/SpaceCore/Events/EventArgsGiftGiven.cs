/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewValley;
using SObject = StardewValley.Object;

namespace SpaceCore.Events
{
    public class EventArgsGiftGiven
    {
        internal EventArgsGiftGiven(NPC npc, SObject o)
        {
            Npc = npc;
            Gift = o;
        }

        public NPC Npc { get; }
        public SObject Gift { get; }
    }
}

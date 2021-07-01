/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using SpaceShared;
using StardewValley;
using SObject = StardewValley.Object;

namespace SpaceCore.Events
{
    public class EventArgsBeforeReceiveObject : CancelableEventArgs
    {
        internal EventArgsBeforeReceiveObject(NPC npc, SObject o)
        {
            this.Npc = npc;
            this.Gift = o;
        }

        public NPC Npc { get; }
        public SObject Gift { get; }
    }
}

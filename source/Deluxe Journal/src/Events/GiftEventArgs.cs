/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;

namespace DeluxeJournal.Events
{
    public class GiftEventArgs : EventArgs
    {
        /// <summary>The NPC receiving the item.</summary>
        public NPC NPC { get; }

        /// <summary>The Item given to the NPC.</summary>
        public Item Item { get; }

        public GiftEventArgs(NPC npc, Item item)
        {
            NPC = npc;
            Item = item;
        }
    }
}

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
    public class ItemReceivedEventArgs : EventArgs
    {
        /// <summary>The player who received the item stack.</summary>
        public Farmer Player { get; }

        /// <summary>Item received.</summary>
        public Item Item { get; }

        /// <summary>Number of items received. May differ from Item.Stack if it was split between stacks or the inventory was full.</summary>
        public int Count { get; }

        public ItemReceivedEventArgs(Farmer player, Item item, int count)
        {
            Player = player;
            Item = item;
            Count = count;
        }
    }
}

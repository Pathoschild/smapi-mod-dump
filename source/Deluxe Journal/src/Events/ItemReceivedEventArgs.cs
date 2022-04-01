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
        /// <summary>Item received.</summary>
        public SObject Item { get; }

        /// <summary>Number of items received (equivalent to Item.Stack).</summary>
        public int Count { get; }

        public ItemReceivedEventArgs(SObject item, int count)
        {
            Item = item;
            Count = count;
        }
    }
}

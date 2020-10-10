/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using StardewValley;
using StardewValley.Objects;

namespace SortMyStuff.Framework
{
    internal class ItemContainer
    {
        /*********
        ** Accessors
        *********/
        public Chest Chest { get; set; }
        public Item Item { get; set; }


        /*********
        ** Public methods
        *********/
        public ItemContainer() { }

        public ItemContainer(Chest chest, Item item)
        {
            this.Chest = chest;
            this.Item = item;
        }
    }
}

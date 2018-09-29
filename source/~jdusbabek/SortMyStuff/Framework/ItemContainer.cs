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

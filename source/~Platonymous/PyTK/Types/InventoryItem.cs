/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;

namespace PyTK.Types
{
    /// <summary>An object that contains the price and stock of an item.</summary>
    public class InventoryItem
    {
        public Item item;
        public int price;
        public int stock;

        public InventoryItem(Item item, int price, int stock = int.MaxValue)
        {
            this.item = item;
            this.price = price;
            this.stock = stock;
        }

    }
}

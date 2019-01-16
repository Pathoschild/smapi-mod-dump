using System.Collections.Generic;
using StardewValley;

namespace FarmersMarketStall.Framework
{
    public class MarketStall
    {
        public List<Item> stock;

        public MarketStall() { }

        public void addItemToSell(Item item)
        {
            this.stock.Add(item);
        }

        public void removeItemFromStock(Item item)
        {
            this.stock.Remove(item);
        }

        public void sellAllItems()
        {
            foreach (var item in this.stock)
                Game1.player.money += (int)(item.salePrice() * 1.10f); //Replace the multiplier with some sort of level.

            this.stock.Clear();
        }
    }
}

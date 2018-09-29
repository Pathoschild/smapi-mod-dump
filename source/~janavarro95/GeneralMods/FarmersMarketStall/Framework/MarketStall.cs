using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmersMarketStall.Framework
{
    public class MarketStall
    {
        public List<Item> stock;

        public MarketStall()
        {

        }

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
            foreach(var item in stock)
            {
                Game1.player.money+=(int)(item.salePrice() * 1.10f); //Replace the multiplier with some sort of level.
            }
            this.stock.Clear();
        }

    }
}

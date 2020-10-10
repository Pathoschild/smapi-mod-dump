/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

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

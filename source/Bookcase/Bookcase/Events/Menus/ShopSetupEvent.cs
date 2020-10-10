/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System;

namespace Bookcase.Events {

    /// <summary>
    /// This event is fired after a shop has been setup.
    /// 
    /// This event can NOT be canceled.
    /// </summary>
    public class ShopSetupEvent : Event {

        /// <summary>
        /// Instance of the shop menu being opened.
        /// </summary>
        public ShopMenu Menu { get; private set; }

        /// <summary>
        /// The owner of the shop. This can be null.
        /// </summary>
        public String ShopOwner { get; private set; }

        /// <summary>
        /// Dictionary of items being sold by the shop. Can be null/empty.
        /// </summary>
        public Dictionary<Item, int[]> ItemsForSale { get; private set; }

        /// <summary>
        /// The types of items you can sell to the shop.
        /// </summary>
        public List<int> CategoriesToSell { get; private set; }

        /// <summary>
        /// List of items being sold by the shop.
        /// </summary>
        public List<Item> ForSale { get; private set; }

        public ShopSetupEvent(ShopMenu menu, string owner, Dictionary<Item, int[]> itemsForSale, List<int> categoriesToSellHere, List<Item> forSale) {

            this.Menu = menu;
            this.ShopOwner = owner;
            this.ItemsForSale = itemsForSale;
            this.CategoriesToSell = categoriesToSellHere;
            this.ForSale = forSale;
        }

        /// <summary>
        /// Adds an item to a shops inventory.
        /// </summary>
        /// <param name="item">The item to sell.</param>
        /// <param name="cost">The cost of the item.</param>
        /// <param name="amount">The amount of items in stock.</param>
        public void AddItemToShop(Item item, int cost, int amount = 2147483647) {

            this.ItemsForSale.Add(item, new int[] { cost, amount });
            this.ForSale.Add(item);
        }
    }
}

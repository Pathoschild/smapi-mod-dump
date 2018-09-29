using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StackSplitX.MenuHandlers
{
    class BuyAction : ShopAction
    {
        /// <summary>Default amount when shift+right clicking</summary>
        private const int DefaultShopStackAmount = 5;

        /// <summary>Constructs an instance.</summary>
        /// <param name="reflection">Reflection helper.</param>
        /// <param name="monitor">Monitor for logging.</param>
        /// <param name="menu">The native shop menu.</param>
        /// <param name="item">The item to buy.</param>
        public BuyAction(IReflectionHelper reflection, IMonitor monitor, ShopMenu menu, Item item)
            : base(reflection, monitor, menu, item)
        {
            // Default amount
            this.Amount = DefaultShopStackAmount;
        }

        /// <summary>Verifies the conditions to perform te action.</summary>
        public override bool CanPerformAction()
        {
            var heldItem = this.Reflection.GetField<Item>(this.NativeShopMenu, "heldItem").GetValue();
            int currentMonies = ShopMenu.getPlayerCurrencyAmount(Game1.player, this.ShopCurrencyType);

            return (this.ClickedItem != null && 
                    (heldItem == null || (this.ClickedItem.canStackWith(heldItem) && heldItem.Stack < heldItem.maximumStackSize())) && // Holding the same item and not hold max stack
                    this.ClickedItem.canStackWith(this.ClickedItem) && // Item type is stackable
                    currentMonies >= this.ClickedItem.salePrice()); // Can afford
        }

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked.</param>
        public override void PerformAction(int amount, Point clickLocation)
        {
            var heldItem = this.Reflection.GetField<Item>(this.NativeShopMenu, "heldItem").GetValue();
            var priceAndStockField = this.Reflection.GetField<Dictionary<Item, int[]>>(this.NativeShopMenu, "itemPriceAndStock");
            var priceAndStockMap = priceAndStockField.GetValue();
            Debug.Assert(priceAndStockMap.ContainsKey(this.ClickedItem));

            // Calculate the number to purchase
            int numInStock = priceAndStockMap[this.ClickedItem][1];
            int itemPrice = priceAndStockMap[this.ClickedItem][0];
            int currentMonies = ShopMenu.getPlayerCurrencyAmount(Game1.player, this.ShopCurrencyType);
            amount = Math.Min(Math.Min(amount, currentMonies / itemPrice), Math.Min(numInStock, this.ClickedItem.maximumStackSize()));

            // If we couldn't grab all that we wanted then only subtract the amount we were able to grab
            int numHeld = heldItem?.Stack ?? 0;
            int overflow = Math.Max((numHeld + amount) - this.ClickedItem.maximumStackSize(), 0);
            amount -= overflow;

            this.Monitor.DebugLog($"Attempting to purchase {amount} of {this.ClickedItem.Name} for {itemPrice * amount}");

            if (amount <= 0)
                return;

            // Try to purchase the item - method returns true if it should be removed from the shop since there's no more.
            var purchaseMethodInfo = this.Reflection.GetMethod(this.NativeShopMenu, "tryToPurchaseItem");
            int index = BuyAction.GetClickedItemIndex(this.Reflection, this.NativeShopMenu, clickLocation);
            if (purchaseMethodInfo.Invoke<bool>(this.ClickedItem, heldItem, amount, clickLocation.X, clickLocation.Y, index))
            {
                this.Monitor.DebugLog($"Purchase of {this.ClickedItem.Name} successful");

                // remove the purchased item from the stock etc.
                priceAndStockMap.Remove(this.ClickedItem);
                priceAndStockField.SetValue(priceAndStockMap);

                var itemsForSaleField = this.Reflection.GetField<List<Item>>(this.NativeShopMenu, "forSale");
                var itemsForSale = itemsForSaleField.GetValue();
                itemsForSale.Remove(this.ClickedItem);
                itemsForSaleField.SetValue(itemsForSale);
            }
        }

        /// <summary>Helper method getting which item in the shop was clicked.</summary>
        /// <param name="reflection">Reflection helper.</param>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="p">Mouse location.</param>
        /// <returns>The clicked item or null if none was clicked.</returns>
        public static Item GetClickedShopItem(IReflectionHelper reflection, ShopMenu shopMenu, Point p)
        {
            var itemsForSale = reflection.GetField<List<Item>>(shopMenu, "forSale").GetValue();
            int index = GetClickedItemIndex(reflection, shopMenu, p);
            Debug.Assert(index < itemsForSale.Count);
            return index >= 0 ? itemsForSale[index] : null;
        }

        /// <summary>Gets the index of the clicked shop item. This index corresponds to the list of buttons and list of items.</summary>
        /// <param name="reflection">Reflection helper.</param>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="p">Mouse location.</param>
        /// <returns>The clicked item or null if none was clicked.</returns>
        public static int GetClickedItemIndex(IReflectionHelper reflection, ShopMenu shopMenu, Point p)
        {
            int currentItemIndex = reflection.GetField<int>(shopMenu, "currentItemIndex").GetValue();
            int saleButtonIndex = shopMenu.forSaleButtons.FindIndex(button => button.containsPoint(p.X, p.Y));
            return saleButtonIndex > -1 ? currentItemIndex + saleButtonIndex : -1;
        }

        /// <summary>Creates an instance of the action.</summary>
        /// <param name="reflection">Reflection helper.</param>
        /// <param name="monitor">Monitor for logging.</param>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="mouse">Mouse position.</param>
        /// <returns>The instance or null if no valid item was selected.</returns>
        public new static ShopAction Create(IReflectionHelper reflection, IMonitor monitor, ShopMenu shopMenu, Point mouse)
        {
            var item = BuyAction.GetClickedShopItem(reflection, shopMenu, mouse);
            return item != null ? new BuyAction(reflection, monitor, shopMenu, item) : null;
        }
    }
}

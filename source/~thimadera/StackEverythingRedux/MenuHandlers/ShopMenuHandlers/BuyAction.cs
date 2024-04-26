/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Diagnostics;

namespace StackEverythingRedux.MenuHandlers.ShopMenuHandlers
{
    internal class BuyAction : ShopAction
    {
        private readonly Guid GUID = Guid.NewGuid();

        private bool? _CanPerformAction = null;
        private int? _MaxPurchasable = null;

        /// <summary>Constructs an instance.</summary>
        /// <param name="menu">The native shop menu.</param>
        /// <param name="item">The item to buy.</param>
        public BuyAction(ShopMenu menu, ISalable item)
            : base(menu, item)
        {
            // Default
            if (CanPerformAction())
            {
                Amount = Math.Min(StackEverythingRedux.Config.DefaultShopAmount, GetMaxPurchasable());
            }

            Log.TraceIfD($"[{nameof(BuyAction)}] Instantiated for shop {menu} item {item}, Amount = {Amount}, GUID = {GUID}");
        }

        ~BuyAction()
        {
            Log.TraceIfD($"[{nameof(BuyAction)}] Finalized for GUID = {GUID}");
        }

        /// <summary>Verifies the conditions to perform the action.</summary>
        public override bool CanPerformAction()
        {
            if (_CanPerformAction is null)
            {
                Item held = StackEverythingRedux.Reflection.GetField<Item>(NativeShopMenu, "heldItem").GetValue();

                _CanPerformAction =
                    ClickedItem is Item chosen    // not null
                    && chosen.canStackWith(chosen)     // Item type is stackable
                    && (
                        held == null                   // not holding anything, or...
                        || (chosen.canStackWith(held) && held.Stack < held.maximumStackSize())  // item held can stack with chosen and at max
                        )
                    && GetMaxPurchasable() > 0         // Can afford
                    ;
            }
            return _CanPerformAction.Value;
        }

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked.</param>
        public override void PerformAction(int amount, Point clickLocation)
        {
            string pfx = $"[{nameof(BuyAction)}.{nameof(PerformAction)}]";
            Item chosen = ClickedItem;
            int chosen_max = chosen.maximumStackSize();
            ShopMenu nativeMenu = NativeShopMenu;
            Item heldItem = StackEverythingRedux.Reflection.GetField<Item>(nativeMenu, "heldItem").GetValue();

            Log.Trace(
                $"{pfx} chosen = {chosen}, nativeMenu = {nativeMenu}, ShopCurrencyType = {ShopCurrencyType} ({ShopCurrencyName})"
                );

            // Using Linq here is slower by A LOT but ultimately MUCH more readable
            amount = new List<int>([amount, GetMaxPurchasable(), chosen_max]).Min();

            // If we couldn't grab all that we wanted then only subtract the amount we were able to grab
            int numHeld = heldItem?.Stack ?? 0;
            if (numHeld + amount > chosen_max)
            {
                amount = chosen_max - numHeld;
            }

            if (amount <= 0)
            {
                Log.Trace($"{pfx} purchasable amount <= 0, purchase aborted");
                return;
            }

            Log.Trace($"{pfx} Purchasing {amount} of {chosen.Name}");

            // Try to purchase the item - method returns true if it should be removed from the shop since there's no more.
            StardewModdingAPI.IReflectedMethod purchaseMethod = StackEverythingRedux.Reflection.GetMethod(nativeMenu, "tryToPurchaseItem");
            if (purchaseMethod.Invoke<bool>(chosen, heldItem, amount, clickLocation.X, clickLocation.Y))
            {
                Log.TraceIfD($"{pfx} Item is limited, reducing stock");
                // remove the purchased item from the stock etc.
                _ = nativeMenu.itemPriceAndStock.Remove(chosen);
                _ = nativeMenu.forSale.Remove(chosen);
            }
        }

        /// <summary>
        /// Determine how many of an item player can purchase based on player's current monies/inventories and shop's current stock
        /// </summary>
        /// <returns>Maximum amount purchasable, cached</returns>
        public int GetMaxPurchasable()
        {
            if (_MaxPurchasable is null)
            {
                string pfx = $"[{nameof(BuyAction)}.{nameof(GetMaxPurchasable)}]";

                Debug.Assert(ClickedItem is not null);
                Item chosen = ClickedItem;
                Dictionary<ISalable, ItemStockInformation> priceAndStockMap = NativeShopMenu.itemPriceAndStock;
                Debug.Assert(priceAndStockMap.ContainsKey(chosen));

                // Calculate the number to purchase
                ItemStockInformation stockData = priceAndStockMap[chosen];
                Log.Trace($"{pfx} chosen stockData = {string.Join(", ", stockData)}");
                int numInStock = stockData.Stock;
                int itemPrice = stockData.Price;
                int currentMonies;
                if (itemPrice > 0)
                {  // using money
                    currentMonies = ShopMenu.getPlayerCurrencyAmount(Game1.player, ShopCurrencyType);
                    Log.TraceIfD($"{pfx} player has {currentMonies} of currency {ShopCurrencyType} ({ShopCurrencyName})");
                }
                else
                {  // barter system. "monies" is now the wanted barter item in TradeItem
                    itemPrice = (int)stockData.TradeItemCount;
                    string barterItem = stockData.TradeItem;
                    currentMonies = Game1.player.Items.CountId(barterItem);
                    Log.TraceIfD($"{pfx} Barter system: player has {currentMonies} of item {barterItem}");
                }
                Log.Trace($"{pfx} chosen item price is {itemPrice}");
                Debug.Assert(itemPrice > 0);

                _MaxPurchasable = Math.Min(currentMonies / itemPrice, numInStock);
            }
            return _MaxPurchasable.Value;
        }

        /// <summary>Helper method getting which item in the shop was clicked.</summary>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="p">Mouse location.</param>
        /// <returns>The clicked item or null if none was clicked.</returns>
        public static ISalable GetClickedShopItem(ShopMenu shopMenu, Point p)
        {
            List<ISalable> itemsForSale = shopMenu.forSale;
            int index = GetClickedItemIndex(shopMenu, p);
            Debug.Assert(index < itemsForSale.Count);
            return index >= 0 ? itemsForSale[index] : null;
        }

        /// <summary>Gets the index of the clicked shop item. This index corresponds to the list of buttons and list of items.</summary>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="p">Mouse location.</param>
        /// <returns>The clicked item or null if none was clicked.</returns>
        public static int GetClickedItemIndex(ShopMenu shopMenu, Point p)
        {
            int currentItemIndex = shopMenu.currentItemIndex;
            int saleButtonIndex = shopMenu.forSaleButtons.FindIndex(button => button.containsPoint(p.X, p.Y));
            return saleButtonIndex > -1 ? currentItemIndex + saleButtonIndex : -1;
        }

        /// <summary>Creates an instance of the action.</summary>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="mouse">Mouse position.</param>
        /// <returns>The instance or null if no valid item was selected.</returns>
        public static ShopAction Create(ShopMenu shopMenu, Point mouse)
        {
            ISalable item = GetClickedShopItem(shopMenu, mouse);
            return item != null ? new BuyAction(shopMenu, item) : null;
        }
    }
}

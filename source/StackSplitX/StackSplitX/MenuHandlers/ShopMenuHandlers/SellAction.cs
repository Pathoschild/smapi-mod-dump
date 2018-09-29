using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StackSplitX.MenuHandlers
{
    public class SellAction : ShopAction
    {
        /// <summary>Constructs an instance.</summary>
        /// <param name="reflection">Reflection helper.</param>
        /// <param name="monitor">Monitor for logging.</param>
        /// <param name="menu">The native shop menu.</param>
        /// <param name="item">The item to buy.</param>
        public SellAction(IReflectionHelper reflection, IMonitor monitor, ShopMenu menu, Item item)
            : base(reflection, monitor, menu, item)
        {
            // Default amount
            this.Amount = (int)Math.Ceiling(this.ClickedItem.Stack / 2.0);
        }

        /// <summary>Verifies the conditions to perform te action.</summary>
        public override bool CanPerformAction()
        {
            return (this.NativeShopMenu.highlightItemToSell(this.ClickedItem) && this.ClickedItem.Stack > 1);
        }

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked.</param>
        public override void PerformAction(int amount, Point clickLocation)
        {
            amount = Math.Min(amount, this.ClickedItem.Stack);
            this.Amount = amount; // Remove if we don't need to carry this around

            // Sell item
            int price = CalculateSalePrice(this.ClickedItem, amount);
            ShopMenu.chargePlayer(Game1.player, this.ShopCurrencyType, price);
            this.Monitor.Log($"Charged player {price} for {amount} of {this.ClickedItem.Name}", LogLevel.Trace);

            // Update the stack amount/remove the item
            var actualInventory = this.Inventory.actualInventory;
            var index = actualInventory.IndexOf(this.ClickedItem);
            if (index >= 0 && index < actualInventory.Count)
            {
                int amountRemaining = this.ClickedItem.Stack - amount;
                if (amountRemaining > 0)
                    actualInventory[index].Stack = amountRemaining;
                else
                    actualInventory[index] = null;
            }

            Game1.playSound("purchaseClick");

            // The animation seems to only play when we sell 1
            if (amount == 1)
            {
                // Play the sell animation
                var animationsField = this.Reflection.GetField<List<TemporaryAnimatedSprite>>(this.NativeShopMenu, "animations");
                var animations = animationsField.GetValue();

                // Messy because it's a direct copy-paste from the source code
                Vector2 value = this.Inventory.snapToClickableComponent(clickLocation.X, clickLocation.Y);
                animations.Add(new TemporaryAnimatedSprite(Game1.debrisSpriteSheet, new Rectangle(Game1.random.Next(2) * Game1.tileSize, 256, Game1.tileSize, Game1.tileSize), 9999f, 1, 999, value + new Vector2(32f, 32f), false, false)
                {
                    alphaFade = 0.025f,
                    motion = Utility.getVelocityTowardPoint(new Point((int)value.X + 32, (int)value.Y + 32), Game1.dayTimeMoneyBox.position + new Vector2(96f, 196f), 12f),
                    acceleration = Utility.getVelocityTowardPoint(new Point((int)value.X + 32, (int)value.Y + 32), Game1.dayTimeMoneyBox.position + new Vector2(96f, 196f), 0.5f)
                });

                animationsField.SetValue(animations);
            }
        }

        // TODO: verify this is correct and Item.sellToShopPrice doesn't do the same thing
        /// <summary>Calculates the sale price of an item based on the algorithms used in the game source.</summary>
        /// <param name="item">Item to get the price for.</param>
        /// <param name="amount">Number being sold.</param>
        /// <returns>The sale price of the item * amount.</returns>
        private int CalculateSalePrice(Item item, int amount)
        {
            // Formula from ShopMenu.cs
            float sellPercentage = this.Reflection.GetField<float>(this.NativeShopMenu, "sellPercentage").GetValue();
            int price = 0;
            if (item is StardewValley.Object)
            {
                float sellPrice = (item as StardewValley.Object).sellToStorePrice();
                price = (int)(sellPrice * sellPercentage) * amount;
            }
            else
            {
                price = (int)(item.salePrice() * 0.5f * sellPercentage) * amount;
            }

            // Invert so we give the player money instead (shitty but it's what the game does).
            return -price;
        }

        /// <summary>Creates an instance of the action.</summary>
        /// <param name="reflection">Reflection helper.</param>
        /// <param name="monitor">Monitor for logging.</param>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="mouse">Mouse position.</param>
        /// <returns>The instance or null if no valid item was selected.</returns>
        public new static ShopAction Create(IReflectionHelper reflection, IMonitor monitor, ShopMenu shopMenu, Point mouse)
        {
            var inventory = shopMenu.inventory;
            var item = inventory.getItemAt(mouse.X, mouse.Y);
            return item != null ? new SellAction(reflection, monitor, shopMenu, item) : null;
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace StackSplitRedux.MenuHandlers
    {
    public class SellAction : ShopAction
        {
        private const int SMALL_TILE = Game1.smallestTileSize;
        private const int HALF_TILE = Game1.tileSize / 2;
        private const int FULL_TILE = Game1.tileSize;

        private readonly Guid GUID = Guid.NewGuid();

        /// <summary>Constructs an instance.</summary>
        /// <param name="menu">The native shop menu.</param>
        /// <param name="item">The item to buy.</param>
        public SellAction(ShopMenu menu, Item item)
            : base(menu, item) {
            // Default amount
            // +1 before /2 ensures we get the number rounded UP
            this.Amount = (this.ClickedItem.Stack + 1) / 2;
            Log.TraceIfD($"[{nameof(SellAction)}] Instantiated for shop {menu} item {item}, GUID = {GUID}");
            }

        ~SellAction() {
            Log.TraceIfD($"[{nameof(SellAction)}] Finalized for GUID = {GUID}");
            }

        /// <summary>Verifies the conditions to perform the action.</summary>
        public override bool CanPerformAction() {
            return (this.NativeShopMenu.highlightItemToSell(this.ClickedItem) && this.ClickedItem.Stack > 1);
            }

        /// <summary>Does the action.</summary>
        /// <param name="amount">Number of items.</param>
        /// <param name="clickLocation">Where the player clicked (i.e., when selecting item to split).</param>
        public override void PerformAction(int amount, Point clickLocation) {
            amount = Math.Min(amount, this.ClickedItem.Stack);
            this.Amount = amount; // Remove if we don't need to carry this around

            // Sell item
            int price = CalculateSalePrice(this.ClickedItem, amount);
            ShopMenu.chargePlayer(Game1.player, this.ShopCurrencyType, price);
            Log.Trace($"[{nameof(SellAction)}.{nameof(PerformAction)}] Charged player {price} for {amount} of {this.ClickedItem.Name}");

            // Update the stack amount/remove the item
            var actualInventory = this.InvMenu.actualInventory;
            var index = actualInventory.IndexOf(this.ClickedItem);
            if (index >= 0 && index < actualInventory.Count) {
                int amountRemaining = this.ClickedItem.Stack - amount;
                if (amountRemaining > 0)
                    actualInventory[index].Stack = amountRemaining;
                else
                    actualInventory[index] = null;
                }

            Game1.playSound("purchaseClick");
            if (amount < 1) return;
            Animate(amount, clickLocation);
            }

        /// <summary>
        /// Create animation of coins flying from the inventory slot to the shop moneybox
        /// </summary>
        /// <param name="amount">Number of items sold; determines the number of flying coins</param>
        /// <param name="clickLocation">Original click location (selection of item to sell).</param>
        public void Animate(int amount, Point clickLocation) {
            // This procedure is copied/adapted from game code
            // StardewValley.Menus.ShopMenu.receiveLeftClick

            // Might want to cap this ...
            int coins = (amount / 8) + 2;  // scale of "1/8" is from game code

            var animationsField = Mod.Reflection.GetField<List<TemporaryAnimatedSprite>>(this.NativeShopMenu, "animations");
            var animations = animationsField.GetValue();

            Vector2 snappedPosition = this.InvMenu.snapToClickableComponent(clickLocation.X, clickLocation.Y);

            var anim_pos = snappedPosition + new Vector2(HALF_TILE, HALF_TILE);

            var startingPoint = new Point((int)snappedPosition.X + HALF_TILE, (int)snappedPosition.Y + HALF_TILE);

            int pos_x = this.NativeShopMenu.xPositionOnScreen;
            int pos_y = this.NativeShopMenu.yPositionOnScreen;
            int height = this.NativeShopMenu.height;
            var endingPoint = new Vector2(pos_x - 36, pos_y + height - this.InvMenu.height - 16);

            var accel1 = new Vector2(0f, 0.5f);
            var accel2 = Utility.getVelocityTowardPoint(startingPoint, endingPoint, 0.5f);
            var motion2 = Utility.getVelocityTowardPoint(startingPoint, endingPoint, 8f);

            for (int j = 0; j < coins; j++) {
                animations.Add(
                    new TemporaryAnimatedSprite(
                        textureName: Game1.debrisSpriteSheetName,
                        sourceRect: new Rectangle(Game1.random.Next(2) * SMALL_TILE, FULL_TILE, SMALL_TILE, SMALL_TILE),
                        animationInterval: 9999f,
                        animationLength: 1,
                        numberOfLoops: 999,
                        position: anim_pos,
                        flicker: false,
                        flipped: false
                        ) {
                            alphaFade = 0.025f,
                            motion = new Vector2(Game1.random.Next(-3, 4), -4f),
                            acceleration = accel1,
                            delayBeforeAnimationStart = j * 25,
                            scale = 2f
                            }
                    );
                animations.Add(
                    new TemporaryAnimatedSprite(
                        textureName: Game1.debrisSpriteSheetName,
                        sourceRect: new Rectangle(Game1.random.Next(2) * SMALL_TILE, FULL_TILE, SMALL_TILE, SMALL_TILE),
                        animationInterval: 9999f,
                        animationLength: 1,
                        numberOfLoops: 999,
                        position: anim_pos,
                        flicker: false,
                        flipped: false
                        ) {
                            alphaFade = 0.025f,
                            motion = motion2,
                            acceleration = accel2,
                            delayBeforeAnimationStart = j * 50,
                            scale = 4f
                            }
                    );
                }  // end_for j

            // Not sure if this is needed, but just to be safe
            animationsField.SetValue(animations);
            }

        // TODO: verify this is correct and Item.sellToShopPrice doesn't do the same thing
        /// <summary>Calculates the sale price of an item based on the algorithms used in the game source.</summary>
        /// <param name="item">Item to get the price for.</param>
        /// <param name="amount">Number being sold.</param>
        /// <returns>The sale price of the item * amount.</returns>
        private int CalculateSalePrice(Item item, int amount) {
            // Formula from ShopMenu.cs
            float sellPercentage = Mod.Reflection.GetField<float>(this.NativeShopMenu, "sellPercentage").GetValue();

            float pricef = sellPercentage * amount;
            pricef *= item is StardewValley.Object sobj
                ? sobj.sellToStorePrice()
                : item.salePrice() * 0.5f
                ;

            // Invert so we give the player money instead (shitty but it's what the game does).
            return -(int)pricef;
            }

        /// <summary>Creates an instance of the action.</summary>
        /// <param name="shopMenu">Native shop menu.</param>
        /// <param name="mouse">Mouse position.</param>
        /// <returns>The instance or null if no valid item was selected.</returns>
        public static ShopAction Create(ShopMenu shopMenu, Point mouse) {
            var inventory = shopMenu.inventory;
            var item = inventory.getItemAt(mouse.X, mouse.Y);
            return item != null ? new SellAction(shopMenu, item) : null;
            }
        }
    }

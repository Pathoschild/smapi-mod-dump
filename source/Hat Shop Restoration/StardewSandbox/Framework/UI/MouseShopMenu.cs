/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/StardewSandbox
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace HatShopRestoration.Framework.UI
{
    internal class MouseShopMenu : ShopMenu
    {
        private const string CHEESE_ID = "(O)424";
        public MouseShopMenu(Dictionary<ISalable, ItemStockInformation> itemPriceAndStock, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null) : base("Hat Mouse", itemPriceAndStock: itemPriceAndStock, who: "Hat Mouse")
        {
            var unlockedHats = ModEntry.GetUnlockedHats(Game1.player);
            foreach (var pair in itemPriceAndStock)
            {
                var item = pair.Key;
                if (item is null || item is not Hat || unlockedHats.Contains(GetFashionableName(item.DisplayName)))
                {
                    itemPriceAndStock.Remove(item);
                    continue;
                }

                var stockInformation = pair.Value;
                stockInformation.Price = 1;
                stockInformation.Stock = 1;

                itemPriceAndStock[pair.Key] = stockInformation;
            }

            base.setItemPriceAndStock(itemPriceAndStock);
        }

        private string GetFashionableName(string hatName)
        {
            return $"Fashionable {hatName}";
        }

        private int GetCheeseInInventory(Farmer who)
        {
            int amountOfCheese = 0;
            foreach (var item in who.Items)
            {
                if (item is null || item.QualifiedItemId != CHEESE_ID)
                {
                    continue;
                }

                amountOfCheese++;
            }

            return amountOfCheese;
        }

        private bool tryToPurchaseItem(ISalable item, ISalable held_item, int numberToBuy, int x, int y, int indexInForSaleList)
        {
            if (this.readOnly)
            {
                return false;
            }
            if (held_item == null)
            {
                if (itemPriceAndStock[item].Stock == 0)
                {
                    this.hoveredItem = null;
                    return true;
                }
                if (item.GetSalableInstance().maximumStackSize() < numberToBuy)
                {
                    numberToBuy = Math.Max(1, item.GetSalableInstance().maximumStackSize());
                }
                int price2 = this.itemPriceAndStock[item].Price * numberToBuy;
                string extraTradeItem2 = null;
                int extraTradeItemCount2 = 5;
                if (this.itemPriceAndStock[item].TradeItem is not null)
                {
                    extraTradeItem2 = this.itemPriceAndStock[item].TradeItem;
                    if (this.itemPriceAndStock[item].TradeItemCount is not null)
                    {
                        extraTradeItemCount2 = this.itemPriceAndStock[item].TradeItemCount.Value;
                    }
                    extraTradeItemCount2 *= numberToBuy;
                }
                if (GetCheeseInInventory(Game1.player) >= price2 && (extraTradeItem2 is null || Game1.player.Items.ContainsId(extraTradeItem2, extraTradeItemCount2)))
                {
                    this.heldItem = null;

                    // Remove cheese from inventory
                    Game1.player.Items.ReduceId(CHEESE_ID, 1);

                    // Unlock the hat
                    ModEntry.UnlockFashionableHat(Game1.player, GetFashionableName(item.Name));

                    if (this.purchaseSound != null)
                    {
                        Game1.playSound(this.purchaseSound);
                    }

                    return true;
                }
                else
                {
                    Game1.playSound("cancel");
                }
            }
            else if (held_item.canStackWith(item))
            {
                numberToBuy = Math.Min(numberToBuy, held_item.maximumStackSize() - held_item.Stack);
                if (numberToBuy > 0)
                {
                    int price = this.itemPriceAndStock[item].Price * numberToBuy;
                    string extraTradeItem = null;
                    int extraTradeItemCount = 5;
                    if (this.itemPriceAndStock[item].TradeItem is not null)
                    {
                        extraTradeItem = this.itemPriceAndStock[item].TradeItem;
                        if (this.itemPriceAndStock[item].TradeItemCount is not null)
                        {
                            extraTradeItemCount = this.itemPriceAndStock[item].TradeItemCount.Value;
                        }
                        extraTradeItemCount *= numberToBuy;
                    }
                    int tmp = item.Stack;
                    item.Stack = numberToBuy + this.heldItem.Stack;
                    if (!item.CanBuyItem(Game1.player))
                    {
                        item.Stack = tmp;
                        Game1.playSound("cancel");
                        return false;
                    }

                    item.Stack = tmp;
                    if (GetCheeseInInventory(Game1.player) >= price && (extraTradeItem is null || Game1.player.Items.ContainsId(extraTradeItem, extraTradeItemCount)))
                    {
                        int amountAddedToStack = numberToBuy;
                        if (itemPriceAndStock[item].Stock == int.MaxValue && item.Stack != int.MaxValue)
                        {
                            amountAddedToStack *= item.Stack;
                        }
                        this.heldItem.Stack += amountAddedToStack;
                        if (itemPriceAndStock[item].Stock != int.MaxValue && !item.IsInfiniteStock())
                        {
                            itemPriceAndStock[item].ItemToSyncStack.Stack -= numberToBuy;
                            this.forSale[indexInForSaleList].Stack -= numberToBuy;
                        }
                        if (this.CanBuyback() && this.buyBackItems.Contains(item))
                        {
                            this.BuyBuybackItem(item, price, amountAddedToStack);
                        }

                        if (Game1.mouseClickPolling > 300)
                        {
                            if (this.purchaseRepeatSound != null)
                            {
                                Game1.playSound(this.purchaseRepeatSound);
                            }
                        }
                        else if (this.purchaseSound != null)
                        {
                            Game1.playSound(this.purchaseSound);
                        }
                        if (extraTradeItem is not null)
                        {
                            Game1.player.Items.ReduceId(extraTradeItem, extraTradeItemCount);
                        }
                        if (!this._isStorageShop && item.actionWhenPurchased(null))
                        {
                            this.heldItem = null;
                        }
                        if (this.onPurchase != null && this.onPurchase(item, Game1.player, numberToBuy))
                        {
                            base.exitThisMenu();
                        }
                    }
                    else
                    {
                        Game1.playSound("cancel");
                    }
                }
            }
            if (itemPriceAndStock[item].Stock <= 0)
            {
                if (this.buyBackItems.Contains(item))
                {
                    this.buyBackItems.Remove(item);
                }
                this.hoveredItem = null;
                return true;
            }
            return false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            for (int i = 0; i < this.forSaleButtons.Count; i++)
            {
                if (this.currentItemIndex + i >= this.forSale.Count || !this.forSaleButtons[i].containsPoint(x, y))
                {
                    continue;
                }

                int index = this.currentItemIndex + i;
                if (this.forSale[index] != null)
                {
                    int toBuy = ((!Game1.oldKBState.IsKeyDown(Keys.LeftShift)) ? 1 : Math.Min(Math.Min(Game1.oldKBState.IsKeyDown(Keys.LeftControl) ? 25 : 5, GetCheeseInInventory(Game1.player) / Math.Max(1, this.itemPriceAndStock[this.forSale[index]].Price)), Math.Max(1, this.itemPriceAndStock[this.forSale[index]].Stock)));
                    toBuy = Math.Min(toBuy, this.forSale[index].maximumStackSize());
                    if (toBuy == -1)
                    {
                        toBuy = 1;
                    }
                    if (this.canPurchaseCheck != null && !this.canPurchaseCheck(index))
                    {
                        return;
                    }
                    if (toBuy > 0 && this.tryToPurchaseItem(this.forSale[index], this.heldItem, toBuy, x, y, index))
                    {
                        this.itemPriceAndStock.Remove(this.forSale[index]);
                        this.forSale.RemoveAt(index);
                    }
                    else if (toBuy <= 0)
                    {
                        Game1.playSound("cancel");
                    }
                    if (this.heldItem != null && (this._isStorageShop || Game1.options.SnappyMenus || (Game1.oldKBState.IsKeyDown(Keys.LeftShift) && this.heldItem.maximumStackSize() == 1)) && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu && Game1.player.addItemToInventoryBool(this.heldItem as Item))
                    {
                        this.heldItem = null;
                        DelayedAction.playSoundAfterDelay("coin", 100);
                    }
                }
                this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));
                this.updateSaleButtonNeighbors();
                ModEntry.modHelper.Reflection.GetMethod(this, "setScrollBarToCurrentIndex").Invoke();
                return;
            }

            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            for (int i = 0; i < this.forSaleButtons.Count; i++)
            {
                if (this.currentItemIndex + i >= this.forSale.Count || !this.forSaleButtons[i].containsPoint(x, y))
                {
                    continue;
                }
                int index = this.currentItemIndex + i;
                if (this.forSale[index] == null)
                {
                    return;
                }
                int toBuy = 1;
                if (this.itemPriceAndStock[this.forSale[index]].Price > 0)
                {
                    toBuy = ((!Game1.oldKBState.IsKeyDown(Keys.LeftShift)) ? 1 : Math.Min(Math.Min(Game1.oldKBState.IsKeyDown(Keys.LeftControl) ? 25 : 5, GetCheeseInInventory(Game1.player) / this.itemPriceAndStock[this.forSale[index]].Price), this.itemPriceAndStock[this.forSale[index]].Stock));
                }
                if (this.canPurchaseCheck == null || this.canPurchaseCheck(index))
                {
                    if (toBuy > 0 && this.tryToPurchaseItem(this.forSale[index], this.heldItem, toBuy, x, y, index))
                    {
                        this.itemPriceAndStock.Remove(this.forSale[index]);
                        this.forSale.RemoveAt(index);
                    }
                    if (this.heldItem != null && (this._isStorageShop || Game1.options.SnappyMenus) && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ShopMenu && Game1.player.addItemToInventoryBool(this.heldItem as Item))
                    {
                        this.heldItem = null;
                        DelayedAction.playSoundAfterDelay("coin", 100);
                    }
                    ModEntry.modHelper.Reflection.GetMethod(this, "setScrollBarToCurrentIndex").Invoke();
                }
                return;
            }

            base.receiveRightClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }

            ShopCachedTheme theme = this.VisualTheme;
            Texture2D purchase_texture = Game1.mouseCursors;
            Rectangle purchase_window_border = new Rectangle(384, 373, 18, 18);
            Rectangle purchase_item_rect = new Rectangle(384, 396, 15, 15);
            Color? purchase_item_text_color = theme.ItemRowTextColor;
            bool purchase_draw_item_background = true;
            Rectangle purchase_item_background = new Rectangle(296, 363, 18, 18);
            Color purchase_selected_color = Color.Wheat;
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen + base.width - this.inventory.width - 32 - 24, base.yPositionOnScreen + base.height - 256 + 40, this.inventory.width + 56, base.height - 448 + 20, Color.White, 4f);
            IClickableMenu.drawTextureBox(b, purchase_texture, purchase_window_border, base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height - 256 + 32 + 4, Color.White, 4f);

            for (int k = 0; k < this.forSaleButtons.Count; k++)
            {
                if (this.currentItemIndex + k >= this.forSale.Count)
                {
                    continue;
                }
                bool failedCanPurchaseCheck = GetCheeseInInventory(Game1.player) == 0;
                if (this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + k))
                {
                    failedCanPurchaseCheck = true;
                }

                bool scrolling = ModEntry.modHelper.Reflection.GetField<bool>(this, "scrolling").GetValue();
                IClickableMenu.drawTextureBox(b, purchase_texture, purchase_item_rect, this.forSaleButtons[k].bounds.X, this.forSaleButtons[k].bounds.Y, this.forSaleButtons[k].bounds.Width, this.forSaleButtons[k].bounds.Height, (this.forSaleButtons[k].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !scrolling) ? purchase_selected_color : Color.White, 4f, drawShadow: false);
                ISalable item = this.forSale[this.currentItemIndex + k];
                bool buyInStacks = item.Stack > 1 && item.Stack != int.MaxValue && itemPriceAndStock[item].Stock == int.MaxValue;
                StackDrawType stackDrawType;

                if (itemPriceAndStock[item].Stock == int.MaxValue)
                {
                    stackDrawType = StackDrawType.HideButShowQuality;
                }
                else
                {
                    stackDrawType = StackDrawType.Draw_OneInclusive;
                    if (this._isStorageShop)
                    {
                        stackDrawType = StackDrawType.Draw;
                    }
                }
                string displayName = GetFashionableName(item.DisplayName);
                if (buyInStacks)
                {
                    displayName = displayName + " x" + item.Stack;
                }
                if (this.forSale[this.currentItemIndex + k].ShouldDrawIcon())
                {
                    if (purchase_draw_item_background)
                    {
                        b.Draw(purchase_texture, new Vector2(this.forSaleButtons[k].bounds.X + 32 - 12, this.forSaleButtons[k].bounds.Y + 24 - 4), purchase_item_background, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    }
                    this.forSale[this.currentItemIndex + k].drawInMenu(b, new Vector2(this.forSaleButtons[k].bounds.X + 32 - 8, this.forSaleButtons[k].bounds.Y + 24), 1f, 1f, 0.9f, stackDrawType, Color.White * ((!failedCanPurchaseCheck) ? 1f : 0.25f), drawShadow: true);
                    if (this.buyBackItems.Contains(this.forSale[this.currentItemIndex + k]))
                    {
                        b.Draw(Game1.mouseCursors2, new Vector2(this.forSaleButtons[k].bounds.X + 32 - 8, this.forSaleButtons[k].bounds.Y + 24), new Rectangle(64, 240, 16, 16), Color.White * ((!failedCanPurchaseCheck) ? 1f : 0.25f), 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 1f);
                    }
                    SpriteText.drawString(b, displayName, this.forSaleButtons[k].bounds.X + 96 + 8, this.forSaleButtons[k].bounds.Y + 28, 999999, -1, 999999, failedCanPurchaseCheck ? 0.5f : 1f, 0.88f, junimoText: false, -1, "", purchase_item_text_color);
                }
                else
                {
                    SpriteText.drawString(b, displayName, this.forSaleButtons[k].bounds.X + 32 + 8, this.forSaleButtons[k].bounds.Y + 28, 999999, -1, 999999, failedCanPurchaseCheck ? 0.5f : 1f, 0.88f, junimoText: false, -1, "", purchase_item_text_color);
                }
                if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].Price > 0)
                {
                    SpriteText.drawString(b, this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].Price + " ", this.forSaleButtons[k].bounds.Right - SpriteText.getWidthOfString(this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].Price + " ") - 60, this.forSaleButtons[k].bounds.Y + 28, 999999, -1, 999999, (GetCheeseInInventory(Game1.player) >= this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].Price && !failedCanPurchaseCheck) ? 1f : 0.5f, 0.88f, junimoText: false, -1, "", purchase_item_text_color);
                    Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2(this.forSaleButtons[k].bounds.Right - 78, this.forSaleButtons[k].bounds.Y + 10), new Rectangle(256, 272, 16, 16), Color.White * ((!failedCanPurchaseCheck) ? 1f : 0.25f), 0f, Vector2.Zero, 4f, flipped: false, 1f, -1, -1, (!failedCanPurchaseCheck) ? 0.35f : 0f);
                }
                else if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].TradeItem != null)
                {
                    int required_item_count = 5;
                    string requiredItem = this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].TradeItem;
                    if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].TradeItemCount.HasValue)
                    {
                        required_item_count = this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].TradeItemCount.Value;
                    }
                    bool hasEnoughToTrade = Game1.player.Items.ContainsId(requiredItem, required_item_count);
                    if (this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + k))
                    {
                        hasEnoughToTrade = false;
                    }
                    float textWidth = SpriteText.getWidthOfString("x" + required_item_count);
                    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(requiredItem);
                    Texture2D texture = dataOrErrorItem.GetTexture();
                    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
                    Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float)(this.forSaleButtons[k].bounds.Right - 88) - textWidth, this.forSaleButtons[k].bounds.Y + 28 - 4), sourceRect, Color.White * (hasEnoughToTrade ? 1f : 0.25f), 0f, Vector2.Zero, -1f, flipped: false, -1f, -1, -1, hasEnoughToTrade ? 0.35f : 0f);
                    SpriteText.drawString(b, "x" + required_item_count, this.forSaleButtons[k].bounds.Right - (int)textWidth - 16, this.forSaleButtons[k].bounds.Y + 44, 999999, -1, 999999, hasEnoughToTrade ? 1f : 0.5f, 0.88f, junimoText: false, -1, "", purchase_item_text_color);
                }
            }
            if (this.forSale.Count == 0 && !this._isStorageShop)
            {
                SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11583"), base.xPositionOnScreen + base.width / 2 - SpriteText.getWidthOfString(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11583")) / 2, base.yPositionOnScreen + base.height / 2 - 128);
            }
            this.inventory.draw(b);

            TemporaryAnimatedSpriteList animations = ModEntry.modHelper.Reflection.GetField<TemporaryAnimatedSpriteList>(this, "animations").GetValue();
            for (int j = animations.Count - 1; j >= 0; j--)
            {
                if (animations[j].update(Game1.currentGameTime))
                {
                    animations.RemoveAt(j);
                }
                else
                {
                    animations[j].draw(b, localPosition: true);
                }
            }

            TemporaryAnimatedSprite poof = ModEntry.modHelper.Reflection.GetField<TemporaryAnimatedSprite>(this, "poof").GetValue();
            if (poof != null)
            {
                poof.draw(b);
            }
            this.upArrow.draw(b);
            this.downArrow.draw(b);
            for (int i = 0; i < this.tabButtons.Count; i++)
            {
                this.tabButtons[i].draw(b);
            }

            Rectangle scrollBarRunner = ModEntry.modHelper.Reflection.GetField<Rectangle>(this, "scrollBarRunner").GetValue();
            if (this.forSale.Count > 4)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
                this.scrollBar.draw(b);
            }

            string hoverText = ModEntry.modHelper.Reflection.GetField<string>(this, "hoverText").GetValue();
            string boldTitleText = ModEntry.modHelper.Reflection.GetField<string>(this, "boldTitleText").GetValue();
            string hoveredItemExtraItemIndex = ModEntry.modHelper.Reflection.GetMethod(this, "getHoveredItemExtraItemIndex").Invoke<string>();
            if (!hoverText.Equals(""))
            {
                IClickableMenu.drawToolTip(b, $"{hoverText}\n\nEquipped via the Hand Mirror.", GetFashionableName(boldTitleText), this.hoveredItem as Item, this.heldItem != null, -1, 3, hoveredItemExtraItemIndex, -1, null, -1);
            }
            if (this.heldItem != null)
            {
                this.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f, 1f, 0.9f, StackDrawType.Draw, Color.White, drawShadow: true);
            }

            if (this.upperRightCloseButton != null && this.shouldDrawCloseButton())
            {
                this.upperRightCloseButton.draw(b);
            }

            int portrait_draw_position = base.xPositionOnScreen - 320;
            if (portrait_draw_position > 0 && Game1.options.showMerchantPortraits)
            {
                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(portrait_draw_position, base.yPositionOnScreen), new Rectangle(603, 414, 74, 74), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.91f);
                if (this.portraitTexture != null)
                {
                    b.Draw(this.portraitTexture, new Vector2(portrait_draw_position + 20, base.yPositionOnScreen + 20), new Rectangle(0, 0, 64, 64), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.92f);
                }
                if (this.potraitPersonDialogue != null)
                {
                    portrait_draw_position = base.xPositionOnScreen - (int)Game1.dialogueFont.MeasureString(this.potraitPersonDialogue).X - 64;
                    if (portrait_draw_position > 0)
                    {
                        IClickableMenu.drawHoverText(b, this.potraitPersonDialogue, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, null, -1, portrait_draw_position, base.yPositionOnScreen + ((this.portraitTexture != null) ? 312 : 0));
                    }
                }
            }
            base.drawMouse(b);
        }
    }
}

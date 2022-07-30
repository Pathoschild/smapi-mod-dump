/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Linq.Expressions;

using AtraBase.Toolkit.Reflection;
using AtraShared.Utils.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace PelicanTownFoodBank.Models;

internal class FoodBankMenu : ShopMenu
{
    private static readonly Lazy<Action<ShopMenu>> DownArrowPressedLazy = new(() =>
    {
        ParameterExpression instance = Expression.Parameter(typeof(ShopMenu), "instance");
        MethodCallExpression call = Expression.Call(
            instance,
            typeof(ShopMenu).InstanceMethodNamed("downArrowPressed"));
        return Expression.Lambda<Action<ShopMenu>>(call, instance).Compile();
    });

    private static readonly Lazy<Action<ShopMenu>> UpArrowPressedLazy = new(() =>
    {
        ParameterExpression? instance = Expression.Parameter(typeof(ShopMenu), "instance");
        MethodCallExpression call = Expression.Call(
            instance,
            typeof(ShopMenu).InstanceMethodNamed("upArrowPressed"));
        return Expression.Lambda<Action<ShopMenu>>(call, instance).Compile();
    });

    private static readonly Lazy<Action<ShopMenu, bool>> ScrollingSetterLazy = new(() =>
    {
        return typeof(ShopMenu).InstanceFieldNamed("scrolling").GetInstanceFieldSetter<ShopMenu, bool>()!;
    });

    private static Vector2? titleSize = null;

    private static Vector2 TitleSize
    {
        get => titleSize ?? Game1.dialogueFont.MeasureString(I18n.PelicanTownFoodBank()) + new Vector2(36, 24);
    }

    private static Action<ShopMenu> DownArrowPressed => DownArrowPressedLazy.Value;

    private static Action<ShopMenu> UpArrowPressed => UpArrowPressedLazy.Value;

    private static Action<ShopMenu, bool> ScrollingSetter => ScrollingSetterLazy.Value;

    public FoodBankMenu(Dictionary<ISalable, int[]> itemPriceAndStock, HashSet<ISalable> buyBacks)
        : base(itemPriceAndStock, currency: 0, who: null, on_purchase: null, on_sell: null, context: null)
    {
        this.Setup(buyBacks);
    }

    public FoodBankMenu(List<ISalable> itemsForSale, HashSet<ISalable> buyBacks)
        : base(itemsForSale, currency: 0, who: null, on_purchase: null, on_sell: null, context: null)
    {
        this.Setup(buyBacks);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) => this.receiveLeftClick(x, y, playSound);

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.upperRightCloseButton?.containsPoint(x, y) == true && this.readyToClose())
        {
            if (playSound)
            {
                Game1.playSound("bigDeSelect");
            }
            this.emergencyShutDown();
            this.exitThisMenu();
            return;
        }
        Vector2 snappedPos = this.inventory.snapToClickableComponent(x, y);
        this.currentItemIndex = Math.Clamp(this.currentItemIndex, 0, this.forSale.Count - 4);

        // Handle scrolling and arrows.
        if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
        {
            DownArrowPressed(this);
            if (playSound)
            {
                Game1.playSound("shwip");
            }
        }
        else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
        {
            UpArrowPressed(this);
            if (playSound)
            {
                Game1.playSound("shwip");
            }
        }
        else if (this.scrollBar.containsPoint(x,y))
        {
            ScrollingSetter(this, true);
        }
        else if (!this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
        {
            ScrollingSetter(this, true);
            this.leftClickHeld(x, y);
            this.releaseLeftClick(x, y);
        }

        if (this.heldItem is null && !this.readOnly)
        { // Handle buybacks.
            Item? toSell = this.inventory.leftClick(x, y, toPlace: null, playSound: false);
            int numberToSell = toSell is null ? 0 : Math.Min(toSell.Stack, MenuUtilities.GetIdealQuantityFromKeyboardState());
            if (toSell is SObject obj && numberToSell > 0)
            {
                ModEntry.ModMonitor.DebugOnlyLog($"{numberToSell} {obj.DisplayName} was sold to shop.", LogLevel.Info);
                toSell.Stack -= numberToSell;
                if (toSell.Stack > 0)
                {
                    this.inventory.leftClick(x, y, toSell, playSound: false);
                }

                Item? sellableInst = obj.getOne();
                sellableInst.Stack = numberToSell;

                this.buyBackItems.Add(sellableInst);
                this.forSale.Add(sellableInst);
                Game1.playSound("sell");
                Game1.playSound("purchase");
            }
        }
    }

    /// <summary>
    /// Called before closing the menu.
    /// </summary>
    public override void emergencyShutDown()
    {
        PantryStockManager.BuyBacks.Clear();
        foreach (ISalable item in this.buyBackItems)
        {
            PantryStockManager.BuyBacks.Add(item);
        }
        base.emergencyShutDown();
    }

    public override void draw(SpriteBatch b)
    {
        if (!Game1.options.showMenuBackground)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        }

        Texture2D purchase_texture = Game1.mouseCursors;
        Rectangle purchase_window_border = new Rectangle(384, 373, 18, 18);
        Rectangle purchase_item_rect = new Rectangle(384, 396, 15, 15);
        int purchase_item_text_color = -1;
        bool purchase_draw_item_background = true;
        Rectangle purchase_item_background = new Rectangle(296, 363, 18, 18);
        Color purchase_selected_color = Color.Wheat;

        IClickableMenu.drawTextureBox(
            b,
            texture: Game1.mouseCursors,
            sourceRect: new Rectangle(384, 373, 18, 18),
            x: this.xPositionOnScreen + this.width - this.inventory.width - 32 - 24,
            y: this.yPositionOnScreen + this.height - 256 + 40,
            width: this.inventory.width + 56,
            height: this.height - 428,
            color: Color.White,
            scale: 4f);

        IClickableMenu.drawTextureBox(
            b,
            texture: purchase_texture,
            sourceRect: purchase_window_border,
            x: this.xPositionOnScreen,
            y: this.yPositionOnScreen,
            width: this.width,
            height: this.height - 220,
            color: Color.White,
            scale: 4f);

        this.drawCurrency(b);

        for (int i = 0; i < this.forSaleButtons.Count; i++)
        {
            if (this.currentItemIndex + i >= this.forSale.Count)
            {
                break;
            }
            for (int k = 0; k < this.forSaleButtons.Count; k++)
            {
                if (this.currentItemIndex + k >= this.forSale.Count)
                {
                    continue;
                }
                bool failedCanPurchaseCheck = false;
                if (this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + k))
                {
                    failedCanPurchaseCheck = true;
                }
                IClickableMenu.drawTextureBox(b, purchase_texture, purchase_item_rect, this.forSaleButtons[k].bounds.X, this.forSaleButtons[k].bounds.Y, this.forSaleButtons[k].bounds.Width, this.forSaleButtons[k].bounds.Height, (this.forSaleButtons[k].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !this.scrolling) ? purchase_selected_color : Color.White, 4f, drawShadow: false);
                ISalable item = this.forSale[this.currentItemIndex + k];
                bool buyInStacks = item.Stack > 1 && item.Stack != int.MaxValue && this.itemPriceAndStock[item][1] == int.MaxValue;
                StackDrawType stackDrawType;
                if (this.itemPriceAndStock[item][1] == int.MaxValue)
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
                string displayName = item.DisplayName;
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
                if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]][0] > 0)
                {
                    SpriteText.drawString(b, this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]][0] + " ", this.forSaleButtons[k].bounds.Right - SpriteText.getWidthOfString(this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]][0] + " ") - 60, this.forSaleButtons[k].bounds.Y + 28, 999999, -1, 999999, (ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) >= this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]][0] && !failedCanPurchaseCheck) ? 1f : 0.5f, 0.88f, junimoText: false, -1, "", purchase_item_text_color);
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(this.forSaleButtons[k].bounds.Right - 52, this.forSaleButtons[k].bounds.Y + 40 - 4), new Rectangle(193 + this.currency * 9, 373, 9, 10), Color.White * ((!failedCanPurchaseCheck) ? 1f : 0.25f), 0f, Vector2.Zero, 4f, flipped: false, 1f, -1, -1, (!failedCanPurchaseCheck) ? 0.35f : 0f);
                }
                else if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].Length > 2)
                {
                    int required_item_count = 5;
                    int requiredItem = this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]][2];
                    if (this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]].Length > 3)
                    {
                        required_item_count = this.itemPriceAndStock[this.forSale[this.currentItemIndex + k]][3];
                    }
                    bool hasEnoughToTrade = Game1.player.hasItemInInventory(requiredItem, required_item_count);
                    if (this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + k))
                    {
                        hasEnoughToTrade = false;
                    }
                    float textWidth = SpriteText.getWidthOfString("x" + required_item_count);
                    Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float)(this.forSaleButtons[k].bounds.Right - 88) - textWidth, this.forSaleButtons[k].bounds.Y + 28 - 4), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, requiredItem, 16, 16), Color.White * (hasEnoughToTrade ? 1f : 0.25f), 0f, Vector2.Zero, -1f, flipped: false, -1f, -1, -1, hasEnoughToTrade ? 0.35f : 0f);
                    SpriteText.drawString(b, "x" + required_item_count, this.forSaleButtons[k].bounds.Right - (int)textWidth - 16, this.forSaleButtons[k].bounds.Y + 44, 999999, -1, 999999, hasEnoughToTrade ? 1f : 0.5f, 0.88f, junimoText: false, -1, "", purchase_item_text_color);
                }
            }

            this.inventory.draw(b);

            this.upArrow.draw(b);
            this.downArrow.draw(b);

            /****
            if (this.forSale.Count > 4)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
                this.scrollBar.draw(b);
            }
            if (!this.hoverText.Equals(""))
            {
                IClickableMenu.drawToolTip(b, this.hoverText, this.boldTitleText, this.hoveredItem as Item, this.heldItem != null, -1, this.currency, this.getHoveredItemExtraItemIndex(), this.getHoveredItemExtraItemAmount(), null, (this.hoverPrice > 0) ? this.hoverPrice : (-1));
            }
            if (this.heldItem is not null)
            {
                this.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f, 1f, 0.9f, StackDrawType.Draw, Color.White, drawShadow: true);
            }
            if (this.upperRightCloseButton != null && this.shouldDrawCloseButton())
            {
                this.upperRightCloseButton.draw(b);
            }
            ****/
            this.drawMouse(b);

            IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen - (int)TitleSize.Y - 12, (int)TitleSize.X, (int)TitleSize.Y, Color.White);
            Utility.drawTextWithShadow(b, I18n.PelicanTownFoodBank(), Game1.dialogueFont, new Vector2(this.xPositionOnScreen + 18, this.yPositionOnScreen - (int)TitleSize.Y), Color.Black);
        }
    }

    internal static void OnLocaleChange()
    {
        titleSize = null;
    }

    private void Setup(HashSet<ISalable> buyBacks)
    {
        this.buyBackItems = buyBacks;
        this._isStorageShop = true;
        this.categoriesToSellHere.AddRange(PantryStockManager.FoodBankCategories);
        this.onSell = this.OnSale;
        this.onPurchase = this.OnPurchase;
    }

    private bool OnSale(ISalable salable)
    {
        ModEntry.ModMonitor.Log(salable.DisplayName, LogLevel.Info);
        this.AddBuybackItem(salable, 0, salable.Stack);
        PantryStockManager.BuyBacks.Add(salable);
        Game1.playSound("sell");
        Game1.playSound("purchase");
        return true;
    }

    private bool OnPurchase(ISalable salable, Farmer who, int count)
    {
        ModEntry.ModMonitor.Log(salable.DisplayName, LogLevel.Info);
        if (salable is SObject obj && PantryStockManager.Sellables?.Contains(obj.ParentSheetIndex) == true)
        {
            PantryStockManager.Sellables.Remove(obj.ParentSheetIndex);
        }
        return false;
    }
}
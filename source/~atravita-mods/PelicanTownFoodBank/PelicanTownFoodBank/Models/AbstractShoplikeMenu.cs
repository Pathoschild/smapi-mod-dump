/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace PelicanTownFoodBank.Models;

internal abstract class AbstractShoplikeMenu : IClickableMenu
{
    public const int region_shopButtonModifier = 3546;
    public const int region_upArrow = 97865;
    public const int region_downArrow = 97866;

    public const int howManyRecipesFitOnPage = 28;

    public const int infiniteStock = int.MaxValue;

    public const int salePriceIndex = 0;

    public const int stockIndex = 1;

    public const int extraTradeItemIndex = 2;

    public const int extraTradeItemCountIndex = 3;

    public const int itemsPerPage = 4;

    public const int numberRequiredForExtraItemTrade = 5;

    private string descriptionText = string.Empty;

    private string hoverText = string.Empty;

    private string boldTitleText = string.Empty;

    public InventoryMenu inventory;

    public ISalable? heldItem;

    public ISalable? hoveredItem;

    private Rectangle scrollBarRunner;

    public List<ISalable> forSale = new List<ISalable>();

    public List<ClickableComponent> forSaleButtons = new List<ClickableComponent>();

    public List<int> categoriesToSellHere = new List<int>();

    private float sellPercentage = 1f;

    public int hoverPrice = -1;

    public int currentItemIndex;

    public ClickableTextureComponent upArrow;
    public ClickableTextureComponent downArrow;
    public ClickableTextureComponent scrollBar;

    private bool scrolling;

    public bool readOnly;

    public HashSet<ISalable> buyBackItems = new();

    public AbstractShoplikeMenu(List<ISalable> itemsForSale)
        : base(Game1.uiViewport.Width / 2 - (800 + borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + borderWidth * 2) / 2, 1000 + (borderWidth * 2), 600 + (borderWidth * 2), showUpperRightCloseButton: true)
    {
        this.forSale = itemsForSale;
        this.updatePosition();

        Game1.player.forceCanMove();
        Game1.playSound("dwop");

        this.ResetComponents();
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }
    }

    public void updateSaleButtonNeighbors()
    {
        ClickableComponent last_valid_button = this.forSaleButtons[0];
        for (int i = 0; i < this.forSaleButtons.Count; i++)
        {
            ClickableComponent button = this.forSaleButtons[i];
            button.upNeighborImmutable = true;
            button.downNeighborImmutable = true;
            button.upNeighborID = i + region_shopButtonModifier - 1;
            button.downNeighborID = i < Math.Min(this.forSale.Count, 3) ? i + region_shopButtonModifier + 1 : -7777;
            if (i >= this.forSale.Count)
            {
                if (button == this.currentlySnappedComponent)
                {
                    this.currentlySnappedComponent = last_valid_button;
                    if (Game1.options.SnappyMenus)
                    {
                        this.snapCursorToCurrentSnappedComponent();
                    }
                }
            }
            else
            {
                last_valid_button = button;
            }
        }
    }

    [UsedImplicitly]
    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
        switch (direction)
        {
            case 2:
            {
                if (this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
                {
                    this.DownArrowPressed();
                    break;
                }
                this.UpdateInventorySnapping(oldID);
                this.currentlySnappedComponent = this.getComponentWithID(this.GetEmptySlot());
                this.snapCursorToCurrentSnappedComponent();
                break;
            }
            case 0:
                if (this.currentItemIndex > 0)
                {
                    this.UpArrowPressed();
                    this.currentlySnappedComponent = this.getComponentWithID(3546);
                    this.snapCursorToCurrentSnappedComponent();
                }
                break;
        }
    }

    public override void snapToDefaultClickableComponent()
    {
        this.currentlySnappedComponent = this.getComponentWithID(3546);
        this.snapCursorToCurrentSnappedComponent();
    }

    public bool highlightItemToSell(Item i)
    {
        if (this.heldItem is not null)
        {
            return this.heldItem.canStackWith(i);
        }
        if (this.categoriesToSellHere.Contains(i.Category))
        {
            return true;
        }
        return false;
    }

    public override void leftClickHeld(int x, int y)
    {
        base.leftClickHeld(x, y);
        if (this.scrolling)
        {
            int y2 = this.scrollBar.bounds.Y;
            this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upArrow.bounds.Height + 20));
            float percentage = (y - this.scrollBarRunner.Y) / (float)this.scrollBarRunner.Height;
            this.currentItemIndex = Math.Min(Math.Max(0, this.forSale.Count - 4), Math.Max(0, (int)(this.forSale.Count * percentage)));
            this.setScrollBarToCurrentIndex();
            this.updateSaleButtonNeighbors();
            if (y2 != this.scrollBar.bounds.Y)
            {
                Game1.playSound("shiny4");
            }
        }
    }

    public override void releaseLeftClick(int x, int y)
    {
        base.releaseLeftClick(x, y);
        this.scrolling = false;
    }

    private void setScrollBarToCurrentIndex()
    {
        if (this.forSale.Count > 0)
        {
            this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.forSale.Count - 4 + 1) * this.currentItemIndex + this.upArrow.bounds.Bottom + 4;
            if (this.currentItemIndex == this.forSale.Count - 4)
            {
                this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
            }
        }
    }

    public override void receiveScrollWheelAction(int direction)
    {
        base.receiveScrollWheelAction(direction);
        if (direction > 0 && this.currentItemIndex > 0)
        {
            this.UpArrowPressed();
            Game1.playSound("shiny4");
        }
        else if (direction < 0 && this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
        {
            this.DownArrowPressed();
            Game1.playSound("shiny4");
        }
    }

    [UsedImplicitly]
    public override void receiveKeyPress(Keys key)
    {
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem is Item)
        {
            this.CollectOrDropHeldItem();
        }
        else
        {
            base.receiveKeyPress(key);
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y);
        if (Game1.activeClickableMenu is null)
        {
            return;
        }
        Vector2 snappedPosition = this.inventory.snapToClickableComponent(x, y);
        if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
        {
            this.DownArrowPressed();
            Game1.playSound("shwip");
        }
        else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
        {
            this.UpArrowPressed();
            Game1.playSound("shwip");
        }
        else if (this.scrollBar.containsPoint(x, y))
        {
            this.scrolling = true;
        }
        else if (!this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
        {
            this.scrolling = true;
            this.leftClickHeld(x, y);
            this.releaseLeftClick(x, y);
        }
        this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));
        if (this.heldItem == null && !this.readOnly)
        {
            Item toSell = this.inventory.leftClick(x, y, null, playSound: false);
            if (toSell is not null)
            {
                this.PerformByBackAction(toSell);
                this.updateSaleButtonNeighbors();
            }
        }
        else
        {
            this.heldItem = this.inventory.leftClick(x, y, this.heldItem as Item);
        }
        for (int i = 0; i < this.forSaleButtons.Count; i++)
        {
            if (this.currentItemIndex + i >= this.forSale.Count || !this.forSaleButtons[i].containsPoint(x, y))
            {
                continue;
            }
            int index = this.currentItemIndex + i;
            if (this.forSale[index] is ISalable objectToBuy)
            {
                int toBuy = Math.Min(MenuUtilities.GetIdealQuantityFromKeyboardState(), Math.Max(objectToBuy.maximumStackSize(), 1));

                this.PerformSellAction(this.forSale[index], this.heldItem, toBuy, x, y, index);

                if (this.heldItem is Item item && Game1.player.addItemToInventoryBool(item))
                {
                    this.heldItem = null;
                    DelayedAction.playSoundAfterDelay("coin", 100);
                }
            }
            this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));
            this.updateSaleButtonNeighbors();
            this.setScrollBarToCurrentIndex();
            return;
        }
        // Clicked outside the menu.
        if (this.readyToClose() && (x < this.xPositionOnScreen - 64 || y < this.yPositionOnScreen - 64 || x > this.xPositionOnScreen + this.width + 128 || y > this.yPositionOnScreen + this.height + 64))
        {
            this.exitThisMenu();
        }
    }

    public virtual bool CanBuyback() => true;

    public override bool readyToClose()
        => this.heldItem is null;

    public override void emergencyShutDown()
    {
        base.emergencyShutDown();
        if (this.heldItem is not null)
        {
            Game1.player.addItemToInventoryBool(this.heldItem as Item);
            Game1.playSound("coin");
        }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
        => this.receiveLeftClick(x, y, playSound);

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
        this.descriptionText = string.Empty;
        this.hoverText = string.Empty;
        this.hoveredItem = null;
        this.hoverPrice = -1;
        this.boldTitleText = string.Empty;
        this.upArrow.tryHover(x, y);
        this.downArrow.tryHover(x, y);
        this.scrollBar.tryHover(x, y);
        if (this.scrolling)
        {
            return;
        }
        for (int i = 0; i < this.forSaleButtons.Count; i++)
        {
            if (this.currentItemIndex + i < this.forSale.Count && this.forSaleButtons[i].containsPoint(x, y))
            {
                this.hoveredItem = this.forSale[this.currentItemIndex + i];

                this.hoverText = this.hoveredItem.getDescription();
                this.boldTitleText = this.hoveredItem.DisplayName;
                this.forSaleButtons[i].scale = Math.Min(this.forSaleButtons[i].scale + 0.03f, 1.1f);
            }
            else
            {
                this.forSaleButtons[i].scale = Math.Max(1f, this.forSaleButtons[i].scale - 0.03f);
            }
        }
        if (this.heldItem is not null)
        {
            return;
        }
        foreach (ClickableComponent c in this.inventory.inventory)
        {
            if (!c.containsPoint(x, y))
            {
                continue;
            }
            Item j = this.inventory.getItemFromClickableComponent(c);
            if (j == null || (this.inventory.highlightMethod != null && !this.inventory.highlightMethod(j)))
            {
                continue;
            }

            this.hoverText = j.getDescription();
            this.boldTitleText = j.DisplayName;
            this.hoveredItem = j;
            break;
        }
    }

    public override void receiveGamePadButton(Buttons b)
    {
        base.receiveGamePadButton(b);
        if (b != Buttons.RightTrigger && b != Buttons.LeftTrigger)
        {
            return;
        }
        if (this.currentlySnappedComponent is not null && this.currentlySnappedComponent.myID >= 3546)
        {

            this.UpdateInventorySnapping(region_shopButtonModifier + this.forSaleButtons.Count - 1);
            this.currentlySnappedComponent = this.getComponentWithID(this.GetEmptySlot());
            this.snapCursorToCurrentSnappedComponent();
        }
        else
        {
            this.snapToDefaultClickableComponent();
        }
        Game1.playSound("shiny4");
    }

    public void updatePosition()
    {
        this.width = 1000 + (borderWidth * 2);
        this.height = 600 + (borderWidth * 2);
        this.xPositionOnScreen = (Game1.uiViewport.Width - this.width) / 2;
        this.yPositionOnScreen = (Game1.uiViewport.Height - this.height) / 2;
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => this.ResetComponents();

    public override void draw(SpriteBatch b)
    {
        if (!Game1.options.showMenuBackground)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        }
        Texture2D purchase_texture = Game1.mouseCursors;
        Rectangle purchase_window_border = new(384, 373, 18, 18);
        Rectangle purchase_item_rect = new(384, 396, 15, 15);
        int purchase_item_text_color = -1;
        Rectangle purchase_item_background = new(296, 363, 18, 18);
        Color purchase_selected_color = Color.Wheat;

        drawTextureBox(
            b,
            texture: purchase_texture,
            sourceRect: purchase_window_border,
            x: this.xPositionOnScreen + this.width - this.inventory.width - 32 - 24,
            y: this.yPositionOnScreen + this.height - 256 + 40,
            width: this.inventory.width + 56,
            height: this.height - 448 + 20,
            color: Color.White,
            scale: 4f);

        drawTextureBox(
            b,
            texture: purchase_texture,
            sourceRect: purchase_window_border,
            x: this.xPositionOnScreen,
            y: this.yPositionOnScreen,
            width: this.width,
            height: this.height - 256 + 32 + 4,
            color: Color.White,
            scale: 4f);

        for (int k = 0; k < this.forSaleButtons.Count; k++)
        {
            if (this.currentItemIndex + k >= this.forSale.Count)
            {
                break;
            }

            drawTextureBox(
                b,
                texture: purchase_texture,
                sourceRect: purchase_item_rect,
                x: this.forSaleButtons[k].bounds.X,
                y: this.forSaleButtons[k].bounds.Y,
                width: this.forSaleButtons[k].bounds.Width,
                height: this.forSaleButtons[k].bounds.Height,
                color: (this.forSaleButtons[k].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !this.scrolling) ? purchase_selected_color : Color.White,
                scale: 4f,
                drawShadow: false);

            ISalable item = this.forSale[this.currentItemIndex + k];

            string displayName = (item.Stack > 1 && item.Stack != int.MaxValue) ? item.DisplayName : $"{item.DisplayName} x{item.Stack}";

            if (item.ShouldDrawIcon())
            {
                b.Draw(
                    texture: purchase_texture,
                    position: new Vector2(this.forSaleButtons[k].bounds.X + 32 - 12, this.forSaleButtons[k].bounds.Y + 24 - 4),
                    sourceRectangle: purchase_item_background,
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: 1f);

                this.forSale[this.currentItemIndex + k]
                    .drawInMenu(
                        b,
                        location: new Vector2(this.forSaleButtons[k].bounds.X + 32 - 8, this.forSaleButtons[k].bounds.Y + 24),
                        scaleSize: 1f,
                        transparency: 1f,
                        layerDepth: 0.9f,
                        drawStackNumber: StackDrawType.Draw,
                        color: Color.White,
                        drawShadow: true);
                if (this.buyBackItems.Contains(item))
                {
                    b.Draw(
                        texture: Game1.mouseCursors2,
                        position: new Vector2(this.forSaleButtons[k].bounds.X + 32 - 8, this.forSaleButtons[k].bounds.Y + 24),
                        sourceRectangle: new Rectangle(64, 240, 16, 16),
                        color: Color.White,
                        rotation: 0f,
                        origin: new Vector2(8f, 8f),
                        scale: 4f,
                        effects: SpriteEffects.None,
                        layerDepth: 1f);
                }
            }
            SpriteText.drawString(
                b,
                displayName,
                this.forSaleButtons[k].bounds.X + 32 + 8,
                this.forSaleButtons[k].bounds.Y + 28,
                color: purchase_item_text_color);
            this.drawPrice(this.forSaleButtons[k], item);
        }

        this.inventory.draw(b);
        this.upArrow.draw(b);
        this.downArrow.draw(b);

        if (this.forSale.Count > 4)
        { // draw scrollbar
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
            this.scrollBar.draw(b);
        }
        if (!string.IsNullOrWhiteSpace(this.hoverText))
        {
            drawToolTip(
                b,
                hoverText: this.hoverText,
                hoverTitle: this.boldTitleText,
                hoveredItem: this.hoveredItem as Item,
                heldItem: this.heldItem is not null);
        }
        this.heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f, 1f, 0.9f, StackDrawType.Draw, Color.White, drawShadow: true);

        this.draw(b);
        this.drawMouse(b);
    }

    protected virtual void drawPrice(ClickableComponent button, ISalable item)
    {
    }

    protected virtual int getPrice(ISalable item) => 0;

    public virtual int ChargePlayer(Farmer who, ISalable item, int quant)
    {
        return 0;
    }

    protected virtual int PlayerCanAfford(Farmer who, ISalable item)
        => int.MaxValue;

    protected virtual void RemoveFromShopStock(int index, int amount)
    {
        this.forSale[index].Stack -= amount;
        if (this.forSale[index].Stack <= 0)
        {
            this.forSale.RemoveAt(index);
        }
    }

    protected virtual int PerformSellAction(ISalable item, ISalable? held_item, int numberToBuy, int x, int y, int indexInForSaleList)
    {
        if (this.readOnly)
        {
            return 0;
        }
        if (held_item is null || (held_item.canStackWith(item) && held_item.Stack <= held_item.maximumStackSize()))
        {
            int maxpurchase = item.GetSalableInstance().maximumStackSize();
            if (held_item is not null)
            {
                maxpurchase -= held_item.Stack;
            }
            maxpurchase = Math.Clamp(maxpurchase, 1, 999);
            int stack = Math.Min(this.PlayerCanAfford(Game1.player, item), maxpurchase);
            if (stack > 0)
            {
                if (this.heldItem is null)
                {
                    this.heldItem = item.GetSalableInstance();
                    this.heldItem.Stack = stack;
                }
                else
                {
                    this.heldItem.Stack += stack;
                }

                if (!this.heldItem.CanBuyItem(Game1.player))
                {
                    Game1.playSound("smallSelect");
                    this.heldItem.Stack += stack;
                    if (this.heldItem.Stack <= 0)
                    {
                        this.heldItem = null;
                    }
                    return 0;
                }

                int price = this.ChargePlayer(Game1.player, item, stack);
                if (!item.IsInfiniteStock())
                {
                    this.RemoveFromShopStock(itemSpotIndex, stack);
                }

                if (this.CanBuyback() && this.buyBackItems.Contains(item))
                {
                    this.HandleBuyBuybackItem(item, stack, price);
                }
                return stack;
            }
            else
            {
                Game1.playSound("cancel");
            }
        }
        return 0;
    }

    protected virtual ISalable? AddBuybackItem(ISalable sold_item, int sell_unit_price, int stack)
    {
        ISalable? target = null;
        while (stack > 0)
        {
            target = null;
            foreach (ISalable buyback_item in this.buyBackItems)
            {
                if (buyback_item.canStackWith(sold_item) && buyback_item.Stack < buyback_item.maximumStackSize())
                {
                    target = buyback_item;
                    break;
                }
            }
            if (target == null)
            {
                target = sold_item.GetSalableInstance();
                int amount_to_deposit2 = Math.Min(stack, target.maximumStackSize());
                this.buyBackItems.Add(target);
                target.Stack = amount_to_deposit2;
                stack -= amount_to_deposit2;
            }
            else
            {
                int amount_to_deposit = Math.Min(stack, target.maximumStackSize() - target.Stack);
                int[] stock_data = this.itemPriceAndStock[target];
                stock_data[1] += amount_to_deposit;
                this.itemPriceAndStock[target] = stock_data;
                target.Stack = stock_data[1];
                stack -= amount_to_deposit;
            }
        }
        return target;
    }

    protected virtual void PerformByBackAction(Item toSell)
    {
        throw new NotImplementedException();
    }

    protected virtual void HandleBuyBuybackItem(ISalable item, int stack, int total_price)
    {
    }

    private void CollectOrDropHeldItem()
    {
        if (this.heldItem is Item item)
        {
            this.heldItem = null;
            if (Utility.CollectOrDrop(item))
            {
                Game1.playSound("stoneStep");
            }
            else
            {
                Game1.playSound("throwDownITem");
            }
        }
    }

    private void UpdateInventorySnapping(int oldID)
    {
        foreach (ClickableComponent slot in this.inventory.inventory)
        {
            slot.upNeighborID = oldID;
        }
    }

    private int GetEmptySlot()
    {
        if (this.heldItem is null || this.inventory.actualInventory is null)
        {
            return 0;
        }

        for (int i = 0; i < 12; i++)
        {
            if (this.inventory.actualInventory.Count > i && this.inventory.actualInventory[i] == null)
            {
                return i;
            }
        }
        return 0;
    }

    [MemberNotNull("downArrow", "upArrow", "inventory", "scrollBar")]
    private void ResetComponents()
    {
        this.updatePosition();
        this.initializeUpperRightCloseButton();
        Game1.player.forceCanMove();
        this.inventory = new(
            xPosition: this.xPositionOnScreen + this.width,
            yPosition: this.yPositionOnScreen + spaceToClearTopBorder + borderWidth + 320 + 40,
            playerInventory: false,
            actualInventory: null,
            highlightMethod: this.highlightItemToSell)
        {
            showGrayedOutSlots = true,
        };
        this.inventory.movePosition(-this.inventory.width - 32, 0);

        this.upArrow = new(
            bounds: new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + 16, 44, 48),
            texture: Game1.mouseCursors,
            sourceRect: new Rectangle(421, 459, 11, 12),
            scale: 4f)
        {
            myID = 97865,
            downNeighborID = 106,
            leftNeighborID = 3546,
        };

        this.downArrow = new(
            bounds: new Rectangle(this.xPositionOnScreen + this.width + 16, this.yPositionOnScreen + this.height - 64, 44, 48),
            texture: Game1.mouseCursors,
            sourceRect: new Rectangle(421, 472, 11, 12),
            scale: 4f)
        {
            myID = 106,
            upNeighborID = 97865,
            leftNeighborID = 3546,
        };

        this.scrollBar = new(
                new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40),
                Game1.mouseCursors,
                new Rectangle(435, 463, 6, 10),
                4f);

        this.scrollBarRunner = new Rectangle(
            this.scrollBar.bounds.X,
            this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4,
            this.scrollBar.bounds.Width,
            this.height - 64 - this.upArrow.bounds.Height - 28);

        this.forSaleButtons.Clear();
        for (int i = 0; i < 4; i++)
        {
            this.forSaleButtons.Add(
                new ClickableComponent(
                    new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + i * ((this.height - 256) / 4), this.width - 32, (this.height - 256) / 4 + 4),
                    i.ToString()));
        }
        foreach (ClickableComponent item in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
        {
            item.upNeighborID = -99998;
        }
    }

    private void DownArrowPressed()
    {
        this.downArrow.scale = this.downArrow.baseScale;
        this.currentItemIndex++;
        this.setScrollBarToCurrentIndex();
        this.updateSaleButtonNeighbors();
    }

    private void UpArrowPressed()
    {
        this.upArrow.scale = this.upArrow.baseScale;
        this.currentItemIndex--;
        this.setScrollBarToCurrentIndex();
        this.updateSaleButtonNeighbors();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace PelicanFiber.Framework
{
    internal class ShopMenu2 : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        private string HoverText = "";
        private string BoldTitleText = "";
        private readonly List<Item> ForSale = new List<Item>();
        private readonly List<ClickableComponent> ForSaleButtons = new List<ClickableComponent>();
        private readonly List<int> CategoriesToSellHere = new List<int>();
        private readonly Dictionary<Item, int[]> ItemPriceAndStock = new Dictionary<Item, int[]>();
        private float SellPercentage = 1f;
        private readonly List<TemporaryAnimatedSprite> Animations = new List<TemporaryAnimatedSprite>();
        private int HoverPrice = -1;
        private const int ItemsPerPage = 4;
        private InventoryMenu Inventory;
        private Item HeldItem;
        private Item HoveredItem;
        private TemporaryAnimatedSprite Poof;
        private Rectangle ScrollBarRunner;
        private readonly int Currency;
        private int CurrentItemIndex;
        private ClickableTextureComponent UpArrow;
        private ClickableTextureComponent DownArrow;
        private ClickableTextureComponent ScrollBar;
        private readonly ClickableTextureComponent BackButton;

        private NPC PortraitPerson;
        private string PortraitPersonDialogue;
        private bool Scrolling;
        private readonly string LocationName;

        private readonly Action ShowMainMenu;
        private readonly ItemUtils ItemUtils;
        private readonly bool GiveAchievements;

        /*********
        ** Public methods
        *********/
        public ShopMenu2(Action showMainMenu, ItemUtils itemUtils, bool giveAchievements, Dictionary<Item, int[]> itemPriceAndStock, int currency = 0, string who = null, string locationName = "")
          : this(showMainMenu, itemUtils, giveAchievements, itemPriceAndStock.Keys.ToList(), currency, who, locationName)
        {
            this.LocationName = locationName;
            this.ItemPriceAndStock = itemPriceAndStock;
            if (this.PortraitPersonDialogue != null)
                return;
            this.SetUpShopOwner(who);
        }

        public ShopMenu2(Action showMainMenu, ItemUtils itemUtils, bool giveAchievements, List<Item> itemsForSale, int currency = 0, string who = null, string locationName = "")
          : base(Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 1000 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true)
        {
            this.ShowMainMenu = showMainMenu;
            this.ItemUtils = itemUtils;
            this.GiveAchievements = giveAchievements;

            this.LocationName = locationName;
            this.Currency = currency;
            if (Game1.viewport.Width < 1500)
                this.xPositionOnScreen = Game1.tileSize / 2;
            Game1.player.forceCanMove();
            Game1.playSound("dwop");
            this.Inventory = new InventoryMenu(this.xPositionOnScreen + this.width, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 + Game1.pixelZoom * 10, false, null, this.HighlightItemToSell)
            {
                showGrayedOutSlots = true
            };
            this.Inventory.movePosition(-this.Inventory.width - Game1.tileSize / 2, 0);
            this.Currency = currency;
            this.UpArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            this.DownArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            this.ScrollBar = new ClickableTextureComponent(new Rectangle(this.UpArrow.bounds.X + Game1.pixelZoom * 3, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            this.BackButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom);
            this.ScrollBarRunner = new Rectangle(this.ScrollBar.bounds.X, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, this.ScrollBar.bounds.Width, this.height - Game1.tileSize - this.UpArrow.bounds.Height - Game1.pixelZoom * 7);
            for (int index = 0; index < ShopMenu2.ItemsPerPage; ++index)
                this.ForSaleButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4 + index * ((this.height - Game1.tileSize * 4) / 4), this.width - Game1.tileSize / 2, (this.height - Game1.tileSize * 4) / 4 + Game1.pixelZoom), string.Concat(index)));
            foreach (Item key in itemsForSale)
            {
                //if (key is StardewValley.Object && (key as StardewValley.Object).isRecipe || (key as StardewValley.Object).category == -425)
                if (key is Object && (key as Object).IsRecipe || locationName.Equals("Junimo"))
                {
                    if (!Game1.player.knowsRecipe(key.Name))
                        key.Stack = 1;
                    else
                        continue;
                }
                this.ForSale.Add(key);
                this.ItemPriceAndStock.Add(key, new[]
                {
          key.salePrice(),
          key.Stack
                });
            }
            if (this.ItemPriceAndStock.Count >= 2)
                this.SetUpShopOwner(who);
            switch (this.LocationName)
            {
                case "SeedShop":
                    this.CategoriesToSellHere.AddRange(new[]
                    {
                        -81,
                        -75,
                        -79,
                        -80,
                        -74,
                        -17,
                        -18,
                        -6,
                        -26,
                        -5,
                        -14,
                        -19,
                        -7,
                        -25
                    });
                    break;
                case "Blacksmith":
                    this.CategoriesToSellHere.AddRange(new[] { -12, -2, -15 });
                    break;
                case "ScienceHouse":
                    this.CategoriesToSellHere.AddRange(new[] { -16 });
                    break;
                case "AnimalShop":
                    this.CategoriesToSellHere.AddRange(new[] { -18, -6, -5, -14 });
                    break;
                case "FishShop":
                    this.CategoriesToSellHere.AddRange(new[]
                    {
                        -4,
                        -23,
                        -21,
                        -22
                    });
                    break;
                case "AdventureGuild":
                    this.CategoriesToSellHere.AddRange(new[]
                    {
                        -28,
                        -98,
                        -97,
                        -96
                    });
                    break;
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (!this.Scrolling)
                return;
            int y1 = this.ScrollBar.bounds.Y;
            this.ScrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - Game1.tileSize - Game1.pixelZoom * 3 - this.ScrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.UpArrow.bounds.Height + Game1.pixelZoom * 5));
            this.CurrentItemIndex = Math.Min(this.ForSale.Count - ShopMenu2.ItemsPerPage, Math.Max(0, (int)(this.ForSale.Count * (double)((y - this.ScrollBarRunner.Y) / (float)this.ScrollBarRunner.Height))));
            this.SetScrollBarToCurrentIndex();
            if (y1 == this.ScrollBar.bounds.Y)
                return;
            Game1.playSound("shiny4");
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            this.Scrolling = false;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.CurrentItemIndex > 0)
            {
                this.UpArrowPressed();
                Game1.playSound("shiny4");
            }
            else
            {
                if (direction >= 0 || this.CurrentItemIndex >= Math.Max(0, this.ForSale.Count - ShopMenu2.ItemsPerPage))
                    return;
                this.DownArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y);
            if (Game1.activeClickableMenu == null)
                return;
            Vector2 clickableComponent = this.Inventory.snapToClickableComponent(x, y);
            if (this.BackButton.containsPoint(x, y))
            {
                this.BackButton.scale = this.BackButton.baseScale;
                this.BackButtonPressed();
            }
            else if (this.DownArrow.containsPoint(x, y) && this.CurrentItemIndex < Math.Max(0, this.ForSale.Count - ShopMenu2.ItemsPerPage))
            {
                this.DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (this.UpArrow.containsPoint(x, y) && this.CurrentItemIndex > 0)
            {
                this.UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (this.ScrollBar.containsPoint(x, y))
                this.Scrolling = true;
            else if (!this.DownArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && (x < this.xPositionOnScreen + this.width + Game1.tileSize * 2 && y > this.yPositionOnScreen) && y < this.yPositionOnScreen + this.height)
            {
                this.Scrolling = true;
                this.leftClickHeld(x, y);
                this.releaseLeftClick(x, y);
            }
            this.CurrentItemIndex = Math.Max(0, Math.Min(this.ForSale.Count - ShopMenu2.ItemsPerPage, this.CurrentItemIndex));
            if (this.HeldItem == null)
            {
                Item obj = this.Inventory.leftClick(x, y, null, false);
                if (obj != null)
                {
                    this.ChargePlayer(Game1.player, this.Currency, -((obj is Object ? (int)((obj as Object).sellToStorePrice() * (double)this.SellPercentage) : (int)(obj.salePrice() / 2 * (double)this.SellPercentage)) * obj.Stack));
                    int num = obj.Stack / 8 + 2;
                    for (int index = 0; index < num; ++index)
                    {
                        this.Animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
                        {
                            alphaFade = 0.025f,
                            motion = new Vector2(Game1.random.Next(-3, 4), -4f),
                            acceleration = new Vector2(0.0f, 0.5f),
                            delayBeforeAnimationStart = index * 25,
                            scale = Game1.pixelZoom * 0.5f
                        });
                        this.Animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16, 64, 16, 16), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
                        {
                            scale = Game1.pixelZoom,
                            alphaFade = 0.025f,
                            delayBeforeAnimationStart = index * 50,
                            motion = Utility.getVelocityTowardPoint(new Point((int)clickableComponent.X + 32, (int)clickableComponent.Y + 32), new Vector2(this.xPositionOnScreen - Game1.pixelZoom * 9, this.yPositionOnScreen + this.height - this.Inventory.height - Game1.pixelZoom * 4), 8f),
                            acceleration = Utility.getVelocityTowardPoint(new Point((int)clickableComponent.X + 32, (int)clickableComponent.Y + 32), new Vector2(this.xPositionOnScreen - Game1.pixelZoom * 9, this.yPositionOnScreen + this.height - this.Inventory.height - Game1.pixelZoom * 4), 0.5f)
                        });
                    }
                    if (obj is Object && (obj as Object).Edibility != -300)
                    {
                        for (int index = 0; index < obj.Stack; ++index)
                        {
                            if (Game1.random.NextDouble() < 0.04)
                                ((SeedShop)Game1.getLocationFromName("SeedShop")).itemsToStartSellingTomorrow.Add(obj.getOne());
                        }
                    }
                    Game1.playSound("sell");
                    Game1.playSound("purchase");
                    if (this.Inventory.getItemAt(x, y) == null)
                        this.Animations.Add(new TemporaryAnimatedSprite(5, clickableComponent + new Vector2(32f, 32f), Color.White)
                        {
                            motion = new Vector2(0.0f, -0.5f)
                        });
                }
            }
            else
                this.HeldItem = this.Inventory.leftClick(x, y, this.HeldItem);
            for (int index1 = 0; index1 < this.ForSaleButtons.Count; ++index1)
            {

                if (this.CurrentItemIndex + index1 < this.ForSale.Count && this.ForSaleButtons[index1].containsPoint(x, y))
                {
                    int index2 = this.CurrentItemIndex + index1;
                    if (this.ForSale[index2] != null)
                    {
                        int numberToBuy = Math.Min(Game1.oldKBState.IsKeyDown(Keys.LeftShift) ? Math.Min(Math.Min(5, this.GetPlayerCurrencyAmount(Game1.player, this.Currency) / Math.Max(1, this.ItemPriceAndStock[this.ForSale[index2]][0])), Math.Max(1, this.ItemPriceAndStock[this.ForSale[index2]][1])) : 1, this.ForSale[index2].maximumStackSize());
                        if (numberToBuy == -1)
                            numberToBuy = 1;
                        if (numberToBuy > 0 && this.TryToPurchaseItem(this.ForSale[index2], this.HeldItem, numberToBuy, x, y, index2))
                        {
                            this.ItemPriceAndStock.Remove(this.ForSale[index2]);
                            this.ForSale.RemoveAt(index2);
                        }
                        else if (numberToBuy <= 0)
                        {
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                            Game1.playSound("cancel");
                        }
                    }
                    this.CurrentItemIndex = Math.Max(0, Math.Min(this.ForSale.Count - ShopMenu2.ItemsPerPage, this.CurrentItemIndex));
                    return;
                }
            }
            if (!this.readyToClose() || x >= this.xPositionOnScreen - Game1.tileSize && y >= this.yPositionOnScreen - Game1.tileSize && (x <= this.xPositionOnScreen + this.width + Game1.tileSize * 2 && y <= this.yPositionOnScreen + this.height + Game1.tileSize))
                return;
            this.exitThisMenu();
        }

        public override bool readyToClose()
        {
            if (this.HeldItem == null)
                return this.Animations.Count == 0;
            return false;
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
            if (this.HeldItem == null)
                return;
            Game1.player.addItemToInventoryBool(this.HeldItem);
            Game1.playSound("coin");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Vector2 clickableComponent = this.Inventory.snapToClickableComponent(x, y);
            if (this.HeldItem == null)
            {
                Item obj1 = this.Inventory.rightClick(x, y, null, false);
                if (obj1 != null)
                {
                    this.ChargePlayer(Game1.player, this.Currency, -((obj1 is Object ? (int)((obj1 as Object).sellToStorePrice() * (double)this.SellPercentage) : (int)(obj1.salePrice() / 2 * (double)this.SellPercentage)) * obj1.Stack));
                    Item obj2 = null;
                    Game1.playSound(Game1.mouseClickPolling > 300 ? "purchaseRepeat" : "purchaseClick");
                    this.Animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * Game1.tileSize, 256, Game1.tileSize, Game1.tileSize), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
                    {
                        alphaFade = 0.025f,
                        motion = Utility.getVelocityTowardPoint(new Point((int)clickableComponent.X + 32, (int)clickableComponent.Y + 32), Game1.dayTimeMoneyBox.position + new Vector2(96f, 196f), 12f),
                        acceleration = Utility.getVelocityTowardPoint(new Point((int)clickableComponent.X + 32, (int)clickableComponent.Y + 32), Game1.dayTimeMoneyBox.position + new Vector2(96f, 196f), 0.5f)
                    });
                    if (obj2 is Object && (obj2 as Object).Edibility != -300 && Game1.random.NextDouble() < 0.04)
                        (Game1.getLocationFromName("SeedShop") as SeedShop).itemsToStartSellingTomorrow.Add(obj2.getOne());
                    if (this.Inventory.getItemAt(x, y) == null)
                    {
                        Game1.playSound("sell");
                        this.Animations.Add(new TemporaryAnimatedSprite(5, clickableComponent + new Vector2(32f, 32f), Color.White)
                        {
                            motion = new Vector2(0.0f, -0.5f)
                        });
                    }
                }
            }
            else
                this.HeldItem = this.Inventory.rightClick(x, y, this.HeldItem);
            for (int index1 = 0; index1 < this.ForSaleButtons.Count; ++index1)
            {
                if (this.CurrentItemIndex + index1 < this.ForSale.Count && this.ForSaleButtons[index1].containsPoint(x, y))
                {
                    int index2 = this.CurrentItemIndex + index1;
                    if (this.ForSale[index2] == null)
                        break;
                    int numberToBuy = Game1.oldKBState.IsKeyDown(Keys.LeftShift) ? Math.Min(Math.Min(5, this.GetPlayerCurrencyAmount(Game1.player, this.Currency) / this.ItemPriceAndStock[this.ForSale[index2]][0]), this.ItemPriceAndStock[this.ForSale[index2]][1]) : 1;
                    if (numberToBuy <= 0 || !this.TryToPurchaseItem(this.ForSale[index2], this.HeldItem, numberToBuy, x, y, index2))
                        break;

                    this.ItemPriceAndStock.Remove(this.ForSale[index2]);
                    this.ForSale.RemoveAt(index2);
                    break;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.BackButton.tryHover(x, y, 1f);

            base.performHoverAction(x, y);
            this.HoverText = "";
            this.HoveredItem = null;
            this.HoverPrice = -1;
            this.BoldTitleText = "";
            this.UpArrow.tryHover(x, y);
            this.DownArrow.tryHover(x, y);
            this.ScrollBar.tryHover(x, y);
            if (this.Scrolling)
                return;
            for (int index = 0; index < this.ForSaleButtons.Count; ++index)
            {
                if (this.CurrentItemIndex + index < this.ForSale.Count && this.ForSaleButtons[index].containsPoint(x, y))
                {
                    Item key = this.ForSale[this.CurrentItemIndex + index];
                    this.HoverText = key.getDescription();
                    this.BoldTitleText = key.Category == -425 ? ((Object)key).name : key.Name;
                    this.HoverPrice = this.ItemPriceAndStock == null || !this.ItemPriceAndStock.ContainsKey(key) ? key.salePrice() : this.ItemPriceAndStock[key][0];
                    this.HoveredItem = key;
                    this.ForSaleButtons[index].scale = Math.Min(this.ForSaleButtons[index].scale + 0.03f, 1.1f);
                }
                else
                    this.ForSaleButtons[index].scale = Math.Max(1f, this.ForSaleButtons[index].scale - 0.03f);
            }
            if (this.HeldItem != null)
                return;
            foreach (ClickableComponent c in this.Inventory.inventory)
            {
                if (c.containsPoint(x, y))
                {
                    Item clickableComponent = this.Inventory.getItemFromClickableComponent(c);
                    if (clickableComponent != null && this.HighlightItemToSell(clickableComponent))
                    {
                        if (clickableComponent.Category == -425)
                            this.HoverText = ((Object)clickableComponent).name + " x " + clickableComponent.Stack;
                        else
                            this.HoverText = clickableComponent.Name + " x " + clickableComponent.Stack;
                        this.HoverPrice = (clickableComponent is Object ? (int)((clickableComponent as Object).sellToStorePrice() * (double)this.SellPercentage) : (int)(clickableComponent.salePrice() / 2 * (double)this.SellPercentage)) * clickableComponent.Stack;
                    }
                }
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (this.Poof == null || !this.Poof.update(time))
                return;
            this.Poof = null;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
            this.width = 1000 + IClickableMenu.borderWidth * 2;
            this.height = 600 + IClickableMenu.borderWidth * 2;
            this.initializeUpperRightCloseButton();
            if (Game1.viewport.Width < 1500)
                this.xPositionOnScreen = Game1.tileSize / 2;
            Game1.player.forceCanMove();
            this.Inventory = new InventoryMenu(this.xPositionOnScreen + this.width, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 + Game1.pixelZoom * 10, false, null, this.HighlightItemToSell)
            {
                showGrayedOutSlots = true
            };
            this.Inventory.movePosition(-this.Inventory.width - Game1.tileSize / 2, 0);
            this.UpArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            this.DownArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            this.ScrollBar = new ClickableTextureComponent(new Rectangle(this.UpArrow.bounds.X + Game1.pixelZoom * 3, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            this.ScrollBarRunner = new Rectangle(this.ScrollBar.bounds.X, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, this.ScrollBar.bounds.Width, this.height - Game1.tileSize - this.UpArrow.bounds.Height - Game1.pixelZoom * 7);
            this.ForSaleButtons.Clear();
            for (int index = 0; index < 4; ++index)
                this.ForSaleButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4 + index * ((this.height - Game1.tileSize * 4) / 4), this.width - Game1.tileSize / 2, (this.height - Game1.tileSize * 4) / 4 + Game1.pixelZoom), string.Concat(index)));
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen + this.width - this.Inventory.width - Game1.tileSize / 2 - Game1.pixelZoom * 6, this.yPositionOnScreen + this.height - Game1.tileSize * 4 + Game1.pixelZoom * 10, this.Inventory.width + Game1.pixelZoom * 14, this.height - Game1.tileSize * 7 + Game1.pixelZoom * 5, Color.White, Game1.pixelZoom);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - Game1.tileSize * 4 + Game1.tileSize / 2 + Game1.pixelZoom, Color.White, Game1.pixelZoom);
            this.DrawCurrency(b);
            for (int index = 0; index < this.ForSaleButtons.Count; ++index)
            {
                if (this.CurrentItemIndex + index < this.ForSale.Count)
                {
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), this.ForSaleButtons[index].bounds.X, this.ForSaleButtons[index].bounds.Y, this.ForSaleButtons[index].bounds.Width, this.ForSaleButtons[index].bounds.Height, !this.ForSaleButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) || this.Scrolling ? Color.White : Color.Wheat, Game1.pixelZoom, false);
                    b.Draw(Game1.mouseCursors, new Vector2(this.ForSaleButtons[index].bounds.X + Game1.tileSize / 2 - Game1.pixelZoom * 3, this.ForSaleButtons[index].bounds.Y + Game1.pixelZoom * 6 - Game1.pixelZoom), new Rectangle(296, 363, 18, 18), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
                    this.ForSale[this.CurrentItemIndex + index].drawInMenu(b, new Vector2(this.ForSaleButtons[index].bounds.X + Game1.tileSize / 2 - Game1.pixelZoom * 2, this.ForSaleButtons[index].bounds.Y + Game1.pixelZoom * 6), 1f);
                    if (this.ForSale[this.CurrentItemIndex + index].Category == -425)
                        SpriteText.drawString(b, ((Object)this.ForSale[this.CurrentItemIndex + index]).name, this.ForSaleButtons[index].bounds.X + Game1.tileSize * 3 / 2 + Game1.pixelZoom * 2, this.ForSaleButtons[index].bounds.Y + Game1.pixelZoom * 7);
                    else
                        SpriteText.drawString(b, this.ForSale[this.CurrentItemIndex + index].Name, this.ForSaleButtons[index].bounds.X + Game1.tileSize * 3 / 2 + Game1.pixelZoom * 2, this.ForSaleButtons[index].bounds.Y + Game1.pixelZoom * 7);
                    SpriteText.drawString(b, this.ItemPriceAndStock[this.ForSale[this.CurrentItemIndex + index]][0] + " ", this.ForSaleButtons[index].bounds.Right - SpriteText.getWidthOfString(this.ItemPriceAndStock[this.ForSale[this.CurrentItemIndex + index]][0] + " ") - Game1.pixelZoom * 8, this.ForSaleButtons[index].bounds.Y + Game1.pixelZoom * 7, 999999, -1, 999999, this.GetPlayerCurrencyAmount(Game1.player, this.Currency) >= this.ItemPriceAndStock[this.ForSale[this.CurrentItemIndex + index]][0] ? 1f : 0.5f);
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(this.ForSaleButtons[index].bounds.Right - Game1.pixelZoom * 13, this.ForSaleButtons[index].bounds.Y + Game1.pixelZoom * 10 - Game1.pixelZoom), new Rectangle(193 + this.Currency * 9, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 1f);
                }
            }
            if (this.ForSale.Count == 0)
                SpriteText.drawString(b, "Out of stock", this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString("Out of stock.") / 2, this.yPositionOnScreen + this.height / 2 - Game1.tileSize * 2);
            this.Inventory.draw(b);
            for (int index = this.Animations.Count - 1; index >= 0; --index)
            {
                if (this.Animations[index].update(Game1.currentGameTime))
                    this.Animations.RemoveAt(index);
                else
                    this.Animations[index].draw(b, true);
            }
            this.Poof?.draw(b);
            this.UpArrow.draw(b);
            this.DownArrow.draw(b);
            if (this.ForSale.Count > ShopMenu2.ItemsPerPage)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.ScrollBarRunner.X, this.ScrollBarRunner.Y, this.ScrollBarRunner.Width, this.ScrollBarRunner.Height, Color.White, Game1.pixelZoom);
                this.ScrollBar.draw(b);
            }
            if (!this.HoverText.Equals(""))
                IClickableMenu.drawToolTip(b, this.HoverText, this.BoldTitleText, this.HoveredItem, this.HeldItem != null, -1, this.Currency, this.GetHoveredItemExtraItemIndex(), this.GetHoveredItemExtraItemAmount(), null, this.HoverPrice);
            this.HeldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
            base.draw(b);
            if (Game1.viewport.Width > 1800 && Game1.options.showMerchantPortraits)
            {
                if (this.PortraitPerson != null)
                {
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(this.xPositionOnScreen - 80 * Game1.pixelZoom, this.yPositionOnScreen), new Rectangle(603, 414, 74, 74), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.91f);
                    if (this.PortraitPerson.Portrait != null)
                        b.Draw(this.PortraitPerson.Portrait, new Vector2(this.xPositionOnScreen - 80 * Game1.pixelZoom + Game1.pixelZoom * 5, this.yPositionOnScreen + Game1.pixelZoom * 5), new Rectangle(0, 0, 64, 64), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.92f);
                }
                if (this.PortraitPersonDialogue != null)
                    IClickableMenu.drawHoverText(b, this.PortraitPersonDialogue, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, this.xPositionOnScreen - (int)Game1.dialogueFont.MeasureString(this.PortraitPersonDialogue).X - Game1.tileSize, this.yPositionOnScreen + (this.PortraitPerson != null ? 78 * Game1.pixelZoom : 0));
            }

            this.BackButton.draw(b);

            this.drawMouse(b);
        }


        /*********
        ** Private methods
        *********/
        private void SetUpShopOwner(string who)
        {
            if (who == null)
                return;
            Random random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
            string text = "Have a look at my wares.";
            switch (who)
            {
                case "Leah":
                    this.PortraitPerson = Game1.getCharacterFromName("Leah");
                    switch (Game1.random.Next(3))
                    {
                        case 0:
                            text = "Much of my selection comes from right here in the valley.";
                            break;
                        case 1:
                            text = "Don't have time to forage? 'Foraged By Leah' has you covered!";
                            break;
                        case 2:
                            text = "You should try our Dandelions.";
                            break;
                    }
                    break;
                case "Robin":
                    this.PortraitPerson = Game1.getCharacterFromName("Robin");
                    switch (Game1.random.Next(5))
                    {
                        case 0:
                            text = "Need some construction supplies? Or are you looking to re-decorate?";
                            break;
                        case 1:
                            text = "I have a rotating selection of hand-made furniture.";
                            break;
                        case 2:
                            text = "I've got some great furniture for sale.";
                            break;
                        case 3:
                            text = "Got any spare construction material to sell?";
                            break;
                        case 4:
                            {
                                string itemName = this.ItemPriceAndStock.ElementAt(Game1.random.Next(2, this.ItemPriceAndStock.Count)).Key.DisplayName;
                                text = $"I've got {Lexicon.getProperArticleForWord(itemName)} {itemName} that would look just {Lexicon.getRandomPositiveAdjectiveForEventOrPerson()} in your house.";
                            }
                            break;
                    }
                    break;
                case "Clint":
                    this.PortraitPerson = Game1.getCharacterFromName("Clint");
                    switch (Game1.random.Next(3))
                    {
                        case 0:
                            text = "Too lazy to mine your own ore? No problem.";
                            break;
                        case 1:
                            text = "I've got lumps of raw metal for sale. Knock yourself out.";
                            break;
                        case 2:
                            text = "Looking to sell any metals or minerals?";
                            break;
                    }
                    break;
                case "ClintUpgrade":
                    this.PortraitPerson = Game1.getCharacterFromName("Clint");
                    text = "I can upgrade your tools with more power. You'll have to leave them with me for a few days, though.";
                    break;
                case "Willy":
                    this.PortraitPerson = Game1.getCharacterFromName("Willy");
                    text = "Need fishing supplies? You've come to the right place.";
                    if (Game1.random.NextDouble() < 0.05)
                        text = "Sorry about the smell.";
                    break;
                case "Pierre":
                    this.PortraitPerson = Game1.getCharacterFromName("Pierre");
                    switch (Game1.dayOfMonth % 7)
                    {
                        case 0:
                            text = "Got anything you want to sell?";
                            break;
                        case 1:
                            text = "Need some supplies?";
                            break;
                        case 2:
                            text = "Don't forget to check out my daily wallpaper and flooring selection!";
                            break;
                        case 3:
                            text = "What can I get for you?";
                            break;
                        case 4:
                            text = "I carry only the finest goods.";
                            break;
                        case 5:
                            text = "I've got quality goods for sale.";
                            break;
                        case 6:
                            text = "Looking to buy something?";
                            break;
                    }
                    text = "Welcome to Pierre's! " + text;
                    if (Game1.dayOfMonth == 28)
                        text = "The season's almost over. I'll be changing stock tomorrow.";
                    break;
                case "Dwarf":
                    this.PortraitPerson = Game1.getCharacterFromName("Dwarf");
                    text = "Buy something?";
                    break;
                case "HatMouse":
                    text = "Hiyo, poke. Did you bring coins? Gud. Me sell hats.";
                    break;
                case "Krobus":
                    this.PortraitPerson = Game1.getCharacterFromName("Krobus");
                    text = "Rare Goods";
                    break;
                case "Traveler":
                    switch (random.Next(5))
                    {
                        case 0:
                            text = "I've got a little bit of everything. Take a look!";
                            break;
                        case 1:
                            text = "I smuggled these goods out of the Gotoro Empire. Why do you think they're so expensive?";
                            break;
                        case 2:
                            text = "I'll have new items every week, so make sure to come back!";
                            break;
                        case 3:
                            {
                                string itemName = this.ItemPriceAndStock.ElementAt(Game1.random.Next(2, this.ItemPriceAndStock.Count)).Key.DisplayName;
                                text = $"Let me see... Oh! I've got just what you need: {Lexicon.getProperArticleForWord(itemName)} {itemName}!";
                            }
                            break;
                        case 4:
                            text = "Beautiful country you have here. One of my favorite stops. The pig likes it, too.";
                            break;
                    }
                    break;
                case "Marnie":
                    this.PortraitPerson = Game1.getCharacterFromName("Marnie");
                    text = "Animal supplies for sale!";
                    if (random.NextDouble() < 0.0001)
                        text = "*sigh*... When the door opened I thought it might be Lewis.";
                    break;
                case "Gus":
                    this.PortraitPerson = Game1.getCharacterFromName("Gus");
                    switch (Game1.random.Next(4))
                    {
                        case 0:
                            text = "What'll you have?";
                            break;
                        case 1:
                            text = "Can you smell that? It's the " + this.ItemPriceAndStock.ElementAt(random.Next(this.ItemPriceAndStock.Count)).Key.Name;
                            break;
                        case 2:
                            text = "Hungry? Thirsty? I've got just the thing.";
                            break;
                        case 3:
                            text = "Welcome to the Stardrop Saloon! What can I get ya?";
                            break;
                    }
                    break;
                case "Marlon":
                    this.PortraitPerson = Game1.getCharacterFromName("Marlon");
                    switch (random.Next(4))
                    {
                        case 0:
                            text = "The caves can be dangerous. Make sure you're prepared.";
                            break;
                        case 1:
                            text = "In the market for a new sword?";
                            break;
                        case 2:
                            text = "Welcome to the adventurer's guild.";
                            break;
                        case 3:
                            text = "Slay any monsters? I'll buy the loot.";
                            break;
                    }
                    if (random.NextDouble() < 0.001)
                        text = "The caves can be dangerous. How do you think I lost this eye?";
                    break;
                case "Sandy":
                    this.PortraitPerson = Game1.getCharacterFromName("Sandy");
                    text = "You won't find these goods anywhere else!";
                    if (random.NextDouble() < 0.0001)
                        text = "I've got just what you need.";
                    break;
            }
            this.PortraitPersonDialogue = Game1.parseText(text, Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
        }

        private bool HighlightItemToSell(Item i)
        {
            return this.CategoriesToSellHere.Contains(i.Category);
        }

        private int GetPlayerCurrencyAmount(Farmer who, int currencyType)
        {
            switch (currencyType)
            {
                case 0:
                    return who.Money;
                case 1:
                    return who.festivalScore;
                case 2:
                    return who.clubCoins;
                default:
                    return 0;
            }
        }

        private void SetScrollBarToCurrentIndex()
        {
            if (this.ForSale.Count <= 0)
                return;
            this.ScrollBar.bounds.Y = this.ScrollBarRunner.Height / Math.Max(1, this.ForSale.Count - ShopMenu2.ItemsPerPage + 1) * this.CurrentItemIndex + this.UpArrow.bounds.Bottom + Game1.pixelZoom;
            if (this.CurrentItemIndex != this.ForSale.Count - ShopMenu2.ItemsPerPage)
                return;
            this.ScrollBar.bounds.Y = this.DownArrow.bounds.Y - this.ScrollBar.bounds.Height - Game1.pixelZoom;
        }

        private void DownArrowPressed()
        {
            this.DownArrow.scale = this.DownArrow.baseScale;
            ++this.CurrentItemIndex;
            this.SetScrollBarToCurrentIndex();
        }

        private void UpArrowPressed()
        {
            this.UpArrow.scale = this.UpArrow.baseScale;
            --this.CurrentItemIndex;
            this.SetScrollBarToCurrentIndex();
        }

        private void BackButtonPressed()
        {
            if (this.readyToClose())
            {
                this.exitThisMenu();
                this.ShowMainMenu();
            }
        }

        private void ChargePlayer(Farmer who, int currencyType, int amount)
        {
            switch (currencyType)
            {
                case 0:
                    who.Money -= amount;
                    break;
                case 1:
                    who.festivalScore -= amount;
                    break;
                case 2:
                    who.clubCoins -= amount;
                    break;
            }
        }

        private bool TryToPurchaseItem(Item item, Item heldItem, int numberToBuy, int x, int y, int indexInForSaleList)
        {
            if (heldItem == null)
            {
                int amount = this.ItemPriceAndStock[item][0] * numberToBuy;
                int num = -1;
                if (this.ItemPriceAndStock[item].Length > 2)
                    num = this.ItemPriceAndStock[item][2];
                if (this.GetPlayerCurrencyAmount(Game1.player, this.Currency) >= amount && (num == -1 || Game1.player.hasItemInInventory(num, 5)))
                {
                    this.HeldItem = item.getOne();
                    this.HeldItem.Stack = numberToBuy;
                    if (!Game1.player.couldInventoryAcceptThisItem(this.HeldItem))
                    {
                        Game1.playSound("smallSelect");
                        this.HeldItem = null;
                        return false;
                    }
                    if (this.ItemPriceAndStock[item][1] != int.MaxValue)
                    {
                        this.ItemPriceAndStock[item][1] -= numberToBuy;
                        this.ForSale[indexInForSaleList].Stack -= numberToBuy;
                    }
                    this.ChargePlayer(Game1.player, this.Currency, amount);
                    if (num != -1)
                        Game1.player.removeItemsFromInventory(num, 5);
                    if (item.actionWhenPurchased())
                    {
                        if (this.HeldItem is Object obj && obj.IsRecipe)
                        {
                            if (obj.name.Contains("Bundle"))
                            {
                                this.ItemUtils.AddBundle(obj.SpecialVariable);
                                Game1.playSound("newRecipe");
                                this.HeldItem = null;
                            }
                            else
                            {
                                string key = this.HeldItem.Name.Substring(0, this.HeldItem.Name.IndexOf("Recipe") - 1);
                                try
                                {
                                    if (((Object)this.HeldItem).Category == -7)
                                    {
                                        Game1.player.cookingRecipes.Add(key, 0);

                                        if (this.LocationName.Equals("Recipe") && this.GiveAchievements)
                                        {
                                            Game1.player.cookedRecipe(item.ParentSheetIndex);
                                            Game1.stats.checkForCookingAchievements();
                                        }
                                    }
                                    else
                                        Game1.player.craftingRecipes.Add(key, 0);
                                    Game1.playSound("newRecipe");
                                }
                                catch
                                {
                                }
                                this.HeldItem = null;
                            }
                        }
                    }
                    else if (Game1.mouseClickPolling > 300)
                        Game1.playSound("purchaseRepeat");
                    else
                        Game1.playSound("purchaseClick");
                }
                else
                {
                    Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                    Game1.playSound("cancel");
                }
            }
            else if (heldItem.Name.Equals(item.Name))
            {
                numberToBuy = Math.Min(numberToBuy, heldItem.maximumStackSize() - heldItem.Stack);
                if (numberToBuy > 0)
                {
                    int amount = this.ItemPriceAndStock[item][0] * numberToBuy;
                    int index = -1;
                    if (this.ItemPriceAndStock[item].Length > 2)
                        index = this.ItemPriceAndStock[item][2];
                    if (this.GetPlayerCurrencyAmount(Game1.player, this.Currency) >= amount)
                    {
                        this.HeldItem.Stack += numberToBuy;
                        if (this.ItemPriceAndStock[item][1] != int.MaxValue)
                            this.ItemPriceAndStock[item][1] -= numberToBuy;
                        this.ChargePlayer(Game1.player, this.Currency, amount);
                        Game1.playSound(Game1.mouseClickPolling > 300 ? "purchaseRepeat" : "purchaseClick");
                        if (index != -1)
                            Game1.player.removeItemsFromInventory(index, 5);
                        if (item.actionWhenPurchased())
                            this.HeldItem = null;
                    }
                    else
                    {
                        Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                        Game1.playSound("cancel");
                    }
                }
            }
            if (this.ItemPriceAndStock[item][1] > 0)
            {
                if (this.GiveAchievements)
                {
                    if (this.LocationName.Equals("FishShop"))
                    {
                        if (item.Category == -4)
                            Game1.player.caughtFish(item.ParentSheetIndex, 12);
                    }
                    else if (this.LocationName.Equals("Artifact"))
                    {
                        if (item.Category == -2)
                            Game1.player.foundMineral(item.ParentSheetIndex);
                        else if (item.Category == -12)
                            Game1.player.foundMineral(item.ParentSheetIndex);
                        else if (item.Category == 0)
                            Game1.player.foundArtifact(item.ParentSheetIndex, numberToBuy);
                    }
                }

                return false;
            }
            this.HoveredItem = null;
            return true;
        }

        private void DrawCurrency(SpriteBatch b)
        {
            switch (this.Currency)
            {
                case 0:
                    Game1.dayTimeMoneyBox.drawMoneyBox(b, this.xPositionOnScreen - Game1.pixelZoom * 9, this.yPositionOnScreen + this.height - this.Inventory.height - Game1.pixelZoom * 3);
                    break;
            }
        }

        private int GetHoveredItemExtraItemIndex()
        {
            if (this.ItemPriceAndStock != null && this.HoveredItem != null && (this.ItemPriceAndStock.ContainsKey(this.HoveredItem) && this.ItemPriceAndStock[this.HoveredItem].Length > 2))
                return this.ItemPriceAndStock[this.HoveredItem][2];
            return -1;
        }

        private int GetHoveredItemExtraItemAmount()
        {
            return 5;
        }
    }
}

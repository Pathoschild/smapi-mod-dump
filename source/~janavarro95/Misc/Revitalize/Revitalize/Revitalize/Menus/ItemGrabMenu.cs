using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace Revitalize.Menus
{
    public class ItemGrabMenu : MenuWithInventory
    {
        public delegate void behaviorOnItemSelect(Item item, StardewValley.Farmer who);

        public const int source_none = 0;

        public const int source_chest = 1;

        public const int source_gift = 2;

        public const int source_fishingChest = 3;

        private StardewValley.Menus.InventoryMenu ItemsToGrabMenu;

        private TemporaryAnimatedSprite poof;

        public bool reverseGrab;

        public bool showReceivingMenu = true;

        public bool drawBG = true;

        public bool destroyItemOnClick;

        public bool canExitOnKey;

        public bool playRightClickSound;

        public bool allowRightClick;

        public bool shippingBin;

        private string message;

        private ItemGrabMenu.behaviorOnItemSelect behaviorFunction;

        public ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab;

        private Item hoverItem;

        private Item sourceItem;

        private ClickableTextureComponent organizeButton;

        private ClickableTextureComponent colorPickerToggleButton;

        private ClickableTextureComponent lastShippedHolder;

        public int source;

        private bool snappedtoBottom;

        private DiscreteColorPicker chestColorPicker;

        public int capacity;

        public int Rows;

        public ItemGrabMenu(List<Item> inventory,int rows) : base(null, true, true, 0, 0)
        {
            Rows = rows;
            this.ItemsToGrabMenu = new StardewValley.Menus.InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen, false, inventory, null, inventory.Capacity, Rows, 0, 0, true);
            capacity = inventory.Capacity;
          //  Log.AsyncO("MAX LOAD"+this.capacity);
            this.reverseGrab = true;
          
            this.organizeButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - Game1.tileSize, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize", new object[0]), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), (float)Game1.pixelZoom, false);
            
            this.inventory.showGrayedOutSlots = true;
        }

        public ItemGrabMenu(List<Item> inventory,int rows, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction, ItemGrabMenu.behaviorOnItemSelect behaviorOnItemSelectFunction, string message, ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false, bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true, bool showOrganizeButton = false, int source = 0, Item sourceItem = null) : base(highlightFunction, true, true, 0, 0)
        {
            this.source = source;
            this.message = message;
            this.reverseGrab = reverseGrab;
            this.showReceivingMenu = showReceivingMenu;
            this.playRightClickSound = playRightClickSound;
            this.allowRightClick = allowRightClick;
            this.inventory.showGrayedOutSlots = true;
            this.sourceItem = sourceItem;
            this.Rows = rows;
            if (source == 1 && sourceItem != null && sourceItem is Chest)
            {
                this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize - IClickableMenu.borderWidth * 2, 0, new Chest(true));
                this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((sourceItem as Chest).playerChoiceColor);
                (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
                this.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 5, 16 * Game1.pixelZoom, 16 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), (float)Game1.pixelZoom, false)
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker", new object[0])
                };
            }
            if (snapToBottom)
            {
                base.movePosition(0, Game1.viewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
                this.snappedtoBottom = true;
            }
            this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen, false, inventory, highlightFunction, inventory.Capacity, Rows, 0, 0, true);
            this.behaviorFunction = behaviorOnItemSelectFunction;
            this.behaviorOnItemGrab = behaviorOnItemGrab;
            this.canExitOnKey = canBeExitedWithKey;
            if (showOrganizeButton)
            {
                this.organizeButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - Game1.tileSize, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize", new object[0]), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), (float)Game1.pixelZoom, false);
            }
            if ((Game1.isAnyGamePadButtonBeingPressed() || !Game1.lastCursorMotionWasMouse) && this.ItemsToGrabMenu.actualInventory.Count > 0 && Game1.activeClickableMenu == null)
            {
                Game1.setMousePosition(this.inventory.inventory[0].bounds.Center);
            }
        }

        public void initializeShippingBin()
        {
            this.shippingBin = true;
            this.lastShippedHolder = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width / 2 - 12 * Game1.pixelZoom, this.yPositionOnScreen + this.height / 2 - 20 * Game1.pixelZoom - Game1.tileSize, 24 * Game1.pixelZoom, 24 * Game1.pixelZoom), "", Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem", new object[0]), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), (float)Game1.pixelZoom, false);
        }

        public void setSourceItem(Item item)
        {
            this.sourceItem = item;
            this.chestColorPicker = null;
            this.colorPickerToggleButton = null;
            if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest)
            {
                this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize - IClickableMenu.borderWidth * 2, 0, new Chest(true));
                this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((this.sourceItem as Chest).playerChoiceColor);
                (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
                this.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 5, 16 * Game1.pixelZoom, 16 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), (float)Game1.pixelZoom, false)
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker", new object[0])
                };
            }
        }

        public void setBackgroundTransparency(bool b)
        {
            this.drawBG = b;
        }

        public void setDestroyItemOnClick(bool b)
        {
            this.destroyItemOnClick = b;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!this.allowRightClick)
            {
                return;
            }
            base.receiveRightClick(x, y, playSound && this.playRightClickSound);
            if (this.heldItem == null && this.showReceivingMenu)
            {
                this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
                if (this.heldItem != null && this.behaviorOnItemGrab != null)
                {
                    this.behaviorOnItemGrab(this.heldItem, Game1.player);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                    {
                        (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
                    }
                }
                if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).parentSheetIndex == 326)
                {
                    this.heldItem = null;
                    Game1.player.canUnderstandDwarves = true;
                    this.poof = new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                    Game1.playSound("fireball");
                    return;
                }
                if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).isRecipe)
                {
                    string key = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
                    try
                    {
                        if ((this.heldItem as StardewValley.Object).category == -7)
                        {
                            Game1.player.cookingRecipes.Add(key, 0);
                        }
                        else
                        {
                            Game1.player.craftingRecipes.Add(key, 0);
                        }
                        this.poof = new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception)
                    {
                    }
                    this.heldItem = null;
                    return;
                }
                if (Game1.player.addItemToInventoryBool(this.heldItem, false))
                {
                    this.heldItem = null;
                    Game1.playSound("coin");
                    return;
                }
            }
            else if (this.reverseGrab || this.behaviorFunction != null)
            {
                this.behaviorFunction(this.heldItem, Game1.player);
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                {
                    (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
                }
                if (this.destroyItemOnClick)
                {
                    this.heldItem = null;
                    return;
                }
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            if (this.snappedtoBottom)
            {
                base.movePosition((newBounds.Width - oldBounds.Width) / 2, Game1.viewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
            }
            if (this.ItemsToGrabMenu != null)
            {
                this.ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
            }
            if (this.organizeButton != null)
            {
                this.organizeButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - Game1.tileSize, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize", new object[0]), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), (float)Game1.pixelZoom, false);
            }
            if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest)
            {
                this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize - IClickableMenu.borderWidth * 2, 0, null);
                this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((this.sourceItem as Chest).playerChoiceColor);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, !this.destroyItemOnClick);
            if (ItemsToGrabMenu.isWithinBounds(x, y) == false)
            {
                
                    //Log.AsyncC("AWESOME");
                    bool f = Util.isInventoryFull(this.ItemsToGrabMenu.actualInventory);
                    if (f == false)
                    {
                        this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
                        Util.addItemToOtherInventory(this.ItemsToGrabMenu.actualInventory, this.heldItem);
                        //Log.AsyncG("not full");
                    }
                    else
                    {
                        // this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
                        // Util.addItemToInventorySilently(this.heldItem);
                    }
                    // this.ItemsToGrabMenu.inventory.Add(new ClickableComponent(new Rectangle(ItemsToGrabMenu.xPositionOnScreen + ItemsToGrabMenu.actualInventory.Count-1 % (this.capacity / this.ItemsToGrabMenu.rows) * Game1.tileSize + this.ItemsToGrabMenu.horizontalGap * (ItemsToGrabMenu.actualInventory.Count-1 % (this.capacity / this.ItemsToGrabMenu.rows)), ItemsToGrabMenu.yPositionOnScreen + ItemsToGrabMenu.actualInventory.Count-1 / (this.capacity / this.ItemsToGrabMenu.rows) * (Game1.tileSize + this.ItemsToGrabMenu.verticalGap) + (ItemsToGrabMenu.actualInventory.Count-1 / (this.capacity / this.ItemsToGrabMenu.rows) - 1) * Game1.pixelZoom -  (Game1.tileSize / 5), Game1.tileSize, Game1.tileSize), string.Concat(ItemsToGrabMenu.actualInventory.Count-1)));
                    if (this.okButton.containsPoint(x, y) == false && this.organizeButton.containsPoint(x, y) == false && f == false)
                    {
                        //  
                        Game1.activeClickableMenu = new ItemGrabMenu(this.ItemsToGrabMenu.actualInventory,this.Rows, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.behaviorFunction, null, this.behaviorOnItemGrab, false, true, true, true, true, this.source, this.sourceItem);
                        Game1.playSound("Ship");

                    }
                    if (this.organizeButton.containsPoint(x, y))
                    {
                        ItemGrabMenu.organizeItemsInList(this.ItemsToGrabMenu.actualInventory);
                    }
                
            }
            else
            {

                if (Util.isInventoryFull(this.ItemsToGrabMenu.actualInventory) == true)
                {
                    Item i=new StardewValley.Object();
                    Item j = new StardewValley.Object();
                    j = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
                    i = this.heldItem;
                    
                  
                    Util.addItemToInventorySilently(j);
                   // this.heldItem = null;

                    foreach (ClickableComponent current in ItemsToGrabMenu.inventory)
                    {
                        if (current.containsPoint(x, y))
                        {
                            int num = Convert.ToInt32(current.name);
                            //   Log.AsyncO(num);
                            this.ItemsToGrabMenu.actualInventory.RemoveAt(num);
                            //  Log.AsyncO("Remaining " + ItemsToGrabMenu.actualInventory.Count);

                        }
                    }
               //   j=  this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
                    Util.addItemToOtherInventory(this.ItemsToGrabMenu.actualInventory, i);
                    //Log.AsyncG("item swap");
                    Game1.activeClickableMenu = new ItemGrabMenu(this.ItemsToGrabMenu.actualInventory,this.Rows, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.behaviorFunction, null, this.behaviorOnItemGrab, false, true, true, true, true, this.source, this.sourceItem);
                    Game1.playSound("Ship");
                    this.heldItem = null;
                    return;
                }
               


                this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
                Util.addItemToInventoryElseDrop(this.heldItem);
                this.heldItem = null;

                foreach (ClickableComponent current in ItemsToGrabMenu.inventory)
                {
                    if (current.containsPoint(x, y))
                    {
                        int num = Convert.ToInt32(current.name);
                     //   Log.AsyncO(num);
                       this.ItemsToGrabMenu.actualInventory.RemoveAt(num);
                      //  Log.AsyncO("Remaining " + ItemsToGrabMenu.actualInventory.Count);
                        
                    }
                }
                Game1.activeClickableMenu = new ItemGrabMenu(this.ItemsToGrabMenu.actualInventory,this.Rows, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.behaviorFunction, null, this.behaviorOnItemGrab, false, true, true, true, true, this.source, this.sourceItem);
                Game1.playSound("Ship");

            }
            return;
            if (this.shippingBin && this.lastShippedHolder.containsPoint(x, y))
            {
                if (Game1.getFarm().lastItemShipped != null && Game1.player.addItemToInventoryBool(Game1.getFarm().lastItemShipped, false))
                {
                    Game1.playSound("coin");
                    Game1.getFarm().shippingBin.Remove(Game1.getFarm().lastItemShipped);
                    Game1.getFarm().lastItemShipped = null;
                    if (Game1.player.ActiveObject != null)
                    {
                        Game1.player.showCarrying();
                        Game1.player.Halt();
                    }
                }
                return;
            }
            if (this.chestColorPicker != null)
            {
                this.chestColorPicker.receiveLeftClick(x, y, true);
                if (this.sourceItem != null && this.sourceItem is Chest)
                {
                    (this.sourceItem as Chest).playerChoiceColor = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
                }
            }
            if (this.colorPickerToggleButton != null && this.colorPickerToggleButton.containsPoint(x, y))
            {
                Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
                this.chestColorPicker.visible = Game1.player.showChestColorPicker;
                Game1.soundBank.PlayCue("drumkit6");
            }
            if (this.heldItem == null && this.showReceivingMenu)
            {
                this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
              //  Log.AsyncC("YAY");

              


                if (this.heldItem != null && this.behaviorOnItemGrab != null)
                {
                    this.behaviorOnItemGrab(this.heldItem, Game1.player);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                    {
                        (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
                    }
                }
                if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).parentSheetIndex == 326)
                {
                    this.heldItem = null;
                    Game1.player.canUnderstandDwarves = true;
                    this.poof = new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                    Game1.playSound("fireball");
                }
                else if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).parentSheetIndex == 102)
                {
                    this.heldItem = null;
                    Game1.player.foundArtifact(102, 1);
                    this.poof = new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                    Game1.playSound("fireball");
                }
                else if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).isRecipe)
                {
                    string key = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
                    try
                    {
                        if ((this.heldItem as StardewValley.Object).category == -7)
                        {
                            Game1.player.cookingRecipes.Add(key, 0);
                        }
                        else
                        {
                            Game1.player.craftingRecipes.Add(key, 0);
                        }
                        this.poof = new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception)
                    {
                    }
                    this.heldItem = null;
                }
                else if (Game1.player.addItemToInventoryBool(this.heldItem, false))
                {
                    this.heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else if ((this.reverseGrab || this.behaviorFunction != null) && this.isWithinBounds(x, y))
            {
                this.behaviorFunction(this.heldItem, Game1.player);
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                {
                    (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
                }
                if (this.destroyItemOnClick)
                {
                    this.heldItem = null;
                    return;
                }
            }
            if (this.organizeButton != null && this.organizeButton.containsPoint(x, y))
            {
                ItemGrabMenu.organizeItemsInList(this.ItemsToGrabMenu.actualInventory);
                Game1.activeClickableMenu = new ItemGrabMenu(this.ItemsToGrabMenu.actualInventory,this.Rows, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.behaviorFunction, null, this.behaviorOnItemGrab, false, true, true, true, true, this.source, this.sourceItem);
                Game1.playSound("Ship");
                return;
            }
            if (this.heldItem != null && !this.isWithinBounds(x, y) && this.heldItem.canBeTrashed())
            {
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                this.heldItem = null;
            }
        }

        public static void organizeItemsInList(List<Item> items)
        {
            items.Sort();
            items.Reverse();
        }

        public bool areAllItemsTaken()
        {
            for (int i = 0; i < this.ItemsToGrabMenu.actualInventory.Count; i++)
            {
                if (this.ItemsToGrabMenu.actualInventory[i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        public override void receiveKeyPress(Keys key)
        {
            if ((this.canExitOnKey || this.areAllItemsTaken()) && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                base.exitThisMenu(true);
                if (Game1.currentLocation.currentEvent != null)
                {
                    Event expr_4C = Game1.currentLocation.currentEvent;
                    int currentCommand = expr_4C.CurrentCommand;
                    expr_4C.CurrentCommand = currentCommand + 1;
                }
            }
            else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem != null)
            {
                Game1.setMousePosition(this.trashCan.bounds.Center);
            }
            if (key == Keys.Delete && this.heldItem != null && this.heldItem.canBeTrashed())
            {
                if (this.heldItem is StardewValley.Object && Game1.player.specialItems.Contains((this.heldItem as StardewValley.Object).parentSheetIndex))
                {
                    Game1.player.specialItems.Remove((this.heldItem as StardewValley.Object).parentSheetIndex);
                }
                this.heldItem = null;
                Game1.playSound("trashcan");
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (this.poof != null && this.poof.update(time))
            {
                this.poof = null;
            }
            if (this.chestColorPicker != null)
            {
                this.chestColorPicker.update(time);
            }
        }

        public override void performHoverAction(int x, int y)
        {
           
            if (this.colorPickerToggleButton != null)
            {
                this.colorPickerToggleButton.tryHover(x, y, 0.25f);
                if (this.colorPickerToggleButton.containsPoint(x, y))
                {
                    this.hoverText = this.colorPickerToggleButton.hoverText;
                    return;
                }
            }
            if (this.ItemsToGrabMenu.isWithinBounds(x, y) && this.showReceivingMenu)
            {
                if (ItemsToGrabMenu.actualInventory.Count == 0) return;
                this.hoveredItem = this.ItemsToGrabMenu.hover(x, y, this.heldItem);
            }
            else
            {
                base.performHoverAction(x, y);
            }
            if (this.organizeButton != null)
            {
                this.hoverText = null;
                this.organizeButton.tryHover(x, y, 0.1f);
                if (this.organizeButton.containsPoint(x, y))
                {
                    this.hoverText = this.organizeButton.hoverText;
                }
            }
            if (this.shippingBin)
            {
                this.hoverText = null;
                if (this.lastShippedHolder.containsPoint(x, y) && Game1.getFarm().lastItemShipped != null)
                {
                    this.hoverText = this.lastShippedHolder.hoverText;
                }
            }
            if (this.chestColorPicker != null)
            {
                this.chestColorPicker.performHoverAction(x, y);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (this.drawBG)
            {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);
            }
            base.draw(b, false, false);
            
            if (this.showReceivingMenu)
            {
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 16 * Game1.pixelZoom), (float)(this.yPositionOnScreen + this.height / 2 + Game1.tileSize + Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 16 * Game1.pixelZoom), (float)(this.yPositionOnScreen + this.height / 2 + Game1.tileSize - Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 10 * Game1.pixelZoom), (float)(this.yPositionOnScreen + this.height / 2 + Game1.tileSize - Game1.pixelZoom * 11)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                if (this.source != 0)
                {
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 18 * Game1.pixelZoom), (float)(this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 18 * Game1.pixelZoom), (float)(this.yPositionOnScreen + Game1.tileSize - Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                    Rectangle value = new Rectangle(127, 412, 10, 11);
                    int num = this.source;
                    if (num != 2)
                    {
                        if (num == 3)
                        {
                            value.X += 10;
                        }
                    }
                    else
                    {
                        value.X += 20;
                    }
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 13 * Game1.pixelZoom), (float)(this.yPositionOnScreen + Game1.tileSize - Game1.pixelZoom * 11)), new Rectangle?(value), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                }
                Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, false, true, null, false);
                this.ItemsToGrabMenu.draw(b);
            }
            else if (this.message != null)
            {
                Game1.drawDialogueBox(Game1.viewport.Width / 2, this.ItemsToGrabMenu.yPositionOnScreen + this.ItemsToGrabMenu.height / 2, false, false, this.message);
            }
            if (this.poof != null)
            {
                this.poof.draw(b, true, 0, 0);
            }
            if (this.shippingBin && Game1.getFarm().lastItemShipped != null)
            {
                this.lastShippedHolder.draw(b);
                Game1.getFarm().lastItemShipped.drawInMenu(b, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * 4), (float)(this.lastShippedHolder.bounds.Y + Game1.pixelZoom * 4)), 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * -2), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 25)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * 21), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 25)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * -2), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 11)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * 21), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 11)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
            }
            if (this.colorPickerToggleButton != null)
            {
                this.colorPickerToggleButton.draw(b);
            }
            if (this.chestColorPicker != null)
            {
                this.chestColorPicker.draw(b);
            }
            if (this.organizeButton != null)
            {
                this.organizeButton.draw(b);
            }
            if (this.hoverText != null && (this.hoveredItem == null || this.hoveredItem == null || this.ItemsToGrabMenu == null))
            {
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
            if (this.hoveredItem != null)
            {
                IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.Name, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
            }
            else if (this.hoveredItem != null && this.ItemsToGrabMenu != null)
            {
                IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
            }
            if (this.heldItem != null)
            {
                this.heldItem.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 8), (float)(Game1.getOldMouseY() + 8)), 1f);
            }
            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }
    }
}

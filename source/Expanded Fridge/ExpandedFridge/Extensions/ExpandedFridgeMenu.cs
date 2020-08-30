// Deprecated, will be removed in future commit

//#define OLD_CODE

#if OLD_CODE

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;

namespace ExpandedFridge
{
    /// <summary>
    /// This menu is just a stripped down version of an ItemGrabMenu.
    /// The main difference is that we use our extended fridge as a guide to what inventories we display and how items are transfered.
    /// We also do some extra mutex callbacks when the menu is closed if the fridge was accessed remotely.
    /// </summary>
    class ExpandedFridgeMenu : MenuWithInventory
    {
        /// Lists of chest tabs and their colors.
        private List<ClickableComponent> chestTabs = new List<ClickableComponent>();
        private List<Color> chestTabsColors = new List<Color>();

        /// Variables for the selected chest tab.
        private ClickableComponent selectedTab;
        private Color selectedTabColor;
        private int selectedTabIndex = 0;

        /// Some constant offsets for adjusting positions of components.
        private const float textOffsetX = Game1.tileSize * 0.35F;
        private const float textOffsetY = Game1.tileSize * 0.38F;
        private const float textOffsetYSelected = Game1.tileSize * 0.25F;
        private const float textOffsetXSelected = Game1.tileSize * 0.32F;
        private const int colorOffsetX = (int)(Game1.tileSize * 0.2F);
        private const int colorOffsetY = (int)(Game1.tileSize * 0.2F);
        private const float colorSizeModY = 0.65F;
        private const float colorSizeModX = 0.625F;

        /// The fridge hub this menu is connected to.
        private ExpandedFridgeHub fridgeHub;

        /// Other components needed to imitate the ItemGrabMenu
        public InventoryMenu ItemsToGrabMenu;
        private Item sourceItem;
        public ClickableTextureComponent organizeButton;
        public ClickableTextureComponent colorPickerToggleButton;
        public List<ClickableComponent> discreteColorPickerCC;
        public DiscreteColorPicker chestColorPicker;

        public ExpandedFridgeMenu(ExpandedFridgeHub fridgeHub) : base(InventoryMenu.highlightAllItems, true, true, 0, 0)
        {
            this.fridgeHub = fridgeHub;

            {
                int i = 0;
                int labelX = this.xPositionOnScreen + Game1.tileSize / 2;
                int labelY = (int)(this.yPositionOnScreen + Game1.tileSize * 3.3F);

                foreach (Chest c in fridgeHub.connectedChests)
                {
                    if (c == fridgeHub.selectedChest)
                    {
                        this.selectedTabIndex = i;
                        this.selectedTab = new ClickableComponent(new Rectangle(labelX + Game1.tileSize * i++, labelY, Game1.tileSize, Game1.tileSize), (i).ToString(), (i).ToString());
                        this.selectedTabColor = c.playerChoiceColor.Value;
                    }
                    else
                    {
                        this.chestTabs.Add(new ClickableComponent(new Rectangle(labelX + Game1.tileSize * i++, labelY, Game1.tileSize, Game1.tileSize), (i).ToString(), (i).ToString()));
                        this.chestTabsColors.Add(c.playerChoiceColor.Value);
                    }
                }
            }

            this.inventory.showGrayedOutSlots = true;
            this.sourceItem = fridgeHub.selectedChest;

            {
                this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, (Item)new Chest(true));
                this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((Color)((NetFieldBase<Color, NetColor>)fridgeHub.selectedChest.playerChoiceColor));
                (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
                ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + 64 + 20, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f, false);
                textureComponent.hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker");
                textureComponent.myID = 27346;
                textureComponent.downNeighborID = 106;//5948;
                textureComponent.leftNeighborID = 11;
                this.colorPickerToggleButton = textureComponent;
            }

            this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + 32, this.yPositionOnScreen, false, this.fridgeHub.selectedChest.items, InventoryMenu.highlightAllItems, -1, 3, 0, 0, true);

            if (Game1.options.SnappyMenus)
            {
                this.ItemsToGrabMenu.populateClickableComponentList();
                for (int index = 0; index < this.ItemsToGrabMenu.inventory.Count; ++index)
                {
                    if (this.ItemsToGrabMenu.inventory[index] != null)
                    {
                        this.ItemsToGrabMenu.inventory[index].myID += 53910;
                        this.ItemsToGrabMenu.inventory[index].upNeighborID += 53910;
                        this.ItemsToGrabMenu.inventory[index].rightNeighborID += 53910;
                        this.ItemsToGrabMenu.inventory[index].downNeighborID = -7777;
                        this.ItemsToGrabMenu.inventory[index].leftNeighborID += 53910;
                        this.ItemsToGrabMenu.inventory[index].fullyImmutable = true;
                    }
                }
            }

            {
                ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), 4f, false);
                textureComponent.myID = 106;
                textureComponent.upNeighborID = 27346;
                textureComponent.downNeighborID = 5948;
                this.organizeButton = textureComponent;
            }

            if ((Game1.isAnyGamePadButtonBeingPressed() || !Game1.lastCursorMotionWasMouse) && (this.ItemsToGrabMenu.actualInventory.Count > 0 && Game1.activeClickableMenu == null))
                Game1.setMousePosition(this.inventory.inventory[0].bounds.Center);
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;

            this.discreteColorPickerCC = new List<ClickableComponent>();
            for (int index = 0; index < this.chestColorPicker.totalColors; ++index)
                this.discreteColorPickerCC.Add(new ClickableComponent(new Rectangle(this.chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + index * 9 * 4, this.chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "")
                {
                    myID = index + 4343,
                    rightNeighborID = index < this.chestColorPicker.totalColors - 1 ? index + 4343 + 1 : -1,
                    leftNeighborID = index > 0 ? index + 4343 - 1 : -1,
                    downNeighborID = this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.inventory.Count <= 0 ? 0 : 53910
                });

            for (int index = 0; index < 12; ++index)
            {
                if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
                    this.inventory.inventory[index].upNeighborID = (this.discreteColorPickerCC == null || this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.inventory.Count > index ? (this.ItemsToGrabMenu.inventory.Count > index ? 53910 + index : 53910) : 4343);
                if (this.discreteColorPickerCC != null && this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count > index)
                    this.ItemsToGrabMenu.inventory[index].upNeighborID = 4343;
            }

            for (int index = 0; index < 36; ++index)
            {
                if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count > index)
                {
                    this.inventory.inventory[index].upNeighborID = -7777;
                    this.inventory.inventory[index].upNeighborImmutable = true;
                }
            }

            if (this.trashCan != null && this.inventory.inventory.Count >= 12 && this.inventory.inventory[11] != null)
                this.inventory.inventory[11].rightNeighborID = 5948;
            if (this.trashCan != null)
                this.trashCan.leftNeighborID = 11;
            if (this.okButton != null)
                this.okButton.leftNeighborID = 11;

            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            switch (direction)
            {
                case 0:
                    if (oldID < 53910 && oldID >= 12)
                    {
                        this.currentlySnappedComponent = this.getComponentWithID(oldID - 12);
                        break;
                    }
                    int num1 = oldID + 24;
                    for (int index = 0; index < 3 && this.ItemsToGrabMenu.inventory.Count <= num1; ++index)
                        num1 -= 12;

                    if (num1 < 0)
                    {
                        if (this.ItemsToGrabMenu.inventory.Count > 0)
                            this.currentlySnappedComponent = this.getComponentWithID(53910 + this.ItemsToGrabMenu.inventory.Count - 1);
                        else if (this.discreteColorPickerCC != null)
                            this.currentlySnappedComponent = this.getComponentWithID(4343);
                    }
                    else
                        this.currentlySnappedComponent = this.getComponentWithID(num1 + 53910);
  
                    this.snapCursorToCurrentSnappedComponent();
                    break;
                case 2:
                    if (oldID >= 53910)
                    {
                        int num2 = oldID - 53910;
                        if (num2 + 12 <= this.ItemsToGrabMenu.inventory.Count - 1)
                        {
                            this.currentlySnappedComponent = this.getComponentWithID(num2 + 12 + 53910);
                            this.snapCursorToCurrentSnappedComponent();
                            break;
                        }
                    }
                    this.currentlySnappedComponent = this.getComponentWithID(oldRegion == 12598 ? 0 : (oldID - 53910) % 12);
                    this.snapCursorToCurrentSnappedComponent();
                    break;
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(this.ItemsToGrabMenu.inventory.Count <= 0 ? 0 : 53910);
            this.snapCursorToCurrentSnappedComponent();
        }

        public void setSourceItem(Item item)
        {
            this.sourceItem = item;
            this.chestColorPicker = (DiscreteColorPicker)null;
            this.colorPickerToggleButton = (ClickableTextureComponent)null;
            if (this.sourceItem == null)
                return;
            this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, (Item)new Chest(true));
            this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((Color)((NetFieldBase<Color, NetColor>)(this.sourceItem as Chest).playerChoiceColor));
            (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
            this.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + 64 + 20, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f, false)
            {
                hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker")
            };
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);
            if (this.heldItem == null)
            {
                this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
                if (this.heldItem != null)
                {
                    this.fridgeHub.grabItemFromChest(this.heldItem, Game1.player);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ExpandedFridgeMenu)
                        (Game1.activeClickableMenu as ExpandedFridgeMenu).setSourceItem(this.sourceItem);
                    if (Game1.options.SnappyMenus)
                    {
                        (Game1.activeClickableMenu as ExpandedFridgeMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                        (Game1.activeClickableMenu as ExpandedFridgeMenu).snapCursorToCurrentSnappedComponent();
                    }
                }

                if (!Game1.player.addItemToInventoryBool(this.heldItem, false))
                    return;
                this.heldItem = (Item)null;
                Game1.playSound("coin");

            }
            else
            {
                this.fridgeHub.grabItemFromInventory(this.heldItem, Game1.player);
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ExpandedFridgeMenu)
                    (Game1.activeClickableMenu as ExpandedFridgeMenu).setSourceItem(this.sourceItem);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
            this.organizeButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), 4f, false);
            this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, (Item)null);
            this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((Color)((NetFieldBase<Color, NetColor>)(this.sourceItem as Chest).playerChoiceColor));
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, true);
            foreach (ClickableComponent cc in this.chestTabs)
            {
                if (cc.containsPoint(x, y))
                {
                    int index = int.Parse(cc.label) - 1;
                    if (this.fridgeHub.SetSelectedChest(index))
                    {
                        Game1.activeClickableMenu = this.fridgeHub.CreateMenu();
                        return;
                    }
                }
            }

            this.chestColorPicker.receiveLeftClick(x, y, true);
            if (this.sourceItem != null && this.sourceItem is Chest)
            {
                (this.sourceItem as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
                this.selectedTabColor = (this.sourceItem as Chest).playerChoiceColor.Value;
            }

            if (this.colorPickerToggleButton.containsPoint(x, y))
            {
                Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
                this.chestColorPicker.visible = Game1.player.showChestColorPicker;
                try
                {
                    Game1.playSound("drumkit6");
                }
                catch (Exception ex)
                {
                }
            }

            if (this.heldItem == null)
            {
                this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
                if (this.heldItem != null)
                {
                    this.fridgeHub.grabItemFromChest(this.heldItem, Game1.player);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ExpandedFridgeMenu)
                    {
                        (Game1.activeClickableMenu as ExpandedFridgeMenu).setSourceItem(this.sourceItem);
                        if (Game1.options.SnappyMenus)
                        {
                            (Game1.activeClickableMenu as ExpandedFridgeMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                            (Game1.activeClickableMenu as ExpandedFridgeMenu).snapCursorToCurrentSnappedComponent();
                        }
                    }
                }
                if (Game1.player.addItemToInventoryBool(this.heldItem, false))
                {
                    this.heldItem = (Item)null;
                    Game1.playSound("coin");
                }
            }
            else if (this.isWithinBounds(x, y))
            {
                this.fridgeHub.grabItemFromInventory(this.heldItem, Game1.player);
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ExpandedFridgeMenu)
                {
                    (Game1.activeClickableMenu as ExpandedFridgeMenu).setSourceItem(this.sourceItem);
                    if (Game1.options.SnappyMenus)
                    {
                        (Game1.activeClickableMenu as ExpandedFridgeMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                        (Game1.activeClickableMenu as ExpandedFridgeMenu).snapCursorToCurrentSnappedComponent();
                    }
                }
            }
            if (this.organizeButton.containsPoint(x, y))
            {
                ExpandedFridgeMenu.organizeItemsInList(this.ItemsToGrabMenu.actualInventory);
                Game1.activeClickableMenu = this.fridgeHub.CreateMenu();
                (Game1.activeClickableMenu as ExpandedFridgeMenu).heldItem = this.heldItem;
                Game1.playSound("Ship");
            }
            else
            {
                if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
                    return;
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection, (GameLocation)null, -1);
                if (this.inventory.onAddItem != null)
                    this.inventory.onAddItem(this.heldItem, Game1.player);
                this.heldItem = (Item)null;
            }

        }

        public static void organizeItemsInList(IList<Item> items)
        {
            List<Item> objList = new List<Item>((IEnumerable<Item>)items);
            objList.Sort();
            for (int index = 0; index < items.Count; ++index)
                items[index] = (Item)null;
            for (int index = 0; index < items.Count; ++index)
                items[index] = objList[objList.Count - 1 - index];
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (b != Buttons.Back)
                return;
            ExpandedFridgeMenu.organizeItemsInList((IList<Item>)Game1.player.items);
            Game1.playSound("Ship");
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                this.applyMovementKey(key);
            if ((Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose()))
            {
                this.exitThisMenu(true);
                if (Game1.currentLocation.currentEvent != null)
                    ++Game1.currentLocation.currentEvent.CurrentCommand;
            }
            else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem != null)
                Game1.setMousePosition(this.trashCan.bounds.Center);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            this.chestColorPicker.update(time);
        }

        public override void performHoverAction(int x, int y)
        {
            this.colorPickerToggleButton.tryHover(x, y, 0.25f);
            if (this.colorPickerToggleButton.containsPoint(x, y))
            {
                this.hoverText = this.colorPickerToggleButton.hoverText;
                return;
            }

            if (this.ItemsToGrabMenu.isWithinBounds(x, y))
                this.hoveredItem = this.ItemsToGrabMenu.hover(x, y, this.heldItem);
            else
                base.performHoverAction(x, y);

            this.hoverText = (string)null;
            this.organizeButton.tryHover(x, y, 0.1f);
            if (this.organizeButton.containsPoint(x, y))
                this.hoverText = this.organizeButton.hoverText;

            this.chestColorPicker.performHoverAction(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);
            this.draw(b, false, false);

            if (!GameMenu.forcePreventClose)
            {
                int i = 0;
                foreach (ClickableComponent tab in this.chestTabs)
                {
                    int width = Game1.tileSize / 2;
                    int height = width;

                    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), tab.bounds.X, tab.bounds.Y, tab.bounds.Width, tab.bounds.Height, Color.White, 1, false);
                    Color tabCol = this.chestTabsColors[i++];
                    if (tabCol == Color.Black)
                        tabCol = Color.Transparent;
                    b.Draw(Game1.staminaRect, new Rectangle(tab.bounds.X + colorOffsetX, tab.bounds.Y + colorOffsetY, (int)(tab.bounds.Width * colorSizeModX), (int)(tab.bounds.Height * colorSizeModY)), tabCol);
                    int digit = int.Parse(tab.label);
                    if (digit < 10)
                        Utility.drawTinyDigits(digit, b, new Vector2(tab.bounds.X + textOffsetX, tab.bounds.Y + textOffsetY), 4f, 1f, Color.White);
                    else
                        Utility.drawTinyDigits(digit, b, new Vector2(tab.bounds.X + textOffsetX - 10, tab.bounds.Y + textOffsetY), 4f, 1f, Color.White);
                }
            }

            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 64), (float)(this.yPositionOnScreen + this.height / 2 + 64 + 16)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 64), (float)(this.yPositionOnScreen + this.height / 2 + 64 - 16)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 40), (float)(this.yPositionOnScreen + this.height / 2 + 64 - 44)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 72), (float)(this.yPositionOnScreen + 64 + 16)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 72), (float)(this.yPositionOnScreen + 64 - 16)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            Rectangle rectangle = new Rectangle((int)sbyte.MaxValue, 412, 10, 11);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 52), (float)(this.yPositionOnScreen + 64 - 44)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, false, true, (string)null, false, false);

            this.ItemsToGrabMenu.draw(b);

            this.colorPickerToggleButton.draw(b);

            this.chestColorPicker.draw(b);

            this.organizeButton.draw(b);

            if (!GameMenu.forcePreventClose)
            {
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), this.selectedTab.bounds.X, this.selectedTab.bounds.Y, this.selectedTab.bounds.Width, this.selectedTab.bounds.Height, Color.White, 1, false);
                Color tabCol = this.selectedTabColor;
                if (tabCol == Color.Black)
                    tabCol = Color.Transparent;
                b.Draw(Game1.staminaRect, new Rectangle(this.selectedTab.bounds.X + colorOffsetX, this.selectedTab.bounds.Y + colorOffsetY,
                    (int)(this.selectedTab.bounds.Width * colorSizeModX), (int)(this.selectedTab.bounds.Height * colorSizeModY)), tabCol);
                int digit = int.Parse(this.selectedTab.label);
                if (digit < 10)
                    Utility.drawTinyDigits(digit, b, new Vector2(this.selectedTab.bounds.X + textOffsetXSelected, this.selectedTab.bounds.Y + textOffsetYSelected), 5f, 1f, Color.White);
                else
                    Utility.drawTinyDigits(digit, b, new Vector2(this.selectedTab.bounds.X + textOffsetXSelected - 12, this.selectedTab.bounds.Y + textOffsetYSelected), 5f, 1f, Color.White);
            }

            if (this.hoverText != null && (this.hoveredItem == null || this.hoveredItem == null || this.ItemsToGrabMenu == null))
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            if (this.hoveredItem != null)
                IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, (CraftingRecipe)null, -1);
            else if (this.hoveredItem != null && this.ItemsToGrabMenu != null)
                IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, (CraftingRecipe)null, -1);
            if (this.heldItem != null)
                this.heldItem.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 8), (float)(Game1.getOldMouseY() + 8)), 1f);

            Game1.mouseCursorTransparency = 1f;
            this.drawMouse(b);
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
            Console.WriteLine("ExpandedFridgeMenu.emergencyShutDown");
            if (this.heldItem != null)
            {
                Console.WriteLine("Taking " + this.heldItem.Name);
                this.heldItem = Game1.player.addItemToInventory(this.heldItem);
            }
            if (this.heldItem != null)
            {
                Game1.playSound("throwDownITem");
                Console.WriteLine("Dropping " + this.heldItem.Name);
                Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection, (GameLocation)null, -1);
                this.heldItem = (Item)null;
            }
            if (this.fridgeHub.remoteOpen)
                this.fridgeHub.RemoteEmergencyClose();
        }

        protected override void cleanupBeforeExit()
        {
            base.cleanupBeforeExit();
            if (this.fridgeHub.remoteOpen)
                this.exitFunction = this.fridgeHub.RemoteClose;
        }
    }
}
#endif
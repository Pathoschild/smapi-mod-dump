/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mystra007/ExtendedFridgeSMAPI
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

//using xTile.Dimensions;


namespace ExtendedFridge
{
    internal class FridgeGrabMenu : MenuWithInventory
    {
        public const int region_itemsToGrabMenuModifier = 53910;

        public const int region_organizeButton = 106;

        public const int region_colorPickToggle = 27346;

        public const int region_specialButton = 12485;

        public const int region_lastShippedHolder = 12598;

        public const int source_none = 0;

        public const int source_chest = 1;

        public const int source_gift = 2;

        public const int source_fishingChest = 3;

        public const int specialButton_junimotoggle = 1;

        public InventoryMenu ItemsToGrabMenu;

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

        //M07: Comment these when copy-pasting code
        //private ItemGrabMenu.behaviorOnItemSelect behaviorFunction;
        //public ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab;
        //M07: And use these instead
        public FridgeGrabMenu.behaviorOnItemSelect behaviorFunction;
        public FridgeGrabMenu.behaviorOnItemSelect behaviorOnItemGrab;
        //END_M07

        private Item hoverItem;

        private Item sourceItem;

        public ClickableTextureComponent organizeButton;

        public ClickableTextureComponent colorPickerToggleButton;

        public ClickableTextureComponent specialButton;

        public ClickableTextureComponent lastShippedHolder;

        public List<ClickableComponent> discreteColorPickerCC;

        public int source;

        public int whichSpecialButton;

        public object specialObject;

        private bool snappedtoBottom;

        public DiscreteColorPicker chestColorPicker;


        #region -- Events --



        public delegate void behaviorOnItemSelect(Item item, StardewValley.Farmer who);
        #endregion

        #region -- Needed for Extended Fridge --        
        private ClickableTextureComponent previousPageButton;
        private ClickableTextureComponent nextPageButton;

        private bool showPrevButton;
        private bool showNextButton;

        public FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickNextButton;
        public FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickPreviousButton;
        public FridgeGrabMenu.behaviorOnOrganizeItems behaviorOnClickOrganize;


        public delegate void behaviorOnPageCtlClick();
        public delegate void behaviorOnOrganizeItems();
        #endregion

        //WAS:
        //public FridgeGrabMenu(List<Item> inventory)
        //    : base((InventoryMenu.highlightThisItem)null, true, true, 0, 0)
        //{
        //    this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen, false, inventory, (InventoryMenu.highlightThisItem)null, -1, 3, 0, 0, true);
        //    this.inventory.showGrayedOutSlots = true;
        //}

        //IS:
        //DONE
        public FridgeGrabMenu(List<Item> inventory) : base(null, true, true, 0, 0)
        {
            this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen, false, inventory, null, -1, 3, 0, 0, true);
            this.trashCan.myID = 106;
            this.ItemsToGrabMenu.populateClickableComponentList();
            for (int i = 0; i < this.ItemsToGrabMenu.inventory.Count; i++)
            {
                if (this.ItemsToGrabMenu.inventory[i] != null)
                {
                    ClickableComponent item = this.ItemsToGrabMenu.inventory[i];
                    item.myID = item.myID + 53910;
                    ClickableComponent clickableComponent = this.ItemsToGrabMenu.inventory[i];
                    clickableComponent.upNeighborID = clickableComponent.upNeighborID + 53910;
                    ClickableComponent item1 = this.ItemsToGrabMenu.inventory[i];
                    item1.rightNeighborID = item1.rightNeighborID + 53910;
                    this.ItemsToGrabMenu.inventory[i].downNeighborID = -7777;
                    ClickableComponent clickableComponent1 = this.ItemsToGrabMenu.inventory[i];
                    clickableComponent1.leftNeighborID = clickableComponent1.leftNeighborID + 53910;
                    this.ItemsToGrabMenu.inventory[i].fullyImmutable = true;
                }
            }
            if (Game1.options.SnappyMenus)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
                    {
                        this.inventory.inventory[j].upNeighborID = (this.shippingBin ? 12598 : -7777);
                    }
                }
                if (!this.shippingBin)
                {
                    for (int k = 0; k < 36; k++)
                    {
                        if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count > k)
                        {
                            this.inventory.inventory[k].upNeighborID = -7777;
                            this.inventory.inventory[k].upNeighborImmutable = true;
                        }
                    }
                }
                if (this.trashCan != null)
                {
                    this.trashCan.leftNeighborID = 11;
                }
                if (this.okButton != null)
                {
                    this.okButton.leftNeighborID = 11;
                }
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
            this.inventory.showGrayedOutSlots = true;
        }

        //WAS:
        //public FridgeGrabMenu(List<Item> inventory, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction, FridgeGrabMenu.behaviorOnItemSelect behaviorOnItemSelectFunction, string message, FridgeGrabMenu.behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false, bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true, bool showOrganizeButton = false, int source = 0, FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickNextButton = null, FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickPreviousButton = null, bool bShowPrevButton = false, bool bShowNextButton = false, FridgeGrabMenu.behaviorOnOrganizeItems behaviorOnClickOrganize = null)
        //    : base(highlightFunction, true, true, 0, 0)
        //{
        //    this.source = source;
        //    this.message = message;
        //    this.reverseGrab = reverseGrab;
        //    this.showReceivingMenu = showReceivingMenu;
        //    this.playRightClickSound = playRightClickSound;
        //    this.allowRightClick = allowRightClick;
        //    this.inventory.showGrayedOutSlots = true;

        //    this.behaviorOnClickPreviousButton = behaviorOnClickPreviousButton;
        //    this.behaviorOnClickNextButton = behaviorOnClickNextButton;
        //    this.behaviorOnClickOrganize = behaviorOnClickOrganize;

        //    if (snapToBottom)
        //    {
        //        this.movePosition(0, Game1.viewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
        //        this.snappedtoBottom = true;
        //    }

        //    this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen, false, inventory, highlightFunction);
        //    this.behaviorFunction = behaviorOnItemSelectFunction;
        //    this.behaviorOnItemGrab = behaviorOnItemGrab;
        //    this.canExitOnKey = canBeExitedWithKey;

        //    if (showOrganizeButton)
        //    {
        //        this.organizeButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - Game1.tileSize, Game1.tileSize, Game1.tileSize), "", "Organize", Game1.mouseCursors, new Rectangle(162, 440, 16, 16), (float)Game1.pixelZoom);
        //    }

        //    this.previousPageButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize, (this.yPositionOnScreen + this.height / 3 - Game1.tileSize) + 2, Game1.tileSize, Game1.tileSize), "", "Previous Page", Game1.mouseCursors, new Rectangle(352, 494, 12, 12), (float)Game1.pixelZoom);
        //    this.nextPageButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width + 2 * Game1.tileSize, (this.yPositionOnScreen + this.height / 3 - Game1.tileSize) + 2, Game1.tileSize, Game1.tileSize), "", "Next Page", Game1.mouseCursors, new Rectangle(365, 494, 12, 12), (float)Game1.pixelZoom);

        //    this.showPrevButton = bShowPrevButton;
        //    this.showNextButton = bShowNextButton;

        //    if (!Game1.isAnyGamePadButtonBeingPressed() && Game1.lastCursorMotionWasMouse || (this.ItemsToGrabMenu.actualInventory.Count <= 0 || Game1.activeClickableMenu != null))
        //    {
        //        return;
        //    }

        //    Game1.setMousePosition(this.inventory.inventory[0].bounds.Center);
        //}

        //M07
        //1. ItemGrabMenu.behaviorOnItemSelect to FridgeGrabMenu.behaviorOnItemSelect 
        //2. ItemGrabMenu.behaviorOnItemSelectFunction to FridgeGrabMenu.behaviorOnItemSelectFunction behaviorOnItemGrab
        //3. Add FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickNextButton = null, FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickPreviousButton = null, bool bShowPrevButton = false, bool bShowNextButton = false, FridgeGrabMenu.behaviorOnOrganizeItems behaviorOnClickOrganize = null at the end
        //DONE
        public FridgeGrabMenu(List<Item> inventory, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction, FridgeGrabMenu.behaviorOnItemSelect behaviorOnItemSelectFunction, string message, FridgeGrabMenu.behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false, bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true, bool showOrganizeButton = false, int source = 0, Item sourceItem = null, int whichSpecialButton = -1, object specialObject = null, FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickNextButton = null, FridgeGrabMenu.behaviorOnPageCtlClick behaviorOnClickPreviousButton = null, bool bShowPrevButton = false, bool bShowNextButton = false, FridgeGrabMenu.behaviorOnOrganizeItems behaviorOnClickOrganize = null) : base(highlightFunction, true, true, 0, 0)
		{
            int num;
            int num1;
            this.source = source;
            this.message = message;
            this.reverseGrab = reverseGrab;
            this.showReceivingMenu = showReceivingMenu;
            this.playRightClickSound = playRightClickSound;
            this.allowRightClick = allowRightClick;
            this.inventory.showGrayedOutSlots = true;
            this.sourceItem = sourceItem;

            //M07
            this.behaviorOnClickPreviousButton = behaviorOnClickPreviousButton;
            this.behaviorOnClickNextButton = behaviorOnClickNextButton;
            this.behaviorOnClickOrganize = behaviorOnClickOrganize;
            //END_M07

            if (source == 1 && sourceItem != null && sourceItem is Chest)
            {
                this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize - IClickableMenu.borderWidth * 2, 0, new Chest(true))
                {
                    colorSelection = this.chestColorPicker.getSelectionFromColor((sourceItem as Chest).playerChoiceColor)
                };
                (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
                ClickableTextureComponent clickableTextureComponent = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 5, 16 * Game1.pixelZoom, 16 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(119, 469, 16, 16), (float)Game1.pixelZoom, false)
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker", new object[0]),
                    myID = 27346,
                    downNeighborID = (showOrganizeButton ? 106 : 5948),
                    leftNeighborID = 11
                };
                this.colorPickerToggleButton = clickableTextureComponent;
            }
            this.whichSpecialButton = whichSpecialButton;
            this.specialObject = specialObject;
            if (whichSpecialButton == 1)
            {
                this.specialButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 5, 16 * Game1.pixelZoom, 16 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(108, 491, 16, 16), (float)Game1.pixelZoom, false)
                {
                    myID = 12485,
                    downNeighborID = (showOrganizeButton ? 106 : 5948)
                };
                if (specialObject != null && specialObject is JunimoHut)
                {
                    this.specialButton.sourceRect.X = ((specialObject as JunimoHut).noHarvest ? 124 : 108);
                }
            }
            if (snapToBottom)
            {
                base.movePosition(0, Game1.viewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
                this.snappedtoBottom = true;
            }
            //new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen, false, inventory, highlightFunction)
            this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen, false, inventory, highlightFunction, -1, 3, 0, 0, true);
            if (Game1.options.SnappyMenus)
            {
                this.ItemsToGrabMenu.populateClickableComponentList();
                for (int i = 0; i < this.ItemsToGrabMenu.inventory.Count; i++)
                {
                    if (this.ItemsToGrabMenu.inventory[i] != null)
                    {
                        ClickableComponent item = this.ItemsToGrabMenu.inventory[i];
                        item.myID = item.myID + 53910;
                        ClickableComponent clickableComponent = this.ItemsToGrabMenu.inventory[i];
                        clickableComponent.upNeighborID = clickableComponent.upNeighborID + 53910;
                        ClickableComponent item1 = this.ItemsToGrabMenu.inventory[i];
                        item1.rightNeighborID = item1.rightNeighborID + 53910;
                        this.ItemsToGrabMenu.inventory[i].downNeighborID = -7777;
                        ClickableComponent clickableComponent1 = this.ItemsToGrabMenu.inventory[i];
                        clickableComponent1.leftNeighborID = clickableComponent1.leftNeighborID + 53910;
                        this.ItemsToGrabMenu.inventory[i].fullyImmutable = true;
                    }
                }
            }
            this.behaviorFunction = behaviorOnItemSelectFunction;
            this.behaviorOnItemGrab = behaviorOnItemGrab;
            this.canExitOnKey = canBeExitedWithKey;

            //M07
            this.previousPageButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize, (this.yPositionOnScreen + this.height / 3 - Game1.tileSize) + 2, Game1.tileSize, Game1.tileSize), "", "Previous Page", Game1.mouseCursors, new Rectangle(352, 494, 12, 12), (float)Game1.pixelZoom);
            this.nextPageButton = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width + 2 * Game1.tileSize, (this.yPositionOnScreen + this.height / 3 - Game1.tileSize) + 2, Game1.tileSize, Game1.tileSize), "", "Next Page", Game1.mouseCursors, new Rectangle(365, 494, 12, 12), (float)Game1.pixelZoom);

            this.showPrevButton = bShowPrevButton;
            this.showNextButton = bShowNextButton;
            //END_M07

            if (showOrganizeButton)
            {
                ClickableTextureComponent clickableTextureComponent1 = new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - Game1.tileSize, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize", new object[0]), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(162, 440, 16, 16), (float)Game1.pixelZoom, false)
                {
                    myID = 106
                };
                if (this.colorPickerToggleButton != null)
                {
                    num1 = 27346;
                }
                else
                {
                    num1 = (this.specialButton != null ? 12485 : -500);
                }
                clickableTextureComponent1.upNeighborID = num1;
                clickableTextureComponent1.downNeighborID = 5948;
                this.organizeButton = clickableTextureComponent1;
            }
            if ((Game1.isAnyGamePadButtonBeingPressed() || !Game1.lastCursorMotionWasMouse) && this.ItemsToGrabMenu.actualInventory.Count > 0 && Game1.activeClickableMenu == null)
            {
                Game1.setMousePosition(this.inventory.inventory[0].bounds.Center);
            }
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                if (this.chestColorPicker != null)
                {
                    this.discreteColorPickerCC = new List<ClickableComponent>();
                    for (int j = 0; j < this.chestColorPicker.totalColors; j++)
                    {
                        this.discreteColorPickerCC.Add(new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + j * 9 * Game1.pixelZoom, this.chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 9 * Game1.pixelZoom, 7 * Game1.pixelZoom), "")
                        {
                            myID = j + 4343,
                            rightNeighborID = (j < this.chestColorPicker.totalColors - 1 ? j + 4343 + 1 : -1),
                            leftNeighborID = (j > 0 ? j + 4343 - 1 : -1),
                            downNeighborID = (this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.inventory.Count <= 0 ? 0 : 53910)
                        });
                    }
                }
                for (int k = 0; k < 12; k++)
                {
                    if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
                    {
                        ClickableComponent item2 = this.inventory.inventory[k];
                        if (this.shippingBin)
                        {
                            num = 12598;
                        }
                        else if (this.discreteColorPickerCC == null || this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.inventory.Count > k)
                        {
                            num = (this.ItemsToGrabMenu.inventory.Count > k ? 53910 + k : 53910);
                        }
                        else
                        {
                            num = 4343;
                        }
                        item2.upNeighborID = num;
                    }
                    if (this.discreteColorPickerCC != null && this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count > k)
                    {
                        this.ItemsToGrabMenu.inventory[k].upNeighborID = 4343;
                    }
                }
                if (!this.shippingBin)
                {
                    for (int l = 0; l < 36; l++)
                    {
                        if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count > l)
                        {
                            this.inventory.inventory[l].upNeighborID = -7777;
                            this.inventory.inventory[l].upNeighborImmutable = true;
                        }
                    }
                }
                if (this.trashCan != null && this.inventory.inventory.Count >= 12 && this.inventory.inventory[11] != null)
                {
                    this.inventory.inventory[11].rightNeighborID = 5948;
                }
                if (this.trashCan != null)
                {
                    this.trashCan.leftNeighborID = 11;
                }
                if (this.okButton != null)
                {
                    this.okButton.leftNeighborID = 11;
                }
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        //NOMOD
        public void initializeShippingBin()
        {
            this.shippingBin = true;
            this.lastShippedHolder = new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width / 2 - 12 * Game1.pixelZoom, this.yPositionOnScreen + this.height / 2 - 20 * Game1.pixelZoom - Game1.tileSize, 24 * Game1.pixelZoom, 24 * Game1.pixelZoom), "", Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem", new object[0]), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(293, 360, 24, 24), (float)Game1.pixelZoom, false)
            {
                myID = 12598,
                region = 12598
            };
            if (Game1.options.snappyMenus)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12)
                    {
                        this.inventory.inventory[i].upNeighborID = -7777;
                        if (i == 11)
                        {
                            this.inventory.inventory[i].rightNeighborID = 5948;
                        }
                    }
                }
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        //NOMOD
        public void setBackgroundTransparency(bool b)
        {
            this.drawBG = b;
        }

        //NOMOD
        public void setDestroyItemOnClick(bool b)
        {
            this.destroyItemOnClick = b;
        }

        //WAS:
        //public override void receiveRightClick(int x, int y, bool playSound = true)
        //{
        //    if (!this.allowRightClick)
        //        return;
        //    base.receiveRightClick(x, y, playSound && this.playRightClickSound);
        //    if (this.heldItem == null && this.showReceivingMenu)
        //    {
        //        this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
        //        if (this.heldItem != null && this.behaviorOnItemGrab != null)
        //            this.behaviorOnItemGrab(this.heldItem, Game1.player);
        //        if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).parentSheetIndex == 326)
        //        {
        //            this.heldItem = (Item)null;
        //            Game1.player.canUnderstandDwarves = true;
        //            this.poof = new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
        //            Game1.playSound("fireball");
        //        }
        //        else if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).isRecipe)
        //        {
        //            string key = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
        //            try
        //            {
        //                if ((this.heldItem as StardewValley.Object).category == -7)
        //                    Game1.player.cookingRecipes.Add(key, 0);
        //                else
        //                    Game1.player.craftingRecipes.Add(key, 0);
        //                this.poof = new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
        //                Game1.playSound("newRecipe");
        //            }
        //            catch (Exception ex)
        //            {
        //                StardewModdingAPI.Log.Debug(String.Format("FridgeGrabMenu Exception in receiveRightClick {0}", ex));
        //            }
        //            this.heldItem = (Item)null;
        //        }
        //        else
        //        {
        //            if (!Game1.player.addItemToInventoryBool(this.heldItem, false))
        //                return;
        //            this.heldItem = (Item)null;
        //            Game1.playSound("coin");
        //        }
        //    }
        //    else
        //    {
        //        if (!this.reverseGrab && this.behaviorFunction == null)
        //            return;
        //        this.behaviorFunction(this.heldItem, Game1.player);
        //        if (!this.destroyItemOnClick)
        //            return;
        //        this.heldItem = (Item)null;
        //    }
        //}

        //IS
        //DONE
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!this.allowRightClick)
            {
                return;
            }
            base.receiveRightClick(x, y, (!playSound ? false : this.playRightClickSound));
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
                    if (Game1.options.SnappyMenus)
                    {
                        (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                        (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
                    }
                }
                if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).parentSheetIndex == 326)
                {
                    this.heldItem = null;
                    Game1.player.canUnderstandDwarves = true;
                    this.poof = new TemporaryAnimatedSprite(Game1.animations, new Microsoft.Xna.Framework.Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                    Game1.playSound("fireball");
                    return;
                }
                if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).isRecipe)
                {
                    string str = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
                    try
                    {
                        if ((this.heldItem as StardewValley.Object).category != -7)
                        {
                            Game1.player.craftingRecipes.Add(str, 0);
                        }
                        else
                        {
                            Game1.player.cookingRecipes.Add(str, 0);
                        }
                        this.poof = new TemporaryAnimatedSprite(Game1.animations, new Microsoft.Xna.Framework.Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception exception)
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

        //DONE
        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
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
                this.organizeButton = new ClickableTextureComponent("", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - Game1.tileSize, Game1.tileSize, Game1.tileSize), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize", new object[0]), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(162, 440, 16, 16), (float)Game1.pixelZoom, false);
            }
            if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest)
            {
                this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize - IClickableMenu.borderWidth * 2, 0, null)
                {
                    colorSelection = this.chestColorPicker.getSelectionFromColor((this.sourceItem as Chest).playerChoiceColor)
                };
            }
        }

        //DONE
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, (this.destroyItemOnClick ? false : true));
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
                try
                {
                    Game1.playSound("drumkit6");
                }
                catch (Exception exception)
                {
                }
            }
            if (this.whichSpecialButton != -1 && this.specialButton != null && this.specialButton.containsPoint(x, y))
            {
                Game1.playSound("drumkit6");
                if (this.whichSpecialButton == 1 && this.specialObject != null && this.specialObject is JunimoHut)
                {
                    (this.specialObject as JunimoHut).noHarvest = !(this.specialObject as JunimoHut).noHarvest;
                    this.specialButton.sourceRect.X = ((this.specialObject as JunimoHut).noHarvest ? 124 : 108);
                }
            }
            if (this.heldItem == null && this.showReceivingMenu)
            {
                this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
                if (this.heldItem != null && this.behaviorOnItemGrab != null)
                {
                    this.behaviorOnItemGrab(this.heldItem, Game1.player);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                    {
                        (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
                        if (Game1.options.SnappyMenus)
                        {
                            (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                            (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
                        }
                    }
                }
                if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).parentSheetIndex == 326)
                {
                    this.heldItem = null;
                    Game1.player.canUnderstandDwarves = true;
                    this.poof = new TemporaryAnimatedSprite(Game1.animations, new Microsoft.Xna.Framework.Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                    Game1.playSound("fireball");
                }
                else if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).parentSheetIndex == 102)
                {
                    this.heldItem = null;
                    Game1.player.foundArtifact(102, 1);
                    this.poof = new TemporaryAnimatedSprite(Game1.animations, new Microsoft.Xna.Framework.Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                    Game1.playSound("fireball");
                }
                else if (this.heldItem is StardewValley.Object && (this.heldItem as StardewValley.Object).isRecipe)
                {
                    string str = this.heldItem.Name.Substring(0, this.heldItem.Name.IndexOf("Recipe") - 1);
                    try
                    {
                        if ((this.heldItem as StardewValley.Object).category != -7)
                        {
                            Game1.player.craftingRecipes.Add(str, 0);
                        }
                        else
                        {
                            Game1.player.cookingRecipes.Add(str, 0);
                        }
                        this.poof = new TemporaryAnimatedSprite(Game1.animations, new Microsoft.Xna.Framework.Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % Game1.tileSize + Game1.tileSize / 4), (float)(y - y % Game1.tileSize + Game1.tileSize / 4)), false, false);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception exception1)
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
                    if (Game1.options.SnappyMenus)
                    {
                        (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                        (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
                    }
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
                Game1.activeClickableMenu = new FridgeGrabMenu(this.ItemsToGrabMenu.actualInventory, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.behaviorFunction, null, this.behaviorOnItemGrab, false, true, true, true, true, this.source, this.sourceItem, -1, null);
                Game1.playSound("Ship");
                return;
            }

            if (this.previousPageButton != null && this.previousPageButton.containsPoint(x, y) && this.behaviorOnClickPreviousButton != null)
            {
                this.behaviorOnClickPreviousButton();
                Game1.playSound("Ship");
                return;
            }

            if (this.nextPageButton != null && this.nextPageButton.containsPoint(x, y) && this.behaviorOnClickNextButton != null)
            {
                this.behaviorOnClickNextButton();
                Game1.playSound("Ship");
                return;
            }

            if (this.heldItem != null && !this.isWithinBounds(x, y) && this.heldItem.canBeTrashed())
            {
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection, null);
                if (this.inventory.onAddItem != null)
                {
                    this.inventory.onAddItem(this.heldItem, Game1.player);
                }
                this.heldItem = null;
            }
        }

        //NOMOD
        public static void organizeItemsInList(List<Item> items)
        {
            items.Sort();
            items.Reverse();
        }

        //NOMOD
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

        //DONE
        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                base.applyMovementKey(key);
            }
            if ((this.canExitOnKey || this.areAllItemsTaken()) && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                base.exitThisMenu(true);
                if (Game1.currentLocation.currentEvent != null)
                {
                    Event currentCommand = Game1.currentLocation.currentEvent;
                    currentCommand.CurrentCommand = currentCommand.CurrentCommand + 1;
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

        //NOMOD
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

        //DONE
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
            if (this.specialButton != null)
            {
                this.specialButton.tryHover(x, y, 0.25f);
            }
            if (!this.ItemsToGrabMenu.isWithinBounds(x, y) || !this.showReceivingMenu)
            {
                base.performHoverAction(x, y);
            }
            else
            {
                this.hoveredItem = this.ItemsToGrabMenu.hover(x, y, this.heldItem);
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

            //M07: Needed to make previous/next page button usable
            if (this.previousPageButton.containsPoint(x, y) && this.showPrevButton)
            {
                this.hoverText = this.previousPageButton.hoverText;
            }

            if (this.nextPageButton.containsPoint(x, y) && this.showNextButton)
            {
                this.hoverText = this.nextPageButton.hoverText;
            }
            //END_M07

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

        //DONE
        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (direction != 2)
            {
                if (direction == 0)
                {
                    if (this.shippingBin && Game1.getFarm().lastItemShipped != null && oldID < 12)
                    {
                        this.currentlySnappedComponent = base.getComponentWithID(12598);
                        this.currentlySnappedComponent.downNeighborID = oldID;
                        this.snapCursorToCurrentSnappedComponent();
                        return;
                    }
                    if (oldID < 53910 && oldID >= 12)
                    {
                        this.currentlySnappedComponent = base.getComponentWithID(oldID - 12);
                        return;
                    }
                    int num = oldID + 24;
                    for (int i = 0; i < 3 && this.ItemsToGrabMenu.inventory.Count <= num; i++)
                    {
                        num = num - 12;
                    }
                    if (num >= 0)
                    {
                        this.currentlySnappedComponent = base.getComponentWithID(num + 53910);
                    }
                    else if (this.ItemsToGrabMenu.inventory.Count > 0)
                    {
                        this.currentlySnappedComponent = base.getComponentWithID(53910 + this.ItemsToGrabMenu.inventory.Count - 1);
                    }
                    else if (this.discreteColorPickerCC != null)
                    {
                        this.currentlySnappedComponent = base.getComponentWithID(4343);
                    }
                    this.snapCursorToCurrentSnappedComponent();
                }
                return;
            }
            for (int j = 0; j < 12; j++)
            {
                if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12 && this.shippingBin)
                {
                    this.inventory.inventory[j].upNeighborID = (this.shippingBin ? 12598 : Math.Min(j, this.ItemsToGrabMenu.inventory.Count - 1) + 53910);
                }
            }
            if (!this.shippingBin && oldID >= 53910)
            {
                int num1 = oldID - 53910;
                if (num1 + 12 <= this.ItemsToGrabMenu.inventory.Count - 1)
                {
                    this.currentlySnappedComponent = base.getComponentWithID(num1 + 12 + 53910);
                    this.snapCursorToCurrentSnappedComponent();
                    return;
                }
            }
            this.currentlySnappedComponent = base.getComponentWithID((oldRegion == 12598 ? 0 : (oldID - 53910) % 12));
            this.snapCursorToCurrentSnappedComponent();
        }

        //TODO
        public override void draw(SpriteBatch b)
        {
            if (this.drawBG)
            {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);
            }

            this.draw(b, false, false);

            if (this.showReceivingMenu)
            {
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 16 * Game1.pixelZoom), (float)(this.yPositionOnScreen + this.height / 2 + Game1.tileSize + Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 16 * Game1.pixelZoom), (float)(this.yPositionOnScreen + this.height / 2 + Game1.tileSize - Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 10 * Game1.pixelZoom), (float)(this.yPositionOnScreen + this.height / 2 + Game1.tileSize - Game1.pixelZoom * 11)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                if (this.source != 0)
                {
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 18 * Game1.pixelZoom), (float)(this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 18 * Game1.pixelZoom), (float)(this.yPositionOnScreen + Game1.tileSize - Game1.pixelZoom * 4)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                    Rectangle rectangle = new Rectangle((int)sbyte.MaxValue, 412, 10, 11);
                    switch (this.source)
                    {
                        case 2:
                            rectangle.X += 20;
                            break;
                        case 3:
                            rectangle.X += 10;
                            break;
                    }
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen - 13 * Game1.pixelZoom), (float)(this.yPositionOnScreen + Game1.tileSize - Game1.pixelZoom * 11)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                }
                Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, false, true, message, false);
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
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * -2), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 25)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * 21), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 25)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * -2), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 11)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(this.lastShippedHolder.bounds.X + Game1.pixelZoom * 21), (float)(this.lastShippedHolder.bounds.Bottom - Game1.pixelZoom * 11)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
            }

            if (this.organizeButton != null)
            {
                this.organizeButton.draw(b);
            }

            if (this.showPrevButton) { this.previousPageButton.draw(b); }
            if (this.showNextButton) { this.nextPageButton.draw(b); }

            if (this.hoverText != null && (this.hoveredItem == null || this.hoveredItem == null || this.ItemsToGrabMenu == null))
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            if (this.hoveredItem != null)
                IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.Name, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, (CraftingRecipe)null, -1);
            else if (this.hoveredItem != null && this.ItemsToGrabMenu != null)
                IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, (CraftingRecipe)null, -1);
            if (this.heldItem != null)
                this.heldItem.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 8), (float)(Game1.getOldMouseY() + 8)), 1f);
            Game1.mouseCursorTransparency = 1f;

            //IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), 200, 20, 880, 76,new Color(125f,125f,125f) , 1f, true);
            this.drawBorderLabel(b, message, Game1.smallFont, this.xPositionOnScreen, 0);

            this.drawMouse(b);
        }

        //NOMOD
        public void setSourceItem(Item item)
        {
            this.sourceItem = item;
            this.chestColorPicker = null;
            this.colorPickerToggleButton = null;
            if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest)
            {
                this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize - IClickableMenu.borderWidth * 2, 0, new Chest(true))
                {
                    colorSelection = this.chestColorPicker.getSelectionFromColor((this.sourceItem as Chest).playerChoiceColor)
                };
                (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
                ClickableTextureComponent clickableTextureComponent = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 5, 16 * Game1.pixelZoom, 16 * Game1.pixelZoom), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(119, 469, 16, 16), (float)Game1.pixelZoom, false)
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker", new object[0])
                };
                this.colorPickerToggleButton = clickableTextureComponent;
            }
        }

        //NOMOD
        public override void snapToDefaultClickableComponent()
        {
            if (!this.shippingBin)
            {
                this.currentlySnappedComponent = base.getComponentWithID((this.ItemsToGrabMenu.inventory.Count > 0 ? 53910 : 0));
            }
            else
            {
                this.currentlySnappedComponent = base.getComponentWithID(0);
            }
            this.snapCursorToCurrentSnappedComponent();
        }

        //NOMOD
        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (b == Buttons.Back && this.organizeButton != null)
            {
                FridgeGrabMenu.organizeItemsInList(Game1.player.items);
                Game1.playSound("Ship");
            }
        }


    }
}

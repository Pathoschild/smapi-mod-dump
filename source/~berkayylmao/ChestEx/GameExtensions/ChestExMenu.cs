/*
   MIT License

   Copyright (c) 2019 Berkay Yigit <berkay2578@gmail.com>
       Copyright holder detail: Nickname(s) used by the copyright holder: 'berkay2578', 'berkayylmao'.

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
*/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx {
   public class ChestExMenu : ItemGrabMenu {
      public class ArbitrationObjects {
         public InventoryMenu.highlightThisItem highlightFunction;
         public Boolean okButton = false;
         public Boolean trashCan = false;
         public Int32 inventoryXOffset = 0;
         public Int32 inventoryYOffset = 0;

         public ArbitrationObjects(InventoryMenu.highlightThisItem highlightFunction, Boolean okButton, Boolean trashCan, Int32 inventoryXOffset, Int32 inventoryYOffset) {
            this.highlightFunction = highlightFunction;
            this.okButton = okButton;
            this.trashCan = trashCan;
            this.inventoryXOffset = inventoryXOffset;
            this.inventoryYOffset = inventoryYOffset;
         }
      }

      private static class DeepBaseCallsGetter {
         private static IntPtr getMethodPointer(String strFuncName, Type[] methodParams) {
            try {
               return Harmony.AccessTools.Method(typeof(MenuWithInventory), strFuncName, methodParams).MethodHandle.GetFunctionPointer();
            } catch (NullReferenceException) {
               return Harmony.AccessTools.Method(typeof(IClickableMenu), strFuncName, methodParams).MethodHandle.GetFunctionPointer();
            }
         }
         public static T GetDeepBaseFunction<T>(ChestExMenu pInstance, String strFuncName, Type[] methodParams = null) {
            return (T)Activator.CreateInstance(typeof(T), pInstance, getMethodPointer(strFuncName, methodParams));
         }
      }
      private class DeepBaseCalls {
         public delegate void tEmergencyShutdown();
         public delegate void tPopulateClickableComponentList();
         public delegate void tDrawMouse(SpriteBatch b);
         public delegate void tUpdate(GameTime time);
         public delegate void tDraw(SpriteBatch b, Boolean drawUpperPortion, Boolean drawDescriptionArea, Int32 red, Int32 green, Int32 blue);
         public delegate void tPerformHoverAction(Int32 x, Int32 y);
         public delegate void tReceiveMouseClick(Int32 x, Int32 y, Boolean playSound);

         public tEmergencyShutdown pEmergencyShutDown;
         public tPopulateClickableComponentList pPopulateClickableComponentList;
         public tDrawMouse pDrawMouse;
         public tUpdate pUpdate;
         public tDraw pDraw;
         public tPerformHoverAction pPerformHoverAction;
         public tReceiveMouseClick pReceiveLeftClick;
         public tReceiveMouseClick pReceiveRightClick;
      }

      public static Int32 bgXDiff = 0;
      public static Int32 bgYDiff => Config.instance.rows > 3 ? 48 * (Config.instance.rows - 3) : 0;

      private DeepBaseCalls deepBaseCalls = new DeepBaseCalls();
      private Boolean defSnappyMenus;

      private behaviorOnItemSelect behaviorFunction;
      private Boolean essential;
      private String message;
      private Item sourceItem;

      private new ChestColorPickerEx chestColorPicker;
      private void setColorPicker() {
         this.chestColorPicker = new ChestColorPickerEx(this.ItemsToGrabMenu.xPositionOnScreen,
                                                        this.ItemsToGrabMenu.yPositionOnScreen,
                                                        this.ItemsToGrabMenu.width,
                                                        this.ItemsToGrabMenu.height,
                                                        sourceItem as Chest);

         this.colorPickerToggleButton = new ClickableTextureComponent(
             new Rectangle(this.inventory.xPositionOnScreen + this.inventory.width - bgXDiff + 60, this.ItemsToGrabMenu.yPositionOnScreen, 64, 64),
             Game1.mouseCursors,
             new Rectangle(119, 469, 16, 16),
             4f,
             false) {
            hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
            myID = ItemGrabMenu.region_colorPickToggle,
            downNeighborID = ItemGrabMenu.region_fillStacksButton,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region = ItemGrabMenu.region_organizationButtons
         };
      }
      private void setOrganizeButton() {
         this.fillStacksButton = new ClickableTextureComponent(
             new Rectangle(this.inventory.xPositionOnScreen + this.inventory.width - bgXDiff + 60, this.ItemsToGrabMenu.yPositionOnScreen + (this.colorPickerToggleButton != null ? 64 + 16 : 0), 64, 64),
             Game1.mouseCursors,
             new Rectangle(103, 469, 16, 16),
             4f,
             false) {
            hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"),
            myID = ItemGrabMenu.region_fillStacksButton,
            upNeighborID = (this.colorPickerToggleButton != null) ? this.colorPickerToggleButton.myID : -500,
            downNeighborID = ItemGrabMenu.region_organizeButton,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region = ItemGrabMenu.region_organizationButtons
         };
         this.organizeButton = new ClickableTextureComponent(
             new Rectangle(this.fillStacksButton.bounds.X, this.fillStacksButton.bounds.Y + 64 + 16, 64, 64),
             Game1.mouseCursors,
             new Rectangle(162, 440, 16, 16),
             4f,
             false) {
            hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"),
            myID = ItemGrabMenu.region_organizeButton,
            upNeighborID = ItemGrabMenu.region_fillStacksButton,
            downNeighborID = MenuWithInventory.region_trashCan,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region = ItemGrabMenu.region_organizationButtons
         };
         this.trashCan = new ClickableTextureComponent(
             new Rectangle(this.organizeButton.bounds.X, this.organizeButton.bounds.Y + 64 + 64 + 16, 64, 104),
             Game1.mouseCursors,
             new Rectangle(564 + (Game1.player.trashCanLevel * 18), 102, 18, 26),
             4f,
             false) {
            myID = MenuWithInventory.region_trashCan,
            upNeighborID = ItemGrabMenu.region_organizeButton,
            downNeighborID = MenuWithInventory.region_okButton,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region = ItemGrabMenu.region_organizationButtons
         };
         this.okButton.bounds.Y += bgYDiff;
      }
      public new void SetupBorderNeighbors() {
         List<ClickableComponent> border = this.inventory.GetBorder(InventoryMenu.BorderSide.Right);
         foreach (ClickableComponent clickableComponent in border) {
            clickableComponent.rightNeighborID = ClickableComponent.SNAP_AUTOMATIC;
            clickableComponent.rightNeighborImmutable = true;
         }
         border = this.inventory.GetBorder(InventoryMenu.BorderSide.Top);
         foreach (ClickableComponent slot in border) {
            slot.upNeighborID = ClickableComponent.CUSTOM_SNAP_BEHAVIOR;
            slot.upNeighborImmutable = true;
         }

         border = this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right);
         foreach (ClickableComponent slot in border) {
            slot.rightNeighborID = ClickableComponent.SNAP_AUTOMATIC;
            slot.rightNeighborImmutable = true;
         }
      }

      protected override void cleanupBeforeExit() {
         Game1.options.snappyMenus = this.defSnappyMenus;
      }
      public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
         if (this.ItemsToGrabMenu != null)
            this.ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);

         if (this.organizeButton != null)
            setOrganizeButton();

         if (this.source == ItemGrabMenu.source_chest && this.sourceItem != null && this.sourceItem is Chest)
            setColorPicker();
      }

      protected override void customSnapBehavior(Int32 direction, Int32 oldRegion, Int32 oldID) {
         if (direction == 2) {
            if (oldID >= ItemGrabMenu.region_itemsToGrabMenuModifier) {
               Int32 index = oldID - ItemGrabMenu.region_itemsToGrabMenuModifier;
               if (index + Config.instance.columns <= this.ItemsToGrabMenu.inventory.Count - 1) {
                  this.currentlySnappedComponent = base.getComponentWithID(index + Config.instance.columns + ItemGrabMenu.region_itemsToGrabMenuModifier);
                  this.snapCursorToCurrentSnappedComponent();
                  return;
               }
            }
            this.currentlySnappedComponent = base.getComponentWithID((oldID - ItemGrabMenu.region_itemsToGrabMenuModifier) % Config.instance.columns);
            this.snapCursorToCurrentSnappedComponent();
            return;
         }
         if (direction == 0) {
            if (oldID < ItemGrabMenu.region_itemsToGrabMenuModifier && oldID >= 12) {
               this.currentlySnappedComponent = base.getComponentWithID(oldID - 12);
               return;
            }
            Int32 id = oldID + (Config.instance.columns * (Config.instance.rows - 1));
            for (Int32 j = 0; j < Config.instance.rows && this.ItemsToGrabMenu.inventory.Count <= id; j++) {
               id -= Config.instance.columns;
            }
            if (this.showReceivingMenu) {
               if (id < 0) {
                  if (this.ItemsToGrabMenu.inventory.Count > 0) {
                     this.currentlySnappedComponent = base.getComponentWithID(ItemGrabMenu.region_itemsToGrabMenuModifier + this.ItemsToGrabMenu.inventory.Count - 1);
                  }
               } else {
                  this.currentlySnappedComponent = base.getComponentWithID(id + ItemGrabMenu.region_itemsToGrabMenuModifier);
               }
            }
            this.snapCursorToCurrentSnappedComponent();
         }
      }
      public override void receiveLeftClick(Int32 x, Int32 y, Boolean playSound = true) {
         deepBaseCalls.pReceiveLeftClick(x, y, !this.destroyItemOnClick);
         if (this.chestColorPicker != null && this.chestColorPicker.visible && this.chestColorPicker.isWithinBounds(x, y)) {
            this.chestColorPicker.receiveLeftClick(x, y, true);
            return;
         }
         if (this.colorPickerToggleButton != null && this.colorPickerToggleButton.containsPoint(x, y)) {
            this.chestColorPicker.visible = Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
            foreach (var item in this.ItemsToGrabMenu.inventory)
               item.visible = !this.chestColorPicker.visible;
            Game1.options.snappyMenus = this.chestColorPicker.visible ? false : defSnappyMenus;

            try {
               Game1.playSound("drumkit6");
            } catch (Exception) {
            }
            return;
         }
         if (this.heldItem == null && this.showReceivingMenu) {
            this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
            if (this.heldItem != null && this.behaviorOnItemGrab != null) {
               this.behaviorOnItemGrab(this.heldItem, Game1.player);
               if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ChestExMenu) {
                  (Game1.activeClickableMenu as ChestExMenu).setSourceItem(this.sourceItem);
                  if (Game1.options.SnappyMenus) {
                     (Game1.activeClickableMenu as ChestExMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                     (Game1.activeClickableMenu as ChestExMenu).snapCursorToCurrentSnappedComponent();
                  }
               }
            }
            if (Game1.player.addItemToInventoryBool(this.heldItem, false)) {
               this.heldItem = null;
               Game1.playSound("coin");
               return;
            }
         } else if (this.reverseGrab || this.behaviorFunction != null) {
            this.behaviorFunction(this.heldItem, Game1.player);
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ChestExMenu) {
               (Game1.activeClickableMenu as ChestExMenu).setSourceItem(this.sourceItem);
               if (Game1.options.SnappyMenus) {
                  (Game1.activeClickableMenu as ChestExMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                  (Game1.activeClickableMenu as ChestExMenu).snapCursorToCurrentSnappedComponent();
               }
            }
            if (this.destroyItemOnClick) {
               this.heldItem = null;
               return;
            }
         }
         if (this.organizeButton != null && this.organizeButton.containsPoint(x, y)) {
            organizeItemsInList(this.ItemsToGrabMenu.actualInventory);
            Game1.activeClickableMenu = new ChestExMenu(
               this.ItemsToGrabMenu.actualInventory,
               false,
               true,
               new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
               this.behaviorFunction,
               null,
               this.behaviorOnItemGrab,
               false,
               true,
               true,
               true,
               true,
               this.source,
               this.sourceItem,
               -1,
               this.context).setEssential(this.essential);
            (Game1.activeClickableMenu as ChestExMenu).heldItem = this.heldItem;
            Game1.playSound("Ship");
            return;
         }
         if (this.fillStacksButton != null && this.fillStacksButton.containsPoint(x, y)) {
            this.FillOutStacks();
            Game1.playSound("Ship");
            return;
         }
      }
      public override void receiveRightClick(Int32 x, Int32 y, Boolean playSound = true) {
         if (!this.allowRightClick)
            return;

         deepBaseCalls.pReceiveRightClick(x, y, playSound && this.playRightClickSound);
         if (this.heldItem == null && this.showReceivingMenu) {
            this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
            if (this.heldItem != null && this.behaviorOnItemGrab != null) {
               this.behaviorOnItemGrab(this.heldItem, Game1.player);
               if (Game1.options.SnappyMenus) {
                  (Game1.activeClickableMenu as ChestExMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                  (Game1.activeClickableMenu as ChestExMenu).snapCursorToCurrentSnappedComponent();
               }
            }

            if (Game1.player.addItemToInventoryBool(this.heldItem, false)) {
               this.heldItem = null;
               Game1.playSound("coin");
            }
         } else if (this.reverseGrab || this.behaviorFunction != null) {
            this.behaviorFunction(this.heldItem, Game1.player);
            if (this.destroyItemOnClick) {
               this.heldItem = null;
            }
         }
      }

      private void fixInternalDrawCallBoundaries() {
         this.yPositionOnScreen += bgYDiff;
      }
      private void unFixInternalDrawCallBoundaries() {
         this.yPositionOnScreen -= bgYDiff;
      }

      private Boolean needToUnFixExternalDrawCallBoundaries = false;
      private void fixExternalDrawCallBoundaries() {
         needToUnFixExternalDrawCallBoundaries = true;
         this.xPositionOnScreen += bgXDiff;
      }
      private void unFixExternalDrawCallBoundaries() {
         if (needToUnFixExternalDrawCallBoundaries)
            this.xPositionOnScreen -= bgXDiff;
         needToUnFixExternalDrawCallBoundaries = false;
      }
      public override void draw(SpriteBatch b) {
         unFixExternalDrawCallBoundaries();
         if (this.drawBG)
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);

         fixInternalDrawCallBoundaries();
         deepBaseCalls.pDraw(b, false, false, -1, -1, -1);
         unFixInternalDrawCallBoundaries();

         if (this.showReceivingMenu) {
            fixInternalDrawCallBoundaries();
            // backpack icon bg
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - 64, this.yPositionOnScreen + (this.height / 2) + 64 + 16), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - 64, this.yPositionOnScreen + (this.height / 2) + 64 - 16), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            // backpack icon
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - 40, this.yPositionOnScreen + (this.height / 2) + 64 - 44), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            unFixInternalDrawCallBoundaries();

            Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder,
               this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16,
               this.ItemsToGrabMenu.width + (IClickableMenu.borderWidth * 2) + (IClickableMenu.spaceToClearSideBorder * 2),
               this.ItemsToGrabMenu.height + (IClickableMenu.borderWidth * 2) + IClickableMenu.spaceToClearTopBorder,
               false, true, null, false, true);
            this.ItemsToGrabMenu.draw(b);
            // chest icon bg
            b.Draw(Game1.mouseCursors, new Vector2(this.inventory.xPositionOnScreen + bgXDiff - 105, this.yPositionOnScreen + 64 + 16), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(this.inventory.xPositionOnScreen + bgXDiff - 105, this.yPositionOnScreen + 64 - 16), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            // chest icon
            b.Draw(Game1.mouseCursors, new Vector2(this.inventory.xPositionOnScreen + bgXDiff - 85, this.yPositionOnScreen + 64 - 44), new Rectangle?(new Rectangle(127, 412, 10, 11)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
         } else if (this.message != null) {
            Game1.drawDialogueBox(Game1.viewport.Width / 2, this.ItemsToGrabMenu.yPositionOnScreen + (this.ItemsToGrabMenu.height / 2), false, false, this.message);
         }
         foreach (ItemGrabMenu.TransferredItemSprite transferredItemSprite in this._transferredItemSprites)
            transferredItemSprite.Draw(b);
         if (this.colorPickerToggleButton != null)
            this.colorPickerToggleButton.draw(b);
         if (this.chestColorPicker != null)
            this.chestColorPicker.draw(b);
         if (this.organizeButton != null)
            this.organizeButton.draw(b);
         if (this.fillStacksButton != null)
            this.fillStacksButton.draw(b);
         if ((this.hoverText != null && !String.IsNullOrWhiteSpace(this.hoverText)) && (this.hoveredItem == null || this.hoveredItem == null || this.ItemsToGrabMenu == null)) {
            if (this.hoverAmount > 0)
               IClickableMenu.drawToolTip(b, this.hoverText, "", null, true, -1, 0, -1, -1, null, this.hoverAmount);
            else
               IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
         }
         if (this.heldItem != null)
            this.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
         if (this.hoveredItem != null) {
            IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
         } else if (this.hoveredItem != null && this.ItemsToGrabMenu != null) {
            IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
         }
         Game1.mouseCursorTransparency = 1f;

         deepBaseCalls.pDrawMouse(b);
         fixExternalDrawCallBoundaries();
      }
      public override void performHoverAction(Int32 x, Int32 y) {
         this.hoveredItem = null;
         this.hoverText = "";
         deepBaseCalls.pPerformHoverAction(x, y);
         if (this.colorPickerToggleButton != null) {
            this.colorPickerToggleButton.tryHover(x, y, 0.25f);
            if (this.colorPickerToggleButton.containsPoint(x, y)) {
               this.hoverText = this.colorPickerToggleButton.hoverText;
            }
         }
         if (this.organizeButton != null) {
            this.organizeButton.tryHover(x, y, 0.25f);
            if (this.organizeButton.containsPoint(x, y)) {
               this.hoverText = this.organizeButton.hoverText;
            }
         }
         if (this.fillStacksButton != null) {
            this.fillStacksButton.tryHover(x, y, 0.25f);
            if (this.fillStacksButton.containsPoint(x, y)) {
               this.hoverText = this.fillStacksButton.hoverText;
            }
         }
         if (this.specialButton != null) {
            this.specialButton.tryHover(x, y, 0.25f);
         }
         if (this.showReceivingMenu) {
            Item item_grab_hovered_item = this.ItemsToGrabMenu.hover(x, y, this.heldItem);
            if (item_grab_hovered_item != null) {
               this.hoveredItem = item_grab_hovered_item;
            }
         }
         if (this.chestColorPicker != null) {
            this.chestColorPicker.performHoverAction(x, y);
         }
      }

      public override void emergencyShutDown() {
         deepBaseCalls.pEmergencyShutDown();
         Console.WriteLine("ChestExMenu.emergencyShutDown");
         Game1.options.snappyMenus = this.defSnappyMenus;
         if (this.heldItem != null) {
            Console.WriteLine("Taking " + this.heldItem.Name);
            this.heldItem = Game1.player.addItemToInventory(this.heldItem);
         }
         if (this.heldItem != null) {
            Game1.playSound("throwDownITem");
            Console.WriteLine("Dropping " + this.heldItem.Name);
            Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection, null, -1);
            this.heldItem = null;
         }
         if (this.essential) {
            Console.WriteLine("essential");
            using (IEnumerator<Item> enumerator = this.ItemsToGrabMenu.actualInventory.GetEnumerator()) {
               while (enumerator.MoveNext()) {
                  Item item = enumerator.Current;
                  if (item != null) {
                     Console.WriteLine("Taking " + item.Name);
                     Item leftOver = Game1.player.addItemToInventory(item);
                     if (leftOver != null) {
                        Console.WriteLine("Dropping " + leftOver.Name);
                        Game1.createItemDebris(leftOver, Game1.player.getStandingPosition(), Game1.player.FacingDirection, null, -1);
                     }
                  }
               }
               return;
            }
         }
         Console.WriteLine("essential");
      }
      public new ChestExMenu setEssential(Boolean essential) {
         this.essential = essential;
         return this;
      }
      public new void setSourceItem(Item item) {
         this.sourceItem = item;
         this.chestColorPicker = null;
         this.colorPickerToggleButton = null;

         if (this.source == ItemGrabMenu.source_chest && this.sourceItem != null && this.sourceItem is Chest)
            setColorPicker();

      }

      public ChestExMenu(IList<Item> inventory, Boolean reverseGrab, Boolean showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction, behaviorOnItemSelect behaviorOnItemSelectFunction, String message, behaviorOnItemSelect behaviorOnItemGrab = null, Boolean snapToBottom = false, Boolean canBeExitedWithKey = false, Boolean playRightClickSound = true, Boolean allowRightClick = true, Boolean showOrganizeButton = false, Int32 source = 0, Item sourceItem = null, Int32 whichSpecialButton = -1, System.Object context = null)
          : base(null, new ArbitrationObjects(highlightFunction, true, false, 0, bgYDiff)) {
         this.source = source;
         this.message = message;
         this.reverseGrab = reverseGrab;
         this.showReceivingMenu = showReceivingMenu;
         this.playRightClickSound = playRightClickSound;
         this.allowRightClick = allowRightClick;
         this.inventory.showGrayedOutSlots = true;
         this.sourceItem = sourceItem;
         this.context = context;
         this.behaviorFunction = behaviorOnItemSelectFunction;
         this.behaviorOnItemGrab = behaviorOnItemGrab;
         this.canExitOnKey = canBeExitedWithKey;

         // get MenuWithInventory/IClickable calls
         {
            deepBaseCalls.pEmergencyShutDown = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tEmergencyShutdown>(this, "emergencyShutDown");
            deepBaseCalls.pPopulateClickableComponentList = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tPopulateClickableComponentList>(this, "populateClickableComponentList");
            deepBaseCalls.pDrawMouse = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tDrawMouse>(this, "drawMouse", new Type[] { typeof(SpriteBatch) });
            deepBaseCalls.pUpdate = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tUpdate>(this, "update", new Type[] { typeof(GameTime) });
            deepBaseCalls.pDraw = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tDraw>(this, "draw", new Type[] { typeof(SpriteBatch), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Int32), typeof(Int32) });
            deepBaseCalls.pPerformHoverAction = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tPerformHoverAction>(this, "performHoverAction", new Type[] { typeof(Int32), typeof(Int32) });
            deepBaseCalls.pReceiveLeftClick = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tReceiveMouseClick>(this, "receiveLeftClick", new Type[] { typeof(Int32), typeof(Int32), typeof(Boolean) });
            deepBaseCalls.pReceiveRightClick = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tReceiveMouseClick>(this, "receiveRightClick", new Type[] { typeof(Int32), typeof(Int32), typeof(Boolean) });
         }
         // calc bgXDiff
         {
            if (Config.instance.columns < 12) { // game's default column amount
               bgXDiff = (12 - Config.instance.columns) * 32;
            } else {
               bgXDiff = (Config.instance.columns - 12) * -32;
            }
         }

         this.inventory.yPositionOnScreen += bgYDiff;
         foreach (var item in this.inventory.inventory) {
            item.bounds.Y += bgYDiff;
         }
         this.ItemsToGrabMenu = new InventoryMenu(this.inventory.xPositionOnScreen + bgXDiff,
                                     this.inventory.yPositionOnScreen + 16 - (64 * Config.instance.rows) - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder,
                                     false, inventory, null, Config.instance.getCapacity(), Config.instance.rows, 0, 0, true);
         this.ItemsToGrabMenu.populateClickableComponentList();
         for (int i = 0; i < this.ItemsToGrabMenu.inventory.Count; i++) {
            if (this.ItemsToGrabMenu.inventory[i] != null) {
               this.ItemsToGrabMenu.inventory[i].myID += ItemGrabMenu.region_itemsToGrabMenuModifier;
               this.ItemsToGrabMenu.inventory[i].upNeighborID += ItemGrabMenu.region_itemsToGrabMenuModifier;
               this.ItemsToGrabMenu.inventory[i].rightNeighborID += ItemGrabMenu.region_itemsToGrabMenuModifier;
               this.ItemsToGrabMenu.inventory[i].downNeighborID = ClickableComponent.CUSTOM_SNAP_BEHAVIOR;
               this.ItemsToGrabMenu.inventory[i].leftNeighborID += ItemGrabMenu.region_itemsToGrabMenuModifier;
            }
         }

         if (source == ItemGrabMenu.source_chest && sourceItem != null && sourceItem is Chest) {
            setColorPicker();
            this.chestColorPicker.visible = Game1.player.showChestColorPicker = false;
            this.defSnappyMenus = Game1.options.snappyMenus;
         }
         if (showOrganizeButton)
            setOrganizeButton();

         if (this.trashCan != null && this.inventory.inventory.Count >= 12 && this.inventory.inventory[11] != null)
            this.inventory.inventory[11].rightNeighborID = this.trashCan.myID;

         if (this.okButton != null)
            this.okButton.leftNeighborID = 11;

         deepBaseCalls.pPopulateClickableComponentList();
         if (Game1.options.SnappyMenus)
            this.snapToDefaultClickableComponent();
         SetupBorderNeighbors();
      }
   }
}

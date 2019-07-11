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
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using Object = StardewValley.Object;

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
         public delegate void tApplyMovementKey(Keys key);
         public delegate void tDrawMouse(SpriteBatch b);
         public delegate void tExitThisMenu(Boolean playSound);
         public delegate void tReceiveGamePadButton(Buttons b);
         public delegate void tUpdate(GameTime time);
         public delegate void tMovePosition(Int32 dx, Int32 dy);
         public delegate void tPerformHoverAction(Int32 x, Int32 y);
         public delegate void tDraw(SpriteBatch b, Boolean drawUpperPortion, Boolean drawDescriptionArea);
         public delegate void tReceiveLeftClick(Int32 x, Int32 y, Boolean playSound);
         public delegate void tReceiveRightClick(Int32 x, Int32 y, Boolean playSound);
         public delegate ClickableComponent tGetComponentWithID(Int32 id);

         public tEmergencyShutdown pEmergencyShutDown;
         public tPopulateClickableComponentList pPopulateClickableComponentList;
         public tApplyMovementKey pApplyMovementKey;
         public tDrawMouse pDrawMouse;
         public tExitThisMenu pExitThisMenu;
         public tReceiveGamePadButton pReceiveGamePadButton;
         public tUpdate pUpdate;
         public tMovePosition pMovePosition;
         public tPerformHoverAction pPerformHoverAction;
         public tDraw pDraw;
         public tReceiveLeftClick pReceiveLeftClick;
         public tReceiveRightClick pReceiveRightClick;
         public tGetComponentWithID pGetComponentWithID;
      }

      public static Int32 bgXDiff = 0;
      public static Int32 bgYDiff => Config.instance.rows > 3 ? 50 * (Config.instance.rows - 3) : 0;

      public new delegate void behaviorOnItemSelect(Item item, Farmer who);
      private DeepBaseCalls deepBaseCalls = new DeepBaseCalls();

      private behaviorOnItemSelect behaviorFunction;
      private new behaviorOnItemSelect behaviorOnItemGrab;
      private Boolean essential;
      private String message;
      private TemporaryAnimatedSprite poof;
      private Item sourceItem;
      private Boolean snappedtoBottom;

      private void setColorPicker() {
         this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - bgYDiff - (IClickableMenu.borderWidth * 3), 0, new Chest(true));
         this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((Netcode.NetColor)(sourceItem as Chest).playerChoiceColor);
         (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
         this.colorPickerToggleButton = new ClickableTextureComponent(
             new Rectangle(this.inventory.xPositionOnScreen + this.inventory.width - bgXDiff + 60, this.yPositionOnScreen - bgYDiff - IClickableMenu.borderWidth, 64, 64),
             Game1.mouseCursors,
             new Rectangle(119, 469, 16, 16),
             4f,
             false) {
            hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
            myID = 27346,
            downNeighborID = 106,
            leftNeighborID = 11
         };
      }
      private void setOrganizeButton() {
         this.organizeButton = new ClickableTextureComponent(
             new Rectangle(this.inventory.xPositionOnScreen + this.inventory.width - bgXDiff + 60, this.yPositionOnScreen - bgYDiff - IClickableMenu.borderWidth + 70, 64, 64),
             Game1.mouseCursors,
             new Rectangle(162, 440, 16, 16),
             4f,
             false) {
            hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"),
            myID = 106,
            upNeighborID = 27346,
            downNeighborID = 5948
         };
         this.trashCan = new ClickableTextureComponent(
             new Rectangle(this.organizeButton.bounds.X, this.inventory.yPositionOnScreen, 64, 104),
             Game1.mouseCursors,
             new Rectangle(669, 261, 16, 26),
             4f,
             false) {
            myID = 5948,
            leftNeighborID = 12,
            upNeighborID = 106
         };
         this.okButton.upNeighborID = -1;
         this.okButton.bounds.Y += bgYDiff;
      }
      public new ChestExMenu setEssential(Boolean essential) {
         this.essential = essential;
         return this;
      }

      protected override void customSnapBehavior(Int32 direction, Int32 oldRegion, Int32 oldID) {
         if (direction == 2) {
            for (Int32 i = 0; i < 12; i++) {
               if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12 && this.shippingBin) {
                  this.inventory.inventory[i].upNeighborID = (this.shippingBin ? 12598 : (Math.Min(i, this.ItemsToGrabMenu.inventory.Count - 1) + 53910));
               }
            }
            if (!this.shippingBin && oldID >= 53910) {
               Int32 index = oldID - 53910;
               if (index + 12 <= this.ItemsToGrabMenu.inventory.Count - 1) {
                  this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID(index + 12 + 53910);
                  this.snapCursorToCurrentSnappedComponent();
                  return;
               }
            }
            this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID((oldRegion == 12598) ? 0 : ((oldID - 53910) % 12));
            this.snapCursorToCurrentSnappedComponent();
            return;
         }
         if (direction == 0) {
            if (oldID < 53910 && oldID >= 12) {
               this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID(oldID - 12);
               return;
            }
            Int32 id = oldID + 24;
            Int32 j = 0;
            while (j < 3 && this.ItemsToGrabMenu.inventory.Count <= id) {
               id -= 12;
               j++;
            }
            if (this.showReceivingMenu) {
               if (id < 0) {
                  if (this.ItemsToGrabMenu.inventory.Count > 0) {
                     this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID(53910 + this.ItemsToGrabMenu.inventory.Count - 1);
                  } else if (this.discreteColorPickerCC != null) {
                     this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID(4343);
                  }
               } else {
                  this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID(id + 53910);
               }
            }
            this.snapCursorToCurrentSnappedComponent();
         }
      }
      public override void snapToDefaultClickableComponent() {
         if (this.shippingBin) {
            this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID(0);
         } else {
            this.currentlySnappedComponent = deepBaseCalls.pGetComponentWithID((this.ItemsToGrabMenu.inventory.Count > 0 && this.showReceivingMenu) ? 53910 : 0);
         }
         this.snapCursorToCurrentSnappedComponent();
      }

      public override void update(GameTime time) {
         deepBaseCalls.pUpdate(time);
         if (this.poof != null && this.poof.update(time)) {
            this.poof = null;
         }
         if (this.chestColorPicker != null) {
            this.chestColorPicker.update(time);
         }
      }
      public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
         if (this.snappedtoBottom) {
            deepBaseCalls.pMovePosition((newBounds.Width - oldBounds.Width) / 2, Game1.viewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
         }
         if (this.ItemsToGrabMenu != null) {
            this.ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
         }
         if (this.organizeButton != null) {
            setOrganizeButton();
         }
         if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest) {
            setColorPicker();
         }
      }
      public override void performHoverAction(Int32 x, Int32 y) {
         if (this.colorPickerToggleButton != null) {
            this.colorPickerToggleButton.tryHover(x, y, 0.25f);
            if (this.colorPickerToggleButton.containsPoint(x, y)) {
               this.hoverText = this.colorPickerToggleButton.hoverText;
               return;
            }
         }
         if (this.ItemsToGrabMenu.isWithinBounds(x, y) && this.showReceivingMenu) {
            this.hoveredItem = this.ItemsToGrabMenu.hover(x, y, this.heldItem);
         } else {
            deepBaseCalls.pPerformHoverAction(x, y);
         }
         if (this.organizeButton != null) {
            this.hoverText = null;
            this.organizeButton.tryHover(x, y, 0.1f);
            if (this.organizeButton.containsPoint(x, y)) {
               this.hoverText = this.organizeButton.hoverText;
            }
         }
         if (this.chestColorPicker != null) {
            this.chestColorPicker.performHoverAction(x, y);
         }
      }

      public override void receiveLeftClick(Int32 x, Int32 y, Boolean playSound = true) {
         deepBaseCalls.pReceiveLeftClick(x, y, !this.destroyItemOnClick);
         if (this.chestColorPicker != null) {
            this.chestColorPicker.receiveLeftClick(x, y, true);
            if (this.sourceItem != null && this.sourceItem is Chest) {
               (this.sourceItem as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
            }
         }
         if (this.colorPickerToggleButton != null && this.colorPickerToggleButton.containsPoint(x, y)) {
            Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
            this.chestColorPicker.visible = Game1.player.showChestColorPicker;
            try {
               Game1.playSound("drumkit6");
            } catch (Exception) {
            }
         }
         if (this.heldItem == null && this.showReceivingMenu) {
            this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
            if (this.heldItem != null && this.behaviorOnItemGrab != null) {
               this.behaviorOnItemGrab(this.heldItem, Game1.player);
               if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ChestExMenu) {
                  if (Game1.options.SnappyMenus) {
                     (Game1.activeClickableMenu as ChestExMenu).currentlySnappedComponent = this.currentlySnappedComponent;
                     (Game1.activeClickableMenu as ChestExMenu).snapCursorToCurrentSnappedComponent();
                  }
               }
            }
            if (Game1.player.addItemToInventoryBool(this.heldItem, false)) {
               this.heldItem = null;
               Game1.playSound("coin");
            }
         } else if ((this.reverseGrab || this.behaviorFunction != null) && this.isWithinBounds(x, y)) {
            this.behaviorFunction(this.heldItem, Game1.player);
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ChestExMenu) {
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
      public override void receiveKeyPress(Keys key) {
         if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            deepBaseCalls.pApplyMovementKey(key);

         if ((this.canExitOnKey || this.areAllItemsTaken()) && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose()) {
            deepBaseCalls.pExitThisMenu(true);
            if (Game1.currentLocation.currentEvent != null) {
               Event currentEvent = Game1.currentLocation.currentEvent;
               Int32 currentCommand = currentEvent.CurrentCommand;
               currentEvent.CurrentCommand = currentCommand + 1;
            }
         }
         if (key == Keys.Delete && this.heldItem != null && this.heldItem.canBeTrashed()) {
            if (this.heldItem is Object && Game1.player.specialItems.Contains((this.heldItem as Object).ParentSheetIndex)) {
               Game1.player.specialItems.Remove((this.heldItem as Object).ParentSheetIndex);
            }
            this.heldItem = null;
            Game1.playSound("trashcan");
         }
      }
      public override void receiveGamePadButton(Buttons b) {
         deepBaseCalls.pReceiveGamePadButton(b);
         if (b == Buttons.Back && this.organizeButton != null) {
            organizeItemsInList(Game1.player.Items);
            Game1.playSound("Ship");
         }
      }

      public override void draw(SpriteBatch b) {
         if (this.drawBG)
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);

         deepBaseCalls.pDraw(b, false, false);
         if (this.showReceivingMenu) {
            // backpack icon bg
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - 64, this.yPositionOnScreen + (this.height / 2) + 64 + 16), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - 64, this.yPositionOnScreen + (this.height / 2) + 64 - 16), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            // backpack icon
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - 40, this.yPositionOnScreen + (this.height / 2) + 64 - 44), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + (IClickableMenu.borderWidth * 2) + (IClickableMenu.spaceToClearSideBorder * 2), this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + (IClickableMenu.borderWidth * 2), false, true, null, false, false);
            this.ItemsToGrabMenu.draw(b);
            // chest icon bg
            b.Draw(Game1.mouseCursors, new Vector2(this.inventory.xPositionOnScreen + bgXDiff - 105, this.yPositionOnScreen + 64 + 16), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(this.inventory.xPositionOnScreen + bgXDiff - 105, this.yPositionOnScreen + 64 - 16), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            // chest icon
            b.Draw(Game1.mouseCursors, new Vector2(this.inventory.xPositionOnScreen + bgXDiff - 85, this.yPositionOnScreen + 64 - 44), new Rectangle?(new Rectangle(127, 412, 10, 11)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
         } else if (this.message != null) {
            Game1.drawDialogueBox(Game1.viewport.Width / 2, this.ItemsToGrabMenu.yPositionOnScreen + (this.ItemsToGrabMenu.height / 2), false, false, this.message);
         }
         if (this.poof != null)
            this.poof.draw(b, true, 0, 0, 1f);
         if (this.colorPickerToggleButton != null)
            this.colorPickerToggleButton.draw(b);
         if (this.chestColorPicker != null)
            this.chestColorPicker.draw(b);
         if (this.organizeButton != null)
            this.organizeButton.draw(b);
         if (this.hoverText != null && (this.hoveredItem == null || this.hoveredItem == null || this.ItemsToGrabMenu == null))
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
         if (this.heldItem != null)
            this.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
         if (this.hoveredItem != null) {
            IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
         } else if (this.hoveredItem != null && this.ItemsToGrabMenu != null) {
            IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
         }
         Game1.mouseCursorTransparency = 1f;
         deepBaseCalls.pDrawMouse(b);
      }

      public override void emergencyShutDown() {
         deepBaseCalls.pEmergencyShutDown();
         Console.WriteLine("ChestExMenu.emergencyShutDown");
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

         // get MenuWithInventory/IClickable calls
         {
            deepBaseCalls.pEmergencyShutDown = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tEmergencyShutdown>(this, "emergencyShutDown");
            deepBaseCalls.pPopulateClickableComponentList = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tPopulateClickableComponentList>(this, "populateClickableComponentList");
            deepBaseCalls.pApplyMovementKey = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tApplyMovementKey>(this, "applyMovementKey", new Type[] { typeof(Keys) });
            deepBaseCalls.pDrawMouse = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tDrawMouse>(this, "drawMouse", new Type[] { typeof(SpriteBatch) });
            deepBaseCalls.pExitThisMenu = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tExitThisMenu>(this, "exitThisMenu", new Type[] { typeof(Boolean) });
            deepBaseCalls.pReceiveGamePadButton = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tReceiveGamePadButton>(this, "receiveGamePadButton", new Type[] { typeof(Buttons) });
            deepBaseCalls.pUpdate = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tUpdate>(this, "update", new Type[] { typeof(GameTime) });
            deepBaseCalls.pMovePosition = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tMovePosition>(this, "movePosition", new Type[] { typeof(Int32), typeof(Int32) });
            deepBaseCalls.pPerformHoverAction = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tPerformHoverAction>(this, "performHoverAction", new Type[] { typeof(Int32), typeof(Int32) });
            deepBaseCalls.pDraw = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tDraw>(this, "draw", new Type[] { typeof(SpriteBatch), typeof(Boolean), typeof(Boolean) });
            deepBaseCalls.pReceiveLeftClick = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tReceiveLeftClick>(this, "receiveLeftClick", new Type[] { typeof(Int32), typeof(Int32), typeof(Boolean) });
            deepBaseCalls.pReceiveRightClick = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tReceiveRightClick>(this, "receiveRightClick", new Type[] { typeof(Int32), typeof(Int32), typeof(Boolean) });
            deepBaseCalls.pGetComponentWithID = DeepBaseCallsGetter.GetDeepBaseFunction<DeepBaseCalls.tGetComponentWithID>(this, "getComponentWithID", new Type[] { typeof(Int32) });
         }
         // calc bgXDiff
         {
            if (Config.instance.columns < 12) { // game's default column amount
               bgXDiff = (12 - Config.instance.columns) * 32;
            } else {
               bgXDiff = (Config.instance.columns - 12) * -32;
            }
         }

         this.ItemsToGrabMenu = new InventoryMenu(this.inventory.xPositionOnScreen + bgXDiff,
                                     this.yPositionOnScreen - Game1.tileSize / 4,
                                     false, inventory, null, Config.instance.getCapacity(), Config.instance.rows, 0, 0, true);
         this.ItemsToGrabMenu.populateClickableComponentList();
         // background placement
         this.yPositionOnScreen += bgYDiff;

         if (source == 1 && sourceItem != null && sourceItem is Chest) {
            setColorPicker();
         }
         this.context = context;
         if (snapToBottom) {
            deepBaseCalls.pMovePosition(0, Game1.viewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
            this.snappedtoBottom = true;
         }

         if (Game1.options.SnappyMenus) {
            this.ItemsToGrabMenu.populateClickableComponentList();
            for (Int32 i = 0; i < this.ItemsToGrabMenu.inventory.Count; i++) {
               if (this.ItemsToGrabMenu.inventory[i] != null) {
                  this.ItemsToGrabMenu.inventory[i].myID += 53910;
                  this.ItemsToGrabMenu.inventory[i].upNeighborID += 53910;
                  this.ItemsToGrabMenu.inventory[i].rightNeighborID += 53910;
                  this.ItemsToGrabMenu.inventory[i].downNeighborID = -7777;
                  this.ItemsToGrabMenu.inventory[i].leftNeighborID += 53910;
                  this.ItemsToGrabMenu.inventory[i].fullyImmutable = true;
               }
            }
         }
         this.behaviorFunction = behaviorOnItemSelectFunction;
         this.behaviorOnItemGrab = behaviorOnItemGrab;
         this.canExitOnKey = canBeExitedWithKey;
         if (showOrganizeButton) {
            setOrganizeButton();
         }
         if ((Game1.isAnyGamePadButtonBeingPressed() || !Game1.lastCursorMotionWasMouse) && this.ItemsToGrabMenu.actualInventory.Count > 0 && Game1.activeClickableMenu == null) {
            Game1.setMousePosition(this.inventory.inventory[0].bounds.Center);
         }
         if (Game1.options.snappyMenus && Game1.options.gamepadControls) {

            for (Int32 k = 0; k < 12; k++) {
               if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count >= 12) {
                  this.inventory.inventory[k].upNeighborID = (this.shippingBin ? 12598 : ((this.discreteColorPickerCC != null && this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count <= k) ? 4343 : ((this.ItemsToGrabMenu.inventory.Count > k) ? (53910 + k) : 53910)));
               }
               if (this.discreteColorPickerCC != null && this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count > k) {
                  this.ItemsToGrabMenu.inventory[k].upNeighborID = 4343;
               }
            }
            for (Int32 l = 0; l < Config.instance.getCapacity(); l++) {
               if (this.inventory != null && this.inventory.inventory != null && this.inventory.inventory.Count > l) {
                  this.inventory.inventory[l].upNeighborID = -7777;
                  this.inventory.inventory[l].upNeighborImmutable = true;
               }
            }
            if (this.okButton != null) {
               this.okButton.leftNeighborID = 11;
            }
            deepBaseCalls.pPopulateClickableComponentList();
            this.snapToDefaultClickableComponent();
         }
      }
   }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

//
//    Copyright (C) 2020 Berkay Yigit <berkaytgy@gmail.com>
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx.Types.BaseTypes {
   /// <summary>
   /// This class is probably created and handled by the game.
   /// Normally, this would derive from <see cref="ICustomMenu"/> but the <see cref="ItemGrabMenu"/> inheritance is crucial for compatibility with other community-made content. This is practically 1:1 with <see cref="ICustomMenu"/>.
   /// </summary>
   /// <remarks>However, this class requires a patch to the '<see cref="ItemGrabMenu.ItemGrabMenu(IList{StardewValley.Item}, Object)"/>' constructor so that <see cref="ItemGrabMenu"/> itself is never initialized.</remarks>
   public partial class ICustomItemGrabMenu : ItemGrabMenu {
      // Private:
      #region Private

      // Input events:
      #region Input events

      private (ICustomMenu.MouseStateEx left, ICustomMenu.MouseStateEx right, ICustomMenu.MouseStateEx middle) _lastMouseStates;

      private void _eventCursorMoved(Object sender, StardewModdingAPI.Events.CursorMovedEventArgs e) {
         if (this.onCursorMoved(e) == ICustomMenu.InformStatus.InformItems) {
            this.MenuItems.ForEach((i) =>
            {
               if (!i.IsVisible)
                  return;

               i.OnCursorMoved(e);
               i.Components.ForEach((c) => { if (c.IsVisible) c.OnCursorMoved(e); });
            });
         }
      }

      private void _eventButtonPressed(Object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e) {
         if (this.onButtonPressed(e) == ICustomMenu.InformStatus.InformItems) {
            this.MenuItems.ForEach((i) =>
            {
               i.OnButtonPressed(e);
               i.Components.ForEach((c) => c.OnButtonPressed(e));
            });
         }

         var last_mouse_state = e.Button switch
         {
            StardewModdingAPI.SButton.MouseLeft => _lastMouseStates.left,
            StardewModdingAPI.SButton.MouseRight => _lastMouseStates.right,
            StardewModdingAPI.SButton.MouseMiddle => _lastMouseStates.middle,
            _ => ICustomMenu.MouseStateEx.Default
         };

         if (!last_mouse_state.Equals(ICustomMenu.MouseStateEx.Default)) {
            last_mouse_state.Pos = e.Cursor.ScreenPixels;
            last_mouse_state.ButtonState = StardewModdingAPI.SButtonState.Pressed;

            // Call OnMouseClick for items
            this.MenuItems.ForEach((i) =>
            {
               if (!i.IsVisible)
                  return;

               if (i.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint())) {
                  if (!i.RaiseMouseClickEventOnRelease)
                     i.OnMouseClick(last_mouse_state);

                  // Call OnMouseClick for this item's components
                  i.Components.ForEach((c) =>
                  {
                     if (!c.IsVisible)
                        return;

                     if (c.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint())) {
                        if (!c.RaiseMouseClickEventOnRelease)
                           c.OnMouseClick(last_mouse_state);
                     }
                  });
               }
            });
         }
      }

      private void _eventButtonReleased(Object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e) {
         if (this.onButtonReleased(e) == ICustomMenu.InformStatus.InformItems) {
            this.MenuItems.ForEach((i) =>
            {
               i.OnButtonReleased(e);
               i.Components.ForEach((c) => c.OnButtonReleased(e));
            });
         }

         // check data for _eventMouseClick
         var last_mouse_state = e.Button switch
         {
            StardewModdingAPI.SButton.MouseLeft => _lastMouseStates.left,
            StardewModdingAPI.SButton.MouseRight => _lastMouseStates.right,
            StardewModdingAPI.SButton.MouseMiddle => _lastMouseStates.middle,
            _ => ICustomMenu.MouseStateEx.Default
         };

         if (!last_mouse_state.Equals(ICustomMenu.MouseStateEx.Default)) { // is a mouse activity
            if (last_mouse_state.ButtonState == StardewModdingAPI.SButtonState.Pressed) { // is a click
               // save OnPressed pos
               var mousePressedPoint = last_mouse_state.Pos.AsXNAPoint();
               // update mouse state pos
               last_mouse_state.Pos = e.Cursor.ScreenPixels;

               // Call OnMouseClick for items
               this.MenuItems.ForEach((i) =>
               {
                  if (!i.IsVisible)
                     return;

                  if (i.Bounds.Contains(mousePressedPoint) /* user pressed while hovering item */
                     && i.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint()) /* user released while hovering item */) {
                     if (i.RaiseMouseClickEventOnRelease)
                        i.OnMouseClick(last_mouse_state);

                     // Call OnMouseClick for this item's components
                     i.Components.ForEach((c) =>
                     {
                        if (!c.IsVisible)
                           return;

                        if (c.Bounds.Contains(mousePressedPoint)
                           && c.Bounds.Contains(e.Cursor.ScreenPixels.AsXNAPoint())) {
                           if (c.RaiseMouseClickEventOnRelease)
                              c.OnMouseClick(last_mouse_state);
                        }
                     });
                  }
               });
            }

            last_mouse_state.ButtonState = StardewModdingAPI.SButtonState.Released;
         }
      }

      #endregion

      #endregion

      // Protected:
      #region Protected

      protected Color colMenuBackground;

      // Virtuals:
      #region Virtuals

      protected virtual ICustomItemGrabMenu clone() {
         return new ICustomItemGrabMenu(
            this.ItemsToGrabMenu.actualInventory, false, true,
            new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.sv_behaviorFunction, null,
            this.behaviorOnItemGrab, false, true, true, true, true, this.source, this.sv_sourceItem,
            this.whichSpecialButton, this.context);
      }

      protected virtual void updateClonedMenu() {
         if (StardewValley.Game1.activeClickableMenu is ICustomItemGrabMenu newMenu) {
            newMenu.setSourceItem(this.sv_sourceItem);
            if (StardewValley.Game1.options.snappyMenus && StardewValley.Game1.options.gamepadControls) {
               newMenu.currentlySnappedComponent = this.currentlySnappedComponent;
               newMenu.snapCursorToCurrentSnappedComponent();
            }
         }
      }

      #endregion

      // Input event handlers:
      #region Input event handlers

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual ICustomMenu.InformStatus onMouseClick(ICustomMenu.MouseStateEx mouseState) {
         return this.IsVisible ? ICustomMenu.InformStatus.InformItems : ICustomMenu.InformStatus.DontInformItems;
      }

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual ICustomMenu.InformStatus onCursorMoved(StardewModdingAPI.Events.CursorMovedEventArgs e) {
         return this.IsVisible ? ICustomMenu.InformStatus.InformItems : ICustomMenu.InformStatus.DontInformItems;
      }

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual ICustomMenu.InformStatus onButtonPressed(StardewModdingAPI.Events.ButtonPressedEventArgs e) {
         return this.IsVisible ? ICustomMenu.InformStatus.InformItems : ICustomMenu.InformStatus.DontInformItems;
      }

      /// <summary>
      /// Base implementation informs this menu's items if '<see cref="IsVisible"/>' is true.
      /// </summary>
      /// <returns>Whether this menu's items are informed of this event.</returns>
      protected virtual ICustomMenu.InformStatus onButtonReleased(StardewModdingAPI.Events.ButtonReleasedEventArgs e) {
         return this.IsVisible ? ICustomMenu.InformStatus.InformItems : ICustomMenu.InformStatus.DontInformItems;
      }

      #endregion

      // ItemGrabMenu related events
      #region ItemGrabMenu related events

      /// <summary>
      /// Base implementation creates <see cref="ItemGrabMenu.region_organizationButtons"/> using game's own code.
      /// </summary>
      /// <param name="createChestColorPicker">Whether to create the chest colour picker.</param>
      /// <param name="createOrganizeButton">Whether to create the organize button.</param>
      /// <param name="createFillStacksButton">Whether to create the fill stacks button.</param>
      protected virtual void createOrganizationButtons(Boolean createChestColorPicker, Boolean createOrganizeButton, Boolean createFillStacksButton) {
         var source_menu_bounds = this.SourceInventoryOptions.Bounds;
         var player_menu_bounds = this.PlayerInventoryOptions.Bounds;

         if (createChestColorPicker) {
            this.chestColorPicker = new DiscreteColorPicker(
               source_menu_bounds.X + 16, source_menu_bounds.Y, 0, new Chest(true));
            this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((this.sv_sourceItem as Chest).playerChoiceColor);
            (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);

            this.colorPickerToggleButton = new ClickableTextureComponent("",
               new Rectangle(source_menu_bounds.X + source_menu_bounds.Width, source_menu_bounds.Y + 92, 64, 64),
               "", StardewValley.Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
               StardewValley.Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f, false)
            {
               myID = ItemGrabMenu.region_colorPickToggle,
               downNeighborID = ItemGrabMenu.region_fillStacksButton,
               leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
               region = ItemGrabMenu.region_organizationButtons
            };

            this.discreteColorPickerCC = new List<ClickableComponent>();
            for (Int32 j = 0; j < this.chestColorPicker.totalColors; j++) {
               this.discreteColorPickerCC.Add(new ClickableComponent(
                  new Rectangle(this.chestColorPicker.xPositionOnScreen + (IClickableMenu.borderWidth / 2) + (j * 9 * 4), this.chestColorPicker.yPositionOnScreen + (IClickableMenu.borderWidth / 2), 36, 28), "")
               {
                  myID = j + 4343,
                  rightNeighborID = (j < this.chestColorPicker.totalColors - 1) ? (j + 4343 + 1) : -1,
                  leftNeighborID = (j > 0) ? (j + 4343 - 1) : -1,
                  downNeighborID = (this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count > 0) ? 53910 : 0
               });
            }
         } else {
            this.chestColorPicker?.exitThisMenu(false);
            this.discreteColorPickerCC?.Clear();

            this.chestColorPicker = null;
            this.colorPickerToggleButton = null;
            this.discreteColorPickerCC = null;
         }

         this.fillStacksButton = createFillStacksButton
            ? new ClickableTextureComponent("",
               new Rectangle(source_menu_bounds.X + source_menu_bounds.Width, source_menu_bounds.Y + 168, 64, 64),
               "", StardewValley.Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"),
               StardewValley.Game1.mouseCursors, new Rectangle(103, 469, 16, 16), 4f, false)
            {
               myID = ItemGrabMenu.region_fillStacksButton,
               upNeighborID = (this.colorPickerToggleButton != null) ? this.colorPickerToggleButton.myID : -500,
               downNeighborID = ItemGrabMenu.region_organizeButton,
               leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
               region = ItemGrabMenu.region_organizationButtons
            }
            : null;

         this.organizeButton = createOrganizeButton
            ? new ClickableTextureComponent("",
               new Rectangle(source_menu_bounds.X + source_menu_bounds.Width, source_menu_bounds.Y + 244, 64, 64),
               "", StardewValley.Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"),
               StardewValley.Game1.mouseCursors, new Rectangle(162, 440, 16, 16), 4f, false)
            {
               myID = ItemGrabMenu.region_organizeButton,
               upNeighborID = ItemGrabMenu.region_fillStacksButton,
               downNeighborID = MenuWithInventory.region_trashCan,
               leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
               region = ItemGrabMenu.region_organizationButtons
            }
            : null;

         this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right).ForEach((cc) =>
         {
            cc.rightNeighborID = createChestColorPicker ? this.colorPickerToggleButton.myID
               : createOrganizeButton ? this.organizeButton.myID
               : createFillStacksButton ? this.fillStacksButton.myID
               : ClickableComponent.SNAP_TO_DEFAULT;
         });
      }

      #endregion

      #endregion

      // Public:
      #region Public

      /// <summary>
      /// The underlying <see cref="ItemGrabMenu"/>'s bound values wrapped in a <see cref="Rectangle"/>.
      /// </summary>
      public Rectangle Bounds {
         get => new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
         private set {
            this.xPositionOnScreen = value.X;
            this.yPositionOnScreen = value.Y;
            this.width = value.Width;
            this.height = value.Height;
         }
      }

      public Boolean IsVisible { get; protected set; }

      public List<ICustomMenuItem> MenuItems { get; set; }

      public T GetSourceAs<T>() where T : StardewValley.Item {
         return this.sv_sourceItem is T ? this.sv_sourceItem as T : null;
      }

      // Virtuals:
      #region Virtuals

      /// <summary>
      /// Base implementation sets '<see cref="IsVisible"/>' to '<paramref name="isVisible"/>'.
      /// </summary>
      /// <param name="isVisible">Whether this menu should be visible.</param>
      public virtual void SetVisible(Boolean isVisible) {
         this.IsVisible = isVisible;
      }

      /// <summary>
      /// <para>Base implementation:</para>
      /// <para>1. Unregisters '<see cref="IClickableMenu.exitFunction"/>'.</para>
      /// <para>2. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
      /// <para>3. Disposes of this menu's items.</para>
      /// <para>4. Unregisters SMAPI input events.</para>
      /// </summary>
      public virtual void OnClose() {
         // unregister menu exit function
         this.exitFunction = null;

         // hide
         this.SetVisible(false);

         // dispose items
         this.MenuItems.ForEach((i) => i.Dispose());
         this.MenuItems.Clear();

         // unregister SMAPI events
         GlobalVars.SMAPIHelper.Events.Input.CursorMoved -= _eventCursorMoved;
         GlobalVars.SMAPIHelper.Events.Input.ButtonPressed -= _eventButtonPressed;
         GlobalVars.SMAPIHelper.Events.Input.ButtonReleased -= _eventButtonReleased;
      }

      #endregion

      // Overrides:
      #region Overrides

      /// <summary>
      /// Base implementation calls '<see cref="ItemGrabMenu.draw(SpriteBatch)"/>' if '<see cref="IsVisible"/>' is true and likewise with this menu's items.
      /// </summary>
      public override void draw(SpriteBatch b) {
         if (!this.IsVisible)
            return;

         // TODO: Add custom colour
         if (this.drawBG)
            b.Draw(StardewValley.Game1.fadeToBlackRect, GlobalVars.GameViewport, Color.Black * 0.65f);

         if (this.SourceInventoryOptions.IsVisible) {
            var wrap_rectangle = this.SourceInventoryOptions.Bounds;
            StardewValley.Game1.drawDialogueBox(
               wrap_rectangle.X, wrap_rectangle.Y, wrap_rectangle.Width, wrap_rectangle.Height, false, true,
               r: this.SourceInventoryOptions.BackgroundColour.R,
               g: this.SourceInventoryOptions.BackgroundColour.G,
               b: this.SourceInventoryOptions.BackgroundColour.B);
            this.ItemsToGrabMenu.draw(b, this.SourceInventoryOptions.BackgroundColour);
         }

         if (this.PlayerInventoryOptions.IsVisible) {
            var wrap_rectangle = this.PlayerInventoryOptions.Bounds;
            StardewValley.Game1.drawDialogueBox(
               wrap_rectangle.X, wrap_rectangle.Y, wrap_rectangle.Width, wrap_rectangle.Height, false, true,
               r: this.PlayerInventoryOptions.BackgroundColour.R,
               g: this.PlayerInventoryOptions.BackgroundColour.G,
               b: this.PlayerInventoryOptions.BackgroundColour.B);
            this.inventory.draw(b, this.PlayerInventoryOptions.BackgroundColour);
         }

         // Game code, slightly modified
         // TODO: add customization here (mouse transparecny etc.)

         this.sv_poof?.draw(b, true, 0, 0, 1f);
         this._transferredItemSprites?.ForEach((s) => s.Draw(b));

         // draw organization buttons
         this.colorPickerToggleButton?.draw(b);
         this.chestColorPicker?.draw(b);
         this.organizeButton?.draw(b);
         this.fillStacksButton?.draw(b);
         this.okButton?.draw(b);
         if (!(this.trashCan is null)) {
            this.trashCan.draw(b);
            b.Draw(StardewValley.Game1.mouseCursors,
                   new Vector2(this.trashCan.bounds.X + 60.0f, this.trashCan.bounds.Y + 40.0f),
                   new Rectangle(564 + (StardewValley.Game1.player.trashCanLevel * 18), 129, 18, 10),
                   Color.White, this.trashCanLidRotation, new Vector2(16.0f, 10.0f), 4f, SpriteEffects.None, 0.86f);
         }

         this.MenuItems.ForEach((i) =>
         {
            if (i.IsVisible)
               i.Draw(b);
         });

         // draw hover tooltip/text
         if (this.hoveredItem is StardewValley.Item) {
            IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, !(this.heldItem is null), -1, 0, -1, -1, null, -1);
         } else if (!String.IsNullOrEmpty(this.hoverText)) {
            if (this.hoverAmount > 0)
               IClickableMenu.drawToolTip(b, this.hoverText, "", null, true, -1, 0, -1, -1, null, this.hoverAmount);
            else
               IClickableMenu.drawHoverText(b, this.hoverText, StardewValley.Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
         }

         // draw held item
         this.heldItem?.drawInMenu(b, StardewValley.Game1.getMousePosition().AsXNAVector2() + new Vector2(8.0f), 1f);

         // draw mouse
         StardewValley.Game1.mouseCursorTransparency = 1.0f;
         base.drawMouse(b);
      }

      /// <summary>
      /// Base implementation calls '<see cref="IClickableMenu.draw(SpriteBatch, Int32, Int32, Int32)"/>' if '<see cref="IsVisible"/>' is true.
      /// </summary>
      public override void draw(SpriteBatch b, Int32 red = -1, Int32 green = -1, Int32 blue = -1) {
         if (this.IsVisible)
            base.draw(b, red, green, blue);
      }

      /// <summary>
      /// Base implementation calls '<see cref="MenuWithInventory.draw(SpriteBatch, Boolean, Boolean, Int32, Int32, Int32)"/>' if '<see cref="IsVisible"/>' is true.
      /// </summary>
      public override void draw(SpriteBatch b, Boolean drawUpperPortion = true, Boolean drawDescriptionArea = true, Int32 red = -1, Int32 green = -1, Int32 blue = -1) {
         if (this.IsVisible)
            base.draw(b, drawUpperPortion, drawDescriptionArea, red, green, blue);
      }

      /// <summary>
      /// <para>Base implementation:</para>
      /// <para>1. Calls '<see cref="OnClose"/>'.</para>
      /// <para>2. Calls '<see cref="ItemGrabMenu.emergencyShutDown"/>'.</para>
      /// </summary>
      public override void emergencyShutDown() {
         Console.WriteLine("ICustomItemGrabMenu.emergencyShutdown");
         this.OnClose();
         base.emergencyShutDown();
      }

      /// <summary>
      /// <para>Base implementation:</para>
      /// <para>1. Informs '<see cref="ItemGrabMenu.ItemsToGrabMenu"/>'.</para>
      /// <para>2. Informs this menu's items.</para>
      /// <para>3. Calls <see cref="createOrganizationButtons(Boolean, Boolean, Boolean)"/>.</para>
      /// </summary>
      /// <remarks>Does *NOT* call '<see cref="ItemGrabMenu.gameWindowSizeChanged(Rectangle, Rectangle)"/>'.</remarks>
      public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
         // TODO: may need .movePosition for sub inventories??
         // inform ItemsToGrabMenu
         this.ItemsToGrabMenu?.gameWindowSizeChanged(oldBounds, newBounds);
         // inform inventory
         this.inventory?.gameWindowSizeChanged(oldBounds, newBounds);
         // inform items
         this.MenuItems.ForEach((i) => i.OnGameWindowSizeChanged(oldBounds, newBounds));
         // re-create organization buttons
         this.createOrganizationButtons(this.source == ItemGrabMenu.source_chest && this.sv_sourceItem is Chest, true, true);
      }

      /// <summary>
      /// <para>Base implementation:</para>
      /// <para>1. Handles '<see cref="MenuWithInventory.receiveLeftClick(Int32, Int32, Boolean)"/>'.</para>
      /// <para>2. Handles item storing/retrieving.</para>
      /// <para>3. Handles the default chest colour picker (if it's <see cref="DiscreteColorPicker"/> and is visible).</para>
      /// <para>4. Handles the default organize button (if it's visible).</para>
      /// <para>5. Handles the default fill stacks button (if it's visible).</para>
      /// </summary>
      /// <remarks>Does *NOT* call '<see cref="ItemGrabMenu.receiveLeftClick(Int32, Int32, Boolean)"/>'.</remarks>
      public override void receiveLeftClick(Int32 x, Int32 y, Boolean playSound = true) {
         // Handle MenuWithInventory
         {
            this.heldItem = this.inventory.leftClick(x, y, this.heldItem, playSound);

            if (!this.isWithinBounds(x, y) && this.readyToClose() && !(this.trashCan is null) && this.trashCan.containsPoint(x, y))
               StardewValley.Game1.SetFreeCursorDrag();

            if (!(this.okButton is null) && this.okButton.containsPoint(x, y) && this.readyToClose()) {
               this.exitThisMenu(true);
               StardewValley.Game1.playSound("bigDeSelect");
            }

            if (!(this.trashCan is null) && this.trashCan.containsPoint(x, y) && !(this.heldItem is null) && this.heldItem.canBeTrashed()) {
               StardewValley.Utility.trashItem(this.heldItem);
               this.heldItem = null;
            }
         }

         // Handle item storing/retrieving
         if (this.heldItem is null && this.showReceivingMenu) {
            this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);

            if (this.heldItem is StardewValley.Item) {
               this.behaviorOnItemGrab?.Invoke(this.heldItem, StardewValley.Game1.player);

               this.updateClonedMenu();
            }

            if (StardewValley.Game1.player.addItemToInventoryBool(this.heldItem, false)) {
               this.heldItem = null;
               StardewValley.Game1.playSound("coin");

               this.updateClonedMenu();
            }
         } else if ((this.reverseGrab || !(this.sv_behaviorFunction is null)) && this.isWithinBounds(x, y)) {
            if (this.heldItem is StardewValley.Item) {
               this.sv_behaviorFunction?.Invoke(this.heldItem, StardewValley.Game1.player);

               this.updateClonedMenu();
            }

            if (this.destroyItemOnClick)
               this.heldItem = null;
         }

         // Handle default chest colour picker
         if (this.chestColorPicker is DiscreteColorPicker) {
            this.chestColorPicker.receiveLeftClick(x, y, true);
            if (this.sv_sourceItem is Chest chest && this.chestColorPicker.visible && this.chestColorPicker.isWithinBounds(x, y))
               chest.playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);

            if (!(this.colorPickerToggleButton is null) && this.colorPickerToggleButton.containsPoint(x, y)) {
               StardewValley.Game1.player.showChestColorPicker.Flip();
               this.chestColorPicker.visible = StardewValley.Game1.player.showChestColorPicker;
               StardewValley.Game1.playSound("drumkit6");

               this.SetupBorderNeighbors();
            }
         }

         // Handle default organize button
         if (!(this.organizeButton is null) && this.organizeButton.visible && this.organizeButton.containsPoint(x, y)) {
            var last_snapped_component = this.currentlySnappedComponent;
            ItemGrabMenu.organizeItemsInList(this.ItemsToGrabMenu.actualInventory);

            StardewValley.Game1.activeClickableMenu = this.clone().setEssential(this.sv_essential);

            if (!(last_snapped_component is null)) {
               StardewValley.Game1.activeClickableMenu.setCurrentlySnappedComponentTo(last_snapped_component.myID);
               if (StardewValley.Game1.options.SnappyMenus)
                  StardewValley.Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }

            StardewValley.Game1.playSound("Ship");

            base.exitThisMenu(false);
            return;
         }

         // Handle default fill stacks button
         if (!(this.fillStacksButton is null) && this.fillStacksButton.visible && this.fillStacksButton.containsPoint(x, y)) {
            this.FillOutStacks();
            StardewValley.Game1.playSound("Ship");
         }

         // Drop item if dragging it out of bounds
         if (!(this.heldItem is null) && !this.isWithinBounds(x, y) && this.heldItem.canBeTrashed()) {
            this.DropHeldItem();
         }
      }

      /// <summary>
      /// <para>Base implemntatio:</para>
      /// <para>1. Handles '<see cref="MenuWithInventory.receiveRightClick(Int32, Int32, Boolean)"/>'.</para>
      /// <para>2. Handles item storing/retrieving.</para>
      /// </summary>
      /// <remarks>Does *NOT* call '<see cref="ItemGrabMenu.receiveRightClick(Int32, Int32, Boolean)"/>'.</remarks>
      public override void receiveRightClick(Int32 x, Int32 y, Boolean playSound = true) {
         // Handle MenuWithInventory
         {
            if (!this.allowRightClick) {
               this.heldItem = this.inventory.rightClick(x, y, this.heldItem, true, true);
               return;
            }

            this.heldItem = this.inventory.rightClick(x, y, this.heldItem, playSound, false);
         }

         // Handle item storing/retrieving
         if (this.heldItem is null && this.showReceivingMenu) {
            this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);

            if (this.heldItem is StardewValley.Item) {
               this.behaviorOnItemGrab?.Invoke(this.heldItem, StardewValley.Game1.player);

               this.updateClonedMenu();
            }

            if (StardewValley.Game1.player.addItemToInventoryBool(this.heldItem, false)) {
               this.heldItem = null;
               StardewValley.Game1.playSound("coin");

               this.updateClonedMenu();
            }
         } else if ((this.reverseGrab || !(this.sv_behaviorFunction is null)) && this.isWithinBounds(x, y)) {
            if (this.heldItem is StardewValley.Item) {
               this.sv_behaviorFunction?.Invoke(this.heldItem, StardewValley.Game1.player);

               this.updateClonedMenu();
            }

            if (this.destroyItemOnClick)
               this.heldItem = null;
         }
      }

      #endregion

      // ItemGrabMenu submenu options:
      #region ItemGrabMenu submenu options

      public struct InventoryMenuOptions {
         private readonly Action<Boolean> _fnSetVisibleExtra;

         public Color BackgroundColour { get; set; }

         public Rectangle Bounds { get; set; }

         public Boolean IsVisible { get; private set; }

         public Rectangle SafeContentRegion {
            get {
               var bounds = this.Bounds;
               return new Rectangle(
                  bounds.X + 36,
                  bounds.Y + IClickableMenu.spaceToClearTopBorder + 4,
                  bounds.Width - IClickableMenu.borderWidth - 32,
                  bounds.Height - IClickableMenu.spaceToClearTopBorder - 40
               );
            }
         }

         public void SetVisible(Boolean isVisible) {
            this.IsVisible = isVisible;
            _fnSetVisibleExtra?.Invoke(isVisible);
         }

         public InventoryMenuOptions(Rectangle bounds, Color backgroundColour, Boolean isVisible = true, Action<Boolean> setVisibleExtraFunctionality = null) {
            _fnSetVisibleExtra = setVisibleExtraFunctionality;

            this.BackgroundColour = backgroundColour;
            this.Bounds = bounds;
            this.IsVisible = !isVisible;

            this.SetVisible(isVisible);
         }
      }

      public InventoryMenuOptions PlayerInventoryOptions;
      public InventoryMenuOptions SourceInventoryOptions;

      #endregion

      // Shadowed ItemGrabMenu functions (to redirect to ICustomItemGrabMenu):
      #region Shadowed ItemGrabMenu functions (to redirect to ICustomItemGrabMenu)
      public new ICustomItemGrabMenu setEssential(Boolean essential) {
         this.sv_essential = essential;
         return this;
      }

      public new void setSourceItem(StardewValley.Item item) {
         this.sv_sourceItem = item;
         this.createOrganizationButtons(!(this.chestColorPicker is null), !(this.organizeButton is null), !(this.fillStacksButton is null));
      }

      #endregion

      // Inaccessible 'ItemGrabMenu + bases' fields exposed through reflection properties:
      #region Inaccessible 'ItemGrabMenu + bases' fields exposed through reflection properties

      public ItemGrabMenu.behaviorOnItemSelect sv_behaviorFunction {
         get => Harmony.Traverse.Create(this).Field<ItemGrabMenu.behaviorOnItemSelect>("behaviorFunction").Value;
         set => Harmony.Traverse.Create(this).Field<ItemGrabMenu.behaviorOnItemSelect>("behaviorFunction").Value = value;
      }
      public Boolean sv_essential {
         get => Harmony.Traverse.Create(this).Field<Boolean>("essential").Value;
         set => Harmony.Traverse.Create(this).Field<Boolean>("essential").Value = value;
      }
      public StardewValley.Item sv_hoverItem {
         get => Harmony.Traverse.Create(this).Field<StardewValley.Item>("hoverItem").Value;
         set => Harmony.Traverse.Create(this).Field<StardewValley.Item>("hoverItem").Value = value;
      }
      public String sv_message {
         get => Harmony.Traverse.Create(this).Field<String>("message").Value;
         set => Harmony.Traverse.Create(this).Field<String>("message").Value = value;
      }
      public StardewValley.TemporaryAnimatedSprite sv_poof {
         get => Harmony.Traverse.Create(this).Field<StardewValley.TemporaryAnimatedSprite>("poof").Value;
         set => Harmony.Traverse.Create(this).Field<StardewValley.TemporaryAnimatedSprite>("poof").Value = value;
      }
      public Boolean sv_snappedtoBottom {
         get => Harmony.Traverse.Create(this).Field<Boolean>("snappedtoBottom").Value;
         set => Harmony.Traverse.Create(this).Field<Boolean>("snappedtoBottom").Value = value;
      }
      public StardewValley.Item sv_sourceItem {
         get => Harmony.Traverse.Create(this).Field<StardewValley.Item>("sourceItem").Value;
         set => Harmony.Traverse.Create(this).Field<StardewValley.Item>("sourceItem").Value = value;
      }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      /// <summary>
      /// Called by patched code in the game. Parameters are 1:1 with the original constructor.
      /// </summary>
      /// <remarks>All '<see cref="ItemGrabMenu"/>' constructors should be patched to skip running their code in order for this menu to function correctly.</remarks>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "This ctor is arbitrated from game code.")]
      public ICustomItemGrabMenu(IList<StardewValley.Item> inventory,
                                 Boolean reverseGrab,
                                 Boolean showReceivingMenu,
                                 InventoryMenu.highlightThisItem highlightFunction,
                                 ItemGrabMenu.behaviorOnItemSelect behaviorOnItemSelectFunction,
                                 String message,
                                 ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab = null,
                                 Boolean snapToBottom = false,
                                 Boolean canBeExitedWithKey = false,
                                 Boolean playRightClickSound = true,
                                 Boolean allowRightClick = true,
                                 Boolean showOrganizeButton = false,
                                 Int32 source = 0,
                                 StardewValley.Item sourceItem = null,
                                 Int32 whichSpecialButton = -1,
                                 Object context = null)
      : base(null, new MenuWithInventoryCtorParams(highlightFunction, true, true, 0, 0, 64)) {
         _lastMouseStates = (
            left: new ICustomMenu.MouseStateEx(StardewModdingAPI.SButton.MouseLeft),
            right: new ICustomMenu.MouseStateEx(StardewModdingAPI.SButton.MouseRight),
            middle: new ICustomMenu.MouseStateEx(StardewModdingAPI.SButton.MouseMiddle));

         //this.colMenuBackground = custom menu colour;
         this.MenuItems = new List<ICustomMenuItem>();
         this.SetVisible(true);

         // register menu exit function
         this.exitFunction = this.OnClose;

         // register SMAPI events
         GlobalVars.SMAPIHelper.Events.Input.CursorMoved += _eventCursorMoved;
         GlobalVars.SMAPIHelper.Events.Input.ButtonPressed += _eventButtonPressed;
         GlobalVars.SMAPIHelper.Events.Input.ButtonReleased += _eventButtonReleased;

         // construct ItemGrabMenu, mostly game code albeit slightly modified
         {
            this.source = source;
            this.sv_message = message;
            this.reverseGrab = reverseGrab;
            this.showReceivingMenu = showReceivingMenu;
            this.playRightClickSound = playRightClickSound;
            this.allowRightClick = allowRightClick;
            this.inventory.showGrayedOutSlots = true;
            this.sv_sourceItem = sourceItem;
            this.whichSpecialButton = whichSpecialButton;
            this.context = context;
            this.sv_behaviorFunction = behaviorOnItemSelectFunction;
            this.behaviorOnItemGrab = behaviorOnItemGrab;
            this.canExitOnKey = canBeExitedWithKey;

            // create chest menu
            {
               this.ItemsToGrabMenu = new InventoryMenu(
                     this.xPositionOnScreen + 32,
                     this.yPositionOnScreen, false, inventory, highlightFunction, -1, 3, 0, 0, true);
               this.ItemsToGrabMenu.populateClickableComponentList();
               for (Int32 i = 0; i < this.ItemsToGrabMenu.inventory.Count; i++) {
                  if (this.ItemsToGrabMenu.inventory[i] != null) {
                     this.ItemsToGrabMenu.inventory[i].myID += ItemGrabMenu.region_itemsToGrabMenuModifier;
                     this.ItemsToGrabMenu.inventory[i].upNeighborID += ItemGrabMenu.region_itemsToGrabMenuModifier;
                     this.ItemsToGrabMenu.inventory[i].rightNeighborID += ItemGrabMenu.region_itemsToGrabMenuModifier;
                     this.ItemsToGrabMenu.inventory[i].downNeighborID = ClickableComponent.CUSTOM_SNAP_BEHAVIOR;
                     this.ItemsToGrabMenu.inventory[i].leftNeighborID += ItemGrabMenu.region_itemsToGrabMenuModifier;
                     this.ItemsToGrabMenu.inventory[i].fullyImmutable = true;
                  }
               }
            }

            // create organization items
            {
               if (showOrganizeButton)
                  this.createOrganizationButtons(this.source == ItemGrabMenu.source_chest && this.sv_sourceItem is Chest, true, true);
            }

            // give trash can neighbour ids
            {
               if (!(this.trashCan is null))
                  this.trashCan.leftNeighborID = 11;
            }

            // give ok button neighbour ids
            {
               if (!(this.okButton is null)) {
                  this.okButton.leftNeighborID = 11;

                  for (Int32 i = 0; i < this.inventory.inventory.Count; i += 11) {
                     if (this.inventory.inventory[i] is ClickableComponent)
                        this.inventory.inventory[i].rightNeighborID = this.okButton.myID;
                  }
               }
            }

            this.populateClickableComponentList();
            if (StardewValley.Game1.options.SnappyMenus)
               this.snapToDefaultClickableComponent();
            this.SetupBorderNeighbors();
         }

         // construct options
         {
            this.PlayerInventoryOptions = new InventoryMenuOptions(
               this.inventory.GetRectangleForDialogueBox(), Color.White, true,
               new Action<Boolean>((isVisible) => this.inventory.inventory.ForEach((c) => c.visible = isVisible)));

            this.SourceInventoryOptions = new InventoryMenuOptions(
               this.ItemsToGrabMenu.GetRectangleForDialogueBox(), Color.White, true,
               new Action<Boolean>((isVisible) => this.ItemsToGrabMenu.inventory.ForEach((c) => c.visible = isVisible)));
         }
      }

      #endregion
   }
}

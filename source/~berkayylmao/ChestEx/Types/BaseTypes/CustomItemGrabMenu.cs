/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// clang-format off
// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
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
// clang-format on

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ChestEx.LanguageExtensions;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Harmony;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using Object = System.Object;

namespace ChestEx.Types.BaseTypes {
  /// <summary>
  /// This class is probably created and handled by the game.
  /// Normally, this would derive from <see cref="CustomMenu"/> but the <see cref="ItemGrabMenu"/> inheritance is crucial for compatibility with other community-made content. This is practically 1:1 with <see cref="CustomMenu"/>.
  /// </summary>
  /// <remarks>However, this class requires a patch to the '<see cref="ItemGrabMenu(System.Collections.Generic.IList{StardewValley.Item}, object)"/>' constructor so that <see cref="ItemGrabMenu"/> itself is never initialized.</remarks>
  public partial class CustomItemGrabMenu : ItemGrabMenu {
    // Private:
  #region Private

    // Input events:
  #region Input events

    private readonly struct MouseStateData {
      public readonly CustomMenu.MouseStateEx mLeft;
      public readonly CustomMenu.MouseStateEx mRight;
      public readonly CustomMenu.MouseStateEx mMiddle;

      public MouseStateData(CustomMenu.MouseStateEx left, CustomMenu.MouseStateEx right, CustomMenu.MouseStateEx middle) {
        this.mLeft   = left;
        this.mRight  = right;
        this.mMiddle = middle;
      }
    }

    private MouseStateData lastMouseStates;

    private void _eventCursorMoved(Object sender, CursorMovedEventArgs e) {
      try {
        if (this.OnCursorMoved(e) == CustomMenu.InformStatus.InformItems) {
          this.mMenuItems.ForEach(i => {
            if (!i.mIsVisible) return;

            Vector2 correct_pos = Utility.ModifyCoordinatesForUIScale(e.NewPosition.ScreenPixels);
            i.OnCursorMoved(correct_pos);
            i.mComponents.ForEach(c => {
              if (c.mIsVisible) c.OnCursorMoved(correct_pos);
            });
          });
        }
      }
      catch (InvalidOperationException) { }
    }

    private void _eventButtonPressed(Object sender, ButtonPressedEventArgs e) {
      try {
        if (this.OnButtonPressed(e) == CustomMenu.InformStatus.InformItems) {
          this.mMenuItems.ForEach(i => {
            i.OnButtonPressed(e);
            i.mComponents.ForEach(c => c.OnButtonPressed(e));
          });
        }

        CustomMenu.MouseStateEx last_mouse_state = e.Button switch {
          SButton.MouseLeft   => this.lastMouseStates.mLeft,
          SButton.MouseRight  => this.lastMouseStates.mRight,
          SButton.MouseMiddle => this.lastMouseStates.mMiddle,
          _                   => CustomMenu.MouseStateEx.gDefault
        };

        if (last_mouse_state.Equals(CustomMenu.MouseStateEx.gDefault)) return;

        Vector2 correct_pos    = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
        Point   xna_cursor_pos = correct_pos.AsXNAPoint();

        last_mouse_state.mPos         = correct_pos;
        last_mouse_state.mButtonState = SButtonState.Pressed;

        // Call OnMouseClick for items
        this.mMenuItems.ForEach(i => {
          if (!i.mIsVisible || !i.mBounds.Contains(xna_cursor_pos)) return;

          if (!i.mRaiseMouseClickEventOnRelease) i.OnMouseClick(last_mouse_state);
          i.mComponents.ForEach(c => {
            if (!c.mIsVisible || !c.mBounds.Contains(xna_cursor_pos)) return;

            if (!c.mRaiseMouseClickEventOnRelease) c.OnMouseClick(last_mouse_state);
          });
        });
      }
      catch (InvalidOperationException) { }
    }

    private void _eventButtonReleased(Object sender, ButtonReleasedEventArgs e) {
      try {
        if (this.OnButtonReleased(e) == CustomMenu.InformStatus.InformItems) {
          this.mMenuItems.ForEach(i => {
            i.OnButtonReleased(e);
            i.mComponents.ForEach(c => c.OnButtonReleased(e));
          });
        }

        // check data for _eventMouseClick
        CustomMenu.MouseStateEx last_mouse_state = e.Button switch {
          SButton.MouseLeft   => this.lastMouseStates.mLeft,
          SButton.MouseRight  => this.lastMouseStates.mRight,
          SButton.MouseMiddle => this.lastMouseStates.mMiddle,
          _                   => CustomMenu.MouseStateEx.gDefault
        };

        if (last_mouse_state.Equals(CustomMenu.MouseStateEx.gDefault)) return;

        if (last_mouse_state.mButtonState == SButtonState.Pressed) {
          Vector2 correct_pos       = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
          Point   xna_cursor_pos    = correct_pos.AsXNAPoint();
          Point   mouse_pressed_pos = last_mouse_state.mPos.AsXNAPoint();

          last_mouse_state.mPos = correct_pos;

          // Call OnMouseClick for items
          this.mMenuItems.ForEach(i => {
            if (!i.mIsVisible || !i.mBounds.Contains(mouse_pressed_pos) || !i.mBounds.Contains(xna_cursor_pos)) return;

            if (i.mRaiseMouseClickEventOnRelease) i.OnMouseClick(last_mouse_state);
            i.mComponents.ForEach(c => {
              if (!c.mIsVisible || !c.mBounds.Contains(mouse_pressed_pos) || !c.mBounds.Contains(xna_cursor_pos)) return;

              if (c.mRaiseMouseClickEventOnRelease) c.OnMouseClick(last_mouse_state);
            });
          });
        }

        last_mouse_state.mButtonState = SButtonState.Released;
      }
      catch (InvalidOperationException) { }
    }

  #endregion

  #endregion

    // Protected:
  #region Protected

    // Input event handlers:
  #region Input event handlers

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual CustomMenu.InformStatus OnMouseClick(CustomMenu.MouseStateEx mouseState) {
      return this.mIsVisible ? CustomMenu.InformStatus.InformItems : CustomMenu.InformStatus.DontInformItems;
    }

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual CustomMenu.InformStatus OnCursorMoved(CursorMovedEventArgs e) {
      return this.mIsVisible ? CustomMenu.InformStatus.InformItems : CustomMenu.InformStatus.DontInformItems;
    }

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual CustomMenu.InformStatus OnButtonPressed(ButtonPressedEventArgs e) {
      return this.mIsVisible ? CustomMenu.InformStatus.InformItems : CustomMenu.InformStatus.DontInformItems;
    }

    /// <summary>
    /// Base implementation informs this menu's items if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    /// <returns>Whether this menu's items are informed of this event.</returns>
    protected virtual CustomMenu.InformStatus OnButtonReleased(ButtonReleasedEventArgs e) {
      return this.mIsVisible ? CustomMenu.InformStatus.InformItems : CustomMenu.InformStatus.DontInformItems;
    }

    protected virtual void RegisterInputEvents() {
      // init mouse states
      this.lastMouseStates = new MouseStateData(new CustomMenu.MouseStateEx(SButton.MouseLeft),
                                                new CustomMenu.MouseStateEx(SButton.MouseRight),
                                                new CustomMenu.MouseStateEx(SButton.MouseMiddle));

      GlobalVars.gSMAPIHelper.Events.Input.CursorMoved    += this._eventCursorMoved;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonPressed  += this._eventButtonPressed;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonReleased += this._eventButtonReleased;
    }

    protected virtual void UnregisterInputEvents() {
      GlobalVars.gSMAPIHelper.Events.Input.CursorMoved    -= this._eventCursorMoved;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonPressed  -= this._eventButtonPressed;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonReleased -= this._eventButtonReleased;
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
    protected virtual void CreateOrganizationButtons(Boolean createChestColorPicker, Boolean createOrganizeButton, Boolean createFillStacksButton) {
      Rectangle source_menu_bounds = this.mSourceInventoryOptions.mBounds;
      Rectangle player_menu_bounds = this.mPlayerInventoryOptions.mBounds;

      if (createChestColorPicker) {
        this.chestColorPicker = new DiscreteColorPicker(source_menu_bounds.X + 16, source_menu_bounds.Y - 16, 0, new Chest(true));
        this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor(this.GetSourceAs<Chest>().playerChoiceColor);
        (this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);

        this.colorPickerToggleButton =
          new ClickableTextureComponent("",
                                        new Rectangle(source_menu_bounds.Right + 32, source_menu_bounds.Y + 92, 64, 64),
                                        "",
                                        Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
                                        Game1.mouseCursors,
                                        new Rectangle(119, 469, 16, 16),
                                        4f) {
            myID = region_colorPickToggle, downNeighborID = region_fillStacksButton, leftNeighborID = ClickableComponent.SNAP_AUTOMATIC, region = region_organizationButtons
          };

        this.discreteColorPickerCC = new List<ClickableComponent>();

        for (Int32 j = 0; j < this.chestColorPicker.totalColors; j++)
          this.discreteColorPickerCC.Add(new ClickableComponent(new Rectangle(this.chestColorPicker.xPositionOnScreen + borderWidth / 2 + j * 9 * 4,
                                                                              this.chestColorPicker.yPositionOnScreen + borderWidth / 2,
                                                                              36,
                                                                              28),
                                                                "") {
            myID            = j + 4343,
            rightNeighborID = j < this.chestColorPicker.totalColors - 1 ? j + 4343 + 1 : -1,
            leftNeighborID  = j > 0 ? j + 4343 - 1 : -1,
            downNeighborID  = this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count > 0 ? 53910 : 0
          });
      }
      else {
        this.chestColorPicker?.exitThisMenu(false);
        this.discreteColorPickerCC?.Clear();

        this.chestColorPicker        = null;
        this.colorPickerToggleButton = null;
        this.discreteColorPickerCC   = null;
      }

      this.fillStacksButton = createFillStacksButton ?
        new ClickableTextureComponent("",
                                      new Rectangle(source_menu_bounds.Right + 32, source_menu_bounds.Top + 168, 64, 64),
                                      "",
                                      Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"),
                                      Game1.mouseCursors,
                                      new Rectangle(103, 469, 16, 16),
                                      4f) {
          myID           = region_fillStacksButton,
          upNeighborID   = this.colorPickerToggleButton?.myID ?? -500,
          downNeighborID = region_organizeButton,
          leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
          region         = region_organizationButtons
        } :
        null;

      this.organizeButton = createOrganizeButton ?
        new ClickableTextureComponent("",
                                      new Rectangle(source_menu_bounds.Right + 32, source_menu_bounds.Top + 244, 64, 64),
                                      "",
                                      Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"),
                                      Game1.mouseCursors,
                                      new Rectangle(162, 440, 16, 16),
                                      4f) {
          myID           = region_organizeButton,
          upNeighborID   = region_fillStacksButton,
          downNeighborID = region_trashCan,
          leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
          region         = region_organizationButtons
        } :
        null;

      this.ItemsToGrabMenu?.GetBorder(InventoryMenu.BorderSide.Right).ForEach(cc => {
        cc.rightNeighborID = createChestColorPicker ? this.colorPickerToggleButton.myID :
          createOrganizeButton                      ? this.organizeButton.myID :
          createFillStacksButton                    ? this.fillStacksButton.myID : ClickableComponent.SNAP_TO_DEFAULT;
      });
    }

  #endregion

  #endregion

    // Public:
  #region Public

    /// <summary>
    /// The underlying <see cref="ItemGrabMenu"/>'s bound values wrapped in a <see cref="Rectangle"/>.
    /// </summary>
    public Rectangle mBounds {
      get => new(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
      private set {
        this.xPositionOnScreen = value.X;
        this.yPositionOnScreen = value.Y;
        this.width             = value.Width;
        this.height            = value.Height;
      }
    }

    /// <summary>
    /// The <see cref="ItemGrabMenu"/>'s bound calculated by custom properties.
    /// </summary>
    [UsedImplicitly]
    public Rectangle mRealBounds =>
      new(Math.Min(this.mSourceInventoryOptions.mBounds.X, this.mPlayerInventoryOptions.mBounds.X), 
          this.mSourceInventoryOptions.mBounds.Y,
          Math.Max(this.mSourceInventoryOptions.mBounds.Width, this.mPlayerInventoryOptions.mBounds.Width), 
          this.mPlayerInventoryOptions.mBounds.Bottom - this.mSourceInventoryOptions.mBounds.Y);

    public Boolean mIsVisible { get; protected set; }

    public List<CustomItemGrabMenuItem> mMenuItems { get; set; }

    public Vector2                 mSourceTile { get; private set; }
    public ExtendedChest.ChestType mSourceType { get; private set; }

    public T GetSourceAs<T>() where T : Item { return this.mSVSourceItem is T ? this.mSVSourceItem as T : null; }

    // Virtuals:
  #region Virtuals

    /// <summary>
    /// Base implementation sets '<see cref="mIsVisible"/>' and this menu's items' visibility to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this menu should be visible.</param>
    public virtual void SetVisible(Boolean isVisible) {
      this.mIsVisible = isVisible;
      this.mMenuItems.ForEach(i => i.SetVisible(isVisible));
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
      this.mMenuItems?.ForEach(i => i?.Dispose());
      this.mMenuItems?.Clear();

      this.UnregisterInputEvents();
    }

  #endregion

    // Overrides:
  #region Overrides

    /// <summary>
    /// Override function to return new column count.
    /// </summary>
    /// <returns>Base implementation returns '<see cref="Config.mColumns"/>'.</returns>
    public override Int32 GetColumnCount() { return Config.Get().mColumns; }

    /// <summary>
    /// Base implementation calls '<see cref="ItemGrabMenu.draw(SpriteBatch)"/>' if '<see cref="mIsVisible"/>' is true and likewise with this menu's items.
    /// </summary>
    public override void draw(SpriteBatch b) {
      if (!this.mIsVisible) return;

      Color chest_bg_colour    = this.mSourceInventoryOptions.mBackgroundColour;
      Color backpack_bg_colour = this.mPlayerInventoryOptions.mBackgroundColour;
      Point mouse_pos          = Game1.getMousePosition();

      if (this.mSourceInventoryOptions.mIsVisible) {
        Rectangle wrap_rectangle = this.mSourceInventoryOptions.mBounds;
        Game1.drawDialogueBox(wrap_rectangle.X,
                              wrap_rectangle.Y,
                              wrap_rectangle.Width,
                              wrap_rectangle.Height,
                              false,
                              true,
                              r: chest_bg_colour.R,
                              g: chest_bg_colour.G,
                              b: chest_bg_colour.B);
        this.ItemsToGrabMenu.DrawEx(b, chest_bg_colour);
      }

      if (this.mPlayerInventoryOptions.mIsVisible) {
        Rectangle wrap_rectangle = this.mPlayerInventoryOptions.mBounds;
        Game1.drawDialogueBox(wrap_rectangle.X,
                              wrap_rectangle.Y,
                              wrap_rectangle.Width,
                              wrap_rectangle.Height,
                              false,
                              true,
                              r: backpack_bg_colour.R,
                              g: backpack_bg_colour.G,
                              b: backpack_bg_colour.B);
        this.inventory.DrawEx(b, backpack_bg_colour);
      }

      // Game code, slightly modified
      {
        this.mSVPoof?.draw(b, true);
        this._transferredItemSprites?.ForEach(s => s.Draw(b));

        // draw organization buttons
        this.colorPickerToggleButton?.draw(b);
        this.chestColorPicker?.draw(b);
        this.organizeButton?.draw(b);
        this.fillStacksButton?.draw(b);

        this.okButton?.draw(b);

        if (this.trashCan is not null) {
          this.trashCan.draw(b);
          b.Draw(Game1.mouseCursors,
                 new Vector2(this.trashCan.bounds.X + 60.0f, this.trashCan.bounds.Y + 40.0f),
                 new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10),
                 Color.White,
                 this.trashCanLidRotation,
                 new Vector2(16.0f, 10.0f),
                 4f,
                 SpriteEffects.None,
                 0.872f);
        }

        this.mMenuItems.ForEach(i => {
          if (i.mIsVisible) i.Draw(b);
        });

        // draw hover tooltip/text
        Color target_colour = this.mSourceInventoryOptions.mBounds.Contains(mouse_pos.X, mouse_pos.Y) ? chest_bg_colour : backpack_bg_colour;

        if (this.hoveredItem is not null) {
          CustomMenu.DrawToolTip(b,
                                 Game1.smallFont,
                                 target_colour,
                                 this.hoveredItem.getDescription(),
                                 this.hoveredItem.DisplayName,
                                 this.hoveredItem,
                                 this.heldItem is not null);
        }
        else if (!String.IsNullOrEmpty(this.hoverText)) {
          if (this.hoverAmount > 0)
            CustomMenu.DrawToolTip(b,
                                   Game1.smallFont,
                                   target_colour,
                                   this.hoverText,
                                   "",
                                   null,
                                   true,
                                   0,
                                   this.hoverAmount);
          else
            CustomMenu.DrawHoverText(b, Game1.smallFont, this.hoverText, backgroundColour: target_colour, textColour: target_colour.ContrastColour());
        }

        // draw held item
        this.heldItem?.drawInMenu(b, mouse_pos.AsXNAVector2() + new Vector2(8.0f), 1f);
      }
    }

    /// <summary>
    /// Base implementation calls '<see cref="IClickableMenu.draw(SpriteBatch, Int32, Int32, Int32)"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    public override void draw(SpriteBatch b, Int32 red = -1, Int32 green = -1, Int32 blue = -1) {
      if (this.mIsVisible) base.draw(b, red, green, blue);
    }

    /// <summary>
    /// Base implementation calls '<see cref="MenuWithInventory.draw(SpriteBatch, Boolean, Boolean, Int32, Int32, Int32)"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    public override void draw(SpriteBatch b,          Boolean drawUpperPortion = true, Boolean drawDescriptionArea = true, Int32 red = -1,
                              Int32       green = -1, Int32   blue             = -1) {
      if (this.mIsVisible) base.draw(b, drawUpperPortion, drawDescriptionArea, red, green, blue);
    }

    /// <summary>
    /// <para>Base implementation:</para>
    /// <para>1. Calls '<see cref="OnClose"/>'.</para>
    /// <para>2. Calls '<see cref="ItemGrabMenu.emergencyShutDown"/>'.</para>
    /// </summary>
    public override void emergencyShutDown() {
      Console.WriteLine(@"ICustomItemGrabMenu.emergencyShutdown");
      this.OnClose();
      base.emergencyShutDown();
    }

    /// <summary>
    /// <para>Base implementation:</para>
    /// <para>1. Informs '<see cref="ItemGrabMenu.ItemsToGrabMenu"/>'.</para>
    /// <para>2. Informs this menu's items.</para>
    /// <para>3. Calls <see cref="CreateOrganizationButtons"/>.</para>
    /// </summary>
    /// <remarks>Does *NOT* call '<see cref="ItemGrabMenu.gameWindowSizeChanged(Rectangle, Rectangle)"/>'.</remarks>
    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
      // TODO: may need .movePosition for sub inventories??
      // inform ItemsToGrabMenu
      this.ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
      // inform inventory
      this.inventory.gameWindowSizeChanged(oldBounds, newBounds);
      // reset bounds
      this.mSourceInventoryOptions.mBounds = this.ItemsToGrabMenu.GetDialogueBoxRectangle();
      this.mPlayerInventoryOptions.mBounds = this.inventory.GetDialogueBoxRectangle();
      // inform items
      this.mMenuItems.ForEach(i => i.OnGameWindowSizeChanged(oldBounds, newBounds));
      // re-create organization buttons
      this.CreateOrganizationButtons(this.source == source_chest && this.mSVSourceItem is Chest chest && chest.SpecialChestType == Chest.SpecialChestTypes.None,
                                     this.organizeButton is not null,
                                     this.fillStacksButton is not null);
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

        if (!this.isWithinBounds(x, y) && this.readyToClose() && this.trashCan is not null && this.trashCan.containsPoint(x, y)) Game1.SetFreeCursorDrag();

        if (this.okButton is not null && this.okButton.containsPoint(x, y) && this.readyToClose()) {
          this.exitThisMenu();
          Game1.playSound("bigDeSelect");
        }

        if (this.trashCan is not null && this.trashCan.containsPoint(x, y) && this.heldItem is not null && this.heldItem.canBeTrashed()) {
          Utility.trashItem(this.heldItem);
          this.heldItem = null;
        }
      }

      // Handle item storing/retrieving
      if (this.heldItem is null && this.showReceivingMenu) {
        this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);

        if (this.heldItem is not null) this.behaviorOnItemGrab?.Invoke(this.heldItem, Game1.player);

        if (Game1.player.addItemToInventoryBool(this.heldItem)) {
          this.heldItem = null;
          Game1.playSound("coin");
        }
      }
      else if ((this.reverseGrab || this.mSVBehaviorFunction is not null) && this.isWithinBounds(x, y)) {
        if (this.heldItem is not null) this.mSVBehaviorFunction?.Invoke(this.heldItem, Game1.player);

        if (this.destroyItemOnClick) this.heldItem = null;
      }

      // Handle default chest colour picker
      if (this.chestColorPicker is DiscreteColorPicker) {
        this.chestColorPicker.receiveLeftClick(x, y);
        if (this.mSVSourceItem is Chest chest && this.chestColorPicker.visible && this.chestColorPicker.isWithinBounds(x, y))
          chest.playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);

        if (this.colorPickerToggleButton is not null && this.colorPickerToggleButton.containsPoint(x, y)) {
          Game1.player.showChestColorPicker.Flip();
          this.chestColorPicker.visible = Game1.player.showChestColorPicker;
          Game1.playSound("drumkit6");

          this.SetupBorderNeighbors();
        }
      }

      // Handle default organize button
      if (this.organizeButton is not null && this.organizeButton.visible && this.organizeButton.containsPoint(x, y)) {
        organizeItemsInList(this.ItemsToGrabMenu.actualInventory);
        Game1.playSound("Ship");

        return;
      }

      // Handle default fill stacks button
      if (this.fillStacksButton is not null && this.fillStacksButton.visible && this.fillStacksButton.containsPoint(x, y)) {
        this.FillOutStacks();
        Game1.playSound("Ship");
      }

      // Drop item if dragging it out of bounds
      if (this.heldItem is not null && (!this.isWithinBounds(x, y) || this.dropItemInvisibleButton.containsPoint(x, y)) && this.heldItem.canBeTrashed()) this.DropHeldItem();
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

        this.heldItem = this.inventory.rightClick(x, y, this.heldItem, playSound);
      }

      // Handle item storing/retrieving
      if (this.heldItem is null && this.showReceivingMenu) {
        this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);

        if (this.heldItem is not null) this.behaviorOnItemGrab?.Invoke(this.heldItem, Game1.player);

        if (Game1.player.addItemToInventoryBool(this.heldItem)) {
          this.heldItem = null;
          Game1.playSound("coin");
        }
      }
      else if ((this.reverseGrab || this.mSVBehaviorFunction is not null) && this.isWithinBounds(x, y)) {
        if (this.heldItem is not null) this.mSVBehaviorFunction?.Invoke(this.heldItem, Game1.player);

        if (this.destroyItemOnClick) this.heldItem = null;
      }
    }

  #endregion

    // ItemGrabMenu submenu options:
  #region ItemGrabMenu submenu options

    public struct InventoryMenuOptions {
      private readonly Action<Boolean> fnSetVisibleExtra;

      public Color mBackgroundColour { get; set; }

      public Rectangle mBounds { get; set; }

      public Boolean mIsVisible { get; private set; }

      public Rectangle mSafeContentRegion {
        get {
          Rectangle bounds = this.mBounds;

          return new Rectangle(bounds.X + 36, bounds.Y + spaceToClearTopBorder + 4, bounds.Width - borderWidth - 32, bounds.Height - spaceToClearTopBorder - 40);
        }
      }

      public void SetVisible(Boolean isVisible) {
        this.mIsVisible = isVisible;
        this.fnSetVisibleExtra?.Invoke(isVisible);
      }

      public InventoryMenuOptions(Rectangle bounds, Color backgroundColour, Boolean isVisible = true, Action<Boolean> setVisibleExtraFunctionality = null) {
        this.fnSetVisibleExtra = setVisibleExtraFunctionality;

        this.mBackgroundColour = backgroundColour;
        this.mBounds           = bounds;
        this.mIsVisible        = !isVisible;

        this.SetVisible(isVisible);
      }
    }

    public InventoryMenuOptions mPlayerInventoryOptions;
    public InventoryMenuOptions mSourceInventoryOptions;

  #endregion

    // Shadowed ItemGrabMenu functions (to redirect to CustomItemGrabMenu):
  #region Shadowed ItemGrabMenu functions (to redirect to CustomItemGrabMenu)

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public new CustomItemGrabMenu setEssential(Boolean essential) {
      this.mSVEssential = essential;

      return this;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public new void setSourceItem(Item item) {
      this.mSVSourceItem = item;
      this.CreateOrganizationButtons(this.chestColorPicker is not null, this.organizeButton is not null, this.fillStacksButton is not null);
    }

  #endregion

    // Inaccessible 'ItemGrabMenu + bases' fields exposed through reflection properties:
  #region Inaccessible 'ItemGrabMenu + bases' fields exposed through reflection properties

    public behaviorOnItemSelect mSVBehaviorFunction {
      get => Traverse.Create(this).Field<behaviorOnItemSelect>("behaviorFunction").Value;
      set => Traverse.Create(this).Field<behaviorOnItemSelect>("behaviorFunction").Value = value;
    }

    public Boolean mSVEssential {
      get => Traverse.Create(this).Field<Boolean>("essential").Value;
      set => Traverse.Create(this).Field<Boolean>("essential").Value = value;
    }

    public Item mSVHoverItem {
      get => Traverse.Create(this).Field<Item>("hoverItem").Value;
      set => Traverse.Create(this).Field<Item>("hoverItem").Value = value;
    }

    public String mSVMessage {
      get => Traverse.Create(this).Field<String>("message").Value;
      set => Traverse.Create(this).Field<String>("message").Value = value;
    }

    public TemporaryAnimatedSprite mSVPoof {
      get => Traverse.Create(this).Field<TemporaryAnimatedSprite>("poof").Value;
      set => Traverse.Create(this).Field<TemporaryAnimatedSprite>("poof").Value = value;
    }

    public Boolean mSVSnappedtoBottom {
      get => Traverse.Create(this).Field<Boolean>("snappedtoBottom").Value;
      set => Traverse.Create(this).Field<Boolean>("snappedtoBottom").Value = value;
    }

    public Item mSVSourceItem {
      get => Traverse.Create(this).Field<Item>("sourceItem").Value;
      set => Traverse.Create(this).Field<Item>("sourceItem").Value = value;
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    /// <summary>
    /// Called by patched code in the game. Parameters are 1:1 with the original constructor.
    /// </summary>
    /// <remarks>All '<see cref="ItemGrabMenu"/>' constructors should be patched to skip running their code in order for this menu to function correctly.</remarks>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "This ctor is arbitrated from game code.")]
    public CustomItemGrabMenu(IList<Item> inventory, Boolean reverseGrab, Boolean showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction,
                              behaviorOnItemSelect behaviorOnItemSelectFunction, String message, behaviorOnItemSelect behaviorOnItemGrab = null, Boolean snapToBottom = false,
                              Boolean canBeExitedWithKey = false, Boolean playRightClickSound = true, Boolean allowRightClick = true, Boolean showOrganizeButton = false,
                              Int32 source = 0, Item sourceItem = null, Int32 whichSpecialButton = -1, Object context = null) : base(null,
                                                                                                                                     new
                                                                                                                                       MenuWithInventoryCtorParams(highlightFunction,
                                                                                                                                                                   true,
                                                                                                                                                                   true,
                                                                                                                                                                   0,
                                                                                                                                                                   0,
                                                                                                                                                                   64)) {
      // Set custom values
      {
        this.mMenuItems = new List<CustomItemGrabMenuItem>();
        this.SetVisible(true);

        Object source_real = sourceItem is Chest ? sourceItem : context;

        // set SourceType
        if (source_real is Chest source_item) {
          // set SourceTile
          this.mSourceTile = new Vector2((Single)source_item.boundingBox.Value.X / Game1.tileSize, (Single)source_item.boundingBox.Value.Y / Game1.tileSize);

          this.mSourceType = source_item.GetChestType();
        }
        else
          this.mSourceType = ExtendedChest.ChestType.None;

        // register menu exit function
        this.exitFunction = this.OnClose;
      }

      // construct ItemGrabMenu, mostly game code albeit slightly modified
      {
        this.source                       = source;
        this.mSVMessage                   = message;
        this.reverseGrab                  = reverseGrab;
        this.showReceivingMenu            = showReceivingMenu;
        this.playRightClickSound          = playRightClickSound;
        this.allowRightClick              = allowRightClick;
        this.inventory.showGrayedOutSlots = true;
        this.whichSpecialButton           = whichSpecialButton;
        this.context                      = context;
        this.mSVBehaviorFunction          = behaviorOnItemSelectFunction;
        this.behaviorOnItemGrab           = behaviorOnItemGrab;
        this.canExitOnKey                 = canBeExitedWithKey;

        this.mSVSourceItem                = sourceItem;
        this._sourceItemInCurrentLocation = sourceItem != null && Game1.currentLocation.objects.Values.Contains(sourceItem);

        // create chest menu
        {
          this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + 32,
                                                   this.yPositionOnScreen,
                                                   false,
                                                   inventory,
                                                   highlightFunction,
                                                   Config.Get().GetCapacity(),
                                                   Config.Get().mRows,
                                                   1,
                                                   1);
          this.ItemsToGrabMenu.populateClickableComponentList();

          foreach (ClickableComponent cc in this.ItemsToGrabMenu.inventory.Where(cc => cc != null)) {
            cc.myID            += region_itemsToGrabMenuModifier;
            cc.upNeighborID    += region_itemsToGrabMenuModifier;
            cc.rightNeighborID += region_itemsToGrabMenuModifier;
            cc.downNeighborID  =  ClickableComponent.CUSTOM_SNAP_BEHAVIOR;
            cc.leftNeighborID  += region_itemsToGrabMenuModifier;
            cc.fullyImmutable  =  true;
          }
        }

        // give trash can neighbour ids
        {
          if (this.trashCan is not null) this.trashCan.leftNeighborID = 11;
        }

        // give ok button neighbour ids
        {
          if (this.okButton is not null) {
            this.okButton.leftNeighborID = 11;

            for (Int32 i = 0; i < this.inventory.inventory.Count; i += 11)
              if (this.inventory.inventory[i] is ClickableComponent cc)
                cc.rightNeighborID = this.okButton.myID;
          }
        }

        this.populateClickableComponentList();
        if (Game1.options.SnappyMenus) this.snapToDefaultClickableComponent();
        this.SetupBorderNeighbors();
      }

      // construct options
      {
        Color bg_col = Color.White;

        if (this.mSourceType == ExtendedChest.ChestType.WoodenChest || this.mSourceType == ExtendedChest.ChestType.StoneChest) bg_col = this.GetSourceAs<Chest>().GetActualColour();

        this.mPlayerInventoryOptions = new InventoryMenuOptions(this.inventory.GetDialogueBoxRectangle(), Color.White, true, isVisible => this.inventory.SetVisibleEx(isVisible));

        this.mSourceInventoryOptions =
          new InventoryMenuOptions(this.ItemsToGrabMenu.GetDialogueBoxRectangle(), bg_col, true, isVisible => this.ItemsToGrabMenu.SetVisibleEx(isVisible));
      }
    }

  #endregion
  }
}

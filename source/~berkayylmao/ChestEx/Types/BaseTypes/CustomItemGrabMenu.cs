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
//    but WITHOUT ANY WARRANTY, without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

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
  /// </summary>
  /// <remarks>However, this class requires a patch to the '<see cref="ItemGrabMenu(System.Collections.Generic.IList{StardewValley.Item}, object)"/>' constructor so that <see cref="ItemGrabMenu"/> itself is never initialized.</remarks>
  [UsedImplicitly]
  public partial class CustomItemGrabMenu : ItemGrabMenu {
    // Private:
  #region Private

    // Input events:
  #region Input events

    private readonly Dictionary<SButton, InputStateEx> inputStates = new();

    private void _eventCursorMoved(Object sender, CursorMovedEventArgs e) {
      Vector2 cursor_pos = Utility.ModifyCoordinatesForUIScale(e.NewPosition.ScreenPixels);
      // Update cursor position
      InputStateEx.gCursorPos = cursor_pos;

      // Inform items of CursorMoved event if they are interactable
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (!i.mData.mIsEnabled || !i.mIsVisible) return;
        // Update cursor status of item
        i.mData.UpdateCursorStatus(i.mBounds.Contains(cursor_pos));
        // Inform
        i.OnCursorMoved(cursor_pos);
      }));
    }
    private void _eventButtonPressed(Object sender, ButtonPressedEventArgs e) {
      Vector2 cursor_pos = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
      // Update cursor position
      InputStateEx.gCursorPos = cursor_pos;
      // Get input
      InputStateEx input_state = this.inputStates[e.Button];
      input_state.mButtonState = SButtonState.Pressed;

      // Inform items of ButtonPressed event if they are interactable
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (!i.mData.mIsEnabled || !i.mIsVisible) return;

        // Update cursor status of item
        i.mData.UpdateCursorStatus(i.mBounds.Contains(cursor_pos), input_state);
        // Inform
        i.OnButtonPressed(input_state);
      }));
    }
    private void _eventButtonReleased(Object sender, ButtonReleasedEventArgs e) {
      Vector2 cursor_pos = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels);
      // Update cursor position
      InputStateEx.gCursorPos = cursor_pos;
      // Get input
      InputStateEx input_state = this.inputStates[e.Button];
      input_state.mButtonState = SButtonState.Released;

      // Inform items of MouseClick event if needed
      if (e.Button is SButton.MouseLeft or SButton.MouseRight or SButton.MouseMiddle && input_state.mLastButtonState == SButtonState.Pressed) {
        this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
          if (!i.mData.mIsEnabled || !i.mIsVisible) return;
          if (!i.mBounds.Contains(InputStateEx.gLastCursorPos) || !i.mBounds.Contains(cursor_pos)) return;

          // Update cursor status of item
          i.mData.UpdateCursorStatus(true, input_state);
          // Inform
          i.OnMouseClick(input_state);
        }));
      }

      // Inform items of ButtonReleased event if they are interactable
      this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
        if (!i.mData.mIsEnabled || !i.mIsVisible) return;

        // Update cursor status of item
        i.mData.UpdateCursorStatus(i.mBounds.Contains(cursor_pos), input_state);
        // Inform
        i.OnButtonReleased(input_state);
      }));
    }

  #endregion

  #endregion

    // Protected:
  #region Protected

    // Input event handlers:
  #region Input event handlers

    protected void RegisterInputEvents() {
      // init input states
      foreach (SButton btn in (SButton[])Enum.GetValues(typeof(SButton))) { this.inputStates[btn] = new InputStateEx(btn); }

      GlobalVars.gSMAPIHelper.Events.Input.CursorMoved    += this._eventCursorMoved;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonPressed  += this._eventButtonPressed;
      GlobalVars.gSMAPIHelper.Events.Input.ButtonReleased += this._eventButtonReleased;
    }
    protected void UnregisterInputEvents() {
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
      Rectangle source_menu_bounds = this.mSourceInventoryOptions.mDialogueBoxBounds;

      if (createChestColorPicker) {
        this.chestColorPicker                = new DiscreteColorPicker(source_menu_bounds.X + 16, source_menu_bounds.Y - 16, 0, new Chest(true));
        this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor(this.GetSourceAs<Chest>().playerChoiceColor);
        // TODO: this crashes?
        ((Chest)this.chestColorPicker.itemToDrawColored).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);

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
    /// The <see cref="ItemGrabMenu"/>'s bound calculated by custom properties.
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public Rectangle mRealBounds =>
      new(Math.Min(this.mSourceInventoryOptions.mDialogueBoxBounds.X, this.mPlayerInventoryOptions.mDialogueBoxBounds.X),
          this.mSourceInventoryOptions.mDialogueBoxBounds.Y,
          Math.Max(this.mSourceInventoryOptions.mDialogueBoxBounds.Width, this.mPlayerInventoryOptions.mDialogueBoxBounds.Width),
          this.mPlayerInventoryOptions.mDialogueBoxBounds.Bottom - this.mSourceInventoryOptions.mDialogueBoxBounds.Y);

    public Boolean mIsVisible { get; private set; } = true;

    public List<ICustomComponent> mComponents { get; } = new();

    public ExtendedChest.ChestType mSourceType { get; }
    public T GetSourceAs<T>()
      where T : Item {
      return this.mSVSourceItem as T;
    }

    // Virtuals:
  #region Virtuals

    /// <summary>
    /// Sets '<see cref="mIsVisible"/>' and this menu's components' visibility to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this menu should be visible.</param>
    public void SetVisible(Boolean isVisible) {
      this.mIsVisible = isVisible;
      this.mComponents.ForEach(i => i.SetVisible(isVisible));
    }
    /// <summary>
    /// <para>1. Unregisters '<see cref="IClickableMenu.exitFunction"/>'.</para>
    /// <para>2. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
    /// <para>3. Disposes of this menu's items.</para>
    /// <para>4. Unregisters SMAPI input events.</para>
    /// </summary>
    public void OnClose() {
      // unregister menu exit function
      this.exitFunction = null;

      // hide
      this.SetVisible(false);

      // dispose items
      this.mComponents?.ForEach(i => i?.Dispose());
      this.mComponents?.Clear();

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

      var   chest_colours    = Colours.GenerateFrom(this.mSourceInventoryOptions.mBackgroundColour);
      var   backpack_colours = Colours.GenerateFrom(this.mPlayerInventoryOptions.mBackgroundColour);
      Point mouse_pos        = InputStateEx.gCursorPos.AsXNAPoint();

      if (this.mSourceInventoryOptions.mIsVisible) {
        b.DrawTextureBox(this.mSourceInventoryOptions.mDialogueBoxBounds, chest_colours);
        this.ItemsToGrabMenu.DrawEx(b, chest_colours.mActiveColour);
      }

      if (this.mPlayerInventoryOptions.mIsVisible) {
        b.DrawTextureBox(this.mPlayerInventoryOptions.mDialogueBoxBounds, backpack_colours);
        this.inventory.DrawEx(b, backpack_colours.mActiveColour);
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

        this.mComponents.ForEach(i => {
          if (i.mIsVisible) i.Draw(b);
        });

        // draw hover tooltip/text
        Colours tooltip_colours = this.mPlayerInventoryOptions.mDialogueBoxBounds.Contains(mouse_pos.X, mouse_pos.Y) ? backpack_colours : chest_colours;

        if (this.hoveredItem is not null) {
          b.DrawToolTip(Game1.smallFont,
                        this.hoveredItem.getDescription(),
                        this.hoveredItem.DisplayName,
                        this.hoveredItem,
                        this.heldItem is not null,
                        colours: tooltip_colours,
                        contentPadding: 8,
                        borderScale: 0.5f);
        }
        else if (!String.IsNullOrEmpty(this.hoverText)) {
          if (this.hoverAmount > 0) {
            b.DrawToolTip(Game1.smallFont,
                          this.hoverText,
                          "",
                          null,
                          true,
                          0,
                          this.hoverAmount,
                          colours: tooltip_colours,
                          contentPadding: 8,
                          borderScale: 0.5f);
          }
          else { b.DrawHoverText(Game1.smallFont, this.hoverText, colours: tooltip_colours, borderScale: 0.5f); }
        }

        // draw held item
        this.heldItem?.drawInMenu(b, mouse_pos.AsXNAVector2() + new Vector2(8.0f), 1f);
      }
    }
    /// <summary>
    /// Base implementation calls '<see cref="IClickableMenu.draw(SpriteBatch, Int32, Int32, Int32)"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter", Justification = "Game code")]
    public override void draw(SpriteBatch b, Int32 red = -1, Int32 green = -1, Int32 blue = -1) {
      if (this.mIsVisible) base.draw(b, red, green, blue);
    }
    /// <summary>
    /// Base implementation calls '<see cref="MenuWithInventory.draw(SpriteBatch, Boolean, Boolean, Int32, Int32, Int32)"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter", Justification = "Game code")]
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
      // reset dialogue box bounds
      this.mSourceInventoryOptions.mDialogueBoxBounds = this.ItemsToGrabMenu.GetTextureBoxRectangle();
      this.mPlayerInventoryOptions.mDialogueBoxBounds = this.inventory.GetTextureBoxRectangle();
      // inform items
      this.mComponents.ForEach(i => i.OnGameWindowSizeChanged(oldBounds, newBounds));
      // re-create organization buttons
      this.CreateOrganizationButtons(this.source == source_chest && this.mSVSourceItem is Chest { SpecialChestType: Chest.SpecialChestTypes.None },
                                     this.organizeButton is not null,
                                     this.fillStacksButton is not null);
    }

    public override void performHoverAction(Int32 x, Int32 y) {
      this.hoveredItem = this.inventory.hover(x, y, this.heldItem);
      this.hoverText   = this.inventory.hoverText;
      this.hoverAmount = 0;

      if (this.trashCan is not null && this.trashCan.containsPoint(x, y)) {
        if (this.trashCanLidRotation <= 0.0f) Game1.playSound("trashcanlid");

        this.trashCanLidRotation = Convert.ToSingle(Math.Min(this.trashCanLidRotation + Math.PI / 48.0f, Math.PI / 2.0f));
        if (this.heldItem is not null && Utility.getTrashReclamationPrice(this.heldItem, Game1.player) > 0) {
          this.hoverText   = Game1.content.LoadString("Strings\\UI:TrashCanSale");
          this.hoverAmount = Utility.getTrashReclamationPrice(this.heldItem, Game1.player);
        }
      }
      else { this.trashCanLidRotation = Convert.ToSingle(Math.Max(this.trashCanLidRotation - Math.PI / 48.0f, 0.0f)); }

      if (this.showReceivingMenu) this.hoveredItem ??= this.ItemsToGrabMenu.hover(x, y, this.heldItem);
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
      if (this.chestColorPicker is DiscreteColorPicker picker) {
        picker.receiveLeftClick(x, y);
        if (this.mSVSourceItem is Chest chest && picker.visible && picker.isWithinBounds(x, y)) chest.playerChoiceColor.Value = picker.getColorFromSelection(picker.colorSelection);

        if (this.colorPickerToggleButton is not null && this.colorPickerToggleButton.containsPoint(x, y)) {
          Game1.player.showChestColorPicker.Flip();
          picker.visible = Game1.player.showChestColorPicker;
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

      public Color     mBackgroundColour;
      public Rectangle mDialogueBoxBounds;
      public Boolean   mIsVisible;
      public Single    mBorderScale;

      public void SetVisible(Boolean isVisible) {
        this.mIsVisible = isVisible;
        this.fnSetVisibleExtra?.Invoke(isVisible);
      }

      public InventoryMenuOptions(Rectangle dialogueBoxBounds, Color backgroundColour, Boolean isVisible = true, Action<Boolean> setVisibleExtraFunctionality = null,
                                  Single    borderScale = 1.0f) {
        this.fnSetVisibleExtra = setVisibleExtraFunctionality;

        this.mBackgroundColour  = backgroundColour;
        this.mDialogueBoxBounds = dialogueBoxBounds;
        this.mIsVisible         = !isVisible;
        this.mBorderScale       = borderScale;

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
                              Int32 source = 0, Item sourceItem = null, Int32 whichSpecialButton = -1, Object context = null)
      : base(null, new MenuWithInventoryCtorParams(highlightFunction, true, true, 0, 0, 64)) {
      // Set custom values
      {
        this.SetVisible(true);

        Object source_real = sourceItem is Chest ? sourceItem : context;

        // set SourceType
        this.mSourceType = source_real is Chest source_item ? source_item.GetChestType() : ExtendedChest.ChestType.None;

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
                                                   6,
                                                   2,
                                                   false);
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
        // ReSharper disable once VirtualMemberCallInConstructor
        if (Game1.options.SnappyMenus) this.snapToDefaultClickableComponent();
        this.SetupBorderNeighbors();
      }

      // construct options
      {
        Color bg_col                                                                                                                  = Color.White;
        if (this.mSourceType == ExtendedChest.ChestType.WoodenChest || this.mSourceType == ExtendedChest.ChestType.StoneChest) bg_col = this.GetSourceAs<Chest>().GetActualColour();

        this.mPlayerInventoryOptions = new InventoryMenuOptions(this.inventory.GetTextureBoxRectangle(), Color.White, true, isVisible => this.inventory.SetVisibleEx(isVisible));
        this.mSourceInventoryOptions =
          new InventoryMenuOptions(this.ItemsToGrabMenu.GetTextureBoxRectangle(), bg_col, true, isVisible => this.ItemsToGrabMenu.SetVisibleEx(isVisible));
      }

      GlobalVars.gSMAPIHelper.Events.GameLoop.UpdateTicked += (_, _) => {
        // Inform items of GameTick event if they are interactable
        this.mComponents.IgnoreExceptions(o => o.ForEach(i => {
          if (i.mData.mIsEnabled && i.mIsVisible) i.OnGameTick();
        }));
      };
    }

  #endregion
  }
}

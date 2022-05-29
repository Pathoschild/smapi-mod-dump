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
//    Copyright (c) 2022 Berkay Yigit <berkaytgy@gmail.com>
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
using System.Linq;

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ChestExMenu.Items;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

using Object = System.Object;

namespace ChestEx.Types.CustomTypes.ChestExMenu {
  public sealed class MainMenu : CustomItemGrabMenu {
    // Private:
    #region Private

    private CustomTextureButton configButton;
    private CustomClickableTextureComponent newFillStacksButton => this.fillStacksButton as CustomClickableTextureComponent;
    private CustomClickableTextureComponent newOrganizeButton => this.organizeButton as CustomClickableTextureComponent;
    private CustomClickableTextureComponent newOKButton => this.okButton as CustomClickableTextureComponent;

    private ChestConfigPanel configPanel;

    #endregion

    // Protected:
    #region Protected

    // Overrides:
    #region Overrides

    protected override void CreateOrganizationButtons(Boolean createChestColorPicker, Boolean createOrganizeButton, Boolean createFillStacksButton) {
      this.mComponents.Clear();

      Rectangle source_menu_bounds = this.mSourceInventoryOptions.mDialogueBoxBounds;
      Rectangle player_menu_bounds = this.mPlayerInventoryOptions.mDialogueBoxBounds;

      if (createChestColorPicker && this.mSourceType != ExtendedChest.ChestType.Fridge) {
        this.configPanel = new ChestConfigPanel(this);
        this.configPanel.SetVisible(false);

        this.configButton = new CustomTextureButton(new Rectangle(source_menu_bounds.Right + 16, source_menu_bounds.Top + 16, 64, 64),
                                                    Colours.gTurnTranslucentOnAction,
                                                    TexturePresets.gConfigButtonTexture,
                                                    "Open configuration panel",
                                                    () => this.configPanel.SetVisible(!this.configPanel.mIsVisible)) {
          myID = region_colorPickToggle,
          upNeighborID = this.colorPickerToggleButton?.myID ?? -500,
          downNeighborID = region_organizeButton,
          leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
          region = region_organizationButtons
        };
      }

      if (createFillStacksButton) {
        this.fillStacksButton =
          new CustomClickableTextureComponent(new Rectangle(source_menu_bounds.Right + 16, this.configButton?.mBounds.Bottom + 16 ?? source_menu_bounds.Top + 16, 64, 64),
                                              Colours.gTurnTranslucentOnAction,
                                              TexturePresets.gFillStacksPickerButtonTexture,
                                              Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks")) {
            myID = region_fillStacksButton,
            upNeighborID = this.configButton?.myID ?? -500,
            downNeighborID = region_organizeButton,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region = region_organizationButtons
          };
      }

      if (createOrganizeButton) {
        this.organizeButton =
          new CustomClickableTextureComponent(new Rectangle(source_menu_bounds.Right + 16, this.fillStacksButton?.bounds.Bottom + 16 ?? source_menu_bounds.Top + 16, 64, 64),
                                              Colours.gTurnTranslucentOnAction,
                                              TexturePresets.gOrganizeButtonTexture,
                                              Game1.content.LoadString("Strings\\UI:ItemGrab_Organize")) {
            myID = region_organizeButton,
            upNeighborID = region_fillStacksButton,
            downNeighborID = region_trashCan,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region = region_organizationButtons
          };
      }

      if (this.dropItemInvisibleButton is not null)
        this.dropItemInvisibleButton.bounds = new Rectangle(this.mSourceInventoryOptions.mDialogueBoxBounds.Right,
                                                            this.mSourceInventoryOptions.mDialogueBoxBounds.Top,
                                                            GlobalVars.gUIViewport.Width - this.mSourceInventoryOptions.mDialogueBoxBounds.Right,
                                                            this.mPlayerInventoryOptions.mDialogueBoxBounds.Bottom);

      if (this.trashCan is not null)
        this.trashCan.bounds = new Rectangle(this.mPlayerInventoryOptions.mDialogueBoxBounds.Right + 24,
                                             this.mPlayerInventoryOptions.mDialogueBoxBounds.Top + 32,
                                             this.trashCan.bounds.Width,
                                             this.trashCan.bounds.Height);

      this.okButton =
        new CustomClickableTextureComponent(new Rectangle(player_menu_bounds.Right + 24,
                                                          this.trashCan?.bounds.Bottom + 16 ?? this.mPlayerInventoryOptions.mDialogueBoxBounds.Top + 32,
                                                          64,
                                                          64),
                                            Colours.gTurnTranslucentOnAction,
                                            TexturePresets.gOKButtonTexture) { myID = region_okButton, upNeighborID = region_trashCan, leftNeighborID = 11 };

      for (Int32 i = 0; i < this.inventory.inventory.Count; i += 11)
        if (this.inventory.inventory[i] is ClickableComponent cc)
          cc.rightNeighborID = this.okButton.myID;

      this.mComponents.AddRange(new ICustomComponent[] { this.configPanel, this.newOKButton, this.newOrganizeButton, this.newFillStacksButton, this.configButton }
                                  .Where(c => c is not null));
    }

    #endregion

    #endregion

    // Public:
    #region Public

    // Overrides:
    #region Overrides

    public override void draw(SpriteBatch b) {
      if (!this.mIsVisible) return;
      if (this.drawBG) b.Draw(Game1.fadeToBlackRect, GlobalVars.gUIViewport, Color.Black * 0.5f);

      if (this.mPlayerInventoryOptions.mIsVisible) {
        // draw backpack icon next to player inventory
        Rectangle player_menu_bounds = this.mPlayerInventoryOptions.mDialogueBoxBounds;
        Point backpack_size = new(player_menu_bounds.Height / 3, player_menu_bounds.Height / 3);

        b.Draw(Game1.uncoloredMenuTexture,
               new Vector2(player_menu_bounds.X - backpack_size.X - 24, player_menu_bounds.Y + player_menu_bounds.Height / 2 - backpack_size.Y / 2),
               Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 38),
               Color.White,
               0f,
               Vector2.Zero,
               backpack_size.X / 64.0f,
               SpriteEffects.None,
               0.86f);
      }

      Action<SpriteBatch> _ = base.draw;
      _(b);

      // draw mouse
      Game1.mouseCursorTransparency = 1.0f;
      this.drawMouse(b);
    }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    public MainMenu(IList<Item> inventory, Boolean reverseGrab, Boolean showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction,
                    behaviorOnItemSelect behaviorOnItemSelectFunction, String message, behaviorOnItemSelect behaviorOnItemGrab = null, Boolean snapToBottom = false,
                    Boolean canBeExitedWithKey = false, Boolean playRightClickSound = true, Boolean allowRightClick = true, Boolean showOrganizeButton = false,
                    Int32 source = 0, Item sourceItem = null, Int32 whichSpecialButton = -1, Object context = null)
      : base(inventory,
             reverseGrab,
             showReceivingMenu,
             highlightFunction,
             behaviorOnItemSelectFunction,
             message,
             behaviorOnItemGrab,
             snapToBottom,
             canBeExitedWithKey,
             playRightClickSound,
             allowRightClick,
             showOrganizeButton,
             source,
             sourceItem,
             whichSpecialButton,
             context) {
      this.UnregisterInputEvents();

      Rectangle ui_viewport = GlobalVars.gUIViewport;

      this.ItemsToGrabMenu = new InventoryMenu(ui_viewport.Width / 2
                                               - this.ItemsToGrabMenu.width / 2
                                               /* organize buttons padding */
                                               - (64 + 16),
                                               Math.Max((GlobalVars.gIsChestsAnywhereLoaded ? 82 : 48) + (GlobalVars.gIsExpandedStorageLoaded ? 52 : 0),
                                                        (Int32)(ui_viewport.Height * 0.5f - this.ItemsToGrabMenu.height * 0.75f)),
                                               false,
                                               this.ItemsToGrabMenu.actualInventory,
                                               this.ItemsToGrabMenu.highlightMethod,
                                               Config.Get().GetCapacity(),
                                               Config.Get().mRows,
                                               this.ItemsToGrabMenu.horizontalGap,
                                               this.ItemsToGrabMenu.verticalGap,
                                               this.ItemsToGrabMenu.drawSlots);
      this.ItemsToGrabMenu.populateClickableComponentList();
      this.mSourceInventoryOptions.mDialogueBoxBounds = this.ItemsToGrabMenu.GetTextureBoxRectangle(borderScale: this.mSourceInventoryOptions.mBorderScale);
      this.mSourceInventoryOptions.SetVisible(this.mSourceInventoryOptions.mIsVisible);
      if (GlobalVars.gIsExpandedStorageLoaded) {
        this.mSourceInventoryOptions.mDialogueBoxBounds.Y -= 52;
        this.mSourceInventoryOptions.mDialogueBoxBounds.Height += 52;
      }

      foreach (ClickableComponent cc in this.ItemsToGrabMenu.inventory.Where(cc => cc != null)) {
        cc.myID += region_itemsToGrabMenuModifier;
        cc.upNeighborID += region_itemsToGrabMenuModifier;
        cc.rightNeighborID += region_itemsToGrabMenuModifier;
        cc.downNeighborID = ClickableComponent.CUSTOM_SNAP_BEHAVIOR;
        cc.leftNeighborID += region_itemsToGrabMenuModifier;
        cc.fullyImmutable = true;
      }

      Int32 compatibility_y_shift = GlobalVars.gIsExpandedStorageLoaded ? 48 : 0;
      this.inventory = new InventoryMenu(this.mSourceInventoryOptions.mDialogueBoxBounds.Center.X - this.inventory.width / 2 - 4,
                                         Math.Min(ui_viewport.Height - this.inventory.height - 32,
                                                  this.mSourceInventoryOptions.mDialogueBoxBounds.Bottom + 48 + compatibility_y_shift),
                                         false,
                                         null,
                                         this.inventory.highlightMethod,
                                         this.inventory.capacity,
                                         this.inventory.rows,
                                         this.inventory.horizontalGap,
                                         this.inventory.verticalGap,
                                         this.inventory.drawSlots);
      this.mPlayerInventoryOptions.mDialogueBoxBounds = this.inventory.GetTextureBoxRectangle(borderScale: this.mPlayerInventoryOptions.mBorderScale);
      this.mPlayerInventoryOptions.SetVisible(this.mPlayerInventoryOptions.mIsVisible);

      this.SetBounds(this.mSourceInventoryOptions.mDialogueBoxBounds.X,
                     this.mSourceInventoryOptions.mDialogueBoxBounds.Y,
                     this.mSourceInventoryOptions.mDialogueBoxBounds.Width,
                     this.mSourceInventoryOptions.mDialogueBoxBounds.Height
                     + this.mSourceInventoryOptions.mDialogueBoxBounds.Y
                     + 32
                     + this.mPlayerInventoryOptions.mDialogueBoxBounds.Height);

      // handle organization buttons
      this.CreateOrganizationButtons(true, true, true);

      // TODO:
      // absolute position/tile position for chest saving?
      // need to sync farmhand hinges color to farmer MP
      // gamepad interaction is fucked

      this.populateClickableComponentList();
      if (Game1.options.SnappyMenus) this.snapToDefaultClickableComponent();
      this.SetupBorderNeighbors();

      this.RegisterInputEvents();
    }

    #endregion
  }
}

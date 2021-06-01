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
  public class MainMenu : CustomItemGrabMenu {
    // Protected:
  #region Protected

    protected ChestConfigPanel mConfigPanel;

    // Overrides:
  #region Overrides

    protected override void CreateOrganizationButtons(Boolean createChestColorPicker, Boolean createOrganizeButton, Boolean createFillStacksButton) {
      Rectangle source_menu_bounds = this.mSourceInventoryOptions.mBounds;
      Rectangle player_menu_bounds = this.mPlayerInventoryOptions.mBounds;

      if (createChestColorPicker && this.mSourceType != ExtendedChest.ChestType.Fridge && this.mConfigPanel is null) {
        this.mConfigPanel = new ChestConfigPanel(this);
        this.mConfigPanel.SetVisible(true);
        this.mMenuItems.Add(this.mConfigPanel);
        this.allClickableComponents?.Add(this.mConfigPanel);
      }

      if (createFillStacksButton)
        this.fillStacksButton =
          new CustomItemGrabMenuTexturedItemPreset(this,
                                                   CustomItemGrabMenuTexturedItemPreset.ButtonType.FillStacks,
                                                   new Rectangle(source_menu_bounds.Right + 16, source_menu_bounds.Top + 90, 64, 64)) {
            myID           = region_fillStacksButton,
            upNeighborID   = this.colorPickerToggleButton?.myID ?? -500,
            downNeighborID = region_organizeButton,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region         = region_organizationButtons
          };

      if (createOrganizeButton)
        this.organizeButton =
          new CustomItemGrabMenuTexturedItemPreset(this,
                                                   CustomItemGrabMenuTexturedItemPreset.ButtonType.Organize,
                                                   new Rectangle(source_menu_bounds.Right + 16, source_menu_bounds.Top + 170, 64, 64)) {
            myID           = region_organizeButton,
            upNeighborID   = region_fillStacksButton,
            downNeighborID = region_trashCan,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            region         = region_organizationButtons
          };

      this.okButton = new CustomItemGrabMenuTexturedItemPreset(this,
                                                               CustomItemGrabMenuTexturedItemPreset.ButtonType.OK,
                                                               new Rectangle(player_menu_bounds.Right + 16, player_menu_bounds.Bottom - 96, 64, 64)) {
        myID = region_okButton, upNeighborID = region_trashCan, leftNeighborID = 11
      };

      for (Int32 i = 0; i < this.inventory.inventory.Count; i += 11)
        if (this.inventory.inventory[i] is ClickableComponent cc)
          cc.rightNeighborID = this.okButton.myID;
    }

  #endregion

  #endregion

    // Public:
  #region Public

    // Overrides:
  #region Overrides

    public override void draw(SpriteBatch b) {
      /*
       *    
       *                       __________ /---------------------------------\
       *                       | Config | |                                 |
       *                       |  BTN   | |                                 |
       *       /-------\       ‾‾‾‾‾‾‾‾‾‾ |                                 |
       *       |       |       __________ |                                 |
       *       | Dummy |       | Colour | |      this.ItemsToGrabMenu       |
       *       | Chest |       |  PLT   | |  (config + colour palette too)  |
       *       |       |       ‾‾‾‾‾‾‾‾‾‾ |                                 |
       *       \-------/       __________ |                                 |
       *                       | Colour | |                                 |
       *                       |  RAW   | |                                 |
       *                       ‾‾‾‾‾‾‾‾‾‾ |                                 |
       *   /----------------\             \---------------------------------/
       *   | ChestsAnywhere |   
       *   \----------------/                  /----------------------\
       *                                       |    this.inventory    |
       *                                       \----------------------/
       *    
       *    
      */

      if (!this.mIsVisible) return;
      if (this.drawBG) b.Draw(Game1.fadeToBlackRect, GlobalVars.gUIViewport, Color.Black * 0.375f);

      Rectangle player_menu_bounds = this.mPlayerInventoryOptions.mBounds;

      // draw backpack icon next to player inventory
      var backpack_size = new Point(player_menu_bounds.Height / 4, player_menu_bounds.Height / 4);
      b.Draw(Game1.uncoloredMenuTexture,
             new Vector2(player_menu_bounds.X - backpack_size.X - 24, player_menu_bounds.Y + player_menu_bounds.Height / 2 - backpack_size.Y / 2 + 24),
             Game1.getSourceRectForStandardTileSheet(Game1.uncoloredMenuTexture, 38),
             Color.White,
             0f,
             Vector2.Zero,
             backpack_size.X / 64.0f,
             SpriteEffects.None,
             0.86f);

      // the rest of the owl
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

    public MainMenu(IList<Item>          inventory,                    Boolean reverseGrab, Boolean showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction,
                    behaviorOnItemSelect behaviorOnItemSelectFunction, String  message, behaviorOnItemSelect behaviorOnItemGrab = null, Boolean snapToBottom = false,
                    Boolean              canBeExitedWithKey = false,   Boolean playRightClickSound = true, Boolean allowRightClick = true, Boolean showOrganizeButton = false,
                    Int32                source             = 0,       Item    sourceItem          = null, Int32 whichSpecialButton = -1, Object context = null)
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
      base.UnregisterInputEvents();

      Rectangle ui_viewport = GlobalVars.gUIViewport;

      this.ItemsToGrabMenu = new InventoryMenu(ui_viewport.Width / 2
                                               - this.ItemsToGrabMenu.width / 2
                                               /* chest icon padding */
                                               + this.ItemsToGrabMenu.width / 24
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
      this.mSourceInventoryOptions.mBounds = this.ItemsToGrabMenu.GetDialogueBoxRectangle();
      this.mSourceInventoryOptions.SetVisible(this.mSourceInventoryOptions.mIsVisible);
      if (GlobalVars.gIsExpandedStorageLoaded) {
        this.mSourceInventoryOptions.mBounds.Y      -= 52;
        this.mSourceInventoryOptions.mBounds.Height += 52;
      }

      foreach (ClickableComponent cc in this.ItemsToGrabMenu.inventory.Where(cc => cc != null)) {
        cc.myID            += region_itemsToGrabMenuModifier;
        cc.upNeighborID    += region_itemsToGrabMenuModifier;
        cc.rightNeighborID += region_itemsToGrabMenuModifier;
        cc.downNeighborID  =  ClickableComponent.CUSTOM_SNAP_BEHAVIOR;
        cc.leftNeighborID  += region_itemsToGrabMenuModifier;
        cc.fullyImmutable  =  true;
      }

      Int32 compatibility_y_shift = GlobalVars.gIsExpandedStorageLoaded ? 48 : 0;
      this.inventory = new InventoryMenu(this.mSourceInventoryOptions.mBounds.Center.X - this.inventory.width / 2 - 4,
                                         Math.Min(ui_viewport.Height - this.inventory.height - 32, this.mSourceInventoryOptions.mBounds.Bottom + 32 + compatibility_y_shift),
                                         false,
                                         null,
                                         this.inventory.highlightMethod,
                                         this.inventory.capacity,
                                         this.inventory.rows,
                                         this.inventory.horizontalGap,
                                         this.inventory.verticalGap,
                                         this.inventory.drawSlots);
      this.mPlayerInventoryOptions.mBounds = this.inventory.GetDialogueBoxRectangle();
      this.mPlayerInventoryOptions.SetVisible(this.mPlayerInventoryOptions.mIsVisible);

      this.SetBounds(this.mSourceInventoryOptions.mBounds.X,
                     this.mSourceInventoryOptions.mBounds.Y,
                     this.mSourceInventoryOptions.mBounds.Width,
                     this.mSourceInventoryOptions.mBounds.Height + this.mSourceInventoryOptions.mBounds.Y + 32 + this.mPlayerInventoryOptions.mBounds.Height);

      // handle organization buttons
      {
        this.CreateOrganizationButtons(true, true, true);

        if (this.dropItemInvisibleButton is not null)
          this.dropItemInvisibleButton.bounds = new Rectangle(this.mSourceInventoryOptions.mBounds.Right,
                                                              this.mSourceInventoryOptions.mBounds.Top,
                                                              ui_viewport.Width - this.mSourceInventoryOptions.mBounds.Right,
                                                              this.mPlayerInventoryOptions.mBounds.Bottom);

        this.trashCan.bounds = new Rectangle(this.mPlayerInventoryOptions.mBounds.Right + 16,
                                             this.mPlayerInventoryOptions.mBounds.Top + 96,
                                             this.trashCan.bounds.Width,
                                             this.trashCan.bounds.Height);
      }

      // TODO:
      // absolute position/tile position for chest saving?
      // need to sync farmhand hinges color to farmer MP
      // gamepad interaction is fucked

      this.populateClickableComponentList();
      if (Game1.options.SnappyMenus) this.snapToDefaultClickableComponent();
      this.SetupBorderNeighbors();

      base.RegisterInputEvents();
    }

  #endregion
  }
}

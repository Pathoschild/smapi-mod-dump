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
using ChestEx.Types.CustomTypes.ChestExMenu.Items;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;

namespace ChestEx.Types.CustomTypes.ChestExMenu {
   public class MainMenu : BaseTypes.ICustomItemGrabMenu {
      // Private:

      // Protected:

      // Overrides:

      protected override BaseTypes.ICustomItemGrabMenu clone() {
         return new MainMenu(
            this.ItemsToGrabMenu.actualInventory, false, true,
            new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), this.sv_behaviorFunction, null,
            this.behaviorOnItemGrab, false, true, true, true, true, this.source, this.sv_sourceItem,
            this.whichSpecialButton, this.context);
      }

      // Public:

      // Overrides:

      public override void draw(SpriteBatch b) {
         /*
          *    
          *                             /---------------------------------\
          *                             |                     | ConfigBTN |
          *                             |                     \-----------|
          *       /-------\             |                                 |
          *       |       |             |                                 |
          *       | Dummy |             |      this.ItemsToGrabMenu       |
          *       | Chest |             |  (config + colour palette too)  |
          *       |       |             |                                 |
          *       \-------/             |                                 |
          *                             \---------------------------------/
          *   /----------------\   
          *   | ChestsAnywhere |   
          *   \----------------/              /----------------------\
          *                                   |    this.inventory    |
          *                                   \----------------------/
          *    
          *    
         */

         Action<SpriteBatch> _ = base.draw;
         _(b);
      }

      // Constructors:

      // TODO: Add custom menu background colour support
      public MainMenu(IList<StardewValley.Item> inventory,
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
         : base(inventory, reverseGrab, showReceivingMenu, highlightFunction, behaviorOnItemSelectFunction, message, behaviorOnItemGrab, snapToBottom, canBeExitedWithKey, playRightClickSound, allowRightClick, showOrganizeButton, source, sourceItem, whichSpecialButton, context) {
         // recreate menu
         {
            var game_viewport = GlobalVars.GameViewport;

            this.inventory = new InventoryMenu(
               game_viewport.Width - this.inventory.width - Convert.ToInt32(game_viewport.Width / (12.0f * StardewValley.Game1.options.zoomLevel)),
               game_viewport.Height - this.inventory.height - (game_viewport.Height / Convert.ToInt32(8.0f * StardewValley.Game1.options.zoomLevel)),
               false, null,
               this.inventory.highlightMethod, this.inventory.capacity, this.inventory.rows,
               this.inventory.horizontalGap, this.inventory.verticalGap, this.inventory.drawSlots);

            this.ItemsToGrabMenu = new InventoryMenu(
               game_viewport.Width - this.ItemsToGrabMenu.width - Convert.ToInt32(game_viewport.Width / (12.0f * StardewValley.Game1.options.zoomLevel)),
               game_viewport.Height / Convert.ToInt32(8.0f * StardewValley.Game1.options.zoomLevel),
               false, this.ItemsToGrabMenu.actualInventory,
               this.ItemsToGrabMenu.highlightMethod, this.ItemsToGrabMenu.capacity, this.ItemsToGrabMenu.rows,
               this.ItemsToGrabMenu.horizontalGap, this.ItemsToGrabMenu.verticalGap, this.ItemsToGrabMenu.drawSlots);

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

            this.PlayerInventoryOptions.Bounds = this.inventory.GetRectangleForDialogueBox();
            this.SourceInventoryOptions.Bounds = this.ItemsToGrabMenu.GetRectangleForDialogueBox();

            this.SetBounds(
               this.SourceInventoryOptions.Bounds.X,
               this.SourceInventoryOptions.Bounds.Y,
               this.SourceInventoryOptions.Bounds.Width,
               this.SourceInventoryOptions.Bounds.Height
                  + (this.PlayerInventoryOptions.Bounds.Y - (this.SourceInventoryOptions.Bounds.Y + this.SourceInventoryOptions.Bounds.Height))
                  + this.PlayerInventoryOptions.Bounds.Height);

            // handle organization buttons
            {
               this.createOrganizationButtons(false, false, false);
               this.okButton = this.trashCan = null;
               if (!(this.dropItemInvisibleButton is null)) {
                  this.dropItemInvisibleButton.bounds = new Rectangle(
                     this.PlayerInventoryOptions.Bounds.X + (this.PlayerInventoryOptions.Bounds.Width / 2),
                     this.PlayerInventoryOptions.Bounds.Y - ((this.PlayerInventoryOptions.Bounds.Y - (this.SourceInventoryOptions.Bounds.Y + this.SourceInventoryOptions.Bounds.Height)) / 2),
                     64, 64);
               }
            }

            this.populateClickableComponentList();
            if (StardewValley.Game1.options.SnappyMenus)
               this.snapToDefaultClickableComponent();
            this.SetupBorderNeighbors();
         }

         this.MenuItems.Add(new ConfigPanel(this));
         this.MenuItems.Add(new ChestColouringPanel(this));
      }
   }
}

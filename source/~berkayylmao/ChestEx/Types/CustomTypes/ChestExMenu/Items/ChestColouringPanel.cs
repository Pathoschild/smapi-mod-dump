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

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
   public partial class ChestColouringPanel : ICustomItemGrabMenuItem {
      // Private:

      private ColourPalette _colourPalette;

      private Boolean _isShowingColourEditors;

      private ExtendedSVObjects.ExtendedChestInCustomItemGrabMenu _panelButton;

      // Component event handlers:
      #region Component event handlers

      private void _componentOnClick(Object sender, ICustomMenu.MouseStateEx mouseState) {
         switch ((sender as BaseTypes.ICustomItemGrabMenuItem.BasicComponent).Name) {
            case "openPanelBTN":
               _isShowingColourEditors.Flip();
               _colourPalette.SetVisible(_isShowingColourEditors);
               this.HostMenu.SourceInventoryOptions.SetVisible(!_isShowingColourEditors);
               break;
            case "palette":
               var pos_as_point = mouseState.Pos.AsXNAPoint();
               this.HostMenu.GetSourceAs<Chest>().playerChoiceColor.Value = _colourPalette.GetColourAt(pos_as_point.X, pos_as_point.Y);
               break;
         }
      }

      #endregion

      // Public:

      // Overrides:

      public override void Draw(SpriteBatch b) {
         if (!this.IsVisible)
            return;

         if (_isShowingColourEditors) {
            var wrap_rectangle_ItemsToGrabMenu = this.HostMenu.ItemsToGrabMenu.GetRectangleForDialogueBox();
            StardewValley.Game1.drawDialogueBox(
               wrap_rectangle_ItemsToGrabMenu.X, wrap_rectangle_ItemsToGrabMenu.Y,
               wrap_rectangle_ItemsToGrabMenu.Width, wrap_rectangle_ItemsToGrabMenu.Height,
               false, true,
               r: this.Colours.BackgroundColour.R,
               g: this.Colours.BackgroundColour.G,
               b: this.Colours.BackgroundColour.B);
         }

         base.Draw(b);
      }

      // Constructors:

      public ChestColouringPanel(ICustomItemGrabMenu hostMenu) : base(hostMenu, GlobalVars.GameViewport, true, Colours.Default) {
         _isShowingColourEditors = false;
         /*
          * 
          * 
            game_viewport.Width - this.ItemsToGrabMenu.width - Convert.ToInt32(game_viewport.Width / (12 * StardewValley.Game1.options.zoomLevel)),
            game_viewport.Height / Convert.ToInt32(8 * StardewValley.Game1.options.zoomLevel),
          * 
          * */

         var source_menu_bounds = this.HostMenu.SourceInventoryOptions.Bounds;
         var player_menu_bounds = this.HostMenu.PlayerInventoryOptions.Bounds;
         var menu_chest_size = new Point(
            source_menu_bounds.Width / Convert.ToInt32(5.83f * StardewValley.Game1.options.zoomLevel),
            source_menu_bounds.Width / Convert.ToInt32(2.91f * StardewValley.Game1.options.zoomLevel) // using 'width' here since Chest.height is basically Chest.width * 2
            );

         _panelButton = new ExtendedSVObjects.ExtendedChestInCustomItemGrabMenu(this,
            new Rectangle(
               (source_menu_bounds.X / 2) - (menu_chest_size.X / 2),
               (this.Bounds.Height / 2) - Convert.ToInt32(menu_chest_size.Y / 1.85f),
               menu_chest_size.X,
               menu_chest_size.Y),
            "openPanelBTN", _componentOnClick, "Configure this chest", BaseTypes.Colours.TurnSlightlyTranslucentOnAction);

         _colourPalette = new ColourPalette(this, this.HostMenu.ItemsToGrabMenu.GetNormalizedInventoryMenuBounds(), _panelButton.MenuChest, "palette", _componentOnClick);
         _colourPalette.SetVisible(false);

         this.Colours = new Colours(Color.FromNonPremultiplied(50, 60, 70, 255), Color.White, Color.White, Color.White);

         this.Components.Add(_panelButton);
         this.Components.Add(_colourPalette);
      }
   }
}

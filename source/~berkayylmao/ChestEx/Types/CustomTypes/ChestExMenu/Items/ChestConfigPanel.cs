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

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

using Object = System.Object;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
  public partial class ChestConfigPanel : CustomItemGrabMenuItem {
    // Private:

    private ExtendedChestInCustomItemGrabMenu chestAsPanelButton;
    private ColourPalette                     colourPalette;
    private ChestSettings                     chestSettings;

    private Boolean isShowingConfigPanel;

    // Component event handlers:
  #region Component event handlers

    private void _componentOnClick(Object sender, CustomMenu.MouseStateEx mouseState) {
      switch ((sender as BasicComponent)?.mName) {
        case "openPanelBTN":
          this.isShowingConfigPanel.Flip();
          this.colourPalette.SetVisible(this.isShowingConfigPanel);
          this.chestSettings.SetVisible(this.isShowingConfigPanel);
          this.mHostMenu.mPlayerInventoryOptions.SetVisible(!this.isShowingConfigPanel);

          break;
      }
    }

  #endregion

    // Public:

    // Overrides:

    public override void Draw(SpriteBatch b) {
      if (!this.mIsVisible) return;

      if (this.isShowingConfigPanel) {
        Rectangle wrap_rect = this.mHostMenu.inventory.GetDialogueBoxRectangle();
        Game1.drawDialogueBox(wrap_rect.X,
                              wrap_rect.Y,
                              wrap_rect.Width,
                              wrap_rect.Height,
                              false,
                              true,
                              r: this.mColours.mBackgroundColour.R,
                              g: this.mColours.mBackgroundColour.G,
                              b: this.mColours.mBackgroundColour.B);
      }

      this.mComponents.ForEach(c => {
        if (c.mIsVisible) c.Draw(b);
      });
    }

    public override void SetVisible(Boolean isVisible) { this.mIsVisible = isVisible; }

    // Constructors:

    public ChestConfigPanel(CustomItemGrabMenu hostMenu) : base(hostMenu, GlobalVars.gUIViewport, true, Colours.GenerateFrom(Color.FromNonPremultiplied(50, 60, 70, 255))) {
      Rectangle source_menu_bounds         = this.mHostMenu.mSourceInventoryOptions.mBounds;
      Rectangle player_menu_content_bounds = this.mHostMenu.inventory.GetContentRectangle();

      this.chestAsPanelButton = new ExtendedChestInCustomItemGrabMenu(this,
                                                                      new Rectangle(source_menu_bounds.X - 64 - 28,
                                                                                    source_menu_bounds.Y + (source_menu_bounds.Height / 2 - 64 / 2),
                                                                                    64,
                                                                                    64),
                                                                      "openPanelBTN",
                                                                      this._componentOnClick,
                                                                      "Toggle configuration panel",
                                                                      Colours.gTurnTranslucentOnAction);
      this.chestAsPanelButton.SetVisible(true);

      this.colourPalette = new ColourPalette(this,
                                             new Rectangle(player_menu_content_bounds.X,
                                                           player_menu_content_bounds.Y,
                                                           ColourPalette.ColourPicker.CONST_X + player_menu_content_bounds.Height + ColourPalette.CONST_BORDER_WIDTH,
                                                           player_menu_content_bounds.Height),
                                             this.chestAsPanelButton.mMenuChest,
                                             "palette",
                                             this._componentOnClick);
      this.colourPalette.SetVisible(false);

      this.chestSettings = new ChestSettings(this,
                                             new Rectangle(player_menu_content_bounds.X + this.colourPalette.mBounds.Width,
                                                           player_menu_content_bounds.Y,
                                                           player_menu_content_bounds.Width - this.colourPalette.mBounds.Width,
                                                           player_menu_content_bounds.Height));
      this.chestSettings.SetVisible(false);

      this.mComponents.Add(this.chestSettings);
      this.mComponents.Add(this.colourPalette);
      this.mComponents.Add(this.chestAsPanelButton);
    }
  }
}

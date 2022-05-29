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

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
  public class ChestConfigPanel : CustomClickableMenu {
    // Private:
    #region Private

    // Consts:
    #region Consts

    private const String CONST_MODE_HEX = "COLOURS_HEX";
    private const String CONST_MODE_HSV_CHEST = "CHEST_HSV";
    private const String CONST_MODE_HSV_HINGES = "HINGES_HSV";
    private const String CONST_MODE_RESET = "COLOURS_RESET";

    #endregion

    private readonly CustomItemGrabMenu hostMenu;
    private Chest hostMenuChest => this.hostMenu.GetSourceAs<Chest>();

    private Boolean colouringChestNeedsReset;
    private readonly ExtendedChest colouringChest;

    private readonly ColouringHEXMenu colouringHEXMenu;
    private readonly ColouringHSVMenu colouringHSVMenu;
    private readonly ColouringMenuSwitcher colouringModeSwitcher;

    private String activeColouringMode = CONST_MODE_HSV_CHEST;

    private void switchColouringMode(String newMode) {
      switch (newMode) {
        case CONST_MODE_HSV_CHEST:
        case CONST_MODE_HSV_HINGES:
          this.colouringHEXMenu.SetVisible(false);

          this.colouringHSVMenu.SetVisible(true);
          this.colouringHSVMenu.SetColour(newMode == CONST_MODE_HSV_CHEST ? this.hostMenuChest.playerChoiceColor.Value : this.hostMenuChest.GetCustomConfigHingesColour(), false);
          break;
        case CONST_MODE_HEX:
          this.colouringHSVMenu.SetVisible(false);

          this.colouringHEXMenu.SetColours(this.hostMenuChest.playerChoiceColor.Value, this.hostMenuChest.GetCustomConfigHingesColour());
          this.colouringHEXMenu.SetVisible(true);
          break;
        case CONST_MODE_RESET:
          this.hostMenuChest.playerChoiceColor.Value = Color.Black;
          this.colouringChest.playerChoiceColor.Value = Color.Black;
          this.colouringChest.mHingesColour = Color.Black;

          this.hostMenu.mSourceInventoryOptions.mBackgroundColour = this.hostMenuChest.GetActualColour();
          this.hostMenuChest.SetCustomConfigHingesColour(Color.Black);

          this.colouringModeSwitcher.ForceSwitch(0, CONST_MODE_HSV_CHEST);
          return;
      }

      this.activeColouringMode = newMode;
    }

    private void onHEXFinalColour(Color chestColour, Color hingesColour) {
      if (this.activeColouringMode != CONST_MODE_HEX) return;

      this.hostMenuChest.playerChoiceColor.Value = chestColour;
      this.colouringChest.playerChoiceColor.Value = chestColour;
      this.colouringChest.mHingesColour = hingesColour;

      this.hostMenu.mSourceInventoryOptions.mBackgroundColour = this.hostMenuChest.GetActualColour();
      this.hostMenuChest.SetCustomConfigHingesColour(hingesColour);
    }

    private void onHSVTryColour(Color colour) {
      this.colouringChestNeedsReset = true;

      switch (this.activeColouringMode) {
        case CONST_MODE_HSV_CHEST:
          this.colouringChest.playerChoiceColor.Value = colour;
          this.hostMenu.mSourceInventoryOptions.mBackgroundColour = this.colouringChest.GetActualColour();
          break;
        case CONST_MODE_HSV_HINGES:
          this.colouringChest.mHingesColour = colour;
          break;
      }
    }

    private void onHSVFinalColour(Color colour) {
      switch (this.activeColouringMode) {
        case CONST_MODE_HSV_CHEST:
          this.hostMenuChest.playerChoiceColor.Value = colour;
          this.hostMenu.mSourceInventoryOptions.mBackgroundColour = this.hostMenuChest.GetActualColour();
          break;
        case CONST_MODE_HSV_HINGES:
          this.hostMenuChest.SetCustomConfigHingesColour(colour);
          break;
      }
    }

    #endregion

    // Public:
    #region Public

    // Overrides:
    #region Overrides

    public override void SetVisible(Boolean isVisible) {
      base.SetVisible(isVisible);

      this.hostMenu.mPlayerInventoryOptions.SetVisible(!isVisible);
    }

    public override void Draw(SpriteBatch spriteBatch) {
      this.mComponents.ForEach(c => {
        if (c.mIsVisible) c.Draw(spriteBatch);
      });

      Rectangle inv_bounds = this.hostMenu.inventory.GetBounds();
      this.colouringChest.Draw(spriteBatch, new Vector2(inv_bounds.X - 118, inv_bounds.Center.Y - 76));
    }

    public override void OnCursorMoved(Vector2 cursorPos) {
      base.OnCursorMoved(cursorPos);

      if (this.colouringChestNeedsReset && (!this.colouringHEXMenu.mBounds.Contains(cursorPos) || !this.colouringHSVMenu.mBounds.Contains(cursorPos))) {
        this.colouringChestNeedsReset = false;

        this.colouringChest.playerChoiceColor.Value = this.hostMenuChest.playerChoiceColor.Value;
        this.colouringChest.mHingesColour = this.hostMenuChest.GetCustomConfigHingesColour();
        this.hostMenu.mSourceInventoryOptions.mBackgroundColour = this.colouringChest.GetActualColour();
      }
    }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    public ChestConfigPanel(CustomItemGrabMenu hostMenu)
      : base(GlobalVars.gUIViewport, Colours.gTransparent) {
      var menu_colour = Colours.GenerateFromMenuTiles();
      var player_chest = hostMenu.GetSourceAs<Chest>();
      Rectangle player_menu_content_bounds = hostMenu.inventory.GetBounds();

      this.colouringChest = new ExtendedChest(new Rectangle(player_menu_content_bounds.X - 24 - 64, player_menu_content_bounds.Center.Y - 32, 64, 64),
                                              player_chest.GetCustomConfigHingesColour(),
                                              hostMenu.mSourceType);
      this.colouringChest.playerChoiceColor.Value = player_chest.playerChoiceColor.Value;

      this.colouringModeSwitcher = new ColouringMenuSwitcher(new Rectangle(player_menu_content_bounds.X - 8,
                                                                           player_menu_content_bounds.Y - 8,
                                                                           48,
                                                                           player_menu_content_bounds.Height + 16),
                                                             menu_colour * 0.75f,
                                                             this.switchColouringMode);
      this.colouringModeSwitcher.AddSwitcher(CONST_MODE_HSV_CHEST, menu_colour, TexturePresets.gChestColouringModeTexture, "Chest colour");
      this.colouringModeSwitcher.AddSwitcher(CONST_MODE_HSV_HINGES, menu_colour, TexturePresets.gChestHingesColouringModeTexture, "Chest hinges colour");
      this.colouringModeSwitcher.AddSwitcher(CONST_MODE_RESET, menu_colour, "R", "Reset colours");
      this.colouringModeSwitcher.AddSwitcher(CONST_MODE_HEX, menu_colour, "#", "Chest colours (HEX)");

      this.colouringHEXMenu = new ColouringHEXMenu(new Rectangle(this.colouringModeSwitcher.mBounds.GetTextureBoxRectangle().Right - 8,
                                                                 player_menu_content_bounds.Y - 8,
                                                                 player_menu_content_bounds.Width / 4,
                                                                 player_menu_content_bounds.Height + 16),
                                                   menu_colour * 0.75f,
                                                   this.onHEXFinalColour);
      this.colouringHEXMenu.SetVisible(false);

      this.colouringHSVMenu = new ColouringHSVMenu(new Rectangle(this.colouringModeSwitcher.mBounds.GetTextureBoxRectangle().Right - 8,
                                                                 player_menu_content_bounds.Y - 8,
                                                                 player_menu_content_bounds.Width / 4,
                                                                 player_menu_content_bounds.Height + 16),
                                                   menu_colour * 0.75f,
                                                   player_chest.playerChoiceColor.Value,
                                                   this.onHSVTryColour,
                                                   this.onHSVFinalColour);
      var chest_settings = new ChestSettings(new Rectangle(this.colouringHSVMenu.mBounds.GetTextureBoxRectangle().Right - 12,
                                                           player_menu_content_bounds.Y + 14,
                                                           player_menu_content_bounds.Width - this.colouringHSVMenu.mBounds.Width - this.colouringModeSwitcher.mBounds.Width,
                                                           player_menu_content_bounds.Height - 28),
                                             menu_colour,
                                             player_chest);

      this.hostMenu = hostMenu;
      this.mComponents.AddRange(new ICustomComponent[] { chest_settings, this.colouringHEXMenu, this.colouringHSVMenu, this.colouringModeSwitcher });
    }

    #endregion
  }
}

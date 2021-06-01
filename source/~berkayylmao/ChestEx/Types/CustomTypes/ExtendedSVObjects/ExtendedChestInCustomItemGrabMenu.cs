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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ExtendedSVObjects {
  public class ExtendedChestInCustomItemGrabMenu : SVObjectInCustomItemGrabMenu {
    // Protected:
  #region Protected

    // Overrides:
  #region Overrides

    protected override void DrawObject(SpriteBatch b) { this.mMenuChest.Draw(b, this.mBounds, this.mTextureTintColourCurrent.A / 255.0f); }

    protected override void PlayObjectAnimation() {
      if (this.mMenuChest.mSVCurrentLidFrame != this.mMenuChest.getLastLidFrame()) this.mMenuChest.mSVCurrentLidFrame++;
    }

    protected override void RevertObjectAnimation() {
      if (this.mMenuChest.mSVCurrentLidFrame != this.mMenuChest.startingLidFrame) this.mMenuChest.mSVCurrentLidFrame--;
    }

  #endregion

  #endregion

    // Public:
  #region Public

    public ExtendedChest mMenuChest => this.GetSVObjectAs<ExtendedChest>();

    // Overrides:
  #region Overrides

    public override void OnMouseClick(CustomMenu.MouseStateEx mouseState) {
      if (mouseState.mButton == SButton.MouseLeft) this.mOnMouseClickEventHandler?.Invoke(this, mouseState);
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public ExtendedChestInCustomItemGrabMenu(CustomItemGrabMenuItem                hostMenuItem,        Rectangle bounds,         String  componentName      = "",
                                             EventHandler<CustomMenu.MouseStateEx> onMouseClick = null, String    hoverText = "", Colours textureTintColours = null)
      : base(hostMenuItem,
             new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height * 2),
             new ExtendedChest(bounds, hostMenuItem.mHostMenu.GetSourceAs<Chest>().GetCustomConfigHingesColour(), hostMenuItem.mHostMenu.GetSourceAs<Chest>().GetChestType()),
             false,
             -1.0f,
             componentName,
             onMouseClick,
             hoverText,
             textureTintColours) {
      // sync dummy chest
      this.mMenuChest.playerChoiceColor.Value = this.mHostMenuItem.mHostMenu.GetSourceAs<Chest>().playerChoiceColor.Value;
    }

  #endregion
  }
}

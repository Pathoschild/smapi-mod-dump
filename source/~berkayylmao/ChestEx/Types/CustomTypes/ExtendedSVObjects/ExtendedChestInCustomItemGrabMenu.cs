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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Objects;

namespace ChestEx.Types.CustomTypes.ExtendedSVObjects {
   public class ExtendedChestInCustomItemGrabMenu : BaseTypes.ISVObjectInCustomItemGrabMenu {
      // Protected:
      #region Protected

      // Overrides:
      #region Overrides

      protected override void drawObject(SpriteBatch b) {
         this.MenuChest.Draw(b, this.Bounds, this.textureTintColourCurrent.A / 255.0f);
      }

      protected override void playObjectAnimation() {
         if (this.MenuChest.sv_currentLidFrame != this.MenuChest.getLastLidFrame())
            this.MenuChest.sv_currentLidFrame++;
      }

      protected override void revertObjectAnimation() {
         if (this.MenuChest.sv_currentLidFrame != this.MenuChest.startingLidFrame)
            this.MenuChest.sv_currentLidFrame--;
      }

      #endregion

      #endregion

      // Public:
      #region Public

      public ExtendedChest MenuChest {
         get => this.GetSVObjectAs<ExtendedChest>();
      }

      // Overrides:
      #region Overrides

      public override void OnMouseClick(BaseTypes.ICustomMenu.MouseStateEx mouseState) {
         if (mouseState.Button == StardewModdingAPI.SButton.MouseLeft)
            this.onMouseClickEventHandler?.Invoke(this, mouseState);
      }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      public ExtendedChestInCustomItemGrabMenu(BaseTypes.ICustomItemGrabMenuItem hostMenuItem,
                                               Rectangle bounds,
                                               String componentName = "",
                                               EventHandler<BaseTypes.ICustomMenu.MouseStateEx> onMouseClick = null,
                                               String hoverText = "",
                                               BaseTypes.Colours textureTintColours = null)
         : base(hostMenuItem, bounds, new ExtendedChest(bounds), false, -1.0f, componentName, onMouseClick, hoverText, textureTintColours) {
         // sync dummy chest
         this.MenuChest.playerChoiceColor.Value = this.HostMenuItem.HostMenu.GetSourceAs<Chest>().playerChoiceColor.Value;
      }

      #endregion
   }
}

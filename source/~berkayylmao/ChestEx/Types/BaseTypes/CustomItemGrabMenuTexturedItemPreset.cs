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

using Microsoft.Xna.Framework;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public class CustomItemGrabMenuTexturedItemPreset : CustomItemGrabMenuTexturedItem {
    // Public:  
  #region Public

    // Enums:
  #region Enums

    public enum ButtonType {
      FillStacks,
      Organize,
      OK,
      Cancel
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomItemGrabMenuTexturedItemPreset(CustomItemGrabMenu hostMenu, ButtonType buttonType, Rectangle bounds)
      : base(hostMenu,
             buttonType switch {
               ButtonType.FillStacks => TexturePresets.gFillStacksPickerButtonTexture,
               ButtonType.Organize   => TexturePresets.gOrganizeButtonTexture,
               ButtonType.OK         => TexturePresets.gOKButtonTexture,
               ButtonType.Cancel     => TexturePresets.gCancelButtonTexture,
               _                     => TexturePresets.gButtonBackgroundTexture
             },
             bounds,
             Colours.gDefault) {
      this.hoverText = buttonType switch {
        ButtonType.FillStacks => Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"),
        ButtonType.Organize   => Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"),
        _                     => String.Empty
      };
    }

    public CustomItemGrabMenuTexturedItemPreset(CustomItemGrabMenuItem hostMenuItem, ButtonType buttonType, Rectangle bounds)
      : this(hostMenuItem.mHostMenu, buttonType, bounds) { }

  #endregion
  }
}

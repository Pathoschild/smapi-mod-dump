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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.Types.BaseTypes {
  public class CustomItemGrabMenuTexturedItem : CustomMenuTexturedItem {
    // Public:
  #region Public

    // Shadowed:
  #region Shadowed

    public new CustomItemGrabMenu mHostMenu { get; private set; }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomItemGrabMenuTexturedItem(CustomItemGrabMenu hostMenu, Texture2D texture, Rectangle bounds, Colours colours) : base(null, texture, bounds, colours) {
      this.mHostMenu = hostMenu;
    }

    public CustomItemGrabMenuTexturedItem(CustomItemGrabMenu hostMenu, Texture2D texture, Rectangle bounds) : this(hostMenu, texture, bounds, Colours.gDefault) { }

  #endregion
  }
}

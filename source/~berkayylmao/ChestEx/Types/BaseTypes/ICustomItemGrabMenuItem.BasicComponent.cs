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

namespace ChestEx.Types.BaseTypes {
   public partial class ICustomItemGrabMenuItem {
      public new class BasicComponent : ICustomMenuItem.BasicComponent {
         // Public:
         #region Public

         // Shadowed:
         #region Shadowed

         public new ICustomItemGrabMenuItem HostMenuItem { get; private set; }

         #endregion

         #endregion

         // Constructors:
         #region Constructors

         public BasicComponent(ICustomItemGrabMenuItem hostMenuItem, Rectangle bounds, Boolean raiseMouseClickEventOnRelease = true, String componentName = "", EventHandler<ICustomMenu.MouseStateEx> onMouseClickHandler = null, String hoverText = "", Colours textureTintColours = null) : base(hostMenuItem, bounds, raiseMouseClickEventOnRelease, componentName, onMouseClickHandler, hoverText, textureTintColours) {
            this.HostMenuItem = hostMenuItem;
         }

         public BasicComponent(ICustomItemGrabMenuItem hostMenuItem, Point point, Boolean raiseMouseClickEventOnRelease = true, String componentName = "", EventHandler<ICustomMenu.MouseStateEx> onMouseClickHandler = null, String hoverText = "", Colours textureTintColours = null) : this(hostMenuItem, new Rectangle(point.X, point.Y, -1, -1), raiseMouseClickEventOnRelease, componentName, onMouseClickHandler, hoverText, textureTintColours) { }

         #endregion
      }
   }
}

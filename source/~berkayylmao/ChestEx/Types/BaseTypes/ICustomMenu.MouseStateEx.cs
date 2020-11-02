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

using Microsoft.Xna.Framework;

using StardewModdingAPI;

namespace ChestEx.Types.BaseTypes {
   public partial class ICustomMenu {
      public class MouseStateEx {
         // Public static instances:
         #region Public static instances

         public static readonly MouseStateEx Default = new MouseStateEx(SButton.None);

         #endregion

         // Public:
         #region Public
         public SButton Button { get; }

         public Vector2 Pos { get; set; }

         public SButtonState ButtonState { get; set; }

         // Overrides:
         #region Overrides

         public override Boolean Equals(Object obj) {
            if (obj is MouseStateEx other)
               return other.Button == this.Button && other.ButtonState == this.ButtonState && other.Pos.NearlyEquals(this.Pos);
            return false;
         }

         public override Int32 GetHashCode() {
            return base.GetHashCode();
         }

         #endregion

         #endregion

         // Constructors:
         #region Constructors

         public MouseStateEx(SButton button) {
            this.Button = button;
            this.Pos = Vector2.Zero;
            this.ButtonState = SButtonState.Released;
         }

         #endregion
      }
   }
}

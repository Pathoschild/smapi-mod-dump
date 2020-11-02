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

using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
   public partial class ICustomItemGrabMenu {
      public class MenuWithInventoryCtorParams {
         // Public:
         #region Public

         public InventoryMenu.highlightThisItem HighlighterMethod;
         public Boolean OKButton;
         public Boolean TrashCan;
         public Int32 InventoryXOffset;
         public Int32 InventoryYOffset;
         public Int32 MenuOffsetHack;

         #endregion

         // Constructors:
         #region Constructors

         public MenuWithInventoryCtorParams(InventoryMenu.highlightThisItem highlighterMethod = null,
                                            Boolean okButton = false,
                                            Boolean trashCan = false,
                                            Int32 inventoryXOffset = 0,
                                            Int32 inventoryYOffset = 0,
                                            Int32 menuOffsetHack = 0) {
            this.HighlighterMethod = highlighterMethod;
            this.OKButton = okButton;
            this.TrashCan = trashCan;
            this.InventoryXOffset = inventoryXOffset;
            this.InventoryYOffset = inventoryYOffset;
            this.MenuOffsetHack = menuOffsetHack;
         }

         #endregion
      }
   }
}

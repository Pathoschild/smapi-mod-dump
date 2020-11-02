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

using ChestEx.Types.BaseTypes;

namespace ChestEx.Types.CustomTypes.ChestExMenu.Items {
   public partial class ConfigPanel : BaseTypes.ICustomItemGrabMenuItem {
      // Private:
      #region Private

      // Component event handlers:
      #region Component event handlers

      private void _componentOnClick(Object sender, ICustomMenu.MouseStateEx mouseState) {
         switch ((sender as BaseTypes.ICustomItemGrabMenuItem.BasicComponent).Name) {
            case "configBTN":
               GlobalVars.SMAPIMonitor.Log("configbtn");
               break;
         }
      }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      public ConfigPanel(MainMenu hostMenu) : base(hostMenu, GlobalVars.GameViewport, true, BaseTypes.Colours.Default) {
      }

      #endregion

      // IDisposable:
      #region IDisposable

      public override void Dispose() {
         base.Dispose();
      }

      #endregion
   }
}

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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public partial class CustomItemGrabMenuItem : CustomMenuItem {
    // Public:
  #region Public

    // Shadowed:
  #region Shadowed

    public new CustomItemGrabMenu mHostMenu { get; private set; }

    public new List<BasicComponent> mComponents { get; protected set; }

  #endregion

    // Overrides:
  #region Overrides

    public override void SetVisible(Boolean isVisible) {
      base.SetVisible(isVisible);
      this.mComponents?.ForEach(c => c?.SetVisible(isVisible));
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomItemGrabMenuItem(CustomItemGrabMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours,
                                  Item               svItem) : base(null, bounds, raiseMouseClickEventOnRelease, colours, svItem) {
      this.mHostMenu   = hostMenu;
      this.mComponents = new List<BasicComponent>();
    }

    public CustomItemGrabMenuItem(CustomItemGrabMenu hostMenu,   Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours,
                                  String             svItemName, String    svItemLabel) : base(null, bounds, raiseMouseClickEventOnRelease, colours, svItemName, svItemLabel) {
      this.mHostMenu   = hostMenu;
      this.mComponents = new List<BasicComponent>();
    }

    public CustomItemGrabMenuItem(CustomItemGrabMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours) : this(hostMenu,
                                                                                                                                                bounds,
                                                                                                                                                raiseMouseClickEventOnRelease,
                                                                                                                                                colours,
                                                                                                                                                String.Empty,
                                                                                                                                                String.Empty) { }

    public CustomItemGrabMenuItem(CustomItemGrabMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease) : this(hostMenu,
                                                                                                                               bounds,
                                                                                                                               raiseMouseClickEventOnRelease,
                                                                                                                               Colours.gDefault) { }

    public CustomItemGrabMenuItem(CustomItemGrabMenu hostMenu, Rectangle bounds) : this(hostMenu, bounds, true) { }

  #endregion

    // IDisposable:
  #region IDisposable

    public override void Dispose() {
      base.Dispose();
      this.mComponents?.ForEach(c => c?.Dispose());
      this.mComponents?.Clear();
    }

  #endregion
  }
}

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
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

namespace ChestEx.Types.BaseTypes {
  public class InputStateEx : IEquatable<InputStateEx> {
    // Private:
  #region Private

    private        SButtonState buttonState = SButtonState.None;
    private static Vector2      sCursorPos  = Vector2.Zero;

  #endregion

    // Public:
  #region Public

    /// <summary>Button</summary>
    public SButton mButton { get; }

    /// <summary>Button state</summary>
    public SButtonState mButtonState {
      get => this.buttonState;
      set {
        if (this.buttonState == value) return;

        this.mLastButtonState = this.mButtonState;
        this.buttonState      = value;
      }
    }

    /// <summary>Last button state</summary>
    public SButtonState mLastButtonState { get; private set; }

    /// <summary>Current position of the cursor (adjusted to zoom and UI scale)</summary>
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    public Vector2 mCursorPos => gCursorPos;

    /// <summary>Current position of the cursor (adjusted to zoom and UI scale)</summary>
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    public Vector2 mLastCursorPos => gLastCursorPos;

    /// <summary>Current position of the cursor (adjusted to zoom and UI scale)</summary>
    public static Vector2 gCursorPos {
      get => sCursorPos;
      set {
        if (sCursorPos == value) return;

        gLastCursorPos = sCursorPos;
        sCursorPos     = value;
      }
    }

    /// <summary>Last position of the cursor (adjusted to zoom and UI scale)</summary>
    public static Vector2 gLastCursorPos { get; set; }

    // Overrides:
  #region Overrides

    public Boolean Equals(InputStateEx other) {
      return other is not null && other.mButton == this.mButton && other.mButtonState == this.mButtonState && other.mLastButtonState == this.mLastButtonState;
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public InputStateEx(SButton button) {
      this.mButton      = button;
      this.mButtonState = SButtonState.None;
    }

  #endregion
  }
}

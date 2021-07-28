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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.Types.BaseTypes {
  public interface ICustomComponent : IDisposable {
    public interface IData {
      /// <summary>Whether the component is active.</summary>
      public Boolean mIsActive { get; }

      /// <summary>Whether the cursor is currently hovering the component.</summary>
      public Boolean mIsCursorHovering { get; }

      /// <summary>Whether the component is enabled.</summary>
      public Boolean mIsEnabled { get; }

      /// <summary>Hover text of the component.</summary>
      public String mHoverText { get; set; }

      /// <summary>Base <see cref="SpriteBatch"/> layer depth of the component.</summary>
      public Single mLayerDepth { get; }

      /// <summary>Colours of the component.</summary>
      public Colours mColours { get; set; }

      /// <summary>Adjusted background colour.</summary>
      public Color mBGColour { get; }

      /// <summary>Adjusted foreground colour.</summary>
      public Color mFGColour { get; }

      /// <summary>Called to change <see cref="mIsEnabled"/>.</summary>
      /// <param name="isEnabled">Whether this component should be enabled.</param>
      public void SetEnabled(Boolean isEnabled);

      /// <summary>Called when the component should update its cursor status.</summary>
      /// <param name="isCursorInBounds">Whether the cursor is on the component.</param>
      /// <param name="inputState">New input state.</param>
      public void UpdateCursorStatus(Boolean isCursorInBounds, InputStateEx inputState = null);
    }

    /// <summary>Bounds of the component.</summary>
    public Rectangle mBounds { get; }

    /// <summary>Internal data of the component.</summary>
    public IData mData { get; }

    /// <summary>Whether the component is visible.</summary>
    public Boolean mIsVisible { get; }

    /// <summary>Called to change <see cref="IData.mIsEnabled"/>.</summary>
    /// <param name="isEnabled">Whether this component should be enabled.</param>
    public void SetEnabled(Boolean isEnabled);

    /// <summary>Called to change <see cref="mIsVisible"/>.</summary>
    /// <param name="isVisible">Whether this component should be visible.</param>
    public void SetVisible(Boolean isVisible);

    /// <summary>Called to draw the component (if visible).</summary>
    public void Draw(SpriteBatch spriteBatch);

    /// <summary>Called when the user has pressed a button (if visible and enabled).</summary>
    public void OnButtonPressed(InputStateEx inputState);

    /// <summary>Called when the user has released a button (if visible and enabled).</summary>
    public void OnButtonReleased(InputStateEx inputState);

    /// <summary>Called when the user has moved the cursor (if visible and enabled).</summary>
    public void OnCursorMoved(Vector2 cursorPos);

    /// <summary>Called when the user has clicked on the component (if visible and enabled).</summary>
    public void OnMouseClick(InputStateEx inputState);

    /// <summary>Called every game tick (if visible and enabled).</summary>
    public void OnGameTick();

    /// <summary>Called when the component should adjust to the new window size</summary>
    public void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds);
  }
}

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
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public partial class CustomMenuItem : ClickableComponent,
                                        IDisposable {
    // Public:
  #region Public

    public Colours mColours { get; set; }

    public Rectangle mBounds {
      get => this.bounds;
      set => this.bounds = value;
    }

    public List<BasicComponent> mComponents { get; protected set; }

    public CustomMenu mHostMenu { get; private set; }

    public Boolean mIsVisible { get; protected set; }

    public Boolean mRaiseMouseClickEventOnRelease { get; protected set; }

    // Virtuals:
  #region Virtuals

    /// <summary>
    /// Base implementation sets '<see cref="mIsVisible"/>' and this item's components' visibility to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this item should be visible.</param>
    public virtual void SetVisible(Boolean isVisible) {
      this.mIsVisible = this.visible = isVisible;
      this.mComponents?.ForEach(c => c?.SetVisible(isVisible));
    }

    /// <summary>
    /// Base implementation draws '<see cref="mComponents"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    public virtual void Draw(SpriteBatch b) {
      if (!this.mIsVisible) return;

      this.mComponents.ForEach(c => {
        if (c.mIsVisible) c.Draw(b);
      });
    }

    /// <summary>Base implementation informs this item's components.</summary>
    public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) { this.mComponents.ForEach(c => c.OnGameWindowSizeChanged(oldBounds, newBounds)); }

    /// <summary>Base implementation does nothing.</summary>
    public virtual void OnMouseClick(CustomMenu.MouseStateEx mouseState) { }

    /// <summary>Base implementation does nothing.</summary>
    public virtual void OnCursorMoved(Vector2 cursorPos) { }

    /// <summary>Base implementation does nothing.</summary>
    public virtual void OnButtonPressed(ButtonPressedEventArgs e) { }

    /// <summary>Base implementation does nothing.</summary>
    public virtual void OnButtonReleased(ButtonReleasedEventArgs e) { }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomMenuItem(CustomMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours,
                          Item       svItem)
      : base(bounds, svItem) {
      this.mColours                       = colours;
      this.mComponents                    = new List<BasicComponent>();
      this.mRaiseMouseClickEventOnRelease = raiseMouseClickEventOnRelease;
      this.mHostMenu                      = hostMenu;
      this.SetVisible(true);
    }

    public CustomMenuItem(CustomMenu hostMenu,   Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours,
                          String     svItemName, String    svItemLabel)
      : base(bounds, svItemName, svItemLabel) {
      this.mColours                       = colours;
      this.mComponents                    = new List<BasicComponent>();
      this.mRaiseMouseClickEventOnRelease = raiseMouseClickEventOnRelease;
      this.mHostMenu                      = hostMenu;
    }

    public CustomMenuItem(CustomMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease, Colours colours)
      : this(hostMenu, bounds, raiseMouseClickEventOnRelease, colours, String.Empty, String.Empty) { }

    public CustomMenuItem(CustomMenu hostMenu, Rectangle bounds, Boolean raiseMouseClickEventOnRelease)
      : this(hostMenu, bounds, raiseMouseClickEventOnRelease, Colours.gDefault) { }

    public CustomMenuItem(CustomMenu hostMenu, Rectangle bounds)
      : this(hostMenu, bounds, true) { }

  #endregion

    // IDisposable:
  #region IDisposable

    /// <summary>
    /// <para>Base implementation:</para>
    /// <para>1. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
    /// <para>2. Disposes of this item's components.</para>
    /// </summary>
    public virtual void Dispose() {
      // hide
      this.SetVisible(false);
      // dispose of components
      this.mComponents?.ForEach(c => c?.Dispose());
      this.mComponents?.Clear();
    }

  #endregion
  }
}

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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public class CustomClickableComponent : ClickableComponent,
                                          ICustomComponent {
    // Private:
  #region Private

    private Single baseScale { get; } = 1.0f;

  #endregion

    // Public:
  #region Public

    public Rectangle              mBounds    => this.bounds;
    public ICustomComponent.IData mData      { get; }
    public Boolean                mIsVisible { get; private set; } = true;

    // Virtuals:
  #region Virtuals

    /// <inheritdoc/>
    public virtual void SetEnabled(Boolean isEnabled) { this.mData.SetEnabled(isEnabled); }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableComponent"/> implementation also updates <see cref="ClickableComponent.visible"/>.
    /// </remarks>
    public virtual void SetVisible(Boolean isVisible) { this.mIsVisible = this.visible = isVisible; }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableComponent"/> implementation draws <see cref="ICustomComponent.IData.mHoverText"/> with <see cref="ICustomComponent.IData.mColours"/> colouring.
    /// </remarks>
    public virtual void Draw(SpriteBatch spriteBatch) {
      if (this.mData.mIsCursorHovering) spriteBatch.DrawHoverText(Game1.smallFont, this.mData.mHoverText, colours: this.mData.mColours, borderScale: 0.5f);
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableComponent"/> implementation updates <see cref="ClickableComponent.scale"/>.
    /// </remarks>
    public virtual void OnGameTick() {
      this.scale = this.mBounds.Contains(InputStateEx.gCursorPos) ? Math.Min(this.scale + 0.01f, this.baseScale + 0.1f) : Math.Max(this.scale - 0.01f, this.baseScale);
    }

    public virtual void OnButtonPressed(InputStateEx      inputState)                     { }
    public virtual void OnButtonReleased(InputStateEx     inputState)                     { }
    public virtual void OnCursorMoved(Vector2             cursorPos)                      { }
    public virtual void OnMouseClick(InputStateEx         inputState)                     { }
    public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) { }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomClickableComponent(Rectangle bounds, Colours colours, String hoverText)
      : base(bounds, String.Empty, String.Empty) {
      this.mData = new CustomComponentData(colours, 0.86f + bounds.Y / 20000.0f, hoverText);
    }

    public CustomClickableComponent(Rectangle bounds, Colours colours)
      : this(bounds, colours, String.Empty) { }

    public CustomClickableComponent(Rectangle bounds)
      : this(bounds, Colours.gDefault) { }

  #endregion

    // IDisposable:
  #region IDisposable

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableComponent"/> implementation:
    /// <para>1. Calls <see cref="ICustomComponent.IData.SetEnabled(Boolean)"/> with 'false'.</para>
    /// <para>2. Calls <see cref="SetVisible(Boolean)"/> with 'false'.</para>
    /// </remarks>
    public virtual void Dispose() {
      this.SetEnabled(false);
      this.SetVisible(false);
    }

  #endregion
  }
}

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
//    Copyright (c) 2022 Berkay Yigit <berkaytgy@gmail.com>
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

using ChestEx.LanguageExtensions;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public class CustomClickableTextureComponent : ClickableTextureComponent,
                                                 ICustomComponent {
    // Public:
    #region Public

    public Rectangle mBounds => this.bounds;
    public ICustomComponent.IData mData { get; }
    public Boolean mIsVisible { get; private set; } = true;

    // Virtuals:
    #region

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    public virtual void SetEnabled(Boolean isEnabled) { this.mData.SetEnabled(isEnabled); }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableTextureComponent"/> implementation also updates <see cref="ClickableComponent.visible"/>.
    /// </remarks>
    public virtual void SetVisible(Boolean isVisible) { this.mIsVisible = this.visible = isVisible; }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <para><see cref="CustomClickableTextureComponent"/> implementation:</para>
    /// <para>1. Draws <see cref="ClickableTextureComponent.texture"/>.</para>
    /// <para>2. Draws <see cref="ICustomComponent.IData.mHoverText"/>.</para>
    /// </remarks>
    public virtual void Draw(SpriteBatch spriteBatch) {
      if (this.texture is not null) {
        spriteBatch.Draw(this.texture,
                         new Vector2(this.mBounds.X + this.mBounds.Width / 2.0f, this.mBounds.Y + this.mBounds.Height / 2.0f),
                         this.texture.Bounds,
                         Color.White,
                         0.0f,
                         this.texture.Bounds.Center.AsXNAVector2(),
                         this.scale,
                         SpriteEffects.None,
                         this.mData.mLayerDepth);
      }

      if (this.mData.mIsCursorHovering) spriteBatch.DrawHoverText(Game1.smallFont, this.mData.mHoverText, colours: this.mData.mColours, borderScale: 0.5f);
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableTextureComponent"/> implementation updates <see cref="ClickableTextureComponent.scale"/>.
    /// </remarks>
    public virtual void OnGameTick() {
      this.scale = this.mBounds.Contains(InputStateEx.gCursorPos) ?
        Math.Min(this.scale + this.baseScale * 0.01f, this.baseScale + this.baseScale * 0.1f) :
        Math.Max(this.scale - this.baseScale * 0.01f, this.baseScale);
    }

    public virtual void OnButtonPressed(InputStateEx inputState) { }
    public virtual void OnButtonReleased(InputStateEx inputState) { }
    public virtual void OnCursorMoved(Vector2 cursorPos) { }
    public virtual void OnMouseClick(InputStateEx inputState) { }
    public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) { }

    #endregion

    // Shadowed:
    #region Shadowed

    /// <summary>Calls <see cref="Draw(SpriteBatch)"/>.</summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Game code")]
    public new void draw(SpriteBatch b, Color c, Single layerDepth, Int32 frameOffset = 0) { this.Draw(b); }

    /// <summary>Calls <see cref="Draw(SpriteBatch)"/>.</summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Game code")]
    public new void draw(SpriteBatch b) { this.Draw(b); }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    public CustomClickableTextureComponent(Rectangle bounds, Colours colours, Texture2D texture, String hoverText)
      : base(bounds, texture, texture?.Bounds ?? Rectangle.Empty, texture is null ? 1.0f : Convert.ToSingle(bounds.Width / texture.Bounds.Width)) {
      this.mData = new CustomComponentData(colours, 0.86f + bounds.Y / 20000.0f, hoverText);
    }

    public CustomClickableTextureComponent(Rectangle bounds, Colours colours, Texture2D texture)
      : this(bounds, colours, texture, String.Empty) { }

    public CustomClickableTextureComponent(Rectangle bounds, Colours colours)
      : this(bounds, colours, null, String.Empty) { }

    public CustomClickableTextureComponent(Rectangle bounds)
      : this(bounds, Colours.gDefault) { }

    #endregion

    // IDisposable:
    #region IDisposable

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomClickableComponent"/> implementation:
    /// <para>1. Calls <see cref="ICustomComponent.IData.SetEnabled(Boolean)"/> with 'false'.</para>
    /// <para>2. Calls <see cref="SetVisible(Boolean)"/> with 'false'.</para>
    /// <para>3. Disposes of <see cref="ClickableTextureComponent.texture"/>.</para>
    /// </remarks>
    public virtual void Dispose() {
      this.SetEnabled(false);
      this.SetVisible(false);

      this.texture?.Dispose();
    }

    #endregion
  }
}

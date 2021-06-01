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
using System.Diagnostics.CodeAnalysis;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public class CustomMenuTexturedItem : ClickableTextureComponent,
                                        IDisposable {
    // Public:
  #region Public

    public Colours mColours { get; set; }

    public Rectangle mBounds {
      get => this.bounds;
      set => this.bounds = value;
    }

    public CustomMenu mHostMenu { get; private set; }

    public Boolean mIsVisible { get; private set; }

    // Virtuals:
  #region Virtuals

    /// <summary>
    /// Base implementation sets '<see cref="mIsVisible"/>' to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this item should be visible.</param>
    public virtual void SetVisible(Boolean isVisible) { this.mIsVisible = this.visible = isVisible; }

  #endregion

    // Shadowed:
  #region Shadowed

    /// <summary>
    /// Draws '<see cref="ClickableTextureComponent.texture"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public new void draw(SpriteBatch b, Color c, Single layerDepth, Int32 frameOffset = 0) {
      if (!this.mIsVisible || this.texture is null) return;

      b.Draw(this.texture,
             this.mBounds.ExtractXYAsXNAVector2(),
             this.texture.Bounds,
             c,
             0.0f,
             Vector2.Zero,
             this.scale,
             SpriteEffects.None,
             layerDepth);
    }

    /// <summary>
    /// Calls '<see cref="draw(SpriteBatch, Color, float, int)"/>' if '<see cref="mIsVisible"/>' is true.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public new void draw(SpriteBatch b) {
      if (this.mIsVisible) this.draw(b, Color.White, 0.86f + this.mBounds.Y / 20000.0f);
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomMenuTexturedItem(CustomMenu hostMenu, Texture2D texture, Rectangle bounds, Colours colours)
      : base(bounds, texture, texture.Bounds, Convert.ToSingle(bounds.Width) / texture.Bounds.Width) {
      this.mColours  = colours;
      this.mHostMenu = hostMenu;
    }

    public CustomMenuTexturedItem(CustomMenu hostMenu, Texture2D texture, Rectangle bounds)
      : this(hostMenu, texture, bounds, Colours.gDefault) { }

  #endregion

    // IDisposable:
  #region IDisposable

    /// <summary>
    /// <para>Base implementation:</para>
    /// <para>1. Calls '<see cref="SetVisible(Boolean)"/>' with 'false'.</para>
    /// <para>2. Disposes of <see cref="ClickableTextureComponent.texture"/>.</para>
    /// </summary>
    public virtual void Dispose() {
      // hide
      this.SetVisible(false);
      // dispose of texture
      this.texture?.Dispose();
    }

  #endregion
  }
}

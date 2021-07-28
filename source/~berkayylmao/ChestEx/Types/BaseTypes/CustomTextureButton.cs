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

namespace ChestEx.Types.BaseTypes {
  internal sealed class CustomTextureButton : CustomButton {
    // Private:
  #region Private

    private readonly Texture2D texture;

  #endregion

    // Public:
  #region Public

    // Overrides:
  #region Overrides

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomTextureButton"/> implementation:
    /// <para>1. Draws <see cref="texture"/> with <see cref="ICustomComponent.IData.mBGColour"/> colouring..</para>
    /// <para>2. Draws <see cref="ICustomComponent.IData.mHoverText"/> with <see cref="ICustomComponent.IData.mColours"/> colouring.</para>
    /// </remarks>
    public override void Draw(SpriteBatch spriteBatch) {
      spriteBatch.Draw(this.texture, this.mBounds.Scale(this.scale), this.texture.Bounds, this.mData.mBGColour);
      if (this.mData.mIsCursorHovering) spriteBatch.DrawHoverText(Game1.smallFont, this.mData.mHoverText, colours: this.mData.mColours, borderScale: 0.5f);
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomTextureButton(Rectangle bounds, Colours colours, Texture2D texture, String hoverText,
                               Action    onClickHandler)
      : base(bounds, colours, "", hoverText, onClickHandler) {
      this.texture = texture;
    }

  #endregion

    // IDisposable:
  #region IDisposable

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomTextureButton"/> implementation:
    /// <para>1. Calss <see cref="CustomClickableComponent.Dispose()"/>.</para>
    /// <para>2. Disposes of <see cref="texture"/>.</para>
    /// </remarks>
    public override void Dispose() {
      base.Dispose();

      this.texture?.Dispose();
    }

  #endregion
  }
}

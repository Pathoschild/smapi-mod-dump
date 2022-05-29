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
using SkiaSharp;
using System.IO;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ChestEx.Types.BaseTypes {
  public partial class ColouringHSVMenu {
    private class HueSlider : CustomClickableTextureComponent {
      // Consts:
      #region Consts

      private const Int32 CONST_SELECTOR_HEIGHT = 4;

      #endregion

      // Private:
      #region Private

      private readonly Action<Color> onColourHovered;
      private readonly Action<Color> onColourChanged;

      private SKBitmap bitmap;
      private Int32 selectorY;
      private Int32 selectorActiveY;
      private Color selectorColour = Color.Red;

      private Texture2D getOrCreateTexture(GraphicsDevice device) {
        if (this.bitmap is null || this.texture is null) {
          this.bitmap?.Dispose();
          this.texture?.Dispose();

          this.bitmap = new SKBitmap(this.mBounds.Width, this.mBounds.Height);

          using var ms = new MemoryStream();
          using var canvas = new SKCanvas(this.bitmap);
          using var paint = new SKPaint() {
            Shader = SKShader.CreateLinearGradient(new SKPoint(0.0f, 0.0f), new SKPoint(this.mBounds.Width, this.mBounds.Height),
            new SKColor[] { SKColors.Red, SKColors.Yellow, SKColors.Lime, SKColors.Cyan, SKColors.Blue, SKColors.Magenta, SKColors.Red }, new Single[] {0f,
                  1 / 6f,
                  2 / 6f,
                  3 / 6f,
                  4 / 6f,
                  5 / 6f,
                  1f }, SKShaderTileMode.Decal)
          };

          canvas.DrawPaint(paint);
          canvas.RotateDegrees(90.0f);

          this.bitmap.Encode(ms, SKEncodedImageFormat.Png, 95);
          this.texture = Texture2D.FromStream(device, ms);
        }

        return this.texture;
      }

      private Color getColourAt(Int32 y) { return this.bitmap.GetPixel(0, Math.Max(0, Math.Min(y, this.mBounds.Height - 1))).AsXNAColor(); }

      private void setSelectorY(Int32 y) {
        if (this.bitmap is null) return;
        if (this.selectorY == y) return;

        this.selectorY = y;
        this.selectorColour = this.getColourAt(y);
        this.onColourHovered(this.selectorColour);
      }

      #endregion

      // Public:
      #region Public

      public void Reset() { this.SetSelectorActiveY(0); }

      public void SetSelectorActiveY(Int32 y, Boolean fireEvents = true) {
        this.selectorY = this.selectorActiveY = y;
        if (this.bitmap is null) {
          if (Game1.graphics.GraphicsDevice is null) return;
          this.getOrCreateTexture(Game1.graphics.GraphicsDevice);
        }

        this.selectorColour = this.getColourAt(y);
        if (fireEvents) this.onColourChanged(this.selectorColour);
      }

      // Overrides:
      #region Overrides

      public override void Draw(SpriteBatch spriteBatch) {
        this.texture = this.getOrCreateTexture(spriteBatch.GraphicsDevice);
        base.Draw(spriteBatch);

        // Draw the selector
        spriteBatch.Draw(Game1.uncoloredMenuTexture,
                         new Rectangle(this.mBounds.X, this.mBounds.Y + this.selectorY, this.mBounds.Width, CONST_SELECTOR_HEIGHT),
                         new Rectangle(64, 412, 16, 4),
                         Color.White);
        spriteBatch.Draw(Game1.uncoloredMenuTexture,
                         new Rectangle(this.mBounds.X + 12, this.mBounds.Y + this.selectorY - CONST_SELECTOR_HEIGHT - 4, 20, 12),
                         new Rectangle(152, 712, 20, 12),
                         Color.White,
                         MathHelper.PiOver2,
                         Vector2.Zero,
                         SpriteEffects.FlipVertically,
                         this.mBounds.Y / 10000.0f);
      }

      public override void OnCursorMoved(Vector2 cursorPosition) {
        if (this.mBounds.Contains(cursorPosition.AsXNAPoint()))
          this.setSelectorY((Int32)cursorPosition.Y - this.mBounds.Y);
        else
          this.setSelectorY(this.selectorActiveY);
      }

      public override void OnMouseClick(InputStateEx inputState) {
        if (inputState.mButton != SButton.MouseLeft) return;
        this.SetSelectorActiveY((Int32)inputState.mCursorPos.Y - this.mBounds.Y);
      }

      // Disable cursor hover scaling
      public override void OnGameTick() { }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      public HueSlider(Rectangle bounds, Action<Color> onColourHovered, Action<Color> onColourChanged)
        : base(bounds) {
        this.onColourHovered = onColourHovered;
        this.onColourChanged = onColourChanged;
      }

      #endregion

      // IDisposable:
      #region IDisposable

      public override void Dispose() {
        base.Dispose();

        this.bitmap?.Dispose();
        this.bitmap = null;
        this.texture = null;
      }

      #endregion
    }
  }
}

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
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ChestEx.Types.BaseTypes {
  public partial class ColouringHSVMenu {
    public class ColourPicker : CustomClickableTextureComponent {
      // Consts:
      #region Consts

      private const Int32 CONST_SELECTOR_SIZE = 12;

      #endregion

      // Private:
      #region Private

      private readonly Action<Color> onColourHovered;
      private readonly Action<Color> onColourChanged;

      private SKBitmap bitmap;
      private Boolean selectorIsVisible = true;
      private Point selectorPos = Point.Zero;
      private Point selectorActivePos = Point.Zero;
      private Color hueColour = Color.Red;

      private Texture2D getOrCreateTexture(GraphicsDevice device) {
        if (this.bitmap is null || this.texture is null) {
          this.bitmap?.Dispose();
          this.texture?.Dispose();

          this.bitmap = new SKBitmap(this.mBounds.Width, this.mBounds.Height);

          using var ms = new MemoryStream();
          using var canvas = new SKCanvas(this.bitmap);
          using var white_skpaint = new SKPaint() { Shader = SKShader.CreateLinearGradient(new SKPoint(0.0f, 0.0f), new SKPoint(this.mBounds.Width, 0.0f), new SKColor[] { SKColors.Transparent, SKColors.White }, SKShaderTileMode.Decal) };
          using var black_skpaint = new SKPaint() { Shader = SKShader.CreateLinearGradient(new SKPoint(0.0f, 0.0f), new SKPoint(this.mBounds.Height, 0.0f), new SKColor[] { SKColors.Transparent, new SKColor(1, 1, 1, 255) }, SKShaderTileMode.Decal) };
          using var colour_skpaint = new SKPaint() { Shader = SKShader.CreateColor(this.hueColour.AsSKColor()) };

          canvas.DrawPaint(colour_skpaint);
          canvas.DrawPaint(white_skpaint);
          canvas.RotateDegrees(90.0f);
          canvas.DrawPaint(black_skpaint);

          this.bitmap.Encode(ms, SKEncodedImageFormat.Png, 95);
          this.texture = Texture2D.FromStream(device, ms);
        }

        return this.texture;
      }

      private Color getColourAt(Point position) {
        return this.bitmap.GetPixel(Math.Max(0, Math.Min(position.X, this.mBounds.Width - 1)), Math.Max(0, Math.Min(position.Y, this.mBounds.Height - 1))).AsXNAColor();
      }

      private void setSelectorPos(Point position) {
        if (this.bitmap is null) return;

        this.selectorPos = new Point(Math.Min(this.mBounds.Width - CONST_SELECTOR_SIZE, Math.Max(0, position.X - 4)),
                                     Math.Min(this.mBounds.Height - CONST_SELECTOR_SIZE, Math.Max(0, position.Y - 4)));
        this.onColourHovered?.Invoke(this.getColourAt(position));
      }

      #endregion

      // Public:
      #region Public

      public void SetHueColour(Color colour) {
        if (this.hueColour == colour) return;
        this.hueColour = colour;

        this.bitmap?.Dispose();
        this.texture?.Dispose();
        this.bitmap = null;
        this.texture = null;
      }

      public void Reset() { this.selectorActivePos = this.selectorPos = Point.Zero; }

      public void SetSelectorVisible(Boolean isVisible) { this.selectorIsVisible = isVisible; }

      public void SetSelectorActivePos(Point position, Boolean fireEvents = true) {
        this.selectorActivePos = this.selectorPos = new Point(Math.Min(this.mBounds.Width - CONST_SELECTOR_SIZE, Math.Max(0, position.X - 4)),
                                                              Math.Min(this.mBounds.Height - CONST_SELECTOR_SIZE, Math.Max(0, position.Y - 4)));
        if (this.bitmap is null) {
          if (Game1.graphics.GraphicsDevice is null) return;
          this.getOrCreateTexture(Game1.graphics.GraphicsDevice);
        }

        Color colour = this.getColourAt(position);
        if (colour != Color.Black && fireEvents) this.onColourChanged?.Invoke(colour);
      }

      // Overrides:
      #region Overrides

      public override void Draw(SpriteBatch spriteBatch) {
        this.texture = this.getOrCreateTexture(spriteBatch.GraphicsDevice);
        base.Draw(spriteBatch);

        // Draw the selector
        if (this.selectorIsVisible) {
          spriteBatch.Draw(TexturePresets.gCursorsGrayScale,
                           new Rectangle(this.mBounds.X + this.selectorPos.X, this.mBounds.Y + this.selectorPos.Y, CONST_SELECTOR_SIZE, CONST_SELECTOR_SIZE),
                           new Rectangle(205, 1888, 12, 12),
                           this.bitmap.GetPixel(this.selectorPos.X, this.selectorPos.Y).ContrastColour().AsXNAColor());
        }
      }

      public override void OnCursorMoved(Vector2 cursorPosition) {
        if (this.mBounds.Contains(cursorPosition.AsXNAPoint()))
          this.setSelectorPos((cursorPosition - this.mBounds.ExtractXYAsXNAVector2()).AsXNAPoint());
        else
          this.selectorPos = this.selectorActivePos;
      }

      public override void OnMouseClick(InputStateEx inputState) {
        if (inputState.mButton != SButton.MouseLeft) return;
        this.SetSelectorActivePos((inputState.mCursorPos - this.mBounds.ExtractXYAsXNAVector2()).AsXNAPoint());
      }

      // Disable cursor hover scaling
      public override void OnGameTick() { }

      #endregion

      #endregion

      // Constructors:
      #region Constructors

      public ColourPicker(Rectangle bounds, Colours colours, Action<Color> onColourHovered, Action<Color> onColourChanged)
        : base(bounds, colours) {
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

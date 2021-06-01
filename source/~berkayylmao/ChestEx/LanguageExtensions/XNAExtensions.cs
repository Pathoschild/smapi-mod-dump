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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.LanguageExtensions {
  public static class XNAExtensions {
    /// <summary>
    /// Constructs and returns a <see cref="Point"/> from the given <see cref="Vector2"/>.
    /// </summary>
    /// <param name="vector2">Vector2 to convert.</param>
    /// <returns>Converted version of '<paramref name="vector2"/>'.</returns>
    public static Point AsXNAPoint(this Vector2 vector2) { return new(Convert.ToInt32(vector2.X), Convert.ToInt32(vector2.Y)); }

    /// <summary>
    /// Constructs and returns a <see cref="Vector2"/> from the given <see cref="Point"/>.
    /// </summary>
    /// <param name="point">Point to convert.</param>
    /// <returns>Converted version of '<paramref name="point"/>'.</returns>
    public static Vector2 AsXNAVector2(this Point point) { return new(Convert.ToSingle(point.X), Convert.ToSingle(point.Y)); }

    public static Point ExtractXYAsXNAPoint(this Rectangle rectangle) { return new(rectangle.X, rectangle.Y); }

    public static Vector2 ExtractXYAsXNAVector2(this Rectangle rectangle) { return new(Convert.ToSingle(rectangle.X), Convert.ToSingle(rectangle.Y)); }

    public static System.Drawing.Color AsDotNetColor(this Color colour) { return System.Drawing.Color.FromArgb(colour.A, colour.R, colour.G, colour.B); }

    public static String AsHexCode(this Color colour) { return $"{colour.R:X2}{colour.G:X2}{colour.B:X2}"; }

    public static Color AsXNAColor(this String hexCode) {
      return Color.FromNonPremultiplied(Int32.Parse(hexCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                                        Int32.Parse(hexCode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                                        Int32.Parse(hexCode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                                        255);
    }

    // https://stackoverflow.com/a/1855903
    public static Color ContrastColour(this Color colour) { return (0.299 * colour.R + 0.587 * colour.G + 0.114 * colour.B) / 255 > 0.5 ? Color.Black : Color.White; }

    public static Color MultKeepAlpha(this Color colour, Single multiplier) {
      return Color.FromNonPremultiplied((Int32)(colour.R * multiplier), (Int32)(colour.G * multiplier), (Int32)(colour.B * multiplier), colour.A);
    }

    public static Texture2D ToGrayScale(this Texture2D texture, GraphicsDevice device) {
      // get original pixels
      var orig_colours = new Color[texture.Width * texture.Height];
      texture.GetData(orig_colours);
      // prep grayscale texture
      var new_colors  = new Color[texture.Width * texture.Height];
      var new_texture = new Texture2D(device, texture.Width, texture.Height);

      for (Int32 i = 0; i < texture.Width; i++) {
        for (Int32 j = 0; j < texture.Height; j++) {
          Int32 index          = i + j * texture.Width;
          Color original_color = orig_colours[index];
          Single gray_scale = (original_color.R / 255.0f * 0.30f + original_color.G / 255.0f * 0.59f + original_color.B / 255.0f * 0.11f + original_color.A / 255.0f * 0.79f)
                              / 1.79f;
          new_colors[index] = new Color(gray_scale, gray_scale, gray_scale, original_color.A / 255.0f);
        }
      }

      new_texture.SetData(new_colors);
      return new_texture;
    }

    public static void DrawStringEx(this SpriteBatch spriteBatch,               SpriteFont font,                       String text, Point position,
                                    Color            textColour,                Single     textAlpha        = 1.0f,    Single layerDepth      = -1.0f, Boolean drawShadow = false,
                                    Single           shadowDistance     = 1.0f, Color      textShadowColour = default, Single textShadowAlpha = 1.0f, Boolean drawUnderline = false,
                                    Single           textUnderlineAlpha = 0.5f) {
      Vector2 pos                                    = position.AsXNAVector2();
      if (layerDepth.NearlyEquals(-1.0f)) layerDepth = pos.Y / 10000f;

      if (drawUnderline) {
        Color underline_colour = Color.FromNonPremultiplied(textColour.R, textColour.G, textColour.B, Convert.ToInt32(textColour.A * textUnderlineAlpha));

        Vector2 size            = font.MeasureString(text);
        Vector2 underscore_size = font.MeasureString("_");
        Single  x               = 0.0f;

        while (x < size.X) {
          spriteBatch.DrawString(font,
                                 "_",
                                 pos + new Vector2(x, 0.0f),
                                 underline_colour,
                                 0f,
                                 Vector2.Zero,
                                 1.0f,
                                 SpriteEffects.None,
                                 layerDepth - 0.0005f);
          x += underscore_size.X - 4f;
        }
      }

      if (drawShadow) {
        if (textShadowColour == default) textShadowColour = Color.Multiply(textColour, 0.5f);
        Color shadow_colour = Color.FromNonPremultiplied(textShadowColour.R, textShadowColour.G, textShadowColour.B, Convert.ToInt32(textShadowColour.A * textShadowAlpha));
        spriteBatch.DrawString(font,
                               text,
                               pos + new Vector2(shadowDistance),
                               shadow_colour,
                               0f,
                               Vector2.Zero,
                               1.0f,
                               SpriteEffects.None,
                               layerDepth - 0.0001f);
        spriteBatch.DrawString(font,
                               text,
                               pos + new Vector2(0.0f, shadowDistance),
                               shadow_colour,
                               0f,
                               Vector2.Zero,
                               1.0f,
                               SpriteEffects.None,
                               layerDepth - 0.0002f);
        spriteBatch.DrawString(font,
                               text,
                               pos + new Vector2(shadowDistance, 0.0f),
                               shadow_colour,
                               0f,
                               Vector2.Zero,
                               1.0f,
                               SpriteEffects.None,
                               layerDepth - 0.0003f);
      }

      spriteBatch.DrawString(font,
                             text,
                             pos,
                             Color.FromNonPremultiplied(textColour.R, textColour.G, textColour.B, Convert.ToInt32(textColour.A * textAlpha)),
                             0f,
                             Vector2.Zero,
                             1.0f,
                             SpriteEffects.None,
                             layerDepth);
    }

    public static void DrawStringEx(this SpriteBatch spriteBatch,               SpriteFont font,                       StringBuilder text, Point position,
                                    Color            textColour,                Single     textAlpha        = 1.0f,    Single layerDepth = -1.0f, Boolean drawShadow = false,
                                    Single           shadowDistance     = 1.0f, Color      textShadowColour = default, Single textShadowAlpha = 1.0f, Boolean drawUnderline = false,
                                    Single           textUnderlineAlpha = 0.5f) {
      DrawStringEx(spriteBatch,
                   font,
                   text.ToString(),
                   position,
                   textColour,
                   textAlpha,
                   layerDepth,
                   drawShadow,
                   shadowDistance,
                   textShadowColour,
                   textShadowAlpha,
                   drawUnderline,
                   textUnderlineAlpha);
    }
  }
}

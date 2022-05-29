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
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkiaSharp;

namespace ChestEx.LanguageExtensions {
  public static class XNAExtensions {
    public static Boolean NearlyEquals(this Vector2 lhs, Vector2 rhs) { return lhs.X.NearlyEquals(rhs.X) && lhs.Y.NearlyEquals(rhs.Y); }

    public static Boolean Contains(this Rectangle rect, Vector2 vector2) { return rect.Contains(vector2.AsXNAPoint()); }

    public static Rectangle Scale(this Rectangle rect, Single scale) {
      Int32 width_diff = (Int32)(rect.Width * (scale - 1.0f));
      Int32 height_diff = (Int32)(rect.Height * (scale - 1.0f));
      return new Rectangle(rect.X - width_diff / 2, rect.Y - height_diff / 2, rect.Width + width_diff, rect.Height + height_diff);
    }

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

    private static readonly Dictionary<SpriteFont, Point> sFontSizeCache = new();
    public static Point GetSize(this SpriteFont font) {
      if (sFontSizeCache.TryGetValue(font, out Point val)) return val;

      sFontSizeCache[font] = font.MeasureString("T").AsXNAPoint();
      return sFontSizeCache[font];
    }
    public static SKColor AsSKColor(this Color colour) {
      return new SKColor(colour.R, colour.G, colour.B, colour.A);
    }

    public static String AsHexCode(this Color colour) { return $"{colour.R:X2}{colour.G:X2}{colour.B:X2}"; }

    public static Color AsXNAColor(this String hexCode) {
      return Color.FromNonPremultiplied(Int32.Parse(hexCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                                        Int32.Parse(hexCode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                                        Int32.Parse(hexCode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                                        255);
    }

    // https://stackoverflow.com/a/1855903
    public static Color ContrastColour(this Color colour) { return (0.375 * colour.R + 0.587 * colour.G + 0.114 * colour.B) / 255 > 0.5 ? Color.Black : Color.White; }

    public static Color MultRGB(this Color colour, Single multiplier) {
      return Color.FromNonPremultiplied(Math.Min(255, Math.Max(0, (Int32)(colour.R * multiplier))),
                                        Math.Min(255, Math.Max(0, (Int32)(colour.G * multiplier))),
                                        Math.Min(255, Math.Max(0, (Int32)(colour.B * multiplier))),
                                        colour.A);
    }

    public static Color MultAlpha(this Color colour, Single multiplier) {
      return Color.FromNonPremultiplied(colour.R, colour.G, colour.B, Math.Min(255, Math.Max(0, (Int32)(colour.A * multiplier))));
    }

    public static Texture2D ToGrayScale(this Texture2D texture, GraphicsDevice device) {
      // get original pixels
      var orig_colours = new Color[texture.Width * texture.Height];
      texture.GetData(orig_colours);
      // prep grayscale texture
      var new_colors = new Color[texture.Width * texture.Height];
      var new_texture = new Texture2D(device, texture.Width, texture.Height);

      for (Int32 i = 0; i < texture.Width; i++) {
        for (Int32 j = 0; j < texture.Height; j++) {
          Int32 index = i + j * texture.Width;
          Color original_color = orig_colours[index];
          Single gray_scale = (original_color.R / 255.0f * 0.30f + original_color.G / 255.0f * 0.59f + original_color.B / 255.0f * 0.11f + original_color.A / 255.0f * 0.79f)
                              / 1.79f;
          new_colors[index] = new Color(gray_scale, gray_scale, gray_scale, original_color.A / 255.0f);
        }
      }

      new_texture.SetData(new_colors);
      return new_texture;
    }

    public static void DrawStringEx(this SpriteBatch spriteBatch, SpriteFont font, String text, Vector2 position,
                                    Color textColour, Single textAlpha = 1.0f, Single layerDepth = -1.0f, Boolean drawShadow = false,
                                    Single shadowDistance = 2.0f, Color textShadowColour = default, Single textShadowAlpha = 0.45f, Single scale = 1.0f) {
      if (layerDepth.Equals(-1.0f)) layerDepth = position.Y / 20000.0f;
      Vector2 origin = font.MeasureString(text) * 0.5f;
      position += origin;

      if (drawShadow) {
        if (textShadowColour == default) textShadowColour = textColour.MultRGB(0.5f);
        Color shadow_colour = textShadowColour.MultAlpha(textShadowAlpha);

        spriteBatch.DrawString(font,
                               text,
                               position + new Vector2(-shadowDistance, shadowDistance),
                               shadow_colour,
                               0.0f,
                               origin,
                               scale,
                               SpriteEffects.None,
                               layerDepth - 0.0001f);
        spriteBatch.DrawString(font,
                               text,
                               position + new Vector2(0.0f, shadowDistance),
                               shadow_colour,
                               0.0f,
                               origin,
                               scale,
                               SpriteEffects.None,
                               layerDepth - 0.0002f);
        spriteBatch.DrawString(font,
                               text,
                               position + new Vector2(-shadowDistance, 0.0f),
                               shadow_colour,
                               0.0f,
                               origin,
                               scale,
                               SpriteEffects.None,
                               layerDepth - 0.0003f);
      }

      spriteBatch.DrawString(font,
                             text,
                             position,
                             textColour.MultAlpha(textAlpha),
                             0.0f,
                             origin,
                             scale,
                             SpriteEffects.None,
                             layerDepth);
    }

    public static void DrawStringEx(this SpriteBatch spriteBatch, SpriteFont font, StringBuilder text, Vector2 position,
                                    Color textColour, Single textAlpha = 1.0f, Single layerDepth = -1.0f, Boolean drawShadow = false,
                                    Single shadowDistance = 2.0f, Color textShadowColour = default, Single textShadowAlpha = 0.45f, Single scale = 1.0f) {
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
                   scale);
    }
  }
}

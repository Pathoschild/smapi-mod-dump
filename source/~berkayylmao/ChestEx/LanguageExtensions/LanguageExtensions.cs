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
using System.Linq;
using System.Text;

using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx.LanguageExtensions {
  public static class DotNetExtensions {
    // https://stackoverflow.com/a/4915891/5071575
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static Boolean NearlyEquals(this Single lhs, Single rhs) {
      Single abs_lhs = Math.Abs(lhs);
      Single abs_rhs = Math.Abs(rhs);
      Single diff    = Math.Abs(lhs - rhs);

      if (lhs == rhs) return true;
      return lhs == 0 || rhs == 0 || diff < Single.MinValue ? diff < Single.Epsilon * Single.MinValue : diff / (abs_lhs + abs_rhs) < Single.Epsilon;
    }

    public static Boolean NearlyEquals(this Vector2 lhs, Vector2 rhs) { return lhs.X.NearlyEquals(rhs.X) & lhs.Y.NearlyEquals(rhs.Y); }

    public static Color AsXNAColor(this System.Drawing.Color colour) { return Color.FromNonPremultiplied(colour.R, colour.G, colour.B, colour.A); }

    // https://stackoverflow.com/a/1855903
    public static System.Drawing.Color ContrastColour(this System.Drawing.Color colour) {
      return (0.299 * colour.R + 0.587 * colour.G + 0.114 * colour.B) / 255 > 0.5 ? System.Drawing.Color.Black : System.Drawing.Color.White;
    }

    /// <summary>
    /// Flips the bit of the given boolean; e.g., true becomes false and false becomes true.
    /// </summary>
    /// <param name="value">Boolean to edit.</param>
    public static void Flip(this ref Boolean value) { value = !value; }
  }

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

  public static class SVExtensions {
    /// <summary>
    /// Gets a <see cref="Rectangle"/> to wrap the given rectangle to be used with'<see cref="StardewValley.Game1.drawDialogueBox(Int32, Int32, Int32, Int32, Boolean, Boolean, String, Boolean, Boolean, Int32, Int32, Int32)"/>'.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> that can be used to draw a dialogue box around the given rectangle.</returns>
    public static Rectangle GetDialogueBoxRectangle(this Rectangle rectangle) {
      return new(rectangle.X - 36,
                 rectangle.Y - IClickableMenu.spaceToClearTopBorder - 8,
                 rectangle.Width + IClickableMenu.borderWidth + 32,
                 rectangle.Height + IClickableMenu.spaceToClearTopBorder + 48);
    }

    /// <summary>
    /// Gets a <see cref="Rectangle"/> to wrap the given <see cref="InventoryMenu"/> using '<see cref="StardewValley.Game1.drawDialogueBox(Int32, Int32, Int32, Int32, Boolean, Boolean, String, Boolean, Boolean, Int32, Int32, Int32)"/>'.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> that can be used to draw a dialogue box around the given menu.</returns>
    public static Rectangle GetDialogueBoxRectangle(this InventoryMenu menu) {
      Point last_slot = menu.GetSlotDrawPositions().Last().AsXNAPoint();

      return new Rectangle(menu.xPositionOnScreen - 36,
                           menu.yPositionOnScreen - IClickableMenu.spaceToClearTopBorder - 12,
                           last_slot.X + (IClickableMenu.borderWidth + 32) * 2 - menu.xPositionOnScreen,
                           last_slot.Y + (IClickableMenu.spaceToClearTopBorder + 10) * 2 - menu.yPositionOnScreen);
    }

    /// <summary>
    /// Gets a <see cref="Rectangle"/> that represents the content rectangle of the given dialogue box.
    /// </summary>
    /// <returns>Content <see cref="Rectangle"/> of the given dialogue box.</returns>
    public static Rectangle GetContentRectangle(this Rectangle dialogueBoxBounds) {
      return new(dialogueBoxBounds.X + 36,
                 dialogueBoxBounds.Y + IClickableMenu.spaceToClearTopBorder + 12,
                 dialogueBoxBounds.Width - IClickableMenu.borderWidth - 36,
                 dialogueBoxBounds.Height - IClickableMenu.spaceToClearTopBorder - 48);
    }

    /// <summary>
    /// Gets a <see cref="Rectangle"/> that represents the content rectangle of the <see cref="InventoryMenu"/>.
    /// </summary>
    /// <returns>Content <see cref="Rectangle"/> of the given <see cref="InventoryMenu"/>.</returns>
    public static Rectangle GetContentRectangle(this InventoryMenu menu) {
      Point last_slot = menu.GetSlotDrawPositions().Last().AsXNAPoint();

      return new Rectangle(menu.xPositionOnScreen - 4,
                           menu.yPositionOnScreen - 12,
                           last_slot.X + IClickableMenu.borderWidth + 32 - menu.xPositionOnScreen,
                           last_slot.Y + IClickableMenu.spaceToClearTopBorder - 12 - menu.yPositionOnScreen);
    }

    /// <summary>
    /// Gets the bounds of the given <see cref="IClickableMenu"/>.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> containing the bounds of the given menu.</returns>
    public static Rectangle GetBounds(this IClickableMenu menu) { return new(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height); }

    /// <summary>
    /// Sets the given menu's X and Y position, and width and height to that of the given bounds.
    /// </summary>
    public static void SetBounds(this IClickableMenu menu, Rectangle bounds) {
      menu.xPositionOnScreen = bounds.X;
      menu.yPositionOnScreen = bounds.Y;
      menu.width             = bounds.Width;
      menu.height            = bounds.Height;
    }

    /// <summary>
    /// Sets the given menu's X and Y position, and width and height to that of the given values.
    /// </summary>
    public static void SetBounds(this IClickableMenu menu, Int32 x, Int32 y, Int32 width,
                                 Int32               height) {
      menu.xPositionOnScreen = x;
      menu.yPositionOnScreen = y;
      menu.width             = width;
      menu.height            = height;
    }

    /// <summary>
    /// Sets the given menu's and its clickable components' visibiliy to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="isVisible">Whether this menu should be visible.</param>
    public static void SetVisibleEx(this InventoryMenu menu, Boolean isVisible) { menu.inventory?.ForEach(cc => cc.visible = isVisible); }

    public static void DrawEx(this InventoryMenu menu, SpriteBatch b, Color slotBorderColour) { menu.draw(b, slotBorderColour.R, slotBorderColour.G, slotBorderColour.B); }

    public static ExtendedChest.ChestType GetChestType(this Chest chest) {
      return chest.fridge.Value       ? ExtendedChest.ChestType.Fridge :
        chest.ParentSheetIndex == 130 ? ExtendedChest.ChestType.WoodenChest :
        chest.ParentSheetIndex == 232 ? ExtendedChest.ChestType.StoneChest : ExtendedChest.ChestType.None;
    }

    public static Color GetActualColour(this Chest chest) {
      Color col = chest.playerChoiceColor.Value;

      if (col == Color.Black)
        col = chest.GetChestType() == ExtendedChest.ChestType.WoodenChest ? Color.FromNonPremultiplied(206, 120, 41, 255) : Color.FromNonPremultiplied(207, 191, 179, 255);

      return col;
    }
  }
}

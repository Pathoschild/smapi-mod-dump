/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

//
//    Copyright (C) 2020 Berkay Yigit <berkaytgy@gmail.com>
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

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChestEx.LanguageExtensions {
   public static class DotNetExtensions {
      // https://stackoverflow.com/a/4915891/5071575
      public static Boolean NearlyEquals(this Single lhs, Single rhs) {
         Single absLHS = Math.Abs(lhs);
         Single absRHS = Math.Abs(rhs);
         Single diff = Math.Abs(lhs - rhs);

         if (lhs == rhs) {
            return true;
         } else if (lhs == 0 || rhs == 0 || diff < Single.MinValue) {
            return diff < (Single.Epsilon * Single.MinValue);
         } else {
            return diff / (absLHS + absRHS) < Single.Epsilon;
         }
      }

      public static Boolean NearlyEquals(this Vector2 lhs, Vector2 rhs) {
         return lhs.X.NearlyEquals(rhs.X) & lhs.Y.NearlyEquals(rhs.Y);
      }

      /// <summary>
      /// Flips the bit of the given boolean; e.g., true becomes false and false becomes true.
      /// </summary>
      /// <param name="value">Boolean to edit.</param>
      public static void Flip(this ref Boolean value) {
         value = !value;
      }
   }

   public static class XNAExtensions {
      /// <summary>
      /// Constructs and returns a <see cref="Point"/> from the given <see cref="Vector2"/>.
      /// </summary>
      /// <param name="vector2">Vector2 to convert.</param>
      /// <returns>Converted version of '<paramref name="vector2"/>'.</returns>
      public static Point AsXNAPoint(this Vector2 vector2) {
         return new Point(Convert.ToInt32(vector2.X), Convert.ToInt32(vector2.Y));
      }

      /// <summary>
      /// Constructs and returns a <see cref="Vector2"/> from the given <see cref="Point"/>.
      /// </summary>
      /// <param name="point">Point to convert.</param>
      /// <returns>Converted version of '<paramref name="point"/>'.</returns>
      public static Vector2 AsXNAVector2(this Point point) {
         return new Vector2(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
      }

      public static Point ExtractXYAsXNAPoint(this Rectangle rectangle) {
         return new Point(rectangle.X, rectangle.Y);
      }
      public static Vector2 ExtractXYAsXNAVector2(this Rectangle rectangle) {
         return new Vector2(Convert.ToSingle(rectangle.X), Convert.ToSingle(rectangle.Y));
      }
   }

   public static class SVExtensions {
      /// <summary>
      /// Gets a <see cref="Rectangle"/> to wrap the given rectangle to be used with'<see cref="StardewValley.Game1.drawDialogueBox(Int32, Int32, Int32, Int32, Boolean, Boolean, String, Boolean, Boolean, Int32, Int32, Int32)"/>'.
      /// </summary>
      /// <returns>A <see cref="Rectangle"/> that can be used to draw a dialogue box around the given rectangle.</returns>
      public static Rectangle GetRectangleForDialogueBox(this Rectangle rectangle) {
         return new Rectangle(
               rectangle.X - 36,
               rectangle.Y - StardewValley.Menus.IClickableMenu.spaceToClearTopBorder - 8,
               rectangle.Width + StardewValley.Menus.IClickableMenu.borderWidth + 32,
               rectangle.Height + StardewValley.Menus.IClickableMenu.spaceToClearTopBorder + 48
            );
      }

      /// <summary>
      /// Gets a <see cref="Rectangle"/> to wrap the given <see cref="StardewValley.Menus.InventoryMenu"/> using '<see cref="StardewValley.Game1.drawDialogueBox(Int32, Int32, Int32, Int32, Boolean, Boolean, String, Boolean, Boolean, Int32, Int32, Int32)"/>'.
      /// </summary>
      /// <returns>A <see cref="Rectangle"/> that can be used to draw a dialogue box around the given menu.</returns>
      public static Rectangle GetRectangleForDialogueBox(this StardewValley.Menus.InventoryMenu menu) {
         return new Rectangle(
               menu.xPositionOnScreen - 36,
               menu.yPositionOnScreen - StardewValley.Menus.IClickableMenu.spaceToClearTopBorder - 8,
               menu.width + StardewValley.Menus.IClickableMenu.borderWidth + 32,
               menu.height + StardewValley.Menus.IClickableMenu.spaceToClearTopBorder + 36
            );
      }

      /// <summary>
      /// Gets a <see cref="Rectangle"/> that represents the content rectangle of the given dialogue box.
      /// </summary>
      /// <returns>Content <see cref="Rectangle"/> of the given dialogue box.</returns>
      public static Rectangle GetContentRectangleOfDialogueBox(this Rectangle dialogueBoxBounds) {
         return new Rectangle(
               dialogueBoxBounds.X + 36,
               dialogueBoxBounds.Y + StardewValley.Menus.IClickableMenu.spaceToClearTopBorder + 8,
               dialogueBoxBounds.Width - StardewValley.Menus.IClickableMenu.borderWidth - 32,
               dialogueBoxBounds.Height - StardewValley.Menus.IClickableMenu.spaceToClearTopBorder - 48
            );
      }

      /// <summary>
      /// Gets a <see cref="Rectangle"/> that correctly represents the real bounds of the given <see cref="StardewValley.Menus.InventoryMenu"/>.
      /// </summary>
      /// <returns>The correct bounds of the given <see cref="StardewValley.Menus.InventoryMenu"/>.</returns>
      public static Rectangle GetNormalizedInventoryMenuBounds(this StardewValley.Menus.InventoryMenu menu) {
         return new Rectangle(
               menu.xPositionOnScreen,
               menu.yPositionOnScreen - 4,
               menu.width,
               menu.height - 4
            );
      }

      /// <summary>
      /// Gets the bounds of the given <see cref="StardewValley.Menus.IClickableMenu"/>.
      /// </summary>
      /// <returns>A <see cref="Rectangle"/> containing the bounds of the given menu.</returns>
      public static Rectangle GetBounds(this StardewValley.Menus.IClickableMenu menu) {
         return new Rectangle(menu.xPositionOnScreen,
                              menu.yPositionOnScreen,
                              menu.width,
                              menu.height);
      }

      /// <summary>
      /// Sets the given menu's X and Y position, and width and height to that of the given bounds.
      /// </summary>
      public static void SetBounds(this StardewValley.Menus.IClickableMenu menu, Rectangle bounds) {
         menu.xPositionOnScreen = bounds.X;
         menu.yPositionOnScreen = bounds.Y;
         menu.width = bounds.Width;
         menu.height = bounds.Height;
      }

      /// <summary>
      /// Sets the given menu's X and Y position, and width and height to that of the given values.
      /// </summary>
      public static void SetBounds(this StardewValley.Menus.IClickableMenu menu, Int32 x, Int32 y, Int32 width, Int32 height) {
         menu.xPositionOnScreen = x;
         menu.yPositionOnScreen = y;
         menu.width = width;
         menu.height = height;
      }

      /// <summary>
      /// Sets the given menu's and its clickable components' visibiliy to '<paramref name="isVisible"/>'.
      /// </summary>
      /// <param name="isVisible">Whether this menu should be visible.</param>
      public static void SetVisibleEx(this StardewValley.Menus.IClickableMenu menu, Boolean isVisible) {
         menu.allClickableComponents.ForEach((cc) => cc.visible = isVisible);
      }

      public static void draw(this StardewValley.Menus.InventoryMenu menu, SpriteBatch b, Color slotBorderColour) {
         menu.draw(b, slotBorderColour.R, slotBorderColour.G, slotBorderColour.B);
      }
   }
}

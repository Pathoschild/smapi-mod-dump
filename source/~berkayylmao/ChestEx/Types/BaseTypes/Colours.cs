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

using Microsoft.Xna.Framework;

namespace ChestEx.Types.BaseTypes {
   public class Colours {
      // Public static instances:
      #region Public static instances

      public static readonly Colours Default = new Colours(
         Color.Transparent,
         Color.White,
         Color.White,
         Color.White);
      public static readonly Colours DarkenOnAction = new Colours(
         Color.Transparent,
         Color.FromNonPremultiplied(255,255,255,255),
         Color.FromNonPremultiplied(204,204,204,255),
         Color.FromNonPremultiplied(153,153,153,255));
      public static readonly Colours LightenOnAction = new Colours(
         Color.Transparent,
         Color.FromNonPremultiplied(153,153,153,255),
         Color.FromNonPremultiplied(204,204,204,255),
         Color.FromNonPremultiplied(255,255,255,255));

      public static readonly Colours TurnTranslucentOnAction = new Colours(
         Color.Transparent,
         Color.Multiply(Color.White, 1.00f),
         Color.Multiply(Color.White, 0.75f),
         Color.Multiply(Color.White, 0.50f));
      public static readonly Colours TurnSlightlyTranslucentOnAction = new Colours(
         Color.Transparent,
         Color.Multiply(Color.White, 1.00f),
         Color.Multiply(Color.White, 0.85f),
         Color.Multiply(Color.White, 0.70f));

      public static readonly Colours TurnOpaqueOnAction = new Colours(
         Color.Transparent,
         Color.Multiply(Color.White, 0.50f),
         Color.Multiply(Color.White, 0.75f),
         Color.Multiply(Color.White, 1.00f));
      public static readonly Colours TurnSlightlyOpaqueOnAction = new Colours(
         Color.Transparent,
         Color.Multiply(Color.White, 0.50f),
         Color.Multiply(Color.White, 0.65f),
         Color.Multiply(Color.White, 0.80f));

      #endregion

      // Public:
      #region Public

      public Color BackgroundColour { get; set; }
      public Color ForegroundColour { get; set; }
      public Color HoverColour { get; set; }
      public Color PressedColour { get; set; }

      #endregion

      // Constructors:
      #region Constructors

      public Colours(Color backgroundColour, Color foregroundColour, Color hoverColour, Color pressedColour) {
         this.BackgroundColour = backgroundColour;
         this.ForegroundColour = foregroundColour;
         this.HoverColour = hoverColour;
         this.PressedColour = pressedColour;
      }

      public Colours() {
         this.BackgroundColour = Default.BackgroundColour;
         this.ForegroundColour = Default.ForegroundColour;
         this.HoverColour = Default.HoverColour;
         this.PressedColour = Default.PressedColour;
      }

      #endregion
   }
}

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

using Microsoft.Xna.Framework;

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
}

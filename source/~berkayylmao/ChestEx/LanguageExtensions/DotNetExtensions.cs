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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace ChestEx.LanguageExtensions {
  public static class DotNetExtensions {
    private const Single CONST_FLT_NORMALIZED = (1 << 23) * Single.Epsilon;

    // https://stackoverflow.com/a/4915891/5071575
    public static Boolean NearlyEquals(this Single lhs, Single rhs) {
      if (lhs.Equals(rhs)) return true;

      Single abs_lhs = Math.Abs(lhs);
      Single abs_rhs = Math.Abs(rhs);
      Single diff    = Math.Abs(lhs - rhs);

      return lhs == 0 || rhs == 0 || diff < CONST_FLT_NORMALIZED ?
        diff < Single.Epsilon * CONST_FLT_NORMALIZED :
        diff / Math.Min(abs_lhs + abs_rhs, Single.MaxValue) < Single.Epsilon;
    }

    public static Microsoft.Xna.Framework.Color AsXNAColor(this Color colour) { return Microsoft.Xna.Framework.Color.FromNonPremultiplied(colour.R, colour.G, colour.B, colour.A); }

    // https://stackoverflow.com/a/1855903
    public static Color ContrastColour(this Color colour) { return (0.375 * colour.R + 0.587 * colour.G + 0.114 * colour.B) / 255 > 0.5 ? Color.Black : Color.White; }

    public static void ToHSV(this Color colour, out Double hue, out Double saturation, out Double value) {
      Int32 max = Math.Max(colour.R, Math.Max(colour.G, colour.B));
      Int32 min = Math.Min(colour.R, Math.Min(colour.G, colour.B));

      hue        = colour.GetHue();
      saturation = Math.Max(0.0d, Math.Min(1.0d, max == 0 ? 0.0d : 1.0d - 1.0d * min / max));
      value      = Math.Max(0.0d, Math.Min(1.0d, max / 255.0d));
    }

    public static Color ColourFromHSV(Double hue, Double saturation, Double value) {
      while (hue < 0.0d) hue    += 360.0d;
      while (hue >= 360.0d) hue -= 360.0d;
      saturation = Math.Max(0.0d, Math.Min(1.0d, saturation));
      value = Math.Max(0.0d, Math.Min(1.0d, value));

      Double hf = hue / 60.0d;
      Int32  hi = Convert.ToInt32(Math.Floor(hf)) % 6;
      Double f  = hf - Math.Floor(hf);

      value *= 255.0d;
      Int32 v = Convert.ToInt32(value);
      Int32 p = Convert.ToInt32(value * (1.0d - saturation));
      Int32 q = Convert.ToInt32(value * (1.0d - f * saturation));
      Int32 t = Convert.ToInt32(value * (1.0d - (1.0d - f) * saturation));

      return hi switch {
        0 => Color.FromArgb(255, v, t, p),
        1 => Color.FromArgb(255, q, v, p),
        2 => Color.FromArgb(255, p, v, t),
        3 => Color.FromArgb(255, p, q, v),
        4 => Color.FromArgb(255, t, p, v),
        _ => Color.FromArgb(255, v, p, q)
      };
    }

    /// <summary>
    /// Flips the bit of the given boolean; e.g., true becomes false and false becomes true.
    /// </summary>
    /// <param name="value">Boolean to edit.</param>
    public static void Flip(this ref Boolean value) { value = !value; }

    public static TResult IgnoreExceptions<T, TResult>(this T obj, Func<T, TResult> function) {
      try { return function(obj); }
      catch { return default; }
    }

    [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
    public static void IgnoreExceptions<T>(this T obj, Action<T> function) {
      try { function(obj); }
      catch { }
    }
  }
}

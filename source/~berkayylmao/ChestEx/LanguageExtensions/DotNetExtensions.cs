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
using System.Diagnostics.CodeAnalysis;

namespace ChestEx.LanguageExtensions {
  public static class DotNetExtensions {
    private const Single CONST_FLT_NORMALIZED = (1 << 23) * Single.Epsilon;

    // https://stackoverflow.com/a/4915891/5071575
    public static Boolean NearlyEquals(this Single lhs, Single rhs) {
      if (lhs.Equals(rhs)) return true;

      Single abs_lhs = Math.Abs(lhs);
      Single abs_rhs = Math.Abs(rhs);
      Single diff = Math.Abs(lhs - rhs);

      return lhs == 0 || rhs == 0 || diff < CONST_FLT_NORMALIZED ?
        diff < Single.Epsilon * CONST_FLT_NORMALIZED :
        diff / Math.Min(abs_lhs + abs_rhs, Single.MaxValue) < Single.Epsilon;
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

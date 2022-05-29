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

using System;

using SkiaSharp;

namespace ChestEx.LanguageExtensions {
  public static class SkiaSharpExtensions {
    public static Microsoft.Xna.Framework.Color AsXNAColor(this SKColor colour) { return Microsoft.Xna.Framework.Color.FromNonPremultiplied(colour.Red, colour.Green, colour.Blue, colour.Alpha); }

    // https://stackoverflow.com/a/1855903
    public static SKColor ContrastColour(this SKColor colour) { return (0.375 * colour.Red + 0.587 * colour.Green + 0.114 * colour.Blue) / 255 > 0.5 ? SKColors.Black : SKColors.White; }
  }
}

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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;

namespace ChestEx.Types.BaseTypes {
  public class Colours {
    // Public static instances:
  #region Public static instances

    public static readonly Colours gDefault = new(Color.White, Color.White, Color.White, Color.White);

    public static readonly Colours gLightButton = new(Color.FromNonPremultiplied(204, 204, 204, 255), Color.Black, Color.White, Color.FromNonPremultiplied(153, 153, 153, 255));

    public static readonly Colours gDarkButton = new(Color.FromNonPremultiplied(50, 60, 70, 255),
                                                     Color.White,
                                                     Color.FromNonPremultiplied(70, 80, 90, 255),
                                                     Color.FromNonPremultiplied(30, 40, 50, 255));

    public static readonly Colours gDarkenOnAction = new(Color.White,
                                                         Color.FromNonPremultiplied(255, 255, 255, 255),
                                                         Color.FromNonPremultiplied(204, 204, 204, 255),
                                                         Color.FromNonPremultiplied(153, 153, 153, 255));

    public static readonly Colours gLightenOnAction = new(Color.White,
                                                          Color.FromNonPremultiplied(153, 153, 153, 255),
                                                          Color.FromNonPremultiplied(204, 204, 204, 255),
                                                          Color.FromNonPremultiplied(255, 255, 255, 255));

    public static readonly Colours gTurnTranslucentOnAction =
      new(Color.White, Color.Multiply(Color.White, 1.00f), Color.Multiply(Color.White, 0.75f), Color.Multiply(Color.White, 0.50f));

    public static readonly Colours gTurnSlightlyTranslucentOnAction =
      new(Color.White, Color.Multiply(Color.White, 1.00f), Color.Multiply(Color.White, 0.85f), Color.Multiply(Color.White, 0.70f));

    public static readonly Colours gTurnOpaqueOnAction =
      new(Color.White, Color.Multiply(Color.White, 0.50f), Color.Multiply(Color.White, 0.75f), Color.Multiply(Color.White, 1.00f));

    public static readonly Colours gTurnSlightlyOpaqueOnAction =
      new(Color.White, Color.Multiply(Color.White, 0.50f), Color.Multiply(Color.White, 0.65f), Color.Multiply(Color.White, 0.80f));

  #endregion

    // Public:
  #region Public

    public Color mBackgroundColour { get; }
    public Color mForegroundColour { get; }
    public Color mHoverColour      { get; }
    public Color mPressedColour    { get; }

    // Statics:
  #region Statics

    public static Colours GenerateFrom(Color backgroundColour) {
      return new(backgroundColour,
                 backgroundColour.ContrastColour(),
                 Color.FromNonPremultiplied((Int32)(backgroundColour.R * 0.75f), (Int32)(backgroundColour.G * 0.75f), (Int32)(backgroundColour.B * 0.75f), backgroundColour.A),
                 Color.FromNonPremultiplied((Int32)(backgroundColour.R * 1.25f), (Int32)(backgroundColour.G * 1.25f), (Int32)(backgroundColour.B * 1.25f), backgroundColour.A));
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    private Colours(Color backgroundColour, Color foregroundColour, Color hoverColour, Color pressedColour) {
      this.mBackgroundColour = backgroundColour;
      this.mForegroundColour = foregroundColour;
      this.mHoverColour      = hoverColour;
      this.mPressedColour    = pressedColour;
    }

    private Colours() {
      this.mBackgroundColour = gDefault.mBackgroundColour;
      this.mForegroundColour = gDefault.mForegroundColour;
      this.mHoverColour      = gDefault.mHoverColour;
      this.mPressedColour    = gDefault.mPressedColour;
    }

  #endregion
  }
}

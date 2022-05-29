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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public class Colours {
    // Public static instances:
    #region Public static instances

    public static readonly Colours gDefault = new(Color.White, Color.White, Color.White, Color.White, Color.White);
    public static readonly Colours gTransparent = new(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent);

    public static readonly Colours gLight = new(Color.FromNonPremultiplied(200, 200, 200, 255),
                                                Color.FromNonPremultiplied(250, 250, 250, 255),
                                                Color.Black,
                                                Color.White,
                                                Color.FromNonPremultiplied(153, 153, 153, 255));

    public static readonly Colours gDark = new(Color.FromNonPremultiplied(50, 60, 70, 255),
                                               Color.FromNonPremultiplied(60, 70, 80, 255),
                                               Color.White,
                                               Color.FromNonPremultiplied(70, 80, 90, 255),
                                               Color.FromNonPremultiplied(35, 45, 55, 255));
    public static readonly Colours gDarkShadow = new(Color.FromNonPremultiplied(30, 30, 30, 75),
                                                     Color.FromNonPremultiplied(30, 30, 30, 75),
                                                     Color.FromNonPremultiplied(30, 30, 30, 75),
                                                     Color.FromNonPremultiplied(30, 30, 30, 75),
                                                     Color.FromNonPremultiplied(30, 30, 30, 75));
    public static readonly Colours gDarker = new(Color.FromNonPremultiplied(25, 35, 45, 255),
                                                 Color.FromNonPremultiplied(35, 45, 55, 255),
                                                 Color.White,
                                                 Color.FromNonPremultiplied(45, 55, 65, 255),
                                                 Color.FromNonPremultiplied(5, 5, 5, 255));

    public static readonly Colours gTurnTranslucentOnAction = new(Color.White, Color.White, Color.Black, Color.White.MultAlpha(0.75f), Color.White.MultAlpha(0.50f));

    #endregion

    // Public:
    #region Public

    public Color mBackgroundColour { get; }
    public Color mBorderColour { get; }
    public Color mForegroundColour { get; }
    public Color mHoverColour { get; }
    public Color mActiveColour { get; }

    public Colours NewBackgroundColour(Color colour) { return new(colour, this.mBorderColour, this.mForegroundColour, this.mHoverColour, this.mActiveColour); }
    public Colours MultAlpha(Single multiplier) {
      return new(this.mBackgroundColour.MultAlpha(multiplier),
                 this.mBorderColour.MultAlpha(multiplier),
                 this.mForegroundColour.MultAlpha(multiplier),
                 this.mHoverColour.MultAlpha(multiplier),
                 this.mActiveColour.MultAlpha(multiplier));
    }

    // Statics:
    #region Statics

    public static Colours GenerateFrom(Color backgroundColour) {
      return new(backgroundColour, backgroundColour.MultRGB(0.5f), backgroundColour.ContrastColour(), backgroundColour.MultRGB(1.25f), backgroundColour.MultRGB(0.675f));
    }

    private static Colours sMenuTileColourCache;
    public static Colours GenerateFromMenuTiles() {
      if (sMenuTileColourCache is null) {
        Color bgColour,
              borderColour,
              fgColour,
              hoverColour,
              activeColour;
        // gen colours
        {
          var pixel = new Color[1];
          Game1.menuTexture.GetData(0, new Rectangle(20, 128, 1, 1), pixel, 0, 1);
          borderColour = pixel[0];
          Game1.menuTexture.GetData(0, new Rectangle(64, 128, 1, 1), pixel, 0, 1);
          bgColour = pixel[0];
          fgColour = bgColour.ContrastColour();
          hoverColour = bgColour.MultRGB(1.25f);
          activeColour = bgColour.MultRGB(0.675f);
        }
        sMenuTileColourCache = new Colours(bgColour, borderColour, fgColour, hoverColour, activeColour);
      }

      return sMenuTileColourCache;
    }

    /// <summary>Resets <see cref="mForegroundColour"/> to the contrast BW colour of the new <see cref="mBackgroundColour"/>.</summary>
    public static Colours operator *(Colours lhs, Single rhs) {
      return new(lhs.mBackgroundColour.MultRGB(rhs),
                 lhs.mBorderColour.MultRGB(rhs),
                 lhs.mBackgroundColour.MultRGB(rhs).ContrastColour(),
                 lhs.mHoverColour.MultRGB(rhs),
                 lhs.mActiveColour.MultRGB(rhs));
    }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    private Colours(Color backgroundColour, Color borderColour, Color foregroundColour, Color hoverColour,
                    Color activeColour) {
      this.mBackgroundColour = backgroundColour;
      this.mBorderColour = borderColour;
      this.mForegroundColour = foregroundColour;
      this.mHoverColour = hoverColour;
      this.mActiveColour = activeColour;
    }

    #endregion
  }
}

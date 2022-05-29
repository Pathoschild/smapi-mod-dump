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
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public partial class ColouringHSVMenu : CustomClickableMenu {
    private readonly Rectangle separatorBounds;

    private readonly HueSlider hueSlider;
    private readonly ColourPicker colourPicker;

    public void SetColour(Color newColour, Boolean fireEvents = true) {
      if (newColour == Color.Black) {
        this.hueSlider.Reset();
        this.colourPicker.Reset();
        return;
      }

      newColour.AsSKColor().ToHsv(out Single hue, out Single sat, out Single val);
      hue /= 360.0f;
      sat = 1.0f - sat;
      val = 1.0f - val;

      this.hueSlider.SetSelectorActiveY((Int32)(this.hueSlider.mBounds.Height * hue));
      this.colourPicker.SetSelectorActivePos(new Point((Int32)(this.colourPicker.mBounds.Width * sat), (Int32)(this.colourPicker.mBounds.Height * val)), fireEvents);
    }

    public override void Draw(SpriteBatch spriteBatch) {
      base.Draw(spriteBatch);

      spriteBatch.Draw(Game1.uncoloredMenuTexture, this.separatorBounds, new Rectangle(88, 36, 16, 1), this.mData.mColours.mBorderColour);
    }

    public ColouringHSVMenu(Rectangle bounds, Colours colours, Color activeColour = default, Action<Color> onTryColourAction = null,
                            Action<Color> onFinalColourAction = null)
      : base(bounds, colours) {
      if (activeColour == default) activeColour = Color.Black;

      Int32 hue_slider_width = Convert.ToInt32(this.mBounds.Width * 0.15f);
      Int32 separator_width = Convert.ToInt32(this.mBounds.Width * 0.075f);

      this.colourPicker = new ColourPicker(new Rectangle(this.mBounds.X + hue_slider_width + separator_width,
                                                         this.mBounds.Y,
                                                         this.mBounds.Width - hue_slider_width - separator_width,
                                                         this.mBounds.Height),
                                           colours,
                                           c => {
                                             this.colourPicker.mData.mHoverText = $"R: {c.R}, G: {c.G}, B: {c.B}{Environment.NewLine}#{c.R:X2}{c.G:X2}{c.B:X2}";
                                             onTryColourAction?.Invoke(c);
                                           },
                                           onFinalColourAction);
      this.hueSlider = new HueSlider(new Rectangle(this.mBounds.X, this.mBounds.Y, hue_slider_width, this.mBounds.Height),
                                     this.colourPicker.SetHueColour,
                                     this.colourPicker.SetHueColour);

      this.separatorBounds = new Rectangle(this.hueSlider.mBounds.Right, this.mBounds.Y - 4, separator_width, this.mBounds.Height + 8);

      this.SetColour(activeColour, false);
      this.mComponents.AddRange(new ICustomComponent[] { this.hueSlider, this.colourPicker });
    }
  }
}

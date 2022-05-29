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

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public class ColouringHEXMenu : CustomClickableMenu {
    // Private:
    #region Private

    private static Tuple<String, String> sCopiedColors;

    private readonly CustomTextBox chestTextBox;
    private readonly CustomTextBox hingesTextBox;
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    private readonly CustomButton copyColoursButton;
    private readonly CustomButton pasteColoursButton;

    #endregion

    // Public:
    #region Public

    public void SetColours(Color chestColour, Color hingesColour) {
      this.chestTextBox.Text = chestColour.AsHexCode();
      this.hingesTextBox.Text = hingesColour.AsHexCode();
    }

    public void SetColours(String chestColour, String hingesColour) {
      this.chestTextBox.Text = chestColour;
      this.hingesTextBox.Text = hingesColour;
    }

    // Overrides:
    #region Overrides

    public override void Draw(SpriteBatch spriteBatch) {
      base.Draw(spriteBatch);

      spriteBatch.DrawString(Game1.smallFont, "Chest colour:", new Vector2(this.mBounds.X + 8, this.mBounds.Y + 8), this.mData.mFGColour);
      spriteBatch.DrawString(Game1.smallFont, "Hinges colour:", new Vector2(this.mBounds.X + 8, this.chestTextBox.mBounds.Bottom + 8), this.mData.mFGColour);
    }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    public ColouringHEXMenu(Rectangle bounds, Colours colours, Action<Color, Color> onFinalColourAction)
      : base(bounds, colours) {
      this.chestTextBox = new CustomTextBox(new Rectangle(this.mBounds.X + 8, this.mBounds.Y + 8 + Game1.smallFont.GetSize().Y, this.mBounds.Width - 16, -1),
                                            colours * 0.75f,
                                            preText: "#",
                                            maxLength: 6,
                                            inputFilterFunction: CustomTextBox.gAcceptHexOnly,
                                            onTextChangedHandler: text => {
                                              if (this.chestTextBox.mText.Length == 6 && this.hingesTextBox.mText.Length == 6)
                                                onFinalColourAction(text.AsXNAColor(), this.hingesTextBox.mText.AsXNAColor());
                                            });

      this.hingesTextBox = new CustomTextBox(new Rectangle(this.mBounds.X + 8, this.chestTextBox.mBounds.Bottom + 8 + Game1.smallFont.GetSize().Y, this.mBounds.Width - 16, -1),
                                             colours * 0.75f,
                                             preText: "#",
                                             maxLength: 6,
                                             inputFilterFunction: CustomTextBox.gAcceptHexOnly,
                                             onTextChangedHandler: text => {
                                               if (this.chestTextBox.mText.Length == 6 && this.hingesTextBox.mText.Length == 6)
                                                 onFinalColourAction(this.chestTextBox.mText.AsXNAColor(), text.AsXNAColor());
                                             });

      this.copyColoursButton =
        new CustomButton(new Rectangle(this.mBounds.X + 6,
                                       this.hingesTextBox.mBounds.Bottom + 8,
                                       (this.mBounds.Width - 20) / 2,
                                       this.mBounds.Bottom - this.hingesTextBox.mBounds.Bottom - 16),
                         colours * 0.5f,
                         "Copy",
                         "",
                         () => {
                           sCopiedColors = new Tuple<String, String>(this.chestTextBox.mText, this.hingesTextBox.mText);
                           this.pasteColoursButton.SetEnabled(true);
                         });
      this.pasteColoursButton =
        new CustomButton(new Rectangle(this.copyColoursButton.mBounds.Right + 8,
                                       this.copyColoursButton.mBounds.Y,
                                       this.copyColoursButton.mBounds.Width,
                                       this.copyColoursButton.mBounds.Height),
                         colours * 0.5f,
                         "Paste",
                         "",
                         () => {
                           this.SetColours(sCopiedColors.Item1, sCopiedColors.Item2);
                           onFinalColourAction(this.chestTextBox.mText.AsXNAColor(), this.hingesTextBox.mText.AsXNAColor());
                         });
      if (sCopiedColors is null) this.pasteColoursButton.SetEnabled(false);

      this.mComponents.AddRange(new ICustomComponent[] { this.pasteColoursButton, this.copyColoursButton, this.hingesTextBox, this.chestTextBox });
    }

    #endregion
  }
}

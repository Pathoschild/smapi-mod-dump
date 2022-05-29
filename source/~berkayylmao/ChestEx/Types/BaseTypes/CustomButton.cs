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

using StardewModdingAPI;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public class CustomButton : CustomClickableComponent {
    // Private:
    #region Private

    private Action onClickHandler { get; }
    private String text { get; }

    #endregion

    // Protected:
    #region Protected

    protected Vector2 mTextPosition { get; set; }

    #endregion

    // Public:
    #region Public

    // Overrides:
    #region Overrides

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomButton"/> implementation:
    /// <para>1. Draws the button background with <see cref="ICustomComponent.IData.mBGColour"/> colouring.</para>
    /// <para>2. Draws the button text with <see cref="ICustomComponent.IData.mFGColour"/> colouring.</para>
    /// <para>3. Calls <see cref="CustomClickableComponent.Draw(SpriteBatch)"/>.</para>
    /// </remarks>
    public override void Draw(SpriteBatch spriteBatch) {
      spriteBatch.Draw(Game1.fadeToBlackRect, this.mBounds.Scale(this.scale), this.mData.mBGColour);
      spriteBatch.DrawStringEx(Game1.smallFont, this.text, this.mTextPosition, this.mData.mFGColour, scale: this.scale);

      base.Draw(spriteBatch);
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomButton"/> implementation:
    /// <para>1. Calls <see cref="CustomClickableComponent.OnMouseClick(InputStateEx)"/></para>
    /// <para>2. Calls the on-click handler if it's a left mouse click.</para>
    /// </remarks>
    public override void OnMouseClick(InputStateEx inputState) {
      base.OnMouseClick(inputState);
      if (inputState.mButton == SButton.MouseLeft) this.onClickHandler();
    }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    public CustomButton(Rectangle bounds, Colours colours, String text, String hoverText,
                        Action onClickHandler)
      : base(bounds, colours, hoverText) {
      this.onClickHandler = onClickHandler;
      this.text = text;

      Vector2 text_size = Game1.smallFont.MeasureString(text);
      if (text == "+") text_size.Y = 26.0f;
      this.mTextPosition = new Vector2(this.mBounds.Center.X - text_size.X / 2.0f, this.mBounds.Center.Y - text_size.Y / 2.0f);
    }

    public CustomButton(Rectangle bounds, Color backgroundColour, String text, String hoverText,
                        Action onClickHandler)
      : this(bounds, Colours.GenerateFrom(backgroundColour), text, hoverText, onClickHandler) { }

    #endregion
  }
}

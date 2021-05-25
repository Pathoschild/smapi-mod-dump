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
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace ChestEx.Types.BaseTypes {
  public class CustomNumericUpDownBox : IDisposable {
    // Private:
  #region Private

    private Int32 curValue;

  #endregion

    // Protected:
  #region Protected

    protected SpriteFont mFont => Game1.smallFont;
    protected Color      mBoxColour;
    protected Color      mTextColour;

    protected CustomTextBox mTextBox;
    protected CustomButton  mUpButton;
    protected CustomButton  mDownButton;

    protected Int32         mMinValue;
    protected Int32         mMaxValue;
    protected Action<Int32> mOnValueChangedHandler;

  #endregion

    // Public:
  #region Public

    public Int32 mValue {
      get => this.curValue;
      set {
        if (this.curValue == value) return;
        this.curValue      = Math.Min(this.mMaxValue, Math.Max(this.mMinValue, value));
        this.mTextBox.Text = this.curValue.ToString();
        this.mOnValueChangedHandler?.Invoke(this.curValue);
      }
    }

    public Rectangle mBounds    { get; private set; }
    public Boolean   mIsVisible { get; private set; }

    // Virtuals
  #region Virtuals

    /// <summary>
    /// Base implementation sets '<see cref="mIsVisible"/>' to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this item should be visible.</param>
    public virtual void SetVisible(Boolean isVisible) {
      this.mIsVisible = isVisible;
      this.mTextBox.SetVisible(isVisible);
    }

    /// <summary>
    /// Must be called on each event to function.
    /// </summary>
    /// <param name="e">SMAPI generated EventArgs.</param>
    public virtual void OnButtonPressed(ButtonPressedEventArgs e) {
      if (!this.mIsVisible) return;
      this.mTextBox.OnButtonPressed(e);

      if (this.mTextBox.Selected) {
        if (!String.IsNullOrWhiteSpace(this.mTextBox.Text)) this.mValue = !Int32.TryParse(this.mTextBox.Text, out Int32 value) ? this.mMinValue : value;

        switch (e.Button) {
          case SButton.OemPlus or SButton.Add: {
            this.mValue++;

            break;
          }
          case SButton.OemMinus or SButton.Subtract: {
            this.mValue--;

            break;
          }
        }
      }

      if (e.Button != SButton.MouseLeft) return;

      Point xna_cursor_pos = Utility.ModifyCoordinatesForUIScale(e.Cursor.ScreenPixels).AsXNAPoint();

      if (this.mBounds.Contains(xna_cursor_pos)) GlobalVars.gSMAPIHelper.Input.Suppress(e.Button);

      if (this.mUpButton.mBounds.Contains(xna_cursor_pos)) {
        if (String.IsNullOrWhiteSpace(this.mTextBox.Text)) this.mValue = this.mMinValue;
        this.mValue++;
        this.mTextBox.SelectMe();
      }
      else if (this.mDownButton.mBounds.Contains(xna_cursor_pos)) {
        if (String.IsNullOrWhiteSpace(this.mTextBox.Text)) this.mValue = this.mMinValue;
        this.mValue--;
        this.mTextBox.SelectMe();
      }
    }

  #endregion

    public void Draw(SpriteBatch spriteBatch) {
      if (!this.mIsVisible) return;
      this.mTextBox.Draw(spriteBatch);
      this.mUpButton.draw(spriteBatch);
      this.mDownButton.draw(spriteBatch);
    }

  #endregion

    // Constructors:
  #region Constructors

    public CustomNumericUpDownBox(Point         position, Color textColour,       Color  boxColour,  Int32 minValue,
                                  Int32         maxValue, Int32 initialValue = 0, String label = "", Color labelColour = default,
                                  Action<Int32> onValueChangedHandler = null) {
      this.mTextBox = new CustomTextBox(new Rectangle(position.X, position.Y, -1, 50),
                                        textColour,
                                        boxColour,
                                        initialValue.ToString(),
                                        "",
                                        Math.Max(minValue.ToString().Length, maxValue.ToString().Length),
                                        label,
                                        labelColour,
                                        CustomTextBox.gAcceptNumbersOnly);
      this.mUpButton = new CustomButton(new Rectangle(this.mTextBox.mBounds.Right + 2, this.mTextBox.mBounds.Y, 24, 24), boxColour, null, "+", null);
      this.mUpButton.SetVisible(true);
      this.mUpButton.SetEnabled(true);
      this.mDownButton = new CustomButton(new Rectangle(this.mUpButton.mBounds.X, this.mUpButton.mBounds.Bottom + 2, 24, 24), boxColour, null, "-", null);
      this.mDownButton.SetVisible(true);
      this.mDownButton.SetEnabled(true);

      this.mBounds                = new Rectangle(position.X, position.Y, this.mDownButton.mBounds.Right - position.X, this.mDownButton.mBounds.Bottom - position.Y);
      this.mBoxColour             = boxColour;
      this.mTextColour            = textColour;
      this.mMinValue              = minValue;
      this.mMaxValue              = maxValue;
      this.mValue                 = initialValue;
      this.mOnValueChangedHandler = onValueChangedHandler;
    }

  #endregion

    // IDisposable:
  #region IDisposable

    public void Dispose() {
      this.SetVisible(false);

      this.mTextBox?.Dispose();
      this.mUpButton?.Dispose();
      this.mDownButton?.Dispose();
    }

  #endregion
  }
}

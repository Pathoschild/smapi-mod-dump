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

namespace ChestEx.Types.BaseTypes {
  public sealed class CustomNumericUpDownBox : CustomTextBox {
    // Private:
    #region Private

    private Int32 minValue { get; }
    private Int32 maxValue { get; }
    private Int32 curValue { get; set; }

    private Action<Int32> onValueChangedHandler { get; }

    private CustomButton upButton { get; }
    private CustomButton downButton { get; }

    /// <summary>Executes <paramref name="action"/> if the button is visible and enabled.</summary>
    private void delegateToUpDownButtons(Action<CustomButton> action) {
      if (this.upButton.mIsVisible && this.upButton.mData.mIsEnabled) action(this.upButton);
      if (this.downButton.mIsVisible && this.downButton.mData.mIsEnabled) action(this.downButton);
    }

    #endregion

    // Public:
    #region Public

    /// <summary>Current value</summary>
    public Int32 mValue {
      get => this.curValue;
      private set {
        if (this.curValue == value) return;
        this.curValue = Math.Min(this.maxValue, Math.Max(this.minValue, value));
        this.mText = this.curValue.ToString();
        this.onValueChangedHandler?.Invoke(this.curValue);
      }
    }

    // Overrides:
    #region Overrides

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomNumericUpDownBox"/> implementation:
    /// <para>1. Calls <see cref="CustomTextBox.SetVisible(Boolean)"/>.</para>
    /// <para>2. Calls <see cref="CustomButton.SetVisible(Boolean)"/> on the up/down buttons.</para>
    /// </remarks>
    public override void SetVisible(Boolean isVisible) {
      base.SetVisible(isVisible);
      this.upButton.SetVisible(isVisible);
      this.downButton.SetVisible(isVisible);
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomNumericUpDownBox"/> implementation:
    /// <para>1. Calls <see cref="CustomTextBox.Draw(SpriteBatch)"/>.</para>
    /// <para>2. Calls <see cref="CustomButton.Draw(SpriteBatch)"/> on the up/down buttons.</para>
    /// </remarks>
    public override void Draw(SpriteBatch spriteBatch) {
      base.Draw(spriteBatch);
      this.upButton.Draw(spriteBatch);
      this.downButton.Draw(spriteBatch);
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomNumericUpDownBox"/> implementation:
    /// <para>1. Calls <see cref="CustomTextBox.OnButtonPressed(InputStateEx)"/>.</para>
    /// <para>2. Calls <see cref="CustomButton.OnButtonPressed(InputStateEx)"/> on the up/down buttons.</para>
    /// </remarks>
    public override void OnButtonPressed(InputStateEx inputState) {
      base.OnButtonPressed(inputState);
      if (this.Selected && !String.IsNullOrWhiteSpace(this.mText)) this.mValue = !Int32.TryParse(this.mText, out Int32 value) ? this.minValue : value;

      this.delegateToUpDownButtons(b => b.OnButtonPressed(inputState));
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomNumericUpDownBox"/> implementation:
    /// <para>1. Calls <see cref="CustomTextBox.OnCursorMoved(Vector2)"/>.</para>
    /// <para>2. Calls <see cref="CustomButton.OnCursorMoved(Vector2)"/> on the up/down buttons.</para>
    /// </remarks>
    public override void OnCursorMoved(Vector2 cursorPos) {
      base.OnCursorMoved(cursorPos);
      this.delegateToUpDownButtons(b => {
        b.mData.UpdateCursorStatus(b.mBounds.Contains(cursorPos));
        b.OnCursorMoved(cursorPos);
      });
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomNumericUpDownBox"/> implementation:
    /// <para>1. Calls <see cref="CustomTextBox.OnMouseClick(InputStateEx)"/>.</para>
    /// <para>2. Calls <see cref="CustomButton.OnMouseClick(InputStateEx)"/> on the up/down buttons.</para>
    /// </remarks>
    public override void OnMouseClick(InputStateEx inputState) {
      base.OnMouseClick(inputState);
      this.delegateToUpDownButtons(b => {
        b.mData.UpdateCursorStatus(b.mBounds.Contains(inputState.mCursorPos));
        if (inputState.mButton == SButton.MouseLeft && b.mBounds.Contains(inputState.mLastCursorPos) && b.mBounds.Contains(inputState.mCursorPos)) b.OnMouseClick(inputState);
      });
    }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    public CustomNumericUpDownBox(Rectangle bounds, Colours colours, String label, Int32 minValue,
                                  Int32 maxValue, Int32 initialValue, Action<Int32> onValueChangedHandler = null)
      : base(bounds,
             colours,
             label,
             String.Empty,
             initialValue.ToString(),
             Math.Max(minValue.ToString().Length, maxValue.ToString().Length) + 1,
             gAcceptNumbersOnly) {
      Int32 button_size = this.GetData().mDetailedBounds.mTextBoxContentBounds.Height / 2;

      this.upButton = new CustomButton(new Rectangle(this.GetData().mDetailedBounds.mTextBoxContentBounds.Right - button_size,
                                                     this.GetData().mDetailedBounds.mTextBoxContentBounds.Y,
                                                     button_size,
                                                     button_size),
                                       colours,
                                       "+",
                                       "Increase",
                                       () => {
                                         if (String.IsNullOrWhiteSpace(this.mText)) this.mValue = this.minValue;
                                         this.mValue++;
                                         this.SelectMe();
                                       });
      this.downButton = new CustomButton(new Rectangle(this.upButton.mBounds.X, this.upButton.mBounds.Bottom, button_size, button_size),
                                         colours,
                                         "-",
                                         "Decrease",
                                         () => {
                                           if (String.IsNullOrWhiteSpace(this.mText)) this.mValue = this.minValue;
                                           this.mValue--;
                                           this.SelectMe();
                                         });

      this.minValue = minValue;
      this.maxValue = maxValue;
      this.mValue = initialValue;
      this.onValueChangedHandler = onValueChangedHandler;
    }

    #endregion

    // IDisposable:
    #region IDisposable

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomNumericUpDownBox"/> implementation:
    /// 1. Calls <see cref="CustomTextBox.Dispose"/>
    /// 2. Disposes of the up/down buttons.
    /// </remarks>
    public override void Dispose() {
      base.Dispose();
      this.upButton?.Dispose();
      this.downButton?.Dispose();
    }

    #endregion
  }
}

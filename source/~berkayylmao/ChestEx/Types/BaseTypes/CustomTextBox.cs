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
using System.Linq;

using ChestEx.LanguageExtensions;

using HarmonyLib;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public partial class CustomTextBox : TextBox,
                                       ICustomComponent,
                                       IKeyboardSubscriber {
    // Private:
    #region Private:

    private Boolean showKeyboard {
      get => Traverse.Create(this).Field<Boolean>("_showKeyboard").Value;
      set => Traverse.Create(this).Field<Boolean>("_showKeyboard").Value = value;
    }

    #endregion

    // Public:
    #region Public

    public const Single CONST_BORDER_SCALE = 0.2f;

    public static readonly Func<Char, Boolean> gAcceptNumbersOnly = Char.IsDigit;
    public static readonly Func<Char, Boolean> gAcceptLettersAndNumbersOnly = Char.IsLetterOrDigit;
    public static readonly Func<Char, Boolean> gSVAcceptedInput = c => c != '|';

    public static readonly Func<Char, Boolean> gAcceptHexOnly = c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks><see cref="CustomTextBox"/> implementation returns the complete bound of the textbox (label + textbox).</remarks>
    public Rectangle mBounds { get; }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks><see cref="CustomTextBox"/> implementation stores <see cref="Data"/>.</remarks>
    public ICustomComponent.IData mData { get; }

    public Boolean mIsVisible { get; private set; } = true;

    /// <summary>Current text</summary>
    public String mText {
      get => this.Text.Substring(0, Math.Min(this.Text.Length, this.GetData().mMaxLength));
      protected set {
        String final_string = value.Substring(0, Math.Min(value.Length, this.GetData().mMaxLength));
        if (this.Text.Equals(final_string, StringComparison.InvariantCultureIgnoreCase)) return;

        this.Text = final_string;
        this.GetData().mHandlers.mOnTextChangedHandler?.Invoke(this.Text);
      }
    }

    /// <summary>Gets the <see cref="Data"/> implementation stored in <see cref="mData"/>.</summary>
    /// <returns><see cref="mData"/> as <see cref="Data"/></returns>
    public Data GetData() { return (Data)this.mData; }

    // Virtuals
    #region Virtuals

    /// <inheritdoc/>
    public virtual void SetEnabled(Boolean isEnabled) { this.mData.SetEnabled(isEnabled); }

    /// <inheritdoc/>
    public virtual void SetVisible(Boolean isVisible) { this.mIsVisible = isVisible; }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomTextBox"/> implementation:
    /// <para>1. Draws the label with <see cref="ICustomComponent.IData.mFGColour"/> colouring.</para>
    /// <para>2. Draws the textbox with <see cref="ICustomComponent.IData.mBGColour"/> coluring.</para>
    /// <para>3. Draws the text with <see cref="ICustomComponent.IData.mFGColour"/> colouring.</para>
    /// <para>4. Draws the caret if the textbox has focus.</para>
    /// </remarks>
    public virtual void Draw(SpriteBatch spriteBatch) {
      // Label
      spriteBatch.DrawString(this.GetData().mFont, this.GetData().mExtraText.mLabel, this.GetData().mDetailedBounds.mLabelBounds.ExtractXYAsXNAVector2(), this.mData.mFGColour);
      // Box outline
      spriteBatch.DrawTextureBox(this.GetData().mDetailedBounds.mTextBoxBounds, this.mData.mColours.NewBackgroundColour(this.mData.mBGColour), false, false, CONST_BORDER_SCALE);
      // Box
      spriteBatch.Draw(Game1.fadeToBlackRect, this.GetData().mDetailedBounds.mTextBoxContentBounds, this.mData.mBGColour);
      // Get text
      String to_draw = $"{this.GetData().mExtraText.mPreText}{this.Text}";
      // Handle text
      while (to_draw.Length > 2 && this.GetData().mFont.MeasureString(to_draw).X > this.GetData().mDetailedBounds.mTextBoxContentBounds.Width)
        to_draw = to_draw.Substring(1, to_draw.Length - 1);
      // Text
      spriteBatch.DrawStringEx(this.GetData().mFont, to_draw, this.GetData().mDetailedBounds.mTextBoxContentBounds.ExtractXYAsXNAVector2(), this.mData.mFGColour);
      // Caret
      if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 >= 500.0 && this.Selected) {
        spriteBatch.Draw(Game1.staminaRect,
                         new Rectangle(this.GetData().mDetailedBounds.mTextBoxContentBounds.X + (Int32)this.GetData().mFont.MeasureString(to_draw).X + 2,
                                       this.GetData().mDetailedBounds.mTextBoxContentBounds.Y + 2,
                                       2,
                                       this.GetData().mFont.GetSize().Y - 4),
                         this.mData.mFGColour);
      }
    }

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks>
    /// <see cref="CustomTextBox"/> implementation:
    /// <para>1. Supresses <paramref name="inputState"/> if the textbox has focus.</para>
    /// <para>2. Updates the textbox's focus state considering <see cref="Data.DetailedBounds.mTextBoxContentBounds"/>.</para>
    /// </remarks>
    public virtual void OnButtonPressed(InputStateEx inputState) {
      if (this.Selected) {
        GlobalVars.gSMAPIHelper.Input.Suppress(inputState.mButton);
        if (inputState.mButton is SButton.Escape or SButton.Enter) {
          this.mData.UpdateCursorStatus(false, new InputStateEx(SButton.Escape));
          this.Selected = false;
        }
      }

      if (inputState.mButton == SButton.MouseLeft) {
        this.Selected = this.GetData().mDetailedBounds.mTextBoxContentBounds.Contains(inputState.mCursorPos);
        if (this.showKeyboard) {
          if (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse) Game1.showTextEntry(this);
          this.showKeyboard = false;
        }
      }
    }

    public virtual void OnButtonReleased(InputStateEx inputState) { }
    public virtual void OnCursorMoved(Vector2 cursorPos) { }
    public virtual void OnMouseClick(InputStateEx inputState) { }
    public virtual void OnGameTick() { }
    public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) { }

    #endregion

    // Overrides:
    #region Overrides

    /// <summary>Calls <see cref="Draw(SpriteBatch)"/>.</summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter", Justification = "Game code")]
    public override void Draw(SpriteBatch spriteBatch, Boolean drawShadow = true) { this.Draw(spriteBatch); }

    #endregion

    #region Stardew Valley Keyboard Subscriber Events

    [UsedImplicitly]
    public new void RecieveTextInput(Char inputChar) {
      if (this.GetData().mHandlers.mInputFilterFunction(inputChar)) this.mText += inputChar;
    }

    [UsedImplicitly]
    public new void RecieveTextInput(String text) {
      String final_string = this.mText;
      this.mText = text.Where(c => this.GetData().mHandlers.mInputFilterFunction(c)).Aggregate(final_string, (s, c) => s + c);
    }

    [UsedImplicitly]
    public new void RecieveCommandInput(Char command) {
      String old_string = this.Text;
      base.RecieveCommandInput(command);
      if (this.Text != old_string) this.GetData().mHandlers.mOnTextChangedHandler?.Invoke(this.Text);
    }

    [UsedImplicitly]
    public new void RecieveSpecialInput(Keys key) {
      String old_string = this.Text;
      base.RecieveSpecialInput(key);
      if (this.Text != old_string) this.GetData().mHandlers.mOnTextChangedHandler?.Invoke(this.Text);
    }

    #endregion

    #endregion

    // Constructors:
    #region Constructors

    public CustomTextBox(Rectangle bounds, Colours colours, String label = "", String preText = "",
                         String text = "", Int32 maxLength = -1, Func<Char, Boolean> inputFilterFunction = null, Action<String> onTextChangedHandler = null)
      : base(null, null, Game1.smallFont, colours.mForegroundColour) {
      // adjust null/empty parameters
      {
        inputFilterFunction ??= _ => true;
        if (maxLength == -1) maxLength = 256;
        if (bounds.Width == -1) bounds.Width = Convert.ToInt32(Game1.smallFont.MeasureString(label).X + 8.0f + Game1.smallFont.MeasureString(new String('T', maxLength + 1)).X);
        if (bounds.Height == -1) bounds.Height = Game1.smallFont.GetSize().Y + 8;
      }

      this.mBounds = bounds;
      this.mData = new Data(colours, this.mBounds, Game1.smallFont, maxLength, new Data.ExtraText(label, preText), new Data.Handlers(inputFilterFunction, onTextChangedHandler));

      // edit some base class properties
      {
        this.X = this.GetData().mDetailedBounds.mTextBoxContentBounds.X;
        this.Y = this.GetData().mDetailedBounds.mTextBoxContentBounds.Y;
        this.Width = this.GetData().mDetailedBounds.mTextBoxContentBounds.Width;
        this.Height = this.GetData().mDetailedBounds.mTextBoxContentBounds.Height;
        this.limitWidth = false;
        this.textLimit = maxLength;
        this.Text = text;
      }
    }

    #endregion

    // IDisposable:
    #region IDisposable

    /// <inheritdoc path="//*[not(self::remarks)]"/>
    /// <remarks><see cref="CustomTextBox"/> implementation calls <see cref="SetVisible(Boolean)"/> with 'false'.</remarks>
    public virtual void Dispose() { this.SetVisible(false); }

    #endregion
  }
}

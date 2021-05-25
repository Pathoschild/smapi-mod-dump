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
using System.Linq;

using ChestEx.LanguageExtensions;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

namespace ChestEx.Types.BaseTypes {
  public class CustomTextBox : TextBox,
                               IDisposable,
                               IKeyboardSubscriber {
    // Protected
  #region Protected

    protected Color               mTextBoxColour;
    protected Func<Char, Boolean> mFilterFunc;
    protected String              mPreText;
    protected String              mLabel;
    protected Color               mLabelColour;
    protected Action<String>      mOnTextChangedHandler;
    protected Rectangle           mLabelBounds;
    protected Rectangle           mTextBounds;

    protected SpriteFont mFont {
      get => this._font;
      set => this._font = value;
    }

    protected Color mTextColour {
      get => this._textColor;
      set => this._textColor = value;
    }

  #endregion

    // Public:
  #region Public

    public static readonly Func<Char, Boolean> gAcceptNumbersOnly           = Char.IsDigit;
    public static readonly Func<Char, Boolean> gAcceptLettersAndNumbersOnly = Char.IsLetterOrDigit;
    public static readonly Func<Char, Boolean> gSVAcceptedInput             = c => c != '|';

    public static readonly Func<Char, Boolean> gAcceptHexOnly = c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';

    public Rectangle mBounds;

    public Rectangle mTextBoxBounds {
      get => new(this.X, this.Y, this.Width, this.Height);
      set {
        this.X      = value.X;
        this.Y      = value.Y;
        this.Width  = value.Width;
        this.Height = value.Height;
      }
    }

    public Boolean mIsVisible { get; private set; }

    public String mText {
      get => this.Text.Substring(0, Math.Min(this.Text.Length, this.mTextMaxLength));
      set {
        String final_string = value.Substring(0, Math.Min(value.Length, this.mTextMaxLength));
        if (this.Text.Equals(final_string, StringComparison.InvariantCultureIgnoreCase)) return;

        this.Text = final_string;
        this.mOnTextChangedHandler?.Invoke(this.Text);
      }
    }

    public Int32 mTextMaxLength {
      get => this.textLimit;
      set => this.textLimit = value == -1 ? 256 : value;
    }

    // Virtuals
  #region Virtuals

    /// <summary>
    /// Base implementation sets '<see cref="mIsVisible"/>' to '<paramref name="isVisible"/>'.
    /// </summary>
    /// <param name="isVisible">Whether this item should be visible.</param>
    public virtual void SetVisible(Boolean isVisible) { this.mIsVisible = isVisible; }

    /// <summary>
    /// Must be called on each event to function.
    /// </summary>
    /// <param name="e">SMAPI generated EventArgs.</param>
    public virtual void OnButtonPressed(ButtonPressedEventArgs e) {
      if (!this.mIsVisible) return;
      if (this.Selected) GlobalVars.gSMAPIHelper.Input.Suppress(e.Button);

      if (e.Button != SButton.MouseLeft) return;
      this.Update();
    }

  #endregion

    // Overrides:
  #region Overrides

    public override void Draw(SpriteBatch spriteBatch, Boolean drawShadow = true) {
      if (!this.mIsVisible) return;

      // Label
      spriteBatch.DrawString(this.mFont, this.mLabel, this.mLabelBounds.ExtractXYAsXNAVector2(), this.mLabelColour);
      // Box outline
      spriteBatch.Draw(Game1.fadeToBlackRect, this.mTextBoxBounds, this.mTextBoxColour.MultKeepAlpha(0.5f));
      // Box
      spriteBatch.Draw(Game1.fadeToBlackRect,
                       new Rectangle(this.mTextBoxBounds.X + 4, this.mTextBoxBounds.Y + 4, this.mTextBoxBounds.Width - 8, this.mTextBoxBounds.Height - 8),
                       this.mTextBoxColour);
      // Handle text
      String to_draw = $"{this.mPreText}{this.Text}";
      while (to_draw.Length > 2 && this.mFont.MeasureString(to_draw).X > this.mTextBounds.Width) // handle extra text
        to_draw = to_draw.Substring(1, to_draw.Length - 1);
      // Text
      spriteBatch.DrawString(this.mFont, to_draw, this.mTextBounds.ExtractXYAsXNAVector2(), this.mTextColour);
      // Caret
      if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 >= 500.0 && this.Selected) {
        spriteBatch.Draw(Game1.staminaRect,
                         new Rectangle(this.mTextBounds.X + (Int32)this.mFont.MeasureString(to_draw).X + 2, this.mTextBounds.Y, 2, this.mTextBounds.Height),
                         this.mTextColour);
      }
    }

  #endregion

  #region Stardew Valley Keyboard Subscriber Events

    [UsedImplicitly]
    public new void RecieveTextInput(Char inputChar) {
      if (this.mFilterFunc(inputChar)) this.mText += inputChar;
    }

    [UsedImplicitly]
    public new void RecieveTextInput(String text) {
      String final_string = this.mText;
      this.mText = text.Where(c => this.mFilterFunc(c)).Aggregate(final_string, (s, c) => s + c);
    }

    [UsedImplicitly]
    public new void RecieveCommandInput(Char command) {
      String old_string = this.Text;
      base.RecieveCommandInput(command);
      if (this.Text != old_string) this.mOnTextChangedHandler?.Invoke(this.Text);
    }

    [UsedImplicitly]
    public new void RecieveSpecialInput(Keys key) {
      String old_string = this.Text;
      base.RecieveSpecialInput(key);
      if (this.Text != old_string) this.mOnTextChangedHandler?.Invoke(this.Text);
    }

  #endregion

  #endregion

    // Constructors:
  #region Constructors

    public CustomTextBox(Rectangle           bounds,                     Color          textColour,                Color  textBoxColour, String text        = "",
                         String              preText             = "",   Int32          maxLength            = -1, String label = "",    Color  labelColour = default,
                         Func<Char, Boolean> inputFilterFunction = null, Action<String> onTextChangedHandler = null) : base(null, null, Game1.smallFont, textColour) {
      this.limitWidth = false;

      this.mLabelColour   = labelColour == default ? textBoxColour : labelColour;
      this.mTextBoxColour = textBoxColour;
      this.mFilterFunc    = inputFilterFunction ?? (_ => true);
      this.mPreText       = preText;
      this.mLabel         = label;
      this.mTextMaxLength = maxLength;
      this.mText          = text;

      this.mOnTextChangedHandler = onTextChangedHandler;

      Point label_size                       = String.IsNullOrWhiteSpace(label) ? Point.Zero : (this.mFont.MeasureString(label) + new Vector2(8.0f, 0.0f)).AsXNAPoint();
      if (bounds.Width == -1) bounds.Width   = (Int32)this.mFont.MeasureString(new String('T', this.mTextMaxLength + 1)).X;
      if (bounds.Height == -1) bounds.Height = (Int32)this.mFont.MeasureString("T").Y + 8;

      this.mBounds = new Rectangle(bounds.X,
                                   bounds.Y,
                                   // label width:
                                   label_size.X
                                   +
                                   // textbox text width:
                                   bounds.Width,
                                   bounds.Height);
      this.mTextBoxBounds = new Rectangle(bounds.X + label_size.X, bounds.Y, bounds.Width, bounds.Height);
      this.mTextBounds    = new Rectangle(this.mTextBoxBounds.X + 8, this.mTextBoxBounds.Center.Y - 16, this.mTextBoxBounds.Width - 16, (Int32)this.mFont.MeasureString("T").Y - 4);
      this.mLabelBounds   = new Rectangle(this.mBounds.X, this.mTextBounds.Y, label_size.X, this.mTextBounds.Height);
    }

  #endregion

    // IDisposable:
  #region IDisposable

    public virtual void Dispose() { this.SetVisible(false); }

  #endregion
  }
}

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
  public partial class CustomTextBox {
    public class Data : CustomComponentData {
      public class DetailedBounds {
        private const Int32 CONST_CONTENT_PADDING = 8;

        public Rectangle mLabelBounds { get; }
        public Rectangle mTextBoxBounds { get; }
        public Rectangle mTextBoxContentBounds { get; }

        public DetailedBounds(Rectangle bounds, SpriteFont font, String label) {
          Point label_size = String.IsNullOrWhiteSpace(label) ? Point.Zero : (font.MeasureString(label) + new Vector2(8.0f, 0.0f)).AsXNAPoint();

          this.mTextBoxContentBounds = new Rectangle(bounds.X + label_size.X + CONST_CONTENT_PADDING / 2,
                                                     bounds.Y + CONST_CONTENT_PADDING / 2,
                                                     bounds.Width - label_size.X - CONST_CONTENT_PADDING,
                                                     bounds.Height - CONST_CONTENT_PADDING);
          this.mTextBoxBounds = this.mTextBoxContentBounds.GetTextureBoxRectangle(CONST_CONTENT_PADDING / 2, CONST_BORDER_SCALE);
          this.mLabelBounds = new Rectangle(bounds.X, this.mTextBoxContentBounds.Y, label_size.X, this.mTextBoxContentBounds.Height);
        }
      }

      public class ExtraText {
        public String mLabel { get; }
        public String mPreText { get; }

        public ExtraText(String label, String preText) {
          this.mLabel = label;
          this.mPreText = preText;
        }
      }

      public class Handlers {
        public Func<Char, Boolean> mInputFilterFunction { get; }
        public Action<String> mOnTextChangedHandler { get; }

        public Handlers(Func<Char, Boolean> inputFilterFunction, Action<String> onTextChangedHandler) {
          this.mInputFilterFunction = inputFilterFunction;
          this.mOnTextChangedHandler = onTextChangedHandler;
        }
      }

      public DetailedBounds mDetailedBounds { get; }
      public ExtraText mExtraText { get; }
      public SpriteFont mFont { get; }
      public Handlers mHandlers { get; }
      public Int32 mMaxLength { get; }

      public override void UpdateCursorStatus(Boolean isCursorInBounds, InputStateEx inputState = null) {
        isCursorInBounds = this.mDetailedBounds.mTextBoxContentBounds.Contains(InputStateEx.gCursorPos);
        if (inputState is not null)
          this.mIsActive = isCursorInBounds && inputState.mLastButtonState is SButtonState.Pressed or SButtonState.Held && inputState.mButtonState == SButtonState.Released;
        base.UpdateCursorStatus(isCursorInBounds, inputState);
      }

      public Data(Colours colours, Rectangle completeBounds, SpriteFont font, Int32 maxLength,
                  ExtraText extraText, Handlers handlers)
        : base(colours, 0.86f + completeBounds.Y / 20000.0f, String.Empty) {
        this.mDetailedBounds = new DetailedBounds(completeBounds, font, extraText.mLabel);
        this.mExtraText = extraText;
        this.mFont = font;
        this.mHandlers = handlers;
        this.mMaxLength = maxLength;
      }
    }
  }
}

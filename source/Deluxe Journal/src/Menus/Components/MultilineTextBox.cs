/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A text editor style TextBox.</summary>
    public class MultilineTextBox : TextBox
    {
        private const double HeldDelay = 200.0;
        private const double RepeatDelay = 40.0;

        public readonly ScrollComponent scrollComponent;

        private Rectangle _bounds;
        private StringBuilder _text;
        private Dictionary<char, SpriteFont.Glyph> _glyphs;
        private Point _caretPosition;
        private int _index;
        private bool _updateCaret;

        private KeyboardState _oldKeyboardState;
        private Keys _arrowKey;
        private double _heldTimer;
        private double _repeatTimer;

        public Rectangle Bounds => _bounds;

        /// <summary>
        /// Replaces the <see cref="TextBox.Text"/> property (sort of a hack because <see cref="TextBox.Text"/>
        /// is not marked virtual).
        /// </summary>
        /// <remarks>
        /// IMPORTANT: Do NOT use the <see cref="TextBox.Text"/> property. It will not behave correctly.
        /// </remarks>
        public string MultilineText
        {
            get
            {
                return _text.ToString().Replace("\r", string.Empty);
            }

            set
            {
                _text.Clear();

                foreach (char c in value)
                {
                    if (_font.Characters.Contains(c))
                    {
                        _text.Append(c);
                    }
                }

                _index = _text.Length;
                WrapText();
            }
        }

        /// <summary>Accesses the raw text including formatting characters.</summary>
        public string RawText
        {
            get
            {
                return _text.ToString();
            }

            set
            {
                _text.Clear();
                _text.Append(value);
                _index = _text.Length;
                WrapText();
            }
        }

        public MultilineTextBox(Rectangle bounds, Texture2D? textBoxTexture, Texture2D? caretTexture, SpriteFont font, Color textColor)
            : base(textBoxTexture, caretTexture, font, textColor)
        {
            _bounds = bounds;
            _text = new StringBuilder();
            _glyphs = _font.GetGlyphs();
            _caretPosition = new Point(0);
            _index = 0;
            _heldTimer = 0.0;
            _repeatTimer = 0.0;

            Rectangle scrollBarBounds = default;
            scrollBarBounds.X = _bounds.X + _bounds.Width + 48;
            scrollBarBounds.Y = _bounds.Y + 116;
            scrollBarBounds.Width = 24;
            scrollBarBounds.Height = _bounds.Height - 152;

            Rectangle scrollContentBounds = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width + 8, _bounds.Height);
            scrollContentBounds = Utility.ConstrainScissorRectToScreen(scrollContentBounds);
            scrollContentBounds.Height = (scrollContentBounds.Height / _font.LineSpacing) * _font.LineSpacing;

            scrollComponent = new ScrollComponent(scrollBarBounds, scrollContentBounds, _font.LineSpacing, true);
        }

        public void SetFont(SpriteFont font)
        {
            _font = font;
            _glyphs = _font.GetGlyphs();
        }

        public void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            scrollComponent.ReceiveLeftClick(x, y, playSound);
        }

        public void LeftClickHeld(int x, int y)
        {
            scrollComponent.LeftClickHeld(x, y);
        }

        public void ReleaseLeftClick(int x, int y)
        {
            scrollComponent.ReleaseLeftClick(x, y);
        }

        public void ReceiveScrollWheelAction(int direction)
        {
            scrollComponent.Scroll(direction);
        }

        public void TryHover(int x, int y)
        {
            scrollComponent.TryHover(x, y);
        }

        public void MoveCaretToPoint(int x, int y)
        {
            MoveCaretToPoint(new Point(x - _bounds.X, y - _bounds.Y));
        }

        private void MoveCaretToPoint(Point target)
        {
            if (_text.Length == 0)
            {
                _caretPosition = new Point(0);
                return;
            }
            else if (target.Y < 0)
            {
                target.Y = 0;
                scrollComponent.Scroll(1);
                MoveCaretToPoint(target);
                return;
            }
            else if (target.Y > _bounds.Height)
            {
                target.Y = _bounds.Height;
                scrollComponent.Scroll(-1);
                MoveCaretToPoint(target);
                return;
            }

            target.X = Math.Min(Math.Max(target.X, 0), _bounds.Width);
            target.Y = (target.Y / _font.LineSpacing) * _font.LineSpacing;
            _caretPosition.X = 0;
            _caretPosition.Y = target.Y;
            _index = 0;

            bool first = true;
            float lineWidth = 0;
            int lineCount = (target.Y + scrollComponent.ScrollAmount) / _font.LineSpacing;

            for (int i = 0; i < _text.Length && lineCount > 0; ++i)
            {
                if (_text[i] == '\n')
                {
                    lineCount--;
                    _index = i + 1;
                }
            }

            if (lineCount > 0)
            {
                _index = _text.Length;
                MoveCaretToIndex();
                return;
            }

            for (; _index < _text.Length; ++_index)
            {
                char c = _text[_index];

                if (c == '\r' || c == '\n')
                {
                    break;
                }
                if (_font.Characters.Contains(c))
                {
                    SpriteFont.Glyph glyph = _glyphs[c];
                    float charWidth;

                    if (first)
                    {
                        charWidth = Math.Max(glyph.LeftSideBearing, 0) + glyph.Width + glyph.RightSideBearing;
                        first = false;
                    }
                    else
                    {
                        charWidth = _font.Spacing + glyph.WidthIncludingBearings;
                    }

                    if (lineWidth + (charWidth / 2f) > target.X)
                    {
                        break;
                    }

                    lineWidth += charWidth;
                }
            }

            _caretPosition.X = (int)lineWidth;
        }

        private void MoveCaretToIndex()
        {
            bool first = true;
            float lineWidth = 0;
            int lineStart = 0;
            int i;

            _caretPosition.X = 0;
            _caretPosition.Y = -scrollComponent.ScrollAmount;

            for (i = 0; i < _index; ++i)
            {
                if (_text[i] == '\n')
                {
                    _caretPosition.Y += _font.LineSpacing;
                    lineStart = i + 1;
                }
            }

            for (i = lineStart; i < _index; ++i)
            {
                char c = _text[i];

                if (_font.Characters.Contains(c))
                {
                    SpriteFont.Glyph glyph = _glyphs[c];

                    if (first)
                    {
                        lineWidth += Math.Max(glyph.LeftSideBearing, 0) + glyph.Width + glyph.RightSideBearing;
                        first = false;
                    }
                    else
                    {
                        lineWidth += _font.Spacing + glyph.WidthIncludingBearings;
                    }
                }
            }

            _caretPosition.X = (int)lineWidth;
            _updateCaret = false;
        }

        public virtual void Update(GameTime time)
        {
            KeyboardState keyboardState = Game1.input.GetKeyboardState();

            UpdateArrowKeyInput(keyboardState, time);
            _oldKeyboardState = keyboardState;

            if (_updateCaret)
            {
                MoveCaretToIndex();
            }
        }

        private void UpdateArrowKeyInput(KeyboardState keyboardState, GameTime time)
        {
            Keys key;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                key = Keys.Left;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                key = Keys.Right;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                key = Keys.Up;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                key = Keys.Down;
            }
            else
            {
                _arrowKey = Keys.None;
                return;
            }

            if (_arrowKey == Keys.None || _oldKeyboardState.IsKeyUp(key))
            {
                _arrowKey = key;
                _heldTimer = HeldDelay;
                _repeatTimer = 0;
            }

            if (_arrowKey != key)
            {
                return;
            }
            else if (_repeatTimer <= 0)
            {
                ArrowKeyInput(key);
                _repeatTimer = RepeatDelay;
            }
            else if (_heldTimer > 0)
            {
                _heldTimer -= time.ElapsedGameTime.TotalMilliseconds;
            }
            else if (_repeatTimer > 0)
            {
                _repeatTimer -= time.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public override void Draw(SpriteBatch b, bool drawShadow = true)
        {
            string text = _text.ToString();
            Vector2 textPosition = new Vector2(_bounds.X, _bounds.Y - scrollComponent.ScrollAmount);
            bool caretVisible = _arrowKey != Keys.None || !(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 < 500.0);
            int caretY = _caretPosition.Y + (int)(_font.LineSpacing * 0.05f);
            int caretHeight = (int)(_font.LineSpacing * 0.9f);

            scrollComponent.BeginScissorTest(b);

            if (drawShadow)
            {
                Utility.drawTextWithShadow(b, text, _font, textPosition, _textColor);
            }
            else
            {
                b.DrawString(_font, text, textPosition, _textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
            }

            if (caretVisible && Selected)
            {
                b.Draw(Game1.staminaRect, new Rectangle(_bounds.X + _caretPosition.X, _bounds.Y + caretY, 4, caretHeight), _textColor);
            }

            scrollComponent.EndScissorTest(b);
            scrollComponent.DrawScrollBar(b);
        }

        public override void RecieveTextInput(char inputChar)
        {
            if (Selected && (textLimit == -1 || _text.Length < textLimit))
            {
                _text.Insert(_index++, inputChar);
                WrapText();
            }
        }

        public override void RecieveTextInput(string text)
        {
            if (Selected && (textLimit == -1 || _text.Length < textLimit))
            {
                _text.Insert(_index, text);
                _index += text.Length;
                WrapText();
            }
        }

        public override void RecieveCommandInput(char command)
        {
            if (!Selected)
            {
                return;
            }

            switch (command)
            {
                case '\b':
                    if (_text.Length <= 0 || _index <= 0)
                    {
                        break;
                    }

                    if (_index > 1 && _text[_index - 2] == '\r')
                    {
                        _index -= 2;
                        _text.Remove(_index, 2);
                    }
                    else
                    {
                        _text.Remove(--_index, 1);
                    }

                    WrapText();
                    break;
                case '\r':
                    _text.Insert(_index++, '\r');
                    WrapText();
                    break;
                case '\t':
                    break;
            }
        }

        protected virtual void ArrowKeyInput(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                    MoveCaretToPoint(new Point(_caretPosition.X, _caretPosition.Y - _font.LineSpacing));
                    break;
                case Keys.Down:
                    MoveCaretToPoint(new Point(_caretPosition.X, _caretPosition.Y + _font.LineSpacing));
                    break;
                case Keys.Left:
                    if (_index > 0 && _text[--_index] == '\n' && _index > 0 && _text[_index - 1] == '\r')
                    {
                        _index--;
                    }
                    MoveCaretToIndex();
                    break;
                case Keys.Right:
                    if (_index < _text.Length && _text[_index++] == '\r' && _index < _text.Length)
                    {
                        _index++;
                    }
                    MoveCaretToIndex();
                    break;
            }
        }

        private void WrapText()
        {
            StringBuilder wrapped = new StringBuilder(_text.Capacity);
            float lineWidth = 0;
            int linesAdded = 0;
            int contentHeight = _font.LineSpacing;
            _updateCaret = true;

            for (int i = 0; i < _text.Length; ++i)
            {
                char c = _text[i];

                if (c == '\r')
                {
                    if (i < _index)
                    {
                        linesAdded++;
                    }

                    lineWidth = 0;
                    contentHeight += _font.LineSpacing;
                    wrapped.Append(c);
                    wrapped.Append('\n');
                }
                else if (c == '\n')
                {
                    if (i < _index)
                    {
                        linesAdded--;
                    }
                }
                else if (_font.Characters.Contains(c))
                {
                    SpriteFont.Glyph glyph = _glyphs[c];
                    lineWidth += glyph.LeftSideBearing + glyph.Width;

                    if (lineWidth > _bounds.Width)
                    {
                        if (i < _index)
                        {
                            linesAdded++;
                        }

                        lineWidth = Math.Max(glyph.LeftSideBearing, 0) + glyph.Width + glyph.RightSideBearing;
                        contentHeight += _font.LineSpacing;
                        wrapped.Append('\n');
                    }
                    else
                    {
                        lineWidth += glyph.RightSideBearing + _font.Spacing;
                    }

                    wrapped.Append(c);
                }
            }

            _text = wrapped;
            _index += linesAdded;
            scrollComponent.ContentHeight = contentHeight;
            scrollComponent.ScrollAmount += linesAdded * _font.LineSpacing;

            if (_index > _text.Length)
            {
                _index = _text.Length;
            }
            else if (_index < 0)
            {
                _index = 0;
            }
        }
    }
}

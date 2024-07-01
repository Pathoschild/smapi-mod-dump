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
using DeluxeJournal.Util;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A text editor style TextBox.</summary>
    public class MultilineTextBox : TextBox
    {
        private const double HeldDelay = 200.0;
        private const double RepeatDelay = 40.0;

        private readonly StringBuilder _text;
        private Rectangle _bounds;
        private SpriteFontTools _fontTools;
        private Point _caretPosition;
        private int _index;
        private string _displayText;

        private KeyboardState _oldKeyboardState;
        private Keys _repeatKey;
        private double _heldTimer;
        private double _repeatTimer;

        /// <summary>Bounding box for the text region.</summary>
        public Rectangle Bounds => _bounds;

        /// <summary>Scrolling controller.</summary>
        public ScrollComponent ScrollComponent { get; }

        /// <summary>
        /// Replaces the <see cref="TextBox.Text"/> property (sort of a hack because <see cref="TextBox.Text"/>
        /// cannot be overridden).
        /// </summary>
        /// <remarks>
        /// IMPORTANT: Do NOT use the base <see cref="TextBox.Text"/> property. It will expose the unused hidden value.
        /// Always explicitly cast to <see cref="MultilineTextBox"/> before accessing.
        /// </remarks>
        public new string Text
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
            _text = new();
            _displayText = string.Empty;
            _fontTools = new(font, string.Empty);
            _caretPosition = Point.Zero;

            Rectangle scrollBarBounds = default;
            scrollBarBounds.X = _bounds.X + _bounds.Width + 48;
            scrollBarBounds.Y = _bounds.Y + 116;
            scrollBarBounds.Width = 24;
            scrollBarBounds.Height = _bounds.Height - 152;

            Rectangle scrollContentBounds = new(_bounds.X, _bounds.Y, _bounds.Width + 8, _bounds.Height);
            scrollContentBounds = Utility.ConstrainScissorRectToScreen(scrollContentBounds);
            scrollContentBounds.Height = scrollContentBounds.Height / _font.LineSpacing * _font.LineSpacing;

            ScrollComponent = new ScrollComponent(scrollBarBounds, scrollContentBounds, _font.LineSpacing, true);
            ScrollComponent.OnScroll += (_) => BuildDisplayText();
        }

        public void SetFont(SpriteFont font)
        {
            _fontTools = new(_font = font);
        }

        public void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            ScrollComponent.ReceiveLeftClick(x, y, playSound);
        }

        public void LeftClickHeld(int x, int y)
        {
            ScrollComponent.LeftClickHeld(x, y);
        }

        public void ReleaseLeftClick(int x, int y)
        {
            ScrollComponent.ReleaseLeftClick(x, y);
        }

        public void ReceiveScrollWheelAction(int direction)
        {
            bool moveCaret = (direction > 0 && ScrollComponent.ScrollAmount > 0) || (direction < 0 && ScrollComponent.GetPercentScrolled() < 1f);
            ScrollComponent.Scroll(direction);

            if (moveCaret)
            {
                MoveCaretToPoint(new(_caretPosition.X, _caretPosition.Y + _font.LineSpacing * Math.Sign(direction)));
            }
        }

        public void TryHover(int x, int y)
        {
            ScrollComponent.TryHover(x, y);
        }

        public void MoveCaretToPoint(int x, int y)
        {
            MoveCaretToPoint(new(x - _bounds.X, y - _bounds.Y));
        }

        private void MoveCaretToPointAndScroll(Point target)
        {
            if (_text.Length == 0)
            {
                _caretPosition = Point.Zero;
                return;
            }
            else if (target.Y < 0)
            {
                target.Y = 0;
                ScrollComponent.Scroll(1);
            }
            else if (target.Y > ScrollComponent.ContentBounds.Height - _font.LineSpacing)
            {
                target.Y = ScrollComponent.ContentBounds.Height - _font.LineSpacing;
                ScrollComponent.Scroll(-1);
            }

            MoveCaretToPoint(target);
        }

        private void MoveCaretToPoint(Point target)
        {
            if (_text.Length == 0)
            {
                _caretPosition = Point.Zero;
                return;
            }

            target.X = Math.Clamp(target.X, 0, _bounds.Width);
            target.Y = target.Y / _font.LineSpacing * _font.LineSpacing;
            _caretPosition.X = 0;
            _caretPosition.Y = target.Y;
            _index = 0;

            bool first = true;
            float lineWidth = 0;
            int lineCount = (target.Y + ScrollComponent.ScrollAmount) / _font.LineSpacing;

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
                    SpriteFont.Glyph glyph = _fontTools.Glyphs[c];
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
            _caretPosition.Y = -ScrollComponent.ScrollAmount;

            for (i = 0; i < _index; ++i)
            {
                if (_text[i] == '\n')
                {
                    _caretPosition.Y += _font.LineSpacing;
                    lineStart = i + 1;
                }
            }

            if (_caretPosition.Y < 0)
            {
                ScrollComponent.ScrollAmount += _caretPosition.Y;
                _caretPosition.Y = 0;
            }
            else if (_caretPosition.Y > ScrollComponent.ContentBounds.Height - _font.LineSpacing)
            {
                ScrollComponent.ScrollAmount += _caretPosition.Y - ScrollComponent.ContentBounds.Height + _font.LineSpacing;
                _caretPosition.Y = ScrollComponent.ContentBounds.Height - _font.LineSpacing;
            }

            for (i = lineStart; i < _index; ++i)
            {
                char c = _text[i];

                if (_font.Characters.Contains(c))
                {
                    SpriteFont.Glyph glyph = _fontTools.Glyphs[c];

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
        }

        public virtual void Update(GameTime time)
        {
            KeyboardState keyboardState = Game1.input.GetKeyboardState();

            UpdateSpecialKeyInput(keyboardState, time);
            _oldKeyboardState = keyboardState;
        }

        private void UpdateSpecialKeyInput(KeyboardState keyboardState, GameTime time)
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
            else if (keyboardState.IsKeyDown(Keys.Delete))
            {
                key = Keys.Delete;
            }
            else
            {
                _repeatKey = Keys.None;
                return;
            }

            if (_repeatKey == Keys.None || _oldKeyboardState.IsKeyUp(key))
            {
                _repeatKey = key;
                _heldTimer = HeldDelay;
                _repeatTimer = 0;
            }

            if (_repeatKey != key)
            {
                return;
            }
            else if (_repeatTimer <= 0)
            {
                SpecialKeyInput(key);
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
            Vector2 textPosition = new(_bounds.X, _bounds.Y);
            bool caretVisible = _repeatKey != Keys.None || !(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 < 500.0);
            int caretY = _caretPosition.Y + (int)(_font.LineSpacing * 0.05f);
            int caretHeight = (int)(_font.LineSpacing * 0.9f);

            ScrollComponent.BeginScissorTest(b);

            if (drawShadow)
            {
                Utility.drawTextWithShadow(b, _displayText, _font, textPosition, _textColor);
            }
            else
            {
                b.DrawString(_font, _displayText, textPosition, _textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
            }

            if (caretVisible && Selected)
            {
                b.Draw(Game1.staminaRect, new Rectangle(_bounds.X + _caretPosition.X, _bounds.Y + caretY, 4, caretHeight), _textColor);
            }

            ScrollComponent.EndScissorTest(b);
            ScrollComponent.DrawScrollBar(b);
        }

        public override void RecieveTextInput(char inputChar)
        {
            if (Selected && (textLimit == -1 || _text.Length < textLimit))
            {
                _text.Insert(_index++, inputChar);
                WrapText(1);
            }
        }

        public override void RecieveTextInput(string text)
        {
            if (Selected && (textLimit == -1 || _text.Length < textLimit))
            {
                _text.Insert(_index, text);
                _index += text.Length;
                WrapText(text.Length);
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
                    if (_index <= 0 || _text.Length == 0)
                    {
                        break;
                    }

                    if (_index > 1 && _text[_index - 1] == '\n')
                    {
                        _index -= 2;
                        _text.Remove(_index, 2);
                        WrapText(-2);
                    }
                    else
                    {
                        _text.Remove(--_index, 1);
                        WrapText(-1);
                    }
                    break;
                case '\r':
                    _text.Insert(_index, "\r\n");
                    _index += 2;
                    WrapText();
                    break;
                case '\t':
                    break;
            }
        }

        protected virtual void SpecialKeyInput(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                    MoveCaretToPointAndScroll(new(_caretPosition.X, _caretPosition.Y - _font.LineSpacing));
                    break;
                case Keys.Down:
                    MoveCaretToPointAndScroll(new(_caretPosition.X, _caretPosition.Y + _font.LineSpacing));
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
                case Keys.Delete:
                    if (_index < _text.Length)
                    {
                        _index++;
                        RecieveCommandInput('\b');
                    }
                    break;
            }
        }

        private void BuildDisplayText()
        {
            int minLine = ScrollComponent.ScrollAmount / _font.LineSpacing;
            int maxLine = minLine + ScrollComponent.ContentBounds.Height / _font.LineSpacing;
            int currentLine = 0;
            int startIndex = minLine == 0 ? 0 : -1;
            int i;

            for (i = 0; i < _text.Length; i++)
            {
                if (_text[i] == '\n' && ++currentLine == minLine && startIndex < 0)
                {
                    startIndex = i + 1;
                }

                if (currentLine >= maxLine)
                {
                    break;
                }
            }

            _displayText = startIndex < 0 || startIndex >= i ? string.Empty : _text.ToString(startIndex, i - startIndex);
        }

        private void WrapText(int indexDelta = 0)
        {
            bool indexAtNewline = _index >= 0 && (_index == _text.Length || _text[_index] == '\n' || _text[_index] == '\r');
            int newlines = _fontTools.Wrap(_text, _bounds.Width);

            ScrollComponent.ContentHeight = (newlines + 1) * _font.LineSpacing;
            _index = Math.Clamp(_index, 0, _text.Length);

            if (indexAtNewline && indexDelta > 0 && _index < _text.Length && !(_text[_index] == '\n' || _text[_index] == '\r'))
            {
                _index++;
            }

            MoveCaretToIndex();
            BuildDisplayText();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Text;

namespace InputFix
{
    public class TextBox_ : TextBox, ITextBox
    {
        #region Vars

        public new virtual int X
        {
            get
            {
                return base.X;
            }
            set
            {
                if (value != base.X)
                {
                    base.X = value;
                }
                DrawOrigin.X = base.X + 16f;
            }
        }

        public new virtual int Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                if (value != base.Y)
                {
                    base.Y = value;
                }
                DrawOrigin.Y = base.Y + 8f;
            }
        }

        public new string Text
        {
            get
            {
                return GetText();
            }
            set
            {
                SetText(value);
            }
        }

        protected Vector2 DrawOrigin = new Vector2(16f, 8f);

        //just for arrow key/mouse selection, dont use it to handle text change
        private SelState selState = SelState.SEL_AE_NONE;

        private StringBuilder text = new StringBuilder();

        #endregion Vars

        public new event TextBoxEvent OnBackspacePressed;

        public TextBox_(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor) : base(textBoxTexture, caretTexture, font, textColor)
        {
        }

        #region ITextBox

        protected Acp acp = new Acp();

        public virtual Acp GetSelection()
        {
            return acp;
        }

        public SelState GetSelState()
        {
            switch (selState)
            {
                case SelState.SEL_AE_START:
                case SelState.SEL_AE_END:
                    if (acp.Start == acp.End)
                    {
                        selState = SelState.SEL_AE_NONE;
                    }
                    break;

                case SelState.SEL_AE_NONE:
                    if (acp.Start != acp.End)
                    {
                        selState = SelState.SEL_AE_START;
                    }
                    break;

                default:
                    break;
            }
            return selState;
        }

        public virtual void SetSelection(int acpStart, int acpEnd)
        {
            acp.Start = acpStart;
            acp.End = acpEnd;

            int len = GetTextLength();
            if (acp.Start > len || acp.End > len)//out of range
                acp.End = acp.Start = len;//reset caret to the tail
        }

        public void SetSelState(SelState state)
        {
            selState = state;
        }

        public virtual string GetText()
        {
            return text.ToString();
        }

        public virtual RECT GetTextExt(Acp _acp)
        {
            var len = GetTextLength();
            if (_acp.End > len)
                _acp.End = len;
            RECT rect = new RECT();

            string text = PasswordBox ? new string('*', GetTextLength()) : GetText();
            //acpend may smaller than acpstart
            var start = Math.Min(_acp.Start, _acp.End);
            var end = Math.Max(_acp.Start, _acp.End);

            rect.left += (int)(Font.MeasureString(text.Substring(0, start)).X + DrawOrigin.X);
            rect.top = (int)DrawOrigin.Y;

            var vec_text = Font.MeasureString(text.Substring(start, end - start));
            rect.right = rect.left + (int)vec_text.X;
            rect.bottom = rect.top + (int)vec_text.Y;

            return rect;
        }

        public virtual int GetTextLength()
        {
            return text.Length;
        }

        public virtual void SetText(string str)
        {
            text.Clear();
            text.Append(str);
            acp.Start = acp.End = text.Length;
        }

        public virtual void ReplaceSelection(string _text)
        {
            if (acp.End != acp.Start)//it means delete and insert text
            {
                var start = Math.Min(acp.Start, acp.End);
                text.Remove(start, Math.Abs(acp.Start - acp.End));
                acp.Start = acp.End = start;
            }
            if (_text != "" && (textLimit == -1 || text.Length + _text.Length < textLimit) && (Font.MeasureString(_text).X + Font.MeasureString(text).X) < Width - 16)
            {
                text.Insert(acp.Start, _text);
                acp.End += _text.Length;
                //IME input dont play sound, english input sound is handled at IKeyboadSubscriber
                //Game1.playSound("cowboy_monsterhit");//TSF may replace some word, which will make the sound strange
            }
            acp.Start = acp.End;
        }

        public virtual Acp GetAcpByRange(RECT rect)
        {
            Acp result = new Acp();
            var text = GetText();
            float width = DrawOrigin.X;
            if (rect.left <= X + Width && rect.top <= Y + Height && rect.right >= X && rect.bottom >= Y)//check if overlap textbox
            {
                if (rect.right <= width)
                {
                    result.Start = result.End = 0;
                }
                else if (rect.left >= Font.MeasureString(text).X + width)
                {
                    result.Start = result.End = GetTextLength();
                }
                else
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        var char_x = Font.MeasureString(text[i].ToString()).X;
                        width += char_x;
                        if (width > rect.left)
                        {
                            result.Start += ((width - char_x / 2) <= rect.left) ? 1 : 0;//divide char from middle, if selection is on the left part, we dont sel this word
                            result.End = result.Start;
                            result.End += (((width - char_x / 2) < rect.right) && ((width - char_x / 2) > rect.left)) ? 1 : 0;
                            if (width >= rect.right)
                            {
                                return result;
                            }
                            for (i++; i < text.Length; i++)
                            {
                                char_x = Font.MeasureString(text[i].ToString()).X;
                                width += char_x;
                                if (width > rect.right)
                                {
                                    result.End += ((width - char_x / 2) < rect.right) ? 1 : 0;//divide char from middle, if selection is on the left part, we dont sel this word
                                    return result;
                                }
                                result.End++;
                            }
                            break;
                        }
                        result.Start++;
                    }
                }
            }
            else
            {
                result.Start = result.End = -1;
            }
            return result;
        }

        public bool AllowIME
        {
            get
            {
                return !numbersOnly;
            }
        }

        #endregion ITextBox

        #region IKeyboardSubscriber

        public override void RecieveCommandInput(char command)//IME will handle key event first, so these method just for english input(if it is using IME, we need to notify TSF)
        {
            switch (command)
            {
                case '\b':
                    if (acp.End == acp.Start && acp.Start > 0)//if not, means it alradey have something selected, we just delete it
                    {
                        acp.Start--;//it selected nothing, reduce end to delete a char
                    }
                    if (acp.End != acp.Start)
                    {
                        ReplaceSelection("");
                        if (Game1.gameMode != 3)
                        {
                            Game1.playSound("tinyWhip");
                            return;
                        }
                    }
                    OnBackspacePressed?.Invoke(this);
                    break;

                case '\r':
                case '\t':
                    base.RecieveCommandInput(command);
                    break;

                default:
                    break;
            }
        }

        public new void RecieveSpecialInput(Keys key)//IME will handle key event first, so these method just for english input(if it is using IME, we need to notify TSF)
        {
            var shiftPressed = (ModEntry._helper.Input.IsDown(StardewModdingAPI.SButton.LeftShift) || ModEntry._helper.Input.IsDown(StardewModdingAPI.SButton.RightShift));
            switch (key)
            {
                case Keys.Left:
                    if (acp.Start > 0 || acp.End > 0)
                    {
                        switch (GetSelState())
                        {
                            case SelState.SEL_AE_START:
                                if (shiftPressed)
                                {
                                    if (acp.End > 0)
                                        acp.End--;
                                    else
                                        return;
                                }
                                else
                                {
                                    acp.End = acp.Start;
                                }
                                break;

                            case SelState.SEL_AE_END:
                                if (shiftPressed)
                                {
                                    if (acp.Start > 0)
                                        acp.Start--;
                                    else
                                        return;
                                }
                                else
                                {
                                    acp.End = acp.Start;
                                }
                                break;

                            case SelState.SEL_AE_NONE:
                                if (shiftPressed)
                                {
                                    acp.Start--;
                                    selState = SelState.SEL_AE_END;
                                }
                                else
                                {
                                    acp.End = acp.Start = --acp.Start;
                                }
                                break;
                        }
                        return;
                    }
                    break;

                case Keys.Right:
                    var len = GetTextLength();
                    if (acp.Start < len || acp.End < len)
                    {
                        switch (GetSelState())
                        {
                            case SelState.SEL_AE_START:
                                if (shiftPressed)
                                {
                                    if (acp.End < len)
                                        acp.End++;
                                    else
                                        return;
                                }
                                else
                                {
                                    acp.Start = acp.End;
                                }
                                break;

                            case SelState.SEL_AE_END:
                                if (shiftPressed)
                                {
                                    if (acp.Start < len)
                                        acp.Start++;
                                    else
                                        return;
                                }
                                else
                                {
                                    acp.Start = acp.End;
                                }
                                break;

                            case SelState.SEL_AE_NONE:
                                if (shiftPressed)
                                {
                                    acp.End++;
                                    selState = SelState.SEL_AE_START;
                                }
                                else
                                {
                                    acp.End = acp.Start = ++acp.Start;
                                }
                                break;
                        }
                        return;
                    }
                    break;

                default:
                    break;
            }
            return;
        }

        public override void RecieveTextInput(char inputChar)//IME will handle key event first, so these method just for english input(if it is using IME, we need to notify TSF)
        {
            RecieveTextInput(inputChar.ToString());
        }

        public override void RecieveTextInput(string text)//IME will handle key event first, so these method just for english input(if it is using IME, we need to notify TSF)
        {
            if (Selected && (!numbersOnly || int.TryParse(text, out _)) && (textLimit == -1 || Text.Length < textLimit))
            {
                if (Game1.gameMode != 3)
                    switch (text)
                    {
                        case "\"":
                            return;

                        case "$":
                            Game1.playSound("money");
                            break;

                        case "*":
                            Game1.playSound("hammer");
                            break;

                        case "+":
                            Game1.playSound("slimeHit");
                            break;

                        case "<":
                            Game1.playSound("crystal");
                            break;

                        case "=":
                            Game1.playSound("coin");
                            break;

                        default:
                            Game1.playSound("cowboy_monsterhit");
                            break;
                    }
                ReplaceSelection(text);
            }
        }

        #endregion IKeyboardSubscriber

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, bool drawShadow = true)
        {
            DrawBackGround(spriteBatch);

            float offset = DrawOrigin.X;
            if (Selected)
            {
                DrawByAcp(spriteBatch, new Acp(0, acp.Start), ref offset, TextColor, drawShadow);
                DrawCaret(spriteBatch, ref offset);
                DrawByAcp(spriteBatch, new Acp(acp.Start, GetTextLength()), ref offset, TextColor, drawShadow);
            }
            else
                DrawByAcp(spriteBatch, new Acp(0, GetTextLength()), ref offset, TextColor, drawShadow);
        }

        protected virtual void DrawByAcp(SpriteBatch spriteBatch, Acp acp, ref float offset, Color color, bool drawShadow = true)
        {
            var len = Math.Abs(acp.Start - acp.End);
            var start = Math.Min(acp.Start, acp.End);
            var _text = PasswordBox ? new string('*', len) : text.ToString(start, len);
            spriteBatch.DrawString(Font, _text, new Vector2(offset, DrawOrigin.Y), color);
            offset += Font.MeasureString(_text).X;
        }

        protected virtual void DrawCaret(SpriteBatch spriteBatch, ref float offset, bool drawShadow = true)
        {
            if (acp.End == acp.Start)
            {
                bool caretVisible = DateTime.UtcNow.Millisecond % 1000 >= 500;
                if (caretVisible)
                {
                    spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)offset, (int)DrawOrigin.Y, 4, 32), TextColor);
                }
                offset += 4;
            }
            else
            {
                //Draw selection
                RECT rect = GetTextExt(acp);
                Texture2D selectionRect = new Texture2D(Game1.game1.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                Color[] colors = new Color[1];
                selectionRect.GetData(colors);
                colors[0] = Color.Gray;
                colors[0].A = (byte)(0.6f * 255);
                selectionRect.SetData(colors);
                spriteBatch.Draw(selectionRect, new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top), Color.Gray);
            }
        }

        protected virtual void DrawBackGround(SpriteBatch spriteBatch)
        {
            if (_textBoxTexture != null)
            {
                spriteBatch.Draw(_textBoxTexture, new Rectangle(X, Y, 16, Height), new Rectangle?(new Rectangle(0, 0, 16, Height)), Color.White);
                spriteBatch.Draw(_textBoxTexture, new Rectangle(X + 16, Y, Width - 32, Height), new Rectangle?(new Rectangle(16, 0, 4, Height)), Color.White);
                spriteBatch.Draw(_textBoxTexture, new Rectangle(X + Width - 16, Y, 16, Height), new Rectangle?(new Rectangle(_textBoxTexture.Bounds.Width - 16, 0, 16, Height)), Color.White);
            }
            else
                Game1.drawDialogueBox(X - 32, Y - 112 + 10, Width + 80, Height, false, true, null, false, true, -1, -1, -1);
        }

        #endregion Draw
    }
}
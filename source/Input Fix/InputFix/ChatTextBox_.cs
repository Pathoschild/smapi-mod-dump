/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Text;

namespace InputFix
{
    public class ChatTextBox_ : ChatTextBox, ITextBox
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
                DrawOrigin.X = base.X + 12f;
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
                DrawOrigin.Y = base.Y + 12f;
            }
        }

        public bool AllowIME => true;

        #endregion Vars

        #region ChatTextBox

        public new event TextBoxEvent OnBackspacePressed;

        public ChatTextBox_(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor) : base(textBoxTexture, caretTexture, font, textColor)
        {
        }

        public new void reset()
        {
            SetText("");
        }

        public new void setText(string text)
        {
            SetText(text);
        }

        public new void receiveEmoji(int emoji)
        {
            ReplaceSelection("");
            if (currentWidth + 40f > Width - 66)
            {
                return;
            }
            int index = 0;
            ChatSnippet chatSnippet = new ChatSnippet(emoji);
            for (int i = 0; i < finalText.Count; i++)
            {
                ChatSnippet item = finalText[i];
                index += item.emojiIndex != -1 ? 1 : item.message.Length;
                if (index == acp.Start)//[text message/emoji][caret]
                {
                    finalText.Insert(i + 1, chatSnippet);
                    goto FinalEmoji;
                }
                else if (index > acp.Start)//[text  [caret]   message]
                {
                    var sep_str1 = new ChatSnippet(item.message.Substring(0, acp.Start - (index - item.message.Length)), LocalizedContentManager.CurrentLanguageCode);
                    var sep_str2 = new ChatSnippet(item.message.Substring(acp.Start - (index - item.message.Length)), LocalizedContentManager.CurrentLanguageCode);
                    finalText[i] = sep_str1;
                    finalText.Insert(i + 1, chatSnippet);
                    finalText.Insert(i + 2, sep_str2);
                    goto FinalEmoji;
                }
            }
            finalText.Add(chatSnippet);
        FinalEmoji:
            updateWidth();
            acp.Start++;
            acp.End++;
            return;
        }

        #endregion ChatTextBox

        #region ITextBox

        protected Vector2 DrawOrigin = new Vector2(16f, 8f);

        //just for arrow key/mouse selection, dont use it to handle text change
        private SelState selState = SelState.SEL_AE_NONE;

        private Acp acp = new Acp();

        public void SetText(string str)
        {
            currentWidth = 0;
            finalText.Clear();
            acp.Start = acp.End = 0;
            RecieveTextInput(str);
        }

        public string GetText()
        {
            StringBuilder sb = new StringBuilder(GetTextLength());
            foreach (var item in finalText)
            {
                if (item.emojiIndex != -1)
                {
                    sb.Append(string.Format("[{0}]", item.emojiIndex));
                }
                else
                {
                    sb.Append(item.message);
                }
            }
            return sb.ToString();
        }

        public int GetTextLength()
        {
            int len = 0;
            foreach (var item in finalText)
            {
                len += item.emojiIndex != -1 ? 1 : item.message.Length;
            }
            return len;
        }

        public Acp GetAcpByRange(RECT rect)
        {
            Acp result = new Acp();
            float width = DrawOrigin.X;
            //emoji Menu button 61------>
            if (rect.left <= X + Width - 61 && rect.top <= Y + Height && rect.right >= X && rect.bottom >= Y)//check if overlap textbox
            {
                if (rect.right <= width)
                {
                    result.Start = result.End = 0;
                }
                else if (rect.left >= currentWidth + width)
                {
                    result.Start = result.End = GetTextLength();
                }
                else
                {
                    bool found_start = false;
                    for (int j = 0; j < finalText.Count; j++)
                    {
                        ChatSnippet item = finalText[j];
                        if ((!found_start && width + item.myLength > rect.left) || (found_start && width + item.myLength > rect.right))
                        {
                            if (item.emojiIndex != -1)
                            {
                                width += item.myLength;
                                if (!found_start)
                                {
                                    //divide char from middle, if selection is on the left part, we dont sel this word
                                    result.Start += (width - item.myLength / 2) <= rect.left ? 1 : 0;
                                    result.End = result.Start;
                                    result.End += (((width - item.myLength / 2) < rect.right) && ((width - item.myLength / 2) > rect.left)) ? 1 : 0;
                                    found_start = true;
                                    if (width >= rect.right)
                                    {
                                        return result;
                                    }
                                    continue;
                                }
                                else
                                {
                                    //divide char from middle, if selection is on the left part, we dont sel this word
                                    result.End += (width - item.myLength / 2) < rect.right ? 1 : 0;
                                    return result;
                                }
                            }
                            else
                            {
                                foreach (char ch in item.message)
                                {
                                    var char_x = Font.MeasureString(ch.ToString()).X;
                                    width += char_x;
                                    if (!found_start && width > rect.left)
                                    {
                                        //divide char from middle, if selection is on the left part, we dont sel this word
                                        result.Start += (width - char_x / 2) <= rect.left ? 1 : 0;
                                        result.End = result.Start;
                                        found_start = true;
                                        result.End += (((width - char_x / 2) < rect.right) && ((width - char_x / 2) > rect.left)) ? 1 : 0;
                                        if (width >= rect.right)
                                        {
                                            return result;
                                        }
                                        continue;
                                    }
                                    else if (found_start && width > rect.right)
                                    {
                                        //divide char from middle, if selection is on the left part, we dont sel this word
                                        result.End += (width - char_x / 2) < rect.right ? 1 : 0;
                                        return result;
                                    }
                                    if (found_start)
                                        result.End++;
                                    else
                                        result.Start++;
                                }
                            }
                            continue;
                        }
                        width += item.myLength;
                        if (found_start)
                            result.End += item.emojiIndex != -1 ? 1 : item.message.Length;
                        else
                            result.Start += item.emojiIndex != -1 ? 1 : item.message.Length;
                    }
                }
            }
            else
            {
                result.Start = result.End = -1;
            }
            return result;
        }

        public RECT GetTextExt(Acp acp)
        {
            var test_len = GetTextLength();
            if (acp.End > test_len)
                acp.End = test_len;
            RECT rect = new RECT();
            rect.left = (int)DrawOrigin.X;

            var start = Math.Min(acp.Start, acp.End);
            var end = Math.Max(acp.Start, acp.End);
            //at start
            if (end == 0)
            {
                goto Finish;
            }
            //at end
            if (start == GetTextLength())
            {
                rect.left += (int)currentWidth;
                goto Finish;
            }

            int index = 0;
            bool foundstart = start == 0;
            if (foundstart)
                rect.right += rect.left;
            foreach (ChatSnippet item in finalText)
            {
                var len = item.emojiIndex != -1 ? 1 : item.message.Length;
                if ((!foundstart && index + len > start) || (foundstart && index + len >= end))
                {
                    if (!foundstart)
                    {
                        if (item.emojiIndex != -1)
                        {
                            rect.right += rect.left;
                            rect.right += (int)item.myLength;
                            index++;
                            if (index == end)
                            {
                                goto Finish;
                            }
                        }
                        else
                        {
                            var sub_len = Math.Min(start - index, item.message.Length);
                            rect.left += (int)Font.MeasureString(item.message.Substring(0, sub_len)).X;
                            rect.right += rect.left;
                            if (index + len >= end)
                            {
                                rect.right += (int)Font.MeasureString(item.message.Substring(sub_len, end - start)).X;
                                goto Finish;
                            }
                            else
                            {
                                rect.right += (int)Font.MeasureString(item.message.Substring(sub_len)).X;
                                index += len;
                            }
                        }
                        foundstart = true;
                        continue;
                    }
                    else
                    {
                        if (item.emojiIndex != -1)
                        {
                            rect.right += (int)item.myLength;
                        }
                        else
                        {
                            var sub_len = end - index;
                            rect.right += (int)Font.MeasureString(item.message.Substring(0, sub_len)).X;
                        }
                        goto Finish;
                    }
                }
                index += len;
                if (!foundstart)
                    rect.left += (int)item.myLength;
                else
                    rect.right += (int)item.myLength;
            }
        Finish:
            rect.top = (int)DrawOrigin.Y;
            rect.bottom = rect.top + 40;//emoji 40

            return rect;
        }

        public void ReplaceSelection(string _text)
        {
            if (acp.Start != acp.End)//delete selection
            {
                if (acp.End < acp.Start)
                {
                    var temp = acp.Start;
                    acp.Start = acp.End;
                    acp.End = temp;
                }
                int _index = 0;
                for (int i = 0; i < finalText.Count && acp.End - acp.Start > 0; i++)//delete text/emoji before end reach start
                {
                    ChatSnippet item = finalText[i];
                    _index += item.emojiIndex != -1 ? 1 : item.message.Length;
                    if (_index > acp.Start)
                    {
                        if (item.emojiIndex != -1)
                        {
                            finalText.RemoveAt(i);
                            i--;
                            acp.End--;
                            _index--;
                            if (i >= 0 && finalText.Count > i + 1 && finalText[i].emojiIndex == -1 && finalText[i + 1].emojiIndex == -1)
                            {
                                //both text,merge it
                                _index -= finalText[i].message.Length;
                                finalText[i].message += finalText[i + 1].message;
                                finalText[i].myLength += finalText[i + 1].myLength;
                                finalText.RemoveAt(i + 1);
                                //re-handle this snippet
                                i--;
                            }
                        }
                        else
                        {
                            //acp selection may cross snippet, dont out of range
                            var start = acp.Start - (_index - item.message.Length);
                            int len = Math.Min(acp.End - acp.Start, item.message.Length - start);
                            item.message = item.message.Remove(start, len);
                            acp.End -= len;
                            _index -= len;
                            if (item.message.Length == 0)//empty, remove it
                            {
                                finalText.RemoveAt(i);
                                i--;
                            }
                            else
                            {
                                item.myLength = Font.MeasureString(item.message).X;
                            }
                        }
                    }
                }
                updateWidth();
            }
            int index = 0;
            ChatSnippet chatSnippet = new ChatSnippet(_text, LocalizedContentManager.CurrentLanguageCode);
            if (chatSnippet.myLength == 0)
                return;
            for (int i = 0; i < finalText.Count; i++)
            {
                if (chatSnippet.myLength + currentWidth >= Width - 66)
                {
                    acp.End = acp.Start;
                    return;
                }
                ChatSnippet item = finalText[i];
                index += item.emojiIndex != -1 ? 1 : item.message.Length;
                if (index >= acp.Start && item.emojiIndex == -1)//[text  [caret > ]   message][ = caret (index)]
                {
                    item.message = item.message.Insert(acp.Start - (index - item.message.Length), chatSnippet.message);
                    item.myLength += chatSnippet.myLength;
                    goto Final;
                }
                else if (index > acp.Start)//[nothing/emoji][caret here][emoji(now index is here, larger than caret pos)]
                {
                    finalText.Insert(i, chatSnippet);
                    goto Final;
                }
            }
            finalText.Add(chatSnippet);
        Final:
            acp.Start = acp.End = acp.Start + chatSnippet.message.Length;
            updateWidth();
            //IME input dont play sound, english input sound is handled at IKeyboadSubscriber
            //Game1.playSound("cowboy_monsterhit");//TSF may replace some word, which will make the sound strange
        }

        public virtual Acp GetSelection()
        {
            return acp;
        }

        public virtual void SetSelection(int acpStart, int acpEnd)
        {
            acp.Start = acpStart;
            acp.End = acpEnd;

            int len = GetTextLength();
            if (acp.Start > len || acp.End > len)//out of range
                acp.End = acp.Start = len;//reset caret to the tail
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

        public void SetSelState(SelState state)
        {
            selState = state;
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

            float xPositionSoFar = DrawOrigin.X;
            if (Selected)
            {
                DrawByAcp(spriteBatch, new Acp(0, acp.Start), ref xPositionSoFar, TextColor, drawShadow);
                DrawCaret(spriteBatch, ref xPositionSoFar);
                DrawByAcp(spriteBatch, new Acp(acp.Start, GetTextLength()), ref xPositionSoFar, TextColor, drawShadow);
            }
            else
                DrawByAcp(spriteBatch, new Acp(0, GetTextLength()), ref xPositionSoFar, TextColor, drawShadow);

            KeyboardInput_.comp.Draw(spriteBatch);
        }

        protected virtual void DrawByAcp(SpriteBatch spriteBatch, Acp acp, ref float offset, Color color, bool drawShadow = true)
        {
            var start = Math.Min(acp.Start, acp.End);
            var end = Math.Max(acp.Start, acp.End);
            int index = 0;
            if (end == 0 || start == GetTextLength())
            {
                return;
            }
            bool foundstart = start == 0;
            foreach (ChatSnippet item in finalText)
            {
                var len = item.emojiIndex != -1 ? 1 : item.message.Length;
                if ((!foundstart && index + len > start) || (foundstart && index + len >= end))
                {
                    if (!foundstart)
                    {
                        if (item.emojiIndex != -1)
                        {
                            index++;
                            DrawChatSnippet(spriteBatch, item, ref offset, drawShadow);
                            if (index == end)
                            {
                                goto Finish;
                            }
                        }
                        else
                        {
                            var sub_len = Math.Min(start - index, len);
                            if (index + len >= end)
                            {
                                ChatSnippet sep_text = new ChatSnippet(item.message.Substring(sub_len, end - start), LocalizedContentManager.CurrentLanguageCode);
                                DrawChatSnippet(spriteBatch, sep_text, ref offset, drawShadow);
                                goto Finish;
                            }
                            else
                            {
                                ChatSnippet sep_text = new ChatSnippet(item.message.Substring(sub_len), LocalizedContentManager.CurrentLanguageCode);
                                DrawChatSnippet(spriteBatch, sep_text, ref offset, drawShadow);
                                index += len;
                            }
                        }
                        foundstart = true;
                        continue;
                    }
                    else
                    {
                        if (item.emojiIndex != -1)
                        {
                            DrawChatSnippet(spriteBatch, item, ref offset, drawShadow);
                        }
                        else
                        {
                            var sub_len = end - index;
                            ChatSnippet sep_text = new ChatSnippet(item.message.Substring(0, sub_len), LocalizedContentManager.CurrentLanguageCode);
                            DrawChatSnippet(spriteBatch, sep_text, ref offset, drawShadow);
                        }
                        goto Finish;
                    }
                }
                index += len;
                if (foundstart)
                    DrawChatSnippet(spriteBatch, item, ref offset, drawShadow);
            }
        Finish:
            return;
        }

        public virtual void DrawChatSnippet(SpriteBatch spriteBatch, ChatSnippet snippet, ref float offset, bool drawShadow = true)
        {
            if (snippet.emojiIndex != -1)
            {
                spriteBatch.Draw(
                    ChatBox.emojiTexture,
                    new Vector2(offset, DrawOrigin.Y),
                    new Rectangle?(new Rectangle(
                        snippet.emojiIndex * 9 % ChatBox.emojiTexture.Width,
                        snippet.emojiIndex * 9 / ChatBox.emojiTexture.Width * 9,
                        9,
                        9)),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.99f);
            }
            else if (snippet.message != null)
            {
                spriteBatch.DrawString(
                    ChatBox.messageFont(LocalizedContentManager.CurrentLanguageCode),
                    snippet.message,
                    new Vector2(offset, DrawOrigin.Y),
                    ChatMessage.getColorFromName(Game1.player.defaultChatColor),
                    0f, Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.99f);
            }
            offset += snippet.myLength;
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
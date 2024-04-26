/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Diagnostics;

namespace StackEverythingRedux.UI
{
    /// <summary>Custom implementation of the NameMenu input text box that has additional functionality.</summary>
    public class InputTextBox : IKeyboardSubscriber
    {
        private const int CARET_WIDTH = Game1.pixelZoom;

        /// <summary>Generic event.</summary>
        /// <param name="sender">Textbox the event originated from.</param>
        public delegate void InputTextboxEvent(InputTextBox sender);

        /// <summary>Invoked when the text is submitted.</summary>
        public event InputTextboxEvent OnSubmit;
        //public event InputTextboxEvent OnClick;
        //public event InputTextboxEvent OnGainFocus;
        //public event InputTextboxEvent OnLoseFocus;

        /// <summary>Position of the text box.</summary>
        public Vector2 Position { get; set; }
        /// <summary>Extent of the text box.</summary>
        public Vector2 Extent { get; set; }

        /// <summary>Regular text color.</summary>
        public Color TextColor { get; set; } = Game1.textColor;

        /// <summary>The text font.</summary>
        public SpriteFont Font { get; set; } = Game1.smallFont;

        /// <summary>Is the text selected? Unused.</summary>
        public bool Selected { get; set; } // IKeyboardSubscriber

        /// <summary>Allow numbers only to be input.</summary>
        public bool NumbersOnly { get; set; } = false;

        /// <summary>Should the text be highlighted by default on construction so that additional input clears the existing text.</summary>
        public bool HighlightByDefault { get; set; } = true;

        /// <summary>The current text that was input.</summary>
        public string Text { get; private set; }

        /// <summary>The texture used to draw the highlight background.</summary>
        private readonly Texture2D HighlightTexture;

        /// <summary>Is the text currently highlighted.</summary>
        private bool IsTextHighlighted = false;

        /// <summary>Maximum allowed characters.</summary>
        private readonly int CharacterLimit = 0;

        /// <summary>The caret used for text navigation.</summary>
        private readonly Caret Caret;

        /// <summary>Constructs an instance.</summary>
        /// <param name="characterLimit">The character limit.</param>
        /// <param name="defaultText">The default text to display.</param>
        public InputTextBox(int characterLimit = 0, string defaultText = "")
        {
            CharacterLimit = characterLimit;
            Caret = new Caret(characterLimit);

            _ = AppendString(defaultText);

            // Create a 1x1 texture that we will scale up to draw the highlight
            HighlightTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            Color[] data = [StaticConfig.HighlightColor];
            HighlightTexture.SetData(data);

            IsTextHighlighted = HighlightByDefault;
            if (IsTextHighlighted)
            {
                SelectAllText();
            }
        }

        /* Begin IKeyboardSubscriber implementation */
        #region IKeyboardSubscriber implementation

        /// <summary>Appends the received character to the text if able to. Clears the current text if it's highlighted.</summary>
        /// <param name="inputChar">The character to append.</param>
        public void RecieveTextInput(char inputChar)
        {
            if (CanAppendChar(inputChar))
            {
                if (IsTextHighlighted)
                {
                    ClearText();
                }

                AppendCharacter(inputChar);
            }
        }

        /// <summary>Appends the received string to the text if able to. Clears the current text if it's highlighted.</summary>
        /// <param name="text">The string to append.</param>
        public void RecieveTextInput(string text)
        {
            if (IsValidString(text))
            {
                if (IsTextHighlighted)
                {
                    ClearText();
                }

                _ = AppendString(text);
            }
        }

        /// <summary>Callback for when 'command' characters are recieved.</summary>
        /// <param name="command">The command received.</param>
        public void RecieveCommandInput(char command)
        {
            // Cast the ascii value to the readable enum value
            Keys key = (Keys)command;
            switch (key)
            {
                case Keys.Back:
                    ClearTextIfHighlighted();
                    RemoveCharacterLeftOfCaret();
                    break;
                // Started using Key.Enter from SMAPI because SDV don't recognize numpad enter
                //case Keys.Enter:
                //    Submit();
                //    break;
                case Keys.Tab:
                    break;
            }
        }

        /// <summary>Handles special input for things like text navigation and manipulation.</summary>
        /// <param name="key">The key received.</param>
        public void RecieveSpecialInput(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    Caret.Regress(1);
                    CancelHighlight();
                    break;
                case Keys.Right:
                    Caret.Advance(1, Text.Length);
                    CancelHighlight();
                    break;
                case Keys.Home:
                    Caret.Start();
                    CancelHighlight();
                    break;
                case Keys.End:
                    Caret.End(Text.Length);
                    CancelHighlight();
                    break;
                case Keys.Delete:
                    ClearTextIfHighlighted();
                    RemoveCharacterRightOfCaret();
                    break;
                case Keys.A:
                    TrySelectAllText();
                    break;
            }
        }
        #endregion IKeyboardSubscriber implementation
        /* End IKeyboardSubscriber implementation */

        /// <summary>Invokes the OnSubmit event.</summary>
        private void Submit()
        {
            OnSubmit(this);
        }

        /// <summary>Unused.</summary>
        public void Update()
        {
            // TODO: handle highlighting and stuff
        }

        /// <summary>If this textbox contains this point.</summary>
        public bool ContainsPoint(float x, float y)
        {
            return x >= Position.X && y >= Position.Y && x <= Extent.X && y <= Extent.Y;
        }

        /// <summary>Draws the text box.</summary>
        /// <param name="spriteBatch">Spritebatch to draw with.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Part of the spritesheet containing the texture we want to draw
            IClickableMenu.drawTextureBox(spriteBatch, (int)Position.X, (int)Position.Y, (int)Extent.X, (int)Extent.Y, Color.White);

            Vector2 textDimensions = Font.MeasureString(Text.Length > 0 ? Text : " ");
            float letterWidth = textDimensions.X / (Text.Length > 0 ? Text.Length : 1);
            Vector2 textPosition = Position + new Vector2(Game1.tileSize / 4, Game1.tileSize / 3);

            // Draw the highlight texture
            if (IsTextHighlighted && Text.Length > 0)
            {
                Vector2 highlightPos = new(textPosition.X, textPosition.Y - Game1.pixelZoom);  // pixelZoom is for spacing
                Rectangle destRect = new((int)highlightPos.X, (int)highlightPos.Y, (int)textDimensions.X, (int)textDimensions.Y);
                spriteBatch.Draw(HighlightTexture, destRect, StaticConfig.HighlightColor * 0.75f);
            }

            // Draw the text
            Color textColor = IsTextHighlighted ? StaticConfig.HighlightTextColor : TextColor;
            spriteBatch.DrawString(Font, Text, textPosition, textColor);

            // Draw the caret
            int caretX = (int)letterWidth * Caret.Index;
            // Offset by a small amount when were not at the end so the caret doesn't go on top of the letter
            caretX = Caret.Index < Text.Length ? caretX - Game1.pixelZoom : caretX;
            spriteBatch.Draw(Game1.staminaRect,
                new Rectangle(
                    (int)Position.X + (Game1.tileSize / 4) + caretX + Game1.pixelZoom,
                    (int)Position.Y + (Game1.tileSize / 3) - Game1.pixelZoom,
                    CARET_WIDTH,
                    (int)textDimensions.Y),
                TextColor);
        }

        #region TextSelection

        /// <summary>Selects all the text if left control is held down.</summary>
        private void TrySelectAllText()
        {
            if (StackEverythingRedux.Input.IsDown(SButton.LeftControl))
            {
                SelectAllText();
            }
        }

        /// <summary>Highlights all the text.</summary>
        private void SelectAllText()
        {
            IsTextHighlighted = true;
        }

        /// <summary>Un-highlights all the text.</summary>
        private void CancelHighlight()
        {
            IsTextHighlighted = false;
        }
        #endregion TextSelection

        #region Text manipulation

        /// <summary>Checks if the character is able to be appended to the text.</summary>
        /// <param name="c">The character to append.</param>
        private bool CanAppendChar(char c)
        {
            return
                (CharacterLimit == 0 || Text.Length < CharacterLimit)
                && (!NumbersOnly || char.IsDigit(c))
                ;
        }

        /// <summary>Checks if the string is valid.</summary>
        /// <param name="s">The string to check.</param>
        private bool IsValidString(string s)
        {
            return s != null && s.Length != 0 && (!NumbersOnly || int.TryParse(s, out _));
        }

        /// <summary>Appends a string to the text. String must be valid.</summary>
        /// <param name="s">The string to append.</param>
        private bool AppendString(string s)
        {
            Debug.Assert(IsValidString(s));
            Text = CharacterLimit > 0 && s.Length > CharacterLimit ? s.Remove(CharacterLimit - 1) : s;

            // Move the caret to the end
            Caret.End(Text.Length);
            return true;
        }

        /// <summary>Appends a character to the text. Character must be valid.</summary>
        /// <param name="c">The character to append.</param>
        private void AppendCharacter(char c)
        {
            Debug.Assert(CanAppendChar(c));
            Text += c;
            Caret.Advance(1, Text.Length);
        }

        /// <summary>Deletes the character on the left side of the caret.</summary>
        private void RemoveCharacterLeftOfCaret()
        {
            // The caret is always to the right of the character we want to remove.
            if (Caret.Index > 0)
            {
                Text = Text.Remove(Caret.Index - 1, 1);
                Caret.Regress(1);
            }
        }

        /// <summary>Deletes the character on the right side of the caret.</summary>
        private void RemoveCharacterRightOfCaret()
        {
            if (Caret.Index < Text.Length)
            {
                Text = Text.Remove(Caret.Index, 1);
            }
        }

        /// <summary>Clears all the text.</summary>
        private void ClearText()
        {
            Text = "";
            Caret.Start();
            IsTextHighlighted = false;
        }

        /// <summary>Clears all the text if it's highlighted.</summary>
        private void ClearTextIfHighlighted()
        {
            if (IsTextHighlighted)
            {
                ClearText();
            }
        }
        #endregion Text manipulation
    }
}

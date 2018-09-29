using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StackSplitX
{
    /// <summary>Custom implementation of the NameMenu input text box that has additional functionality.</summary>
    public class InputTextBox : IKeyboardSubscriber
    {
        // TODO: create proper event args
        /// <summary>Generic event.</summary>
        /// <param name="textbox">Textbox the event originated from.</param>
        public delegate void InputTextboxEvent(InputTextBox textbox);

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
        
        /// <summary>Text color when the text is highlighted. This should contrast with HighlightColor.</summary>
        public Color HighlightTextColor { get; set; } = Color.White;
        
        /// <summary>The background color of the highlighted text.</summary>
        public Color HighlightColor { get; set; } = Color.Blue;
        
        /// <summary>The text font.</summary>
        public SpriteFont Font { get; set; } = Game1.smallFont;
        
        /// <summary>Is the text selected? Unused.</summary>
        public bool Selected { get; set; } // IKeyboardSubscriber
        
        /// <summary>Allow numbers only to be input.</summary>
        public bool NumbersOnly { get; set; } = false;
        
        /// <summary>Should the text be highlighted by default on construction so that additional input clears the existing text.</summary>
        public bool HighlightByDefault { get; set; } = true; // TODO: make config option
        
        /// <summary>The current text that was input.</summary>
        public string Text { get; private set; }

        /// <summary>The texture used to draw the highlight background.</summary>
        private Texture2D HighlightTexture;

        /// <summary>Is the text currently highlighted.</summary>
        private bool IsTextHighlighted = false;

        /// <summary>Maximum allowed characters.</summary>
        private int CharacterLimit = 0;

        /// <summary>The caret used for text navigation.</summary>
        private Caret Caret;

        /// <summary>Constructs an instance.</summary>
        /// <param name="characterLimit">The character limit.</param>
        /// <param name="defaultText">The default text to display.</param>
        public InputTextBox(int characterLimit = 0, string defaultText = "")
        {
            this.CharacterLimit = characterLimit;
            this.Caret = new Caret(characterLimit);

            AppendString(defaultText);

            // Create a 1x1 texture that we will scale up to draw the highlight
            this.HighlightTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            Color[] data = new Color[1] { this.HighlightColor };
            this.HighlightTexture.SetData(data);

            this.IsTextHighlighted = this.HighlightByDefault;
            if (this.IsTextHighlighted)
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
                if (this.IsTextHighlighted)
                    ClearText();

                AppendCharacter(inputChar);
            }
        }

        /// <summary>Appends the received string to the text if able to. Clears the current text if it's highlighted.</summary>
        /// <param name="text">The string to append.</param>
        public void RecieveTextInput(string text)
        {
            if (IsValidString(text))
            {
                if (this.IsTextHighlighted)
                    ClearText();

                AppendString(text);
            }
        }

        /// <summary>Callback for when 'command' characters are recieved.</summary>
        /// <param name="command">The command recieved.</param>
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
                case Keys.Enter:
                    Submit();
                    break;
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
                    this.Caret.Regress(1);
                    CancelHighlight();
                    break;
                case Keys.Right:
                    this.Caret.Advance(1, this.Text.Length);
                    CancelHighlight();
                    break;
                case Keys.Home:
                    this.Caret.Start();
                    CancelHighlight();
                    break;
                case Keys.End:
                    this.Caret.End(this.Text.Length);
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
            this.OnSubmit(this);
        }

        /// <summary>Unused.</summary>
        public void Update()
        {
            // TODO: handle highlighting and stuff
        }

        /// <summary>If this textbox contains this point.</summary>
        public bool ContainsPoint(float x, float y)
        {
            return (x >= this.Position.X && y >= this.Position.Y && x <= this.Extent.X && y <= this.Extent.Y);
        }

        /// <summary>Draws the text box.</summary>
        /// <param name="spriteBatch">Spritebatch to draw with.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Part of the spritesheet containing the texture we want to draw
            var menuTextureSourceRect = new Rectangle(0, 256, 60, 60);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, menuTextureSourceRect, (int)this.Position.X, (int)this.Position.Y, (int)this.Extent.X, (int)this.Extent.Y, Color.White);

            var textDimensions = this.Font.MeasureString(this.Text.Length > 0 ? this.Text : " ");
            var letterWidth = (textDimensions.X / (this.Text.Length > 0 ? this.Text.Length : 1));
            var textPosition = this.Position + new Vector2(Game1.tileSize / 4, Game1.tileSize / 3);

            // Draw the highlight texture
            if (this.IsTextHighlighted && this.Text.Length > 0)
            {
                var highlightPos = new Vector2(textPosition.X, textPosition.Y - Game1.pixelZoom);
                var destRect = new Rectangle((int)highlightPos.X, (int)highlightPos.Y, (int)textDimensions.X, (int)textDimensions.Y);
                spriteBatch.Draw(this.HighlightTexture, destRect, this.HighlightColor * 0.75f);
            }

            // Draw the text
            Color textColor = this.IsTextHighlighted ? this.HighlightTextColor : this.TextColor;
            spriteBatch.DrawString(this.Font, this.Text, textPosition, textColor);

            // Draw the caret
            int caretX = ((int)letterWidth * this.Caret.Index);
            // Offset by a small amount when were not at the end so the caret doesn't go on top of the letter
            caretX = (this.Caret.Index < this.Text.Length) ? caretX - Game1.pixelZoom : caretX;
            spriteBatch.Draw(Game1.staminaRect, 
                new Rectangle((int)this.Position.X + Game1.tileSize / 4 + caretX + Game1.pixelZoom, 
                              (int)this.Position.Y + Game1.tileSize / 3 - Game1.pixelZoom, 4, 
                              (int)textDimensions.Y), 
                this.TextColor);
        }

        #region TextSelection

        /// <summary>Selects all the text if left control is held down.</summary>
        private void TrySelectAllText()
        {
            if (Utils.IsKeyDown(Keyboard.GetState(), Keys.LeftControl))
                SelectAllText();
        }

        /// <summary>Highlights all the text.</summary>
        private void SelectAllText()
        {
            this.IsTextHighlighted = true;
        }

        /// <summary>Un-highlights all the text.</summary>
        private void CancelHighlight()
        {
            this.IsTextHighlighted = false;
        }
        #endregion TextSelection

        #region Text manipulation

        /// <summary>Checks if the character is able to be appended to the text.</summary>
        /// <param name="c">The character to append.</param>
        private bool CanAppendChar(char c)
        {
            return ((this.CharacterLimit == 0 || this.Text.Length < this.CharacterLimit) &&
                    (!this.NumbersOnly || char.IsDigit(c)));
        }

        /// <summary>Checks if the string is valid.</summary>
        /// <param name="s">The string to check.</param>
        private bool IsValidString(string s)
        {
            if (s == null || s.Length == 0)
                return false;

            int dummy = 0;
            if (this.NumbersOnly && !int.TryParse(s, out dummy))
                return false;
            return true;
        }

        /// <summary>Appends a string to the text. String must be valid.</summary>
        /// <param name="s">The string to append.</param>
        private bool AppendString(string s)
        {
            Debug.Assert(IsValidString(s));
            if (this.CharacterLimit > 0 && s.Length > this.CharacterLimit)
            {
                this.Text = s.Remove(this.CharacterLimit - 1);
            }
            else
            {
                this.Text = s;
            }

            // Move the caret to the end
            this.Caret.End(this.Text.Length);
            return true;
        }

        /// <summary>Appends a character to the text. Character must be valid.</summary>
        /// <param name="c">The character to append.</param>
        private void AppendCharacter(char c)
        {
            Debug.Assert(CanAppendChar(c));
            this.Text += c;
            this.Caret.Advance(1, this.Text.Length);
        }

        /// <summary>Deletes the character on the left side of the caret.</summary>
        private void RemoveCharacterLeftOfCaret()
        {
            // The caret is always to the right of the character we want to remove.
            if (this.Caret.Index > 0)
            {
                this.Text = this.Text.Remove(this.Caret.Index - 1, 1);
                this.Caret.Regress(1);
            }
        }

        /// <summary>Deletes the character on the right side of the caret.</summary>
        private void RemoveCharacterRightOfCaret()
        {
            if (this.Caret.Index < this.Text.Length)
            {
                this.Text = this.Text.Remove(this.Caret.Index, 1);
            }
        }

        /// <summary>Clears all the text.</summary>
        private void ClearText()
        {
            this.Text = "";
            this.Caret.Start();
            this.IsTextHighlighted = false;
        }

        /// <summary>Clears all the text if it's highlighted.</summary>
        private void ClearTextIfHighlighted()
        {
            if (this.IsTextHighlighted)
                ClearText();
        }
        #endregion Text manipulation
    }
}

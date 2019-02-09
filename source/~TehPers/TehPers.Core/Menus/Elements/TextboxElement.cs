using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Menus.BoxModel;
using xTile.Dimensions;

namespace TehPers.Core.Menus.Elements {
    public class TextboxElement : Element {
        private static readonly Location TextPadding = new Location(14, 4);
        private static readonly Regex NewlineRegex = new Regex(@"\r\n?|\n");

        public override bool FocusOnClick { get; } = true;
        public override bool BlockChatbox { get; } = true;
        public override bool RepeatKeystrokes { get; } = true;

        private string _text = "";
        public string Text {
            get => this._text;
            set {
                if (this._text == value || !this.IsValidText(value))
                    return;

                this._text = value;
                this.OnValueChanged();
            }
        }
        public SpriteFont Font { get; set; } = Game1.smallFont;
        public int Cursor {
            get {
                this._cursor = Math.Max(Math.Min(this._cursor, this.Text.Length), 0);
                return this._cursor;
            }
            set => this._cursor = value;
        }
        public Color Color { get; set; } = Game1.textColor;

        private readonly Texture2D _background;
        private int _cursor;

        public TextboxElement() {
            this._background = Game1.content.Load<Texture2D>(@"LooseSprites\textBox.xnb");
            this.Size = new BoxVector(0, this._background.Height, 1f, 0);
            this.Padding = new OuterSize(14, 4);
        }

        protected override void OnDraw(SpriteBatch batch, Rectangle2I parentBounds) {
            // Draw the background
            Rectangle2I bounds = this.Bounds.ToAbsolute(parentBounds);
            float depth = this.GetGlobalDepth(0);
            batch.Draw(this._background, new Microsoft.Xna.Framework.Rectangle(bounds.X, bounds.Y, Game1.tileSize / 4, bounds.Height), new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.tileSize / 4, this._background.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(this._background, new Microsoft.Xna.Framework.Rectangle(bounds.X + Game1.tileSize / 4, bounds.Y, bounds.Width - Game1.tileSize / 2, bounds.Height), new Microsoft.Xna.Framework.Rectangle(Game1.tileSize / 4, 0, 4, this._background.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            batch.Draw(this._background, new Microsoft.Xna.Framework.Rectangle(bounds.X + bounds.Width - Game1.tileSize / 4, bounds.Y, Game1.tileSize / 4, bounds.Height), new Microsoft.Xna.Framework.Rectangle(this._background.Width - Game1.tileSize / 4, 0, Game1.tileSize / 4, this._background.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);

            // Draw the text
            bounds = this.GetChildrenBounds(parentBounds);
            Vector2 origSize = this.Font.MeasureString(this.Text);
            float scale = origSize.Y > 0 ? bounds.Height / origSize.Y : 0;
            this.WithScissorRect(batch, bounds.ToRectangle(), b => {
                b.DrawString(this.Font, this.Text, bounds.Location.ToVector2(), this.Color, 0, Vector2.Zero, scale, SpriteEffects.None, this.GetGlobalDepth(1));
            });

            // Draw the cursor
            if (this.IsFocused && DateTime.Now.Millisecond % 1000 >= 500) {
                float cursorOffset = this.Font.MeasureString(this.Text.Substring(0, this.Cursor)).X * scale;
                Microsoft.Xna.Framework.Rectangle cursorRect = new Microsoft.Xna.Framework.Rectangle(bounds.X + (int) cursorOffset - 2, bounds.Y + 6, 4, bounds.Height - 12);
                batch.Draw(Game1.staminaRect, cursorRect, cursorRect, this.Color, 0f, Vector2.Zero, SpriteEffects.None, this.GetGlobalDepth(0.9f));
            }
        }

        protected override bool OnKeyPressed(Keys key) {
            int length = this.Text.Length;
            if (key == Keys.Back) {
                if (length > 0 && this.Cursor > 0) {
                    StringBuilder newText = new StringBuilder();
                    newText.Append(this.Text.Substring(0, this.Cursor - 1));
                    newText.Append(this.Text.Substring(this.Cursor));
                    if (this.IsValidText(newText.ToString())) {
                        this.Cursor--;
                        this.Text = newText.ToString();
                    }
                }
            } else if (key == Keys.Delete) {
                if (length > 0) {
                    StringBuilder newText = new StringBuilder();
                    newText.Append(this.Text.Substring(0, this.Cursor));
                    if (this.Cursor < this.Text.Length) {
                        newText.Append(this.Text.Substring(this.Cursor + 1));
                    }

                    if (this.IsValidText(newText.ToString())) {
                        this.Text = newText.ToString();
                    }
                }
            } else if (key == Keys.Left) {
                this.Cursor--;
            } else if (key == Keys.Right) {
                this.Cursor++;
            } else if (key == Keys.Home) {
                this.Cursor = 0;
            } else if (key == Keys.End) {
                this.Cursor = this.Text.Length;
            } else {
                return key.IsPrintable();
            }

            return true;
        }

        /// <summary>Whether this is valid text for the textbox to have</summary>
        /// <param name="newText">The potential new text</param>
        /// <returns>True if valid</returns>
        protected virtual bool IsValidText(string newText) => true;

        protected override bool OnTextEntered(string text) {
            string newText = this.Text.Substring(0, this.Cursor) + TextboxElement.NewlineRegex.Replace(text, "") + this.Text.Substring(this.Cursor);
            if (this.IsValidText(newText)) {
                this.Text = newText;
                this.Cursor += text.Length;
            }
            return true;
        }

        protected override bool OnLeftClick(Vector2I mousePos, Rectangle2I parentBounds) {
            if (!base.OnLeftClick(mousePos, parentBounds))
                return false;

            Rectangle2I bounds = this.Bounds.ToAbsolute(parentBounds);

            int curLen = this.Text.Length + 1;
            Vector2 curSize = this.Font.MeasureString(this.Text);
            float scale = curSize.Y > 0 ? (bounds.Height - TextboxElement.TextPadding.Y) / curSize.Y : 0;
            curSize *= scale;
            while (curLen > 0 && curSize.X + bounds.X + TextboxElement.TextPadding.X > mousePos.X) {
                curLen--;
                curSize = this.Font.MeasureString(this.Text.Substring(0, curLen)) * scale;
            }

            this.Cursor = curLen;
            return true;
        }

        public void SetValue(string value) => this.Text = value;
        public string GetValue() => this.Text;

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged() => this.ValueChanged?.Invoke(this, EventArgs.Empty);
    }
}


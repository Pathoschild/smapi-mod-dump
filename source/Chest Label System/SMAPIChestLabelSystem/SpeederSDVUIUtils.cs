using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeederSDVUIUtils
{
    public abstract class SpeederIClickableMenu : IClickableMenu
    {
        public static void drawSimpleTooltipZoomAware(SpriteBatch b, string hoverText, SpriteFont font)
        {            
            int width = (int)((font.MeasureString(hoverText).X + Game1.tileSize / 2) * Game1.options.zoomLevel);
            int height = (int)(Math.Max(60, font.MeasureString(hoverText).Y + Game1.tileSize / 2) * Game1.options.zoomLevel); //60 is "cornerSize" * 3 on SDV source
            int x = (int)((Game1.getOldMouseX() + Game1.tileSize / 2) * Game1.options.zoomLevel);
            int y = (int)((Game1.getOldMouseY() + Game1.tileSize / 2) * Game1.options.zoomLevel);
            if (x + width > Game1.viewport.Width/Game1.options.zoomLevel)
            {
                x = (int)(Game1.viewport.Width/Game1.options.zoomLevel - width);
                y += (int)((Game1.tileSize / 4)*Game1.options.zoomLevel);
            }
            if (y + height > Game1.viewport.Height/Game1.options.zoomLevel)
            {
                x += (int)((Game1.tileSize / 4)*Game1.options.zoomLevel);
                y = (int)(Game1.viewport.Height/Game1.options.zoomLevel - height);
            }
            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White, 1f, true);
            if (hoverText.Count() > 1)
            {
                Vector2 tPosVector = new Vector2(x + (Game1.tileSize / 4)*Game1.options.zoomLevel, y + (Game1.tileSize / 4 + 4)*Game1.options.zoomLevel);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 2f) * Game1.options.zoomLevel, Game1.textShadowColor, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(0f, 2f) * Game1.options.zoomLevel, Game1.textShadowColor, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 0f) * Game1.options.zoomLevel, Game1.textShadowColor, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector, Game1.textColor * 0.9f, 0, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 0);                
            }
        }

        public static void drawSimpleTooltip(SpriteBatch b, string hoverText, SpriteFont font)
        {
            int width = (int)font.MeasureString(hoverText).X + Game1.tileSize / 2;
            int height = Math.Max(60, (int)font.MeasureString(hoverText).Y + Game1.tileSize / 2); //60 is "cornerSize" * 3 on SDV source
            int x = Game1.getOldMouseX() + Game1.tileSize / 2;
            int y = Game1.getOldMouseY() + Game1.tileSize / 2;
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += Game1.tileSize / 4;
            }
            if (y + height > Game1.viewport.Height)
            {
                x += Game1.tileSize / 4;
                y = Game1.viewport.Height - height;
            }
            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White, 1f, true);
            if (hoverText.Count() > 1)
            {
                Vector2 tPosVector = new Vector2(x + (Game1.tileSize / 4), y + (Game1.tileSize / 4 + 4));
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(0f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 0f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector, Game1.textColor * 0.9f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }
    }
    
    public abstract class SpeederUIUtility : Utility
    {
        public static void drawWithShadowZoomAware(SpriteBatch b, Texture2D texture, Vector2 position, Rectangle sourceRect, Color color, float rotation, Vector2 origin, float scale = -1f, bool flipped = false, float layerDepth = -1f, int horizontalShadowOffset = -1, int verticalShadowOffset = -1, float shadowIntensity = 0.35f)
        {
            Utility.drawWithShadow(b, texture, position*Game1.options.zoomLevel, sourceRect, color, rotation, origin, scale*Game1.options.zoomLevel, flipped, layerDepth, horizontalShadowOffset, verticalShadowOffset, shadowIntensity);
        }
    }

    public delegate void TextBoxEvent(TextBox sender);

    public class TextBox : IKeyboardSubscriber
    {
        private Texture2D _textBoxTexture;

        private Texture2D _caretTexture;

        public SpriteFont _font;

        public Color textColor;

        public bool numbersOnly;

        public int textLimit = -1;

        private string _text = "";

        private MouseState _previousMouse;

        public event TextBoxEvent Clicked;

        public event TextBoxEvent OnEnterPressed;

        public event TextBoxEvent OnTabPressed;

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public bool Highlighted
        {
            get;
            set;
        }

        public bool PasswordBox
        {
            get;
            set;
        }

        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
                if (this._text == null)
                {
                    this._text = "";
                }
                if (this._text != "")
                {
                    string filtered = "";
                    for (int i = 0; i < value.Length; i++)
                    {
                        char c = value[i];
                        if (this._font.Characters.Contains(c))
                        {
                            filtered += c;
                        }
                    }
                    this._text = filtered;
                    if (this._font.MeasureString(this._text).X > (float)(this.Width - Game1.tileSize / 3))
                    {
                        this.Text = this._text.Substring(0, this._text.Length - 1);
                    }
                }
            }
        }

        public bool Selected
        {
            get;
            set;
        }

        public TextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor)
        {
            this._textBoxTexture = textBoxTexture;
            if (textBoxTexture != null)
            {
                this.Width = textBoxTexture.Width;
                this.Height = textBoxTexture.Height;
            }
            this._caretTexture = caretTexture;
            this._font = font;
            this.textColor = textColor;
            this._previousMouse = Mouse.GetState();
        }

        public void SelectMe()
        {
            this.Highlighted = true;
            this.Selected = true;
            Game1.keyboardDispatcher.Subscriber = this;
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Point mousePoint = new Point(mouse.X, mouse.Y);
            Rectangle position = new Rectangle(this.X, this.Y, this.Width, this.Height);
            if (position.Contains(mousePoint))
            {
                this.Highlighted = true;
                this.Selected = true;
                Game1.keyboardDispatcher.Subscriber = this;
                if (this._previousMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && this.Clicked != null)
                {
                    this.Clicked(this);
                    return;
                }
            }
            else
            {
                this.Selected = false;
                this.Highlighted = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, false);
        }

        public void Draw(SpriteBatch spriteBatch, bool obeyZoomLevel)
        {
            float zoomLevel = 1f;
            if (obeyZoomLevel)
            {
                zoomLevel = Game1.options.zoomLevel;
            }
            bool caretVisible = DateTime.Now.Millisecond % 1000 >= 500;
            string toDraw = this.Text;
            if (this.PasswordBox)
            {
                toDraw = "";
                for (int i = 0; i < this.Text.Length; i++)
                {
                    toDraw += '•';
                }
            }
            if (this._textBoxTexture != null)
            {
                int zX = (int)(X * zoomLevel);
                int zY = (int)(Y * zoomLevel);
                int zHeight = (int)(Height * zoomLevel);
                int zWidth = (int)(Width * zoomLevel);
                int startEndSliceWidth = (int)(Game1.tileSize / 4 * zoomLevel);
                spriteBatch.Draw(_textBoxTexture, new Rectangle(zX, zY, startEndSliceWidth, zHeight), new Rectangle?(new Rectangle(0, 0, Game1.tileSize / 4, Height)), Color.White);
                spriteBatch.Draw(_textBoxTexture, new Rectangle(zX + startEndSliceWidth, zY, zWidth - startEndSliceWidth*2, zHeight), new Rectangle?(new Rectangle(Game1.tileSize / 4, 0, 4, Height)), Color.White);
                spriteBatch.Draw(_textBoxTexture, new Rectangle(zX + zWidth - startEndSliceWidth, zY, startEndSliceWidth, zHeight), new Rectangle?(new Rectangle(this._textBoxTexture.Bounds.Width - Game1.tileSize / 4, 0, Game1.tileSize / 4, Height)), Color.White);
            }
            else
            {
                Game1.drawDialogueBox(this.X - Game1.tileSize / 2, this.Y - Game1.tileSize * 7 / 4 + 10, this.Width + Game1.tileSize * 5 / 4, this.Height, false, true, null, false);
            }
            Vector2 size = this._font.MeasureString(toDraw);
            if (caretVisible && Selected)
            {
                spriteBatch.Draw(Game1.staminaRect, new Rectangle( (int)((X + Game1.tileSize / 4 + size.X + 2)*zoomLevel), (int)((Y + 10)*zoomLevel), 2, (int)(28*zoomLevel)), textColor);
            }
            Utility.drawTextWithShadow(spriteBatch, toDraw, this._font, new Vector2(X*zoomLevel + (Game1.tileSize / 4)*zoomLevel, Y*zoomLevel + zoomLevel*( (this._textBoxTexture != null) ? (Game1.tileSize / 4 - Game1.pixelZoom) : (Game1.pixelZoom * 2) ) ), this.textColor, zoomLevel, -1f, -1, -1, 1f, 3);
        }

        public void RecieveTextInput(char inputChar)
        {
            if (this.Selected && (!this.numbersOnly || char.IsDigit(inputChar)) && (this.textLimit == -1 || this.Text.Length < this.textLimit))
            {
                if (Game1.gameMode != 3)
                {
                    switch (inputChar)
                    {
                        case '"':
                            return;
                        case '#':
                            break;
                        case '$':
                            Game1.playSound("money");
                            goto IL_CA;
                        default:
                            switch (inputChar)
                            {
                                case '*':
                                    Game1.playSound("hammer");
                                    goto IL_CA;
                                case '+':
                                    Game1.playSound("slimeHit");
                                    goto IL_CA;
                                default:
                                    switch (inputChar)
                                    {
                                        case '<':
                                            Game1.playSound("crystal");
                                            goto IL_CA;
                                        case '=':
                                            Game1.playSound("coin");
                                            goto IL_CA;
                                    }
                                    break;
                            }
                            break;
                    }
                    Game1.playSound("cowboy_monsterhit");
                }
                IL_CA:
                this.Text += inputChar;
            }
        }

        public void RecieveTextInput(string text)
        {
            int dummy = -1;
            if (this.Selected && (!this.numbersOnly || int.TryParse(text, out dummy)) && (this.textLimit == -1 || this.Text.Length < this.textLimit))
            {
                this.Text += text;
            }
        }

        public void RecieveCommandInput(char command)
        {
            if (this.Selected)
            {
                switch (command)
                {
                    case '\b':
                        if (this.Text.Length > 0)
                        {
                            this.Text = this.Text.Substring(0, this.Text.Length - 1);
                            if (Game1.gameMode != 3)
                            {
                                Game1.playSound("tinyWhip");
                                return;
                            }
                        }
                        break;
                    case '\t':
                        if (this.OnTabPressed != null)
                        {
                            this.OnTabPressed(this);
                        }
                        break;
                    default:
                        if (command != '\r')
                        {
                            return;
                        }
                        if (this.OnEnterPressed != null)
                        {
                            this.OnEnterPressed(this);
                            return;
                        }
                        break;
                }
            }
        }

        public void RecieveSpecialInput(Keys key)
        {
        }
    }
}

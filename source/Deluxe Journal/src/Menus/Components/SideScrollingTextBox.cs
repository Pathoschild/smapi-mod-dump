/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A TextBox that does not limit character width and scrolls horizontally.</summary>
    public class SideScrollingTextBox : TextBox
    {
        public SideScrollingTextBox(Texture2D? textBoxTexture, Texture2D? caretTexture, SpriteFont font, Color textColor) :
            base(textBoxTexture, caretTexture, font, textColor)
        {
            limitWidth = false;
        }

        public bool ContainsPoint(int x, int y)
        {
            return new Rectangle(X, Y, Width, Height).Contains(x, y);
        }

        public override void Draw(SpriteBatch b, bool drawShadow = true)
        {
            Rectangle cachedScissorRect = b.GraphicsDevice.ScissorRectangle;
            string text = PasswordBox ? new string('•', Text.Length) : Text;
            Vector2 size = _font.MeasureString(text);
            int scroll = (size.X > Width - 18) ? (int)size.X - Width + 18 : 0;
            bool caretVisible = !(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 < 500.0);

            if (_textBoxTexture != null)
            {
                b.Draw(_textBoxTexture, new Rectangle(X, Y, 16, Height), new Rectangle(0, 0, 16, Height), Color.White);
                b.Draw(_textBoxTexture, new Rectangle(X + 16, Y, Width - 32, Height), new Rectangle(16, 0, 4, Height), Color.White);
                b.Draw(_textBoxTexture, new Rectangle(X + Width - 16, Y, 16, Height), new Rectangle(_textBoxTexture.Bounds.Width - 16, 0, 16, Height), Color.White);
            }
            else
            {
                Game1.drawDialogueBox(X - 32, Y - 112 + 10, Width + 80, Height, speaker: false, drawOnlyBox: true);
            }

            if (caretVisible && Selected)
            {
                b.Draw(Game1.staminaRect, new Rectangle(X + (int)size.X - scroll + 18, Y + 8, 4, 32), _textColor);
            }

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState()
            {
                ScissorTestEnable = true
            });

            b.GraphicsDevice.ScissorRectangle = new Rectangle(X + 4, Y, Width, Height);

            if (drawShadow)
            {
                Utility.drawTextWithShadow(b, text, _font, new Vector2(X - scroll + 16, Y + ((_textBoxTexture != null) ? 12 : 8)), _textColor);
            }
            else
            {
                b.DrawString(_font, text, new Vector2(X - scroll + 16, Y + ((_textBoxTexture != null) ? 12 : 8)), _textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
            }

            b.End();
            b.GraphicsDevice.ScissorRectangle = cachedScissorRect;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        }
    }
}

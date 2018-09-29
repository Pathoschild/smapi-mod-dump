using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace GetDressed.Framework
{
    /// <summary>Displays a message on the screen.</summary>
    public class Alert
    {
        /*********
        ** Properties
        *********/
        /// <summary>The X position from which to draw the alert.</summary>
        private readonly int X;

        /// <summary>The Y position from which to draw the alert.</summary>
        private readonly int Y;

        /// <summary>The message text to display.</summary>
        private readonly string Text;

        /// <summary>Whether the alert should appear with a fade-in effect.</summary>
        private readonly bool FadeIn;

        /// <summary>The icon texture (if any).</summary>
        private readonly Texture2D Texture;

        /// <summary>The icon sprite within the <see cref="Texture"/> (if applicable).</summary>
        private readonly Rectangle SourceRect = Rectangle.Empty;

        /// <summary>The delay until the alert should appear.</summary>
        private float DelayTimeLeft;

        /// <summary>The time left until the message should disappear.</summary>
        private float TimeLeft;

        /// <summary>The alert transparency.</summary>
        private float Transparency = 1f;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="x">The X position from which to draw the alert.</param>
        /// <param name="y">The Y position from which to draw the alert.</param>
        /// <param name="text">The message text to display.</param>
        /// <param name="timeLeft">The time left until the message should disappear.</param>
        /// <param name="fadeIn">Whether the alert should appear with a fade-in effect.</param>
        /// <param name="delayTimeLeft">The delay until the alert should appear.</param>
        public Alert(int x, int y, string text, float timeLeft, bool fadeIn, float delayTimeLeft = 0f)
        {
            this.X = x;
            this.Y = y;
            this.Text = text;
            this.TimeLeft = timeLeft;
            this.FadeIn = fadeIn;
            this.DelayTimeLeft = delayTimeLeft;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="texture">The icon texture (if any).</param>
        /// <param name="sourceRect">The icon sprite within the <see cref="Texture"/> (if applicable).</param>
        /// <param name="x">The X position from which to draw the alert.</param>
        /// <param name="y">The Y position from which to draw the alert.</param>
        /// <param name="text">The message text to display.</param>
        /// <param name="timeLeft">The time left until the message should disappear.</param>
        /// <param name="fadeIn">Whether the alert should appear with a fade-in effect.</param>
        /// <param name="delayTimeLeft">The delay until the alert should appear.</param>
        public Alert(Texture2D texture, Rectangle sourceRect, int x, int y, string text, float timeLeft, bool fadeIn, float delayTimeLeft = 0f)
            : this(x, y, text, timeLeft, fadeIn, delayTimeLeft)
        {
            this.Texture = texture;
            this.SourceRect = sourceRect;
        }

        /// <summary>Update the alert state.</summary>
        /// <param name="time">The current elapsed game time.</param>
        public bool Update(GameTime time)
        {
            // count down appear timer
            if (this.DelayTimeLeft > 0f)
            {
                this.DelayTimeLeft -= time.ElapsedGameTime.Milliseconds;
                return false;
            }

            // count down disappear timer
            this.TimeLeft -= time.ElapsedGameTime.Milliseconds;
            if (this.TimeLeft < 0f)
            {
                this.Transparency -= 0.02f;
                if (this.Transparency < 0f)
                    return true;
            }

            // appear
            else if (this.FadeIn)
                this.Transparency = Math.Min(this.Transparency + 0.02f, 1f);
            return false;
        }

        /// <summary>Draw the alert to the given sprite batch.</summary>
        /// <param name="spriteBatch">The sprite batch to update.</param>
        /// <param name="font">The font with which to render text.</param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (this.DelayTimeLeft > 0f)
                return;

            int num = (int)font.MeasureString(this.Text).X;

            spriteBatch.Draw(Game1.mouseCursors, new Vector2(this.X + 370, this.Y - 50), new Rectangle(319, 360, 1, 24), Color.White * this.Transparency, 0f, Vector2.Zero, new Vector2(num, Game1.pixelZoom), SpriteEffects.None, 0.0001f);
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(this.X + 275, this.Y - 50), new Rectangle(293, 360, 24, 24), Color.White * this.Transparency, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(this.X + 370 + num, this.Y - 50), new Rectangle(322, 360, 7, 24), Color.White * this.Transparency, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
            if (this.Texture != null)
                spriteBatch.Draw(this.Texture, new Vector2(this.X + 290, this.Y - 35), this.SourceRect, Color.White * this.Transparency, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);

            Utility.drawTextWithShadow(spriteBatch, this.Text, Game1.smallFont, new Vector2(this.X + 375, this.Y - 15), Color.Black * this.Transparency, 1f, 1f, -1, -1, this.Transparency);
        }
    }
}

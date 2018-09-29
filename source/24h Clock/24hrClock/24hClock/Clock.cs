using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Lajna.Mods.MilitaryTime
{
    /// <summary>An overlay menu which replaces the game's default clock.</summary>
    internal class Clock : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        /// <summary>The source rectangle of the clock background in the spritesheet.</summary>
        private readonly Rectangle SourceRect = new Rectangle(360, 460, 40, 9);

        /// <summary>The game's time box to change.</summary>
        private readonly DayTimeMoneyBox TimeBox;
       

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="timeBox">The game's time box to change.</param>
        public Clock(DayTimeMoneyBox timeBox)
        {
            this.TimeBox = timeBox;
        }

        /// <summary>The method invoked when the player right-clicks on the lookup UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        /// <summary>Render the UI.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            // format time
            int time = Game1.timeOfDay;
            time = time % 2400;

            string Stime = time.ToString("0000"); // zero-pad up to length 4
            
            Stime = Stime.Substring(0, 2) + ":" + Stime.Substring(2, 2);

            // get positions
            Vector2 textPosition = this.TimeBox.position + new Vector2(-20, -9);
            Vector2 backgroundPosition = this.TimeBox.position + new Vector2(108, 115);
            Vector2 textSize = Game1.dialogueFont.MeasureString(Game1.timeOfDay.ToString());
            Vector2 textOffset = new Vector2((float)(this.SourceRect.X * 0.55 - textSize.X / 2.0), (float)(this.SourceRect.Y * .31 - textSize.Y / 2.0));

            // draw clock
            spriteBatch.Draw(Game1.mouseCursors, backgroundPosition, this.SourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
            Utility.drawTextWithShadow(spriteBatch, Stime, Game1.dialogueFont, textPosition + textOffset, Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
        }


    }
}
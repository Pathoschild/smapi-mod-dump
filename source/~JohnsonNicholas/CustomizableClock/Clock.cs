using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace KN.CustomizableClock
{
    /// <summary>An overlay menu which replaces the game's default clock.</summary>
    internal class Clock : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        /// <summary>The source rectangle of the clock background in the spritesheet.</summary>
        private readonly Rectangle SourceRect = new Rectangle(360, 460, 40, 9);

        private readonly DayTimeMoneyBox TimeBox;
        protected ClockConfig ModConfig;

        /*********
        ** Public methods
        *********/
        public Clock(DayTimeMoneyBox timeBox, ClockConfig OurConfig)
        {
            TimeBox = timeBox;
            ModConfig = OurConfig;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            string time = "";

            if (ModConfig.Is24hClock)
            {
                time = (Game1.timeOfDay % 2400).ToString("0000"); // zero-pad up to length 4
                time = time.Substring(0, 2) + ":" + time.Substring(2, 2);
            }
            else
            {
                int min = Game1.timeOfDay % 100;
                time = Game1.timeOfDay / 100 % 12 == 0
                    ? "12"
                    : string.Concat((object) (Game1.timeOfDay / 100 % 12));
                time += ":" + (Game1.timeOfDay % 100) + (Game1.timeOfDay % 100 == 0 ? "0" : "");

                if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
                    time = time + " " + (Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400
                               ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370")
                               : Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371"));
                else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja ||
                         ModConfig.UseJAChar)
                    time = Game1.timeOfDay < 1200 || Game1.timeOfDay >= 2400
                        ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") + " " + time
                        : Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371") + " " + time;
                else if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh ||
                         ModConfig.UseZHChar)
                    time = Game1.timeOfDay < 600 || Game1.timeOfDay >= 2400
                        ? "凌晨 " + time
                        : (Game1.timeOfDay < 1200
                            ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") + " " +
                              time
                            : (Game1.timeOfDay < 1300
                                ? "中午  " + time
                                : (Game1.timeOfDay < 1900
                                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371") +
                                      " " + time
                                    : "晚上  " + time)));
            }

            int hr = Game1.timeOfDay / 100;

            // get positions
            Vector2 textPosition = TimeBox.position + new Vector2(-20, -9);
            if (!ModConfig.Is24hClock && hr > 9)
                textPosition = TimeBox.position + new Vector2(-46, -9);
            else if (!ModConfig.Is24hClock && hr < 10)
                textPosition = TimeBox.position + new Vector2(-36, -9);
            
            Vector2 backgroundPosition = TimeBox.position + new Vector2(108, 115);
            Vector2 textSize = Game1.dialogueFont.MeasureString(Game1.timeOfDay.ToString());
            Vector2 textOffset = new Vector2((float) (SourceRect.X * 0.55 - textSize.X / 2.0),
                (float) (SourceRect.Y * .31 - textSize.Y / 2.0));

            // draw clock
            spriteBatch.Draw(Game1.mouseCursors, backgroundPosition, SourceRect, Color.White, 0f, Vector2.Zero,
                4f, SpriteEffects.None, 0.9f);
            Utility.drawTextWithShadow(spriteBatch, time, Game1.dialogueFont, textPosition + textOffset,
                Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
        }
    }
}
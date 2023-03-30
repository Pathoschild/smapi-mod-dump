/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley;
using System;

namespace Circuit.UI
{
    public class RunTimerMenu : IClickableMenu
    {
        public int SecondsRemaining { get; set; } = 0;

        public bool IsStarted { get; set; } = false;

        public RunTimerMenu(int initialSeconds) : base()
        {
            SecondsRemaining = initialSeconds;

            CalculatePositions();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);

            CalculatePositions();
        }

        private void CalculatePositions()
        {
            xPositionOnScreen = 84;
            yPositionOnScreen = 7;

            width = 174;
            height = 90;
        }

        public void Tick(int amount = 1)
        {
            SecondsRemaining = Math.Max(SecondsRemaining - amount, 0);
        }

        public static string GetHHMMSS(int seconds)
        {
            TimeSpan span = TimeSpan.FromSeconds(seconds);
            return $"{span:hh\\:mm\\:ss}";
        }

        private void DrawTimerBox(SpriteBatch b)
        {
            drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White,
                drawShadow: false
            );
        }

        private void DrawTimeRemaining(SpriteBatch b)
        {
            string text = GetHHMMSS(SecondsRemaining);
            Vector2 textSize = Game1.dialogueFont.MeasureString(text);

            Utility.drawTextWithShadow(
                b,
                text,
                Game1.dialogueFont,
                new Vector2(
                    xPositionOnScreen + (width / 2) - ((int)textSize.X / 2),
                    yPositionOnScreen + (height / 2) - ((int)textSize.Y / 2) + 2
                ),
                Game1.textColor
            );
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            DrawTimerBox(b);
            DrawTimeRemaining(b);

            drawMouse(b);
        }
    }
}

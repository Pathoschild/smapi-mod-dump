/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace StardewRoguelike.UI
{
    internal class MusicAnnounceMenu : IClickableMenu
    {
        private static readonly float DisplayDuration = 10_000;

        private float durationRemaining = DisplayDuration;

        private readonly string DisplayText;

        private readonly Vector2 DisplayTextSize;

        public MusicAnnounceMenu(string displayText) : base()
        {
            DisplayText = displayText;
            DisplayTextSize = Game1.smallFont.MeasureString(DisplayText);

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            width = (int)DisplayTextSize.X + 74;
            height = (int)DisplayTextSize.Y + 32;

            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width / 2) - 18;
            yPositionOnScreen = 10;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            UpdatePositions();
        }

        public override void update(GameTime time)
        {
            if (destroy)
                return;

            base.update(time);

            durationRemaining -= (float)time.ElapsedGameTime.TotalMilliseconds;

            if (durationRemaining <= 0)
                destroy = true;
        }

        public override void draw(SpriteBatch b)
        {
            if (destroy)
                return;

            float alpha;
            if (durationRemaining > 7500)
                alpha = 1 - (durationRemaining - 7500) / 2500;
            else
                alpha = durationRemaining > 1250 ? 1f : durationRemaining / 1250;

            int drawHeight = yPositionOnScreen;
            if (BossKillAnnounceMenu.IsToolbarOnTop())
                drawHeight += 150;

            drawTextureBox(
                b,
                xPositionOnScreen,
                drawHeight,
                width,
                height,
                Color.White * alpha
            );

            b.Draw(
                ChatBox.emojiTexture,
                new Vector2(xPositionOnScreen + 18, yPositionOnScreen + 18),
                new Rectangle(54, 27, 9, 9),
                Color.White * alpha,
                0f,
                Vector2.Zero,
                3f,
                SpriteEffects.None,
                0.1f
            );

            Utility.drawBoldText(
                b,
                DisplayText,
                Game1.smallFont,
                new(xPositionOnScreen + (width / 2) - (DisplayTextSize.X / 2) + 18, yPositionOnScreen + 18),
                Game1.textColor * alpha
            );

            base.draw(b);
        }
    }
}

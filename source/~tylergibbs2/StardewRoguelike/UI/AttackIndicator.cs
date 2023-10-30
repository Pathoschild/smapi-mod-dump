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

namespace StardewRoguelike.UI
{
    internal class AttackIndicator : IClickableMenu
    {
        private float DurationRemaining;

        public AttackIndicator(float durationTicks) : base()
        {
            // ticks -> millis
            DurationRemaining = durationTicks / 60 * 1000;
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            width = 18;
            height = 26;

            xPositionOnScreen = width;
            yPositionOnScreen = Game1.uiViewport.Height - 54 - height;
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

            DurationRemaining -= (float)time.ElapsedGameTime.TotalMilliseconds;

            if (DurationRemaining <= 0)
                destroy = true;
        }

        public override void draw(SpriteBatch b)
        {
            if (destroy)
                return;

            float alpha = 1;

            drawTextureBox(
                b,
                xPositionOnScreen,
                yPositionOnScreen,
                width + 36,
                height + 36,
                Color.White * alpha
            );

            b.Draw(
                ChatBox.emojiTexture,
                new Vector2(xPositionOnScreen + 18, yPositionOnScreen + 18),
                new Rectangle(54, 81, 9, 9),
                Color.White * alpha,
                0f,
                Vector2.Zero,
                3f,
                SpriteEffects.None,
                0.1f
            );

            base.draw(b);
        }
    }
}

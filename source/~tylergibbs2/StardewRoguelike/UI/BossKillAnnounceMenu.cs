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
    internal class BossKillAnnounceMenu : IClickableMenu
    {
        private static readonly float DisplayDuration = 7500;

        private static readonly string DefeatedText = I18n.UI_BossKillAnnounceMenu_Defeated();

        private static readonly Vector2 DefeatedTextSize = Game1.smallFont.MeasureString(DefeatedText);

        private float durationRemaining = DisplayDuration;

        private readonly string BossText;

        private readonly string TimeText;

        private readonly Vector2 BossTextSize;

        private readonly Vector2 TimeTextSize;

        public BossKillAnnounceMenu(string bossName, int killSeconds) : base()
        {
            TimeSpan duration = TimeSpan.FromSeconds(killSeconds);
            BossText = bossName;
            TimeText = $"{duration:hh\\h\\ mm\\m\\ ss\\s}";

            BossTextSize = Game1.smallFont.MeasureString(BossText);
            TimeTextSize = Game1.smallFont.MeasureString(TimeText);

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            width = (int)Math.Max(Math.Max(BossTextSize.X, TimeTextSize.X), DefeatedTextSize.X) + 32;
            height = (int)DefeatedTextSize.Y + (int)BossTextSize.Y + (int)TimeTextSize.Y + 32;

            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width / 2);
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

        internal static bool IsToolbarOnTop()
        {
            foreach (var menu in Game1.onScreenMenus)
            {
                if (menu is Toolbar toolbar)
                    return toolbar.yPositionOnScreen <= 200;
            }

            return false;
        }

        public override void draw(SpriteBatch b)
        {
            if (destroy || ModEntry.Config.DisableBossSplits)
                return;

            float alpha = durationRemaining > 1250 ? 1f : durationRemaining / 1250;

            int drawHeight = yPositionOnScreen;
            if (IsToolbarOnTop())
                drawHeight += 150;

            drawTextureBox(
                b,
                xPositionOnScreen,
                drawHeight,
                width,
                height,
                Color.White * alpha
            );

            Utility.drawBoldText(
                b,
                DefeatedText,
                Game1.smallFont,
                new(xPositionOnScreen + (width / 2) - (DefeatedTextSize.X / 2), yPositionOnScreen + 18),
                Color.Red * alpha
            );

            Utility.drawTextWithShadow(
                b,
                BossText,
                Game1.smallFont,
                new(xPositionOnScreen + (width / 2) - (BossTextSize.X / 2), yPositionOnScreen + 18 + DefeatedTextSize.Y),
                Game1.textColor * alpha
            );

            Utility.drawTextWithShadow(
                b,
                TimeText,
                Game1.smallFont,
                new(xPositionOnScreen + (width / 2) - (TimeTextSize.X / 2), yPositionOnScreen + 18 + DefeatedTextSize.Y + BossTextSize.Y),
                Game1.textColor * alpha
            );

            base.draw(b);
        }
    }
}

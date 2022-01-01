/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace BattleRoyale.UI
{
    class VictoryRoyale : IClickableMenu
    {
        public static readonly int MillisecondsToShowFor = 15000;
        public static readonly float SpeedModifier = 15f;

        public Farmer Player;

        public VictoryRoyale(Farmer player)
        {
            calculatePositions();
            Player = player;
        }

        public void calculatePositions()
        {
            width = Game1.daybg.Width;
            height = Game1.daybg.Height;

            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - height;
        }

        public TimeSpan? GetTimeSince()
        {
            Round round = ModEntry.BRGame.GetActiveRound();
            if (round == null || round.EndTime == null)
                return null;

            return (TimeSpan)(DateTime.Now - round.EndTime);
        }

        public float GetAlpha()
        {
            TimeSpan? timeSince = GetTimeSince();
            if (timeSince == null)
                return 0f;

            if (((TimeSpan)timeSince).TotalMilliseconds > MillisecondsToShowFor)
            {
                Game1.onScreenMenus.Remove(this);
                return 0f;
            }

            float percentage = (float)(((TimeSpan)timeSince).TotalMilliseconds / (MillisecondsToShowFor / SpeedModifier));

            return percentage > 1f ? 1f : percentage;
        }

        public override void draw(SpriteBatch b)
        {
            float alpha = GetAlpha();
            calculatePositions();

            Texture2D bg = Game1.timeOfDay > 1500 ? Game1.nightbg : Game1.daybg;

            b.Draw(bg, new Vector2(xPositionOnScreen, yPositionOnScreen), Color.White * alpha);
            Player.FarmerRenderer.draw(b, Player.FarmerSprite.CurrentAnimationFrame, Player.FarmerSprite.CurrentFrame, Player.FarmerSprite.SourceRect, new Vector2(xPositionOnScreen + width / 2 - 32, yPositionOnScreen + height - 160), Vector2.Zero, 0.8f, Color.White * alpha, 0f, 1f, Player);
            SpriteText.drawStringWithScrollCenteredAt(b, "Winner!", xPositionOnScreen + width / 2, yPositionOnScreen - 80, scrollType: 1, alpha: alpha);
            SpriteText.drawStringWithScrollCenteredAt(b, Player.Name, xPositionOnScreen + width / 2, yPositionOnScreen + height - 8, alpha: alpha);

            if (Player.isEmoting)
            {
                Vector2 emotePosition = new(xPositionOnScreen + width / 2 - 32, yPositionOnScreen + height - 64);
                emotePosition.Y -= 160f;
                b.Draw(Game1.emoteSpriteSheet, emotePosition, new Rectangle(Player.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, Player.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.81f);
            }
        }
    }
}

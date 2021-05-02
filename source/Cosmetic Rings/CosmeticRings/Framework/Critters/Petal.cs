/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CosmeticRings
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmeticRings.Framework.Critters
{
    internal class Petal : Critter
    {
        public int which;
        public int animationIndex;
        public int animationTimer = 100;
        public int animationDirection = 1;
        public int animationIntervalOffset;

        public float dx;
        public float dy;

        public Rectangle sourceRect;

        private int alpha;
        private int lifeTimer;
        private int fadeTimer;
        private bool isFlipped;
        private bool blowing;
        private bool slow = false;

        public Petal(Vector2 position, int which, float rotationVelocity, float dx, float dy)
        {
            this.position = (position * 64f) + new Vector2(Game1.random.Next(-16, 48), -96f);
            this.isFlipped = Game1.random.Next(0, 2) == 1 ? true : false;
            this.which = which;
            this.dx = dx;
            this.dy = dy;

            switch (which)
            {
                case 0:
                    this.sourceRect = new Rectangle(352, 1184, 16, 16);
                    this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
                    break;
                case 1:
                    this.sourceRect = new Rectangle(352, 1200, 16, 16);
                    this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
                    break;
                case 2:
                    this.sourceRect = new Rectangle(352, 1216, 16, 16);
                    this.animationIntervalOffset = (Game1.random.Next(25) - 12) * 2;
                    break;
                case 3:
                    this.sourceRect = new Rectangle(391 + 4 * Game1.random.Next(5), 1236, 4, 4);
                    break;
            }

            alpha = 255;
            lifeTimer = Game1.random.Next(2000, 3000);
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            this.position.X += this.dx + (slow ? 0f : WeatherDebris.globalWind);
            this.position.Y += this.dy - (slow ? 0f : (-0.5f));
            if (this.dy < 0f && !this.blowing)
            {
                this.dy += 0.01f;
            }

            this.lifeTimer -= time.ElapsedGameTime.Milliseconds;
            if (lifeTimer <= 0)
            {
                this.fadeTimer -= time.ElapsedGameTime.Milliseconds;
                if (fadeTimer <= 0)
                {
                    fadeTimer = 100;
                    alpha -= 25;

                    if (alpha <= 0)
                    {
                        return true;
                    }
                }
            }



            this.animationTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.animationTimer > 0)
            {
                return false;
            }
            this.animationTimer = 100 + this.animationIntervalOffset;
            this.animationIndex += this.animationDirection;
            if (this.animationDirection == 0)
            {
                if (this.animationIndex >= 9)
                {
                    this.animationDirection = -1;
                }
                else
                {
                    this.animationDirection = 1;
                }
            }
            if (this.animationIndex > 10)
            {
                if (Game1.random.NextDouble() < 0.82)
                {
                    this.animationIndex--;
                    this.animationDirection = 0;
                    this.dx += 0.1f;
                    this.dy -= 0.2f;
                }
                else
                {
                    this.animationIndex = 0;
                }
            }
            else if (this.animationIndex == 4 && this.animationDirection == -1)
            {
                this.animationIndex++;
                this.animationDirection = 0;
                this.dx -= 0.1f;
                this.dy -= 0.1f;
            }
            if (this.animationIndex == 7 && this.animationDirection == -1)
            {
                this.dy -= 0.2f;
            }
            if (this.which != 3)
            {
                this.sourceRect.X = 352 + this.animationIndex * 16;
            }

            return false;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.position), this.sourceRect, new Color(255 - (255 - alpha), 255 - (255 - alpha), 255 - (255 - alpha), alpha), 0f, Vector2.Zero, 3f, isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {

        }
    }
}

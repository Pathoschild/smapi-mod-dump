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
    internal class RainCloud : Critter
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

        private float scale = 1f;
        private float elapsedTime;
        private int raindropSpeed;

        public RainCloud(Vector2 position, int which, float rotationVelocity, float dx, float dy)
        {
            this.position = (position * 64f) + new Vector2(32f, -96f);
            this.isFlipped = Game1.random.Next(0, 2) == 1 ? true : false;
            this.which = which;
            this.dx = dx;
            this.dy = dy;

            alpha = 255;
            lifeTimer = Game1.random.Next(2000, 3000);

            base.baseFrame = 0;
            base.sprite = new AnimatedSprite(ResourceManager.raindropsTexturePath, base.baseFrame, 16, 16);
            base.sprite.loop = false;
            base.startingPosition = position;

            raindropSpeed = 50;
        }

        internal void doneWithRaindrop(Farmer who)
        {

        }

        public override bool update(GameTime time, GameLocation environment)
        {
            Vector2 targetPosition = Game1.player.position + new Vector2(32f, -96f);
            Vector2 smoothedPositionSlow = Vector2.Lerp(this.position, targetPosition, 0.03f);

            this.position = smoothedPositionSlow;

            this.scale = 0.15f * (6f + (float)Math.Sin(2 * Math.PI * elapsedTime));
            elapsedTime = (elapsedTime + (float)time.ElapsedGameTime.TotalMilliseconds / 3000) % 1;

            if (base.sprite.CurrentAnimation == null)
            {
                base.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                {
                    new FarmerSprite.AnimationFrame(base.baseFrame + 1, raindropSpeed),
                    new FarmerSprite.AnimationFrame(base.baseFrame + 2, raindropSpeed),
                    new FarmerSprite.AnimationFrame(base.baseFrame + 3, raindropSpeed),
                    new FarmerSprite.AnimationFrame(base.baseFrame + 2, raindropSpeed),
                    new FarmerSprite.AnimationFrame(base.baseFrame + 1, raindropSpeed),
                    new FarmerSprite.AnimationFrame(base.baseFrame, raindropSpeed, secondaryArm: false, flip: false, doneWithRaindrop)
                });
            }

            return base.update(time, environment); ;
        }

        public override void draw(SpriteBatch b)
        {

        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            base.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-16f, 12f)), base.position.Y / 10000f, 0, 0, Color.White, base.flip, 2f);
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.position), new Rectangle(648, 1045, 52, 33), Color.White, 0f, new Vector2(26f, 16f), this.scale, isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
        }
    }
}

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
    internal class FireflyFollower : Critter
    {
        private bool glowing;
        private float elapsedTime;
        private int id;
        private Vector2 motion;
        private float motionMultiplier;
        internal LightSource light;
        private float spawnOffsetY;
        private float spawnOffsetX;

        public FireflyFollower()
        {
        }

        public FireflyFollower(Vector2 position)
        {
            // Determine if X spawn will be to left or right of player
            spawnOffsetX = 20f + (Game1.random.Next(0, 2) * 64f);
            spawnOffsetY = 30f + Game1.random.Next(0, 5);

            base.baseFrame = -1;
            base.position = position * 64f;
            base.startingPosition = position * 64f;
            this.motion = new Vector2((float)Game1.random.Next(-10, 11) * 0.1f, (float)Game1.random.Next(-10, 11) * 0.1f);
            this.motionMultiplier = 1f;
            this.id = (int)(position.X * 10099f + position.Y * 77f + (float)Game1.random.Next(99999));
            this.light = new LightSource(4, position, (float)Game1.random.Next(1, 6) * 0.1f, Color.Purple * 0.8f, this.id, LightSource.LightContext.None, 0L);
            this.glowing = true;
            Game1.currentLightSources.Add(this.light);
        }

        internal void resetForNewLocation(Vector2 position)
        {
            base.position = position * 64f;
            base.startingPosition = base.position;
            Game1.currentLightSources.Add(this.light);
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            this.light.radius.Value = 0.1f * (5f + (float)Math.Sin(2 * Math.PI * elapsedTime));
            elapsedTime = (elapsedTime + (float)time.ElapsedGameTime.TotalMilliseconds / 3000) % 1;

            if (Game1.player.isMoving())
            {
                spawnOffsetX = Game1.player.position.X + spawnOffsetX < this.position.X ? 84f : 20f;
                this.motionMultiplier = 1f;
            }

            Vector2 targetPosition = Game1.player.position + new Vector2(spawnOffsetX, spawnOffsetY);
            Vector2 smoothedPosition = Vector2.Lerp(this.position, targetPosition, 0.05f);
            Vector2 smoothedPositionSlow = Vector2.Lerp(this.position, targetPosition, 0.02f);

            // Setting up wander zone
            if (Vector2.Distance(targetPosition, this.position) >= 64f && Game1.player.isMoving())
            {
                this.position = smoothedPosition;
            }
            else
            {
                this.position = smoothedPositionSlow;
            }

            base.position += this.motion * this.motionMultiplier;

            this.motion.X += (float)Game1.random.Next(-1, 2) * 0.1f;
            this.motion.Y += (float)Game1.random.Next(-1, 2) * 0.1f;
            if (this.motion.X < -1f)
            {
                this.motion.X = -1f;
            }
            if (this.motion.X > 1f)
            {
                this.motion.X = 1f;
            }
            if (this.motion.Y < -1f)
            {
                this.motion.Y = -1f;
            }
            if (this.motion.Y > 1f)
            {
                this.motion.Y = 1f;
            }
            if (this.glowing)
            {
                this.light.position.Value = base.position;
            }

            return false;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.staminaRect, Game1.GlobalToLocal(base.position), Game1.staminaRect.Bounds, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, base.position.Y / 10000f);
        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {

        }
    }
}

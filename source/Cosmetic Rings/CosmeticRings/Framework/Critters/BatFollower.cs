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
    internal class BatFollower : Critter
    {
        private float elapsedTime;
        private ICue batFlap;
        private Vector2 motion;
        private float motionMultiplier;

        private float spawnOffsetY;
        private float spawnOffsetX;

        public BatFollower()
        {
        }

        public BatFollower(Vector2 position)
        {
            // Determine if X spawn will be to left or right of player
            spawnOffsetX = -32f + (Game1.random.Next(0, 2) * 96f);
            spawnOffsetY = -80f - Game1.random.Next(0, 16);

            base.baseFrame = 0;
            base.position = position * 64f;

            switch (Game1.random.Next(6))
            {
                case 0:
                    base.sprite = new AnimatedSprite("Characters\\Monsters\\Bat");
                    break;
                case 1:
                    base.sprite = new AnimatedSprite("Characters\\Monsters\\Bat_dangerous");
                    break;
                case 2:
                    base.sprite = new AnimatedSprite("Characters\\Monsters\\Frost Bat");
                    break;
                case 3:
                    base.sprite = new AnimatedSprite("Characters\\Monsters\\Frost Bat_dangerous");
                    break;
                case 4:
                    base.sprite = new AnimatedSprite("Characters\\Monsters\\Iridium Bat");
                    break;
                case 5:
                    base.sprite = new AnimatedSprite("Characters\\Monsters\\Lava Bat");
                    break;
            }
            base.startingPosition = position * 64f;

            this.motion = new Vector2((float)Game1.random.Next(-10, 11) * 0.1f, (float)Game1.random.Next(-10, 11) * 0.1f);
            this.motionMultiplier = 1f;
        }

        internal void resetForNewLocation(Vector2 position)
        {
            base.position = position * 64f;
            base.startingPosition = base.position;
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            if (Game1.player.isMoving() && !(Game1.player.position.X - 16f < this.position.X && Game1.player.position.X + 32f > this.position.X))
            {
                spawnOffsetX = Game1.player.position.X + spawnOffsetX < this.position.X ? 64f : -32f;
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

            elapsedTime = (elapsedTime + (float)time.ElapsedGameTime.TotalMilliseconds / 250) % 1;
            this.position.Y += 2f * ((float)Math.Sin(2 * Math.PI * elapsedTime));

            base.position += this.motion * this.motionMultiplier;
            this.motionMultiplier -= 0.0005f * (float)time.ElapsedGameTime.Milliseconds;
            if (this.motionMultiplier <= 0f)
            {
                this.motionMultiplier = 1f;
            }

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

            this.sprite.Animate(time, 0, 4, 80f);
            if (this.sprite.currentFrame % 3 == 0 && (this.batFlap == null || !this.batFlap.IsPlaying) && Game1.soundBank != null)
            {
                this.batFlap = Game1.soundBank.GetCue("batFlap");
                this.batFlap.Play();
            }

            return false;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(this.sprite.Texture, Game1.GlobalToLocal(base.position), this.sprite.SourceRect, Color.White, 0f, new Vector2(8f, 16f), 3.5f, base.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.92f);
            //b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(base.position) + new Vector2(0f, 32f), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 2f, SpriteEffects.None, 0.0001f);
        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {

        }
    }
}

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
    internal class ButterflyFollower : Critter
    {
        internal bool summerButterfly;

        private int flapTimer;
        private int checkForLandingSpotTimer;
        private bool hasLanded;
        private int flapSpeed = 50;
        private Vector2 motion;
        private Vector2 landingSpot;
        private float motionMultiplier = 1f;
        public bool stayInbounds;

        private float spawnOffsetY;
        private float spawnOffsetX;

        public ButterflyFollower(Vector2 position)
        {
            // Determine if X spawn will be to left or right of player
            spawnOffsetX = 20f + (Game1.random.Next(0, 2) * 64f);
            spawnOffsetY = 30f + Game1.random.Next(0, 5);

            base.position = position * 64f;
            base.startingPosition = base.position;

            switch (Game1.random.NextDouble())
            {
                case var chance when chance <= 0.15:
                    // Rare chance for island butterflies
                    base.baseFrame = Game1.random.Next(4) * 4 + 364;
                    this.summerButterfly = true;
                    break;
                case var chance when chance <= 0.50:
                    // Common chance for summer butterflies
                    base.baseFrame = ((Game1.random.NextDouble() < 0.5) ? (Game1.random.Next(3) * 4 + 128) : (Game1.random.Next(3) * 4 + 148));
                    this.summerButterfly = true;
                    break;
                default:
                    // Spring butterflies are default
                    base.baseFrame = ((Game1.random.NextDouble() < 0.5) ? (Game1.random.Next(3) * 3 + 160) : (Game1.random.Next(3) * 3 + 180));
                    break;
            }


            this.motion = new Vector2((float)(Game1.random.NextDouble() + 0.25) * 3f * (float)((!(Game1.random.NextDouble() < 0.5)) ? 1 : (-1)) / 2f, (float)(Game1.random.NextDouble() + 0.5) * 3f * (float)((!(Game1.random.NextDouble() < 0.5)) ? 1 : (-1)) / 2f);
            this.flapSpeed = Game1.random.Next(45, 80);
            base.sprite = new AnimatedSprite(Critter.critterTexture, base.baseFrame, 16, 16);
            base.sprite.loop = false;
            base.startingPosition = position;

            this.checkForLandingSpotTimer = 2000;

            landingSpot = new Vector2(Game1.random.Next(56, 73), Game1.random.Next(28, 37));
        }

        internal void resetForNewLocation(Vector2 position)
        {
            base.position = position * 64f;
            base.startingPosition = base.position;
        }

        internal void doneWithFlap(Farmer who)
        {
            this.flapTimer = 200 + Game1.random.Next(-5, 6);
        }

        internal void performFlap(GameTime time, bool applyMotion, bool overrideAnimation = false)
        {
            this.flapTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.flapTimer <= 0 && (base.sprite.CurrentAnimation == null || overrideAnimation))
            {
                if (this.summerButterfly)
                {
                    base.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(base.baseFrame + 1, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame + 2, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame + 3, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame + 2, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame + 1, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame, this.flapSpeed, secondaryArm: false, flip: false, doneWithFlap)
                    });
                }
                else
                {
                    base.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(base.baseFrame + 1, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame + 2, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame + 1, this.flapSpeed),
                        new FarmerSprite.AnimationFrame(base.baseFrame, this.flapSpeed, secondaryArm: false, flip: false, doneWithFlap)
                    });
                }

                if (applyMotion)
                {
                    this.motionMultiplier = 1f;
                    this.motion.X += (float)Game1.random.Next(-80, 81) / 100f;
                    this.motion.Y = (float)(Game1.random.NextDouble() + 0.25) * -3f / 2f;
                    if (Math.Abs(this.motion.X) > 1.5f)
                    {
                        this.motion.X = 3f * (float)Math.Sign(this.motion.X) / 2f;
                    }
                    if (Math.Abs(this.motion.Y) > 3f)
                    {
                        this.motion.Y = 3f * (float)Math.Sign(this.motion.Y);
                    }
                }
            }

            if (applyMotion)
            {
                base.position += this.motion * this.motionMultiplier;
                this.motion.Y += 0.005f * (float)time.ElapsedGameTime.Milliseconds;
                this.motionMultiplier -= 0.0005f * (float)time.ElapsedGameTime.Milliseconds;
                if (this.motionMultiplier <= 0f)
                {
                    this.motionMultiplier = 0f;
                }
            }
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            this.checkForLandingSpotTimer = Game1.player.isMoving() ? Game1.random.Next(5000, 10000) : checkForLandingSpotTimer - time.ElapsedGameTime.Milliseconds;
            if (this.checkForLandingSpotTimer <= 0)
            {
                performFlap(time, false);

                if (Vector2.Distance(Game1.player.position + landingSpot, this.position) <= 2f && !this.hasLanded)
                {
                    this.hasLanded = true;
                    this.flapSpeed = Game1.random.Next(550, 1000);
                }
                else
                {
                    this.position = Vector2.Lerp(this.position, Game1.player.position + landingSpot, 0.02f);
                }

                return base.update(time, environment);
            }

            if (this.hasLanded)
            {
                this.hasLanded = false;
                this.flapSpeed = Game1.random.Next(45, 80);
                performFlap(time, true, true);
            }

            if (Game1.player.isMoving())
            {
                spawnOffsetX = Game1.player.position.X + spawnOffsetX < this.position.X ? 84f : 20f;
                this.flapSpeed = Math.Max(35, flapSpeed - 1);
            }
            else if (this.flapSpeed == 35)
            {
                this.flapSpeed = Game1.random.Next(45, 80);
            }

            Vector2 targetPosition = Game1.player.position + new Vector2(spawnOffsetX, spawnOffsetY);
            Vector2 smoothedPosition = Vector2.Lerp(this.position, targetPosition, 0.05f);
            Vector2 smoothedPositionSlow = Vector2.Lerp(this.position, targetPosition, 0.02f);

            // Setting up wander zone
            if (Vector2.Distance(targetPosition, this.position) >= 64f)
            {
                this.position = smoothedPosition;
            }
            else
            {
                this.position = smoothedPositionSlow;
            }

            performFlap(time, true);
            return base.update(time, environment);
        }

        public override void draw(SpriteBatch b)
        {
            base.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, base.position + new Vector2(-64f, -128f + base.yJumpOffset + base.yOffset)), base.position.Y / 10000f, 0, 0, Color.White, base.flip, 4f);
        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {

        }
    }
}

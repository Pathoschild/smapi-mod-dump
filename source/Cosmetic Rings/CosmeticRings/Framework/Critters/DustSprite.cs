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
using Netcode;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmeticRings.Framework.Critters
{
    internal class DustSprite : NPC
    {
        public byte voice;
        private ICue meep;
        private int meepTimer;
        private int movementTimer;
        private readonly NetVector2 motion = new NetVector2(Vector2.Zero);
        private new readonly NetRectangle nextPosition = new NetRectangle();

        public DustSprite(Vector2 position) : base(new AnimatedSprite("Characters\\Monsters\\Dust Spirit"), position * 64f, 2, "DustSprite")
        {
            base.Breather = false;
            base.speed = CosmeticRings.config.walkingSpeed;
            base.forceUpdateTimer = 9999;
            base.collidesWithOtherCharacters.Value = false;
            base.farmerPassesThrough = true;
            base.HideShadow = true;
            this.nextPosition.Value = this.GetBoundingBox();

            this.voice = (byte)Game1.random.Next(1, 24);
            this.Sprite.interval = 45f;
            base.Scale = (float)Game1.random.Next(55, 70) / 100f;
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.currentLocation = location;
            base.update(time, location);
            base.forceUpdateTimer = 99999;

            Farmer f = Utility.isThereAFarmerWithinDistance(base.getTileLocation(), 10, base.currentLocation);
            if (f != null)
            {
                meepTimer -= time.ElapsedGameTime.Milliseconds;
                if (base.yJumpOffset == 0)
                {
                    this.jumpWithoutSound();
                    base.yJumpVelocity = (float)Game1.random.Next(50, 70) / 10f;

                    if (Game1.random.NextDouble() < 0.1 && (this.meep == null || !this.meep.IsPlaying) && Game1.soundBank != null && meepTimer <= 0)
                    {
                        this.meep = Game1.soundBank.GetCue("dustMeep");
                        this.meep.SetVariable("Pitch", this.voice * 100 + Game1.random.Next(-100, 100));
                        this.meep.Play();

                        meepTimer = Game1.random.Next(5000, 8000);
                    }
                }
                this.Sprite.AnimateDown(time);

                if (base.yJumpOffset == 0)
                {
                    if (Game1.random.NextDouble() < 0.01)
                    {
                        base.yJumpVelocity *= 2f;
                    }
                }

                Vector2 v = Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), f);
                base.xVelocity += (0f - v.X) / 150f + ((Game1.random.NextDouble() < 0.01) ? ((float)Game1.random.Next(-50, 50) / 10f) : 0f);
                if (Math.Abs(base.xVelocity) > 5f)
                {
                    base.xVelocity = Math.Sign(base.xVelocity) * 5;
                }
                base.yVelocity += (0f - v.Y) / 150f + ((Game1.random.NextDouble() < 0.01) ? ((float)Game1.random.Next(-50, 50) / 10f) : 0f);
                if (Math.Abs(base.yVelocity) > 5f)
                {
                    base.yVelocity = Math.Sign(base.yVelocity) * 5;
                }

                movementTimer = !isMoving() && Vector2.Distance(base.Position, f.Position) > 256f ? movementTimer - time.ElapsedGameTime.Milliseconds : 1000;
                if (Vector2.Distance(base.Position, f.Position) > 640f || movementTimer <= 0)
                {
                    base.position.Value = f.position;
                    this.movementTimer = 1000;
                }
                else if (Vector2.Distance(base.Position, f.Position) > 64f)
                {
                    if (Game1.random.NextDouble() < 0.007)
                    {
                        this.jumpWithoutSound(Game1.random.Next(6, 9));
                    }
                    this.setMoving(Utility.getVelocityTowardPlayer(new Point((int)base.Position.X, (int)base.Position.Y), base.speed, f));
                }
                else
                {
                    this.motion.Value = Vector2.Zero;
                }
            }

            if (!base.IsInvisible && base.controller == null)
            {
                this.nextPosition.Value = this.GetBoundingBox();
                this.nextPosition.X += (int)this.motion.X;
                if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, this))
                {
                    base.position.X += (int)this.motion.X;
                }
                this.nextPosition.X -= (int)this.motion.X;
                this.nextPosition.Y += (int)this.motion.Y;
                if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, this))
                {
                    base.position.Y += (int)this.motion.Y;
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(this.Sprite.Texture, base.getLocalPosition(Game1.viewport) + new Vector2(32 + ((base.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), 64 + base.yJumpOffset), this.Sprite.SourceRect, Color.White, base.rotation, new Vector2(8f, 16f), new Vector2((float)base.scale + (float)Math.Max(-0.1, (double)(base.yJumpOffset + 32) / 128.0), (float)base.scale - Math.Max(-0.1f, (float)base.yJumpOffset / 256f)) * 4f, base.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, base.drawOnTop ? 0.991f : ((float)base.getStandingY() / 10000f)));
            if (base.isGlowing)
            {
                b.Draw(this.Sprite.Texture, base.getLocalPosition(Game1.viewport) + new Vector2(32f, 64 + base.yJumpOffset), this.Sprite.SourceRect, base.glowingColor * base.glowingTransparency, base.rotation, new Vector2(8f, 16f), Math.Max(0.2f, base.scale) * 4f, base.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, base.drawOnTop ? 0.99f : ((float)base.getStandingY() / 10000f + 0.001f)));
            }
            b.Draw(Game1.shadowTexture, base.getLocalPosition(Game1.viewport) + new Vector2(32f, 80f), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 2f + (float)base.yJumpOffset / 64f, SpriteEffects.None, (float)(base.getStandingY() - 1) / 10000f);
        }

        public void setMoving(Vector2 motion)
        {
            this.motion.Value = motion;
        }

        internal void resetForNewLocation(Vector2 position)
        {
            base.Position = position * 64f;
        }
    }
}

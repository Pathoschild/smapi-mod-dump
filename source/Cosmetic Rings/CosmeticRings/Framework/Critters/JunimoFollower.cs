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
    internal class JunimoFollower : NPC
    {
        private int jumpTimer;
        private int movementTimer;
        private readonly NetVector2 motion = new NetVector2(Vector2.Zero);
        private new readonly NetRectangle nextPosition = new NetRectangle();
        private readonly NetColor color = new NetColor();


        public JunimoFollower(Vector2 position) : base(new AnimatedSprite("Characters\\Junimo", 0, 16, 16), position * 64f, 2, "JunimoFollower")
        {
            base.Breather = false;
            base.speed = CosmeticRings.config.walkingSpeed;
            base.forceUpdateTimer = 9999;
            base.collidesWithOtherCharacters.Value = false;
            base.farmerPassesThrough = true;
            base.HideShadow = true;
            base.Scale = 0.75f;
            this.nextPosition.Value = this.GetBoundingBox();

            if (Game1.random.NextDouble() < 0.25)
            {
                switch (Game1.random.Next(8))
                {
                    case 0:
                        this.color.Value = Color.Red;
                        break;
                    case 1:
                        this.color.Value = Color.Goldenrod;
                        break;
                    case 2:
                        this.color.Value = Color.Yellow;
                        break;
                    case 3:
                        this.color.Value = Color.Lime;
                        break;
                    case 4:
                        this.color.Value = new Color(0, 255, 180);
                        break;
                    case 5:
                        this.color.Value = new Color(0, 100, 255);
                        break;
                    case 6:
                        this.color.Value = Color.MediumPurple;
                        break;
                    case 7:
                        this.color.Value = Color.Salmon;
                        break;
                }
                if (Game1.random.NextDouble() < 0.01)
                {
                    this.color.Value = Color.White;
                }
            }
            else
            {
                switch (Game1.random.Next(8))
                {
                    case 0:
                        this.color.Value = Color.LimeGreen;
                        break;
                    case 1:
                        this.color.Value = Color.Orange;
                        break;
                    case 2:
                        this.color.Value = Color.LightGreen;
                        break;
                    case 3:
                        this.color.Value = Color.Tan;
                        break;
                    case 4:
                        this.color.Value = Color.GreenYellow;
                        break;
                    case 5:
                        this.color.Value = Color.LawnGreen;
                        break;
                    case 6:
                        this.color.Value = Color.PaleGreen;
                        break;
                    case 7:
                        this.color.Value = Color.Turquoise;
                        break;
                }
            }
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.currentLocation = location;
            base.update(time, location);
            base.forceUpdateTimer = 99999;

            Farmer f = Utility.isThereAFarmerWithinDistance(base.getTileLocation(), 10, base.currentLocation);
            if (f != null)
            {
                jumpTimer -= time.ElapsedGameTime.Milliseconds;
                movementTimer = !isMoving() && Vector2.Distance(base.Position, f.Position) > 256f ? movementTimer - time.ElapsedGameTime.Milliseconds : 1000;
                if (Vector2.Distance(base.Position, f.Position) > 640f || movementTimer <= 0)
                {
                    this.jump();
                    base.position.Value = f.position;
                    this.movementTimer = 1000;
                }
                else if (Vector2.Distance(base.Position, f.Position) > 64f)
                {
                    if (this.motion.Equals(Vector2.Zero))
                    {
                        if (Game1.random.NextDouble() < 0.4)
                        {
                            this.jump();
                        }
                        else
                        {
                            this.jumpWithoutSound(Game1.random.Next(6, 9));
                        }
                    }
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
            if (base.controller != null || !this.motion.Equals(Vector2.Zero))
            {
                if (base.moveRight || (Math.Abs(this.motion.X) > Math.Abs(this.motion.Y) && this.motion.X > 0f))
                {
                    base.flip = false;
                    this.Sprite.Animate(time, 16, 8, 50f);
                    this.FacingDirection = 1;
                }
                else if (base.moveLeft || (Math.Abs(this.motion.X) > Math.Abs(this.motion.Y) && this.motion.X < 0f))
                {
                    base.flip = true;
                    this.Sprite.Animate(time, 16, 8, 50f);
                    this.FacingDirection = 3;
                }
                else if (base.moveUp || (Math.Abs(this.motion.Y) > Math.Abs(this.motion.X) && this.motion.Y < 0f))
                {
                    this.Sprite.Animate(time, 32, 8, 50f);
                    this.FacingDirection = 0;
                }
                else
                {
                    this.Sprite.Animate(time, 0, 8, 50f);
                    this.FacingDirection = 2;
                }
            }
            else
            {
                switch (this.FacingDirection)
                {
                    case 0:
                        this.Sprite.Animate(time, 40, 4, 100f);
                        break;
                    case 1:
                        this.Sprite.Animate(time, 24, 4, 100f);
                        break;
                    case 2:
                        this.Sprite.Animate(time, 8, 4, 100f);
                        break;
                    case 3:
                        this.Sprite.Animate(time, 24, 4, 100f);
                        break;
                }
            }
        }

        public void setMoving(Vector2 motion)
        {
            this.motion.Value = motion;
        }

        internal void resetForNewLocation(Vector2 position)
        {
            base.Position = position * 64f;
        }

        public override void jump()
        {
            if (jumpTimer <= 0)
            {
                base.jump();
                this.currentLocation.localSound("junimoMeep1");
                jumpTimer = Game1.random.Next(500, 3000);
            }
        }

        public override void jumpWithoutSound(float velocity = 8)
        {
            if (jumpTimer <= 0)
            {
                base.jumpWithoutSound(velocity);
                jumpTimer = Game1.random.Next(500, 3000);
            }
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (!base.IsInvisible)
            {
                this.Sprite.UpdateSourceRect();
                b.Draw(this.Sprite.Texture, base.getLocalPosition(Game1.viewport) + new Vector2(this.Sprite.SpriteWidth * 4 / 2, (float)this.Sprite.SpriteHeight * 3f / 4f * 4f / (float)Math.Pow(this.Sprite.SpriteHeight / 16, 2.0) + (float)base.yJumpOffset - 8f) + ((base.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Sprite.SourceRect, this.color.Value, base.rotation, new Vector2(this.Sprite.SpriteWidth * 4 / 2, (float)(this.Sprite.SpriteHeight * 4) * 3f / 4f) / 4f, Math.Max(0.2f, base.scale) * 4f, base.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, base.drawOnTop ? 0.991f : ((float)base.getStandingY() / 10000f)));
                if (!base.swimming && !base.HideShadow)
                {
                    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, base.Position + new Vector2((float)(this.Sprite.SpriteWidth * 4) / 2f, 44f)), Game1.shadowTexture.Bounds, this.color.Value, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), (4f + (float)base.yJumpOffset / 40f) * (float)base.scale, SpriteEffects.None, Math.Max(0f, (float)base.getStandingY() / 10000f) - 1E-06f);
                }
            }
        }
    }
}

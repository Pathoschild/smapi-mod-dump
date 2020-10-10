/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace CustomMonsters.Framework.Monsters
{
    internal class CustomBigSlime : BigSlime 
    {
        public CustomBigSlime()
        {
        }

        public CustomBigSlime(Vector2 position, MineShaft mine)
            : this(position, mine.getMineArea(-1))
        {
            this.Sprite.ignoreStopAnimation = true;
            this.ignoreMovementAnimations = true;
            this.HideShadow = true;
        }

        public CustomBigSlime(Vector2 position, int mineArea)
            : base(position, mineArea)
        {
            this.ignoreMovementAnimations = true;
            this.Sprite.ignoreStopAnimation = true;
            this.Sprite.SpriteWidth = 32;
            this.Sprite.SpriteHeight = 32;
            this.Sprite.UpdateSourceRect();
            this.Sprite.framesPerAnimation = 8;
            this.c.Value = Color.White;
            switch (mineArea)
            {
                case 0:
                    this.c.Value = Color.Green;
                    break;
                case 40:
                    this.c.Value = Color.Turquoise;
                    this.Health *= 2;
                    this.ExperienceGained *= 2;
                    break;
                case 80:
                    this.c.Value = Color.Red;
                    this.Health *= 3;
                    this.DamageToFarmer *= 2;
                    this.ExperienceGained *= 3;
                    break;
                case 121:
                    this.c.Value = Color.BlueViolet;
                    this.Health *= 4;
                    this.DamageToFarmer *= 3;
                    this.ExperienceGained *= 3;
                    break;
            }
            int r = (int)this.c.R;
            int g = (int)this.c.G;
            int b = (int)this.c.B;
            int val2_1 = r + Game1.random.Next(-20, 21);
            int val2_2 = g + Game1.random.Next(-20, 21);
            int val2_3 = b + Game1.random.Next(-20, 21);
            this.c.R = (byte)Math.Max(Math.Min((int)byte.MaxValue, val2_1), 0);
            this.c.G = (byte)Math.Max(Math.Min((int)byte.MaxValue, val2_2), 0);
            this.c.B = (byte)Math.Max(Math.Min((int)byte.MaxValue, val2_3), 0);
            Color c = this.c;
            c = c * ((float)Game1.random.Next(7, 11) / 10f);
            this.Sprite.interval = 300f;
            this.HideShadow = true;
        }

        public override void reloadSprite()
        {
            Sprite = new AnimatedSprite("Characters\\Monsters\\" + this.Name, 0, 16, 16);
            this.Sprite.SpriteWidth = 32;
            this.Sprite.SpriteHeight = 32;
            this.Sprite.interval = 300f;
            this.Sprite.ignoreStopAnimation = true;
            this.ignoreMovementAnimations = true;
            this.HideShadow = true;
            this.Sprite.UpdateSourceRect();
            this.Sprite.framesPerAnimation = 8;
        }

        public override int takeDamage(
            int damage,
            int xTrajectory,
            int yTrajectory,
            bool isBomb,
            double addedPrecision,
            Farmer who)
        {
            int num1 = Math.Max(1, damage - (int)(this.resilience));
            if (Game1.random.NextDouble() < (double)(this.missChance) - (double)(this.missChance) * addedPrecision)
            {
                num1 = -1;
            }
            else
            {
                this.Slipperiness = 3;
                this.Health -= num1;
                this.setTrajectory(xTrajectory, yTrajectory);
                this.currentLocation.playSound("hitEnemy");
                this.IsWalkingTowardPlayer = true;
                if (this.Health <= 0)
                {
                    this.deathAnimation();
                    ++Game1.stats.SlimesKilled;
                    if (Game1.gameMode == (byte)3 && Game1.random.NextDouble() < 0.75)
                    {
                        int num2 = Game1.random.Next(2, 5);
                        for (int index = 0; index < num2; ++index)
                        {
                            this.currentLocation.characters.Add((NPC)new GreenSlime(this.Position, Game1.CurrentMineLevel));
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].setTrajectory(xTrajectory / 8 + Game1.random.Next(-2, 3), yTrajectory / 8 + Game1.random.Next(-2, 3));
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].willDestroyObjectsUnderfoot = false;
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].moveTowardPlayer(4);
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].Scale = (float)(0.75 + (double)Game1.random.Next(-5, 10) / 100.0);
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].currentLocation = this.currentLocation;
                        }
                    }
                }
            }
            return num1;
        }

        protected override void localDeathAnimation()
        {
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position, (Color)(this.c), 10, false, 70f, 0, -1, -1f, -1, 0));
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2(-32f, 0.0f), (Color)(this.c), 10, false, 70f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 100
            });
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2(32f, 0.0f), (Color)(this.c), 10, false, 70f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 200
            });
            this.currentLocation.localSound("slimedead");
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2(0.0f, -32f), (Color)(this.c), 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 300
            });
        }

        protected override void updateAnimation(GameTime time)
        {
            int currentFrame = this.Sprite.currentFrame;
            this.Sprite.AnimateDown(time, 0, "");
            if (this.isMoving())
                this.Sprite.interval = 100f;
            else
                this.Sprite.interval = 200f;
            if (Utility.isOnScreen(this.Position, 128) && this.Sprite.currentFrame == 0 && currentFrame == 7)
                this.currentLocation.localSound("slimeHit");
            this.resetAnimationSpeed();
        }

        public override void draw(SpriteBatch b)
        {
            if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128))
                return;
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float)(16 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), (Color)(this.c), this.rotation, new Vector2(16f, 16f), Math.Max(0.2f, (float)(this.scale)) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)this.getStandingY() / 10000f));
            if (!this.isGlowing)
                return;
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float)(16 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, new Vector2(16f, 16f), 4f * Math.Max(0.2f, (float)(this.scale)), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)this.Position.X + 8, (int)this.Position.Y, this.Sprite.SpriteWidth * 4 * 3 / 4, 64);
        }

        public override void shedChunks(int number, float scale)
        {
        }
    }
}
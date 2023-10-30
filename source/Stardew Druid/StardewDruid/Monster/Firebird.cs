/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using static StardewValley.IslandGemBird;

namespace StardewDruid.Monster
{
    public class Firebird : StardewValley.Monsters.Monster
    {

        private float firingTimer;

        public float height;

        public int[] idleAnimation = new int[10]
        {
            1,1,1,1,1,1,1,1,1,1,
        };

        public int[] scratchAnimation = new int[19]
        {
            0, 1, 2, 3, 2, 3, 2, 3, 2, 3,
            2, 3, 2, 3, 2, 3, 2, 3, 2
        };

        public int[] flyAnimation = new int[11]
        {
            4, 5, 6, 7, 7, 6, 6, 5, 5, 4,
            4
        };

        public bool defeated;

        public int[] currentAnimation;

        public float frameTimer;

        public int currentTickIndex;

        public int currentFrameIndex;

        public float alpha = 1f;

        public string birdType;

        public Color birdColor;

        public int birdItem;

        public List<string> ouchList;

        public List<string> dialogueList;

        public Texture2D birdTexture;

        public Firebird(Vector2 vector, int combatModifier)
            : base("Shadow Brute",vector * 64)
        {

            birdTexture = Game1.content.Load<Texture2D>("LooseSprites\\GemBird");

            Health = combatModifier * 10;

            MaxHealth = Health;

            focusedOnFarmers = true;

            DamageToFarmer = (int)(combatModifier * 0.1);

            birdColor = new Color(255, 38, 38);

            birdItem = 64;

            objectsToDrop.Clear();

            firingTimer = Game1.random.Next(2, 6) * 1000f;

            ouchList = new()
            {
                "tweep tweep",
                "CAWW"

            };

        }

        public override void reloadSprite()
        {

        }

        public void setBirdType(string newType)
        {

            birdType = newType;

            birdColor = birdType switch
            {
                "Emerald" => new Color(67, 255, 83),
                "Aquamarine" => new Color(74, 243, 255),
                "Ruby" => new Color(255, 38, 38),
                "Amethyst" => new Color(255, 67, 251),
                "Topaz" => new Color(255, 156, 33),
                _ => Color.White,
            };

            birdItem = birdType switch 
            {
                "Emerald" => 60,
                "Aquamarine" => 62,
                "Ruby" => 64,
                "Amethyst" => 66,
                "Topaz" => 68,
                _ => 0,
            };

        }

        public override void draw(SpriteBatch b)
        {

            int num = 1;

            if (currentAnimation != null)
            {

                num = currentAnimation[Math.Min(currentFrameIndex, currentAnimation.Length - 1)];

                currentTickIndex++;

                if(currentTickIndex == 8)
                {

                    if (currentAnimation == flyAnimation)
                    {

                        alpha -= 0.1f;
                        
                    }

                    currentFrameIndex++;

                    if (currentFrameIndex == currentAnimation.Length)
                    {
                        
                        if (currentAnimation == flyAnimation)
                        {

                            defeated = true;

                            Health = 0;

                        }
                        else if(currentAnimation == scratchAnimation)
                        {

                            currentAnimation = idleAnimation;

                        }
                        else
                        {

                            currentAnimation = null;

                        }

                        currentFrameIndex = 0;

                    }

                    currentTickIndex = 0;

                }

                if (currentAnimation == flyAnimation)
                {
                    
                    height += 4f;

                    position.X -= 3f;

                }


            }

            b.Draw(birdTexture, Game1.GlobalToLocal(Game1.viewport, position.Value + new Vector2(0f, 0f - height)), new Rectangle(num * 32, 0, 32, 32), Color.White * alpha, 0f, new Vector2(8f, 0), 4f, SpriteEffects.None, (position.Value.Y - 1f) / 10000f);

            b.Draw(birdTexture, Game1.GlobalToLocal(Game1.viewport, position.Value + new Vector2(0f, 0f - height)), new Rectangle(num * 32, 32, 32, 32), birdColor * alpha, 0f, new Vector2(8f,0), 4f, SpriteEffects.None, position.Value.Y / 10000f);

            b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, position.Value), Game1.shadowTexture.Bounds, Color.White * alpha, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, (position.Y - 2f) / 10000f);

        }


        public override Rectangle GetBoundingBox()
        {
            Vector2 vector = Position;

            return new Rectangle((int)vector.X, (int)vector.Y+16, 64, 112);

        }


        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (currentAnimation == null)
            {
                currentAnimation = scratchAnimation;
            
                currentFrameIndex = 0;
            
            }

            if(currentAnimation == flyAnimation)
            {

                return 0;

            }

            if (damage >= Health)
            {

                currentAnimation = flyAnimation;

                currentFrameIndex = 0;

                Health = 1;

                int newDamage = Health - 1;

                showTextAboveHead("Tweeeeep!", duration: 3000);

                Game1.playSound("batFlap");

                Vector2 birdVector = getTileLocation();

                Game1.createObjectDebris(birdItem, (int)birdVector.X, (int)birdVector.Y);

                return newDamage;

            }

            Health -= damage;

            int ouchIndex = Game1.random.Next(15);

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 3000);

            }

            return damage;

        }

        public override void updateMovement(GameLocation location, GameTime time)
        {


        }

        public override void defaultMovementBehavior(GameTime time)
        {
            
        }

        protected override void sharedDeathAnimation()
        {

        }

        protected override void updateMonsterSlaveAnimation(GameTime time)
        {

        }

        public override void behaviorAtGameTick(GameTime time)
        {

            if (currentAnimation == null || currentAnimation == idleAnimation)
            {

                firingTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;

                if (firingTimer <= 0f)
                {

                    base.IsWalkingTowardPlayer = false;
                    
                    Vector2 vector = new Vector2(base.Position.X, base.Position.Y + 64f);
                    
                    Halt();
                    
                    Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(getStandingPosition(), new Vector2(base.Player.GetBoundingBox().X, base.Player.GetBoundingBox().Y) + new Vector2(Game1.random.Next(-128, 128)), 8f);
                    
                    BasicProjectile basicProjectile = new BasicProjectile(damageToFarmer, 10, 2, 4, 0f, velocityTowardPoint.X, velocityTowardPoint.Y, getStandingPosition() - new Vector2(32f, 0f), "", "", explode: true, damagesMonsters: false, base.currentLocation, this);
                    
                    basicProjectile.height.Value = 48f;
                    
                    base.currentLocation.projectiles.Add(basicProjectile);
                    
                    base.currentLocation.playSound("fireball");
                    
                    firingTimer = Game1.random.Next(2,6) * 1000f;

                }

            }

        }

    }

}

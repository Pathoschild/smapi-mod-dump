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
using StardewDruid.Map;
using StardewValley;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster
{

    public class RedDragon : StardewValley.Monsters.DinoMonster
    {
        public List<string> ouchList;

        public List<string> dialogueList;

        public Queue<Vector2> burningQueue;

        public int burnDamage;

        public bool firingAction;

        public bool defeated;

        public RedDragon(Vector2 vector, int combatModifier)
            : base(vector * 64)
        {
            Health = combatModifier * 12;

            MaxHealth = Health;

            focusedOnFarmers = true;

            DamageToFarmer = (int)(combatModifier * 0.15);

            objectsToDrop.Clear();

            burnDamage = (int)(combatModifier * 0.1);

            burningQueue = new();

            // -------------------------

            hideShadow.Value = true;

            ouchList = new()
            {
                "ARRRGGHHH",
                "I'll Answer That... With FIRE!",
                "insolence!",
                "creep"
            };

            dialogueList = new()
            {
                "creep",
                "burn",
                "ROOOOARRRR",
                "Kneel Before Tyrannatus!"
            };

            Sprite = MonsterData.MonsterSprite("RedDragon");

            Sprite.interval = 250f;

        }

        public void HardMode()
        {
            Health *= 3;

            Health /= 2;

            MaxHealth = Health;

            DamageToFarmer *= 3;

            DamageToFarmer /= 2;

            ouchList = new()
            {
                "Ah ha ha ha ha",
                "What pitiful strikes",
                "My helmet provides +3 Intelligence!",
                "creep"
            };

            dialogueList = new()
            {
                "CREEP",
                "I've strengthened since our last battle",
                "Your only option now... is subjugation",
                "Kneel Before Tyrannatus!"
            };

        }


        public override void update(GameTime time, GameLocation location)
        {

            if (Sprite.loadedTexture == null || Sprite.loadedTexture.Length == 0)
            {

                Sprite.spriteTexture = MonsterData.MonsterTexture("RedDragon");

                Sprite.loadedTexture = Sprite.textureName.Value;

            }

            base.update(time, location);

        }

        public override void reloadSprite()
        {
            Sprite = MonsterData.MonsterSprite("RedDragon");

            HideShadow = true;
        }

        public override void draw(SpriteBatch b)
        {
            if (!IsInvisible && Utility.isOnScreen(Position, 128))
            {
                b.Draw(
                    Sprite.Texture,
                    getLocalPosition(Game1.viewport),
                    Sprite.SourceRect,
                    Color.White,
                    rotation,
                    new Vector2(25.6f, 25.6f),
                    //new Vector2(0, 0),
                    5f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0.990f
                );

            }

        }

        public override void shedChunks(int number)
        {

        }

        protected override void sharedDeathAnimation()
        {

        }

        protected override void localDeathAnimation()
        {

        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 vector = Position;

            return new Rectangle((int)vector.X - 128, (int)vector.Y - 64, 320, 256);

        }


        public override List<Item> getExtraDropItems()
        {
            List<Item> list = new List<Item>();

            return list;

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            int num = Math.Max(1, damage - resilience.Value);

            Health -= num;

            if (Game1.random.Next(5) == 0)
            {
                setTrajectory(xTrajectory, yTrajectory);
            }

            if (Health <= 0)
            {
                deathAnimation();

                defeated = true;
            }

            int ouchIndex = Game1.random.Next(15);

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 3000);

                burningQueue.Enqueue(getTileLocation());

                DelayedAction.functionAfterDelay(burningDesert, 600);

            }

            return num;
        }

        public override void behaviorAtGameTick(GameTime time)
        {

            if (attackState.Value == 1)
            {
                IsWalkingTowardPlayer = false;
                Halt();
            }
            else if (withinPlayerThreshold())
            {
                IsWalkingTowardPlayer = true;

                firingAction = false;
            }
            else
            {
                IsWalkingTowardPlayer = false;
                nextChangeDirectionTime -= time.ElapsedGameTime.Milliseconds;
                nextWanderTime -= time.ElapsedGameTime.Milliseconds;
                if (nextChangeDirectionTime < 0)
                {
                    nextChangeDirectionTime = Game1.random.Next(500, 1000);
                    _ = FacingDirection;
                    facingDirection.Value = (facingDirection.Value + (Game1.random.Next(0, 3) - 1) + 4) % 4;
                }

                if (nextWanderTime < 0)
                {
                    if (wanderState)
                    {
                        nextWanderTime = Game1.random.Next(1000, 2000);
                    }
                    else
                    {
                        nextWanderTime = Game1.random.Next(1000, 3000);
                    }

                    wanderState = !wanderState;
                }

                if (wanderState)
                {
                    moveLeft = moveUp = moveRight = moveDown = false;
                    tryToMoveInDirection(facingDirection.Value, isFarmer: false, DamageToFarmer, isGlider);
                }
                firingAction = false;
            }

            timeUntilNextAttack -= time.ElapsedGameTime.Milliseconds;
            if (attackState.Value == 0 && withinPlayerThreshold(6))
            {
                firing.Set(newValue: false);
                if (timeUntilNextAttack < 0)
                {
                    timeUntilNextAttack = 0;
                    attackState.Set(1);
                    nextFireTime = 500;
                    totalFireTime = 3000;
                    currentLocation.playSound("croak");

                    int dialogueIndex = Game1.random.Next(6);
                    if (dialogueList.Count - 1 >= dialogueIndex)
                    {
                        showTextAboveHead(dialogueList[dialogueIndex], duration: 3000);
                    }

                }
            }
            else
            {
                if (totalFireTime <= 0)
                {
                    //bigHat();

                    return;
                }

                if (!firing)
                {
                    Farmer player = Player;
                    if (player != null)
                    {
                        faceGeneralDirection(player.Position);
                    }
                }

                totalFireTime -= time.ElapsedGameTime.Milliseconds;
                if (nextFireTime > 0)
                {
                    nextFireTime -= time.ElapsedGameTime.Milliseconds;
                    if (nextFireTime <= 0)
                    {
                        if (!firing.Value)
                        {
                            firing.Set(newValue: true);
                            currentLocation.playSound("furnace");
                        }

                        float num = 0f;
                        Vector2 startingPosition;
                        Vector2 startingPosition2;
                        Vector2 startingPosition3;

                        Vector2 burningVector = new(-1);
                        //Vector2 burningVector2 = new(-1);

                        Microsoft.Xna.Framework.Rectangle box = GetBoundingBox();

                        switch (facingDirection.Value)
                        {
                            case 0:
                                yVelocity = -1f;
                                startingPosition = new Vector2(box.Center.X, box.Top) - new Vector2(0, 64);
                                startingPosition2 = startingPosition - new Vector2(16, 0);
                                startingPosition3 = startingPosition + new Vector2(16, 0);
                                num = 90f;
                                burningVector = new Vector2(GetBoundingBox().Center.X, GetBoundingBox().Center.Y - 512);
                                //burningVector2 = new Vector2(GetBoundingBox().Center.X, GetBoundingBox().Center.Y - 360);
                                break;
                            case 1:
                                xVelocity = -1f;
                                startingPosition = new Vector2(box.Right, box.Top);
                                startingPosition2 = startingPosition + new Vector2(0, 16);
                                startingPosition3 = startingPosition + new Vector2(0, 32);
                                num = 0f;
                                burningVector = new Vector2(GetBoundingBox().Center.X + 512, GetBoundingBox().Center.Y);
                                //burningVector2 = new Vector2(GetBoundingBox().Center.X + 360, GetBoundingBox().Center.Y);
                                break;
                            case 3:
                                xVelocity = 1f;
                                startingPosition = new Vector2(box.Left, box.Top);
                                startingPosition2 = startingPosition + new Vector2(0, 16);
                                startingPosition3 = startingPosition + new Vector2(0, 32);
                                num = 180f;
                                burningVector = new Vector2(GetBoundingBox().Center.X - 512, GetBoundingBox().Center.Y);
                                //burningVector2 = new Vector2(GetBoundingBox().Center.X - 360, GetBoundingBox().Center.Y);
                                break;
                            default: // 2 / down
                                yVelocity = 1f;
                                startingPosition = new Vector2(box.Center.X, box.Top) + new Vector2(0, 32);
                                startingPosition2 = startingPosition - new Vector2(16, 0);
                                startingPosition3 = startingPosition + new Vector2(16, 0);
                                num = 270f;
                                burningVector = new Vector2(GetBoundingBox().Center.X, GetBoundingBox().Center.Y + 512f);
                                //burningVector2 = new Vector2(GetBoundingBox().Center.X, GetBoundingBox().Center.Y + 360f);
                                break;
                        }

                        num += (float)Math.Sin((double)(totalFireTime / 1000f * 180f) * Math.PI / 180.0) * 25f;
                        Vector2 vector = new Vector2((float)Math.Cos((double)num * Math.PI / 180.0), 0f - (float)Math.Sin((double)num * Math.PI / 180.0));
                        vector *= 10f;
                        BasicProjectile basicProjectile = new BasicProjectile(burnDamage, 10, 0, 1, MathF.PI / 16f, vector.X, vector.Y, startingPosition, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                        basicProjectile.ignoreTravelGracePeriod.Value = true;
                        basicProjectile.maxTravelDistance.Value = 512;
                        currentLocation.projectiles.Add(basicProjectile);

                        BasicProjectile basicProjectile2 = new BasicProjectile(burnDamage, 10, 0, 1, MathF.PI / 16f, vector.X, vector.Y, startingPosition2, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                        basicProjectile2.ignoreTravelGracePeriod.Value = true;
                        basicProjectile2.maxTravelDistance.Value = 512;
                        currentLocation.projectiles.Add(basicProjectile2);

                        BasicProjectile basicProjectile3 = new BasicProjectile(burnDamage, 10, 0, 1, MathF.PI / 16f, vector.X, vector.Y, startingPosition3, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                        basicProjectile3.ignoreTravelGracePeriod.Value = true;
                        basicProjectile3.maxTravelDistance.Value = 512;
                        currentLocation.projectiles.Add(basicProjectile3);

                        nextFireTime = 50;

                        if (burningQueue.Count == 0)
                        {
                            burningQueue.Enqueue(burningVector);
                            DelayedAction.functionAfterDelay(burningDesert, 600);
                        }

                        firingAction = true;

                    }
                }

                if (totalFireTime <= 0)
                {
                    totalFireTime = 0;
                    nextFireTime = 0;
                    attackState.Set(0);
                    timeUntilNextAttack = Game1.random.Next(1000, 2000);
                    burningQueue.Clear();

                    firingAction = false;
                }
            }

        }

        public void burningDesert()
        {

            if (burningQueue.Count > 0)
            {

                Vector2 explosionVector = burningQueue.Dequeue();

                currentLocation.explode(explosionVector / 64, 2, Game1.player, true, 20 + Game1.player.CombatLevel * 2);

            }

        }

    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Bosses
{
    public class Dragon : Serpent, IBossMonster
    {
        private enum State
        {
            Normal,
            ChargingUp,
            Charging,
            Attacking
        }

        private enum AttackType
        {
            FireBreath,
            Charge,
            RandomFire
        }

        public string DisplayName => "Cavrag the Dragon Prince";

        public string MapPath
        {
            get { return "boss-dragon"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Serpent"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(25, 25); }
        }

        public List<string> MusicTracks
        {
            get { return new() { "hold_your_ground" }; }
        }

        public bool InitializeWithHealthbar
        {
            get { return true; }
        }

        private float _difficulty;

        public float Difficulty
        {
            get { return _difficulty; }
            set { _difficulty = value; }
        }

        private int timeUntilNextAttack;

        private readonly NetBool firing = new();

        private readonly NetEnum<State> currentState = new(State.Normal);

        private readonly NetEnum<AttackType> currentAttack = new();

        private AttackType previousAttack;

        private int nextFireTime;

        private int totalFireTime;

        private int nextChargeTime;

        private int currentChargeDuration;

        public Dragon() { }

        public Dragon(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Sprite.SpriteWidth = 32;
            Sprite.SpriteHeight = 32;
            Sprite.LoadTexture(TextureName);
            Scale = 2f;

            timeUntilNextAttack = 6000;
            moveTowardPlayerThreshold.Value = 20;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(firing, currentState, currentAttack);
        }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (currentState.Value == State.Normal)
                base.MovePosition(time, viewport, currentLocation);
        }

        public override void collisionWithFarmerBehavior()
        {
            base.collisionWithFarmerBehavior();

            if (currentState.Value == State.Charging)
                currentState.Value = State.Normal;
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (float.IsNaN(Position.X) || float.IsNaN(Position.Y))
                Position = SpawnLocation * 64f;

            this.KeepInMap();
            base.update(time, location);

            if (currentState.Value == State.ChargingUp && !isGlowing)
                startGlowing(Color.Black, false, 0.15f);
            else if (currentState.Value != State.ChargingUp)
                stopGlowing();
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (currentState.Value == State.Normal)
            {
                base.behaviorAtGameTick(time);
                if (currentLocation is MineShaft mine)
                    DamageToFarmer = (int)(BossManager.GetBaseDamageToFarmer(mine, GetType()) * Difficulty);
            }

            if (currentState.Value == State.ChargingUp || currentState.Value == State.Attacking)
                facePlayer(Player);

            if (timeUntilNextAttack > 0 && currentState.Value == State.Normal)
                timeUntilNextAttack -= time.ElapsedGameTime.Milliseconds;

            if (timeUntilNextAttack <= 0 && currentState.Value == State.Normal)
            {
                timeUntilNextAttack = 0;
                previousAttack = currentAttack.Value;

                var validAttacks = new List<AttackType>((IEnumerable<AttackType>)Enum.GetValues(typeof(AttackType)));
                validAttacks.Remove(previousAttack);

                currentAttack.Value = validAttacks[Game1.random.Next(validAttacks.Count)];

                if (currentAttack.Value == AttackType.FireBreath || currentAttack.Value == AttackType.RandomFire)
                {
                    nextFireTime = 60;
                    totalFireTime = this.AdjustRangeForHealth(2000, 4000);
                    currentState.Value = State.Attacking;
                }
                else if (currentAttack.Value == AttackType.Charge)
                {
                    nextChargeTime = 4000 - this.AdjustRangeForHealth(0, 3000);
                    currentChargeDuration = 0;
                    currentState.Value = State.ChargingUp;

                    if (Roguelike.HardMode)
                        nextChargeTime -= 900;
                }

                timeUntilNextAttack = 4000 - this.AdjustRangeForHealth(0, 2500);
            }

            if (currentState.Value == State.Attacking)
            {
                if (totalFireTime > 0)
                {
                    if (!firing.Value)
                    {
                        if (Player is not null)
                            faceGeneralDirection(Player.Position, 0, false);
                    }

                    totalFireTime -= time.ElapsedGameTime.Milliseconds;
                    if (nextFireTime > 0 && Player is not null)
                    {
                        nextFireTime -= time.ElapsedGameTime.Milliseconds;
                        if (nextFireTime <= 0)
                        {
                            if (!firing.Value)
                            {
                                firing.Value = true;
                                currentLocation.playSound("furnace");
                            }

                            if (currentAttack.Value == AttackType.FireBreath)
                            {
                                Vector2 shot_origin = new(GetBoundingBox().Center.X, GetBoundingBox().Center.Y);

                                Vector2 playerAngle = (Player.GetBoundingBox().Center.ToVector2()) - (GetBoundingBox().Center.ToVector2());
                                playerAngle.Normalize();
                                faceGeneralDirection(Player.Position, 0, false);

                                float fire_angle = RoguelikeUtility.VectorToRadians(playerAngle);
                                fire_angle *= (float)(180 / Math.PI);
                                fire_angle += (float)Math.Sin(totalFireTime / 1000f * RoguelikeUtility.DegreesToRadians(200)) * 30f;
                                fire_angle = RoguelikeUtility.DegreesToRadians(fire_angle);

                                Vector2 shot_velocity = new((float)Math.Cos(fire_angle), (float)Math.Sin(fire_angle));
                                shot_velocity *= 11f;

                                BasicProjectile projectile = new(DamageToFarmer, 10, 0, 1, 0.196349546f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", false, false, currentLocation, this, false, null);
                                projectile.ignoreTravelGracePeriod.Value = true;
                                projectile.IgnoreLocationCollision = true;
                                projectile.ignoreMeleeAttacks.Value = true;
                                projectile.maxTravelDistance.Value = 800;
                                if (Roguelike.HardMode)
                                    projectile.maxTravelDistance.Value += 200;

                                currentLocation.projectiles.Add(projectile);

                                nextFireTime = 60;
                            }
                            else if (currentAttack.Value == AttackType.RandomFire)
                            {
                                Vector2 shot_origin = new(GetBoundingBox().Center.X, GetBoundingBox().Center.Y);
                                int degreesToPlayer = RoguelikeUtility.VectorToDegrees(Player.Position - Position);
                                Vector2 trajectory = RoguelikeUtility.VectorFromDegrees(degreesToPlayer + Game1.random.Next(-65, 66));
                                if (Roguelike.HardMode)
                                    trajectory *= 12f;
                                else
                                    trajectory *= 10f;
                                BasicProjectile projectile = new(DamageToFarmer, 10, 0, 1, 0.196349546f, trajectory.X, trajectory.Y, shot_origin, "", "fireball", false, false, currentLocation, this, false, null);
                                projectile.ignoreMeleeAttacks.Value = true;
                                projectile.ignoreTravelGracePeriod.Value = true;
                                projectile.IgnoreLocationCollision = true;
                                currentLocation.projectiles.Add(projectile);

                                nextFireTime = 100;
                                if (Roguelike.HardMode)
                                    nextFireTime -= 30;
                            }
                        }
                    }

                    if (totalFireTime <= 0)
                    {
                        totalFireTime = 0;
                        nextFireTime = 0;
                        firing.Value = false;
                        currentState.Value = State.Normal;
                    }
                }
            }
            else if (currentState.Value == State.ChargingUp)
            {
                nextChargeTime -= time.ElapsedGameTime.Milliseconds;

                if (nextChargeTime <= 0)
                {
                    nextChargeTime = 0;
                    currentState.Value = State.Charging;
                }
            }
            else if (currentState == State.Charging)
            {
                if (DamageToFarmer == (int)(BossManager.GetBaseDamageToFarmer((MineShaft)currentLocation, GetType()) * Difficulty))
                    DamageToFarmer = (int)Math.Round(DamageToFarmer * 1.5f);

                Vector2 chargeVector = Player.GetBoundingBox().Center.ToVector2() - GetBoundingBox().Center.ToVector2();
                chargeVector.Normalize();
                chargeVector *= this.AdjustRangeForHealth(15f, 21f);
                Position += chargeVector;
                rotation = RoguelikeUtility.VectorToRadians(chargeVector) + RoguelikeUtility.DegreesToRadians(90);

                currentChargeDuration += time.ElapsedGameTime.Milliseconds;
                if (currentChargeDuration >= 5000)
                {
                    currentState.Value = State.Normal;
                    timeUntilNextAttack = 4000 - this.AdjustRangeForHealth(0, 2000);
                }
            }
        }

        public override void reloadSprite()
        {
            Sprite = new(TextureName)
            {
                SpriteWidth = 32,
                SpriteHeight = 32
            };
            Sprite.LoadTexture(TextureName);
            HideShadow = true;
        }

        public override Rectangle GetBoundingBox()
        {
            return new((int)(Position.X - 8 * Scale), (int)Position.Y, (int)(Sprite.SpriteWidth * 4 * 3 / 4 * Scale), (int)(96 * Scale));
        }

        public override void drawAboveAllLayers(SpriteBatch b)
        {
            b.Draw(Game1.shadowTexture, getLocalPosition(Game1.viewport) + new Vector2(64f, GetBoundingBox().Height), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (getStandingY() - 1) / 10000f);
            b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + new Vector2(Sprite.SpriteWidth * 2, (GetBoundingBox().Height / 2)), new Rectangle?(Sprite.SourceRect), Color.White, rotation, new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight / 2), scale * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((getStandingY() + 8) / 10000f)));
            if (isGlowing)
                b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + new Vector2(Sprite.SpriteWidth * 2, (GetBoundingBox().Height / 2)), new Rectangle?(Sprite.SourceRect), glowingColor * glowingTransparency, rotation, new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight / 2), scale * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((getStandingY() + 8) / 10000f + 0.0001f)));
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            if (currentState.Value == State.Charging)
                currentState.Value = State.Normal;
            else if (currentState.Value == State.Attacking || currentState.Value == State.ChargingUp)
            {
                currentLocation.playSound("crafting");
                return 0;
            }

            int result = base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
            if (Health <= 0)
                BossManager.Death(who.currentLocation, who, DisplayName, new(23, 4));

            return result;
        }
    }
}

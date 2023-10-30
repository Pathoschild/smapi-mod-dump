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

        public string MapPath => "boss-dragon";

        public string TextureName => "Characters\\Monsters\\Serpent";

        public Vector2 SpawnLocation => new(25, 25);

        public List<string> MusicTracks => new() { "hold_your_ground" };

        public bool InitializeWithHealthbar => true;

        public float Difficulty { get; set; }

        private int TimeUntilNextAttack;

        private readonly NetBool Firing = new();

        private readonly NetEnum<State> CurrentState = new(State.Normal);

        private readonly NetEnum<AttackType> CurrentAttack = new();

        private AttackType PreviousAttack;

        private int NextFireTime;

        private int TotalFireTime;

        private int NextChargeTime;

        private int CurrentChargeDuration;

        public Dragon() { }

        public Dragon(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Sprite.SpriteWidth = 32;
            Sprite.SpriteHeight = 32;
            Sprite.LoadTexture(TextureName);
            Scale = 2f;

            TimeUntilNextAttack = 6000;
            moveTowardPlayerThreshold.Value = 20;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(Firing, CurrentState, CurrentAttack);
        }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (CurrentState.Value == State.Normal)
                base.MovePosition(time, viewport, currentLocation);
        }

        public override void collisionWithFarmerBehavior()
        {
            base.collisionWithFarmerBehavior();

            if (CurrentState.Value == State.Charging)
                CurrentState.Value = State.Normal;
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (float.IsNaN(Position.X) || float.IsNaN(Position.Y))
                Position = SpawnLocation * 64f;

            this.KeepInMap();
            base.update(time, location);

            if (CurrentState.Value == State.ChargingUp && !isGlowing)
                startGlowing(Color.Black, false, 0.15f);
            else if (CurrentState.Value != State.ChargingUp)
                stopGlowing();
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (CurrentState.Value == State.Normal)
            {
                base.behaviorAtGameTick(time);
                if (currentLocation is MineShaft mine)
                    DamageToFarmer = (int)(BossManager.GetBaseDamageToFarmer(mine, GetType()) * Difficulty);
            }

            if (CurrentState.Value == State.ChargingUp || CurrentState.Value == State.Attacking)
                facePlayer(Player);

            if (TimeUntilNextAttack > 0 && CurrentState.Value == State.Normal)
                TimeUntilNextAttack -= time.ElapsedGameTime.Milliseconds;

            if (TimeUntilNextAttack <= 0 && CurrentState.Value == State.Normal)
            {
                TimeUntilNextAttack = 0;
                PreviousAttack = CurrentAttack.Value;

                var validAttacks = new List<AttackType>((IEnumerable<AttackType>)Enum.GetValues(typeof(AttackType)));
                validAttacks.Remove(PreviousAttack);

                CurrentAttack.Value = validAttacks[Game1.random.Next(validAttacks.Count)];

                if (CurrentAttack.Value == AttackType.FireBreath || CurrentAttack.Value == AttackType.RandomFire)
                {
                    NextFireTime = 60;
                    TotalFireTime = this.AdjustRangeForHealth(2000, 4000);
                    CurrentState.Value = State.Attacking;
                }
                else if (CurrentAttack.Value == AttackType.Charge)
                {
                    NextChargeTime = 4000 - this.AdjustRangeForHealth(0, 3000);
                    CurrentChargeDuration = 0;
                    CurrentState.Value = State.ChargingUp;

                    if (Roguelike.HardMode)
                        NextChargeTime -= 900;
                }

                TimeUntilNextAttack = 4000 - this.AdjustRangeForHealth(0, 2500);
            }

            if (CurrentState.Value == State.Attacking)
            {
                if (TotalFireTime > 0)
                {
                    if (!Firing.Value)
                    {
                        if (Player is not null)
                            faceGeneralDirection(Player.Position, 0, false);
                    }

                    TotalFireTime -= time.ElapsedGameTime.Milliseconds;
                    if (NextFireTime > 0 && Player is not null)
                    {
                        NextFireTime -= time.ElapsedGameTime.Milliseconds;
                        if (NextFireTime <= 0)
                        {
                            if (!Firing.Value)
                            {
                                Firing.Value = true;
                                currentLocation.playSound("furnace");
                            }

                            if (CurrentAttack.Value == AttackType.FireBreath)
                            {
                                Vector2 shot_origin = new(GetBoundingBox().Center.X, GetBoundingBox().Center.Y);

                                Vector2 playerAngle = (Player.GetBoundingBox().Center.ToVector2()) - (GetBoundingBox().Center.ToVector2());
                                playerAngle.Normalize();
                                faceGeneralDirection(Player.Position, 0, false);

                                float fire_angle = RoguelikeUtility.VectorToRadians(playerAngle);
                                fire_angle *= (float)(180 / Math.PI);
                                fire_angle += (float)Math.Sin(TotalFireTime / 1000f * RoguelikeUtility.DegreesToRadians(200)) * 30f;
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

                                NextFireTime = 60;
                            }
                            else if (CurrentAttack.Value == AttackType.RandomFire)
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

                                NextFireTime = 100;
                                if (Roguelike.HardMode)
                                    NextFireTime -= 30;
                            }
                        }
                    }

                    if (TotalFireTime <= 0)
                    {
                        TotalFireTime = 0;
                        NextFireTime = 0;
                        Firing.Value = false;
                        CurrentState.Value = State.Normal;
                    }
                }
            }
            else if (CurrentState.Value == State.ChargingUp)
            {
                NextChargeTime -= time.ElapsedGameTime.Milliseconds;

                if (NextChargeTime <= 0)
                {
                    NextChargeTime = 0;
                    CurrentState.Value = State.Charging;
                }
            }
            else if (CurrentState == State.Charging)
            {
                if (DamageToFarmer == (int)(BossManager.GetBaseDamageToFarmer((MineShaft)currentLocation, GetType()) * Difficulty))
                    DamageToFarmer = (int)Math.Round(DamageToFarmer * 1.5f);

                Vector2 chargeVector = Player.GetBoundingBox().Center.ToVector2() - GetBoundingBox().Center.ToVector2();
                chargeVector.Normalize();
                chargeVector *= this.AdjustRangeForHealth(15f, 21f);
                Position += chargeVector;
                rotation = RoguelikeUtility.VectorToRadians(chargeVector) + RoguelikeUtility.DegreesToRadians(90);

                CurrentChargeDuration += time.ElapsedGameTime.Milliseconds;
                if (CurrentChargeDuration >= 5000)
                {
                    CurrentState.Value = State.Normal;
                    TimeUntilNextAttack = 4000 - this.AdjustRangeForHealth(0, 2000);
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
            if (CurrentState.Value == State.Charging)
                CurrentState.Value = State.Normal;
            else if (CurrentState.Value == State.Attacking || CurrentState.Value == State.ChargingUp)
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

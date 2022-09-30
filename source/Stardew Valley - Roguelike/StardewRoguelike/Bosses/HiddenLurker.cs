/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.Extensions;
using StardewRoguelike.Projectiles;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Bosses
{
    internal class HiddenLurker : LavaLurk, IBossMonster
    {
        public string DisplayName => "Chatto the Eternal Lurker";

        public string MapPath => "custom-lavalurk";

        public string TextureName => "Characters/Monsters/Lava Lurk";

        public Vector2 SpawnLocation => new(22, 13);

        public List<string> MusicTracks => new() { "VolcanoMines1", "VolcanoMines2" };

        public bool InitializeWithHealthbar => true;

        private float _difficulty;

        public float Difficulty
        {
            get { return _difficulty; }
            set { _difficulty = value; }
        }

        private enum AttackType
        {
            Barrage,
            FireWall,
            ExplodingRock,
            BigFireball
        }

        private List<AttackType> AttackHistory = new();

        private AttackType currentAttack;

        private int fireWallAngle = 0;

        private float fireballTimer;

        private bool waitingToDive = false;

        // max health percent, shot angle
        private readonly List<(float, int)> whenToFireWall = new()
        {
            (0.145f * 6, 135),
            (0.145f * 4, 45),
            (0.145f * 2, 90)
        };

        public HiddenLurker() : base() { }

        public HiddenLurker(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Scale = 2f;

            DamageToFarmer = (int)Math.Round(17 * Difficulty);
            int hits = Roguelike.HardMode ? 10 : 7;
            MaxHealth = DamageToFarmer * hits;
            Health = MaxHealth;

            moveTowardPlayerThreshold.Value = 12;
            Speed = 0;

            resilience.Value = 0;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
        }

        public override bool TargetInRange()
        {
            if (targettedFarmer == null)
                return false;

            if (Math.Abs(targettedFarmer.Position.X - Position.X) <= (12 * 64f) && Math.Abs(targettedFarmer.Position.Y - Position.Y) <= (12 * 64f))
                return true;

            return false;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (targettedFarmer is null || targettedFarmer.currentLocation != currentLocation)
            {
                targettedFarmer = null;
                targettedFarmer = findPlayer();
            }

            if (stateTimer > 0f)
            {
                stateTimer -= (float)time.ElapsedGameTime.TotalSeconds;
                if (stateTimer <= 0f)
                    stateTimer = 0f;
            }

            if (currentState.Value == State.Submerged)
            {
                swimSpeed = this.AdjustRangeForHealth(2, 6);
                if (stateTimer == 0f)
                {
                    currentState.Value = State.Lurking;
                    stateTimer = 0.1f;
                }
            }
            else if (currentState.Value == State.Lurking)
            {
                swimSpeed = this.AdjustRangeForHealth(1, 4);
                if (stateTimer == 0f)
                {
                    currentState.Value = State.Emerged;
                    stateTimer = 0.75f;
                    swimSpeed = 0;
                }
            }
            else if (currentState.Value == State.Emerged)
            {
                if (stateTimer == 0f && !waitingToDive && TargetInRange())
                {
                    AttackHistory.Add(currentAttack);

                    if (AttackHistory.Count >= 2)
                        AttackHistory.RemoveAt(0);

                    var validAttacks = new List<AttackType>((IEnumerable<AttackType>)Enum.GetValues(typeof(AttackType)));
                    validAttacks.Remove(AttackType.FireWall);
                    validAttacks.RemoveAll(attackType => AttackHistory.Contains(attackType));

                    currentState.Value = State.Firing;
                    stateTimer = 1.5f;

                    currentAttack = validAttacks[Game1.random.Next(validAttacks.Count)];

                    if (whenToFireWall.Count > 0 && Health <= (int)Math.Round(MaxHealth * whenToFireWall[0].Item1))
                    {
                        fireWallAngle = whenToFireWall[0].Item2;
                        whenToFireWall.RemoveAt(0);
                        currentAttack = AttackType.FireWall;
                    }

                    if (currentAttack == AttackType.ExplodingRock || currentAttack == AttackType.Barrage)
                        stateTimer += this.AdjustRangeForHealth(0f, 2f);
                    else if (currentAttack == AttackType.BigFireball)
                        stateTimer = 5f;

                    fireTimer = 0.25f;
                }
                else if (stateTimer == 0f && waitingToDive)
                {
                    currentState.Value = State.Diving;
                    stateTimer = 0.5f;
                    waitingToDive = false;
                }
            }
            else if (currentState.Value == State.Firing)
            {
                if (stateTimer == 0f && fireballTimer == 0f)
                {
                    currentState.Value = State.Emerged;
                    stateTimer = 2.5f;
                    waitingToDive = true;

                    if (Roguelike.HardMode)
                    {
                        stateTimer = 1f;
                        waitingToDive = false;
                    }
                }

                if (fireballTimer > 0f)
                {
                    fireballTimer -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (fireballTimer <= 0f)
                    {
                        Vector2 shot_origin = Position + new Vector2(0f, -32f);
                        Vector2 shot_velocity = targettedFarmer.Position - shot_origin;
                        shot_velocity.Normalize();
                        shot_velocity *= 7f + (Roguelike.HardMode ? 2f : 0);
                        currentLocation.playSound("fireball");

                        ReturningProjectile returningShot = new(10f, DamageToFarmer, 14, 0, 3, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                        returningShot.ignoreTravelGracePeriod.Value = true;

                        currentLocation.projectiles.Add(returningShot);

                        fireballTimer = 0f;
                    }
                }

                if (fireTimer > 0f)
                {
                    fireTimer -= (float)time.ElapsedGameTime.TotalSeconds;
                    if (fireTimer <= 0f)
                    {
                        if (currentAttack == AttackType.Barrage)
                        {
                            fireTimer = 0.25f;
                            if (targettedFarmer != null)
                            {
                                Vector2 shot_origin = Position + new Vector2(0f, -32f);
                                Vector2 shot_velocity = targettedFarmer.Position - shot_origin;
                                shot_velocity.Normalize();
                                shot_velocity *= 7f + (Roguelike.HardMode ? 2f : 0);
                                currentLocation.playSound("fireball");

                                BasicProjectile projectile = new(DamageToFarmer, 10, 0, 3, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                                projectile.ignoreMeleeAttacks.Value = true;
                                projectile.ignoreTravelGracePeriod.Value = true;
                                currentLocation.projectiles.Add(projectile);

                                if (stateTimer <= 0.25f)
                                {
                                    fireballTimer = 0.25f;
                                    fireTimer = 0f;
                                }
                            }
                        }
                        else if (currentAttack == AttackType.FireWall)
                        {
                            if (targettedFarmer != null)
                            {
                                Vector2 shot_origin = Position + new Vector2(0f, -32f);

                                Vector2 shot_velocity;
                                shot_velocity = BossManager.VectorFromDegree(fireWallAngle);
                                shot_velocity *= 7f;

                                currentLocation.playSound("furnace");

                                for (int i = 64; i <= 64 * 18; i += 32)
                                {
                                    FireWallProjectile projectile = new(i, 120f, DamageToFarmer * 2, 10, 0, 0, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                                    projectile.ignoreMeleeAttacks.Value = true;
                                    projectile.ignoreTravelGracePeriod.Value = true;
                                    projectile.startingScale.Value = Roguelike.HardMode ? 3f : 1f;
                                    currentLocation.projectiles.Add(projectile);
                                }
                            }

                            if (Health < (int)(MaxHealth * 0.33f) && stateTimer > 0f)
                            {
                                fireTimer = 1f;
                                stateTimer = 0f;
                            }
                        }
                        else if (currentAttack == AttackType.ExplodingRock)
                        {
                            if (targettedFarmer != null)
                            {
                                int spreadX = Game1.random.Next(-5, 6);
                                int spreadY = Game1.random.Next(-5, 6);

                                Vector2 targetTile = new(
                                    targettedFarmer.getTileLocation().X + spreadX,
                                    targettedFarmer.getTileLocation().Y + spreadY
                                );

                                Vector2 shot_origin = Position + new Vector2(0f, -32f);
                                float speedMultiplier = Roguelike.HardMode ? 10f : 8f;
                                ExplodingRockProjectile projectile = new(targetTile, speedMultiplier, DamageToFarmer, shot_origin, "fireball", currentLocation, this);

                                projectile.maxTravelDistance.Value = -1;
                                projectile.ignoreMeleeAttacks.Value = true;
                                projectile.ignoreTravelGracePeriod.Value = true;

                                currentLocation.projectiles.Add(projectile);

                                fireTimer = 0.175f;

                                if (stateTimer <= 0.25f)
                                {
                                    fireballTimer = 0.25f;
                                    fireTimer = 0f;
                                }
                            }
                        }
                        else if (currentAttack == AttackType.BigFireball)
                        {
                            if (targettedFarmer != null)
                            {
                                Vector2 shot_origin = Position + new Vector2(0f, -32f);
                                Vector2 shot_velocity = targettedFarmer.Position - shot_origin;
                                shot_velocity.Normalize();
                                shot_velocity *= 6f + this.AdjustRangeForHealth(0f, 10f) + (Roguelike.HardMode ? 2f : 0);
                                currentLocation.playSound("fireball");

                                BasicProjectile projectile = new(DamageToFarmer, 10, 0, 0, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                                projectile.ignoreMeleeAttacks.Value = true;
                                projectile.ignoreTravelGracePeriod.Value = true;
                                projectile.startingScale.Value = 2.5f;
                                currentLocation.projectiles.Add(projectile);

                                fireTimer = Roguelike.HardMode ? 0.5f : 1f;

                                if (stateTimer <= 1.1f)
                                {
                                    fireballTimer = 1f;
                                    fireTimer = 0f;
                                }
                            }
                        }
                    }
                }
            }
            else if (currentState.Value == State.Diving && stateTimer == 0f)
            {
                currentState.Value = State.Submerged;
                stateTimer = 0.75f;

                targettedFarmer = findPlayer();
            }
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            int result = base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
            if (Health <= 0)
                BossManager.Death(currentLocation, who, DisplayName);

            return result;
        }
    }
}

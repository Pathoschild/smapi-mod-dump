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
using Netcode;
using StardewRoguelike.Extensions;
using StardewRoguelike.Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Bosses
{
    public class Modulosaurus : DinoMonster, IBossMonster
    {
        public string DisplayName => "Nadith the Extinct";

        public string MapPath => "boss-modulosaurus";

        public string TextureName => "Characters\\Monsters\\Pepper Rex";

        public Vector2 SpawnLocation => new(25, 30);

        public List<string> MusicTracks => new() { "invoke_the_ancient" };

        public bool InitializeWithHealthbar => true;

        public float Difficulty { get; set; }

        private readonly NetEvent2Field<Vector2, NetVector2, int, NetInt> MeteorStrikeWarningEvent = new();

        private int TicksToAttack = 300;

        private readonly List<int> AttackHistory = new();

        private int TicksOfTotalFireBreath = 0;
        private int TicksUntilNextFireBreath = 0;

        private int TicksUntilMeteorStrike = 0;
        private int MeteorStrikeTicksLeft = 0;

        private int TicksUntilShotgunFireballs = 0;

        private int TicksUntilLavaLurk = 0;

        private int TicksUntilInterweavingFireballs = 0;
        private int TicksOfTotalInterweavingFireballs = 0;
        private int TicksUntilNextInterweavingFireballs = 0;
        private bool InterweavingIsOffset = false;

        private Rectangle LavaLurkBox1 = new(11, 22, 6, 6);
        private Rectangle LavaLurkBox2 = new(33, 21, 6, 6);

        public Modulosaurus() { }

        public Modulosaurus(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Sprite.LoadTexture(TextureName);
            Sprite.SpriteHeight = 32;
            Sprite.SpriteWidth = 32;
            Sprite.UpdateSourceRect();
            HideShadow = true;
            Scale = 3f;

            Speed += 1;

            resilience.Value = 0;

            moveTowardPlayerThreshold.Value = 20;
        }

        public override void reloadSprite()
        {
            Sprite = new(TextureName);
            Sprite.LoadTexture(TextureName);
            Sprite.SpriteHeight = 32;
            Sprite.SpriteWidth = 32;
            Sprite.UpdateSourceRect();
            HideShadow = true;
        }

        protected override void initNetFields()
        {
            NetFields.AddFields(MeteorStrikeWarningEvent);
            MeteorStrikeWarningEvent.onEvent += MeteorStrikeWarning;
            base.initNetFields();
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);
            MeteorStrikeWarningEvent.Poll();
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            DoMovementTick(time);

            if (Health <= 0)
                return;

            if (MeteorStrikeTicksLeft > 0)
                MeteorStrikeTicksLeft--;

            if (TicksOfTotalFireBreath > 0)
            {
                TicksOfTotalFireBreath--;

                if (TicksOfTotalFireBreath == 0)
                {
                    attackState.Value = 0;
                    firing.Value = false;

                    TicksToAttack = 90 - this.AdjustRangeForHealth(0, 60);
                    return;
                }

                if (TicksUntilNextFireBreath > 0)
                {
                    TicksUntilNextFireBreath--;
                    return;
                }

                ShootFireBreath();
                TicksUntilNextFireBreath = 1;
            }

            if (TicksUntilLavaLurk > 0)
            {
                TicksUntilLavaLurk--;

                if (TicksUntilLavaLurk == 0)
                {
                    DoLavaLurkAttack();
                    if (Roguelike.HardMode)
                        DoLavaLurkAttack();

                    TicksToAttack = 90 - this.AdjustRangeForHealth(0, 60);
                }
            }

            if (TicksUntilShotgunFireballs > 0)
            {
                TicksUntilShotgunFireballs--;

                if (TicksUntilShotgunFireballs == 0)
                {
                    ShootShotgunFireballs();

                    attackState.Value = 0;
                    firing.Value = false;

                    TicksToAttack = 90 - this.AdjustRangeForHealth(0, 60);
                }
            }

            if (TicksUntilInterweavingFireballs > 0)
            {
                TicksUntilInterweavingFireballs--;

                if (TicksUntilInterweavingFireballs == 0)
                {
                    TicksOfTotalInterweavingFireballs = 60 * this.AdjustRangeForHealth(5, 8);
                    TicksUntilNextInterweavingFireballs = 30;
                }
            }

            if (TicksOfTotalInterweavingFireballs > 0)
            {
                TicksOfTotalInterweavingFireballs--;

                if (TicksOfTotalInterweavingFireballs == 0)
                {
                    attackState.Value = 0;
                    firing.Value = false;

                    TicksToAttack = 120 - this.AdjustRangeForHealth(0, 60);
                    return;
                }

                if (TicksUntilNextInterweavingFireballs > 0)
                {
                    TicksUntilNextInterweavingFireballs--;
                    return;
                }

                ShootInterweavingFireballs(InterweavingIsOffset);
                InterweavingIsOffset = !InterweavingIsOffset;
                TicksUntilNextInterweavingFireballs = 45 - this.AdjustRangeForHealth(0, 20);
            }

            if (TicksUntilMeteorStrike > 0)
            {
                TicksUntilMeteorStrike--;

                if (TicksUntilMeteorStrike == 0)
                {
                    attackState.Value = 0;

                    DoMeteorStrike();

                    double rng = Game1.random.NextDouble();
                    if (rng < ((float)1 / 3))
                        TicksOfTotalFireBreath = 120;
                    else if (rng < ((float)2 / 3))
                        TicksUntilShotgunFireballs = 120;
                    else
                        TicksUntilLavaLurk = 120;
                }
            }

            if (TicksToAttack > 0)
            {
                TicksToAttack--;
                return;
            }
            else if (IsAttacking())
                return;

            int attack = 0;
            while (attack == -1 || AttackHistory.Contains(attack))
            {
                if (MeteorStrikeTicksLeft > 0)
                    attack = Game1.random.Next(2, 5);
                else
                    attack = Game1.random.Next(0, 5);
            }

            if (attack == 0)
            {
                attackState.Value = 1;
                firing.Value = true;
                currentLocation.playSound("croak");

                TicksUntilInterweavingFireballs = 45;
            }
            else if (attack == 1)
            {
                attackState.Value = 1;
                currentLocation.playSound("croak");

                TicksUntilMeteorStrike = 120;
            }
            else if (attack == 2)
            {
                firing.Value = false;
                attackState.Value = 1;
                currentLocation.playSound("croak");

                TicksUntilNextFireBreath = 30;
                TicksOfTotalFireBreath = 3 * 60;

                if (Player is not null)
                    faceGeneralDirection(Player.Position);
            }
            else if (attack == 3)
            {
                attackState.Value = 1;
                firing.Value = true;
                currentLocation.playSound("croak");

                TicksUntilShotgunFireballs = 45;
            }
            else if (attack == 4)
            {
                currentLocation.playSound("croak");

                DoLavaLurkAttack();
                if (Roguelike.HardMode)
                    DoLavaLurkAttack();

                TicksToAttack = 3 * 60;
            }

            AttackHistory.Add(attack);
            if (AttackHistory.Count >= 3)
                AttackHistory.RemoveAt(0);
        }

        public bool IsAttacking()
        {
            return attackState.Value == 1;
        }

        private void MeteorStrikeWarning(Vector2 position, int delay)
        {
            Rectangle spriteSourceRect = new(48, 0, 16, 16);
            float markerScale = 4f;
            Vector2 drawPosition = new(position.X * 64, position.Y * 64 - 16);

            Game1.currentLocation.TemporarySprites.Add(new("TileSheets\\Projectiles", spriteSourceRect, 9999f, 1, 999, drawPosition, false, Game1.random.NextDouble() < 0.5, (position.Y + 32f) / 10000f + 0.001f, 0.0075f, Color.White, markerScale, 0f, 0f, 0f, false)
            {
                delayBeforeAnimationStart = delay,
                light = true,
                lightRadius = 1f,
                lightcolor = Color.White,
                endFunction = (_) =>
                {
                    currentLocation.playSound("explosion");
                    currentLocation.explode(position, 1, Player, true, 30);
                }
            });
        }

        public static Vector2 GetRandomTileInRect(Rectangle rect)
        {
            int x = Game1.random.Next(rect.X, rect.X + rect.Width + 1);
            int y = Game1.random.Next(rect.Y, rect.Y + rect.Height + 1);

            return new(x, y);
        }

        public void DoLavaLurkAttack()
        {
            Vector2 tileLoc1 = GetRandomTileInRect(LavaLurkBox1);
            Vector2 tileLoc2 = GetRandomTileInRect(LavaLurkBox2);
            Vector2[] locs = new Vector2[] { tileLoc1, tileLoc2 };

            for (int i = 0; i < 2; i++)
            {
                ModulosaurusMinion lurker = new(locs[i]);
                currentLocation.characters.Add(lurker);
            }
        }

        public void DoMeteorStrike()
        {
            if (Player is null)
                return;

            FacingDirection = 2;

            List<Vector2> randomTiles = new();
            while (randomTiles.Count < 100)
            {
                Vector2 randomTile = currentLocation.getRandomTile();
                if (currentLocation.isTileLocationOpen(new xTile.Dimensions.Location((int)randomTile.X, (int)randomTile.Y)))
                    randomTiles.Add(randomTile);
            }

            int maxDelay = 0;
            foreach (Vector2 randomTile in randomTiles)
            {
                int delay = Game1.random.Next(500, 3100);
                if (Roguelike.HardMode)
                    delay -= 200;

                if (delay > maxDelay)
                    maxDelay = delay;

                MeteorStrikeWarningEvent.Fire(randomTile, delay);
            }

            MeteorStrikeTicksLeft = maxDelay;
        }

        public void ShootInterweavingFireballs(bool offset)
        {
            FacingDirection = 2;

            currentLocation.playSound("furnace");

            Vector2 shot_origin = new(GetBoundingBox().Center.X - 32f, GetBoundingBox().Center.Y - 32f);

            for (int angle = 0; angle <= 360; angle += 30)
            {
                int fireAngle = angle + (offset ? 15 : 0) % 360;
                Vector2 shot_velocity = new((float)Math.Cos(RoguelikeUtility.DegreesToRadians(fireAngle)), -(float)Math.Sin(RoguelikeUtility.DegreesToRadians(fireAngle)));
                shot_velocity *= 10f;
                BasicProjectile projectile = new((int)Math.Round(35 * Difficulty), 10, 0, 1, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", false, false, currentLocation, this, false, null);
                projectile.ignoreTravelGracePeriod.Value = true;
                projectile.IgnoreLocationCollision = true;
                projectile.ignoreMeleeAttacks.Value = true;
                projectile.startingScale.Value = 2.5f;
                currentLocation.projectiles.Add(projectile);
            }
        }

        public void ShootShotgunFireballs()
        {
            if (Player is null)
                return;

            faceGeneralDirection(Player.Position, 0, false);
            currentLocation.playSound("furnace");

            Vector2 shot_origin = new(GetBoundingBox().Center.X - 32f, GetBoundingBox().Center.Y - 32f);
            float fire_angle = RoguelikeUtility.VectorToDegrees(Player.Position - Position);

            for (int angleOffset = -20; angleOffset <= 20; angleOffset += 20)
            {
                Vector2 shot_velocity = new((float)Math.Cos(RoguelikeUtility.DegreesToRadians(fire_angle + angleOffset)), (float)Math.Sin(RoguelikeUtility.DegreesToRadians(fire_angle + angleOffset)));
                shot_velocity *= 10f;

                BasicProjectile projectile = new((int)Math.Round(35 * Difficulty), 10, 0, 1, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", false, false, currentLocation, this, false, null);
                projectile.ignoreTravelGracePeriod.Value = true;
                projectile.ignoreMeleeAttacks.Value = true;
                projectile.IgnoreLocationCollision = true;
                projectile.startingScale.Value = 3f;
                projectile.maxTravelDistance.Value = 470;
                currentLocation.projectiles.Add(projectile);
            }
        }

        public void ShootFireBreath()
        {
            if (!firing.Value)
            {
                currentLocation.playSound("furnace");
                firing.Value = true;
            }

            Vector2 shot_origin = new(GetBoundingBox().Center.X - 32f, GetBoundingBox().Center.Y - 32f);

            float fire_angle = RoguelikeUtility.VectorToDegrees(Player.Position - Position);
            fire_angle += (float)Math.Sin(RoguelikeUtility.DegreesToRadians(TicksOfTotalFireBreath * 60 / 1000f * 180f)) * 25f;

            Vector2 shot_velocity = new((float)Math.Cos(RoguelikeUtility.DegreesToRadians(fire_angle)), (float)Math.Sin(RoguelikeUtility.DegreesToRadians(fire_angle)));
            shot_velocity *= 10f;

            BasicProjectile projectile = new(25, 10, 0, 1, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", explode: false, damagesMonsters: false, currentLocation, this);
            projectile.ignoreTravelGracePeriod.Value = true;
            projectile.ignoreMeleeAttacks.Value = true;
            projectile.IgnoreLocationCollision = true;
            projectile.maxTravelDistance.Value = Roguelike.HardMode ? 800 : 512;
            currentLocation.projectiles.Add(projectile);
        }

        public void DoMovementTick(GameTime time)
        {
            if (IsAttacking())
            {
                IsWalkingTowardPlayer = false;
                Halt();
            }
            else if (withinPlayerThreshold())
                IsWalkingTowardPlayer = true;
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
                        nextWanderTime = Game1.random.Next(1000, 2000);
                    else
                        nextWanderTime = Game1.random.Next(1000, 3000);

                    wanderState = !wanderState;
                }
                if (wanderState)
                {
                    moveLeft = moveUp = moveRight = moveDown = false;
                    tryToMoveInDirection(facingDirection.Value, isFarmer: false, DamageToFarmer, isGlider);
                }
            }
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth;
            int boxHeight;
            if (FacingDirection == 0 || FacingDirection == 2)
            {
                // up and down
                boxWidth = (int)(Sprite.SpriteWidth * 3 * Scale);
                boxHeight = (int)(Sprite.SpriteHeight * Scale * 2);

                return new Rectangle((int)Position.X - boxWidth / 3, (int)Position.Y - boxHeight / 8, boxWidth, boxHeight);
            }
            else
            {
                // side to side
                boxWidth = (int)(Sprite.SpriteWidth * 4 * Scale);
                boxHeight = (int)(Sprite.SpriteHeight * Scale * 1.5);

                return new Rectangle((int)Position.X - boxWidth / 3, (int)Position.Y + boxHeight / 4, boxWidth, boxHeight);
            }
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            if (isBomb)
                return 0;

            int result = base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
            if (Health <= 0)
                BossManager.Death(currentLocation, who, DisplayName, SpawnLocation);

            return result;
        }
    }
}

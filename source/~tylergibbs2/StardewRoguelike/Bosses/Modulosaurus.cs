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

        public string MapPath
        {
            get { return "boss-modulosaurus"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Pepper Rex"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(25, 30); }
        }

        public List<string> MusicTracks
        {
            get { return new() { "VolcanoMines1", "VolcanoMines2" }; }
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

        private readonly NetEvent2Field<Vector2, NetVector2, int, NetInt> meteorStrikeWarningEvent = new();

        private int ticksToAttack = 300;

        private readonly List<int> attackHistory = new();

        private int ticksOfTotalFireBreath = 0;
        private int ticksUntilNextFireBreath = 0;

        private int ticksUntilMeteorStrike = 0;
        private int meteorStrikeTicksLeft = 0;

        private int ticksUntilShotgunFireballs = 0;

        private int ticksUntilLavaLurk = 0;

        private int ticksUntilInterweavingFireballs = 0;
        private int ticksOfTotalInterweavingFireballs = 0;
        private int ticksUntilNextInterweavingFireballs = 0;
        private bool interweavingIsOffset = false;

        private Rectangle lavaLurkBox1 = new(11, 22, 6, 6);
        private Rectangle lavaLurkBox2 = new(33, 21, 6, 6);

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
            NetFields.AddFields(meteorStrikeWarningEvent);
            meteorStrikeWarningEvent.onEvent += MeteorStrikeWarning;
            base.initNetFields();
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);
            meteorStrikeWarningEvent.Poll();
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            DoMovementTick(time);

            if (Health <= 0)
                return;

            if (meteorStrikeTicksLeft > 0)
                meteorStrikeTicksLeft--;

            if (ticksOfTotalFireBreath > 0)
            {
                ticksOfTotalFireBreath--;

                if (ticksOfTotalFireBreath == 0)
                {
                    attackState.Value = 0;
                    firing.Value = false;

                    ticksToAttack = 90 - this.AdjustRangeForHealth(0, 60);
                    return;
                }

                if (ticksUntilNextFireBreath > 0)
                {
                    ticksUntilNextFireBreath--;
                    return;
                }

                ShootFireBreath();
                ticksUntilNextFireBreath = 1;
            }

            if (ticksUntilLavaLurk > 0)
            {
                ticksUntilLavaLurk--;

                if (ticksUntilLavaLurk == 0)
                {
                    DoLavaLurkAttack();
                    if (Roguelike.HardMode)
                        DoLavaLurkAttack();

                    ticksToAttack = 90 - this.AdjustRangeForHealth(0, 60);
                }
            }

            if (ticksUntilShotgunFireballs > 0)
            {
                ticksUntilShotgunFireballs--;

                if (ticksUntilShotgunFireballs == 0)
                {
                    ShootShotgunFireballs();

                    attackState.Value = 0;
                    firing.Value = false;

                    ticksToAttack = 90 - this.AdjustRangeForHealth(0, 60);
                }
            }

            if (ticksUntilInterweavingFireballs > 0)
            {
                ticksUntilInterweavingFireballs--;

                if (ticksUntilInterweavingFireballs == 0)
                {
                    ticksOfTotalInterweavingFireballs = 60 * this.AdjustRangeForHealth(5, 8);
                    ticksUntilNextInterweavingFireballs = 30;
                }
            }

            if (ticksOfTotalInterweavingFireballs > 0)
            {
                ticksOfTotalInterweavingFireballs--;

                if (ticksOfTotalInterweavingFireballs == 0)
                {
                    attackState.Value = 0;
                    firing.Value = false;

                    ticksToAttack = 120 - this.AdjustRangeForHealth(0, 60);
                    return;
                }

                if (ticksUntilNextInterweavingFireballs > 0)
                {
                    ticksUntilNextInterweavingFireballs--;
                    return;
                }

                ShootInterweavingFireballs(interweavingIsOffset);
                interweavingIsOffset = !interweavingIsOffset;
                ticksUntilNextInterweavingFireballs = 45 - this.AdjustRangeForHealth(0, 20);
            }

            if (ticksUntilMeteorStrike > 0)
            {
                ticksUntilMeteorStrike--;

                if (ticksUntilMeteorStrike == 0)
                {
                    attackState.Value = 0;

                    DoMeteorStrike();

                    double rng = Game1.random.NextDouble();
                    if (rng < ((float)1 / 3))
                        ticksOfTotalFireBreath = 120;
                    else if (rng < ((float)2 / 3))
                        ticksUntilShotgunFireballs = 120;
                    else
                        ticksUntilLavaLurk = 120;
                }
            }

            if (ticksToAttack > 0)
            {
                ticksToAttack--;
                return;
            }
            else if (IsAttacking())
                return;

            int attack = 0;
            while (attack == -1 || attackHistory.Contains(attack))
            {
                if (meteorStrikeTicksLeft > 0)
                    attack = Game1.random.Next(2, 5);
                else
                    attack = Game1.random.Next(0, 5);
            }

            if (attack == 0)
            {
                attackState.Value = 1;
                firing.Value = true;
                currentLocation.playSound("croak");

                ticksUntilInterweavingFireballs = 45;
            }
            else if (attack == 1)
            {
                attackState.Value = 1;
                currentLocation.playSound("croak");

                ticksUntilMeteorStrike = 120;
            }
            else if (attack == 2)
            {
                firing.Value = false;
                attackState.Value = 1;
                currentLocation.playSound("croak");

                ticksUntilNextFireBreath = 30;
                ticksOfTotalFireBreath = 3 * 60;

                if (Player is not null)
                    faceGeneralDirection(Player.Position);
            }
            else if (attack == 3)
            {
                attackState.Value = 1;
                firing.Value = true;
                currentLocation.playSound("croak");

                ticksUntilShotgunFireballs = 45;
            }
            else if (attack == 4)
            {
                currentLocation.playSound("croak");

                DoLavaLurkAttack();
                if (Roguelike.HardMode)
                    DoLavaLurkAttack();

                ticksToAttack = 3 * 60;
            }

            attackHistory.Add(attack);
            if (attackHistory.Count >= 3)
                attackHistory.RemoveAt(0);
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
            Vector2 tileLoc1 = GetRandomTileInRect(lavaLurkBox1);
            Vector2 tileLoc2 = GetRandomTileInRect(lavaLurkBox2);
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

                meteorStrikeWarningEvent.Fire(randomTile, delay);
            }

            meteorStrikeTicksLeft = maxDelay;
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
            fire_angle += (float)Math.Sin(RoguelikeUtility.DegreesToRadians(ticksOfTotalFireBreath * 60 / 1000f * 180f)) * 25f;

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

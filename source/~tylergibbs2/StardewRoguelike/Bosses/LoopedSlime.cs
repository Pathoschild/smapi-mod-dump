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
using StardewModdingAPI;
using StardewRoguelike.Projectiles;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StardewRoguelike.Bosses
{
    internal class LoopedSlime : GreenSlime, IBossMonster
    {
        public string DisplayName => "Goobins' Juniors";

        public string MapPath
        {
            get { return "boss-slime"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Green Slime_dangerous"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(24, 24); }
        }

        public List<string> MusicTracks
        {
            get { return new() { "jelly_junktion" }; }
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

        private readonly int totalSlimes;

        private int nextBreakHP;

        public LoopedSlime() { }

        public LoopedSlime(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Speed = 3;
            Sprite.LoadTexture(TextureName);
            Scale = 3f;
            color.Value = new Color(Game1.random.Next(180, 250), 20, 120);

            moveTowardPlayerThreshold.Value = 20;

            nextBreakHP = (int)Math.Round(MaxHealth * (2f / 3f));
            stackedSlimes.Value = Roguelike.HardMode ? 4 : 2;
            totalSlimes = stackedSlimes.Value + 1;

            CalculateNextBreak();
        }

        private void CalculateNextBreak()
        {
            int slimesLeft = stackedSlimes.Value;
            if (slimesLeft == 0)
            {
                nextBreakHP = -1;
                return;
            }

            float breakEvery = 1f / totalSlimes;
            float nextBreakPercentage = Math.Max(breakEvery * slimesLeft, breakEvery);
            nextBreakHP = (int)(MaxHealth * nextBreakPercentage);
        }

        public override void OnAttacked(Vector2 trajectory)
        {
            if (!Context.IsMainPlayer || stackedSlimes.Value == 0)
                return;

            if (trajectory.LengthSquared() == 0f)
                trajectory = new Vector2(0f, -1f);
            else
                trajectory.Normalize();
            trajectory *= 16f;

            if (Health > nextBreakHP)
            {
                double roll = Game1.random.NextDouble();
                if (roll <= 0.2)
                {
                    BasicProjectile miniProjectile = new(DamageToFarmer / 3 * 2, 13, Game1.random.Next(3, 7), 0, (float)Math.PI / 16f, trajectory.X, trajectory.Y, Position, "", "", explode: true, damagesMonsters: false, base.currentLocation, this);
                    miniProjectile.height.Value = 24f;
                    miniProjectile.color.Value = color.Value;
                    miniProjectile.ignoreMeleeAttacks.Value = true;
                    miniProjectile.hostTimeUntilAttackable = 0.1f;
                    if (Game1.random.NextDouble() < 0.5)
                        miniProjectile.debuff.Value = 13;

                    currentLocation.projectiles.Add(miniProjectile);
                }
                else if (roll <= 0.45)
                {
                    MineShaft mine = (MineShaft)currentLocation;
                    Monster slime = mine.BuffMonsterIfNecessary(new GreenSlime(getTileLocation() * 64f, 80));
                    slime.isHardModeMonster.Value = true;
                    slime.moveTowardPlayerThreshold.Value = 25;
                    if (!slime.Sprite.textureName.Value.EndsWith("_dangerous"))
                        slime.Sprite.LoadTexture(slime.Sprite.textureName.Value + "_dangerous");
                    Roguelike.AdjustMonster((MineShaft)currentLocation, ref slime);
                    mine.characters.Add(slime);
                }

                return;
            }

            stackedSlimes.Value--;

            SlimeSpawnProjectile projectile = new(Scale, DamageToFarmer / 3 * 2, trajectory.X, trajectory.Y, Position, "", currentLocation, this);
            projectile.height.Value = 24f;
            projectile.color.Value = color.Value;
            projectile.ignoreMeleeAttacks.Value = true;
            projectile.hostTimeUntilAttackable = 0.1f;
            if (Game1.random.NextDouble() < 0.5)
                projectile.debuff.Value = 13;

            currentLocation.projectiles.Add(projectile);

            CalculateNextBreak();
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth = (int)(Sprite.SpriteWidth * 4 * Scale);
            int boxHeight = (int)(Sprite.SpriteHeight * Scale * 2);
            return new Rectangle((int)Position.X, (int)Position.Y + boxHeight, boxWidth, boxHeight);
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            if (stackedSlimes.Value > 0)
            {
                attackedEvent.Fire(new Vector2(xTrajectory, -yTrajectory));
                xTrajectory = 0;
                yTrajectory = 0;
            }
            int actualDamage = Math.Max(1, damage - resilience.Value);

            if (Game1.random.NextDouble() < (double)missChance.Value - (double)missChance.Value * addedPrecision)
                actualDamage = -1;
            else
            {
                if (Game1.random.NextDouble() < 0.025 && cute.Value)
                {
                    if (!focusedOnFarmers)
                    {
                        DamageToFarmer += DamageToFarmer / 2;
                        shake(1000);
                    }
                    focusedOnFarmers = true;
                }

                Slipperiness = 3;
                Health -= actualDamage;
                setTrajectory(xTrajectory, yTrajectory);
                currentLocation.playSound("slimeHit");
                GetType().BaseType!.GetField("readyToJump", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(this, -1);
                IsWalkingTowardPlayer = true;
                if (Health <= 0)
                {
                    currentLocation.playSound("slimedead");

                    Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

                    multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(44, Position, color.Value * 0.66f, 10)
                    {
                        interval = 70f,
                        holdLastFrame = true,
                        alphaFade = 0.01f
                    });
                    multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(44, Position + new Vector2(-16f, 0f), color.Value * 0.66f, 10)
                    {
                        interval = 70f,
                        delayBeforeAnimationStart = 0,
                        holdLastFrame = true,
                        alphaFade = 0.01f
                    });
                    multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(44, Position + new Vector2(0f, 16f), color.Value * 0.66f, 10)
                    {
                        interval = 70f,
                        delayBeforeAnimationStart = 100,
                        holdLastFrame = true,
                        alphaFade = 0.01f
                    });
                    multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(44, Position + new Vector2(16f, 0f), color.Value * 0.66f, 10)
                    {
                        interval = 70f,
                        delayBeforeAnimationStart = 200,
                        holdLastFrame = true,
                        alphaFade = 0.01f
                    });
                }
            }

            if (Health <= 0)
                BossManager.Death(who.currentLocation, who, DisplayName, SpawnLocation);

            return actualDamage;
        }

        public override void draw(SpriteBatch b)
        {
            if (base.IsInvisible || !Utility.isOnScreen(Position, 128))
                return;

            for (int i = 0; i <= stackedSlimes.Value; i++)
            {
                bool top_slime = i == stackedSlimes.Value;
                Vector2 stack_adjustment = Vector2.Zero;
                if (stackedSlimes.Value > 0)
                    stack_adjustment = new Vector2((float)Math.Sin((double)randomStackOffset + Game1.currentGameTime.TotalGameTime.TotalSeconds * Math.PI * 2.0 + (double)(i * 30)) * 8f * Scale, -30 * i * Scale);

                int wagTimer = (int)GetType().BaseType!.GetField("wagTimer", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this)!;
                int readyToJump = (int)GetType().BaseType!.GetField("readyToJump", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this)!;
                NetVector2 facePosition = (NetVector2)GetType().BaseType!.GetField("facePosition", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this)!;

                b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + new Vector2(32f * Scale, GetBoundingBox().Height / 2 * Scale + yOffset) + stack_adjustment, Sprite.SourceRect, prismatic ? Utility.GetPrismaticColor(348 + specialNumber.Value, 5f) : color.Value, 0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, (float)Scale - 0.4f * ((float)ageUntilFullGrown.Value / 120000f)), SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((float)(getStandingY() + i * 2) / 10000f)));
                if (top_slime)
                {
                    int xDongleSource = ((isMoving() || wagTimer > 0) ? (16 * Math.Min(7, Math.Abs(((wagTimer > 0) ? (992 - wagTimer) : (Game1.currentGameTime.TotalGameTime.Milliseconds % 992)) - 496) / 62) % 64) : 48);
                    int yDongleSource = ((isMoving() || wagTimer > 0) ? (24 * Math.Min(1, Math.Max(1, Math.Abs(((wagTimer > 0) ? (992 - wagTimer) : (Game1.currentGameTime.TotalGameTime.Milliseconds % 992)) - 496) / 62) / 4)) : 24);
                    if (hasSpecialItem.Value)
                        yDongleSource += 48;


                    b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + stack_adjustment + new Vector2(32f, GetBoundingBox().Height - 24 * Scale + ((readyToJump <= 0) ? (4 * (-2 + Math.Abs(Sprite.currentFrame % 4 - 2))) : (4 + 4 * (Sprite.currentFrame % 4 % 3))) + yOffset) * Scale, new Microsoft.Xna.Framework.Rectangle(xDongleSource, 168 + yDongleSource, 16, 24), hasSpecialItem ? Color.White : color.Value, 0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, (float)Scale - 0.4f * (ageUntilFullGrown.Value / 120000f)), flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f + 0.0001f)));
                }

                b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + stack_adjustment + (new Vector2(32f, GetBoundingBox().Height / 2 + ((readyToJump <= 0) ? (4 * (-2 + Math.Abs(Sprite.currentFrame % 4 - 2))) : (4 - 4 * (Sprite.currentFrame % 4 % 3))) + yOffset) + facePosition) * Math.Max(0.2f, (float)Scale - 0.4f * (ageUntilFullGrown.Value / 120000f)), new Microsoft.Xna.Framework.Rectangle(32 + ((readyToJump > 0 || focusedOnFarmers) ? 16 : 0), 120 + ((readyToJump < 0 && (focusedOnFarmers || invincibleCountdown > 0)) ? 24 : 0), 16, 24), Color.White * ((FacingDirection == 0) ? 0.5f : 1f), 0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, (float)Scale - 0.4f * (ageUntilFullGrown.Value / 120000f)), SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((getStandingY() + i * 2) / 10000f + 0.0001f)));

                if (isGlowing)
                    b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + stack_adjustment + new Vector2(32f * Scale, GetBoundingBox().Height / 2 * Scale + yOffset), Sprite.SourceRect, glowingColor * glowingTransparency, 0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, Scale), SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.99f : (getStandingY() / 10000f + 0.001f)));
            }
        }
    }
}

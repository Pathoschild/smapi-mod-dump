/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewRoguelike.Extensions;
using StardewRoguelike.Projectiles;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StardewRoguelike.Bosses
{
    public class Skelly : Skeleton, IBossMonster
    {
        public string DisplayName => "Arc the Skeleton Mage";

        public string MapPath => "boss-skeleton";

        public string TextureName => "Characters\\Monsters\\Skeleton";

        public Vector2 SpawnLocation => new(16, 17);

        public List<string> MusicTracks
        {
            get
            {
                if (Roguelike.FloorTickCounter <= TicksToChangeMusic)
                    return new() { "gelus_defensor" };

                return new() { "gelus_defensor_no_intro" };
            }
        }

        public bool InitializeWithHealthbar => true;

        public float Difficulty { get; set; }

        private readonly List<Vector2> MageSpawnLocations = new()
        {
            new(16, 9),
            new(24, 15),
            new(24, 25),
            new(8, 25),
            new(8, 15)
        };

        private readonly List<CircularProjectile> ActiveBones = new();

        private bool SpottedPlayer;

        private readonly NetBool ThrowingAnim = new();

        private readonly NetBool Invincible = new();

        private readonly NetBool Charging = new();

        private bool SpawnedMages = false;
        private bool CachedMagesAreDead = false;

        private int TicksToAttack = 60 * 5;
        private int PreviousAttack = 2;

        private int BonesToThrow = 0;
        private int NextBoneThrow = 0;

        private int FrostBoltsToThrow = 0;
        private int NextFrostBolt = 0;

        private int TicksToSpawnBoneCircles = 0;
        private int TicksToDespawnBoneCircle = 0;

        private int ChargingTicksLeft = 0;
        private int TicksToFreezeAOE = 11 * 60;

        private readonly double TicksToChangeMusic = (129_706 / 1000) * 60;

        public Skelly() : base(Vector2.Zero) { }

        public Skelly(float difficulty) : this()
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Sprite.LoadTexture(TextureName);
            Scale = 3f;

            Speed++;

            moveTowardPlayerThreshold.Value = 16;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(Invincible, Charging, ThrowingAnim);
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (Invincible.Value)
                startGlowing(Color.Orange, false, 0.1f);
            else if (Charging.Value)
                startGlowing(Color.Turquoise, false, 0.1f);

            if (!Invincible.Value && glowingColor == Color.Orange)
                stopGlowing();
            if (!Charging.Value && glowingColor == Color.Turquoise)
                stopGlowing();

            base.update(time, location);
        }

        public override void updateMovement(GameLocation location, GameTime time)
        {
            if (Charging.Value || ThrowingAnim.Value)
            {
                faceDirection(2);
                Halt();
            }
            else
                base.updateMovement(location, time);
        }

        protected override void updateMonsterSlaveAnimation(GameTime time)
        {
            base.updateMonsterSlaveAnimation(time);

            if (ThrowingAnim.Value)
            {
                if (invincibleCountdown > 0)
                {
                    invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
                    if (invincibleCountdown <= 0)
                        stopGlowing();
                }

                Sprite.Animate(time, 20, 5, 150f);
                if (Sprite.currentFrame == 24)
                    Sprite.currentFrame = 23;
            }
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (ThrowingAnim.Value && !Charging.Value)
            {
                if (invincibleCountdown > 0)
                {
                    invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
                    if (invincibleCountdown <= 0)
                        stopGlowing();
                }

                Sprite.Animate(time, 20, 5, 150f);
                if (Sprite.currentFrame == 24)
                {
                    ThrowingAnim.Value = false;
                    Sprite.currentFrame = 23;
                }
            }

            if (Charging.Value)
            {
                if (ChargingTicksLeft > 0)
                {
                    ChargingTicksLeft--;

                    if (ChargingTicksLeft == 0)
                    {
                        FreezeAOE(SpawnedMages ? 5 : 3);
                        currentLocation.playSound("coldSpell");
                        Charging.Value = false;
                        TicksToFreezeAOE = SpawnedMages ? 8 * 60 : 14 * 60;
                    }
                }

                return;
            }

            if (TicksToFreezeAOE > 0 && !Invincible.Value)
            {
                TicksToFreezeAOE--;

                if (TicksToFreezeAOE == 0)
                {
                    ChargingTicksLeft = 80;
                    Charging.Value = true;
                }
            }

            if (!SpottedPlayer && Utility.doesPointHaveLineOfSightInMine(currentLocation, getTileLocation(), Player.getTileLocation(), 8))
            {
                controller = new PathFindController(this, currentLocation, new(Player.getStandingX() / 64, Player.getStandingY() / 64), -1, null, 200);
                SpottedPlayer = true;
                if (controller is null || controller.pathToEndPoint is null || controller.pathToEndPoint.Count == 0)
                {
                    Halt();
                    facePlayer(Player);
                }
                currentLocation.playSound("skeletonStep");
                IsWalkingTowardPlayer = true;
            }

            if (TicksToDespawnBoneCircle > 0)
            {
                TicksToDespawnBoneCircle--;

                if (TicksToDespawnBoneCircle == 0)
                    DespawnBoneCircles();
            }

            if (TicksToSpawnBoneCircles > 0)
            {
                TicksToSpawnBoneCircles--;

                if (TicksToSpawnBoneCircles == 60)
                    RoguelikeUtility.DoAttackCue(currentLocation, 60);

                if (TicksToSpawnBoneCircles == 0)
                    SpawnBoneCircles();
            }

            if (BonesToThrow > 0)
            {
                if (NextBoneThrow == 0)
                {
                    ThrowBone();
                    BonesToThrow--;
                    NextBoneThrow = 20;
                }
                else
                    NextBoneThrow--;
            }

            if (FrostBoltsToThrow > 0)
            {
                if (NextFrostBolt == 0)
                {
                    ThrowFrostBolt();
                    FrostBoltsToThrow--;
                    NextFrostBolt = 20;
                }
                else
                    NextFrostBolt--;
            }


            if (ThrowingAnim.Value && BonesToThrow == 0 && FrostBoltsToThrow == 0)
                ThrowingAnim.Value = false;

            if (TicksToAttack == 0 && TicksToSpawnBoneCircles == 0)
                TicksToAttack = SpawnedMages ? 60 * 4 : 60 * 5;

            if (TicksToAttack > 0)
            {
                TicksToAttack--;

                if (TicksToAttack == 0)
                {
                    int attack = PreviousAttack switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 0,
                        _ => throw new NotImplementedException()
                    };
                    PreviousAttack = attack;

                    if (attack == 0)
                    {
                        ThrowingAnim.Value = true;
                        BonesToThrow = SpawnedMages ? 5 : 3;
                        if (Roguelike.HardMode)
                            BonesToThrow += 2;
                        NextBoneThrow = 20;
                    }
                    else if (attack == 1)
                    {
                        ThrowingAnim.Value = true;
                        FrostBoltsToThrow = SpawnedMages ? 5 : 3;
                        if (Roguelike.HardMode)
                            BonesToThrow += 2;
                        NextFrostBolt = 20;
                    }
                    else
                        TicksToSpawnBoneCircles = 61;
                }
            }

            if (SpawnedMages && MagesAreDead())
                Invincible.Value = false;

            if (Health <= MaxHealth * ((float)1 / 2) && !SpawnedMages)
            {
                SpawnMages();
                Invincible.Value = true;
            }
        }

        private void SpawnMages()
        {
            foreach (Vector2 tilePosition in MageSpawnLocations)
            {
                SkellyMinion mage = new(tilePosition * 64f, Difficulty, isMage: true);
                if (!Roguelike.HardMode)
                    mage.Speed = 0;
                currentLocation.characters.Add(mage);
            }

            Speed++;
            SpawnedMages = true;
        }

        private bool MagesAreDead()
        {
            // cached
            if (CachedMagesAreDead)
                return true;

            int count = 0;

            foreach (Character character in currentLocation.characters)
            {
                if (character is Skeleton && character != this)
                    count++;
            }

            if (count == 0)
            {
                CachedMagesAreDead = true;
                if (!Roguelike.HardMode)
                    Speed--;

                return true;
            }

            return false;
        }

        public void FreezeAOE(int radius)
        {
            Vector2 tileLocation = getTileLocation();
            Vector2 currentTile = new(Math.Min(currentLocation.map.Layers[0].LayerWidth - 1, Math.Max(0f, tileLocation.X - radius)), Math.Min(currentLocation.map.Layers[0].LayerHeight - 1, Math.Max(0f, tileLocation.Y - radius)));

            Rectangle areaOfEffect = new((int)(tileLocation.X - radius) * 64, (int)(tileLocation.Y - radius) * 64, (radius * 2 + 1) * 64, (radius * 2 + 1) * 64);

            List<TemporaryAnimatedSprite> sprites = new()
            {
                new TemporaryAnimatedSprite(23, 9999f, 6, 1, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5))
                {
                    light = true,
                    lightRadius = radius,
                    lightcolor = Color.Black,
                    alphaFade = 0.03f - radius * 0.003f,
                    Parent = currentLocation
                }
            };
            MethodInfo rumbleAndFade = currentLocation.GetType().BaseType!.GetMethod("rumbleAndFade", BindingFlags.Instance | BindingFlags.NonPublic)!;
            rumbleAndFade.Invoke(currentLocation, new object[] { 300 + radius * 100 });

            currentLocation.debuffPlayers(areaOfEffect, 19);

            Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

            int startingI = (int)(tileLocation.X - radius) * 64;
            int startingJ = (int)(tileLocation.Y - radius) * 64;

            for (int i = startingI; i <= startingI + (radius * 2) * 64; i += 64)
            {
                for (int j = startingJ; j <= startingJ + (radius * 2) * 64; j += 64)
                {
                    sprites.Add(new(13, new Vector2(i, j), Color.White, 9, flipped: false, 50f)
                    {
                        delayBeforeAnimationStart = Game1.random.Next(200),
                        scale = Game1.random.Next(5, 15) / 10f
                    });
                }
            }

            multiplayer.broadcastSprites(currentLocation, sprites);
        }

        public void SpawnBoneCircles()
        {
            currentLocation.playSound("skeletonHit");

            foreach (Farmer farmer in currentLocation.farmers)
            {
                Vector2 playerCenter = new(farmer.Position.X, farmer.Position.Y - farmer.Sprite.getHeight() - 24);
                for (int xOffset = -64; xOffset <= 64; xOffset += 128)
                {
                    for (int yOffset = -64; yOffset <= 64; yOffset += 128)
                    {
                        CircularProjectile proj = new(
                            (int)(DamageToFarmer * Difficulty),
                            4,
                            0,
                            0,
                            xOffset < 0 ? 0.3f : -0.3f,
                            3f,
                            playerCenter,
                            new Vector2(playerCenter.X + xOffset, playerCenter.Y + yOffset),
                            "skeletonHit",
                            "",
                            false,
                            false,
                            currentLocation,
                            this,
                            false,
                            null!
                        );
                        currentLocation.projectiles.Add(proj);
                        proj.IgnoreLocationCollision = true;
                        ActiveBones.Add(proj);
                    }
                }
            }

            TicksToDespawnBoneCircle = 100;
        }

        public void DespawnBoneCircles()
        {
            currentLocation.playSound("skeletonHit");

            foreach (CircularProjectile bone in ActiveBones)
                currentLocation.projectiles.Remove(bone);

            ActiveBones.Clear();
        }

        public void ThrowBone()
        {
            if (Player is null)
                return;

            float boneSpeed = SpawnedMages ? 10f : 8f;
            Vector2 v = Utility.getVelocityTowardPlayer(new Point((int)Position.X, (int)Position.Y), boneSpeed, Player);
            BasicProjectile projectile = new((int)Math.Round(DamageToFarmer * Difficulty), 4, 0, 0, (float)Math.PI / 16f, v.X, v.Y, new Vector2(Position.X, Position.Y), "skeletonHit", "skeletonStep", explode: false, damagesMonsters: false, currentLocation, this);
            projectile.IgnoreLocationCollision = true;
            projectile.ignoreMeleeAttacks.Value = true;
            currentLocation.projectiles.Add(projectile);
        }

        public void ThrowFrostBolt()
        {
            float boneSpeed = SpawnedMages ? 10f : 8f;
            Vector2 v = Utility.getVelocityTowardPlayer(new Point((int)Position.X, (int)Position.Y), boneSpeed, Player);
            BasicProjectile projectile = new((int)Math.Round(DamageToFarmer * Difficulty), 9, 0, 4, 0f, v.X, v.Y, new Vector2(Position.X, Position.Y), "flameSpellHit", "flameSpell", explode: false, damagesMonsters: false, currentLocation, this);
            projectile.IgnoreLocationCollision = true;
            projectile.ignoreMeleeAttacks.Value = true;
            currentLocation.projectiles.Add(projectile);
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth = (int)(Sprite.SpriteWidth * 4 * 3 / 4 * Scale);
            int boxHeight = (int)(Sprite.SpriteHeight * Scale);
            return new Rectangle((int)Position.X - boxWidth / 3 + 8, (int)Position.Y + boxHeight / 2, boxWidth, boxHeight);
        }

        public override void shedChunks(int number)
        {
            Game1.createRadialDebris(currentLocation, Sprite.textureName.Value, new Rectangle(0, Sprite.SpriteHeight * 4, Sprite.SpriteWidth, Sprite.SpriteWidth), 8, GetBoundingBox().Center.X, GetBoundingBox().Center.Y, number, (int)getTileLocation().Y, Color.White, 4f);
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            int actualDamage = Math.Max(1, damage - resilience.Value);
            if (Invincible.Value || Charging.Value)
            {
                currentLocation.playSound("crafting");
                return 0;
            }
            if (Game1.random.NextDouble() < missChance - missChance * addedPrecision)
                actualDamage = -1;
            else
            {
                Health -= actualDamage;
                currentLocation.playSound("hitEnemy");
                setTrajectory(xTrajectory / 3, yTrajectory / 3);
                if (Health <= 0)
                {
                    deathAnimation();
                    BossManager.Death(currentLocation, who, DisplayName, SpawnLocation);
                }
            }

            return actualDamage;
        }
    }
}

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

        public string MapPath
        {
            get { return "boss-skeleton"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Skeleton"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(16, 17); }
        }

        public List<string> MusicTracks
        {
            get { return new() { "megalovania" }; }
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

        private readonly List<Vector2> mageSpawnLocations = new()
        {
            new(16, 9),
            new(24, 15),
            new(24, 25),
            new(8, 25),
            new(8, 15)
        };

        private List<CircularProjectile> activeBones = new();

        private bool spottedPlayer;

        private readonly NetBool throwingAnim = new();

        private readonly NetBool invincible = new();

        private readonly NetBool charging = new();

        private bool spawnedMages = false;
        private bool magesAreDead = false;

        private int ticksToAttack = 60 * 5;
        private int previousAttack = 2;

        private int bonesToThrow = 0;
        private int nextBoneThrow = 0;

        private int frostBoltsToThrow = 0;
        private int nextFrostBolt = 0;

        private int ticksToSpawnBoneCircles = 0;
        private int ticksToDespawnBoneCircle = 0;

        private int chargingTicksLeft = 0;
        private int ticksToFreezeAOE = 11 * 60;

        public Skelly() { }

        public Skelly(float difficulty) : base(Vector2.Zero)
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
            NetFields.AddFields(invincible, charging, throwingAnim);
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (invincible.Value)
                startGlowing(Color.Orange, false, 0.1f);
            else if (charging.Value)
                startGlowing(Color.Turquoise, false, 0.1f);

            if (!invincible.Value && glowingColor == Color.Orange)
                stopGlowing();
            if (!charging.Value && glowingColor == Color.Turquoise)
                stopGlowing();

            base.update(time, location);
        }

        public override void updateMovement(GameLocation location, GameTime time)
        {
            if (charging.Value || throwingAnim.Value)
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

            if (throwingAnim.Value)
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
            if (throwingAnim.Value && !charging.Value)
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
                    throwingAnim.Value = false;
                    Sprite.currentFrame = 23;
                }
            }

            if (charging.Value)
            {
                if (chargingTicksLeft > 0)
                {
                    chargingTicksLeft--;

                    if (chargingTicksLeft == 0)
                    {
                        FreezeAOE(spawnedMages ? 5 : 3);
                        currentLocation.playSound("coldSpell");
                        charging.Value = false;
                        ticksToFreezeAOE = spawnedMages ? 8 * 60 : 14 * 60;
                    }
                }

                return;
            }

            if (ticksToFreezeAOE > 0 && !invincible.Value)
            {
                ticksToFreezeAOE--;

                if (ticksToFreezeAOE == 0)
                {
                    chargingTicksLeft = 80;
                    charging.Value = true;
                }
            }

            if (!spottedPlayer && Utility.doesPointHaveLineOfSightInMine(currentLocation, getTileLocation(), Player.getTileLocation(), 8))
            {
                controller = new PathFindController(this, currentLocation, new(Player.getStandingX() / 64, Player.getStandingY() / 64), -1, null, 200);
                spottedPlayer = true;
                if (controller is null || controller.pathToEndPoint is null || controller.pathToEndPoint.Count == 0)
                {
                    Halt();
                    facePlayer(Player);
                }
                currentLocation.playSound("skeletonStep");
                IsWalkingTowardPlayer = true;
            }

            if (ticksToDespawnBoneCircle > 0)
            {
                ticksToDespawnBoneCircle--;

                if (ticksToDespawnBoneCircle == 0)
                    DespawnBoneCircles();
            }

            if (ticksToSpawnBoneCircles > 0)
            {
                ticksToSpawnBoneCircles--;

                if (ticksToSpawnBoneCircles == 60)
                    currentLocation.playSound("shadowpeep");

                if (ticksToSpawnBoneCircles == 0)
                    SpawnBoneCircles();
            }

            if (bonesToThrow > 0)
            {
                if (nextBoneThrow == 0)
                {
                    ThrowBone();
                    bonesToThrow--;
                    nextBoneThrow = 20;
                }
                else
                    nextBoneThrow--;
            }

            if (frostBoltsToThrow > 0)
            {
                if (nextFrostBolt == 0)
                {
                    ThrowFrostBolt();
                    frostBoltsToThrow--;
                    nextFrostBolt = 20;
                }
                else
                    nextFrostBolt--;
            }


            if (throwingAnim.Value && bonesToThrow == 0 && frostBoltsToThrow == 0)
                throwingAnim.Value = false;

            if (ticksToAttack == 0 && ticksToSpawnBoneCircles == 0)
                ticksToAttack = spawnedMages ? 60 * 4 : 60 * 5;

            if (ticksToAttack > 0)
            {
                ticksToAttack--;

                if (ticksToAttack == 0)
                {
                    int attack = previousAttack switch
                    {
                        0 => 1,
                        1 => 2,
                        2 => 0,
                        _ => throw new NotImplementedException()
                    };
                    previousAttack = attack;

                    if (attack == 0)
                    {
                        throwingAnim.Value = true;
                        bonesToThrow = spawnedMages ? 5 : 3;
                        if (Roguelike.HardMode)
                            bonesToThrow += 2;
                        nextBoneThrow = 20;
                    }
                    else if (attack == 1)
                    {
                        throwingAnim.Value = true;
                        frostBoltsToThrow = spawnedMages ? 5 : 3;
                        if (Roguelike.HardMode)
                            bonesToThrow += 2;
                        nextFrostBolt = 20;
                    }
                    else
                        ticksToSpawnBoneCircles = 61;
                }
            }

            if (spawnedMages && MagesAreDead())
                invincible.Value = false;

            if (Health <= MaxHealth * ((float)1 / 2) && !spawnedMages)
            {
                SpawnMages();
                invincible.Value = true;
            }
        }

        private void SpawnMages()
        {
            foreach (Vector2 tilePosition in mageSpawnLocations)
            {
                SkellyMinion mage = new(tilePosition * 64f, Difficulty, isMage: true);
                if (!Roguelike.HardMode)
                    mage.Speed = 0;
                currentLocation.characters.Add(mage);
            }

            Speed++;
            spawnedMages = true;
        }

        private bool MagesAreDead()
        {
            // cached
            if (magesAreDead)
                return true;

            int count = 0;

            foreach (Character character in currentLocation.characters)
            {
                if (character is Skeleton && character != this)
                    count++;
            }

            if (count == 0)
            {
                magesAreDead = true;
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
                        activeBones.Add(proj);
                    }
                }
            }

            ticksToDespawnBoneCircle = 100;
        }

        public void DespawnBoneCircles()
        {
            currentLocation.playSound("skeletonHit");

            foreach (CircularProjectile bone in activeBones)
                currentLocation.projectiles.Remove(bone);

            activeBones.Clear();
        }

        public void ThrowBone()
        {
            if (Player is null)
                return;

            float boneSpeed = spawnedMages ? 10f : 8f;
            Vector2 v = Utility.getVelocityTowardPlayer(new Point((int)Position.X, (int)Position.Y), boneSpeed, Player);
            BasicProjectile projectile = new((int)Math.Round(DamageToFarmer * Difficulty), 4, 0, 0, (float)Math.PI / 16f, v.X, v.Y, new Vector2(Position.X, Position.Y), "skeletonHit", "skeletonStep", explode: false, damagesMonsters: false, currentLocation, this);
            projectile.IgnoreLocationCollision = true;
            projectile.ignoreMeleeAttacks.Value = true;
            currentLocation.projectiles.Add(projectile);
        }

        public void ThrowFrostBolt()
        {
            float boneSpeed = spawnedMages ? 10f : 8f;
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
            if (invincible.Value || charging.Value)
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

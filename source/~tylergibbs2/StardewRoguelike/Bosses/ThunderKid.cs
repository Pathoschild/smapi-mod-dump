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
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StardewRoguelike.Bosses
{
    public class ThunderKid : SquidKid, IBossMonster
    {
        public string DisplayName => "Telesto the Full Moon";

        public string MapPath
        {
            get { return "boss-thunderkid"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Squid Kid"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(25, 25); }
        }

        public List<string> MusicTracks
        {
            get { return new() { "photophobia" }; }
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

        private static readonly List<Vector2> TeleportationPositions = new()
        {
            new(16, 15),
            new(25, 15),
            new(34, 15),
            new(16, 25),
            new(25, 25),
            new(34, 25),
            new(16, 34),
            new(25, 34),
            new(34, 34),
            new(22, 16),
            new(30, 13),
            new(37, 21),
            new(13, 19),
            new(24, 19),
            new(21, 23),
            new(33, 26),
            new(13, 29),
            new(23, 34),
            new(35, 30),
            new(16, 27)
        };

        private readonly List<Vector2> TeleporationHistory = new()
        {
            new(25, 25)
        };

        private float lastIceBall;
        private float lastLightning = 2000f;
        private bool startedLightning = false;
        private Vector2 positionToStrike;
        private int projectileCount = 0;

        private int ticksToBurn = 0;

        private int burnCooldown = 0;

        private readonly NetBool charging = new();

        private readonly NetEvent0 onAttacked = new();

        private readonly NetEvent1Field<Vector2, NetVector2> lightningStrikeWarningEvent = new();

        private readonly NetEvent1Field<Vector2, NetVector2> lightningStrikeEvent = new();

        public ThunderKid() { }

        public ThunderKid(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Sprite.SpriteWidth = 16;
            Sprite.SpriteHeight = 16;
            Sprite.LoadTexture(TextureName);

            Scale = 4f;
            moveTowardPlayerThreshold.Value = 20;
        }

        protected override void initNetFields()
        {
            NetFields.AddFields(lightningStrikeEvent, lightningStrikeWarningEvent, onAttacked, charging);
            lightningStrikeEvent.onEvent += LightningStrike;
            lightningStrikeWarningEvent.onEvent += LightningStrikeWarning;
            onAttacked.onEvent += OnAttacked;
            base.initNetFields();
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (charging.Value)
                startGlowing(Color.DarkRed, false, 0.1f);
            else if (!charging.Value && glowingColor == Color.DarkRed)
                stopGlowing();

            base.update(time, location);
            lightningStrikeEvent.Poll();
            lightningStrikeWarningEvent.Poll();
            onAttacked.Poll();
        }

        private void OnAttacked()
        {
            if (!Context.IsMainPlayer)
                return;

            if (burnCooldown == 0 && ticksToBurn == 0)
            {
                ticksToBurn = 90;
                charging.Value = true;
                currentLocation.playSound("croak");
            }
        }

        public void BurnAOE(int radius)
        {
            Vector2 tileLocation = getTileLocation();
            Vector2 currentTile = new(Math.Min(currentLocation.map.Layers[0].LayerWidth - 1, Math.Max(0f, tileLocation.X - radius)), Math.Min(currentLocation.map.Layers[0].LayerHeight - 1, Math.Max(0f, tileLocation.Y - radius)));

            Rectangle areaOfEffect = new((int)(tileLocation.X - radius) * 64, (int)(tileLocation.Y - radius) * 64, (radius * 2 + 1) * 64, (radius * 2 + 1) * 64);

            List<TemporaryAnimatedSprite> sprites = new()
            {
                new TemporaryAnimatedSprite(35, 9999f, 6, 1, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5))
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

            currentLocation.debuffPlayers(areaOfEffect, 12);

            MethodInfo damagePlayers = currentLocation.GetType().BaseType!.GetMethod("damagePlayers", BindingFlags.Instance | BindingFlags.NonPublic)!;
            damagePlayers.Invoke(currentLocation, new object[] { areaOfEffect, DamageToFarmer });

            Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

            int startingI = (int)(tileLocation.X - radius) * 64;
            int startingJ = (int)(tileLocation.Y - radius) * 64;

            for (int i = startingI; i <= startingI + (radius * 2) * 64; i += 64)
            {
                for (int j = startingJ; j <= startingJ + (radius * 2) * 64; j += 64)
                {
                    sprites.Add(new(5, new Vector2(i, j), Color.White, 9, flipped: false, 50f)
                    {
                        delayBeforeAnimationStart = Game1.random.Next(200),
                        scale = Game1.random.Next(5, 15) / 10f
                    });
                }
            }

            multiplayer.broadcastSprites(currentLocation, sprites);
        }

        private void TeleportToRandomPosition()
        {
            Vector2 lastPosition = TeleporationHistory.Last();
            if (TeleportationPositions.Count == TeleporationHistory.Count)
            {
                TeleporationHistory.Clear();
                TeleporationHistory.Add(lastPosition);
            }

            Vector2 randomPosition;
            do
            {
                randomPosition = TeleportationPositions[Game1.random.Next(TeleportationPositions.Count)];
            } while (TeleporationHistory.Contains(randomPosition));

            TeleporationHistory.Add(randomPosition);

            Position = randomPosition * 64f;
        }

        protected override void updateMonsterSlaveAnimation(GameTime time)
        {
            base.updateMonsterSlaveAnimation(time);

            if (charging.Value)
            {
                Sprite.currentFrame = 1;
                faceDirection(2);
            }
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (burnCooldown > 0)
                burnCooldown--;

            if (ticksToBurn > 0)
            {
                Sprite.currentFrame = 1;
                faceDirection(2);
                ticksToBurn--;

                if (ticksToBurn == 0)
                {
                    currentLocation.playSound("explosion");
                    BurnAOE(4);
                    burnCooldown = 7 * 60;
                    charging.Value = false;
                    TeleportToRandomPosition();
                }

                return;
            }

            typeof(ThunderKid).BaseType!.GetField("lastFireball", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(this, 1000f);
            base.behaviorAtGameTick(time);
            if (Health <= 0)
                return;

            if (Player is not null && withinPlayerThreshold(20))
            {
                lastIceBall = Math.Max(0f, lastIceBall - time.ElapsedGameTime.Milliseconds);
                lastLightning = Math.Max(0f, lastLightning - time.ElapsedGameTime.Milliseconds);

                if (!startedLightning && lastLightning < (Health < MaxHealth / 2 ? 500f : 1000f))
                {
                    startedLightning = true;
                    positionToStrike = Player.Position;
                    lightningStrikeWarningEvent.Fire(positionToStrike);
                }

                if (lastLightning == 0f)
                {
                    startedLightning = false;
                    lightningStrikeEvent.Fire(positionToStrike);
                    lastLightning = Game1.random.Next(2000, 4000) * (Health < MaxHealth / 2 ? 1 : 2);
                    if (Roguelike.HardMode)
                        lastLightning -= 600;
                }

                if (lastIceBall == 0f)
                {
                    Vector2 trajectory = RoguelikeUtility.VectorFromDegrees(Game1.random.Next(0, 360)) * 10f;
                    BasicProjectile projectile = new(DamageToFarmer, 9, 3, 4, 0f, trajectory.X, trajectory.Y, GetProjectileOrigin(), "", "", false, false, currentLocation, this, false, null);
                    projectile.ignoreMeleeAttacks.Value = true;
                    projectile.debuff.Value = 19;
                    currentLocation.projectiles.Add(projectile);

                    projectileCount++;

                    if (projectileCount >= (Health < MaxHealth / 2 ? 8 : 4))
                    {
                        projectileCount = 0;
                        lastIceBall = Game1.random.Next(1400, 3600);
                        if (Roguelike.HardMode)
                            lastIceBall -= 400;
                    }
                    else
                    {
                        if (Roguelike.HardMode)
                            lastIceBall = 150;
                        else
                            lastIceBall = 250;
                    }

                    if (lastIceBall != 0f && Game1.random.NextDouble() < 0.05)
                        Halt();
                }

            }
        }

        private Vector2 GetProjectileOrigin()
        {
            return new(GetBoundingBox().X, GetBoundingBox().Y);
        }

        private void LightningStrike(Vector2 playerLocation)
        {
            Farm.LightningStrikeEvent lightningEvent = new();
            lightningEvent.bigFlash = false;
            lightningEvent.smallFlash = false;
            lightningEvent.createBolt = true;
            lightningEvent.boltPosition = playerLocation + new Vector2(32f, 32f);
            Game1.playSound("thunder");
            Utility.drawLightningBolt(lightningEvent.boltPosition, currentLocation);

            foreach (Farmer farmer in currentLocation.farmers)
            {
                if (farmer.currentLocation == currentLocation && farmer.GetBoundingBox().Intersects(new Rectangle((int)Math.Round(playerLocation.X - 32), (int)Math.Round(playerLocation.Y - 32), 64, 64)))
                    farmer.takeDamage((int)Math.Round(DamageToFarmer * 1.25f), true, null);
            }
        }

        private void LightningStrikeWarning(Vector2 position)
        {
            Rectangle lightningSourceRect = new(0, 0, 16, 16);
            float markerScale = 8f;
            Vector2 drawPosition = position + new Vector2(-16 * markerScale / 2 + 32f, -16 * markerScale / 2 + 32f);

            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\Projectiles", lightningSourceRect, 9999f, 1, 999, drawPosition, false, Game1.random.NextDouble() < 0.5, (position.Y + 32f) / 10000f + 0.001f, 0.025f, Color.White, markerScale, 0f, 0f, 0f, false)
            {
                light = true,
                lightRadius = 2f,
                delayBeforeAnimationStart = 200,
                lightcolor = Color.Black
            });
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth = (int)(Sprite.SpriteWidth * 4 * 1 / 2 * Scale);
            int boxHeight = (int)(Sprite.SpriteHeight * Scale * 1.5);
            return new Rectangle((int)Position.X - boxWidth / 4, (int)Position.Y + boxHeight / 6, boxWidth, boxHeight);
        }
        public override void drawAboveAllLayers(SpriteBatch b)
        {
            b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + new Vector2(Sprite.SpriteWidth * 2, (float)(21 + yOffset)), new Rectangle?(Sprite.SourceRect), Color.White, 0f, new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight), scale * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f)));
            b.Draw(Game1.shadowTexture, getLocalPosition(Game1.viewport) + new Vector2(Sprite.SpriteWidth * 2, Sprite.SpriteHeight * 4), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f + yOffset / 20f, SpriteEffects.None, (getStandingY() - 1) / 10000f);
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            onAttacked.Fire();
            int result = base.takeDamage(damage, 0, 0, isBomb, addedPrecision, who);
            setTrajectory(xTrajectory / 6, yTrajectory / 6);
            if (Health <= 0)
                BossManager.Death(currentLocation, who, DisplayName, SpawnLocation);

            return result;
        }
    }
}

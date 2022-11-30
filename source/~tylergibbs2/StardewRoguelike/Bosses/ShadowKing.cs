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
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using StardewRoguelike.VirtualProperties;
using StardewValley.Locations;

namespace StardewRoguelike.Bosses
{
    public class MinionSpawnRectangle
    {
        public int X1;

        public int Y1;

        public int X2;

        public int Y2;

        public int Height;

        public int Width;

        public int Perimeter;

        public MinionSpawnRectangle(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;

            Height = Y2 - Y1;
            Width = X2 - X1;

            Perimeter = (X2 - X1) * 2 + (Y2 - Y1) * 2;
        }

        public List<Vector2> GetRandomPositions(int amount)
        {
            List<Vector2> positions = new();

            while (amount > 0)
            {
                Vector2 pos;

                int perimeterPoint = Game1.random.Next(Perimeter);
                if (perimeterPoint < Height)
                    pos = new(X1, Y1 + perimeterPoint);
                else if (perimeterPoint < Height + Width)
                    pos = new(X1 + perimeterPoint - Height, Y1 + Height);
                else if (perimeterPoint < (Height + Width) + Height)
                    pos = new(X1 + Width, Y1 + perimeterPoint - (Height + Width));
                else
                    pos = new(X1 + perimeterPoint - (Height + Width + Height), Y1);

                if (positions.Contains(pos))
                    continue;

                positions.Add(pos);
                amount--;
            }

            return positions;
        }
    }

    public class ShadowKing : Monster, IBossMonster
    {
        public string DisplayName => "Ozul the Shadow King";

        public string MapPath
        {
            get { return "boss-shadowking"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Shadow Shaman_dangerous"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(21, 94); }
        }

        public List<string> MusicTracks
        {
            get
            {
                if (encountered.Value)
                    return new() { "cowboy_boss" };

                return new() { "Lava_Ambient" };
            }
        }

        private float _difficulty;

        public float Difficulty
        {
            get { return _difficulty; }
            set { _difficulty = value; }
        }

        public bool InitializeWithHealthbar
        {
            get { return false; }
        }

        private bool mapChangesEnabled = false;

        private readonly int[] tilesToBlockIds = new int[] { 104, 106, 103, 120, 122, 119 };

        private readonly List<Vector2> tilesToBlock = new()
        {
            new(20, 75),
            new(21, 75),
            new(22, 75),
            new(20, 76),
            new(21, 76),
            new(22, 76)
        };

        private readonly List<Vector2> tilesToWarp = new()
        {
            new(20, 70),
            new(21, 70),
            new(22, 70),
            new(20, 75),
            new(21, 75),
            new(22, 75),
            new(20, 76),
            new(21, 76),
            new(22, 76)
        };

        private readonly Vector2 warpDestination = new(21, 78);

        private bool oldEncountered = false;

        public readonly NetBool encountered = new();

        private readonly int explosionCooldownTicks = 300;

        private readonly int width;
        private readonly int height;

        private int attackPhase = 0;

        private int ticksSinceLastExplosion = 0;
        private int ticksToExplode = 0;
        private int explosionRadius;
        private int explosionDamage;

        private int ticksToArrowVolley = 0;

        private int ticksToSpawnBadMinions = 0;
        private int ticksToJinxCircle = 0;

        private int ticksToSpawnGoodMinions = 0;

        public MinionSpawnRectangle phase2_SpawnRect = new(10, 82, 32, 104);

        public ShadowKing() { }

        public ShadowKing(float difficulty) : base("Shadow Shaman", Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            width = 16;
            height = 24;
            Sprite.SpriteWidth = width;
            Sprite.SpriteHeight = height;
            Sprite.LoadTexture(TextureName);
            Scale = 2f;

            moveTowardPlayerThreshold.Value = 16;
        }

        protected override void initNetFields()
        {
            NetFields.AddFields(new INetSerializable[]
            {
                encountered
            });
            base.initNetFields();
        }

        public override void reloadSprite()
        {
            Sprite = new(TextureName)
            {
                SpriteWidth = width,
                SpriteHeight = height
            };
            Sprite.LoadTexture(TextureName);
            HideShadow = true;
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);

            if (Health > 0 && !mapChangesEnabled && encountered.Value)
                EnableMapChanges();

            if (oldEncountered != encountered.Value)
                RevealBoss();

            oldEncountered = encountered.Value;
        }

        private static void RevealBoss()
        {
            BossManager.StartRenderHealthBar();
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            base.behaviorAtGameTick(time);

            if (!mapChangesEnabled && withinPlayerThreshold(16))
                EnableMapChanges();

            if (Health <= 0)
                return;

            // Explode on hit
            if (ticksToExplode > 0)
            {
                ticksToExplode--;

                if (ticksToExplode == 0)
                {
                    stopGlowing();
                    currentLocation.netAudio.StopPlaying("fuse");

                    currentLocation.playSound("explosion");
                    currentLocation.explode(new Vector2(Position.X / 64, Position.Y / 64), explosionRadius, Player, damageFarmers: true, damage_amount: (int)(explosionDamage * Difficulty));
                    ticksSinceLastExplosion = 0;
                }
            }
            else
            {
                ticksSinceLastExplosion++;
            }

            if (attackPhase == 0)
            {
                // do phase 1 stuff

                if (withinPlayerThreshold(6) && ticksToExplode == 0 && ticksToArrowVolley == 0)
                    ticksToArrowVolley = Game1.random.Next(60 * 5, 60 * 7);

                if (ticksToArrowVolley > 0 && ticksToExplode == 0)
                {
                    ticksToArrowVolley--;

                    if (ticksToArrowVolley == 45)
                        currentLocation.playSound("shadowpeep");

                    if (ticksToArrowVolley == 0)
                        FireArrowCone();
                }

                // Phase 2 transition
                if (Health <= MaxHealth * 0.66f)
                {
                    currentLocation.netAudio.StopPlaying("fuse");
                    ticksToExplode = 0;
                    ticksToArrowVolley = 0;

                    Halt();
                    currentLocation.playSound("explosion");
                    foreach (Farmer farmer in currentLocation.farmers)
                    {
                        Vector2 trajectory = Utility.getAwayFromPositionTrajectory(Game1.player.GetBoundingBox(), Position);
                        farmer.setTrajectory(trajectory * 2);
                    }

                    ticksToJinxCircle = 120;
                    ticksToSpawnBadMinions = 180;

                    attackPhase = 1;
                }
            }
            else if (attackPhase == 1)
            {
                // do phase 2 stuff

                if (ticksToSpawnBadMinions == 0)
                    ticksToSpawnBadMinions = 60 * Game1.random.Next(10, 13);

                if (ticksToJinxCircle == 0)
                    ticksToJinxCircle = 60 * Game1.random.Next(5, 9);

                if (ticksToSpawnBadMinions > 0)
                {
                    ticksToSpawnBadMinions--;

                    if (ticksToSpawnBadMinions == 0)
                    {
                        if (Roguelike.HardMode)
                            SpawnGoodMinions(7);
                        else
                            SpawnBadMinions(20);
                    }
                }

                if (ticksToJinxCircle > 0)
                {
                    ticksToJinxCircle--;

                    if (ticksToJinxCircle == 0)
                        FireJinxCircle();
                }

                if (Health <= MaxHealth * 0.25f)
                {
                    ticksToSpawnGoodMinions = 120;

                    speed = 4;
                    attackPhase = 2;
                }
            }
            else if (attackPhase == 2)
            {
                // do phase 3 stuff

                if (ticksToSpawnGoodMinions == 0)
                    ticksToSpawnGoodMinions = 60 * Game1.random.Next(8, 11);

                if (ticksToJinxCircle == 0)
                    ticksToJinxCircle = 60 * Game1.random.Next(3, 7);

                if (withinPlayerThreshold(6) && ticksToArrowVolley == 0)
                    ticksToArrowVolley = Game1.random.Next(60 * 5, 60 * 7);

                if (ticksToJinxCircle > 0)
                {
                    ticksToJinxCircle--;

                    if (ticksToJinxCircle == 0)
                        FireJinxCircle();
                }

                if (ticksToArrowVolley > 0)
                {
                    ticksToArrowVolley--;

                    if (ticksToArrowVolley == 45)
                        currentLocation.playSound("shadowpeep");

                    if (ticksToArrowVolley == 0)
                    {
                        speed = 4;
                        FireArrowCone();
                    }
                }

                if (ticksToSpawnGoodMinions > 0)
                {
                    ticksToSpawnGoodMinions--;

                    if (ticksToSpawnGoodMinions == 0)
                    {
                        if (Roguelike.HardMode)
                        {
                            SpawnGoodMinions(7);
                            SpawnBadMinions(10);
                        }
                        else
                            SpawnGoodMinions(3);
                        speed = 7;
                    }
                }
            }
        }

        private void StartExplosion()
        {
            if (attackPhase == 0)
            {
                ticksToExplode = 120;
                explosionDamage = 25;
                explosionRadius = 3;
            }
            else if (attackPhase == 1)
            {
                ticksToExplode = 90;
                explosionDamage = 40;
                explosionRadius = 6;
            }

            Halt();
            startGlowing(Color.Red, true, 0.5f);
            currentLocation.netAudio.StartPlaying("fuse");
        }

        private void SpawnBadMinions(int amount)
        {
            foreach (Vector2 pos in phase2_SpawnRect.GetRandomPositions(amount))
            {
                ShadowKingMinion minion = new(pos, Difficulty);
                currentLocation.characters.Add(minion);
            }
        }

        private void SpawnGoodMinions(int amount)
        {
            foreach (Vector2 pos in phase2_SpawnRect.GetRandomPositions(amount))
            {
                ShadowKingMinion minion = new(pos, Difficulty, true);
                currentLocation.characters.Add(minion);
            }
        }

        private void FireArrowCone()
        {
            if (Player is null)
                return;

            faceGeneralDirection(Player.Position, 0, false);

            Vector2 shot_origin = new(GetBoundingBox().X, GetBoundingBox().Y);
            float fire_angle = 0f;
            if (Roguelike.HardMode)
                fire_angle = RoguelikeUtility.VectorToDegrees(Player.Position - Position) - 90;
            else
            {
                switch (facingDirection.Value)
                {
                    case 0:
                        shot_origin.Y -= 64f;
                        fire_angle = 90f;
                        break;
                    case 1:
                        shot_origin.X += 64f;
                        fire_angle = 0f;
                        break;
                    case 2:
                        fire_angle = 270f;
                        break;
                    case 3:
                        shot_origin.X -= 64f;
                        fire_angle = 180f;
                        break;
                }
            }

            currentLocation.playSound("Cowboy_gunshot");
            for (int angleOffset = -45; angleOffset <= 45; angleOffset += 18)
            {
                Vector2 shot_velocity = new((float)Math.Cos(RoguelikeUtility.DegreesToRadians(fire_angle + angleOffset)), -(float)Math.Sin(RoguelikeUtility.DegreesToRadians(fire_angle + angleOffset)));
                shot_velocity *= 10f;
                BasicProjectile projectile = new((int)Math.Round(35 * Difficulty), 12, 0, 1, 0f, shot_velocity.X, shot_velocity.Y, shot_origin, "", "", false, false, currentLocation, this, false, null);
                projectile.startingRotation.Value = RoguelikeUtility.VectorToRadians(shot_velocity) + (float)RoguelikeUtility.DegreesToRadians(90);
                projectile.ignoreTravelGracePeriod.Value = true;
                projectile.ignoreMeleeAttacks.Value = true;
                currentLocation.projectiles.Add(projectile);
            }
        }

        private void FireJinxCircle()
        {
            if (Player is not null)
                faceGeneralDirection(Player.Position, 0, false);

            float fire_angle = 0f;
            for (int angleOffset = 0; angleOffset <= 360; angleOffset += 45)
            {
                Vector2 shot_velocity = new((float)Math.Cos(RoguelikeUtility.DegreesToRadians(fire_angle + angleOffset)), -(float)Math.Sin(RoguelikeUtility.DegreesToRadians(fire_angle + angleOffset)));
                shot_velocity *= 10f;
                DebuffingProjectile projectile = new(14, 7, 1, 4, (float)Math.PI / 16f, shot_velocity.X, shot_velocity.Y, new Vector2(GetBoundingBox().X, GetBoundingBox().Y), currentLocation, this);
                projectile.maxTravelDistance.Value = 64 * 5;
                projectile.ignoreTravelGracePeriod.Value = true;
                currentLocation.projectiles.Add(projectile);
            }
        }

        private bool ShouldBeStill()
        {
            return (
                ticksToExplode > 0
            );
        }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (ShouldBeStill())
                return;

            base.MovePosition(time, viewport, currentLocation);
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth = (int)(Sprite.SpriteWidth * 4 * 5 / 6 * Scale);
            int boxHeight = (int)(Sprite.SpriteHeight * Scale);
            return new Rectangle((int)Position.X - boxWidth / 4, (int)Position.Y + boxHeight / 2, boxWidth, boxHeight);
        }

        public void EnableMapChanges()
        {
            encountered.Value = true;
            currentLocation.playSound("boulderBreak");

            for (int i = 0; i < tilesToBlock.Count; i++)
            {
                int tileId = tilesToBlockIds[i];
                Vector2 tile = tilesToBlock[i];

                currentLocation.setMapTileIndex((int)tile.X, (int)tile.Y, tileId, "Buildings");
            }

            foreach (Vector2 warpTile in tilesToWarp)
                currentLocation.warps.Add(new((int)warpTile.X, (int)warpTile.Y, currentLocation.Name, (int)warpDestination.X, (int)warpDestination.Y, flipFarmer: false));

            ((MineShaft)currentLocation).get_MineShaftCustomDestination().Value = warpDestination;

            mapChangesEnabled = true;
        }

        public void DisableMapChanges()
        {
            ((MineShaft)currentLocation).createLadderAt(warpDestination);
            mapChangesEnabled = false;
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            if (isBomb)
                return 0;

            if ((attackPhase == 0 || attackPhase == 1) && ticksSinceLastExplosion > explosionCooldownTicks && ticksToExplode == 0)
                StartExplosion();

            int result = base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
            if (Health <= 0)
            {
                DisableMapChanges();
                BossManager.Death(who.currentLocation, who, DisplayName, SpawnLocation);
            }

            return result;
        }

        protected override void sharedDeathAnimation()
        {
            Game1.createRadialDebris(currentLocation, TextureName, new Rectangle(Sprite.SourceRect.X, Sprite.SourceRect.Y, 16, 5), 16, getStandingX(), getStandingY() - 32, 1, getStandingY() / 64, Color.White);
            Game1.createRadialDebris(currentLocation, TextureName, new Rectangle(Sprite.SourceRect.X + 2, Sprite.SourceRect.Y + 5, 16, 5), 10, getStandingX(), getStandingY() - 32, 1, getStandingY() / 64, Color.White);
            Game1.createRadialDebris(currentLocation, TextureName, new Rectangle(0, 10, 16, 5), 16, getStandingX(), getStandingY() - 32, 1, getStandingY() / 64, Color.White);
        }

        protected override void localDeathAnimation()
        {
            Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, base.Position, Color.White, 10), base.currentLocation);
            for (int i = 1; i < 3; i++)
            {
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(1f, 1f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(1f, -1f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(-1f, 1f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
                base.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, base.Position + new Vector2(-1f, -1f) * 64f * i, Color.Gray * 0.75f, 10)
                {
                    delayBeforeAnimationStart = i * 159
                });
            }
        }

        protected override void updateMonsterSlaveAnimation(GameTime time)
        {
            if (this.isMoving())
            {
                if (this.FacingDirection == 0)
                {
                    this.Sprite.AnimateUp(time);
                }
                else if (this.FacingDirection == 3)
                {
                    this.Sprite.AnimateLeft(time);
                }
                else if (this.FacingDirection == 1)
                {
                    this.Sprite.AnimateRight(time);
                }
                else if (this.FacingDirection == 2)
                {
                    this.Sprite.AnimateDown(time);
                }
            }
            else
            {
                this.Sprite.StopAnimation();
            }
        }
    }
}

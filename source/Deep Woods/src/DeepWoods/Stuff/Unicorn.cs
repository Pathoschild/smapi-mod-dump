/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using DeepWoodsMod.API.Impl;

namespace DeepWoodsMod
{
    public class Unicorn : Horse
    {
        private NetBool isScared = new NetBool(false);
        private NetBool isPetted = new NetBool(false);
        private NetInt fleeFacingDirection = new NetInt(0);

        public Unicorn()
            : base()
        {
        }

        public Unicorn(Vector2 location)
            : base(Guid.NewGuid(), (int)location.X, (int)location.Y)
        {
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddFields(this.isScared, this.isPetted, this.fleeFacingDirection);
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (!isScared && !isPetted)
            {
                isPetted.Value = true;
                this.farmerPassesThrough = true;
                who.health = who.maxHealth;
                who.Stamina = who.MaxStamina;
                who.addedLuckLevel.Value = Math.Max(10, who.addedLuckLevel.Value);
                if (!DeepWoodsState.PlayersWhoGotStardropFromUnicorn.Contains(who.UniqueMultiplayerID))
                {
                    who.addItemByMenuIfNecessaryElseHoldUp(new StardewValley.Object(434, 1), null);
                    DeepWoodsState.PlayersWhoGotStardropFromUnicorn.Add(who.UniqueMultiplayerID);
                    if (!Game1.IsMasterGame)
                    {
                        ModEntry.SendMessage(MessageId.SetUnicornStardropReceived, Game1.MasterPlayer.UniqueMultiplayerID);
                    }
                }
                else
                {
                    l.playSoundAt(Sounds.STARDROP, this.getTileLocation());
                }
                l.playSoundAt(Sounds.ACHIEVEMENT, this.getTileLocation());
                l.playSoundAt(Sounds.HEAL_SOUND, this.getTileLocation());
                l.playSoundAt(Sounds.REWARD, this.getTileLocation());
                l.playSoundAt(Sounds.SECRET1, this.getTileLocation());
                l.playSoundAt(Sounds.SHINY4, this.getTileLocation());
                l.playSoundAt(Sounds.YOBA, this.getTileLocation());
                l.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, this.Position, false, false));
            }
            return true;
        }

        private void CheckScared()
        {
            if (isScared || isPetted)
                return;

            foreach (Farmer farmer in this.currentLocation.farmers)
            {
                if ((farmer.Position - this.Position).Length() < (Settings.Objects.Unicorn.FarmerScareDistance * 64))
                {
                    if (farmer.running || farmer.getMovementSpeed() >= Settings.Objects.Unicorn.FarmerScareSpeed)
                    {
                        isScared.Value = true;
                        this.farmerPassesThrough = true;
                        Game1.player.team.sharedDailyLuck.Value = -0.12;
                        farmer.addedLuckLevel.Value = Math.Min(-10, farmer.addedLuckLevel.Value);
                        this.currentLocation.playSoundAt(Sounds.THUNDER_SMALL, this.getTileLocation());
                        this.currentLocation.playSoundAt(Sounds.GHOST, this.getTileLocation());
                        Game1.isRaining = true;
                        Game1.isLightning = true;
                        Game1.changeMusicTrack("rain");
                        return;
                    }
                }
            }
        }

        public override bool canPassThroughActionTiles()
        {
            return true;
        }

        public override void collisionWithFarmerBehavior()
        {
            base.collisionWithFarmerBehavior();
        }

        private Vector2 GetFleeFromPos()
        {
            int farmerCount = 0;
            Vector2 fleeFromPos = new Vector2();
            foreach (Farmer farmer in this.currentLocation.farmers)
            {
                fleeFromPos += farmer.Position;
                farmerCount++;
            }
            return farmerCount > 0 ? fleeFromPos /= farmerCount : fleeFromPos;
        }

        private void Flee(GameTime time)
        {
            // Calculate flee direction
            Vector2 fleeFromPos = GetFleeFromPos();
            Vector2 fleeDirection = this.Position - fleeFromPos;
            if (fleeDirection.Length() <= float.Epsilon)
                fleeDirection = new Vector2((float)Game1.random.NextDouble() - 0.5f, (float)Game1.random.NextDouble() - 0.5f);
            fleeDirection.Normalize();

            // Calculate facing direction
            if (Math.Abs(fleeDirection.X) > Math.Abs(fleeDirection.Y))
            {
                this.fleeFacingDirection.Value = fleeDirection.X < 0 ? 3 : 1;
            }
            else
            {
                this.fleeFacingDirection.Value = fleeDirection.Y < 0 ? 0 : 2;
            }

            // Flee
            this.position.X += fleeDirection.X * Settings.Objects.Unicorn.FleeSpeed;
            this.position.Y += fleeDirection.Y * Settings.Objects.Unicorn.FleeSpeed;
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (isPetted)
            {
                this.currentLocation.characters.Remove(this);
                return;
            }
            if (!isScared)
            {
                CheckScared();
            }
            if (isScared)
            {
                if (this.currentLocation.farmers.Count == 0)
                {
                    this.currentLocation.characters.Remove(this);
                    return;
                }

                this.farmerPassesThrough = true;

                Flee(time);

                if (this.FacingDirection != this.fleeFacingDirection.Value)
                {
                    this.Sprite.StopAnimation();
                    this.faceDirection(this.fleeFacingDirection.Value);
                }

                if (this.Sprite.CurrentAnimation == null)
                {
                    AnimatedSprite.endOfAnimationBehavior frameBehavior = (AnimatedSprite.endOfAnimationBehavior)(x =>
                    {
                        string stepAudio = "thudStep";

                        string str = this.currentLocation.doesTileHaveProperty((int)this.getTileLocation().X, (int)this.getTileLocation().Y, "Type", "Back");
                        if (str == "Wood")
                            stepAudio = "woodyStep";
                        else if (str == "Stone")
                            stepAudio = "stoneStep";

                        this.currentLocation.localSoundAt(stepAudio, this.getTileLocation());
                    });

                    if (this.FacingDirection == 1)
                        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(8, 70),
                            new FarmerSprite.AnimationFrame(9, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(10, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(11, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(12, 70),
                            new FarmerSprite.AnimationFrame(13, 70)
                        });
                    else if (this.FacingDirection == 3)
                        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(8, 70, false, true, null, false),
                            new FarmerSprite.AnimationFrame(9, 70, false, true, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(10, 70, false, true, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(11, 70, false, true, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(12, 70, false, true, null, false),
                            new FarmerSprite.AnimationFrame(13, 70, false, true, null, false)
                        });
                    else if (this.FacingDirection == 0)
                        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(15, 70),
                            new FarmerSprite.AnimationFrame(16, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(17, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(18, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(19, 70),
                            new FarmerSprite.AnimationFrame(20, 70)
                        });
                    else if (this.FacingDirection == 2)
                        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(1, 70),
                            new FarmerSprite.AnimationFrame(2, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(3, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(4, 70, false, false, frameBehavior, false),
                            new FarmerSprite.AnimationFrame(5, 70),
                            new FarmerSprite.AnimationFrame(6, 70)
                        });
                }

                if (this.FacingDirection == 3)
                    this.drawOffset.Set(Vector2.Zero);
                else
                    this.drawOffset.Set(new Vector2(-16f, 0.0f));

                this.flip = this.FacingDirection == 3;

                if (!this.currentLocation.isTileOnMap(this.getTileLocation()))
                {
                    this.currentLocation.characters.Remove(this);
                    this.currentLocation.playSoundAt(Sounds.LEAFRUSTLE, this.getTileLocation());
                    return;
                }

                BaseUpdate(time, location);
            }
            else
            {
                base.update(time, location);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (isPetted)
                return;

            this.Sprite.spriteTexture = DeepWoodsTextures.Textures.Unicorn;
            base.draw(b);
        }

        private Action<GameTime, GameLocation> BaseUpdate
        {
            get
            {
                var baseUpdatePtr = typeof(NPC).GetMethod("update", new Type[] { typeof(GameTime), typeof(GameLocation) }).MethodHandle.GetFunctionPointer();
                return (Action<GameTime, GameLocation>)Activator.CreateInstance(typeof(Action<GameTime, GameLocation>), this, baseUpdatePtr);
            }
        }

    }
}

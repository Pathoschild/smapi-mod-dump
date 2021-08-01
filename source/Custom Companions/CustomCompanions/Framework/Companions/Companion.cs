/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using CustomCompanions.Framework.Managers;
using CustomCompanions.Framework.Models.Companion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCompanions.Framework.Companions
{
    public class Companion : NPC
    {
        internal NetString companionKey = new NetString();
        internal NetVector2 targetTile = new NetVector2();
        internal NetLong ownerId = new NetLong();
        internal Farmer owner;
        internal CompanionModel model;
        internal IdleBehavior idleBehavior;

        private float lightPulseTimer;

        private int? soundIdleTimer = null;
        private int? soundMovingTimer = null;
        private int? soundAlwaysTimer = null;

        private SoundModel idleSound;
        private SoundModel movingSound;
        private SoundModel alwaysSound;

        internal LightSource light;
        internal bool wasIdle;

        internal List<FarmerSprite.AnimationFrame> idleUniformFrames;
        internal List<FarmerSprite.AnimationFrame> idleUpFrames;
        internal List<FarmerSprite.AnimationFrame> idleRightFrames;
        internal List<FarmerSprite.AnimationFrame> idleDownFrames;
        internal List<FarmerSprite.AnimationFrame> idleLeftFrames;

        internal List<FarmerSprite.AnimationFrame> activeUniformFrames;
        internal List<FarmerSprite.AnimationFrame> activeUpFrames;
        internal List<FarmerSprite.AnimationFrame> activeRightFrames;
        internal List<FarmerSprite.AnimationFrame> activeDownFrames;
        internal List<FarmerSprite.AnimationFrame> activeLeftFrames;

        internal readonly NetBool hasShadow = new NetBool();
        internal readonly NetBool hasReachedPlayer = new NetBool();
        internal readonly NetInt specialNumber = new NetInt();
        internal readonly NetBool isPrismatic = new NetBool();
        internal readonly NetInt previousDirection = new NetInt();
        internal readonly NetBool isIdle = new NetBool();
        internal readonly NetColor color = new NetColor();
        internal readonly NetVector2 motion = new NetVector2(Vector2.Zero);
        internal new readonly NetRectangle nextPosition = new NetRectangle();

        public Companion()
        {

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddFields(this.companionKey, this.ownerId, this.targetTile, this.hasShadow, this.hasReachedPlayer, this.specialNumber, this.isPrismatic, this.previousDirection, this.isIdle, this.color, this.motion, this.nextPosition);

            if (this.model != null)
            {
                this.companionKey.Value = this.model.GetId();
            }
        }

        public Companion(CompanionModel model, Farmer owner, Vector2? targetTile = null) : base(new AnimatedSprite(model.TileSheetPath, 0, model.FrameSizeWidth, model.FrameSizeHeight), (owner is null ? (Vector2)targetTile : owner.getTileLocation()) * 64f + new Vector2(model.SpawnOffsetX, model.SpawnOffsetY), model.SpawnDirection == -1 ? Game1.random.Next(4) : model.SpawnDirection, model.Name)
        {
            base.HideShadow = true; // Always hiding the default shadow, as we are allowing user to config beyond normal settings
            base.Sprite.loop = false;
            base.Scale = model.Scale;
            base.collidesWithOtherCharacters.Value = (model.Type.ToUpper() == "FLYING" ? false : true);
            base.Breather = model.EnableBreathing;
            base.displayName = null;

            this.model = model;
            this.companionKey.Value = model.GetId();

            this.hasShadow.Value = model.EnableShadow;
            this.specialNumber.Value = Game1.random.Next(100);
            this.previousDirection.Value = this.FacingDirection;

            // Pick a random color (Color.White if none given) or use prismatic if IsPrismatic is true
            color.Value = Color.White;
            if (model.Colors.Count > 0 || model.IsPrismatic)
            {
                if (model.Colors.Count == 0 && model.IsPrismatic)
                {
                    this.isPrismatic.Value = true;
                }
                else if (model.Colors.Count > 0)
                {
                    int randomColorIndex = Game1.random.Next(model.Colors.Count + (model.IsPrismatic ? 1 : 0));
                    if (randomColorIndex > model.Colors.Count - 1)
                    {
                        // Primsatic color has been selected
                        this.isPrismatic.Value = true;
                    }
                    else
                    {
                        this.color.Value = CustomCompanions.GetColorFromArray(model.Colors[randomColorIndex]);
                    }
                }
            }

            if (owner != null)
            {
                this.owner = owner;
                this.ownerId = owner.uniqueMultiplayerID;
                this.currentLocation = owner.currentLocation;

                // Verify the location the companion is spawning on isn't occupied (if collidesWithOtherCharacters == true)
                if (this.collidesWithOtherCharacters)
                {
                    this.PlaceInEmptyTile();
                }
                this.nextPosition.Value = this.GetBoundingBox();
            }

            this.SetUpCompanion();
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (!Game1.shouldTimePass())
            {
                return;
            }

            if (this.model is null)
            {
                this.owner = Game1.getAllFarmers().FirstOrDefault(f => f.uniqueMultiplayerID == this.ownerId);
                this.model = CompanionManager.companionModels.First(c => c.GetId() == this.companionKey.Value);
                this.SetUpCompanion();
                this.UpdateModel(this.model);
            }

            base.currentLocation = location;
            base.update(time, location);
            base.forceUpdateTimer = 99999;

            if (this.owner == Game1.player)
            {
                // Handle any movement
                this.AttemptMovement(time, location);

                // Update light location, if applicable
                this.UpdateLight(time);

                // Play any sound(s) that are required
                if (Utility.isThereAFarmerWithinDistance(base.getTileLocation(), 10, base.currentLocation) != null)
                {
                    this.PlayRequiredSounds(time, this.isMoving());
                }
            }
            else
            {
                if ((this.yJumpOffset == 0 && this.IsHovering() && this.isIdle) || this.IsJumper())
                {
                    this.idleBehavior.PerformIdleBehavior(this, time, this.model.IdleArguments);
                }
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }

        protected override void updateSlaveAnimation(GameTime time)
        {
            // Do nothing
        }

        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
        {
            // Do nothing
        }

        public override bool CanSocialize
        {
            get
            {
                return false;
            }
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (this.owner is null && !String.IsNullOrEmpty(this.model.InspectionDialogue))
            {
                string dialogueText = this.model.InspectionDialogue;
                if (this.model.Translations.GetTranslations().Any(t => t.Key == dialogueText))
                {
                    dialogueText = this.model.Translations.Get(this.model.InspectionDialogue);
                }

                // Check if displaying a portrait is required
                if (this.Portrait != null)
                {
                    this.CurrentDialogue.Push(new Dialogue(dialogueText, this));
                    Game1.drawDialogue(this);
                }
                else
                {
                    Game1.drawObjectDialogue(dialogueText);
                }

                return true;
            }
            return false;
        }

        public override bool isMoving()
        {
            return !this.motion.Equals(Vector2.Zero);
        }


        public override bool shouldCollideWithBuildingLayer(GameLocation location)
        {
            if (IsFlying())
            {
                return false;
            }

            return true;
        }

        public override bool collideWith(StardewValley.Object o)
        {
            if (IsFlying())
            {
                return false;
            }

            return base.collideWith(o);
        }

        public override bool isColliding(GameLocation l, Vector2 tile)
        {
            if (IsFlying())
            {
                return false;
            }

            return base.isColliding(l, tile);
        }

        public override void collisionWithFarmerBehavior()
        {
            base.farmerPassesThrough = this.owner is null && this.model.EnableFarmerCollision ? false : true;
        }

        public override Rectangle GetBoundingBox()
        {
            if (!this.HasCustomCollisionBox())
            {
                return base.GetBoundingBox();
            }

            if (this.Sprite == null)
            {
                return Rectangle.Empty;
            }

            Vector2 position = this.Position;
            return new Rectangle((int)position.X + this.model.CollisionPositionX, (int)position.Y + this.model.CollisionPositionY, this.model.CollisionPositionWidth, this.model.CollisionPositionHeight);
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (this.model is null || this.model.AppearUnderwater)
            {
                return;
            }

            this.DoDraw(b, alpha);
        }

        internal void DoDraw(SpriteBatch b, float alpha = 1f)
        {
            var spriteLayerDepth = this.IsFlying() ? 0.991f : Math.Max(0f, base.drawOnTop ? 0.991f : ((float)base.getStandingY() / 10000f));
            float layer_depth = ((float)(this.GetBoundingBox().Center.Y + 4) + base.Position.X / 20000f) / 10000f;

            b.Draw(this.Sprite.Texture, base.getLocalPosition(Game1.viewport) + new Vector2(this.GetSpriteWidthForPositioning() * 4 / 2, this.Sprite.getHeight() / 2) + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), this.Sprite.SourceRect, this.isPrismatic ? Utility.GetPrismaticColor(348 + (int)this.specialNumber, 5f) : color, this.rotation, new Vector2(this.Sprite.SpriteWidth / 2, (float)this.Sprite.SpriteHeight * 3f / 4f), Math.Max(0.2f, base.scale) * 4f, (base.flip || (this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer_depth);
            if (this.Breather && this.shakeTimer <= 0 && !this.isMoving())
            {
                Rectangle chestBox = this.Sprite.SourceRect;
                chestBox.Y += this.Sprite.SpriteHeight / 2;
                chestBox.Height = this.Sprite.SpriteHeight / 4;
                chestBox.X += this.Sprite.SpriteWidth / 4;
                chestBox.Width = this.Sprite.SpriteWidth / 2;
                Vector2 chestPosition = new Vector2(this.Sprite.SpriteWidth * 4 / 2, 8f);
                float breathScale = Math.Max(0f, (float)Math.Ceiling(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 600.0 + (double)(base.DefaultPosition.X * 20f))) / 4f);
                b.Draw(this.Sprite.Texture, base.getLocalPosition(Game1.viewport) + chestPosition + ((this.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), chestBox, this.isPrismatic ? Utility.GetPrismaticColor(348 + (int)this.specialNumber, 5f) : color.Value * alpha, this.rotation, new Vector2(chestBox.Width / 2, chestBox.Height / 2 + 1), Math.Max(0.2f, base.scale) * 4f + breathScale, base.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, spriteLayerDepth + 0.001f);
            }

            var shadowLayerDepth = spriteLayerDepth - 0.001f;
            if (this.hasShadow)
            {
                if (this.model.Shadow != null)
                {
                    Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(this.model.Shadow.OffsetX, this.model.Shadow.OffsetY) + this.Position + new Vector2((float)(this.GetSpriteWidthForPositioning() * 4) / 2f, this.Sprite.getHeight())), Game1.shadowTexture.Bounds, new Color(255, 255, 255, this.model.Shadow.Alpha), 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), Math.Max(0f, (4f + (float)this.yJumpOffset / 40f) * this.model.Shadow.Scale), SpriteEffects.None, shadowLayerDepth);
                }
                else
                {
                    // Default game shadow
                    Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.GetShadowOffset() + this.Position + new Vector2((float)(this.GetSpriteWidthForPositioning() * 4) / 2f, this.Sprite.getHeight())), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), Math.Max(0f, (4f + (float)this.yJumpOffset / 40f) * (float)this.scale), SpriteEffects.None, shadowLayerDepth);
                }
            }
        }

        internal void DrawUnderwater(SpriteBatch b)
        {
            this.DoDraw(b, 1f);
        }

        internal bool HasCustomCollisionBox()
        {
            if (this.model is null)
            {
                return false;
            }

            if (this.model.CollisionPositionHeight == 0 && this.model.CollisionPositionWidth == 0 && this.model.CollisionPositionX == 0 && this.model.CollisionPositionY == 0)
            {
                return false;
            }

            return true;
        }

        internal void SetUpCompanion()
        {
            // Standard configuration
            base.speed = this.model.TravelSpeed;
            base.forceUpdateTimer = 9999;
            base.farmerPassesThrough = true;

            this.idleBehavior = new IdleBehavior(this, model.IdleBehavior);
            this.SetMovingDirection(this.FacingDirection);

            // Set up the sounds to play, if any
            this.idleSound = this.model.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "IDLE");
            if (this.idleSound != null && CustomCompanions.IsSoundValid(this.idleSound.SoundName, true))
            {
                this.soundIdleTimer = this.idleSound.TimeBetweenSound;
            }

            this.movingSound = this.model.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "MOVING");
            if (this.movingSound != null && CustomCompanions.IsSoundValid(this.movingSound.SoundName, true))
            {
                this.soundMovingTimer = this.movingSound.TimeBetweenSound;
            }

            this.alwaysSound = this.model.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "ALWAYS");
            if (this.alwaysSound != null && CustomCompanions.IsSoundValid(this.alwaysSound.SoundName, true))
            {
                this.soundAlwaysTimer = this.alwaysSound.TimeBetweenSound;
            }

            // Set up the light to give off, if any
            if (model.Light != null)
            {
                this.lightPulseTimer = model.Light.PulseSpeed;

                this.light = new LightSource(1, new Vector2(this.position.X + model.Light.OffsetX, this.position.Y + model.Light.OffsetY), model.Light.Radius, CustomCompanions.GetColorFromArray(model.Light.Color), this.id, LightSource.LightContext.None, 0L);
                Game1.currentLightSources.Add(this.light);
            }

            // Set up uniform frames
            if (this.model.UniformAnimation != null && !CustomCompanions.CompanionHasFullMovementSet(model))
            {
                this.idleUniformFrames = (this.model.UniformAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(this.model.UniformAnimation.IdleAnimation.ManualFrames) : GetManualFrames(model.UniformAnimation.IdleAnimation.StartingFrame, model.UniformAnimation.IdleAnimation.NumberOfFrames, model.UniformAnimation.IdleAnimation.Duration);
                this.activeUniformFrames = (this.model.UniformAnimation.ManualFrames != null) ? GetManualFrames(this.model.UniformAnimation.ManualFrames) : GetManualFrames(model.UniformAnimation.StartingFrame, model.UniformAnimation.NumberOfFrames, model.UniformAnimation.Duration);
            }
            else
            {
                // Up frames
                this.idleUpFrames = (this.model.UpAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(this.model.UpAnimation.IdleAnimation.ManualFrames) : GetManualFrames(model.UpAnimation.IdleAnimation.StartingFrame, model.UpAnimation.IdleAnimation.NumberOfFrames, model.UpAnimation.IdleAnimation.Duration);
                this.activeUpFrames = (this.model.UpAnimation.ManualFrames != null) ? GetManualFrames(this.model.UpAnimation.ManualFrames) : GetManualFrames(model.UpAnimation.StartingFrame, model.UpAnimation.NumberOfFrames, model.UpAnimation.Duration);

                // Right frames
                this.idleRightFrames = (this.model.RightAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(this.model.RightAnimation.IdleAnimation.ManualFrames) : GetManualFrames(model.RightAnimation.IdleAnimation.StartingFrame, model.RightAnimation.IdleAnimation.NumberOfFrames, model.RightAnimation.IdleAnimation.Duration);
                this.activeRightFrames = (this.model.RightAnimation.ManualFrames != null) ? GetManualFrames(this.model.RightAnimation.ManualFrames) : GetManualFrames(model.RightAnimation.StartingFrame, model.RightAnimation.NumberOfFrames, model.RightAnimation.Duration);

                // Down frames
                this.idleDownFrames = (this.model.DownAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(this.model.DownAnimation.IdleAnimation.ManualFrames) : GetManualFrames(model.DownAnimation.IdleAnimation.StartingFrame, model.DownAnimation.IdleAnimation.NumberOfFrames, model.DownAnimation.IdleAnimation.Duration);
                this.activeDownFrames = (this.model.DownAnimation.ManualFrames != null) ? GetManualFrames(this.model.DownAnimation.ManualFrames) : GetManualFrames(model.DownAnimation.StartingFrame, model.DownAnimation.NumberOfFrames, model.DownAnimation.Duration);

                // Left frames
                this.idleLeftFrames = (this.model.LeftAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(this.model.LeftAnimation.IdleAnimation.ManualFrames) : GetManualFrames(model.LeftAnimation.IdleAnimation.StartingFrame, model.LeftAnimation.IdleAnimation.NumberOfFrames, model.LeftAnimation.IdleAnimation.Duration);
                this.activeLeftFrames = (this.model.LeftAnimation.ManualFrames != null) ? GetManualFrames(this.model.LeftAnimation.ManualFrames) : GetManualFrames(model.LeftAnimation.StartingFrame, model.LeftAnimation.NumberOfFrames, model.LeftAnimation.Duration);
            }
        }

        internal bool HasIdleFrames(int direction = -1)
        {

            switch (direction)
            {
                case -1:
                    return !(this.model.UniformAnimation is null || (this.model.UniformAnimation.IdleAnimation.StartingFrame == -1 && this.model.UniformAnimation.IdleAnimation.ManualFrames is null));
                case 0:
                    return !(this.model.UpAnimation is null || (this.model.UpAnimation.IdleAnimation.StartingFrame == -1 && this.model.UpAnimation.IdleAnimation.ManualFrames is null));
                case 1:
                    return !(this.model.RightAnimation is null || (this.model.RightAnimation.IdleAnimation.StartingFrame == -1 && this.model.RightAnimation.IdleAnimation.ManualFrames is null));
                case 2:
                    return !(this.model.DownAnimation is null || (this.model.DownAnimation.IdleAnimation.StartingFrame == -1 && this.model.DownAnimation.IdleAnimation.ManualFrames is null));
                case 3:
                    return !(this.model.LeftAnimation is null || (this.model.LeftAnimation.IdleAnimation.StartingFrame == -1 && this.model.LeftAnimation.IdleAnimation.ManualFrames is null));
                default:
                    return false;
            }
        }

        private void AttemptMovement(GameTime time, GameLocation location)
        {
            // Heed thy warnin', there be spaghetti code ahead
            if (owner != null || targetTile != null)
            {
                this.lastPosition = this.position;

                var targetDistance = Vector2.Distance(base.Position, this.GetTargetPosition());
                if (targetDistance > this.model.MaxDistanceBeforeTeleport && this.model.MaxDistanceBeforeTeleport != -1)
                {
                    this.hasReachedPlayer.Value = false;
                    base.position.Value = this.GetTargetPosition();
                }
                else if ((targetDistance >= 64f && ((owner != null && owner.isMoving()) || !this.hasReachedPlayer.Value)) || (targetDistance >= this.model.MaxIdleDistance && this.model.MaxIdleDistance != -1))
                {
                    this.hasReachedPlayer.Value = false;
                    this.motion.Value = Vector2.Zero;
                    this.SetMovingDirection(-1);

                    base.Speed = model.TravelSpeed;
                    if (targetDistance > this.model.MaxIdleDistance)
                    {
                        base.Speed = model.TravelSpeed + (int)(targetDistance / 64f) - 1;
                    }

                    if (IsJumper())
                    {
                        var gravity = -0.5f;
                        var jumpScale = 10f;
                        var randomJumpBoostMultiplier = 2f;
                        if (this.model.IdleArguments != null)
                        {
                            if (this.model.IdleArguments.Length > 0)
                            {
                                gravity = this.model.IdleArguments[0];
                            }
                            if (this.model.IdleArguments.Length > 1)
                            {
                                jumpScale = this.model.IdleArguments[1];
                            }
                            if (this.model.IdleArguments.Length > 2)
                            {
                                randomJumpBoostMultiplier = this.model.IdleArguments[2];
                            }
                        }
                        if (this.yJumpOffset == 0)
                        {
                            this.jumpWithoutSound();
                            this.yJumpGravity = Math.Abs(gravity) * -1;
                            this.yJumpVelocity = (float)Game1.random.Next(50, 70) / jumpScale;

                            if (Game1.random.NextDouble() < 0.01)
                            {
                                this.yJumpVelocity *= randomJumpBoostMultiplier;
                            }
                        }
                    }
                    this.SetMotion(Utility.getVelocityTowardPoint(base.Position, this.GetTargetPosition(), base.speed));
                }
                else if (this.owner is null || (!this.hasReachedPlayer.Value && !this.owner.isMoving()))
                {
                    this.motion.Value = this.owner is null && IsFlying() ? this.motion.Value : Vector2.Zero;
                    this.hasReachedPlayer.Value = true;
                }
            }

            // Perform the position movement
            if (!this.hasReachedPlayer.Value || this.idleBehavior.PerformIdleBehavior(this, time, this.model.IdleArguments))
            {
                this.nextPosition.Value = this.GetBoundingBox();
                this.nextPosition.X += (int)this.motion.X;
                if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, this) || IsFlying())
                {
                    base.position.X += (int)this.motion.X;
                }
                this.nextPosition.X -= (int)this.motion.X;
                this.nextPosition.Y += (int)this.motion.Y;
                if (!location.isCollidingPosition(this.nextPosition, Game1.viewport, this) || IsFlying())
                {
                    base.position.Y += (int)this.motion.Y;
                }
            }

            // Check if the collision based companion is stuck, if so set it to idle animations
            if (this.owner != null && !this.owner.isMoving() && this.lastPosition.Equals(this.position) && !this.IsFlying())
            {
                this.hasReachedPlayer.Value = true;
            }

            // Update any animations
            if ((!this.lastPosition.Equals(this.position) && !this.IsHovering()) || (this.owner != null && this.owner.isMoving()) || (this.owner is null && (!this.hasReachedPlayer.Value || !this.motion.Equals(Vector2.Zero))))
            {
                this.previousDirection.Value = this.FacingDirection;

                if (model.UniformAnimation != null && !CustomCompanions.CompanionHasFullMovementSet(model))
                {
                    this.FacingDirection = 2;
                }
                else if (CustomCompanions.CompanionHasFullMovementSet(model))
                {
                    if (base.moveUp || Math.Abs(this.motion.Y) > Math.Abs(this.motion.X) && this.motion.Y < 0f)
                    {
                        this.FacingDirection = 0;
                    }
                    else if (base.moveRight || Math.Abs(this.motion.X) > Math.Abs(this.motion.Y) && this.motion.X > 0f)
                    {
                        this.FacingDirection = 1;
                    }
                    else if (base.moveDown || Math.Abs(this.motion.Y) > Math.Abs(this.motion.X) && this.motion.Y > 0f)
                    {
                        this.FacingDirection = 2;
                    }
                    else if (base.moveLeft || Math.Abs(this.motion.X) > Math.Abs(this.motion.Y) && this.motion.X < 0f)
                    {
                        this.FacingDirection = 3;
                    }
                }

                this.isIdle.Value = false;
                this.Animate(time, this.isIdle);
                this.wasIdle = false;
            }
            else
            {
                this.isIdle.Value = true;
                this.Animate(time, this.isIdle);
                this.wasIdle = true;
            }
        }

        internal void Animate(GameTime time, bool isIdle = false)
        {
            bool hasIdleFrames = HasIdleFrames(this.idleUniformFrames != null ? -1 : this.FacingDirection);

            if (this.Sprite.CurrentAnimation != null && (!hasIdleFrames || (hasIdleFrames && this.wasIdle == isIdle)) && (this.previousDirection == this.FacingDirection || this.activeUniformFrames != null))
            {
                if (!this.Sprite.animateOnce(time))
                {
                    return;
                }
            }

            if (isIdle && hasIdleFrames)
            {
                if (this.idleUniformFrames != null)
                {
                    this.Sprite.setCurrentAnimation(this.idleUniformFrames);
                }
                else
                {
                    switch (this.FacingDirection)
                    {
                        case 0:
                            this.Sprite.setCurrentAnimation(this.idleUpFrames);
                            break;
                        case 1:
                            this.Sprite.setCurrentAnimation(this.idleRightFrames);
                            break;
                        case 2:
                            this.Sprite.setCurrentAnimation(this.idleDownFrames);
                            break;
                        case 3:
                            this.Sprite.setCurrentAnimation(this.idleLeftFrames);
                            break;
                    }
                }
            }
            else
            {
                if (this.activeUniformFrames != null)
                {
                    this.Sprite.setCurrentAnimation(this.activeUniformFrames);
                }
                else
                {
                    switch (this.FacingDirection)
                    {
                        case 0:
                            this.Sprite.setCurrentAnimation(this.activeUpFrames);
                            break;
                        case 1:
                            this.Sprite.setCurrentAnimation(this.activeRightFrames);
                            break;
                        case 2:
                            this.Sprite.setCurrentAnimation(this.activeDownFrames);
                            break;
                        case 3:
                            this.Sprite.setCurrentAnimation(this.activeLeftFrames);
                            break;
                    }
                }
            }

            this.Sprite.animateOnce(time);
        }

        private List<FarmerSprite.AnimationFrame> GetManualFrames(List<ManualFrameModel> manualFrames)
        {
            var frames = new List<FarmerSprite.AnimationFrame>();
            foreach (var frame in manualFrames)
            {
                frames.Add(new FarmerSprite.AnimationFrame(frame.Frame, frame.Duration, false, flip: frame.Flip));
            }

            return frames;
        }

        private List<FarmerSprite.AnimationFrame> GetManualFrames(int startingFrame, int numberOfFrames, int duration)
        {
            var frames = new List<FarmerSprite.AnimationFrame>();
            for (int x = 0; x < numberOfFrames; x++)
            {
                frames.Add(new FarmerSprite.AnimationFrame(startingFrame + x, duration, false, flip: false));
            }

            return frames;
        }

        private int GetPitchRandomness(SoundModel sound)
        {
            if (sound.MinPitchRandomness > sound.MaxPitchRandomness)
            {
                return Game1.random.Next(sound.MinPitchRandomness < 0 ? 0 : sound.MinPitchRandomness);
            }

            return Game1.random.Next(sound.MinPitchRandomness, sound.MaxPitchRandomness);
        }

        internal void UpdateLight(GameTime time)
        {
            if (light != null)
            {
                if (this.model.Light.PulseSpeed != 0)
                {
                    this.light.radius.Value = this.model.Light.PulseMinRadius + (0.5f * (this.model.Light.Radius - this.model.Light.PulseMinRadius) * (1 + (float)Math.Sin(2 * Math.PI * lightPulseTimer)));
                    this.lightPulseTimer = (this.lightPulseTimer + (float)time.ElapsedGameTime.TotalMilliseconds / this.model.Light.PulseSpeed) % 1;
                }

                this.light.position.Value = new Vector2(this.position.X + this.model.Light.OffsetX, this.position.Y + this.model.Light.OffsetY);
            }
        }

        internal void PlayRequiredSounds(GameTime time, bool currentlyMoving)
        {
            if (this.soundAlwaysTimer != null)
            {
                this.soundAlwaysTimer = Math.Max(0, (int)this.soundAlwaysTimer - time.ElapsedGameTime.Milliseconds);
                if (soundAlwaysTimer <= 0)
                {
                    if (Game1.random.NextDouble() <= alwaysSound.ChanceOfPlaying)
                    {
                        this.currentLocation.netAudio.PlayLocal(alwaysSound.SoundName, alwaysSound.Pitch + this.GetPitchRandomness(alwaysSound));
                    }
                    soundAlwaysTimer = alwaysSound.TimeBetweenSound;
                }
            }

            if (currentlyMoving && this.soundMovingTimer != null)
            {
                this.soundMovingTimer = Math.Max(0, (int)this.soundMovingTimer - time.ElapsedGameTime.Milliseconds);
                if (soundMovingTimer <= 0)
                {
                    if (Game1.random.NextDouble() <= movingSound.ChanceOfPlaying)
                    {
                        this.currentLocation.netAudio.PlayLocal(movingSound.SoundName, movingSound.Pitch + this.GetPitchRandomness(movingSound));
                    }
                    soundMovingTimer = movingSound.TimeBetweenSound;
                }
            }

            if (!currentlyMoving && this.soundIdleTimer != null)
            {
                this.soundIdleTimer = Math.Max(0, (int)this.soundIdleTimer - time.ElapsedGameTime.Milliseconds);
                if (soundIdleTimer <= 0)
                {
                    if (Game1.random.NextDouble() <= idleSound.ChanceOfPlaying)
                    {
                        this.currentLocation.netAudio.PlayLocal(idleSound.SoundName, idleSound.Pitch + this.GetPitchRandomness(idleSound));
                    }
                    soundIdleTimer = idleSound.TimeBetweenSound;
                }
            }
        }

        internal bool IsPlayingIdleFrames(int direction = -1)
        {
            if (this.Sprite.CurrentAnimation is null || !this.HasIdleFrames(direction))
            {
                return false;
            }

            switch (direction)
            {
                case -1:
                    return this.Sprite.CurrentAnimation == this.idleUniformFrames;
                case 0:
                    return this.Sprite.CurrentAnimation == this.idleUpFrames;
                case 1:
                    return this.Sprite.CurrentAnimation == this.idleRightFrames;
                case 2:
                    return this.Sprite.CurrentAnimation == this.idleDownFrames;
                case 3:
                    return this.Sprite.CurrentAnimation == this.idleLeftFrames;
            }

            return false;
        }

        private void FlipDirection()
        {
            this.previousDirection.Value = this.FacingDirection;
            this.FacingDirection = Utility.GetOppositeFacingDirection(this.FacingDirection);
        }

        internal void PlaceInEmptyTile(int attempts = 5)
        {
            var currentTile = this.getTileLocation();
            for (int iteration = 0; iteration < attempts; iteration++)
            {
                if (!String.IsNullOrEmpty(this.currentLocation.doesTileHaveProperty((int)currentTile.X, (int)currentTile.Y, "NPCBarrier", "Back")))
                {
                    base.Position = this.GetRandomAdjacentOpenTile(currentTile, this.currentLocation) * 64f;
                }
                else
                {
                    foreach (var character in this.currentLocation.characters.Where(c => c != this))
                    {
                        if (character.GetBoundingBox().Intersects(this.GetBoundingBox()))
                        {
                            base.Position = this.GetRandomAdjacentOpenTile(currentTile, this.currentLocation) * 64f;
                        }
                    }
                }


                if (base.Position != Vector2.Zero)
                {
                    break;
                }


                // Select a random adjacent tile as our next checking point
                var adjacentTiles = Utility.getAdjacentTileLocations(currentTile);
                currentTile = adjacentTiles[Game1.random.Next(adjacentTiles.Count)];
            }
        }

        internal Vector2 GetRandomAdjacentOpenTile(Vector2 tile, GameLocation location)
        {
            // Using a mostly modified version of Utility.getRandomAdjacentOpenTile
            List<Vector2> i = Utility.getAdjacentTileLocations(tile);
            int iter = 0;
            int which = Game1.random.Next(i.Count);

            Vector2 v = i[which];
            for (; iter < 4; iter++)
            {
                if (!location.isTileOccupiedForPlacement(v) && location.isTilePassable(new xTile.Dimensions.Location((int)v.X, (int)v.Y), Game1.viewport) && String.IsNullOrEmpty(location.doesTileHaveProperty((int)v.X, (int)v.Y, "NPCBarrier", "Back")))
                {
                    break;
                }
                which = (which + 1) % i.Count;
                v = i[which];
            }
            if (iter >= 4)
            {
                return Vector2.Zero;
            }

            return v;
        }

        internal bool IsHovering()
        {
            return this.idleBehavior.behavior == Behavior.HOVER;
        }

        internal bool IsFlying()
        {
            return this.model.Type.ToUpper() == "FLYING";
        }

        internal bool IsJumper()
        {
            return this.model.Type.ToUpper() == "JUMPING";
        }

        internal void PerformJumpMovement(float jumpScale, float randomJumpBoostMultiplier, float gravity, Vector2 targetTile)
        {
            if (this.yJumpOffset == 0)
            {
                this.jumpWithoutSound();
                this.yJumpGravity = Math.Abs(gravity) * -1;
                this.yJumpVelocity = (float)Game1.random.Next(50, 70) / jumpScale;

                if (Game1.random.NextDouble() < 0.01)
                {
                    this.yJumpVelocity *= randomJumpBoostMultiplier;
                }
            }

            Vector2 v = Utility.getAwayFromPositionTrajectory(this.GetBoundingBox(), targetTile);
            this.xVelocity += (0f - v.X) / 150f + ((Game1.random.NextDouble() < 0.01) ? ((float)Game1.random.Next(-50, 50) / 10f) : 0f);
            if (Math.Abs(this.xVelocity) > 5f)
            {
                this.xVelocity = Math.Sign(this.xVelocity) * 5;
            }
            this.yVelocity += (0f - v.Y) / 150f + ((Game1.random.NextDouble() < 0.01) ? ((float)Game1.random.Next(-50, 50) / 10f) : 0f);
            if (Math.Abs(this.yVelocity) > 5f)
            {
                this.yVelocity = Math.Sign(this.yVelocity) * 5;
            }

            this.SetMotion(Utility.getVelocityTowardPoint(new Point((int)this.Position.X, (int)this.Position.Y), targetTile, this.speed));
        }

        internal Vector2 GetTargetPosition()
        {
            if (owner != null && owner.currentLocation == this.currentLocation)
            {
                return owner.position + new Vector2(this.model.SpawnOffsetX, this.model.SpawnOffsetY);
            }

            return targetTile;
        }

        internal void SetMotion(Vector2 motion)
        {
            this.motion.Value = motion;
        }

        internal void SetFacingDirection(int direction)
        {
            this.previousDirection.Value = this.FacingDirection;
            this.FacingDirection = direction;
        }

        internal void SetMovingDirection(int direction)
        {
            base.moveUp = false;
            base.moveDown = false;
            base.moveRight = false;
            base.moveLeft = false;

            switch (direction)
            {
                case 0:
                    moveUp = true;
                    break;
                case 1:
                    moveRight = true;
                    break;
                case 2:
                    moveDown = true;
                    break;
                case 3:
                    moveLeft = true;
                    break;
            }
        }

        internal void ResetForNewLocation(GameLocation location, Vector2 position)
        {
            base.Position = position * 64f;
            this.currentLocation = location;

            if (this.light != null)
            {
                Game1.currentLightSources.Add(this.light);
            }

            if (this.collidesWithOtherCharacters)
            {
                this.PlaceInEmptyTile();
            }
        }

        internal void PrepareForDeletion()
        {
            if (this.light != null)
            {
                Game1.currentLightSources.Remove(this.light);
            }
        }

        internal virtual void UpdateModel(CompanionModel updatedModel)
        {
            // Update sounds
            if (this.idleSound is null || this.idleSound != updatedModel.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "IDLE"))
            {
                this.idleSound = updatedModel.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "IDLE");
                if (this.idleSound != null && CustomCompanions.IsSoundValid(this.idleSound.SoundName, true))
                {
                    this.soundIdleTimer = this.idleSound.TimeBetweenSound;
                }
            }
            if (this.movingSound is null || this.movingSound != updatedModel.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "MOVING"))
            {
                this.movingSound = updatedModel.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "MOVING");
                if (this.movingSound != null && CustomCompanions.IsSoundValid(this.movingSound.SoundName, true))
                {
                    this.soundMovingTimer = this.movingSound.TimeBetweenSound;
                }
            }
            if (this.alwaysSound is null || this.alwaysSound != updatedModel.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "ALWAYS"))
            {
                this.alwaysSound = updatedModel.Sounds.FirstOrDefault(s => s.WhenToPlay.ToUpper() == "ALWAYS");
                if (this.alwaysSound != null && CustomCompanions.IsSoundValid(this.alwaysSound.SoundName, true))
                {
                    this.soundMovingTimer = this.alwaysSound.TimeBetweenSound;
                }
            }

            // Update color
            if (Game1.IsMasterGame)
            {
                if (!updatedModel.ContainsColor(this.color.Value) || this.model.IsPrismatic != updatedModel.IsPrismatic)
                {
                    this.isPrismatic.Value = false;
                    if (updatedModel.Colors.Count == 0 && updatedModel.IsPrismatic)
                    {
                        this.isPrismatic.Value = true;
                    }
                    else if (updatedModel.Colors.Count > 0 && !updatedModel.ContainsColor(this.color.Value))
                    {
                        int randomColorIndex = Game1.random.Next(updatedModel.Colors.Count + (updatedModel.IsPrismatic ? 1 : 0));
                        if (randomColorIndex > updatedModel.Colors.Count - 1)
                        {
                            // Primsatic color has been selected
                            this.isPrismatic.Value = true;
                        }
                        else
                        {
                            this.color.Value = CustomCompanions.GetColorFromArray(updatedModel.Colors[randomColorIndex]);
                        }
                    }
                    else if (updatedModel.Colors.Count == 0 && !updatedModel.IsPrismatic)
                    {
                        this.color.Value = Color.White;
                    }
                }

                // Set up the light to give off, if any
                if (this.model.Light != updatedModel.Light)
                {
                    if (this.light != null)
                    {
                        Game1.currentLightSources.Remove(this.light);
                        this.light = null;
                    }

                    if (updatedModel.Light != null)
                    {
                        this.lightPulseTimer = updatedModel.Light.PulseSpeed;

                        this.light = new LightSource(1, new Vector2(this.position.X + updatedModel.Light.OffsetX, this.position.Y + updatedModel.Light.OffsetY), updatedModel.Light.Radius, CustomCompanions.GetColorFromArray(updatedModel.Light.Color), this.id, LightSource.LightContext.None, 0L);
                        Game1.currentLightSources.Add(this.light);
                    }
                }
            }

            // Clear previous animation data
            idleUniformFrames = null;
            activeUniformFrames = null;
            idleUpFrames = null;
            activeUpFrames = null;
            idleRightFrames = null;
            activeRightFrames = null;
            idleDownFrames = null;
            activeDownFrames = null;
            idleLeftFrames = null;
            activeLeftFrames = null;

            // Set up uniform frames
            if (updatedModel.UniformAnimation != null && !CustomCompanions.CompanionHasFullMovementSet(updatedModel))
            {
                idleUniformFrames = (updatedModel.UniformAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.UniformAnimation.IdleAnimation.ManualFrames) : GetManualFrames(updatedModel.UniformAnimation.IdleAnimation.StartingFrame, updatedModel.UniformAnimation.IdleAnimation.NumberOfFrames, updatedModel.UniformAnimation.IdleAnimation.Duration);
                activeUniformFrames = (updatedModel.UniformAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.UniformAnimation.ManualFrames) : GetManualFrames(updatedModel.UniformAnimation.StartingFrame, updatedModel.UniformAnimation.NumberOfFrames, updatedModel.UniformAnimation.Duration);
            }
            else
            {
                // Up frames
                idleUpFrames = (updatedModel.UpAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.UpAnimation.IdleAnimation.ManualFrames) : GetManualFrames(updatedModel.UpAnimation.IdleAnimation.StartingFrame, updatedModel.UpAnimation.IdleAnimation.NumberOfFrames, updatedModel.UpAnimation.IdleAnimation.Duration);
                activeUpFrames = (updatedModel.UpAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.UpAnimation.ManualFrames) : GetManualFrames(updatedModel.UpAnimation.StartingFrame, updatedModel.UpAnimation.NumberOfFrames, updatedModel.UpAnimation.Duration);

                // Right frames
                idleRightFrames = (updatedModel.RightAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.RightAnimation.IdleAnimation.ManualFrames) : GetManualFrames(updatedModel.RightAnimation.IdleAnimation.StartingFrame, updatedModel.RightAnimation.IdleAnimation.NumberOfFrames, updatedModel.RightAnimation.IdleAnimation.Duration);
                activeRightFrames = (updatedModel.RightAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.RightAnimation.ManualFrames) : GetManualFrames(updatedModel.RightAnimation.StartingFrame, updatedModel.RightAnimation.NumberOfFrames, updatedModel.RightAnimation.Duration);

                // Down frames
                idleDownFrames = (updatedModel.DownAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.DownAnimation.IdleAnimation.ManualFrames) : GetManualFrames(updatedModel.DownAnimation.IdleAnimation.StartingFrame, updatedModel.DownAnimation.IdleAnimation.NumberOfFrames, updatedModel.DownAnimation.IdleAnimation.Duration);
                activeDownFrames = (updatedModel.DownAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.DownAnimation.ManualFrames) : GetManualFrames(updatedModel.DownAnimation.StartingFrame, updatedModel.DownAnimation.NumberOfFrames, updatedModel.DownAnimation.Duration);

                // Left frames
                idleLeftFrames = (updatedModel.LeftAnimation.IdleAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.LeftAnimation.IdleAnimation.ManualFrames) : GetManualFrames(updatedModel.LeftAnimation.IdleAnimation.StartingFrame, updatedModel.LeftAnimation.IdleAnimation.NumberOfFrames, updatedModel.LeftAnimation.IdleAnimation.Duration);
                activeLeftFrames = (updatedModel.LeftAnimation.ManualFrames != null) ? GetManualFrames(updatedModel.LeftAnimation.ManualFrames) : GetManualFrames(updatedModel.LeftAnimation.StartingFrame, updatedModel.LeftAnimation.NumberOfFrames, updatedModel.LeftAnimation.Duration);
            }

            // Preserve the translations
            updatedModel.Translations = this.model.Translations;

            // Update the model itself
            this.model = updatedModel;

            // Leftover settings
            base.displayName = null;
            base.Breather = model.EnableBreathing;
            base.speed = model.TravelSpeed;
            base.Scale = model.Scale;

            if (Game1.IsMasterGame)
            {
                this.hasShadow.Value = model.EnableShadow;
                base.collidesWithOtherCharacters.Value = (model.Type.ToUpper() == "FLYING" ? false : true);
                if (base.Sprite.loadedTexture != model.TileSheetPath || base.Sprite.SpriteWidth != model.FrameSizeWidth || base.Sprite.SpriteHeight != model.FrameSizeHeight)
                {
                    base.Sprite = new AnimatedSprite(model.TileSheetPath, 0, model.FrameSizeWidth, model.FrameSizeHeight);
                }
            }

            // Avoid issue where MaxHaltTime may be higher than MinHaltTime
            if (this.model.MinHaltTime > this.model.MaxHaltTime)
            {
                this.model.MinHaltTime = this.model.MaxHaltTime;
            }

            this.idleBehavior = new IdleBehavior(this, model.IdleBehavior);
        }
    }
}

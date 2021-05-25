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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace CustomCompanions.Framework.Companions
{
    public class MapCompanion : Companion
    {
        private int pauseTimer;
        private bool canHalt;
        private float motionMultiplier;
        private float behaviorTimer;

        public MapCompanion()
        {

        }

        public MapCompanion(CompanionModel model, Vector2 targetTile, GameLocation location) : base(model, null, targetTile)
        {
            base.targetTile.Value = targetTile * 64f;
            base.currentLocation = location;
            base.willDestroyObjectsUnderfoot = false;

            base.farmerPassesThrough = model.EnableFarmerCollision ? false : true;

            // Avoid issue where MaxHaltTime may be higher than MinHaltTime
            if (this.model.MinHaltTime > this.model.MaxHaltTime)
            {
                this.model.MinHaltTime = this.model.MaxHaltTime;
            }

            this.canHalt = !base.IsFlying();
            this.motionMultiplier = 1f;

            // Verify the location the companion is spawning on isn't occupied (if collidesWithOtherCharacters == true)
            if (this.collidesWithOtherCharacters)
            {
                this.PlaceInEmptyTile();
            }
            this.nextPosition.Value = this.GetBoundingBox();
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (!Game1.shouldTimePass())
            {
                return;
            }

            if (this.model is null)
            {
                this.model = CompanionManager.companionModels.First(c => c.GetId() == this.companionKey.Value);
                this.SetUpCompanion();
                this.UpdateModel(this.model);

                base.currentLocation = location;
                base.willDestroyObjectsUnderfoot = false;

                base.farmerPassesThrough = model.EnableFarmerCollision ? false : true;

                // Avoid issue where MaxHaltTime may be higher than MinHaltTime
                if (this.model.MinHaltTime > this.model.MaxHaltTime)
                {
                    this.model.MinHaltTime = this.model.MaxHaltTime;
                }

                this.canHalt = !base.IsFlying();
                this.motionMultiplier = 1f;

                // Verify the location the companion is spawning on isn't occupied (if collidesWithOtherCharacters == true)
                if (this.collidesWithOtherCharacters)
                {
                    this.PlaceInEmptyTile();
                }
                this.nextPosition.Value = this.GetBoundingBox();
            }

            base.currentLocation = location;

            // Do Idle Behaviors
            this.PerformBehavior(base.idleBehavior.behavior, base.model.IdleArguments, time, location);

            // Timers
            if (this.pauseTimer > 0)
            {
                this.pauseTimer -= time.ElapsedGameTime.Milliseconds;
            }
            if (this.shakeTimer > 0)
            {
                this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
            }

            if (Game1.IsMasterGame)
            {
                // Update light location, if applicable
                base.UpdateLight(time);

                // Play any sound(s) that are required
                if (Utility.isThereAFarmerWithinDistance(base.getTileLocation(), 10, base.currentLocation) != null)
                {
                    base.PlayRequiredSounds(time, this.isMoving());
                }
            }
        }

        public override bool isMoving()
        {
            if (this.pauseTimer > 0 && canHalt)
            {
                return false;
            }

            if (!this.moveUp && !this.moveDown && !this.moveRight && !this.moveLeft)
            {
                return this.position.Field.IsInterpolating();
            }

            return true;
        }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            // Unused
            return;
        }

        public override void Halt()
        {
            if (canHalt)
            {
                base.Halt();
                base.Sprite.UpdateSourceRect();
                base.speed = this.model.TravelSpeed;
                base.addedSpeed = 0;
            }
        }

        internal override void UpdateModel(CompanionModel updatedModel)
        {
            base.UpdateModel(updatedModel);

            base.farmerPassesThrough = updatedModel.EnableFarmerCollision ? false : true;
        }

        internal void FaceAndMoveInDirection(int direction)
        {
            this.SetFacingDirection(direction);
            this.SetMovingDirection(direction);
        }

        private bool IsCollidingPosition(Microsoft.Xna.Framework.Rectangle position, GameLocation location)
        {
            if (base.currentLocation.isCollidingPosition(position, Game1.viewport, isFarmer: false, 0, glider: false, this))
            {
                return true;
            }

            if (base.currentLocation.isCollidingPosition(position, Game1.viewport, isFarmer: true, 0, glider: false, this))
            {
                return true;
            }

            if (!base.farmerPassesThrough && base.currentLocation.isTileOccupiedByFarmer(new Vector2(position.X / 64, position.Y / 64)) != null)
            {
                return true;
            }

            if (!String.IsNullOrEmpty(location.doesTileHaveProperty(position.X / 64, position.Y / 64, "NPCBarrier", "Back")))
            {
                return true;
            }

            return false;
        }

        private bool HandleCollision(Microsoft.Xna.Framework.Rectangle next_position)
        {
            if (Game1.random.NextDouble() < this.model.ChanceForHalting)
            {
                this.pauseTimer = Game1.random.Next(this.model.MinHaltTime, this.model.MaxHaltTime);
            }

            return false;
        }

        private void AttemptRandomDirection(float chanceWhileMoving, float chanceWhileIdle)
        {
            if (this.pauseTimer > 0 && canHalt)
            {
                this.FaceAndMoveInDirection(this.FacingDirection);
                return;
            }

            if (!Game1.IsClient && Game1.random.NextDouble() < (this.isMoving() ? chanceWhileMoving : chanceWhileIdle))
            {
                int newDirection = Game1.random.Next(5);
                if (newDirection != (this.FacingDirection + 2) % 4)
                {
                    if (newDirection < 4)
                    {
                        if (!base.IsFlying() && this.IsCollidingPosition(this.nextPosition(newDirection), this.currentLocation))
                        {
                            return;
                        }
                    }
                    switch (newDirection)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            this.FaceAndMoveInDirection(newDirection);
                            break;
                        default:
                            if (Game1.random.NextDouble() < this.model.ChanceForHalting)
                            {
                                this.Halt();
                                this.pauseTimer = Game1.random.Next(this.model.MinHaltTime, this.model.MaxHaltTime);
                            }
                            break;
                    }
                }
            }
        }

        private void MovePositionViaSpeed(GameTime time, GameLocation currentLocation)
        {
            if (this.pauseTimer > 0 && canHalt)
            {
                if (this.previousDirection.Value != this.FacingDirection)
                {
                    this.previousDirection.Value = this.facingDirection;
                }
                return;
            }

            Location next_tile = base.nextPositionTile();
            if (!currentLocation.isTileOnMap(new Vector2(next_tile.X, next_tile.Y)))
            {
                this.FaceAndMoveInDirection(Utility.GetOppositeFacingDirection(base.FacingDirection));
                return;
            }
            if (base.moveUp)
            {
                if (!this.IsCollidingPosition(this.nextPosition(0), currentLocation))
                {
                    base.position.Y -= base.speed + base.addedSpeed;
                    this.FaceAndMoveInDirection(0);
                }
                else if (!this.HandleCollision(this.nextPosition(0)))
                {
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(2);
                    }
                }
            }
            else if (base.moveRight)
            {
                if (!this.IsCollidingPosition(this.nextPosition(1), currentLocation))
                {
                    base.position.X += base.speed + base.addedSpeed;
                    this.FaceAndMoveInDirection(1);
                }
                else if (!this.HandleCollision(this.nextPosition(1)))
                {
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(3);
                    }
                }
            }
            else if (base.moveDown)
            {
                if (!this.IsCollidingPosition(this.nextPosition(2), currentLocation))
                {
                    base.position.Y += base.speed + base.addedSpeed;
                    this.FaceAndMoveInDirection(2);
                }
                else if (!this.HandleCollision(this.nextPosition(2)))
                {
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(0);
                    }
                }
            }
            else if (base.moveLeft)
            {
                if (!this.IsCollidingPosition(this.nextPosition(3), currentLocation))
                {
                    base.position.X -= base.speed + base.addedSpeed;
                    this.FaceAndMoveInDirection(3);
                }
                else if (!this.HandleCollision(this.nextPosition(3)))
                {
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(1);
                    }
                }
            }

            var targetDistance = Vector2.Distance(base.Position, this.GetTargetPosition());
            if (targetDistance > this.model.MaxDistanceBeforeTeleport && this.model.MaxDistanceBeforeTeleport != -1)
            {
                base.position.Value = this.GetTargetPosition();
            }
            else if (targetDistance > this.model.MaxIdleDistance && this.model.MaxIdleDistance != -1)
            {
                this.FaceAndMoveInDirection(this.getGeneralDirectionTowards(this.GetTargetPosition(), 0, opposite: false, useTileCalculations: false));

                if (Game1.random.NextDouble() < this.model.ChanceForHalting)
                {
                    this.pauseTimer = Game1.random.Next(this.model.MinHaltTime, this.model.MaxHaltTime);
                }
            }
        }

        private void MovePositionViaMotion(GameTime time, GameLocation currentLocation, bool canCollide = false)
        {
            if (this.pauseTimer > 0 && canHalt)
            {
                if (this.previousDirection.Value != this.FacingDirection)
                {
                    this.previousDirection.Value = this.facingDirection;
                }
                return;
            }

            if (base.moveUp)
            {
                if (!canCollide || !this.IsCollidingPosition(this.nextPosition(0), currentLocation))
                {
                    base.motion.Y -= Game1.random.Next(1, 2) * 0.1f;
                    this.FaceAndMoveInDirection(0);
                }
                else if (!this.HandleCollision(this.nextPosition(0)))
                {
                    var oldMotion = base.motion.Y;

                    base.motion.Y = 0;
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(2);
                        if (Game1.random.NextDouble() < 0.5)
                        {
                            base.motion.Y = Math.Abs(oldMotion / 2f);
                        }
                    }
                }
            }
            else if (base.moveRight)
            {
                if (!canCollide || !this.IsCollidingPosition(this.nextPosition(1), currentLocation))
                {
                    base.motion.X += Game1.random.Next(1, 2) * 0.1f;
                    this.FaceAndMoveInDirection(1);
                }
                else if (!this.HandleCollision(this.nextPosition(1)))
                {
                    var oldMotion = base.motion.X;

                    base.motion.X = 0;
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(3);
                        if (Game1.random.NextDouble() < 0.5)
                        {
                            base.motion.X = Math.Abs(oldMotion / 2f) * -1;
                        }
                    }
                }
            }
            else if (base.moveDown)
            {
                if (!canCollide || !this.IsCollidingPosition(this.nextPosition(2), currentLocation))
                {
                    base.motion.Y += Game1.random.Next(1, 2) * 0.1f;
                    this.FaceAndMoveInDirection(2);
                }
                else if (!this.HandleCollision(this.nextPosition(2)))
                {
                    var oldMotion = base.motion.Y;

                    base.motion.Y = 0;
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(0);
                        if (Game1.random.NextDouble() < 0.5)
                        {
                            base.motion.Y = Math.Abs(oldMotion / 2f) * -1;
                        }
                    }
                }
            }
            else if (base.moveLeft)
            {
                if (!canCollide || !this.IsCollidingPosition(this.nextPosition(3), currentLocation))
                {
                    base.motion.X -= Game1.random.Next(1, 2) * 0.1f;
                    this.FaceAndMoveInDirection(3);
                }
                else if (!this.HandleCollision(this.nextPosition(3)))
                {
                    var oldMotion = base.motion.X;

                    base.motion.X = 0;
                    if (Game1.random.NextDouble() < 0.6)
                    {
                        this.FaceAndMoveInDirection(1);
                        if (Game1.random.NextDouble() < 0.5)
                        {
                            base.motion.X = Math.Abs(oldMotion / 2f);
                        }
                    }
                }
            }

            // Restrict motion
            this.KeepMotionWithinBounds(1f, 1f);

            Location next_tile = base.nextPositionTile();
            var targetDistance = Vector2.Distance(base.Position, this.GetTargetPosition());
            if (targetDistance > this.model.MaxDistanceBeforeTeleport && this.model.MaxDistanceBeforeTeleport != -1)
            {
                base.position.Value = this.GetTargetPosition();
            }
            else if ((targetDistance > this.model.MaxIdleDistance && this.model.MaxIdleDistance != -1) || !currentLocation.isTileOnMap(new Vector2(next_tile.X, next_tile.Y)))
            {
                this.FaceAndMoveInDirection(this.getGeneralDirectionTowards(this.GetTargetPosition(), 0, opposite: false, useTileCalculations: false));

                if (canCollide)
                {
                    this.motion.Value = Vector2.Zero;
                }
            }
            else if (canCollide && this.IsCollidingPosition(this.GetBoundingBox(), currentLocation))
            {
                this.FaceAndMoveInDirection(this.getGeneralDirectionTowards(this.GetTargetPosition(), 0, opposite: false, useTileCalculations: false));
                this.motion.Value = Vector2.Zero;
            }

            // Update position
            base.Position += this.motion.Value * this.motionMultiplier * base.model.TravelSpeed;
            this.motionMultiplier -= 0.0005f * time.ElapsedGameTime.Milliseconds;
            if (this.motionMultiplier < 1f)
            {
                this.motionMultiplier = 1f;
            }
        }

        private void KeepMotionWithinBounds(float xBounds, float yBounds)
        {
            if (base.motion.X < Math.Abs(xBounds) * -1)
            {
                base.motion.X = Math.Abs(xBounds) * -1;
            }
            if (base.motion.X > Math.Abs(xBounds))
            {
                base.motion.X = Math.Abs(xBounds);
            }
            if (base.motion.Y < Math.Abs(yBounds) * -1)
            {
                base.motion.Y = Math.Abs(yBounds) * -1;
            }
            if (base.motion.Y > Math.Abs(yBounds))
            {
                base.motion.Y = Math.Abs(yBounds);
            }
        }

        private bool PerformBehavior(Behavior behavior, float[] arguments, GameTime time, GameLocation location)
        {
            switch (behavior)
            {
                case Behavior.WANDER:
                    if (base.IsFlying())
                    {
                        DoWanderFly(arguments, time, location);
                    }
                    else
                    {
                        DoWanderWalk(arguments, time, location);
                    }
                    return true;
                case Behavior.HOVER:
                    DoHover(arguments, time, location);
                    return true;
                case Behavior.JUMPER:
                    DoJump(arguments, time, location);
                    return true;
                default:
                    DoNothing(arguments, time, location);
                    return false;
            }
        }

        private void DoWanderFly(float[] arguments, GameTime time, GameLocation location)
        {
            // Handle arguments
            if (Game1.IsMasterGame)
            {
                float dashMultiplier = 1f;
                int minTimeBetweenDash = 5000;
                if (arguments != null)
                {
                    if (arguments.Length > 0)
                    {
                        dashMultiplier = arguments[0];
                    }
                    if (arguments.Length > 1)
                    {
                        minTimeBetweenDash = (int)arguments[1];
                    }
                }

                // Handle random directional changes
                this.AttemptRandomDirection(base.model.DirectionChangeChanceWhileMoving, base.model.DirectionChangeChanceWhileIdle);

                this.behaviorTimer -= time.ElapsedGameTime.Milliseconds;
                if (this.behaviorTimer <= 0)
                {
                    this.motionMultiplier = dashMultiplier;
                    this.behaviorTimer = Game1.random.Next(minTimeBetweenDash, minTimeBetweenDash * 2);
                }

                // Handle animating
                base.isIdle.Value = !this.isMoving();
                base.Animate(time, base.isIdle);
                base.update(time, location, -1, move: false);
                base.wasIdle = base.isIdle;

                this.MovePositionViaMotion(time, location);
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }

        private void DoWanderWalk(float[] arguments, GameTime time, GameLocation location)
        {
            // Handle random movement
            if (Game1.IsMasterGame)
            {
                this.AttemptRandomDirection(base.model.DirectionChangeChanceWhileMoving, base.model.DirectionChangeChanceWhileIdle);

                // Handle animating
                base.isIdle.Value = !this.isMoving();
                base.Animate(time, base.isIdle);
                base.update(time, location, -1, move: false);
                base.wasIdle = base.isIdle;

                this.MovePositionViaSpeed(time, location);
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }

        private void DoHover(float[] arguments, GameTime time, GameLocation location)
        {
            // Handle animating
            if (Game1.IsMasterGame)
            {
                base.isIdle.Value = false;
                base.Animate(time, base.isIdle);
                base.update(time, location, -1, move: false);
                base.wasIdle = base.isIdle;

                var gravity = -0.5f;
                if (arguments != null)
                {
                    if (arguments.Length > 0)
                    {
                        gravity = arguments[0];
                    }
                }

                if (this.yJumpOffset == 0)
                {
                    this.jumpWithoutSound(5);
                    this.yJumpGravity = Math.Abs(gravity) * -1;
                }
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }

        private void DoJump(float[] arguments, GameTime time, GameLocation location)
        {
            // Handle random movement
            if (Game1.IsMasterGame)
            {
                this.AttemptRandomDirection(base.model.DirectionChangeChanceWhileMoving, base.model.DirectionChangeChanceWhileIdle);

                // Handle animating
                base.isIdle.Value = !this.isMoving();
                base.Animate(time, base.isIdle);
                base.update(time, location, -1, move: false);
                base.wasIdle = base.isIdle;

                var gravity = -0.5f;
                var jumpScale = 10f;
                var randomJumpBoostMultiplier = 2f;
                if (arguments != null)
                {
                    if (arguments.Length > 0)
                    {
                        gravity = arguments[0];
                    }
                    if (arguments.Length > 1)
                    {
                        jumpScale = arguments[1];
                    }
                    if (arguments.Length > 2)
                    {
                        randomJumpBoostMultiplier = arguments[2];
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

                this.MovePositionViaMotion(time, location, true);
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }

        private void DoNothing(float[] arguments, GameTime time, GameLocation location)
        {
            // Handle random movement
            if (Game1.IsMasterGame)
            {
                this.AttemptRandomDirection(base.model.DirectionChangeChanceWhileMoving, base.model.DirectionChangeChanceWhileIdle);

                // Handle animating
                base.isIdle.Value = true;
                base.Animate(time, base.isIdle);
                base.update(time, location, -1, move: false);
                base.wasIdle = base.isIdle;

                this.FaceAndMoveInDirection(this.FacingDirection);
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }
    }
}
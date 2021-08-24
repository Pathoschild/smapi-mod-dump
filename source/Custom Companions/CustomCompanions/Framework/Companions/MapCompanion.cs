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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.PathFindController;

namespace CustomCompanions.Framework.Companions
{
    public class MapCompanion : Companion
    {
        private int? despawnTimer;
        private int stuckTimer;
        private int pauseTimer;
        private bool canHalt;
        private float motionMultiplier;
        private float behaviorTimer;
        private float overheadTextSelectionTimer;

        // Path finder related
        private bool bypassCollision;
        private bool hasReachedDestination;
        private Stack<Point> activePath;

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

            this.canHalt = !base.IsFlying() && this.model.CanHalt;
            this.motionMultiplier = 1f;

            // Verify the location the companion is spawning on isn't occupied (if collidesWithOtherCharacters == true)
            if (this.collidesWithOtherCharacters)
            {
                this.PlaceInEmptyTile();
            }
            this.nextPosition.Value = this.GetBoundingBox();

            this.activePath = new Stack<Point>();

            // Set up despawn timer, if valid
            if (this.model.DespawnOnTimer >= 0)
            {
                this.despawnTimer = this.model.DespawnOnTimer;
            }

            // Set up overhead text timer
            this.overheadTextSelectionTimer = this.model.OverheadTextCheckInterval;

            // Set up portrait, if valid
            if (this.model.Portrait != null && !String.IsNullOrEmpty(this.model.Portrait.PortraitSheetPath))
            {
                this.displayName = String.IsNullOrEmpty(this.model.Portrait.PortraitDisplayName) ? this.displayName : this.model.Portrait.PortraitDisplayName;
                this.Portrait = CustomCompanions.modHelper.ContentPacks.GetOwned().First(c => c.Manifest.UniqueID == this.model.Owner).LoadAsset<Texture2D>(this.model.Portrait.PortraitSheetPath);
            }
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

            // Check if player is nearby for UpdateWhenPlayerNearby property, if applicable
            if (this.model.UpdateWhenPlayerNearby != null && Utility.isThereAFarmerWithinDistance(base.getTileLocation(), this.model.MinTilesForNearby, base.currentLocation) != null)
            {
                this.UpdateModel(this.model.UpdateWhenPlayerNearby);
            }

            // Do Idle Behaviors
            this.PerformBehavior(base.idleBehavior.behavior, base.model.IdleArguments, time, location);

            // Check for overhead text
            this.AttemptOverheadText(time);

            // Timers
            if (this.pauseTimer > 0)
            {
                this.pauseTimer -= time.ElapsedGameTime.Milliseconds;
            }
            if (this.shakeTimer > 0)
            {
                this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
            }
            if (this.despawnTimer > 0)
            {
                this.despawnTimer -= time.ElapsedGameTime.Milliseconds;
            }

            // Stuck timer
            this.CheckStuckStatus(location, time);

            if (Game1.IsMasterGame)
            {
                if (this.despawnTimer <= 0)
                {
                    this.Despawn();
                    return;
                }

                // Update light location, if applicable
                base.UpdateLight(time);

                // Play any sound(s) that are required
                if (Utility.isThereAFarmerWithinDistance(base.getTileLocation(), 10, base.currentLocation) != null)
                {
                    base.PlayRequiredSounds(time, this.isMoving());
                }
            }
        }

        internal void AttemptOverheadText(GameTime time)
        {
            if (this.model.OverheadTexts.Count == 0)
            {
                return;
            }

            // Overhead text timer
            if (this.overheadTextSelectionTimer > 0)
            {
                this.overheadTextSelectionTimer -= time.ElapsedGameTime.Milliseconds;
            }
            else
            {
                if (Game1.random.NextDouble() < this.model.OverheadTextChance && String.IsNullOrEmpty(base.textAboveHead))
                {
                    var weightedSelection = this.model.OverheadTexts.Where(o => o.ChanceWeight >= Game1.random.NextDouble()).ToList();
                    if (weightedSelection.Count == 0)
                    {
                        weightedSelection = this.model.OverheadTexts;
                    }

                    var selectedOverheadObject = weightedSelection[Game1.random.Next(weightedSelection.Count)];

                    // Check for any translations
                    var selectedText = selectedOverheadObject.Text;
                    if (this.model.Translations.GetTranslations().Any(t => t.Key == selectedText))
                    {
                        selectedText = this.model.Translations.Get(selectedText);
                    }

                    base.showTextAboveHead(selectedText, duration: selectedOverheadObject.TextLifetime);
                }

                this.overheadTextSelectionTimer = this.model.OverheadTextCheckInterval;
            }

            // Handle text display
            if (base.textAboveHeadTimer > 0)
            {
                if (base.textAboveHeadPreTimer > 0)
                {
                    base.textAboveHeadPreTimer -= time.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    base.textAboveHeadTimer -= time.ElapsedGameTime.Milliseconds;
                    if (base.textAboveHeadTimer > 500)
                    {
                        base.textAboveHeadAlpha = Math.Min(1f, base.textAboveHeadAlpha + 0.1f);
                    }
                    else
                    {
                        base.textAboveHeadAlpha = Math.Max(0f, base.textAboveHeadAlpha - 0.04f);
                    }

                    if (base.textAboveHeadTimer <= 0)
                    {
                        base.clearTextAboveHead();
                    }
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

            // Set up despawn timer, if valid
            if (updatedModel.DespawnOnTimer >= 0)
            {
                this.despawnTimer = updatedModel.DespawnOnTimer;
            }

            // Set up overhead text timer
            this.overheadTextSelectionTimer = this.model.OverheadTextCheckInterval;

            // Set up portrait, if valid
            if (this.model.Portrait != null && !String.IsNullOrEmpty(this.model.Portrait.PortraitSheetPath))
            {
                this.displayName = String.IsNullOrEmpty(this.model.Portrait.PortraitDisplayName) ? this.displayName : this.model.Portrait.PortraitDisplayName;
                this.Portrait = CustomCompanions.modHelper.ContentPacks.GetOwned().First(c => c.Manifest.UniqueID == this.model.Owner).LoadAsset<Texture2D>(this.model.Portrait.PortraitSheetPath);
            }
        }

        internal void FaceAndMoveInDirection(int direction)
        {
            this.SetFacingDirection(direction);
            this.SetMovingDirection(direction);
        }

        internal void RotateDirectionClockwise(int direction, bool reverse = false)
        {
            if (direction >= 3 && !reverse)
            {
                direction = -1;
            }
            else if (direction <= 0 && reverse)
            {
                direction = 4;
            }

            FaceAndMoveInDirection(direction + (reverse ? -1 : 1));
        }

        internal Vector2 GetTargetTile()
        {
            return new Vector2(base.targetTile.X / 64, base.targetTile.Y / 64);

        }

        internal void Despawn()
        {
            base.PrepareForDeletion();
            base.currentLocation.characters.Remove(this);

            // Check if we need to disable respawning
            if (!this.model.Respawn)
            {
                CompanionManager.DenyCompanionFromRespawning(base.currentLocation, this.GetTargetTile(), this);
            }
        }

        private void CheckStuckStatus(GameLocation location, GameTime time)
        {
            var collidingCharacter = location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection));
            bool isCollidingWithCharacter = collidingCharacter != null && (!collidingCharacter.Equals(this) || (collidingCharacter is MapCompanion && (collidingCharacter as MapCompanion).targetTile != this.targetTile));

            if (base.currentLocation.isTileLocationTotallyClearAndPlaceable(new Vector2(this.nextPosition(this.FacingDirection).X, this.nextPosition(this.FacingDirection).Y)))
            {
                this.stuckTimer = 0;
                this.bypassCollision = false;
            }
            else
            {
                this.stuckTimer += time.ElapsedGameTime.Milliseconds;
            }

            if (this.stuckTimer > 5000)
            {
                this.bypassCollision = true;
            }
        }

        private bool IsCollidingWithFarmer(GameLocation location, Rectangle position)
        {
            return location.farmers.Any(f => f != null && f.GetBoundingBox().Intersects(position));
        }

        internal bool IsCollidingPosition(Rectangle position, GameLocation location, bool isPathFinding = false)
        {
            var collidingCharacter = location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection));
            if (this.bypassCollision && collidingCharacter != null && (!collidingCharacter.Equals(this) || (collidingCharacter is MapCompanion && (collidingCharacter as MapCompanion).targetTile != this.targetTile)))
            {
                return false;
            }

            if (location.isCollidingPosition(position, Game1.viewport, isFarmer: false, 0, glider: false, this, pathfinding: isPathFinding))
            {
                return true;
            }

            if (location.isCollidingPosition(position, Game1.viewport, isFarmer: true, 0, glider: false, this, pathfinding: isPathFinding))
            {
                return true;
            }

            if (!base.farmerPassesThrough && location.isTileOccupiedByFarmer(new Vector2(position.X / 64f, position.Y / 64f)) != null)
            {
                return true;
            }

            if (!String.IsNullOrEmpty(location.doesTileHaveProperty(position.X / 64, position.Y / 64, "NPCBarrier", "Back")))
            {
                return true;
            }

            return false;
        }

        private bool HandleCollision(GameLocation location, Rectangle next_position)
        {
            if (Game1.random.NextDouble() < this.model.ChanceForHalting)
            {
                this.pauseTimer = Game1.random.Next(this.model.MinHaltTime, this.model.MaxHaltTime);
            }

            if (base.idleBehavior.behavior == Behavior.WALK_SQUARE)
            {
                if (!this.IsCollidingWithFarmer(location, next_position))
                {
                    this.RotateDirectionClockwise(this.FacingDirection);
                    base.lastCrossroad = new Rectangle(base.getTileX() * 64, base.getTileY() * 64, 64, 64);
                }

                return true;
            }
            if (base.idleBehavior.behavior == Behavior.PACING)
            {
                if (!this.IsCollidingWithFarmer(location, next_position))
                {
                    activePath = new Stack<Point>();
                    this.FaceAndMoveInDirection(Utility.GetOppositeFacingDirection(this.FacingDirection));
                }

                return true;
            }
            if (base.idleBehavior.behavior == Behavior.SIMPLE_PATH)
            {
                activePath = new Stack<Point>();

                return true;
            }
            if (base.idleBehavior.behavior == Behavior.FOLLOW)
            {
                return true;
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

            xTile.Dimensions.Location next_tile = base.nextPositionTile();
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(0)))
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(1)))
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(2)))
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(3)))
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(0)))
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(1)))
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(2)))
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
                else if (!this.HandleCollision(currentLocation, this.nextPosition(3)))
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

            var next_tile = base.nextPositionTile();
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

        private void MoveInSquare(GameTime time, GameLocation currentLocation, int width, int length)
        {
            var distance = Vector2.Distance(this.position, new Vector2(this.lastCrossroad.X, this.lastCrossroad.Y));
            if (distance > width * 64f && this.getVerticalMovement() == 0)
            {
                this.RotateDirectionClockwise(this.FacingDirection);
                base.lastCrossroad = new Rectangle(base.getTileX() * 64, base.getTileY() * 64, 64, 64);
            }
            else if (distance > length * 64f && this.getHorizontalMovement() == 0)
            {
                this.RotateDirectionClockwise(this.FacingDirection);
                base.lastCrossroad = new Rectangle(base.getTileX() * 64, base.getTileY() * 64, 64, 64);
            }
        }

        private void FollowActivePath()
        {
            if (activePath is null || this.pauseTimer > 0)
            {
                return;
            }

            Point peek = activePath.Peek();
            Rectangle targetTile = new Rectangle(peek.X * 64, peek.Y * 64, 64, 64);
            targetTile.Inflate(-2, 0);
            Rectangle bbox = base.GetBoundingBox();
            if ((targetTile.Contains(bbox) || (bbox.Width > targetTile.Width && targetTile.Contains(bbox.Center))) && targetTile.Bottom - bbox.Bottom >= 2)
            {
                activePath.Pop();
                //base.stopWithoutChangingFrame();
                if (activePath.Count == 0)
                {
                    this.bypassCollision = false;
                    /*
                    this.Halt();
                    if (this.endBehaviorFunction != null)
                    {
                        this.endBehaviorFunction(this.character, this.location);
                    }
                    */
                    // Check for despawn conditions
                    if (this.model.DespawnOnTile != null && this.model.DespawnOnTile.Length > 1)
                    {
                        var despawnTile = new Vector2(this.model.DespawnOnTile[0], this.model.DespawnOnTile[1]);
                        if (base.getTileLocation() == despawnTile)
                        {
                            this.Despawn();
                            return;
                        }
                    }
                }

                return;
            }

            string name = base.Name;
            foreach (NPC c in base.currentLocation.characters)
            {
                if (!c.Equals(this) && c.GetBoundingBox().Intersects(bbox) && c.isMoving() && string.Compare(c.Name, name, StringComparison.Ordinal) < 0)
                {
                    //this.Halt();
                    return;
                }
            }

            if (bbox.Left < targetTile.Left && bbox.Right < targetTile.Right)
            {
                this.FaceAndMoveInDirection(1);
            }
            else if (bbox.Right > targetTile.Right && bbox.Left > targetTile.Left)
            {
                this.FaceAndMoveInDirection(3);
            }
            else if (bbox.Top <= targetTile.Top)
            {
                this.FaceAndMoveInDirection(2);
            }
            else if (bbox.Bottom >= targetTile.Bottom - 2)
            {
                this.FaceAndMoveInDirection(0);
            }
        }

        private void AttemptDespawnNearEdge()
        {
            if (!this.isMoving())
            {
                // Check for despawn conditions
                if (this.model.DespawnOnTile != null && this.model.DespawnOnTile.Length > 1)
                {
                    Point peek = new Point(this.model.DespawnOnTile[0], this.model.DespawnOnTile[1]);
                    Rectangle targetTile = new Rectangle(peek.X * 64, peek.Y * 64, 64, 64);
                    targetTile.Inflate(32, 32);
                    Rectangle bbox = base.GetBoundingBox();

                    if (targetTile.Contains(bbox))
                    {
                        this.Despawn();
                        return;
                    }
                }
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
                case Behavior.WALK_SQUARE:
                    DoWalkSquare(arguments, time, location);
                    return true;
                case Behavior.PACING:
                    DoPacing(arguments, time, location);
                    return true;
                case Behavior.SIMPLE_PATH:
                    DoSimplePath(arguments, time, location);
                    return true;
                case Behavior.FOLLOW:
                    DoFollow(arguments, time, location);
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

        private void DoWalkSquare(float[] arguments, GameTime time, GameLocation location)
        {
            if (Game1.IsMasterGame)
            {
                var squareWidth = 2;
                var squareHeight = 2;
                if (arguments != null)
                {
                    if (arguments.Length > 0)
                    {
                        squareWidth = (int)arguments[0];
                    }
                    if (arguments.Length > 1)
                    {
                        squareHeight = (int)arguments[1];
                    }
                }

                if (base.lastCrossroad == Rectangle.Empty)
                {
                    base.lastCrossroad = new Rectangle(base.getTileX() * 64, base.getTileY() * 64, 64, 64);
                }

                this.MoveInSquare(time, location, squareWidth, squareHeight);

                this.isIdle.Value = false;
                base.Animate(time, this.isIdle);
                base.update(time, location, -1, move: false);
                this.wasIdle = this.isIdle;

                this.MovePositionViaSpeed(time, location);
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }

        }

        private void DoPacing(float[] arguments, GameTime time, GameLocation location)
        {
            if (Game1.IsMasterGame)
            {
                var xPacingTiles = 5;
                var yPacingTiles = 0;
                if (arguments != null)
                {
                    if (arguments.Length > 0)
                    {
                        xPacingTiles = (int)arguments[0];
                    }
                    if (arguments.Length > 1)
                    {
                        yPacingTiles = (int)arguments[1];
                    }
                }

                var destinationTile = this.GetTargetTile() + new Vector2(xPacingTiles, yPacingTiles);
                if (activePath is null || activePath.Count == 0)
                {
                    if (base.getTileLocation() == destinationTile)
                    {
                        this.hasReachedDestination = true;
                    }
                    else if (base.getTileLocation() == this.GetTargetTile())
                    {
                        this.hasReachedDestination = false;
                    }

                    if (this.hasReachedDestination)
                    {
                        destinationTile = this.GetTargetTile();
                    }

                    //base.stopWithoutChangingFrame();
                    //base.SetFacingDirection(this.getGeneralDirectionTowards(destinationTile, 0, opposite: false, useTileCalculations: true));
                    activePath = PathFindController.findPathForNPCSchedules(new Point((int)base.getTileLocation().X, (int)base.getTileLocation().Y), new Point((int)destinationTile.X, (int)destinationTile.Y), base.currentLocation, 300);
                }

                this.FollowActivePath();

                this.isIdle.Value = !this.isMoving();
                base.Animate(time, this.isIdle);
                base.update(time, location, -1, move: false);
                this.wasIdle = this.isIdle;

                this.MovePositionViaSpeed(time, location);
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }

        private void DoSimplePath(float[] arguments, GameTime time, GameLocation location)
        {
            if (Game1.IsMasterGame)
            {
                var xDestination = this.GetTargetTile().X;
                var yDestination = this.GetTargetTile().Y;
                var waitTime = 5000;
                var stopAtDestination = false;
                if (arguments != null)
                {
                    if (arguments.Length > 1)
                    {
                        xDestination = (int)arguments[0];
                        yDestination = (int)arguments[1];
                    }
                    if (arguments.Length > 2)
                    {
                        waitTime = (int)arguments[2];
                    }
                    if (arguments.Length > 3)
                    {
                        stopAtDestination = arguments[3] >= 1;
                    }
                }

                var destinationTile = new Vector2(xDestination, yDestination);
                if (activePath is null || activePath.Count == 0)
                {
                    if (this.hasReachedDestination && stopAtDestination)
                    {
                        // Do nothing
                        this.SetMovingDirection(-1);
                        this.pauseTimer = 1000;
                    }
                    else
                    {
                        if (base.getTileLocation() == destinationTile)
                        {
                            this.hasReachedDestination = true;
                            this.pauseTimer = waitTime;
                        }
                        else if (base.getTileLocation() == this.GetTargetTile())
                        {
                            this.hasReachedDestination = false;
                            this.pauseTimer = waitTime;
                        }

                        if (this.hasReachedDestination && !stopAtDestination)
                        {
                            destinationTile = this.GetTargetTile();
                        }

                        activePath = PathFindController.findPathForNPCSchedules(new Point((int)base.getTileLocation().X, (int)base.getTileLocation().Y), new Point((int)destinationTile.X, (int)destinationTile.Y), base.currentLocation, 300);
                    }
                }

                this.FollowActivePath();

                this.isIdle.Value = !this.isMoving();
                base.Animate(time, this.isIdle);
                base.update(time, location, -1, move: false);
                this.wasIdle = this.isIdle;

                this.MovePositionViaSpeed(time, location);

                this.AttemptDespawnNearEdge();
            }
            else
            {
                this.Animate(time, this.isIdle);
                this.wasIdle = this.isIdle;
            }
        }

        private void DoFollow(float[] arguments, GameTime time, GameLocation location)
        {
            if (Game1.IsMasterGame)
            {
                var followTileRadius = 5;
                if (arguments != null)
                {
                    if (arguments.Length > 0)
                    {
                        followTileRadius = (int)arguments[0];
                    }
                }

                var destinationTile = this.GetTargetTile();
                var targetFollower = Utility.isThereAFarmerWithinDistance(this.GetTargetTile(), followTileRadius, location);
                if (targetFollower != null)
                {
                    destinationTile = targetFollower.getTileLocation();
                }

                if (base.getTileLocation() != destinationTile)
                {
                    if (activePath is null || activePath.Count == 0 || (activePath.Count < 3 && !activePath.Last().Equals(new Point((int)destinationTile.X, (int)destinationTile.Y))))
                    {
                        activePath = PathFindController.findPath(new Point((int)base.getTileLocation().X, (int)base.getTileLocation().Y), new Point((int)destinationTile.X, (int)destinationTile.Y), PathFindController.isAtEndPoint, base.currentLocation, this, 300);
                    }
                }
                else
                {
                    activePath = null;
                    base.SetMovingDirection(-1);
                }

                this.FollowActivePath();

                this.isIdle.Value = !this.isMoving();
                base.Animate(time, this.isIdle);
                base.update(time, location, -1, move: false);
                this.wasIdle = this.isIdle;

                this.MovePositionViaSpeed(time, location);

                this.AttemptDespawnNearEdge();
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
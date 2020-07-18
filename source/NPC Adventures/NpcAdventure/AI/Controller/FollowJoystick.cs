using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NpcAdventure.AI.Controller
{
    /// <summary>
    /// A joystic for follow a path to a target tile. The core of NPC following.
    /// </summary>
    public class FollowJoystick : Internal.IUpdateable
    {
        public const float PATH_NODE_TOLERANCE = 3f;

        #region public fields
        public NPC follower;
        public Queue<Vector2> pathToFollow;
        public Vector2 currentFollowedPoint;
        #endregion

        #region events
        public event EventHandler<MoveEventArgs> Move;
        public event EventHandler<EndOfRouteReachedEventArgs> EndOfRouteReached;
        #endregion

        #region protected fields
        protected PathFinder pathFinder;
        #endregion

        #region private fields
        private bool gatesInThisLocation;
        private bool movedLastFrame;
        private int facingDirection;
        private int switchDirectionSpeed;
        private readonly FieldInfo characterMoveUp;
        private readonly FieldInfo characterMoveDown;
        private readonly FieldInfo characterMoveLeft;
        private readonly FieldInfo characterMoveRight;
        private Vector2 negativeOne = new Vector2(-1, -1);
        private Vector2 lastFrameMovement;
        private Vector2 lastFrameVelocity;
        private Vector2 lastFramePosition;
        private Vector2 lastMovementDirection;
        private Vector2 animationUpdateSum;
        private float lastNodeDiffLen;
        private int timeout;
        #endregion

        /// <summary>
        /// Returns state of joystick is following a path or not.
        /// </summary>
        public bool IsFollowing => this.currentFollowedPoint != this.negativeOne;

        /// <summary>
        /// A speed of movement. If this value set to zero, then follower stands up on stativ position.
        /// </summary>
        public float Speed { get; set; }

        public int BlockedTimer { get; set; } = 90;

        public bool NoCharging { get; set; } = false;

        /// <summary>
        /// Create a new instance of follow path to tile joystick
        /// </summary>
        /// <param name="follower">An NPC who follows a path</param>
        /// <param name="pathFinder">Path finder for find a path to target tile</param>
        public FollowJoystick(NPC follower, PathFinder pathFinder)
        {
            this.follower = follower;
            this.pathFinder = pathFinder;

            this.characterMoveUp = typeof(Character).GetField("moveUp", BindingFlags.NonPublic | BindingFlags.Instance);
            this.characterMoveDown = typeof(Character).GetField("moveDown", BindingFlags.NonPublic | BindingFlags.Instance);
            this.characterMoveLeft = typeof(Character).GetField("moveLeft", BindingFlags.NonPublic | BindingFlags.Instance);
            this.characterMoveRight = typeof(Character).GetField("moveRight", BindingFlags.NonPublic | BindingFlags.Instance);

            this.Reset();
        }

        /// <summary>
        /// Reset the joystick and prepare it to following new path in new location.
        /// </summary>
        public void Reset()
        {
            this.timeout = this.BlockedTimer;
            this.pathToFollow = new Queue<Vector2>();
            this.pathFinder.GameLocation = this.follower.currentLocation;
            this.currentFollowedPoint = this.negativeOne;
            this.gatesInThisLocation = this.CheckForGatesInLocation(this.follower.currentLocation);
            this.follower.Halt();
        }

        /// <summary>
        /// Acquire a target tile to find path and follow it.
        /// </summary>
        /// <param name="targetTile">Tile to acquire</param>
        /// <returns></returns>
        public bool AcquireTarget(Vector2 targetTile)
        {
            this.follower.isCharging = false;
            this.pathToFollow = this.pathFinder.Pathfind(this.follower.getTileLocation(), targetTile);
            this.timeout = this.BlockedTimer;

            if (this.pathToFollow != null && this.pathToFollow.Count > 0 && this.follower.getTileLocation() != this.pathToFollow.Peek())
            {
                this.currentFollowedPoint = this.pathToFollow.Dequeue();
                return true;
            }
            
            this.currentFollowedPoint = this.negativeOne;

            return false;
        }

        /// <summary>
        /// Update the controller. Use it in game/mod update lifecycle.
        /// </summary>
        /// <param name="e"></param>
        public void Update(UpdateTickedEventArgs e)
        {
            this.PathfindingNodeUpdateCheck();

            if (this.IsFollowing)
                this.MoveToTile(this.currentFollowedPoint);
        }

        /// <summary>
        /// Get a facing direction of movement direction.
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        internal int GetFacingDirectionFromMovement(Vector2 movement)
        {
            if (movement == Vector2.Zero)
                return -1;
            int dir = 2;
            if (Math.Abs(movement.X) > Math.Abs(movement.Y))
                dir = movement.X > 0 ? 1 : 3;
            else if (Math.Abs(movement.X) < Math.Abs(movement.Y))
                dir = movement.Y > 0 ? 2 : 0;
            return dir;
        }

        /// <summary>
        /// Move to a current tile as part of the path.
        /// </summary>
        /// <param name="endPointTile">Current tile to move a character onto</param>
        protected void MoveToTile(Vector2 endPointTile)
        {
            Rectangle tileBox = new Rectangle((int)endPointTile.X * 64, (int)endPointTile.Y * 64, 64, 64);
            tileBox.Inflate(-2, 0);
            Point fp = this.follower.GetBoundingBox().Center;
            this.lastFrameMovement = new Vector2(fp.X, fp.Y) - this.lastFramePosition;

            if (this.Speed > 0)
            {
                Point np = new Point(((int)endPointTile.X * 64) + 32, ((int)endPointTile.Y * 64) + 32);
                Vector2 nodeDiff = new Vector2(np.X, np.Y) - new Vector2(fp.X, fp.Y);
                float nodeDiffLen = nodeDiff.Length();

                if (nodeDiffLen <= PATH_NODE_TOLERANCE)
                    return;

                nodeDiff /= nodeDiffLen;

                this.follower.xVelocity = nodeDiff.X * this.Speed;
                this.follower.yVelocity = -nodeDiff.Y * this.Speed;
                this.HandleWallSliding();
                this.HandleGates();

                this.lastFrameVelocity = new Vector2(this.follower.xVelocity, this.follower.yVelocity);
                this.lastFramePosition = new Vector2(fp.X, fp.Y);

                this.animationUpdateSum += new Vector2(this.follower.xVelocity, -this.follower.yVelocity);
                this.AnimationSubUpdate();

                //this.follower.MovePosition(Game1.currentGameTime, Game1.viewport, this.follower.currentLocation); // Update follower movement
                this.ApplyVelocity(this.follower.currentLocation);
                this.lastMovementDirection = this.lastFrameVelocity / this.lastFrameVelocity.Length();

                this.movedLastFrame = true;
                this.Move?.Invoke(this, new MoveEventArgs(false));
            }
            else if (this.movedLastFrame)
            {
                this.follower.Halt();
                this.follower.Sprite.faceDirectionStandard(this.GetFacingDirectionFromMovement(new Vector2(this.lastMovementDirection.X, -this.lastMovementDirection.Y)));
                this.movedLastFrame = false;
                this.Move?.Invoke(this, new MoveEventArgs(true));
            }
            else
            {
                this.follower.xVelocity = 0;
                this.follower.yVelocity = 0;
            }
        }

        protected void ApplyVelocity(GameLocation currentLocation)
        {
            if (currentLocation == null)
                return;

            Rectangle boundingBox = this.follower.GetBoundingBox();
            boundingBox.X += (int)this.follower.xVelocity;
            boundingBox.Y -= (int)this.follower.yVelocity;

            NPC collidingNpc = currentLocation?.isCollidingWithCharacter(boundingBox);
            bool collidingWithVillagerOrMonster = collidingNpc != null && (collidingNpc.isVillager() || collidingNpc.IsMonster);

            if (collidingWithVillagerOrMonster
                || !currentLocation.isCollidingPosition(boundingBox, Game1.viewport, true, 0, false, this.follower)
                || this.follower.isCharging)
            {
                this.follower.position.X += this.follower.xVelocity;
                this.follower.position.Y -= this.follower.yVelocity;
            }
            this.follower.xVelocity = (int)(this.follower.xVelocity - this.follower.xVelocity / 2.0);
            this.follower.yVelocity = (int)(this.follower.yVelocity - this.follower.yVelocity / 2.0);
        }

        private void PathfindingNodeUpdateCheck()
        {
            if (this.currentFollowedPoint != this.negativeOne && this.pathToFollow != null)
            {
                Point w = this.follower.GetBoundingBox().Center;
                Point n = new Point(((int)this.currentFollowedPoint.X * 64) + 32, ((int)this.currentFollowedPoint.Y * 64) + 32);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                float tolerance = PATH_NODE_TOLERANCE + (this.Speed > 5.3f ? this.Speed - 5.28f : 0f);
                if (nodeDiffLen <= tolerance)
                {
                    if (this.pathToFollow.Count == 0)
                    {
                        this.follower.Sprite.StopAnimation();
                        this.follower.Halt();
                        this.pathToFollow = null;
                        this.EndOfRouteReached?.Invoke(this, new EndOfRouteReachedEventArgs(this.currentFollowedPoint));
                        this.currentFollowedPoint = this.negativeOne;
                        this.lastNodeDiffLen = 0;
                        this.timeout = this.BlockedTimer;
                        this.follower.isCharging = false;
                        return;
                    }
                    this.currentFollowedPoint = this.pathToFollow.Dequeue();
                    this.follower.stopWithoutChangingFrame();
                }

                if (nodeDiffLen == this.lastNodeDiffLen && nodeDiffLen > tolerance && this.follower.isMoving() && this.Speed > 0)
                {
                    if (!this.NoCharging && --this.timeout <= 0)
                    {
                        if (Game1.random.Next(1, 4) == 1)
                            this.follower.doEmote(8);
                        this.follower.isCharging = true;
                    }
                }

                this.lastNodeDiffLen = nodeDiffLen;
            }
        }

        private void AnimationSubUpdate()
        {
            if (++this.switchDirectionSpeed == 5)
            {
                this.facingDirection = this.GetFacingDirectionFromMovement(this.animationUpdateSum);
                this.animationUpdateSum = Vector2.Zero;
                this.switchDirectionSpeed = 0;
            }

            if (this.facingDirection >= 0)
            {
                this.follower.faceDirection(this.facingDirection);
                this.SetMovementDirectionAnimation(this.facingDirection);
            }
        }

        private void SetMovementDirectionAnimation(int dir)
        {
            string footStepSound = Utility.isOnScreen(this.follower.getTileLocationPoint(), 1, this.follower.currentLocation) ? "Cowboy_Footstep" : "";
            if (dir < 0 || dir > 3)
                return;

            this.SetMovingOnlyOneDirection(dir);

            switch (dir)
            {
                case 0:
                    this.follower.Sprite.AnimateUp(Game1.currentGameTime, 0, footStepSound); break;
                case 1:
                    this.follower.Sprite.AnimateRight(Game1.currentGameTime, 0, footStepSound); break;
                case 2:
                    this.follower.Sprite.AnimateDown(Game1.currentGameTime, 0, footStepSound); break;
                case 3:
                    this.follower.Sprite.AnimateLeft(Game1.currentGameTime, 0, footStepSound); break;
            }
        }

        private void SetMovingOnlyOneDirection(int dir)
        {
            if (dir < 0 || dir > 3)
                return;

            switch (dir)
            {
                case 0:
                    this.characterMoveUp.SetValue(this.follower, true);
                    this.characterMoveDown.SetValue(this.follower, false);
                    this.characterMoveLeft.SetValue(this.follower, false);
                    this.characterMoveRight.SetValue(this.follower, false);
                    break;
                case 1:
                    this.characterMoveUp.SetValue(this.follower, false);
                    this.characterMoveDown.SetValue(this.follower, false);
                    this.characterMoveLeft.SetValue(this.follower, false);
                    this.characterMoveRight.SetValue(this.follower, true);
                    break;
                case 2:
                    this.characterMoveUp.SetValue(this.follower, false);
                    this.characterMoveDown.SetValue(this.follower, true);
                    this.characterMoveLeft.SetValue(this.follower, false);
                    this.characterMoveRight.SetValue(this.follower, false);
                    break;
                case 3:
                    this.characterMoveUp.SetValue(this.follower, false);
                    this.characterMoveDown.SetValue(this.follower, false);
                    this.characterMoveLeft.SetValue(this.follower, true);
                    this.characterMoveRight.SetValue(this.follower, false);
                    break;
            }
        }

        private void HandleWallSliding()
        {
            if (this.lastFrameVelocity != Vector2.Zero && this.lastFrameMovement == Vector2.Zero &&
                (this.follower.xVelocity != 0 || this.follower.yVelocity != 0))
            {
                Rectangle wbBB = this.follower.GetBoundingBox();
                int ts = Game1.tileSize;
                bool xBlocked, yBlocked;
                xBlocked = yBlocked = false;

                if (this.follower.xVelocity != 0)
                {
                    int velocitySign = Math.Sign(this.follower.xVelocity) * 15;
                    int leftOrRight = ((this.follower.xVelocity > 0 ? wbBB.Right : wbBB.Left) + velocitySign) / ts;
                    bool[] xTiles = new bool[3];
                    xTiles[0] = this.pathFinder.IsWalkableTile(new Vector2(leftOrRight, wbBB.Top / ts));
                    xTiles[1] = this.pathFinder.IsWalkableTile(new Vector2(leftOrRight, wbBB.Center.Y / ts));
                    xTiles[2] = this.pathFinder.IsWalkableTile(new Vector2(leftOrRight, wbBB.Bottom / ts));
                    foreach (bool b in xTiles)
                    {
                        if (!b)
                        {
                            this.follower.xVelocity *= -0.25f;
                            xBlocked = true;
                            break;
                        }
                    }
                }

                if (this.follower.yVelocity != 0)
                {
                    int velocitySign = Math.Sign(this.follower.yVelocity) * 15;
                    int topOrBottom = ((this.follower.yVelocity < 0 ? wbBB.Bottom : wbBB.Top) - velocitySign) / ts;
                    bool[] yTiles = new bool[3];
                    yTiles[0] = this.pathFinder.IsWalkableTile(new Vector2(wbBB.Left / ts, topOrBottom));
                    yTiles[1] = this.pathFinder.IsWalkableTile(new Vector2(wbBB.Center.X / ts, topOrBottom));
                    yTiles[2] = this.pathFinder.IsWalkableTile(new Vector2(wbBB.Right / ts, topOrBottom));
                    foreach (bool b in yTiles)
                    {
                        if (!b)
                        {
                            this.follower.yVelocity *= -0.25f;
                            yBlocked = true;
                            break;
                        }
                    }
                }

                if (xBlocked)
                    this.follower.yVelocity *= 2.5f;
                else if (yBlocked)
                    this.follower.xVelocity *= 2.5f;
            }
        }

        private void HandleGates()
        {
            if (this.gatesInThisLocation && (this.follower.xVelocity != 0 || this.follower.yVelocity != 0))
            {
                GameLocation l = this.follower.currentLocation;
                Vector2 velocity = new Vector2(this.follower.xVelocity, -this.follower.yVelocity);
                velocity.Normalize();
                velocity = velocity * 64 * 1.3f;
                Vector2 bbVector = new Vector2(this.follower.GetBoundingBox().Center.X, this.follower.GetBoundingBox().Center.Y);
                Vector2 tile = this.follower.getTileLocation();
                Vector2 tileAhead = (bbVector + velocity) / 64;
                Vector2 tileBehind = (bbVector - velocity) / 64;
                Vector2 tileBehindNeighbor1, tileBehindNeighbor2;

                if (Math.Abs(velocity.X) > Math.Abs(velocity.Y))
                {
                    tileBehindNeighbor1 = tileBehind + new Vector2(-Math.Sign(velocity.X), 1);
                    tileBehindNeighbor2 = tileBehind + new Vector2(-Math.Sign(velocity.X), -1);
                }
                else
                {
                    tileBehindNeighbor1 = tileBehind + new Vector2(1, -Math.Sign(velocity.Y));
                    tileBehindNeighbor2 = tileBehind + new Vector2(-1, -Math.Sign(velocity.Y));
                }
                Fence[] fences = new Fence[5];
                fences[0] = (l.getObjectAtTile((int)tile.X, (int)tile.Y)) as Fence;
                fences[1] = (l.getObjectAtTile((int)tileAhead.X, (int)tileAhead.Y)) as Fence;
                fences[2] = (l.getObjectAtTile((int)tileBehind.X, (int)tileBehind.Y)) as Fence;
                fences[3] = (l.getObjectAtTile((int)tileBehindNeighbor1.X, (int)tileBehindNeighbor1.Y)) as Fence;
                fences[4] = (l.getObjectAtTile((int)tileBehindNeighbor2.X, (int)tileBehindNeighbor2.Y)) as Fence;

                if (fences[0] != null && fences[0].isGate.Value && fences[0].gatePosition.Value == 0)
                {
                    fences[0].gatePosition.Value = 88;
                    l.playSound("doorClose");
                }
                else if (fences[1] != null && fences[1].isGate.Value && fences[1].gatePosition.Value == 0)
                {
                    fences[1].gatePosition.Value = 88;
                    l.playSound("doorClose");
                }
                else if (fences[2] != null && fences[2].isGate.Value && fences[2].gatePosition.Value == 88)
                {
                    fences[2].gatePosition.Value = 0;
                    l.playSound("doorClose");
                }
                else if (fences[3] != null && fences[3].isGate.Value && fences[3].gatePosition.Value == 88)
                {
                    fences[3].gatePosition.Value = 0;
                    l.playSound("doorClose");
                }
                else if (fences[4] != null && fences[4].isGate.Value && fences[4].gatePosition.Value == 88)
                {
                    fences[4].gatePosition.Value = 0;
                    l.playSound("doorClose");
                }
            }
        }

        private bool CheckForGatesInLocation(GameLocation location)
        {
            foreach (Vector2 o in location.Objects.Keys)
                if (location.Objects[o] is Fence f && f.isGate.Value)
                    return true;

            return false;
        }

        public class MoveEventArgs
        {
            public MoveEventArgs(bool isLastFrame)
            {
                this.IsLastFrame = isLastFrame;
            }

            public bool IsLastFrame { get; }
        }

        public class EndOfRouteReachedEventArgs
        {
            public EndOfRouteReachedEventArgs(Vector2 endTile)
            {
                this.EndTile = endTile;
            }

            public Vector2 EndTile { get; }
        }
    }
}

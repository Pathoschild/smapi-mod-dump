using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using NpcAdventure.Utils;
using StardewModdingAPI;
using System.Reflection;

namespace NpcAdventure.AI.Controller
{
    public class FollowController : IController
    {
        public const int FOLLOWING_LOST_TIMEOUT = 15;
        public const float SPEEDUP_DISTANCE_THRESHOLD = 7;
        public const float MOVE_THRESHOLD_DISTANCE = 2.65f;
        public const float DECELERATE_THRESHOLD = 1.35f;
        public const float LOST_DISTANCE = 16;
        public const float OUT_OF_RANGE_DISTANCE = 64;
        public const int PATH_MAX_NODE_COUNT = 28;
        public const float PATH_NODE_TOLERANCE = 3f;
        public const float DECELERATION = 0.025f;

        private Vector2 negativeOne = new Vector2(-1, -1);

        public Character leader;
        public NPC follower;
        protected PathFinder pathFinder;
        public int followingLostTime = 0;
        public Queue<Vector2> pathToFollow;
        protected readonly AI_StateMachine ai;
        private FieldInfo characterMoveUp;
        private FieldInfo characterMoveDown;
        private FieldInfo characterMoveLeft;
        private FieldInfo characterMoveRight;
        public Vector2 currentFollowedPoint;
        public Vector2 leaderLastTileCheckPoint;
        private bool gatesInThisLocation;
        private Vector2 lastFrameMovement;
        private Vector2 lastFrameVelocity;
        private Vector2 lastFramePosition;
        private Vector2 lastMovementDirection;
        private bool movedLastFrame;
        protected float speed;
        private int switchDirectionSpeed;
        private int facingDirection;
        private Vector2 animationUpdateSum;

        public virtual bool IsIdle => false;

        public FollowController(AI_StateMachine ai)
        {
            this.pathToFollow = new Queue<Vector2>();
            this.ai = ai;
            this.leader = ai.player;
            this.follower = ai.npc;
            this.pathFinder = new PathFinder(this.follower.currentLocation, this.follower, this.leader);

            this.characterMoveUp = typeof(Character).GetField("moveUp", BindingFlags.NonPublic | BindingFlags.Instance);
            this.characterMoveDown = typeof(Character).GetField("moveDown", BindingFlags.NonPublic | BindingFlags.Instance);
            this.characterMoveLeft = typeof(Character).GetField("moveLeft", BindingFlags.NonPublic | BindingFlags.Instance);
            this.characterMoveRight = typeof(Character).GetField("moveRight", BindingFlags.NonPublic | BindingFlags.Instance);

            this.ai.LocationChanged += this.Ai_LocationChanged;

            this.gatesInThisLocation = this.CheckForGatesInThisLocation();
        }

        private void Ai_LocationChanged(object sender, EventArgsLocationChanged e)
        {
            // We are not active controller? Don't handle changed location event
            if (this.ai.CurrentController != this)
                return;

            this.pathFinder.GameLocation = e.CurrentLocation;
            this.currentFollowedPoint = this.negativeOne;
            this.leaderLastTileCheckPoint = this.negativeOne;
            this.gatesInThisLocation = this.CheckForGatesInThisLocation();
            this.PathfindingRemakeCheck();
        }

        public virtual void Update(UpdateTickedEventArgs e)
        {
            if (this.follower == null || this.leader == null || !Context.CanPlayerMove)
                return;

            this.PathfindingNodeUpdateCheck();

            if (this.currentFollowedPoint != this.negativeOne)
                this.FollowTile(this.currentFollowedPoint);

            if (e.IsMultipleOf(15))
                this.PathfindingRemakeCheck();
        }

        protected virtual void AnimationSubUpdate()
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

        protected int GetFacingDirectionFromMovement(Vector2 movement)
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

        protected void SetMovementDirectionAnimation(int dir)
        {
            if (dir < 0 || dir > 3)
                return;

            this.SetMovingOnlyOneDirection(dir);

            switch (dir)
            {
                case 0:
                    this.follower.Sprite.AnimateUp(Game1.currentGameTime, 0, ""); break;
                case 1:
                    this.follower.Sprite.AnimateRight(Game1.currentGameTime, 0, ""); break;
                case 2:
                    this.follower.Sprite.AnimateDown(Game1.currentGameTime, 0, ""); break;
                case 3:
                    this.follower.Sprite.AnimateLeft(Game1.currentGameTime, 0, ""); break;
            }
        }
        protected void SetMovingOnlyOneDirection(int dir)
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

        protected virtual void PathfindingRemakeCheck()
        {
            Vector2 leaderCurrentTile = this.leader.getTileLocation();

            if (this.pathFinder.GameLocation != this.leader.currentLocation) {
                this.pathFinder.GameLocation = this.leader.currentLocation;
            }

            if (this.leaderLastTileCheckPoint != leaderCurrentTile)
            {
                this.pathToFollow = this.pathFinder.Pathfind(this.follower.getTileLocation(), leaderCurrentTile);

                if (this.pathToFollow != null && this.pathToFollow.Count > 0 && this.follower.getTileLocation() != this.pathToFollow.Peek())
                    this.currentFollowedPoint = this.pathToFollow.Dequeue();
                else
                    this.currentFollowedPoint = this.negativeOne;
            }

            this.leaderLastTileCheckPoint = leaderCurrentTile;
        }

        protected void PathfindingNodeUpdateCheck()
        {
            if (this.currentFollowedPoint != this.negativeOne && this.pathToFollow != null)
            {
                Point w = this.follower.GetBoundingBox().Center;
                Point n = new Point(((int)this.currentFollowedPoint.X * 64) + 32, ((int)this.currentFollowedPoint.Y * 64) + 32);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                float tolerance = PATH_NODE_TOLERANCE + (this.speed > 5.3f ? this.speed - 5.28f : 0f);
                if (nodeDiffLen <= tolerance)
                {
                    if (this.pathToFollow.Count == 0)
                    {
                        this.follower.Sprite.StopAnimation();
                        this.pathToFollow = null;
                        this.currentFollowedPoint = this.negativeOne;
                        return;
                    }
                    this.currentFollowedPoint = this.pathToFollow.Dequeue();
                }
            }
        }

        protected virtual float GetMovementSpeedBasedOnDistance(float distanceFromFarmer)
        {
            if (distanceFromFarmer > MOVE_THRESHOLD_DISTANCE * Game1.tileSize * 4)
            {
                if (this.leader is Farmer farmer && farmer.getMovementSpeed() > 5.28f)
                    return farmer.getMovementSpeed() + 2.65f;

                return 5.28f;
            }

            if (distanceFromFarmer > MOVE_THRESHOLD_DISTANCE * Game1.tileSize)
            {
                if (this.leader is Farmer farmer)
                    return farmer.getMovementSpeed();
                return 4f;
            }
            else if (distanceFromFarmer > DECELERATE_THRESHOLD * Game1.tileSize)
            {
                return this.speed - DECELERATION;
            }

            return 0;
        }

        public void FollowTile(Vector2 endPointTile)
        {
            Rectangle tileBox = new Rectangle((int)endPointTile.X * 64, (int)endPointTile.Y * 64, 64, 64);
            tileBox.Inflate(-2, 0);
            Point fp = this.follower.GetBoundingBox().Center;
            Point lp = this.leader.GetBoundingBox().Center;
            this.lastFrameMovement = new Vector2(fp.X, fp.Y) - this.lastFramePosition;

            Vector2 diff = new Vector2(lp.X, lp.Y) - new Vector2(fp.X, fp.Y);
            this.speed = this.GetMovementSpeedBasedOnDistance(diff.Length());

            if (this.speed > 0)
            {
                Point np = new Point(((int)endPointTile.X * 64) + 32, ((int)endPointTile.Y * 64) + 32);
                Vector2 nodeDiff = new Vector2(np.X, np.Y) - new Vector2(fp.X, fp.Y);
                float nodeDiffLen = nodeDiff.Length();

                if (nodeDiffLen <= PATH_NODE_TOLERANCE)
                    return;

                nodeDiff /= nodeDiffLen;

                this.follower.xVelocity = nodeDiff.X * this.speed;
                this.follower.yVelocity = -nodeDiff.Y * this.speed;
                this.HandleWallSliding();
                this.HandleGates();

                this.lastFrameVelocity = new Vector2(this.follower.xVelocity, this.follower.yVelocity);
                this.lastFramePosition = new Vector2(fp.X, fp.Y);

                this.animationUpdateSum += new Vector2(this.follower.xVelocity, -this.follower.yVelocity);
                this.AnimationSubUpdate();

                this.follower.MovePosition(Game1.currentGameTime, Game1.viewport, this.follower.currentLocation); // Update follower movement
                this.lastMovementDirection = this.lastFrameVelocity / this.lastFrameVelocity.Length();

                this.movedLastFrame = true;
            }
            else if (this.movedLastFrame)
            {
                this.follower.Halt();
                this.follower.Sprite.faceDirectionStandard(this.GetFacingDirectionFromMovement(new Vector2(this.lastMovementDirection.X, -this.lastMovementDirection.Y)));
                this.movedLastFrame = false;
            }
            else
            {
                this.follower.xVelocity = 0;
                this.follower.yVelocity = 0;
            }
        }

        protected void HandleWallSliding()
        {
            if (this.lastFrameVelocity != Vector2.Zero && this.lastFrameMovement == Vector2.Zero &&
                (this.follower.xVelocity != 0 || this.follower.yVelocity != 0))
            {
                Rectangle wbBB = this.follower.GetBoundingBox();
                GameLocation location = this.follower.currentLocation;
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

        protected void HandleGates()
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
                bool neighborsUpDown = Math.Abs(velocity.X) > Math.Abs(velocity.Y);
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

        private bool CheckForGatesInThisLocation()
        {
            foreach (Vector2 o in this.follower.currentLocation.Objects.Keys)
                if (this.follower.currentLocation.Objects[o] is Fence f && f.isGate.Value)
                    return true;

            return false;
        }

        public virtual void Activate()
        {
            if (this.leader != null)
            {
                this.PathfindingRemakeCheck();
            }
        }

        public virtual void Deactivate() {}
    }
}

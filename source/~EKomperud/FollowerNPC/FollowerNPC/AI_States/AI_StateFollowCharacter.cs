using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using StardewValley;
using StardewValley.Monsters;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace FollowerNPC.AI_States
{
    class AI_StateFollowCharacter: AI_State
    {
        protected Character me;
        protected Character leader;
        protected bool leaderIsFarmer;
        protected aStar aStar;
        protected Character originalLeader;
        protected Monster aggroMonster;
        protected AI_StateMachine machine;

        protected Queue<Vector2> path;
        protected Vector2 currentPathNode;
        public float pathNodeTolerance;

        protected Vector2 targetLastTile;

        private bool monsterAggroed;

        // Companion Parameters //
        public int switchDirectionSpeed;
        public int facingDirection;
        public float followThreshold;
        public float decelerateThreshold;
        public float deceleration;
        public float currentMovespeed;

        protected float monsterAggroRadius;
        // ******************** //

        // Companion Movement Memory //
        public Vector2 lastMovementDirection;
        public Vector2 lastFrameVelocity;
        public Vector2 lastFramePosition;
        public Vector2 lastFrameMovement;
        public Vector2 animationUpdateSum;
        public bool movedLastFrame;
        // ************************* //

        // Constants //
        public Vector2 negativeOne = new Vector2(-1, -1);
        public int fullTile = Game1.tileSize;
        public int halfTile = Game1.tileSize / 2;
        // ********* //

        // Reflection Variables //
        public FieldInfo characterMoveUp;
        public FieldInfo characterMoveDown;
        public FieldInfo characterMoveLeft;
        public FieldInfo characterMoveRight;
        // ******************** //

        public AI_StateFollowCharacter(Character me, Character leader, AI_StateMachine machine)
        {
            this.me = me;
            this.leader = leader;
            Farmer f = leader as Farmer;
            if (f != null)
                leaderIsFarmer = true;
            this.machine = machine;

            aStar = new aStar(me.currentLocation, me, leader);

            followThreshold = 2.25f * fullTile;
            decelerateThreshold = 1.75f * fullTile;
            deceleration = 0.075f;
            pathNodeTolerance = 5f;
            monsterAggroRadius = 6f * fullTile;

            characterMoveUp = typeof(Character).GetField("moveUp", BindingFlags.NonPublic | BindingFlags.Instance);
            characterMoveDown = typeof(Character).GetField("moveDown", BindingFlags.NonPublic | BindingFlags.Instance);
            characterMoveLeft = typeof(Character).GetField("moveLeft", BindingFlags.NonPublic | BindingFlags.Instance);
            characterMoveRight = typeof(Character).GetField("moveRight", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override void EnterState()
        {
            aStar.gameLocation = leader.currentLocation;
            ModEntry.modHelper.Events.World.DebrisListChanged += World_DebrisListChanged;
            ModEntry.modHelper.Events.World.ObjectListChanged += World_ObjectListChanged;
            ModEntry.modHelper.Events.World.TerrainFeatureListChanged += World_TerrainFeatureListChanged;
            ModEntry.modHelper.Events.Player.Warped += Player_Warped;
        }

        public override void ExitState()
        {
            ModEntry.modHelper.Events.World.DebrisListChanged -= World_DebrisListChanged;
            ModEntry.modHelper.Events.World.ObjectListChanged -= World_ObjectListChanged;
            ModEntry.modHelper.Events.World.TerrainFeatureListChanged -= World_TerrainFeatureListChanged;
            ModEntry.modHelper.Events.Player.Warped -= Player_Warped;
        }

        public override void Update(UpdateTickedEventArgs e)
        {
            me.farmerPassesThrough = true;
            PathfindingNodeUpdateCheck();
            MovementAndAnimationUpdate();
            aggroMonster = CheckAggroRadiusForMonsters();
            eAI_State potentialNewState = TransitionsCheck();
            if (potentialNewState != eAI_State.nil)
            {
                machine.ChangeState(potentialNewState);
                return;
            }
            if (e.IsMultipleOf(15))
            {
                PathfindingRemakeCheck();
            }
        }

        private eAI_State TransitionsCheck()
        {
            if (monsterAggroed)
            {
                monsterAggroed = false;
                AI_StateAggroEnemy monsterAggroedState = (AI_StateAggroEnemy)machine.states[(int) eAI_State.aggroEnemy];
                monsterAggroedState.SetMonster(aggroMonster);
                return eAI_State.aggroEnemy;
            }

            return eAI_State.nil;
        }

        public void SetCharacterToFollow(Character c)
        {
            leader = c;
            Farmer f = leader as Farmer;
            if (f != null)
                leaderIsFarmer = true;
        }

        /// <summary>
        /// Remakes our path if the farmer has changed tiles since the last time
        /// this function was called. (Every quarter second as of right now)
        /// </summary>
        protected virtual void PathfindingRemakeCheck()
        {
            Vector2 leaderCurrentTile = leader.getTileLocation();

            if (targetLastTile != leaderCurrentTile)
            {
                path = aStar.Pathfind(me.getTileLocation(), leaderCurrentTile);
                //if (me.getTileLocation() != currentPathNode)
                //    currentPathNode = path != null && path.Count != 0 ? path.Dequeue() : negativeOne;
                if (path != null && path.Count != 0 && me.getTileLocation() != path.Peek())
                    currentPathNode = path.Dequeue();
                else
                    currentPathNode = negativeOne;
            }

            targetLastTile = leaderCurrentTile;
        }

        /// <summary>
        /// Iterates to the next goal node in our current path if the current
        /// goal node has been reached since the last time this function was called.
        /// (Every 1/60 of a second as of right now)
        /// </summary>
        protected void PathfindingNodeUpdateCheck()   
        {
            if (currentPathNode != negativeOne && path != null)
            {
                Point w = me.GetBoundingBox().Center;
                Point n = new Point(((int)currentPathNode.X * fullTile) + halfTile, ((int)currentPathNode.Y * Game1.tileSize) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                float tolerance = pathNodeTolerance + (currentMovespeed > 5.3f ? currentMovespeed - 5.28f : 0f);
                if (nodeDiffLen <= tolerance)
                {
                    if (path.Count == 0)
                    {
                        path = null;
                        currentPathNode = negativeOne;
                        return;
                    }
                    currentPathNode = path.Dequeue();
                    n = new Point(((int)currentPathNode.X * fullTile) + halfTile, ((int)currentPathNode.Y * fullTile) + halfTile);
                    nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                    nodeDiffLen = nodeDiff.Length();
                }
            }
        }

        /// <summary>
        /// Provides updates to the companion's movement.
        /// </summary>
        protected virtual void MovementAndAnimationUpdate()
        {
            Point f = leader.GetBoundingBox().Center;
            Point c = me.GetBoundingBox().Center;
            Vector2 companionBoundingBox = new Vector2(c.X, c.Y);
            lastFrameMovement = companionBoundingBox - lastFramePosition;

            Vector2 diff = new Vector2(f.X, f.Y) - new Vector2(c.X, c.Y);
            float diffLen = diff.Length();
            currentMovespeed = GetMovementSpeedBasedOnDistance(diffLen);
            if (currentMovespeed > 0 && currentPathNode != negativeOne)
            {
                Point n = new Point(((int)currentPathNode.X * fullTile) + halfTile, ((int)currentPathNode.Y * fullTile) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(c.X, c.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= pathNodeTolerance)
                    return;
                nodeDiff /= nodeDiffLen;

                
                me.xVelocity = nodeDiff.X * currentMovespeed;
                me.yVelocity = -nodeDiff.Y * currentMovespeed;
                //if (me.xVelocity != 0 && me.yVelocity != 0)
                //{
                //    me.xVelocity *= 1.05f;
                //    me.yVelocity *= 1.05f;
                //}
                HandleWallSliding();
                lastFrameVelocity = new Vector2(me.xVelocity, me.yVelocity);
                lastFramePosition = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);

                animationUpdateSum += new Vector2(me.xVelocity, -me.yVelocity);
                AnimationSubUpdate();
                me.MovePosition(Game1.currentGameTime, Game1.viewport, me.currentLocation);
                lastMovementDirection = lastFrameVelocity / lastFrameVelocity.Length();

                movedLastFrame = true;
            }
            else if (movedLastFrame)
            {
                me.Halt();
                me.Sprite.faceDirectionStandard(GetFacingDirectionFromMovement(new Vector2(lastMovementDirection.X, -lastMovementDirection.Y)));
                movedLastFrame = false;
            }
        }

        /// <summary>
        /// Returns what the current movement speed should be, based on distance from
        /// the leader.
        /// </summary>
        protected float GetMovementSpeedBasedOnDistance(float distanceFromFarmer)
        {
            if (distanceFromFarmer > followThreshold)
            {
                if (leaderIsFarmer)
                    return (leader as Farmer).getMovementSpeed();
                // 5.28f
                return 4f;
            }
            else if (distanceFromFarmer > decelerateThreshold)
            {
                return currentMovespeed - 0.075f;
            }
            return 0;
        }

        /// <summary>
        /// Provides updates to the companion's animation;
        /// </summary>
        protected virtual void AnimationSubUpdate()
        {
            if (++switchDirectionSpeed == 5)
            {
                facingDirection = GetFacingDirectionFromMovement(animationUpdateSum);
                animationUpdateSum = Vector2.Zero;
                switchDirectionSpeed = 0;
            }

            if (facingDirection >= 0)
            {
                me.faceDirection(facingDirection);
                SetMovementDirectionAnimation(facingDirection);
            }
        }

        /// <summary>
        /// Allows the Companion to "slide" along walls instead of getting stuck on them
        /// </summary>
        protected void HandleWallSliding()
        {
            if (lastFrameVelocity != Vector2.Zero && lastFrameMovement == Vector2.Zero &&
                (me.xVelocity != 0 || me.yVelocity != 0))
            {
                Rectangle wbBB = me.GetBoundingBox();
                int ts = Game1.tileSize;

                if (me.xVelocity != 0)
                {
                    int velocitySign = Math.Sign(me.xVelocity) * 5;
                    int leftOrRight = ((me.xVelocity > 0 ? wbBB.Right : wbBB.Left) + velocitySign) / ts;
                    bool[] xTiles = new bool[3];
                    xTiles[0] = aStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Top / ts));
                    xTiles[1] = aStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Center.Y / ts));
                    xTiles[2] = aStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Bottom / ts));
                    foreach (bool b in xTiles)
                    {
                        if (!b)
                            me.xVelocity = 0;
                    }
                }

                if (me.yVelocity != 0)
                {
                    int velocitySign = Math.Sign(me.yVelocity) * 5;
                    int topOrBottom = ((me.yVelocity < 0 ? wbBB.Bottom : wbBB.Top) - velocitySign) / ts;
                    bool[] yTiles = new bool[3];
                    yTiles[0] = aStar.IsWalkableTile(new Vector2(wbBB.Left / ts, topOrBottom));
                    yTiles[1] = aStar.IsWalkableTile(new Vector2(wbBB.Center.X / ts, topOrBottom));
                    yTiles[2] = aStar.IsWalkableTile(new Vector2(wbBB.Right / ts, topOrBottom));
                    foreach (bool b in yTiles)
                    {
                        if (!b)
                            me.yVelocity = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the proper animation for the Companion based on their direction.
        /// </summary>
        protected void SetMovementDirectionAnimation(int dir)
        {
            if (dir < 0 || dir > 3)
                return;
            SetMovingOnlyOneDirection(dir);
            switch (dir)
            {
                case 0:
                    me.Sprite.AnimateUp(Game1.currentGameTime, 0, ""); break;
                case 1:
                    me.Sprite.AnimateRight(Game1.currentGameTime, 0, ""); break;
                case 2:
                    me.Sprite.AnimateDown(Game1.currentGameTime, 0, ""); break;
                case 3:
                    me.Sprite.AnimateLeft(Game1.currentGameTime, 0, ""); break;
            }
        }

        /// <summary>
        /// Returns an int 0, 1, 2, or 3, representing the direction the Companion should face 
        /// based on their current velocity. North is 0, and the rest follow clockwise.
        /// </summary>
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

        protected void SetMovingOnlyOneDirection(int dir)
        {
            if (dir < 0 || dir > 3)
                return;

            switch (dir)
            {
                case 0:
                    characterMoveUp.SetValue(me, true);
                    characterMoveDown.SetValue(me, false);
                    characterMoveLeft.SetValue(me, false);
                    characterMoveRight.SetValue(me, false);
                    break;
                case 1:
                    characterMoveUp.SetValue(me, false);
                    characterMoveDown.SetValue(me, false);
                    characterMoveLeft.SetValue(me, false);
                    characterMoveRight.SetValue(me, true);
                    break;
                case 2:
                    characterMoveUp.SetValue(me, false);
                    characterMoveDown.SetValue(me, true);
                    characterMoveLeft.SetValue(me, false);
                    characterMoveRight.SetValue(me, false);
                    break;
                case 3:
                    characterMoveUp.SetValue(me, false);
                    characterMoveDown.SetValue(me, false);
                    characterMoveLeft.SetValue(me, true);
                    characterMoveRight.SetValue(me, false);
                    break;
            }
        }

        protected Monster CheckAggroRadiusForMonsters()
        {
            Monster aggroMonster = null;
            float aggroMonsterDistance = float.PositiveInfinity; 
            Vector2 i = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);
            foreach (Character c in aStar.gameLocation.characters)
            {
                Monster asMonster = c as Monster;
                if (asMonster != null && asMonster.currentLocation != null)
                {
                    Vector2 m = new Vector2(asMonster.GetBoundingBox().Center.X, asMonster.GetBoundingBox().Center.Y);
                    float distance = (m - i).Length();
                    if (distance <= monsterAggroRadius && distance < aggroMonsterDistance)
                    {
                        aggroMonster = asMonster;
                        aggroMonsterDistance = distance;
                        monsterAggroed = true;
                    }
                }
            }
            return aggroMonster;
        }

        #region Events
        protected void World_DebrisListChanged(object sender, DebrisListChangedEventArgs e)
        {
            PathfindingRemakeCheck();
        }

        protected void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            PathfindingRemakeCheck();
        }

        protected void World_TerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
        {
            PathfindingRemakeCheck();
        }

        protected void Player_Warped(object sender, WarpedEventArgs e)
        {

            aStar.gameLocation = leader.currentLocation;
        }
        #endregion
    }
}

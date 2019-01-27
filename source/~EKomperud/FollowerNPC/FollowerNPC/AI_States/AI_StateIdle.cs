using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace FollowerNPC.AI_States
{
    public partial class AI_StateIdle: AI_State
    {
        private Character me;
        private Character leader;
        private AI_StateMachine stateMachine;
        public aStar aStar { get; private set; }
        public  Random r { get; private set; }
        private float followRadius;

        private IdleBehavior currentIdleBehavior;
        private IdleBehavior transitioningBehavior;
        private IdleBehavior nextIdleBehavior;
        private IdleBehavior[] idleBehaviors;
        private float[] behaviorTendencies;
        private int behaviorIndex;
        private string subBehaviorIndex;

        private Dialogue idleDialogue;
        private bool idleDialogueSeen;

        public AI_StateIdle(Character me, Character leader, AI_StateMachine machine)
        {
            this.me = me;
            this.leader = leader;
            this.stateMachine = machine;
            this.r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + Game1.timeOfDay);
            this.followRadius = (7.5f * Game1.tileSize) * (7.5f * Game1.tileSize);
            aStar = new aStar(me.currentLocation, me, leader);
            transitioningBehavior = new TransitioningBehavior(this, 1, 2);

            switch (me.Name)
            {
                case "Abigail":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 12, 16);
                    idleBehaviors[1] = new WanderBehavior(this, 10, 15, 3, 5, 11, 5);
                    idleBehaviors[2] = new LookAroundBehavior(this, 2, 5);
                    behaviorTendencies = new float[3] { 5f, 3f, 2f };
                    break;
                case "Alex":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 12, 15);
                    idleBehaviors[1] = new WanderBehavior(this, 8, 15, 4, 6, 10, 5);
                    idleBehaviors[2] = new LookAroundBehavior(this, 2, 5);
                    behaviorTendencies = new float[3] { 2.5f, 5f, 2.5f };
                    break;
                case "Elliott":
                    idleBehaviors = new IdleBehavior[4];
                    idleBehaviors[0] = new AnimateBehavior(this, 15, 20);
                    idleBehaviors[1] = new WanderBehavior(this, 9, 12, 5, 6, 10, 3.5f);
                    idleBehaviors[2] = new LookAroundBehavior(this, 4, 8);
                    idleBehaviors[3] = new FishBehavior(this, 20, 30, 31, 32, 6, 3.5f);
                    behaviorTendencies = new float[4] { 3f, 3f, 3f, 3f };
                    break;
                case "Emily":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 16, 22);
                    idleBehaviors[1] = new WanderBehavior(this, 9, 12, 5, 8, 12, 4);
                    idleBehaviors[2] = new LookAroundBehavior(this, 4, 8);
                    behaviorTendencies = new float[3] { 3f, 3f, 3f };
                    break;
                case "Haley":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 6, 12);
                    idleBehaviors[1] = new WanderBehavior(this, 4, 6, 7, 8, 6, 4);
                    idleBehaviors[2] = new LookAroundBehavior(this, 6, 9);
                    behaviorTendencies = new float[3] { 4f, 2f, 2f };
                    break;
                case "Harvey":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 16, 22);
                    idleBehaviors[1] = new WanderBehavior(this, 4, 6, 7, 8, 6, 3);
                    idleBehaviors[2] = new LookAroundBehavior(this, 8, 12);
                    behaviorTendencies = new float[3] { 5f, 1f, 3f };
                    break;
                case "Leah":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 8, 12);
                    idleBehaviors[1] = new WanderBehavior(this, 9, 12, 6, 12, 10, 4);
                    idleBehaviors[2] = new LookAroundBehavior(this, 6, 10);
                    behaviorTendencies = new float[3] { 3f, 4.5f, 2.5f };
                    break;
                case "Maru":
                    idleBehaviors = new IdleBehavior[2];
                    idleBehaviors[0] = new WanderBehavior(this, 9, 12, 6, 12, 12, 3.5f);
                    idleBehaviors[1] = new LookAroundBehavior(this, 10, 12);
                    behaviorTendencies = new float[2] { 2f, 2f };
                    break;
                case "Penny":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 16, 22);
                    idleBehaviors[1] = new WanderBehavior(this, 3, 4, 5, 6, 6, 2.5f);
                    idleBehaviors[2] = new LookAroundBehavior(this, 6, 8);
                    behaviorTendencies = new float[3] { 7f, 1.5f, 1.5f };
                    break;
                case "Sam":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 6, 12);
                    idleBehaviors[1] = new WanderBehavior(this, 9, 12, 4, 6, 10, 5);
                    idleBehaviors[2] = new LookAroundBehavior(this, 6, 10);
                    behaviorTendencies = new float[3] { 3f, 3f, 3f };
                    break;
                case "Sebastian":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 10, 20);
                    idleBehaviors[1] = new WanderBehavior(this, 8, 12, 13, 14, 12, 3.5f);
                    idleBehaviors[2] = new LookAroundBehavior(this, 8, 12);
                    behaviorTendencies = new float[3] { 2f, 3.5f, 4.5f };
                    break;
                case "Shane":
                    idleBehaviors = new IdleBehavior[3];
                    idleBehaviors[0] = new AnimateBehavior(this, 8, 12);
                    idleBehaviors[1] = new WanderBehavior(this, 4, 6, 7, 8, 8, 3);
                    idleBehaviors[2] = new LookAroundBehavior(this, 4, 6);
                    behaviorTendencies = new float[3] { 7f, 1.5f, 1.5f };
                    break;
            }

        }

        public override void EnterState()
        {
            base.EnterState();
            aStar.gameLocation = leader.currentLocation;
            ModEntry.modHelper.Events.Player.Warped += Player_Warped;
            behaviorIndex = ChooseIdleBehavior();
            if (!idleBehaviors[behaviorIndex].CanPerformThisBehavior())
                behaviorIndex = ++behaviorIndex >= idleBehaviors.Length ? 0 : behaviorIndex;
            currentIdleBehavior = idleBehaviors[behaviorIndex];
            currentIdleBehavior.StartBehavior();
            if (idleBehaviors[behaviorIndex] is AnimateBehavior)
                subBehaviorIndex = (idleBehaviors[behaviorIndex] as AnimateBehavior).animationIndexChar + "";
            else
                subBehaviorIndex = "";

            
            if (!idleDialogueSeen)
            {
                if (idleDialogue != null)
                    TryRemoveIdleDialogue();
                Dialogue d = stateMachine.owner.stateMachine.manager.GenerateDialogue("Idle"+(behaviorIndex+1)+subBehaviorIndex, me.Name, true);
                idleDialogue =
                    d ?? throw new Exception(
                        "Tried to push an idle dialogue, but there weren't any for this character!");
                stateMachine.owner.stateMachine.companion.CurrentDialogue.Push(d);
            }
            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            aStar.gameLocation = leader.currentLocation;
        }

        public override void ExitState()
        {
            base.ExitState();
            ModEntry.modHelper.Events.Display.MenuChanged -= Display_MenuChanged;
            TryRemoveIdleDialogue();
        }

        public override void Update(UpdateTickedEventArgs e)
        {
            eAI_State potentialNewState = TransitionsCheck();
            if (potentialNewState != eAI_State.nil)
            {
                currentIdleBehavior.StopBehavior();
                stateMachine.ChangeState(potentialNewState);
            }

            currentIdleBehavior.Update(e);
        }

        private eAI_State TransitionsCheck()
        {
            if (CheckIfLeaderLeftFollowRadius())
                return eAI_State.followFarmer;
            return eAI_State.nil;
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu != null)
            {
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && d.speaker != null && 
                        stateMachine.owner != null && d.speaker.Equals(stateMachine.owner.stateMachine.companion))
                    {
                        if (d.Equals(idleDialogue))
                        {
                            idleDialogueSeen = true;
                            idleDialogue = null;
                        }
                    }
                }
            }
        }

        private bool CheckIfLeaderLeftFollowRadius()
        {
            Vector2 i = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);
            Vector2 l = new Vector2(leader.GetBoundingBox().Center.X, leader.GetBoundingBox().Center.Y);
            return (l - i).LengthSquared() > followRadius;
        }

        private int ChooseIdleBehavior()
        {
            float t = 0f;
            foreach (float f in behaviorTendencies)
                t += f;
            float b = (float)r.NextDouble() * t;
            for (int i = 0; i < behaviorTendencies.Length; i++)
            {
                b -= behaviorTendencies[i];
                if (b <= 0f)
                    return i;
            }
            return behaviorTendencies.Length - 1;
        }

        public bool TryRemoveIdleDialogue()
        {
            if (idleDialogue == null)
                return false;
            Stack<Dialogue> temp = new Stack<Dialogue>(stateMachine.owner.stateMachine.companion.CurrentDialogue.Count);
            Dialogue t;
            bool ret = false;
            while (stateMachine.owner.stateMachine.companion.CurrentDialogue.Count != 0)
            {
                t = stateMachine.owner.stateMachine.companion.CurrentDialogue.Pop();
                if (!t.Equals(idleDialogue))
                    temp.Push(t);
                else
                    ret = true;
            }
            while (temp.Count != 0)
                stateMachine.owner.stateMachine.companion.CurrentDialogue.Push(temp.Pop());
            idleDialogue = null;
            return ret;
        }

        public void TransitionIdleState(int forceBehavior = -1)
        {
            if (currentIdleBehavior.Equals(transitioningBehavior))
            {
                currentIdleBehavior.StopBehavior();
                
                currentIdleBehavior = idleBehaviors[behaviorIndex];
                currentIdleBehavior.StartBehavior();

                if (idleBehaviors[behaviorIndex] is AnimateBehavior)
                    subBehaviorIndex = (idleBehaviors[behaviorIndex] as AnimateBehavior).animationIndexChar + "";
                else
                    subBehaviorIndex = "";
                if (!idleDialogueSeen)
                {
                    if (idleDialogue != null)
                        TryRemoveIdleDialogue();
                    Dialogue d = stateMachine.owner.stateMachine.manager.GenerateDialogue("Idle" + (behaviorIndex + 1) + subBehaviorIndex, me.Name, true);
                    idleDialogue =
                        d ?? throw new Exception(
                            "Tried to push an idle dialogue, but there weren't any for this character!");
                    stateMachine.owner.stateMachine.companion.CurrentDialogue.Push(d);
                }
            }
            else
            {
                behaviorIndex = forceBehavior == -1 ? ChooseIdleBehavior() : forceBehavior;
                while (!idleBehaviors[behaviorIndex].CanPerformThisBehavior())
                    behaviorIndex = ++behaviorIndex >= idleBehaviors.Length ? 0 : behaviorIndex;
                nextIdleBehavior = idleBehaviors[behaviorIndex];
                if (!nextIdleBehavior.Equals(currentIdleBehavior))
                {
                    currentIdleBehavior.StopBehavior();
                    currentIdleBehavior = transitioningBehavior;
                    currentIdleBehavior.StartBehavior();
                }
                else
                {
                    currentIdleBehavior.SelfTransition();
                }
            }
        }
    }

    public partial class AI_StateIdle
    {
        private class IdleBehavior
        {
            protected AI_StateIdle idleState;
            protected int minFramesBeforeTransition;
            protected int maxFramesBeforeTransition;
            protected int framesBeforeTransition;

            public IdleBehavior(AI_StateIdle idleState, int minSeconds, int maxSeconds)
            {
                this.idleState = idleState;
                minFramesBeforeTransition = minSeconds * 60;
                maxFramesBeforeTransition = maxSeconds * 60;
            }

            public virtual void StartBehavior()
            {
                framesBeforeTransition = idleState.r.Next(minFramesBeforeTransition, maxFramesBeforeTransition + 1);
            }

            public virtual void StopBehavior()
            {

            }

            public virtual void SelfTransition()
            {
                StartBehavior();
            }

            public virtual void Update(UpdateTickedEventArgs e)
            {
                if (TransitionsCheck())
                    return;
            }

            public virtual bool TransitionsCheck()
            {
                if (--framesBeforeTransition == 0)
                {
                    idleState.TransitionIdleState();
                    return true;
                }
                return true;
            }

            public virtual bool CanPerformThisBehavior()
            {
                return true;
            }
        }

        private class TransitioningBehavior : IdleBehavior
        {
            public TransitioningBehavior(AI_StateIdle idleState, int minSeconds, int maxSeconds) : base(idleState, minSeconds, maxSeconds)
            {
            }

            public override void Update(UpdateTickedEventArgs e)
            {
                base.Update(e);
                idleState.me.xVelocity = 0f;
                idleState.me.yVelocity = 0f;
                //idleState.me.Sprite.faceDirectionStandard(idleState.me.FacingDirection);
            }

            public override void SelfTransition()
            {
                
            }
        }

        private class WanderBehavior : IdleBehavior
        {
            protected Vector2 targetTile;
            protected int minRestlessness;
            protected int maxRestlessness;
            protected int restlessness;
            protected int maxTilesToWander;
            protected float speed;
            protected Action targetReachedAction;
            protected int backupBehavior = 2;

            protected int fullTile = Game1.tileSize;
            protected int halfTile = Game1.tileSize / 2;
            protected Vector2 negativeOne = new Vector2(-1, -1);

            protected Queue<Vector2> path;
            protected Vector2 currentPathNode;
            protected float pathNodeTolerance = 5f;
            protected float switchDirectionSpeed;
            protected int facingDirection;
            protected bool gatesInThisLocation;
            protected bool movedLastFrame;
            protected Vector2 lastFrameVelocity;
            protected Vector2 lastFramePosition;
            protected Vector2 lastFrameMovement;
            protected Vector2 lastMovementDirection;
            protected Vector2 animationUpdateSum;

            protected FieldInfo characterMoveUp;
            protected FieldInfo characterMoveDown;
            protected FieldInfo characterMoveLeft;
            protected FieldInfo characterMoveRight;

            public WanderBehavior(AI_StateIdle idleState, int minSeconds, int maxSeconds, int minRestlessness, int maxRestlessness, int maxTilesToWander, float speed) : base(idleState, minSeconds, maxSeconds)
            {
                this.minRestlessness = minRestlessness * 60;
                this.maxRestlessness = maxRestlessness * 60;
                this.maxTilesToWander = maxTilesToWander;
                this.speed = speed;
                this.targetReachedAction = new Action(HaltAndSetRestlessness);
            }

            public override void StartBehavior()
            {
                base.StartBehavior();
                restlessness = idleState.r.Next(minRestlessness, maxRestlessness + 1);
                targetTile = PickTile();
                if (targetTile != negativeOne)
                {
                    path = idleState.aStar.Pathfind(idleState.me.getTileLocation(), targetTile);
                    if (path != null && path.Count != 0 && idleState.me.getTileLocation() != path.Peek())
                        currentPathNode = path.Dequeue();
                    else
                        currentPathNode = negativeOne;

                    gatesInThisLocation = CheckForGatesInThisLocation();

                    characterMoveUp = typeof(Character).GetField("moveUp", BindingFlags.NonPublic | BindingFlags.Instance);
                    characterMoveDown = typeof(Character).GetField("moveDown", BindingFlags.NonPublic | BindingFlags.Instance);
                    characterMoveLeft = typeof(Character).GetField("moveLeft", BindingFlags.NonPublic | BindingFlags.Instance);
                    characterMoveRight = typeof(Character).GetField("moveRight", BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else
                {
                    idleState.TransitionIdleState();
                }
            }

            public override void Update(UpdateTickedEventArgs e)
            {
                base.Update(e);
                PathfindingNodeUpdateCheck();
                MovementAndAnimationUpdate();

                if (--restlessness == 0)
                {
                    targetTile = PickTile();
                    path = idleState.aStar.Pathfind(idleState.me.getTileLocation(), targetTile);
                    if (path != null && path.Count != 0 && idleState.me.getTileLocation() != path.Peek())
                        currentPathNode = path.Dequeue();
                    else
                        currentPathNode = negativeOne;
                }
            }

            protected virtual Vector2 PickTile()
            {
                int tilesToWalk = idleState.r.Next(2, maxTilesToWander);
                Vector2 thisTile = idleState.me.getTileLocation();
                Vector2 leaderTile = idleState.leader.getTileLocation();
                while (tilesToWalk-- > 0)
                {
                    int dir = idleState.r.Next(0, 4);
                    Vector2 nextTile;
                    switch (dir)
                    {
                        case 0:
                            nextTile = new Vector2(thisTile.X, thisTile.Y - 1); break;
                        case 1:
                            nextTile = new Vector2(thisTile.X + 1, thisTile.Y); break;
                        case 2:
                            nextTile = new Vector2(thisTile.X, thisTile.Y + 1); break;
                        case 3:
                            nextTile = new Vector2(thisTile.X - 1, thisTile.Y); break;
                        default:
                            nextTile = thisTile; break;
                    }

                    if (idleState.aStar.IsWalkableTile(nextTile) &&
                        TileIsWithinLeaderRadius(nextTile))
                        thisTile = nextTile;
                }
                return thisTile;
            }

            private bool TileIsWithinLeaderRadius(Vector2 tile)
            {
                Vector2 leaderPoint = new Vector2(idleState.leader.GetBoundingBox().Center.X,
                    idleState.leader.GetBoundingBox().Center.Y);
                Vector2 tilePoint = new Vector2(tile.X * fullTile + (halfTile),
                    tile.Y * fullTile + (halfTile));
                return (leaderPoint - tilePoint).LengthSquared() < idleState.followRadius;
            }

            private void MovementAndAnimationUpdate()
            {
                Point c = idleState.me.GetBoundingBox().Center;
                Vector2 companionBoundingBox = new Vector2(c.X, c.Y);
                lastFrameMovement = companionBoundingBox - lastFramePosition;
                if (currentPathNode != negativeOne)
                {
                    Point n = new Point(((int)currentPathNode.X * fullTile) + halfTile, ((int)currentPathNode.Y * fullTile) + halfTile);
                    Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(c.X, c.Y);
                    float nodeDiffLen = nodeDiff.Length();
                    if (nodeDiffLen <= pathNodeTolerance)
                        return;
                    nodeDiff /= nodeDiffLen;

                    idleState.me.xVelocity = nodeDiff.X * speed;
                    idleState.me.yVelocity = -nodeDiff.Y * speed;

                    lastFrameVelocity = new Vector2(idleState.me.xVelocity, idleState.me.yVelocity);
                    lastFramePosition = new Vector2(idleState.me.GetBoundingBox().Center.X, idleState.me.GetBoundingBox().Center.Y);

                    animationUpdateSum += new Vector2(idleState.me.xVelocity, -idleState.me.yVelocity);
                    AnimationSubUpdate();
                    idleState.me.MovePosition(Game1.currentGameTime, Game1.viewport, idleState.me.currentLocation);
                    lastMovementDirection = lastFrameVelocity / lastFrameVelocity.Length();

                    movedLastFrame = true;
                }
                else if (movedLastFrame)
                {
                    targetReachedAction.Invoke();
                    movedLastFrame = false;
                }
                else if (currentPathNode == negativeOne)
                {
                    idleState.me.xVelocity = 0f;
                    idleState.me.yVelocity = 0f;
                }
                else
                {
                    idleState.me.xVelocity = 0f;
                    idleState.me.yVelocity = 0f;
                }
            }

            protected void HandleWallSliding()
            {
                if (lastFrameVelocity != Vector2.Zero && lastFrameMovement == Vector2.Zero &&
                    (idleState.me.xVelocity != 0 || idleState.me.yVelocity != 0))
                {
                    Rectangle wbBB = idleState.me.GetBoundingBox();
                    int ts = Game1.tileSize;

                    if (idleState.me.xVelocity != 0)
                    {
                        int velocitySign = Math.Sign(idleState.me.xVelocity) * 15;
                        int leftOrRight = ((idleState.me.xVelocity > 0 ? wbBB.Right : wbBB.Left) + velocitySign) / ts;
                        bool[] xTiles = new bool[3];
                        xTiles[0] = idleState.aStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Top / ts));
                        xTiles[1] = idleState.aStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Center.Y / ts));
                        xTiles[2] = idleState.aStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Bottom / ts));
                        foreach (bool b in xTiles)
                        {
                            if (!b)
                                idleState.me.xVelocity = 0;
                        }
                    }

                    if (idleState.me.yVelocity != 0)
                    {
                        int velocitySign = Math.Sign(idleState.me.yVelocity) * 15;
                        int topOrBottom = ((idleState.me.yVelocity < 0 ? wbBB.Bottom : wbBB.Top) - velocitySign) / ts;
                        bool[] yTiles = new bool[3];
                        yTiles[0] = idleState.aStar.IsWalkableTile(new Vector2(wbBB.Left / ts, topOrBottom));
                        yTiles[1] = idleState.aStar.IsWalkableTile(new Vector2(wbBB.Center.X / ts, topOrBottom));
                        yTiles[2] = idleState.aStar.IsWalkableTile(new Vector2(wbBB.Right / ts, topOrBottom));
                        foreach (bool b in yTiles)
                        {
                            if (!b)
                                idleState.me.yVelocity = 0;
                        }
                    }
                }
            }

            protected void AnimationSubUpdate()
            {
                if (++switchDirectionSpeed == 5)
                {
                    facingDirection = GetFacingDirectionFromMovement(animationUpdateSum);
                    animationUpdateSum = Vector2.Zero;
                    switchDirectionSpeed = 0;
                }

                if (facingDirection >= 0)
                {
                    idleState.me.faceDirection(facingDirection);
                    SetMovementDirectionAnimation(facingDirection);
                }
            }

            protected void SetMovementDirectionAnimation(int dir)
            {
                if (dir < 0 || dir > 3)
                    return;
                SetMovingOnlyOneDirection(dir);
                switch (dir)
                {
                    case 0:
                        idleState.me.Sprite.AnimateUp(Game1.currentGameTime, 0, ""); break;
                    case 1:
                        idleState.me.Sprite.AnimateRight(Game1.currentGameTime, 0, ""); break;
                    case 2:
                        idleState.me.Sprite.AnimateDown(Game1.currentGameTime, 0, ""); break;
                    case 3:
                        idleState.me.Sprite.AnimateLeft(Game1.currentGameTime, 0, ""); break;
                }
            }

            protected void SetMovingOnlyOneDirection(int dir)
            {
                if (dir < 0 || dir > 3)
                    return;

                switch (dir)
                {
                    case 0:
                        characterMoveUp.SetValue(idleState.me, true);
                        characterMoveDown.SetValue(idleState.me, false);
                        characterMoveLeft.SetValue(idleState.me, false);
                        characterMoveRight.SetValue(idleState.me, false);
                        break;
                    case 1:
                        characterMoveUp.SetValue(idleState.me, false);
                        characterMoveDown.SetValue(idleState.me, false);
                        characterMoveLeft.SetValue(idleState.me, false);
                        characterMoveRight.SetValue(idleState.me, true);
                        break;
                    case 2:
                        characterMoveUp.SetValue(idleState.me, false);
                        characterMoveDown.SetValue(idleState.me, true);
                        characterMoveLeft.SetValue(idleState.me, false);
                        characterMoveRight.SetValue(idleState.me, false);
                        break;
                    case 3:
                        characterMoveUp.SetValue(idleState.me, false);
                        characterMoveDown.SetValue(idleState.me, false);
                        characterMoveLeft.SetValue(idleState.me, true);
                        characterMoveRight.SetValue(idleState.me, false);
                        break;
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

            protected void HandleGates()
            {
                if (gatesInThisLocation)
                {
                    Vector2 velocity = new Vector2(idleState.me.xVelocity, -idleState.me.yVelocity);
                    velocity.Normalize();
                    velocity = velocity * fullTile * 1.26f;
                    Vector2 bbVector = new Vector2(idleState.me.GetBoundingBox().Center.X, idleState.me.GetBoundingBox().Center.Y);
                    Vector2 tile = idleState.me.getTileLocation();
                    Vector2 tileAhead = (bbVector + velocity) / fullTile;
                    Vector2 tileBehind = (bbVector - velocity) / fullTile;
                    Fence[] fences = new Fence[3];
                    fences[0] = (idleState.aStar.gameLocation.getObjectAtTile((int)tileAhead.X, (int)tileAhead.Y)) as Fence;
                    fences[1] = (idleState.aStar.gameLocation.getObjectAtTile((int)tileBehind.X, (int)tileBehind.Y)) as Fence;
                    fences[2] = (idleState.aStar.gameLocation.getObjectAtTile((int)tile.X, (int)tile.Y)) as Fence;

                    if (fences[2] != null && fences[2].isGate.Value && fences[2].gatePosition.Value == 0)
                    {
                        fences[2].gatePosition.Value = 88;
                        idleState.aStar.gameLocation.playSound("doorClose");
                    }
                    else if (fences[0] != null && fences[0].isGate.Value && fences[0].gatePosition.Value == 0)
                    {
                        fences[0].gatePosition.Value = 88;
                        idleState.aStar.gameLocation.playSound("doorClose");
                    }
                    else if (fences[1] != null && fences[1].isGate.Value && fences[1].gatePosition.Value == 88)
                    {
                        fences[1].gatePosition.Value = 0;
                        idleState.aStar.gameLocation.playSound("doorClose");
                    }
                }
            }

            private bool CheckForGatesInThisLocation()
            {
                foreach (Vector2 o in idleState.aStar.gameLocation.Objects.Keys)
                {
                    Fence f = idleState.aStar.gameLocation.Objects[o] as Fence;
                    if (f != null && f.isGate.Value)
                        return true;
                }
                return false;
            }

            private void PathfindingNodeUpdateCheck()
            {
                if (currentPathNode != negativeOne && path != null)
                {
                    Point w = idleState.me.GetBoundingBox().Center;
                    Point n = new Point(((int)currentPathNode.X * fullTile) + halfTile, ((int)currentPathNode.Y * Game1.tileSize) + halfTile);
                    Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                    float nodeDiffLen = nodeDiff.Length();
                    if (nodeDiffLen <= pathNodeTolerance)
                    {
                        if (path.Count == 0)
                        {
                            path = null;
                            currentPathNode = negativeOne;
                            targetReachedAction.Invoke();
                            return;
                        }
                        currentPathNode = path.Dequeue();
                        n = new Point(((int)currentPathNode.X * fullTile) + halfTile, ((int)currentPathNode.Y * fullTile) + halfTile);
                        nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                        nodeDiffLen = nodeDiff.Length();
                    }
                }
            }

            private void HaltAndSetRestlessness()
            {
                idleState.me.Halt();
                int dir = GetFacingDirectionFromMovement(new Vector2(lastMovementDirection.X, -lastMovementDirection.Y));
                idleState.me.FacingDirection = dir;
                idleState.me.Sprite.faceDirectionStandard(dir);
                restlessness = idleState.r.Next(minRestlessness, maxRestlessness);
            }
        }

        private class LookAroundBehavior : IdleBehavior
        {
            private int direction;
            private int framesBeforeTurn;

            public LookAroundBehavior(AI_StateIdle idleState, int minSeconds, int maxSeconds) : base(idleState, minSeconds, maxSeconds)
            {
            }

            public override void StartBehavior()
            {
                base.StartBehavior();
                direction = idleState.r.Next(0, 4);
                idleState.me.FacingDirection = direction;
                idleState.me.Sprite.faceDirectionStandard(direction);
                framesBeforeTurn = idleState.r.Next(60 / 2, 60 * 4);
            }

            public override void Update(UpdateTickedEventArgs e)
            {
                base.Update(e);

                if (--framesBeforeTurn == 0)
                {
                    direction = idleState.r.Next(0, 4);
                    idleState.me.FacingDirection = direction;
                    idleState.me.Sprite.faceDirectionStandard(direction);
                    framesBeforeTurn = idleState.r.Next(60 / 2, 60 * 4);
                }
            }

        }

        private class AnimateBehavior : IdleBehavior
        {
            private bool hasPublicAnimations;
            private string[] privateLocations;

            public char animationIndexChar { get; private set; }
            private int animationIndex;
            private List<FarmerSprite.AnimationFrame>[] idleAnimations;
            private List<FarmerSprite.AnimationFrame>[] otherAnimations;
            private bool[] idleAnimationLoops;
            private bool[] idleAnimationsShy;

            private int animationRestartTimer;

            public AnimateBehavior(AI_StateIdle idleState, int minSeconds, int maxSeconds) :base(idleState, minSeconds, maxSeconds)
            {
                privateLocations = new string[16] { "FarmHouse", "Farm", "Saloon", "Beach",
                    "Mountain", "Forest", "BusStop", "Desert", "ArchaeologyHouse", "Woods",
                    "Railroad", "Summit", "CommunityCenter", "Greenhouse", "Backwoods", "BeachNightMarket"
                };
                switch (idleState.me.Name)
                {
                    case "Abigail":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(16, 400),
                            new FarmerSprite.AnimationFrame(17, 400),
                            new FarmerSprite.AnimationFrame(18, 400),
                            new FarmerSprite.AnimationFrame(19, 400, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(26, 1000),
                            new FarmerSprite.AnimationFrame(27, 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(AbigailSittingLoop), true)
                        };
                        idleAnimationLoops = new bool[2] { true, false };
                        idleAnimationsShy = new bool[2] { true, true };
                        hasPublicAnimations = false;
                        otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(27,1000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        break;
                    case "Alex":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(16, 100),
                            new FarmerSprite.AnimationFrame(17, 100),
                            new FarmerSprite.AnimationFrame(18, 100),
                            new FarmerSprite.AnimationFrame(19, 100),
                            new FarmerSprite.AnimationFrame(20, 100),
                            new FarmerSprite.AnimationFrame(21, 100),
                            new FarmerSprite.AnimationFrame(22, 100),
                            new FarmerSprite.AnimationFrame(23, 4000)
                        };
                        idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(36, 100),
                            new FarmerSprite.AnimationFrame(38, 100),
                            new FarmerSprite.AnimationFrame(35, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(AlexSittingLoop), true)
                        };
                        idleAnimationLoops = new bool[2] { true, false };
                        idleAnimationsShy = new bool[2] { true, true };
                        hasPublicAnimations = false;
                        otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(35,1000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        break;
                    case "Elliott":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        AnimatedSprite.endOfAnimationBehavior elliottReadingLoop =
                            new AnimatedSprite.endOfAnimationBehavior(ElliottReadingLoop);
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(32, 100),
                            new FarmerSprite.AnimationFrame(33, 100),
                            new FarmerSprite.AnimationFrame(34, 100, false, false, ElliottReadingLoop, true)
                        };
                        idleAnimationLoops = new bool[1] { false };
                        idleAnimationsShy = new bool[1] { false };
                        hasPublicAnimations = true;
                        otherAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(34,1000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        otherAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(40, 4000),
                            new FarmerSprite.AnimationFrame(42, 4000)
                        };

                        break;
                    case "Emily":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(24, 2000),
                            new FarmerSprite.AnimationFrame(25, 2000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(18, 300),
                            new FarmerSprite.AnimationFrame(19, 300),
                            new FarmerSprite.AnimationFrame(18, 300),
                            new FarmerSprite.AnimationFrame(19, 300),
                            new FarmerSprite.AnimationFrame(18, 300),

                            new FarmerSprite.AnimationFrame(22, 300),
                            new FarmerSprite.AnimationFrame(23, 300),
                            new FarmerSprite.AnimationFrame(22, 300),
                            new FarmerSprite.AnimationFrame(23, 300),
                            new FarmerSprite.AnimationFrame(22, 300),

                            new FarmerSprite.AnimationFrame(16, 300),
                            new FarmerSprite.AnimationFrame(17, 300),
                            new FarmerSprite.AnimationFrame(16, 300),
                            new FarmerSprite.AnimationFrame(17, 300),
                            new FarmerSprite.AnimationFrame(16, 300),

                            new FarmerSprite.AnimationFrame(20, 300),
                            new FarmerSprite.AnimationFrame(21, 300),
                            new FarmerSprite.AnimationFrame(20, 300),
                            new FarmerSprite.AnimationFrame(21, 300),
                            new FarmerSprite.AnimationFrame(20, 300)
                        };
                        idleAnimationLoops = new bool[2] { true, true };
                        idleAnimationsShy = new bool[2] { true, true };
                        hasPublicAnimations = false;
                        break;
                    case "Haley":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        AnimatedSprite.endOfAnimationBehavior haleyCameraFlip =
                            new AnimatedSprite.endOfAnimationBehavior(HaleyCameraFlip);
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(30, 1500),
                            new FarmerSprite.AnimationFrame(31, 100),
                            new FarmerSprite.AnimationFrame(24, 1000),
                            new FarmerSprite.AnimationFrame(31, 100),
                            new FarmerSprite.AnimationFrame(30, 1499, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true),
                            new FarmerSprite.AnimationFrame(30, 1, false, false, haleyCameraFlip, true),
                        };
                        idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(33, 1500),
                            new FarmerSprite.AnimationFrame(32, 100),
                            new FarmerSprite.AnimationFrame(25, 1000),
                            new FarmerSprite.AnimationFrame(32, 100),
                            new FarmerSprite.AnimationFrame(33, 1499, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionRight), true),
                            new FarmerSprite.AnimationFrame(33, 1, false, false, haleyCameraFlip, true)
                        };
                        idleAnimationLoops = new bool[2] { true, true };
                        idleAnimationsShy = new bool[2] { false, false };
                        hasPublicAnimations = true;
                        otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(33, 1500, false, true, null, false),
                            new FarmerSprite.AnimationFrame(32, 100, false, true, null, false),
                            new FarmerSprite.AnimationFrame(25, 1000, false, true, null, false),
                            new FarmerSprite.AnimationFrame(32, 100, false, true, null, false),
                            new FarmerSprite.AnimationFrame(33, 1499, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionLeft), true),
                            new FarmerSprite.AnimationFrame(33, 1, false, true, haleyCameraFlip, true)
                        };
                        break;
                    case "Harvey":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(42, 2000),
                            new FarmerSprite.AnimationFrame(43, 6000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(36, 5000),
                            new FarmerSprite.AnimationFrame(37, 100),
                            new FarmerSprite.AnimationFrame(38, 800),
                            new FarmerSprite.AnimationFrame(37, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionRight), true)
                        };
                        idleAnimationLoops = new bool[2] { true, true };
                        idleAnimationsShy = new bool[2] { false, false };
                        hasPublicAnimations = true;
                        break;
                    case "Leah":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(32, 1000),
                            new FarmerSprite.AnimationFrame(33, 1000),
                            new FarmerSprite.AnimationFrame(34, 1000),
                            new FarmerSprite.AnimationFrame(33, 1000),
                            new FarmerSprite.AnimationFrame(32, 1000),
                            new FarmerSprite.AnimationFrame(35, 3000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        idleAnimationLoops = new bool[1] { true };
                        idleAnimationsShy = new bool[1] { false };
                        hasPublicAnimations = true;
                        break;
                    case "Maru":
                        idleAnimations = null;
                        idleAnimationLoops = null;
                        break;
                    case "Penny":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        AnimatedSprite.endOfAnimationBehavior pennyReadingLoop =
                            new AnimatedSprite.endOfAnimationBehavior(PennyReadingLoop);
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(16, 100),
                            new FarmerSprite.AnimationFrame(17, 100),
                            new FarmerSprite.AnimationFrame(19, 100),
                            new FarmerSprite.AnimationFrame(18, 1000, false, false, pennyReadingLoop, true)
                        };
                        idleAnimationLoops = new bool[1] { false };
                        idleAnimationsShy = new bool[1] { false };
                        hasPublicAnimations = true;
                        otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(18, 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true)
                        };
                        break;
                    case "Sam":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(16, 200),
                            new FarmerSprite.AnimationFrame(17, 200),
                            new FarmerSprite.AnimationFrame(18, 200),
                            new FarmerSprite.AnimationFrame(19, 2000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionLeft), true)
                        };
                        idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(20, 200),
                            new FarmerSprite.AnimationFrame(21, 200),
                            new FarmerSprite.AnimationFrame(20, 200),
                            new FarmerSprite.AnimationFrame(21, 200),
                            new FarmerSprite.AnimationFrame(20, 200),

                            new FarmerSprite.AnimationFrame(22, 200),
                            new FarmerSprite.AnimationFrame(23, 200),
                            new FarmerSprite.AnimationFrame(22, 200),
                            new FarmerSprite.AnimationFrame(23, 200),
                            new FarmerSprite.AnimationFrame(22, 200),

                            new FarmerSprite.AnimationFrame(20, 200),
                            new FarmerSprite.AnimationFrame(21, 200),
                            new FarmerSprite.AnimationFrame(20, 200),
                            new FarmerSprite.AnimationFrame(21, 200),
                            new FarmerSprite.AnimationFrame(20, 200),

                            new FarmerSprite.AnimationFrame(22, 200, false, true, null, false),
                            new FarmerSprite.AnimationFrame(23, 200, false, true, null, false),
                            new FarmerSprite.AnimationFrame(22, 200, false, true, null, false),
                            new FarmerSprite.AnimationFrame(23, 200, false, true, null, false),
                            new FarmerSprite.AnimationFrame(22, 200, false, true, null, false)
                        };
                        idleAnimationLoops = new bool[2] { true, true };
                        idleAnimationsShy = new bool[2] { false, true };
                        hasPublicAnimations = true;
                        break;
                    case "Sebastian":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                        AnimatedSprite.endOfAnimationBehavior sebastianStopSmoking =
                            new AnimatedSprite.endOfAnimationBehavior(SebastianStopSmoking);
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(16, 8000),
                            new FarmerSprite.AnimationFrame(17, 100),
                            new FarmerSprite.AnimationFrame(18, 100),
                            new FarmerSprite.AnimationFrame(19, 100),
                            new FarmerSprite.AnimationFrame(20, 100),
                            new FarmerSprite.AnimationFrame(21, 1000),
                            new FarmerSprite.AnimationFrame(22, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionRight), true),
                            new FarmerSprite.AnimationFrame(23, 100, false, false, sebastianStopSmoking, true)
                        };
                        idleAnimationLoops = new bool[] { true };
                        idleAnimationsShy = new bool[1] { true };
                        hasPublicAnimations = false;
                        break;
                    case "Shane":
                        idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                        AnimatedSprite.endOfAnimationBehavior shaneStopLooping =
                            new AnimatedSprite.endOfAnimationBehavior(ShaneStopLooping);
                        idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(30, 500),
                            new FarmerSprite.AnimationFrame(31, 1000),
                            new FarmerSprite.AnimationFrame(30, 500),
                            new FarmerSprite.AnimationFrame(31, 1000),
                            new FarmerSprite.AnimationFrame(30, 500),
                            new FarmerSprite.AnimationFrame(31, 1000),
                            new FarmerSprite.AnimationFrame(30, 500),
                            new FarmerSprite.AnimationFrame(31, 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true),
                            new FarmerSprite.AnimationFrame(19, 6000, false, false, shaneStopLooping, true)
                        };
                        idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(20, 4000),
                            new FarmerSprite.AnimationFrame(21, 100),
                            new FarmerSprite.AnimationFrame(22, 100),
                            new FarmerSprite.AnimationFrame(23, 100),
                            new FarmerSprite.AnimationFrame(24, 1200),
                            new FarmerSprite.AnimationFrame(23, 100),
                            new FarmerSprite.AnimationFrame(22, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(SetPostAnimationDirectionDown), true),
                            new FarmerSprite.AnimationFrame(21, 100, false, false, shaneStopLooping, true)
                        };
                        idleAnimationLoops = new bool[2] { true, true };
                        idleAnimationsShy = new bool[2] { false, false };
                        hasPublicAnimations = true;
                        break;
                }
            }

            public override void StartBehavior()
            {
                base.StartBehavior();
                int i = idleState.r.Next(0, idleAnimations.Length);
                if (!privateLocations.Contains(idleState.me.currentLocation.Name) &&
                    idleAnimationsShy[i])
                {
                    do
                    {
                        i = ++i >= idleAnimationLoops.Length ? 0 : i;
                    } while (idleAnimationsShy[i]);
                }
                animationIndex = i;
                animationIndexChar = (char)(i + 97);
                idleState.me.Sprite.setCurrentAnimation(idleAnimations[i]);
                idleState.me.Sprite.loop = idleAnimationLoops[i];
                Patches.dontFace = true;
            }

            public override void StopBehavior()
            {
                idleState.me.Sprite.currentAnimation.Clear();
                idleState.me.Sprite.faceDirectionStandard(idleState.me.FacingDirection);
                Patches.dontFace = true;
            }

            public override void Update(UpdateTickedEventArgs e)
            {
                base.Update(e);
                if (--animationRestartTimer == 0)
                {
                    RestartAnimation();
                }
            }

            public override bool CanPerformThisBehavior()
            {
                if (hasPublicAnimations)
                    return true;
                else
                {
                    NPC n = (idleState.me as NPC);
                    return privateLocations.Contains(idleState.me.currentLocation.Name) ||
                            (n != null && idleState.me.currentLocation.Equals(n.DefaultMap));
                }
            }

            private void PennyReadingLoop(Farmer leader)
            {
                idleState.me.Sprite.setCurrentAnimation(otherAnimations[0]);
                idleState.me.Sprite.loop = true;
            }

            private void AbigailSittingLoop(Farmer leader)
            {
                idleState.me.Sprite.setCurrentAnimation(otherAnimations[0]);
                idleState.me.Sprite.loop = true;
            }

            private void AlexSittingLoop(Farmer leader)
            {
                idleState.me.Sprite.setCurrentAnimation(otherAnimations[0]);
                idleState.me.Sprite.loop = true;
            }

            private void ElliottReadingLoop(Farmer leader)
            {
                idleState.me.Sprite.setCurrentAnimation(otherAnimations[0]);
                idleState.me.Sprite.loop = true;
            }

            private void HaleyCameraFlip(Farmer leader)
            {
                int newAnimation = idleState.r.Next(3);
                if (newAnimation == 0)
                    idleState.me.Sprite.setCurrentAnimation(idleAnimations[1]);
                else if (newAnimation == 1)
                    idleState.me.Sprite.setCurrentAnimation(otherAnimations[0]);
                else
                {
                    idleState.me.Sprite.setCurrentAnimation(idleAnimations[0]);
                }
            }

            private void SebastianStopSmoking(Farmer leader)
            {
                if (idleState.r.Next(3) == 0)
                {
                    idleState.me.Sprite.loop = false;
                    SetAnimationRestartTimer();
                }
            }

            private void ShaneStopLooping(Farmer leader)
            {
                if (idleState.r.Next(3) == 0)
                {
                    idleState.me.Sprite.loop = false;
                    SetAnimationRestartTimer();
                }
            }

            private void SetPostAnimationDirectionRight(Farmer leader)
            {
                idleState.me.FacingDirection = 1;
            }

            private void SetPostAnimationDirectionDown(Farmer leader)
            {
                idleState.me.FacingDirection = 2;
            }

            private void SetPostAnimationDirectionLeft(Farmer leader)
            {
                idleState.me.FacingDirection = 3;
            }

            private void SetAnimationRestartTimer()
            {
                animationRestartTimer = 60 * idleState.r.Next(8, 16);
            }

            private void RestartAnimation()
            {
                int i = idleState.r.Next(idleAnimationLoops.Length);
                if (!privateLocations.Contains(idleState.me.currentLocation.Name) &&
                    idleAnimationsShy[i])
                {
                    do
                    {
                        i = ++i >= idleAnimationLoops.Length ? 0 : i;
                    } while (idleAnimationsShy[i]);
                }
                idleState.me.Sprite.setCurrentAnimation(idleAnimations[i]);
                idleState.me.Sprite.loop = idleAnimationLoops[i];
                animationRestartTimer = -1;
            }
        }

        private class FishBehavior : WanderBehavior
        {
            private Buffs.ElliottBuff buff;
            private bool goneFishing;
            private bool fishingFacingRight;
            private List<FarmerSprite.AnimationFrame> fishingLeftAnim;
            private List<FarmerSprite.AnimationFrame> fishingRightAnim;

            public FishBehavior(AI_StateIdle idleState, int minSeconds, int maxSeconds, int minRestlessness, int maxRestlessness, int squareRadius, float speed) 
                : base(idleState, minSeconds, maxSeconds, minRestlessness, maxRestlessness, squareRadius, speed)
            {
                targetReachedAction = new Action(StartFishing);
                backupBehavior = 1;
                
                fishingLeftAnim = new List<FarmerSprite.AnimationFrame>
                {
                    new FarmerSprite.AnimationFrame(20, 4000, false, true, null, false),
                    new FarmerSprite.AnimationFrame(21, 4000, false, true, null, false)
                };
                fishingRightAnim = new List<FarmerSprite.AnimationFrame>
                {
                    new FarmerSprite.AnimationFrame(20, 4000),
                    new FarmerSprite.AnimationFrame(21, 4000)
                };
            }

            public override void StartBehavior()
            {
                base.StartBehavior();
                buff = idleState.stateMachine.owner.buff as Buffs.ElliottBuff;
                if (buff == null)
                    throw new Exception("Tried to add a Fishing Behavior for someone who can't fish!");
                Patches.dontFace = true;
            }

            //private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
            //{
            //    if (goneFishing)
            //    {
            //        Rectangle r = idleState.me.GetBoundingBox();
            //        r.X -= Game1.viewport.X;
            //        r.Y -= Game1.viewport.Y;
            //        e.SpriteBatch.Draw(Game1.mouseCursors, r, new Rectangle?(new Rectangle(194, 388, 64, 64)), Color.Red);
            //    }
            //}

            public override void StopBehavior()
            {
                base.StopBehavior();
                if (goneFishing)
                {
                    buff.StopFishing();
                    (idleState.me as NPC).HideShadow = false;
                    (idleState.me as NPC).reloadSprite();
                    idleState.me.Sprite.SpriteWidth = 16;
                    idleState.me.drawOffset.Value = new Vector2(0, 0);
                    idleState.me.Sprite.UpdateSourceRect();
                    Patches.dontFace = false;
                    goneFishing = false;
                }
            }

            public override bool CanPerformThisBehavior()
            {
                return idleState.aStar.gameLocation.waterTiles != null;
            }

            public override void SelfTransition()
            {
                StopBehavior();
                StartBehavior();
            }

            protected override Vector2 PickTile()
            {
                Vector2 tile = negativeOne;
                if (idleState.aStar.gameLocation.waterTiles == null)
                    return tile;

                int totalTiles = ((maxTilesToWander * 2) + 1) * ((maxTilesToWander * 2) + 1);
                char[,] tileCache = new char[(maxTilesToWander * 2) + 1, (maxTilesToWander * 2) + 1];
                tileCache[maxTilesToWander, maxTilesToWander] = (char) 1;
                Vector2 loc = idleState.me.getTileLocation();
                Vector3 translate = new Vector3(loc.X, loc.Y, 0) - new Vector3(maxTilesToWander, maxTilesToWander, 0);
                Queue<Vector3> tileQueue = new Queue<Vector3>();
                tileQueue.Enqueue(new Vector3(loc.X, loc.Y, 0));
                while (tileQueue.Count != 0)
                {
                    Vector3 t = tileQueue.Dequeue();
                    Vector3[] neighbors = idleState.aStar.GetDirectWalkableNeighbors(t);
                    foreach (Vector3 neighbor in neighbors)
                    {
                        Vector3 pos = neighbor - translate;
                        if (pos.X >= 0 && pos.X <= maxTilesToWander * 2 &&
                            pos.Y >= 0 && pos.Y <= maxTilesToWander * 2 &&
                            tileCache[(int)pos.X, (int)pos.Y] == (char)0)
                        {
                            if (neighbor.Z == 1)
                            {
                                tileCache[(int)pos.X, (int)pos.Y] = (char)1;
                                tileQueue.Enqueue(neighbor);
                            }
                            else
                            {
                                if (idleState.aStar.gameLocation.waterTiles[(int)neighbor.X, (int)neighbor.Y])
                                    tileCache[(int)pos.X, (int)pos.Y] = (char)3;
                                else
                                    tileCache[(int)pos.X, (int)pos.Y] = (char)2;
                            }
                        }
                    }
                }
                tile = negativeOne;

                // Key:
                // 0 = unreachable
                // 1 = reachable, walkable
                // 2 = reachable, unwalkable
                // 3 = reachable, water
                List<Vector3> fishingTiles = new List<Vector3>(maxTilesToWander);
                int xDim = maxTilesToWander * 2;
                for (int y = 0; y < (maxTilesToWander * 2) + 1; y++)
                {
                    for (int x = 0; x < (maxTilesToWander * 2) + 1; x++)
                    {
                        if (tileCache[x, y] == (char) 3)
                        {
                            if (x > 0 && tileCache[x - 1, y] == (char) 1)
                                fishingTiles.Add(new Vector3(x - 1, y, 1));
                            if (x < xDim && tileCache[x + 1, y] == (char) 1)
                                fishingTiles.Add(new Vector3(x + 1, y, -1));
                        }
                    }
                }

                if (fishingTiles.Count > 0)
                {
                    Vector3 t = fishingTiles[idleState.r.Next(fishingTiles.Count)] + translate;
                    tile = new Vector2((int)t.X, (int)t.Y);
                    fishingFacingRight = t.Z > 0 ? true : false;
                }
                return tile;
            }

            private void StartFishing()
            {
                if (!goneFishing)
                {
                    goneFishing = true;
                    idleState.me.Sprite.SpriteWidth = 32;
                    if (fishingFacingRight)
                    {
                        idleState.me.Sprite.setCurrentAnimation(fishingRightAnim);
                    }
                    else
                    {
                        idleState.me.drawOffset.Value = new Vector2(-64f, 0);
                        idleState.me.Sprite.setCurrentAnimation(fishingLeftAnim);
                    }
                    idleState.me.Sprite.loop = true;
                    idleState.me.Sprite.ignoreSourceRectUpdates = false;
                    buff.StartFishing();
                    (idleState.me as NPC).HideShadow = true;
                }
            }
        }
    }

    
}

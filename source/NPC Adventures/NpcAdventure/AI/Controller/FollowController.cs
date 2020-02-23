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
    internal class FollowController : IController
    {
        public const float MOVE_THRESHOLD_DISTANCE = 2.65f;
        public const float DECELERATE_THRESHOLD = 1.35f;
        public const float DECELERATION = 0.025f;

        private Vector2 negativeOne = new Vector2(-1, -1);

        public Character leader;
        public NPC follower;
        protected PathFinder pathFinder;
        protected FollowJoystick joystick;
        public int followingLostTime = 0;
        public Queue<Vector2> pathToFollow;
        protected readonly AI_StateMachine ai;
        public Vector2 leaderLastTileCheckPoint;
        private int idleTimer;

        public virtual bool IsIdle => this.idleTimer == 0;

        internal FollowController(AI_StateMachine ai)
        {
            this.pathToFollow = new Queue<Vector2>();
            this.ai = ai;
            this.leader = ai.player;
            this.follower = ai.npc;
            this.pathFinder = new PathFinder(this.follower.currentLocation, this.follower, this.leader);
            this.joystick = new FollowJoystick(ref this.follower, this.pathFinder);

            this.ai.LocationChanged += this.Ai_LocationChanged;
            this.joystick.Move += this.OnMove;
        }

        private void OnMove(object sender, FollowJoystick.MoveEventArgs e)
        {
            this.idleTimer = e.IsLastFrame ? Game1.random.Next(480, 840) : -1;
        }

        private void Ai_LocationChanged(object sender, EventArgsLocationChanged e)
        {
            // We are not active controller? Don't handle changed location event
            if (this.ai.CurrentController != this)
                return;

            this.leaderLastTileCheckPoint = this.negativeOne;
            this.joystick.Reset();
            this.PathfindingRemakeCheck();
        }

        public virtual void Update(UpdateTickedEventArgs e)
        {
            if (this.follower == null || this.leader == null || (!Context.IsPlayerFree && !Context.IsMultiplayer))
                return;

            this.CheckIdleState();

            if (this.joystick.IsFollowing)
                this.DriveSpeed();

            this.joystick.Update(e);

            if (e.IsMultipleOf(15))
                this.PathfindingRemakeCheck();
        }

        private void CheckIdleState()
        {
            if (this.idleTimer > 0)
                --this.idleTimer;
        }

        protected virtual void PathfindingRemakeCheck()
        {
            if (this.leader == null || this.leader.currentLocation == null)
                return;

            Vector2 leaderCurrentTile = this.leader.getTileLocation();

            if (this.pathFinder.GameLocation != this.leader.currentLocation) {
                this.pathFinder.GameLocation = this.leader.currentLocation;
            }

            if (this.leaderLastTileCheckPoint != leaderCurrentTile)
            {
                this.joystick.AcquireTarget(leaderCurrentTile);
            }

            this.leaderLastTileCheckPoint = leaderCurrentTile;
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
                return this.joystick.Speed - DECELERATION;
            }

            return 0;
        }

        public void DriveSpeed()
        {
            Point fp = this.follower.GetBoundingBox().Center;
            Point lp = this.leader.GetBoundingBox().Center;

            Vector2 diff = new Vector2(lp.X, lp.Y) - new Vector2(fp.X, fp.Y);
            this.joystick.Speed = this.GetMovementSpeedBasedOnDistance(diff.Length());
        }

        public virtual void Activate()
        {
            if (this.leader != null)
            {
                this.PathfindingRemakeCheck();
            }

            this.idleTimer = Game1.random.Next(500, 800);
        }

        public virtual void Deactivate() {
            this.idleTimer = 0;
        }
    }
}

using NpcAdventure.AI.Controller;
using NpcAdventure.Model;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.AI
{
    /// <summary>
    /// State machine for companion AI
    /// </summary>
    public class AI_StateMachine : Internal.IUpdateable
    {
        public enum State
        {
            FOLLOW,
            FIGHT,
        }

        private const float MONSTER_DISTANCE = 9f;
        public readonly NPC npc;
        public readonly Character player;
        private readonly IModEvents events;
        public readonly IMonitor monitor;
        internal readonly CompanionMetaData metadata;
        private Dictionary<State, IController> controllers;
        private int idleTimer = 0;

        internal AI_StateMachine(NPC npc, Character player, CompanionMetaData metadata, IModEvents events, IMonitor monitor)
        {
            this.npc = npc ?? throw new ArgumentNullException(nameof(npc));
            this.player = player ?? throw new ArgumentNullException(nameof(player));
            this.events = events ?? throw new ArgumentException(nameof(events));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.metadata = metadata;
        }

        public State CurrentState { get; private set; }
        internal IController CurrentController { get => this.controllers[this.CurrentState]; }

        public event EventHandler<EventArgsLocationChanged> LocationChanged;

        /// <summary>
        /// Setup AI state machine
        /// </summary>
        public void Setup()
        {
            this.controllers = new Dictionary<State, IController>()
            {
                [State.FOLLOW] = new FollowController(this),
                [State.FIGHT] = new FightController(this, this.events, this.metadata.Sword),
            };

            // By default AI following the player
            this.ChangeState(State.FOLLOW);
        }

        private void ChangeState(State state)
        {
            this.monitor.Log($"AI changes state {this.CurrentState} -> {state}");

            if (this.CurrentController != null)
            {
                this.CurrentController.Deactivate();
            }

            this.CurrentState = state;
            this.CurrentController.Activate();
        }

        private bool IsThereAnyMonster()
        {
            return Helper.GetNearestMonsterToCharacter(this.npc, MONSTER_DISTANCE) != null;
        }

        private bool PlayerIsNear()
        {
            return Helper.Distance(this.player.getTileLocationPoint(), this.npc.getTileLocationPoint()) < 11f;
        }

        private void CheckPotentialStateChange()
        {
            if (this.idleTimer == 0 && this.CurrentState != State.FIGHT && this.PlayerIsNear() && this.IsThereAnyMonster())
            {
                this.ChangeState(State.FIGHT);
                this.monitor.Log("A 50ft monster is here!");
            }

            if (this.CurrentState == State.FIGHT && this.CurrentController.IsIdle)
            {
                this.idleTimer = 100;
                this.ChangeState(State.FOLLOW);
            }
        }

        public void Update(UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(15))
            {
                this.CheckPotentialStateChange();
            }

            if (this.idleTimer > 0)
                this.idleTimer--;

            if (this.CurrentController != null)
                this.CurrentController.Update(e);
        }

        public void ChangeLocation(GameLocation l)
        {
            GameLocation previousLocation = this.npc.currentLocation;
            
            // Warp NPC to player's location at theirs position
            Helper.WarpTo(this.npc, l, this.player.getTileLocationPoint());

            // Fire location changed event
            this.OnLocationChanged(previousLocation, this.npc.currentLocation);
        }

        private void OnLocationChanged(GameLocation previous, GameLocation next)
        {
            EventArgsLocationChanged args = new EventArgsLocationChanged()
            {
                PreviousLocation = previous,
                CurrentLocation = next,
            };

            this.LocationChanged?.Invoke(this, args);
        }

        public void Dispose()
        {
            this.controllers.Clear();
            this.controllers = null;
        }
    }
}

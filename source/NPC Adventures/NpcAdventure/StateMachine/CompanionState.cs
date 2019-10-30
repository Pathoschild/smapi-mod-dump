using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.StateMachine
{
    public interface ICompanionState
    {
        /// <summary>
        /// Enter to this state
        /// </summary>
        void Entry();
        
        /// <summary>
        /// Exit from this state
        /// </summary>
        void Exit();
    }

    internal abstract class CompanionState : ICompanionState
    {
        public CompanionStateMachine StateMachine { get; private set; }
        protected IModEvents Events { get; }

        public CompanionState(CompanionStateMachine stateMachine, IModEvents events)
        {
            this.StateMachine = stateMachine ?? throw new Exception("State Machine must be set!");
            this.Events = events;
        }

        /// <summary>
        /// By default do nothing when this state entered
        /// </summary>
        public virtual void Entry() {}

        /// <summary>
        /// By default do nothing when this state was exited
        /// </summary>
        public virtual void Exit() {}
    }
}

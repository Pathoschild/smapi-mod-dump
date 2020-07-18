using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Events
{
    /// <summary>
    /// Quest Framework lifecycle state change event arguments.
    /// </summary>
    public class ChangeStateEventArgs : EventArgs
    {
        /// <summary>
        /// Previous lifecycle state
        /// </summary>
        public State OldState { get; }

        /// <summary>
        /// Current lifecycle state
        /// </summary>
        public State NewState { get; }

        internal ChangeStateEventArgs(State oldState, State newState)
        {
            this.OldState = oldState;
            this.NewState = newState;
        }
    }
}

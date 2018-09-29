using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Framework.FunctionEvents
{
    /// <summary>
    /// Used to handle various functions that occur on player interaction.
    /// </summary>
    public class PlayerEvents
    {
        /// <summary>
        /// Occurs when the player enters the same tile as this event.
        /// </summary>
        public functionEvent onPlayerEnter;
        /// <summary>
        /// Occurs when the player leaves the same tile as this event.
        /// </summary>
        public functionEvent onPlayerLeave;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="OnPlayerEnter"></param>
        /// <param name="OnPlayerLeave"></param>
        public PlayerEvents(functionEvent OnPlayerEnter, functionEvent OnPlayerLeave)
        {
            this.onPlayerEnter = OnPlayerEnter;
            this.onPlayerLeave = OnPlayerLeave;
        }
    }
}

namespace EventSystem.Framework.FunctionEvents
{
    /// <summary>Used to handle various functions that occur on player interaction.</summary>
    public class PlayerEvents
    {
        /// <summary>Occurs when the player enters the same tile as this event.</summary>
        public functionEvent onPlayerEnter;

        /// <summary>Occurs when the player leaves the same tile as this event.</summary>
        public functionEvent onPlayerLeave;

        /// <summary>Construct an instance.</summary>
        /// <param name="OnPlayerEnter">Occurs when the player enters the same tile as this event.</param>
        /// <param name="OnPlayerLeave">Occurs when the player leaves the same tile as this event.</param>
        public PlayerEvents(functionEvent OnPlayerEnter, functionEvent OnPlayerLeave)
        {
            this.onPlayerEnter = OnPlayerEnter;
            this.onPlayerLeave = OnPlayerLeave;
        }
    }
}

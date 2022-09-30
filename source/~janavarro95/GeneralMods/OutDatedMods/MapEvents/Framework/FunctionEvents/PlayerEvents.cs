/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

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

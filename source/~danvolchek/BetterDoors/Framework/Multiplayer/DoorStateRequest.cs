/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace BetterDoors.Framework.Multiplayer
{
    /// <summary>Represents a serializable door state request.</summary>
    internal class DoorStateRequest
    {
        /*********
        ** Accessors
        *********/

        /// <summary>The location to request states for.</summary>
        public string LocationName { get; }

        /*********
        ** Public methods
        *********/

        /// <summary>Constructs an instance.</summary>
        /// <param name="locationName">The location to request states for.</param>
        public DoorStateRequest(string locationName)
        {
            this.LocationName = locationName;
        }
    }
}

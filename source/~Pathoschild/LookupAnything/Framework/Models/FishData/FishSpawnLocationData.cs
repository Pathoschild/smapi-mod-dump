/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Pathoschild.Stardew.LookupAnything.Framework.Models.FishData
{
    /// <summary>Location-specific spawn rules for a fish.</summary>
    /// <param name="DisplayName">The translated name for the location and area.</param>
    /// <param name="LocationId">The location's internal name.</param>
    /// <param name="Area">The area ID within the location, if applicable.</param>
    /// <param name="Seasons">The required seasons.</param>
    internal record FishSpawnLocationData(string DisplayName, string LocationId, string? Area, HashSet<string> Seasons)
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="displayName">The translated name for the location and area.</param>
        /// <param name="locationId">The location's internal name.</param>
        /// <param name="area">The area ID within the location, if applicable.</param>
        /// <param name="seasons">The required seasons.</param>
        internal FishSpawnLocationData(string displayName, string locationId, int? area, string[] seasons)
            : this(displayName, locationId, area >= 0 ? area.ToString() : null, seasons) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="displayName">The translated name for the location and area.</param>
        /// <param name="locationId">The location's internal name.</param>
        /// <param name="area">The area ID within the location, if applicable.</param>
        /// <param name="seasons">The required seasons.</param>
        internal FishSpawnLocationData(string displayName, string locationId, string? area, string[] seasons)
            : this(displayName, locationId, area, new HashSet<string>(seasons, StringComparer.OrdinalIgnoreCase)) { }

        /// <summary>Get whether this matches a given location name.</summary>
        /// <param name="locationId">The location internal name to match.</param>
        public bool MatchesLocation(string locationId)
        {
            // specific mine level (e.g. Lava Eel in UndergroundMine100)
            if (this.LocationId == "UndergroundMine" && !string.IsNullOrWhiteSpace(this.Area))
                return locationId == $"{this.LocationId}{this.Area}";

            // location name
            return locationId == this.LocationId;
        }
    }
}

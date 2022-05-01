/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using System;
using System.Collections.Generic;

namespace Pathoschild.Stardew.LookupAnything.Framework.Models.FishData
{
    /// <summary>Location-specific spawn rules for a fish.</summary>
    internal class FishSpawnLocationData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The location's internal name.</summary>
        public string LocationName { get; }

        /// <summary>The location's translated name.</summary>
        public string LocationDisplayName => this.Area != null
            ? I18n.LocationOverrides.AreaName(this.LocationName, this.Area)
            : I18n.LocationOverrides.LocationName(this.LocationName);

        /// <summary>The area ID within the location, if applicable.</summary>
        public string Area { get; }

        /// <summary>The required seasons.</summary>
        public ISet<string> Seasons { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="locationName">The location name.</param>
        /// <param name="area">The area ID within the location, if applicable.</param>
        /// <param name="seasons">The required seasons.</param>
        internal FishSpawnLocationData(string locationName, int? area, string[] seasons)
            : this(locationName, area >= 0 ? area.ToString() : null, seasons) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="locationName">The location name.</param>
        /// <param name="area">The area ID within the location, if applicable.</param>
        /// <param name="seasons">The required seasons.</param>
        public FishSpawnLocationData(string locationName, string area, string[] seasons)
        {
            this.LocationName = locationName;
            this.Area = area;
            this.Seasons = new HashSet<string>(seasons, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Get whether this matches a given location name.</summary>
        /// <param name="locationName">The location name to match.</param>
        public bool MatchesLocation(string locationName)
        {
            // specific mine level (e.g. Lava Eel in UndergroundMine100)
            if (this.LocationName == "UndergroundMine" && !string.IsNullOrWhiteSpace(this.Area))
                return locationName == $"{this.LocationName}{this.Area}";

            // location name
            return locationName == this.LocationName;
        }
    }
}

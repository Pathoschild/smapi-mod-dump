/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace MoreGrass.Models
{
    /// <summary>Represents the key used to cache results from <see cref="Utilities.ContainsLocation(string, List{string})"/>.</summary>
    /// <remarks>The reason a Tuple couldn't be used instead is because Mono doesn't have support for them causing the mod to be incompatible on Unix.</remarks>
    internal class ContainsLocationData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the location to check if it's in <see cref="Locations"/>.</summary>
        public string LocationName { get; set; }

        /// <summary>The list of locations to check against.</summary>
        public List<string> Locations { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="locationName">The name of the location to check if it's in <paramref name="locations"/>.</param>
        /// <param name="locations">The list of locations to check against.</param>
        public ContainsLocationData(string locationName, List<string> locations)
        {
            LocationName = locationName;
            Locations = locations;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ContainsLocationData data
                && LocationName == data.LocationName
                && EqualityComparer<List<string>>.Default.Equals(Locations, data.Locations);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1394484220;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationName);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(Locations);
            return hashCode;
        }
    }
}

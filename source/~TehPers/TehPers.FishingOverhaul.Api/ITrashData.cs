/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.Core.Api.Weighted;

namespace TehPers.FishingOverhaul.Api {
    public interface ITrashData : IWeighted {
        /// <summary>All the possible IDs this trash can be.</summary>
        IEnumerable<int> PossibleIds { get; }

        /// <summary>Whether this fish meets the given criteria and can be caught.</summary>
        /// <param name="who">The farmer that is fishing.</param>
        /// <param name="locationName">The name of the current location being fished in.</param>
        /// <param name="waterType">The type of water this fish is in.</param>
        /// <param name="date">The current date.</param>
        /// <param name="weather">The current weather.</param>
        /// <param name="time">The current time.</param>
        /// <param name="fishingLevel">The current farmer's fishing level.</param>
        /// <param name="mineLevel">The current level in the mine, or null if not in the mine.</param>
        /// <returns>True if this fish can be caught, false otherwise.</returns>
        bool MeetsCriteria(Farmer who, string locationName, WaterType waterType, SDate date, Weather weather, int time, int fishingLevel, int? mineLevel);
    }
}

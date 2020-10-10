/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace StardewValleyMods.CategorizeChests.Framework.Persistence
{
    /// <summary>
    /// A key that uniquely identifies a spot in the world where a chest exists.
    /// </summary>
    class ChestAddress
    {
        public ChestLocationType LocationType { get; set; }

        /// <summary>
        /// The name of the GameLocation where the chest is.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LocationName { get; set; }

        /// <summary>
        /// The name of the building the chest is in, if the location is a
        /// buildable location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BuildingName { get; set; }

        /// <summary>
        /// The tile the chest is found on.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Vector2 Tile { get; set; }
    }
}
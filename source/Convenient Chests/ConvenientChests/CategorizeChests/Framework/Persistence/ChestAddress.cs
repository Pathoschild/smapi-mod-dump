/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework;

namespace ConvenientChests.CategorizeChests.Framework.Persistence
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
        public string LocationName { get; set; }

        /// <summary>
        /// The name of the building the chest is in, if the location is a
        /// buildable location.
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// The tile the chest is found on.
        /// </summary>
        public Vector2 Tile { get; set; }

        public ChestAddress()
        {
        }
        
        public ChestAddress(string locationName, Vector2 tile, ChestLocationType locationType = ChestLocationType.Normal, string buildingName = "")
        {
            LocationName = locationName;
            Tile = tile;
            LocationType = locationType;
            BuildingName = buildingName;
        }
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using StardewValley;

namespace ChestFeatureSet.Framework
{
    public static class LocationExtension
    {
        public static string[] FarmArea { get; } = new string[] { "Farm", "FarmHouse", "Cellar", "GreenHouse", "Coop", "Barn", "SlimeHutch", "Shed", "FarmCave" };

        /// <summary>
        /// Get all game's locations
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GameLocation> GetAllLocations()
        {
            return Game1.locations.Concat(
                from location in Game1.locations
                where location.IsBuildableLocation()
                from building in location.buildings
                where building.indoors.Value != null
                select building.indoors.Value
            );
        }
    }
}

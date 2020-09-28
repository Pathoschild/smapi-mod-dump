using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace RemoteFridgeStorage
{
    public class Util
    {
        /// <summary>
        /// Get all game locations
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations.Concat(
                from location in Game1.locations.OfType<BuildableGameLocation>()
                from building in location.buildings
                where building.indoors.Value != null
                select building.indoors.Value
            );
        }

        /// <summary>
        /// Gets all the chests in the world
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ChestLocationPair> GetAllChests()
        {
            return new List<ChestLocationPair>().Concat(from location in GetLocations()
                from @object in location.objects.Values
                where @object is Chest
                select new ChestLocationPair((Chest) @object, location));
        }
    }

    /// <summary>
    /// Pair of chest and their locations
    /// </summary>
    public class ChestLocationPair
    {
        public readonly Chest Chest;
        public readonly GameLocation Location;

        public ChestLocationPair(Chest chest, GameLocation location)
        {
            Chest = chest;
            Location = location;
        }
    }
}
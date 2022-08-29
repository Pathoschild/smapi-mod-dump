/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace DecidedlyShared.Utilities
{
    public class Maps
    {
        private static List<GameLocation> maps;

        public static Dictionary<GameLocation, string> NameMap { get; set; }

        public static void PopulateMapList()
        {
            maps = Game1.locations.ToList();
        }

        public static GameLocation GetLocation(string locationName)
        {
            var mapQuery = from map in maps where map.Name.Contains(locationName) select map;

            return mapQuery.First(map => map.Name.Contains(locationName));
        }

        public static List<GameLocation> GetLocations(string searchTerm)
        {
            return Game1.locations.Where(map => map.Name.Contains(searchTerm)).ToList();
        }
    }
}

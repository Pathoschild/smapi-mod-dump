using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace Common
{
    internal static class Utilities
    {
        [NotNull]
        public static IEnumerable<GameLocation> GetLocations(IModHelper helper)
        {
            if (Context.IsMainPlayer)
            {
                // From https://stardewvalleywiki.com/Modding:Common_tasks#Get_all_locations on 2019/03/16
                return Game1.locations.Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
            }

            return helper.Multiplayer.GetActiveLocations();
        }
    }
}
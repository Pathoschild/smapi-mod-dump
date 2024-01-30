/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpNetwork.framework
{
	internal class Queries
	{
		public static void Register()
		{
			GameStateQuery.Register("ANY_OBELISK_BUILT", IsAnyObeliskBuilt);
		}

		// Accepts one required location parameter. May be 'Here', 'Target', 'All', or the name of a location.
		private static bool IsAnyObeliskBuilt(string[] query, GameStateQueryContext context)
		{
			if (!ArgUtility.TryGet(query, 1, out var locationName, out var err, false))
				return GameStateQuery.Helpers.ErrorResult(query, err);

			bool allLocations = string.Equals(locationName, "All", StringComparison.OrdinalIgnoreCase);

			if (!allLocations)
			{
				var location = GameStateQuery.Helpers.GetLocation(locationName, context.Location);
				if (location is null)
					return GameStateQuery.Helpers.ErrorResult(query, 
						$"required index 2 has '{locationName}', which doesn't match an existing location name or one of the special keys (All, Here, or Target)");

				if (location is IslandWest island && island.farmObelisk.Value)
					return true;

				foreach (var building in location.buildings)
					if (building.buildingType.Value.Contains("obelisk", StringComparison.OrdinalIgnoreCase))
						return true;

				return false;
            }

			foreach (var location in Game1.locations)
				if (location is IslandWest island && island.farmObelisk.Value)
					return true;
				else
                    foreach (var building in location.buildings)
						if (building.buildingType.Value.Contains("obelisk", StringComparison.OrdinalIgnoreCase))
							return true;

			return false;
        }
	}
}

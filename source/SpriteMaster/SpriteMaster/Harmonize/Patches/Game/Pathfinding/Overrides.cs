/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using StardewValley;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize.Patches.Game.Pathfinding;

static partial class Pathfinding {
	[Harmonize(
		typeof(NPC),
		"getLocationRoute",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool GetLocationRoute(NPC __instance, ref List<string>? __result, string startingLocation, string endingLocation) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.OptimizeWarpPoints) {
			return true;
		}

		if (__instance is StardewValley.Monsters.Monster) {
			__result = null;
			return false;
		}

		// TODO : Handle the MensLocker/WomensLocker overrides. We effectively will need two more route maps (or one if gender can never be 'neither'.
		// && ((int)this.gender == 0 || !s.Contains<string>("BathHouse_MensLocker", StringComparer.Ordinal)) && ((int)this.gender != 0 || !s.Contains<string>("BathHouse_WomensLocker", StringComparer.Ordinal))

		if (FasterRouteMap.TryGetValue(startingLocation, out var innerRoute)) {
			if (innerRoute.TryGetValue(endingLocation, out var route) && route is not null && route.Count != 0) {
				__result = route;
				return false;
			}
		}

		return true;
	}



	[Harmonize(
		typeof(NPC),
		"populateRoutesFromLocationToLocationList",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool PopulateRoutesFromLocationToLocationList() {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.OptimizeWarpPoints) {
			return true;
		}

		var routeList = new ConcurrentBag<List<string>>();

		var locations = new Dictionary<string, GameLocation?>(Game1.locations.SelectF(location => new KeyValuePair<string, GameLocation?>(location.Name, location)));

		GameLocation? backwoodsLocation = Game1.locations.FirstOrDefaultF(location => location.Name == "Backwoods");

		// Iterate over every location in parallel, and collect all paths to every other location.
		Parallel.ForEach(Game1.locations, location => {
			if (Config.Extras.AllowNPCsOnFarm || location is not Farm && location != backwoodsLocation) {
				var route = new List<string>();
				ExploreWarpPointsImpl(location, route, routeList, locations);
			}
		});

		// Set the RoutesFromLocationToLocation list, and also generate a faster 'FasterRouteMap' to perform path lookups.
		if (RoutesFromLocationToLocationSet is not null) {
			RoutesFromLocationToLocationSet(routeList.ToList());
		}
		FasterRouteMap.Clear();
		foreach (var route in routeList) {
			var innerRoutes = FasterRouteMap.GetOrAddDefault(route.FirstF(), () => new Dictionary<string, List<string>>());
			innerRoutes![route.LastF()] = route;
		}

		return false;
	}

	[Harmonize(
		typeof(NPC),
		"exploreWarpPoints",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	[MethodImpl(Runtime.MethodImpl.Hot)]
	public static bool ExploreWarpPoints(ref bool __result, GameLocation l, List<string> route) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.OptimizeWarpPoints) {
			return true;
		}

		// RoutesFromLocationToLocation is always a new list when first entering this method
		var routeList = new ConcurrentBag<List<string>>();

		var locations = new Dictionary<string, GameLocation?>(Game1.locations.SelectF(location => new KeyValuePair<string, GameLocation?>(location.Name, location)));

		// Single location pathing search.
		__result = ExploreWarpPointsImpl(l, route, routeList, locations);

		if (RoutesFromLocationToLocationSet is not null) {
			RoutesFromLocationToLocationSet(routeList.ToList());
		}
		FasterRouteMap.Clear();
		foreach (var listedRoute in routeList) {
			var innerRoutes = FasterRouteMap.GetOrAddDefault(listedRoute.FirstF(), () => new Dictionary<string, List<string>>());
			innerRoutes![listedRoute.LastF()] = listedRoute;
		}
		return false;
	}
}

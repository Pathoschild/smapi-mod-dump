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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize.Patches.Game.Pathfinding;

internal static partial class Pathfinding {
	private static readonly Action<List<List<string>>>? RoutesFromLocationToLocationSetter = typeof(NPC).GetFieldSetter<List<List<string>>>("routesFromLocationToLocation");

	private static bool RoutesFromLocationToLocationSet(ConcurrentBag<List<string>> routes) {
		if (RoutesFromLocationToLocationSetter is not {} func) {
			return false;
		}

		lock (PathLock) {
			func(routes.ToList());
			return true;
		}
	}

	static Pathfinding() {
		if (RoutesFromLocationToLocationSetter is null) {
			Debug.Warning("Could not find 'NPC.routesFromLocationToLocation'");
		}
	}

	[Harmonize(
		typeof(NPC),
		"getLocationRoute",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool GetLocationRoute(NPC __instance, ref List<string>? __result, string startingLocation, string endingLocation) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.Pathfinding.OptimizeWarpPoints) {
			return true;
		}

		if (__instance is StardewValley.Monsters.Monster) {
			__result = null;
			return false;
		}

		// TODO : Handle the MensLocker/WomensLocker overrides. We effectively will need two more route maps (or one if gender can never be 'neither'.
		// && ((int)this.gender == 0 || !s.Contains<string>("BathHouse_MensLocker", StringComparer.Ordinal)) && ((int)this.gender != 0 || !s.Contains<string>("BathHouse_WomensLocker", StringComparer.Ordinal))

		if (FasterRouteMap.TryGetValue(startingLocation, out var innerRoute)) {
			if (innerRoute.TryGetValue(endingLocation, out var route) && route.Count != 0) {
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
	public static bool PopulateRoutesFromLocationToLocationList() {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.Pathfinding.OptimizeWarpPoints) {
			return true;
		}

		var routeList = new ConcurrentBag<List<string>>();

		var locations = new Dictionary<string, GameLocation>(Game1.locations.WhereF(location => location is not null).SelectF(location => new KeyValuePair<string, GameLocation>(location.Name, location)));

		GameLocation? backwoodsLocation = Game1.locations.FirstOrDefaultF(location => location.Name == "Backwoods");

		// Iterate over every location in parallel, and collect all paths to every other location.
		Parallel.ForEach(Game1.locations, location => {
			if (Config.Extras.Pathfinding.AllowNPCsOnFarm || location is not Farm && !ReferenceEquals(location, backwoodsLocation)) {
				var route = new List<string>();
				ExploreWarpPointsImpl(location, route, routeList, locations);
			}
		});

		// Set the RoutesFromLocationToLocation list, and also generate a faster 'FasterRouteMap' to perform path lookups.
		RoutesFromLocationToLocationSet(routeList);
		FasterRouteMap.Clear();
		foreach (var route in routeList) {
			var innerRoutes = FasterRouteMap.GetOrAddDefault(route.FirstF(), () => new());
			innerRoutes[route.LastF()] = route;
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
	public static bool ExploreWarpPoints(ref bool __result, GameLocation l, List<string> route) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.Pathfinding.OptimizeWarpPoints) {
			return true;
		}

		// RoutesFromLocationToLocation is always a new list when first entering this method
		var routeList = new ConcurrentBag<List<string>>();

		var locations = new Dictionary<string, GameLocation>(Game1.locations.WhereF(location => location is not null).SelectF(location => new KeyValuePair<string, GameLocation>(location.Name, location)));

		// Single location pathing search.
		__result = ExploreWarpPointsImpl(l, route, routeList, locations);

		RoutesFromLocationToLocationSet(routeList);
		FasterRouteMap.Clear();
		foreach (var listedRoute in routeList) {
			var innerRoutes = FasterRouteMap.GetOrAddDefault(listedRoute.FirstF(), () => new());
			innerRoutes[listedRoute.LastF()] = listedRoute;
		}
		return false;
	}
}

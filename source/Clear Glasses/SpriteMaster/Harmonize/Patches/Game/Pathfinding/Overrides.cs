/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

//#define VALIDATE_ROUTES

using HarmonyLib;
using LinqFasterer;
using Microsoft.Toolkit.HighPerformance.Helpers;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Types.Exceptions;
using SpriteMaster.Types.Reflection;
using StardewValley;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize.Patches.Game.Pathfinding;

internal static partial class Pathfinding {
	private static readonly VariableInfo? RoutesFromLocationToLocationInfo = typeof(NPC).GetStaticVariable("routesFromLocationToLocation");
	private static readonly VariableStaticAccessor<List<List<string>>>? RoutesFromLocationToLocation = RoutesFromLocationToLocationInfo?.GetStaticAccessor<List<List<string>>>();

	private static bool RoutesFromLocationToLocationSet(List<List<string>> routes) {
		if (RoutesFromLocationToLocation is not {} accessor) {
			return false;
		}

		lock (PathLock) {
			accessor.Value = routes;
			return true;
		}
	}

	static Pathfinding() {
		if (RoutesFromLocationToLocation is null) {
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

		// TODO : Handle the MensLocker/WomensLocker overrides. We effectively will need two more route maps (or one if gender can never be 'neither').
		// && ((int)this.gender == 0 || !s.Contains<string>("BathHouse_MensLocker", StringComparer.Ordinal)) && ((int)this.gender != 0 || !s.Contains<string>("BathHouse_WomensLocker", StringComparer.Ordinal))

		if (FasterRouteMap.TryGetRoute((startingLocation, endingLocation), (Gender)__instance.Gender, out __result)) {
			return false;
		}

		return true;
	}

#if VALIDATE_ROUTES
	[MethodImpl(MethodImplOptions.NoInlining)]
	[Harmonize(
		typeof(NPC),
		"populateRoutesFromLocationToLocationList",
		Harmonize.Fixation.Reverse,
		instance: false,
		critical: false
	)]
	public static void PopulateRoutesFromLocationToLocationListReverse() {
		throw new ReversePatchException();
	}

	private static bool RealPopulate = false;
#endif



	[Conditional("VALIDATE_ROUTES")]
	private static void ValidateRoutes(List<List<string>> referenceRoutes, List<List<string>> calculatedRoutes) {
		Dictionary<(string Start, string End), List<string>> referenceRoutesMap = new(referenceRoutes.Count);
		foreach (var route in referenceRoutes) {
			if (!referenceRoutesMap.TryAdd((route.FirstF(), route.LastF()), route)) {
				if (route.Count < referenceRoutesMap[(route.FirstF(), route.LastF())].Count) {
					referenceRoutesMap[(route.FirstF(), route.LastF())] = route;
				}
			}
		}

		var duplicateRoutes = new HashSet<(string Start, string End)>();
		var mismatchedRoutes = new Dictionary<(string Start, string End), (List<string> Reference, List<string> Calculated)>();


		Dictionary<(string Start, string End), List<string>> calculatedRoutesMap = new(calculatedRoutes.Count);
		foreach (var route in calculatedRoutes) {
			if (route.ContainsF("BathHouse_MensLocker") || route.ContainsF("BathHouse_WomensLocker")) {
				continue;
			}

			if (!calculatedRoutesMap.TryAdd((route.FirstF(), route.LastF()), route)) {
				duplicateRoutes.Add((route.FirstF(), route.LastF()));
			}
		}

		if (referenceRoutesMap.Count != calculatedRoutesMap.Count) {
			Debug.Error($"Route Counts mismatch");
		}
		foreach (var routePair in referenceRoutesMap) {
			if (routePair.Value.ContainsF("BathHouse_MensLocker") || routePair.Value.ContainsF("BathHouse_WomensLocker")) {
				continue;
			}

			if (!calculatedRoutesMap.TryGetValue(routePair.Key, out var calculatedRoute)) {
				Debug.Error($"Calculated Routes Map missing '{routePair.Key}'");
				continue;
			}

			if (!routePair.Value.SequenceEqualF(calculatedRoute)) {
				mismatchedRoutes.TryAdd(routePair.Key, (routePair.Value, calculatedRoute));
			}
		}
		foreach (var routePair in calculatedRoutesMap) {
			if (routePair.Value.ContainsF("BathHouse_MensLocker") || routePair.Value.ContainsF("BathHouse_WomensLocker")) {
				continue;
			}

			if (!referenceRoutesMap.TryGetValue(routePair.Key, out var referenceRoute)) {
				Debug.Error($"Reference Routes Map missing '{routePair.Key}'");
				continue;
			}

			if (!routePair.Value.SequenceEqualF(referenceRoute)) {
				mismatchedRoutes.TryAdd(routePair.Key, (referenceRoute, routePair.Value));
			}
		}

		/*
		if (duplicateRoutes.Count != 0) {
			Debug.Error("Duplicate Routes:");
			foreach (var route in duplicateRoutes) {
				Debug.Error($"Duplicate Route {(route.Start, route.End)}");
			}
		}
		*/

		if (mismatchedRoutes.Count != 0) {
			Debug.Error("Mismatched Routes:");
			foreach (var (key, routes) in mismatchedRoutes) {
				Debug.Error($"Route '{key}' mismatch:\n  ref: {string.Join(" : ", routes.Reference)}\n  clc: {string.Join(" : ", routes.Calculated)}");
			}
		}
	}

	[Harmonize(
		typeof(NPC),
		"populateRoutesFromLocationToLocationList",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool PopulateRoutesFromLocationToLocationListPrefix() {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.Pathfinding.OptimizeWarpPoints) {
			return true;
		}

#if VALIDATE_ROUTES
		RealPopulate = true;
		try {
			PopulateRoutesFromLocationToLocationListReverse();
		}
		finally {
			RealPopulate = false;
		}

		var referenceRoutes = RoutesFromLocationToLocation!?.Value!;
#endif

		var locations = new Dictionary<string, GameLocation>(Game1.locations.WhereF(location => location is not null).SelectF(location => new KeyValuePair<string, GameLocation>(location.Name, location)));

		var routeList = new RouteList(locations.Count);

		GameLocation? backwoodsLocation = Game1.locations.FirstOrDefaultF(location => location.Name == "Backwoods");

		GenderedTuple<ConcurrentDictionary<RouteKey, List<string>>>? concurrentRoutes =
			SMConfig.Extras.Pathfinding.EnableCrossThreadOptimizations ?
				new(new(), new(), new()) :
				null;

		// Iterate over every location in parallel, and collect all paths to every other location.
		Parallel.ForEach(Game1.locations, location => {
			if (location is not Farm && !ReferenceEquals(location, backwoodsLocation)) {
				ExploreWarpPointsImpl(location, in routeList, locations, concurrentRoutes);
			}
		});

		// Set the RoutesFromLocationToLocation list, and also generate a faster 'FasterRouteMap' to perform path lookups.
		FasterRouteMap.Clear();
		var allRoutes = FasterRouteMap.Add(in routeList);
		RoutesFromLocationToLocationSet(allRoutes);

#if VALIDATE_ROUTES
		ValidateRoutes(referenceRoutes, routeList.General.Concat(routeList.Male).Concat(routeList.Female).ToList());
#endif

		return false;
	}

	[Harmonize(
		typeof(NPC),
		"populateRoutesFromLocationToLocationList",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static void PopulateRoutesFromLocationToLocationListPostfix() {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.Pathfinding.OptimizeWarpPoints) {
			return;
		}

		UpdateWarpPointsReverse(null);
	}

	[Harmonize(
		typeof(NPC),
		"exploreWarpPoints",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool ExploreWarpPointsPre(ref bool __result, GameLocation l, List<string> route) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.Pathfinding.OptimizeWarpPoints) {
			return true;
		}

#if VALIDATE_ROUTES
		if (RealPopulate) {
			return true;
		}
#endif

		// RoutesFromLocationToLocation is always a new list when first entering this method

		var locations = new Dictionary<string, GameLocation>(
			Game1.locations
				.WhereF(location => location is not null)
				.SelectF(location => new KeyValuePair<string, GameLocation>(location.Name, location))
		);

		var routeList = new RouteList(locations.Count);

		// Single location pathing search.
		__result = ExploreWarpPointsImpl(l, in routeList, locations);
		route.Clear();

		FasterRouteMap.Clear(l.Name);
		var allRoutes = FasterRouteMap.Add(in routeList);
		RoutesFromLocationToLocationSet(allRoutes);

		return false;
	}

	[Harmonize(
		typeof(NPC),
		"exploreWarpPoints",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static void ExploreWarpPointsPost(ref bool __result, GameLocation l, List<string> route) {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.Pathfinding.OptimizeWarpPoints) {
			return;
		}

#if VALIDATE_ROUTES
		if (RealPopulate) {
			return;
		}
#endif

		UpdateWarpPointsReverse(l);
		route.Clear();
	}

	private static void UpdateWarpPointsReverse(GameLocation? l) {
		if (RoutesFromLocationToLocation is not { HasGetter: true } routes) {
			return;
		}

		bool honorGender = SMConfig.Extras.Pathfinding.HonorGenderLocking;

		var gameRoutes = routes.Value;

		foreach (var route in gameRoutes.ReverseF()) {
			if (l is not null && route[0] != l.Name) {
				continue;
			}
			bool male = honorGender && MaleLocations.AnyF(route.Contains);
			bool female = honorGender && FemaleLocations.AnyF(route.Contains);
			switch (male, female) {
				case (true, false):
					RouteMap.Add(FasterRouteMap.Male, route, route);
					break;
				case (false, true):
					RouteMap.Add(FasterRouteMap.Female, route, route);
					break;
				default:
					RouteMap.Add(FasterRouteMap.General, route, route);
					break;
			}
		}
	}
}

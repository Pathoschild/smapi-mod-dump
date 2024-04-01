/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using LinqFasterer;
using Microsoft.VisualBasic.CompilerServices;
using Priority_Queue;
using SpriteMaster.Extensions;
using SpriteMaster.Types.Pooling;
using StardewValley;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using xTile.Dimensions;

namespace SpriteMaster.Harmonize.Patches.Game.Pathfinding;

using DoorPair = KeyValuePair<XNA.Point, string>;

[UsedImplicitly]
internal static partial class Pathfinding {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool GetTarget(this Warp? warp, Dictionary<string, GameLocation> locations, [NotNullWhen(true)] out GameLocation? target) {
		if (warp?.TargetName is not {} targetName) {
			target = null;
			return false;
		}

		// Warps can never path to "Volcano", and can only path to certain Locations when it's explicitly allowed in the settings.
		switch (targetName) {
			case "Volcano":
			case "Farm":
			case "Woods":
			case "Backwoods":
			case "Tunnel":
				target = null;
				return false;
			case "BoatTunnel":
				targetName = "IslandSouth";
				break;
		}

		return locations.TryGetValue(targetName, out target);
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool GetTarget(this in DoorPair door, Dictionary<string, GameLocation> locations, [NotNullWhen(true)] out GameLocation? target) {
		var targetName = door.Value;
		if (targetName == "BoatTunnel") {
			targetName = "IslandSouth";
		}

		return locations.TryGetValue(targetName, out target);
	}

	private sealed class QueueLocation {
		internal readonly GameLocation Location;
		internal QueueLocation? Previous = null;
		internal int ListDistance = int.MaxValue;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal QueueLocation(GameLocation location) => Location = location;

		internal readonly struct Comparer : IEqualityComparer<QueueLocation> {
			[MethodImpl(Runtime.MethodImpl.Inline)]
			public readonly bool Equals(QueueLocation? x, QueueLocation? y) => ReferenceEquals(x?.Location, y?.Location);
			[MethodImpl(Runtime.MethodImpl.Inline)]
			public readonly int GetHashCode(QueueLocation obj) => obj.Location.GetHashCode();
		}
	}

	private static List<string>? Dijkstra(GameLocation start, GameLocation end, Dictionary<string, GameLocation> locations, string[] filter) {
		try {
			// Get a dummy NPC to pass to the sub-pathfinders to validate routes to warps/doors.
			var queue = new SimplePriorityQueue<QueueLocation, int>(
				priorityComparer: [MethodImpl(Runtime.MethodImpl.Inline)] (x, y) => x - y,
				itemEquality: new QueueLocation.Comparer()
			);
			var startQueueLocation = new QueueLocation(start) { ListDistance = 0 };
			var queueDataMap = new Dictionary<GameLocation, QueueLocation>(locations.Count) {
				[start] = startQueueLocation
			};

			var processedSet = new HashSet<QueueLocation>(locations.Count) {startQueueLocation};

			queue.Enqueue(startQueueLocation, 0);

			foreach (var location in Game1.locations) {
				if (filter.ContainsFast(location.Name)) {
					continue;
				}

				var queueLocation = new QueueLocation(location);

				if (queueDataMap.TryAdd(location, queueLocation)) {
					//queue.Enqueue(queueLocation, int.MaxValue);
				}
			}

			while (queue.Count != 0) {
				var distance = queue.GetPriority(queue.First);

				// There was no valid path to the destination.
				if (distance == int.MaxValue) {
					return null;
				}

				var qLocation = queue.Dequeue();

				if (filter.ContainsFast(qLocation.Location.Name)) {
					continue;
				}

				// Once we've reached the end node, traverse in reverse over the previous instances, to build a route
				if (ReferenceEquals(qLocation.Location, end)) {
					var result = new string[qLocation.ListDistance + 1];

					QueueLocation? current = qLocation;
					int insertionIndex = qLocation.ListDistance;
					while (current is { } qCurrent) {
						result[insertionIndex--] = qCurrent.Location.Name;
						current = qCurrent.Previous;
					}

					return result.BeList();
				}

				// Process a neighboring node and potentially add it to the queue
				[MethodImpl(Runtime.MethodImpl.Inline)]
				void ProcessNeighbor(GameLocation node) {
					if (!queueDataMap.TryGetValue(node, out var dataNode)) {
						return;
					}

					// Calculate the distance
					var nodeDistance = distance + 1;

					if (!queue.Contains(dataNode)) {
						if (!processedSet.Add(dataNode)) {
							return;
						}

						dataNode.ListDistance = qLocation.ListDistance + 1;
						dataNode.Previous = qLocation;
						queue.Enqueue(dataNode, nodeDistance);
					}
					else if (nodeDistance < queue.GetPriority(dataNode)) {
						dataNode.ListDistance = qLocation.ListDistance + 1;
						dataNode.Previous = qLocation;
						queue.UpdatePriority(dataNode, nodeDistance);
					}
				}

				foreach (var warp in qLocation.Location.warps) {
					if (!warp.GetTarget(locations, out var neighbor)) {
						continue;
					}

					ProcessNeighbor(neighbor);
				}

				foreach (var door in qLocation.Location.doors.Pairs) {
					if (!door.GetTarget(locations, out var neighbor)) {
						continue;
					}

					ProcessNeighbor(neighbor);
				}
			}
		}
		catch (Exception ex) {
			Debug.Error("Exception generating warp points route list", ex);
		}

		return null; // Also no path
	}

	private enum RouteGender {
		General,
		Male,
		Female
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static RouteGender GetRouteGender(this List<string> route, bool honorGender) {
		if (honorGender) {
			if (MaleLocations.AnyF(route.Contains)) {
				return RouteGender.Male;
			}

			if (FemaleLocations.AnyF(route.Contains)) {
				return RouteGender.Female;
			}
		}

		return RouteGender.General;
	}

	private readonly struct RouteContainerWrapper : IDisposable {
		private readonly bool HonorGender;
		private readonly GenderedTuple<DefaultPooledObject<HashSet<string>>> FoundPooled;

		internal readonly GenderedTuple<HashSet<string>> Found;
		internal readonly GenderedTuple<ConcurrentBag<List<string>>> Routes;
		internal readonly GenderedTuple<ConcurrentDictionary<RouteKey, List<string>>>? ConcurrentRoutes;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly HashSet<string> GetFoundSet(RouteGender gender) {
			if (!HonorGender) {
				return Found.General;
			}

			return gender switch {
				RouteGender.General => Found.General,
				RouteGender.Male => Found.Male,
				RouteGender.Female => Found.Female,
				_ => ThrowHelper.ThrowArgumentOutOfRangeExceptionFromValue<HashSet<string>>(gender)
			};
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly ConcurrentBag<List<string>> GetRoutes(RouteGender gender) {
			if (!HonorGender) {
				return Routes.General;
			}

			return gender switch {
				RouteGender.General => Routes.General,
				RouteGender.Male => Routes.Male,
				RouteGender.Female => Routes.Female,
				_ => ThrowHelper.ThrowArgumentOutOfRangeExceptionFromValue<ConcurrentBag<List<string>>>(gender)
			};
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly ConcurrentDictionary<RouteKey, List<string>>? GetConcurrentRoutes(RouteGender gender) {
			if (ConcurrentRoutes is not { } concurrentRoutes) {
				return null;
			}

			if (!HonorGender) {
				return concurrentRoutes.General;
			}

			return gender switch {
				RouteGender.General => concurrentRoutes.General,
				RouteGender.Male => concurrentRoutes.Male,
				RouteGender.Female => concurrentRoutes.Female,
				_ => ThrowHelper.ThrowArgumentOutOfRangeExceptionFromValue<ConcurrentDictionary<RouteKey, List<string>>?>(gender)
			};
		}

		internal readonly record struct RouteSet(
			HashSet<string> Found,
			ConcurrentBag<List<string>> Routes,
			ConcurrentDictionary<RouteKey, List<string>>? ConcurrentRoutes
		);

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly RouteSet Get(RouteGender gender) {
			if (!HonorGender) {
				return new(Found.General, Routes.General, ConcurrentRoutes?.General);
			}

			return gender switch {
				RouteGender.General => new(Found.General, Routes.General, ConcurrentRoutes?.General),
				RouteGender.Male => new(Found.Male, Routes.Male, ConcurrentRoutes?.Male),
				RouteGender.Female => new(Found.Female, Routes.Female, ConcurrentRoutes?.Female),
				_ => ThrowHelper.ThrowArgumentOutOfRangeExceptionFromValue<RouteSet>(gender)
			};
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal readonly RouteSet Get(List<string> route) =>
			Get(route.GetRouteGender(HonorGender));

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal RouteContainerWrapper(
			bool honorGender,
			in RouteList routeList,
			in GenderedTuple<ConcurrentDictionary<RouteKey, List<string>>>? concurrentRoutes
		) {
			HonorGender = honorGender;

			int routeListCapacity = routeList.Capacity;
			void ClearAndSetCapacity(HashSet<string> set) {
				set.Clear();
				set.EnsureCapacity(routeListCapacity);
			}

			FoundPooled = new(
				ObjectPoolExt.Take<HashSet<string>>(ClearAndSetCapacity),
				ObjectPoolExt.Take<HashSet<string>>(ClearAndSetCapacity),
				ObjectPoolExt.Take<HashSet<string>>(ClearAndSetCapacity)
			);

			Found = new(
				FoundPooled.General.Value,
				FoundPooled.Male.Value,
				FoundPooled.Female.Value
			);

			Routes = new(
				routeList.General,
				routeList.Male,
				routeList.Female
			);

			ConcurrentRoutes = concurrentRoutes;
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public readonly void Dispose() {
			foreach (var pooled in FoundPooled) {
				pooled.Dispose();
			}
		}
	}

	private static bool ExploreWarpPointsImpl(
		GameLocation startLocation,
		in RouteList routeList,
		Dictionary<string, GameLocation> locations,
		GenderedTuple<ConcurrentDictionary<RouteKey, List<string>>>? concurrentRoutes = null
	) {
		try {
			bool honorGender = SMConfig.Extras.Pathfinding.HonorGenderLocking;

			using RouteContainerWrapper routeWrapper = new(
				honorGender,
				routeList,
				concurrentRoutes
			);

			[MethodImpl(Runtime.MethodImpl.Inline)]
			void AddToRouteList(RouteGender gender, List<string> route, bool check = true) {
				var (
					found,
					routes,
					concurrentMap
					) = routeWrapper.Get(gender);

				// Do not add the route if it's already been found
				if (check && !found.Add(route.LastF())) {
					return;
				}

				routes.Add(route);
				// Add the route to the concurrent map so that other tasks are aware of it
				concurrentMap?.TryAdd(route.GetRouteKey(), route);

				// This adds a subroute (an internal range of an existing route)
				[MethodImpl(Runtime.MethodImpl.Inline)]
				void AddSubList(List<string> subRoute, bool toThis) {
					var (_, subList, subConcurrentMap) = routeWrapper.Get(subRoute);

					// Only add it to our own list if specified
					if (toThis) {
						subList?.Add(subRoute);
					}

					subConcurrentMap?.TryAdd(subRoute.GetRouteKey(), subRoute);
				}

				// Repeatedly remove the last element from the route, and add it into the routeList. This allows us to bypass having to recalculate for smaller paths in many cases,
				// as we've already calculated them.
				for (int len = route.Count - 1; len >= 2; --len) {
					if (!found.Add(route[len - 1])) {
						break;
					}

					var subList = route.GetRange(0, len);
					AddSubList(subList, true);
				}

				// Only process routes that don't start from our starting location if we are processing concurrent routes for tasks
				if (concurrentRoutes is not null) {
					for (int start = 1; start < route.Count - 1; ++start) {
						for (int len = route.Count - start; len >= 2; --len) {
							var subList = route.GetRange(start, len);
							// Do not add these to our own route list.
							AddSubList(subList, false);
						}
					}
				}
			}

			// Iterate over each location, performing a recursive Dijkstra traversal on each.
			foreach (var location in Game1.locations) {
				if (startLocation.Equals(location)) {
					continue;
				}

				// If we've already found a (non-gendered) path to this location, there is no reason to process it again.
				if (!routeWrapper.Found.General.Add(location.Name)) {
					continue;
				}

				List<string> result;

				bool haveMale = false;
				bool haveFemale = false;

				// Check the concurrent routes to see if another task has already generated this particular route
				if (routeWrapper.ConcurrentRoutes is { } concurrentRoutesValue) {
					var routeKey = new RouteKey(startLocation.Name, location.Name);

					// Check for a general result
					if (concurrentRoutesValue.General.TryGetValue(routeKey, out var concurrentGeneralResult)) {
						haveMale = true;
						haveFemale = true;
						AddToRouteList(RouteGender.General, concurrentGeneralResult, check: false);
					}
					else if (honorGender) {
						// Otherwise, check for gendered results
						if (concurrentRoutesValue.Male.TryGetValue(routeKey, out var concurrentMaleResult)) {
							haveMale = true;
							AddToRouteList(RouteGender.Male, concurrentMaleResult);
						}

						if (concurrentRoutesValue.Female.TryGetValue(routeKey, out var concurrentFemaleResult)) {
							haveFemale = true;
							AddToRouteList(RouteGender.Female, concurrentFemaleResult);
						}
					}
				}

				// If we've found routes for both males and females (or a general route) then we're already done.
				if (haveMale && haveFemale) {
					continue;
				}

				if (honorGender) {
					// If we have only a male or a female result, try to calculate a result for the opposing gender
					if (haveMale || haveFemale) {
						if (haveMale) {
							if (!MaleLocations.ContainsF(startLocation.Name) && !MaleLocations.ContainsF(location.Name)) {
								if (Dijkstra(startLocation, location, locations, MaleLocations) is { } femaleResult) {
									haveFemale = true;
									AddToRouteList(RouteGender.Female, femaleResult);
								}
							}
						}
						else {
							if (!FemaleLocations.ContainsF(startLocation.Name) && !FemaleLocations.ContainsF(location.Name)) {
								if (Dijkstra(startLocation, location, locations, FemaleLocations) is { } maleResult) {
									haveMale = true;
									AddToRouteList(RouteGender.Male, maleResult);
								}
							}
						}

						// If we have routes for both genders, continue.
						if (haveMale && haveFemale) {
							continue;
						}
					}
				}

				if (Dijkstra(startLocation, location, locations, Array.Empty<string>()) is { } calculatedResult) {
					result = calculatedResult;
				}
				else {
					continue;
				}

				bool containsMale = honorGender && MaleLocations.AnyF(result.Contains);
				bool containsFemale = honorGender && FemaleLocations.AnyF(result.Contains);
				switch (containsMale, containsFemale) {
					case (true, true): {
						// Both?!
						// Female
						if (!haveFemale && !MaleLocations.ContainsF(startLocation.Name) &&
								!MaleLocations.ContainsF(location.Name)) {
							if (Dijkstra(startLocation, location, locations, MaleLocations) is { } femaleResult) {
								AddToRouteList(RouteGender.Female, femaleResult);
							}
						}

						// Male
						if (!haveMale && !FemaleLocations.ContainsF(startLocation.Name) &&
								!FemaleLocations.ContainsF(location.Name)) {
							if (Dijkstra(startLocation, location, locations, FemaleLocations) is { } maleResult) {
								AddToRouteList(RouteGender.Male, maleResult);
							}
						}
					}
						break;
					case (true, false): {
						// Male
						AddToRouteList(RouteGender.Male, result);

						if (!haveFemale && !MaleLocations.ContainsF(startLocation.Name) &&
								!MaleLocations.ContainsF(location.Name)) {
							if (Dijkstra(startLocation, location, locations, MaleLocations) is { } femaleResult) {
								AddToRouteList(RouteGender.Female, femaleResult);
							}
						}
					}
						break;
					case (false, true): {
						// Female
						AddToRouteList(RouteGender.Female, result);

						if (!haveMale && !FemaleLocations.ContainsF(startLocation.Name) &&
								!FemaleLocations.ContainsF(location.Name)) {
							if (Dijkstra(startLocation, location, locations, FemaleLocations) is { } maleResult) {
								AddToRouteList(RouteGender.Male, maleResult);
							}
						}
					}
						break;
					case (false, false):
						AddToRouteList(RouteGender.General, result, check: false);
						break;
				}
			}

			return true;
		}
		catch (Exception ex) {
			Debug.Error($"Exception in pathfinder: {startLocation.Name}", ex);
			throw;
		}
	}
}

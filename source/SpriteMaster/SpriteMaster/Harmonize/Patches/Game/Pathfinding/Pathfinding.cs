/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using LinqFasterer;
using Microsoft.VisualBasic.CompilerServices;
using Priority_Queue;
using SpriteMaster.Extensions;
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
				priorityComparer: (x, y) => x - y,
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

	private static bool ExploreWarpPointsImpl(
		GameLocation startLocation,
		List<string> route,
		in RouteList routeList,
		Dictionary<string, GameLocation> locations
	) {
		using var generalFoundDisposable = ObjectPoolExt.Take<HashSet<string>>(s => s.Clear());
		var generalFound = generalFoundDisposable.Value;
		generalFound.EnsureCapacity(routeList.Capacity);
		using var maleFoundDisposable = ObjectPoolExt.Take<HashSet<string>>(s => s.Clear());
		var maleFound = maleFoundDisposable.Value;
		maleFound.EnsureCapacity(routeList.Capacity);
		using var femaleFoundDisposable = ObjectPoolExt.Take<HashSet<string>>(s => s.Clear());
		var femaleFound = femaleFoundDisposable.Value;
		femaleFound.EnsureCapacity(routeList.Capacity);

		bool honorGender = SMConfig.Extras.Pathfinding.HonorGenderLocking;

		void AddToRouteList(ConcurrentBag<List<string>> list, HashSet<string> found, List<string> route) {
			list.Add(route);

			// Repeatedly remove the last element from the route, and add it into the routeList. This allows us to bypass having to recalculate for smaller paths in many cases,
			// as we've already calculated them.
			for (int len = route.Count - 1; len >= 2; --len) {
				if (!found.Add(route[len - 1])) {
					break;
				}
				var subList = route.GetRange(0, len);
				list.Add(subList);
			}
		}

		void AddToRouteListChecked(ConcurrentBag<List<string>> list, HashSet<string> found, List<string> route) {
			if (!found.Add(route.LastF())) {
				return;
			}

			AddToRouteList(list, found, route);
		}

		// Iterate over each location, performing a recursive Dijkstra traversal on each.
		foreach (var location in Game1.locations) {
			if (startLocation.Equals(location)) {
				continue;
			}

			// If we've already found a path to this location, there is no reason to process it again.
			if (!generalFound.Add(location.Name)) {
				continue;
			}

			if (Dijkstra(startLocation, location, locations, Array.Empty<string>()) is not { } result) {
				continue;
			}

			bool containsMale = honorGender && MaleLocations.AnyF(result.Contains);
			bool containsFemale = honorGender && FemaleLocations.AnyF(result.Contains);
			switch (containsMale, containsFemale) {
				case (true, true): {
					// Both?!
					if (Dijkstra(startLocation, location, locations, MaleLocations) is { } femaleResult) {
						AddToRouteListChecked(routeList.Female, femaleFound, femaleResult);
					}
					if (Dijkstra(startLocation, location, locations, FemaleLocations) is { } maleResult) {
						AddToRouteListChecked(routeList.Male, maleFound, maleResult);
					}
				} break;
				case (true, false): {
						// Male
						AddToRouteListChecked(routeList.Male, maleFound, result);

					if (Dijkstra(startLocation, location, locations, MaleLocations) is { } femaleResult) {
						AddToRouteListChecked(routeList.Female, femaleFound, femaleResult);
					}
				} break;
				case (false, true): {
						// Female
						AddToRouteListChecked(routeList.Female, femaleFound, result);

					if (Dijkstra(startLocation, location, locations, FemaleLocations) is { } maleResult) {
						AddToRouteListChecked(routeList.Male, maleFound, maleResult);
					}
				} break;
				case (false, false):
					AddToRouteList(routeList.General, generalFound, result);
					break;
			}
		}

		return true;
	}
}

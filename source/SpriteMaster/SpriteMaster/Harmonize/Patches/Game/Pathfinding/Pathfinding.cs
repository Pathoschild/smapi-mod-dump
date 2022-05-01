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
using Priority_Queue;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static StardewValley.PathFindController;

namespace SpriteMaster.Harmonize.Patches.Game.Pathfinding;

using DoorPair = KeyValuePair<XNA.Point, string>;

static partial class Pathfinding {
	private static readonly Action<List<List<string>>>? RoutesFromLocationToLocationSet = typeof(NPC).GetFieldSetter<List<List<string>>>("routesFromLocationToLocation");
	private static readonly Dictionary<string, Dictionary<string, List<string>>> FasterRouteMap = new();

	static Pathfinding() {
		if (RoutesFromLocationToLocationSet is null) {
			Debug.Warning($"Could not find 'NPC.routesFromLocationToLocation'");
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool GetTarget(this Warp? warp, Dictionary<string, GameLocation?> locations, [NotNullWhen(true)] out GameLocation? target) {
		if (warp is null) {
			target = null;
			return false;
		}

		// Warps can never path to "Volcano", and can only path to certain Locations when it's explicitly allowed in the settings.
		if (warp.TargetName is "Volcano" || (!Config.Extras.AllowNPCsOnFarm && warp.TargetName is ("Farm" or "Woods" or "Backwoods" or "Tunnel"))) {
			target = null;
			return false;
		}

		target = locations.GetValueOrDefault(
			key: warp.TargetName switch {
				"BoatTunnel" => "IslandSouth",
				_ => warp.TargetName
			},
			defaultValue: null
		);
		return target is not null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool GetTarget(this in DoorPair door, Dictionary<string, GameLocation?> locations, [NotNullWhen(true)] out GameLocation? target) {
		if (door.Value is null) {
			target = null;
			return false;
		}

		target = locations.GetValueOrDefault(
			key: door.Value switch {
				"BoatTunnel" => "IslandSouth",
				_ => door.Value
			},
			defaultValue: null
		);
		return target is not null;
	}

	private readonly record struct PointPair(Vector2I Start, Vector2I End);
	private static readonly ConcurrentDictionary<GameLocation, ConcurrentDictionary<PointPair, int?>> CachedPathfindPoints = new();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static NPC? GetDummyNPC() {
		NPC? dummyNPC = null;
		foreach (var location in Game1.locations) {
			dummyNPC = location.getCharacters().FirstOrDefaultF(c => c is NPC);
			if (dummyNPC is not null) {
				break;
			}
		}

		return dummyNPC;
	}

	private sealed class QueueLocation {
		internal readonly GameLocation Location;
		internal QueueLocation? Previous = null;
		internal Vector2I? StartPosition = null;
		internal int ListDistance = int.MaxValue;

		internal QueueLocation(GameLocation location) => Location = location;

		internal struct Comparer : IEqualityComparer<QueueLocation> {
			[MethodImpl(Runtime.MethodImpl.Hot)]
			public bool Equals(QueueLocation? x, QueueLocation? y) => x?.Location == y?.Location;
			[MethodImpl(Runtime.MethodImpl.Hot)]
			public int GetHashCode([DisallowNull] QueueLocation obj) => obj.Location.GetHashCode();
		}
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static List<string>? Dijkstra(GameLocation start, GameLocation end, Dictionary<string, GameLocation?> locations) {
		try {
			// Get a dummy NPC to pass to the sub-pathfinders to validate routes to warps/doors.
			NPC? dummyNPC = GetDummyNPC();

			var queue = new SimplePriorityQueue<QueueLocation, int>(
				priorityComparer: (x, y) => x - y,
				itemEquality: new QueueLocation.Comparer()
			);
			var startQueueLocation = new QueueLocation(start) { ListDistance = 0 };
			var queueDataMap = new Dictionary<GameLocation, QueueLocation>(locations.Count) {
				[start] = startQueueLocation
			};

			queue.Enqueue(startQueueLocation, 0);

			foreach (var location in Game1.locations) {
				var queueLocation = new QueueLocation(location);
				if (queueDataMap.TryAdd(location, queueLocation)) {
					queue.EnqueueWithoutDuplicates(queueLocation, int.MaxValue);
				}
			}

			while (queue.Count != 0) {
				var distance = queue.GetPriority(queue.First);

				// There was no valid path to the destination.
				if (distance == int.MaxValue) {
					return null;
				}

				var qLocation = queue.Dequeue();

				// Once we've reached the end node, traverse in reverse over the previous instances, to build a route
				if (qLocation.Location == end) {
					var result = new string[qLocation.ListDistance + 1];

					QueueLocation? current = qLocation;
					int insertionIndex = qLocation.ListDistance;
					while (current is QueueLocation qCurrent) {
						result[insertionIndex--] = qCurrent.Location.Name;
						current = qCurrent.Previous;
					}

					return result.BeList();
				}

				// Process a neighboring node and potentially add it to the queue
				void ProcessNeighbor(GameLocation node, Vector2I egress, Vector2I? ingress, int length) {
					var dataNode = queueDataMap[node];

					if (!queue.Contains(dataNode)) {
						return;
					}

					int nodeDistance;

					// Calculate the distance
					if (Config.Extras.TrueShortestPath && qLocation.StartPosition is Vector2I currentPos) {
						// If we are (and can) calculate the true distance, we do that based upon egress position
						//var straightDistance = (egress - currentPos).LengthSquared;
						nodeDistance = distance + 1 + length;
					}
					else {
						// Otherwise, we assume a distance of '1' for each node (this is vanilla behavior)
						nodeDistance = distance + 1;
					}

					if (nodeDistance < queue.GetPriority(dataNode)) {
						dataNode.StartPosition = ingress;
						dataNode.ListDistance = qLocation.ListDistance + 1;
						dataNode.Previous = qLocation;
						queue.UpdatePriority(dataNode, nodeDistance);
					}
				}

				// Check if a given point within a given GameLocation is accessible from our current position.
				int? GetPointDistance(GameLocation node, Vector2I point) {
					if (!qLocation.StartPosition.HasValue) {
						return 0;
					}

					var pointPair = new PointPair(qLocation.StartPosition.Value, point);

					// Check if this ingress/point pair has already been calculated.
					var pointDictionary = CachedPathfindPoints.GetOrAdd(node, _ => new());
					if (pointDictionary.TryGetValue(pointPair, out var pathLength)) {
						return pathLength;
					}

					// Check for a valid path. Locked because the 'findPath' methods rely on some internal state within the game
					// and thus are not threadsafe
					Stack<XNA.Point>? result;

					if (qLocation.StartPosition.Value == point) {
						result = new();
						result.Push(point);
					}
					else {
						lock (node) {
							if (node.Name == "Farm") {
								try {
									result = FindPathOnFarm(qLocation.StartPosition.Value, point, node, int.MaxValue);
								}
								catch {
									result = null;
								}
							}
							else {
								try {
									result = findPathForNPCSchedules(qLocation.StartPosition.Value, point, node, int.MaxValue);
								}
								catch {
									result = null;
								}
							}
						}
					}
					int? count = result?.Count;
					pointDictionary.TryAdd(pointPair, count);

					return count;
				}

				foreach (var warp in qLocation.Location.warps) {
					if (!warp.GetTarget(locations, out var neighbor)) {
						continue;
					}

					// If the warp is not accessible from the start location, skip it. This prevents NPCs from trying to path in,
					// for instance, the backwoods to get to the bus.
					var length = GetPointDistance(qLocation.Location, (warp.X, warp.Y));
					if (!length.HasValue) {
						continue;
					}

					ProcessNeighbor(neighbor, (warp.X, warp.Y), (warp.TargetX, warp.TargetY), length.Value);
				}

				foreach (var door in qLocation.Location.doors.Pairs) {
					if (!door.GetTarget(locations, out var neighbor)) {
						continue;
					}

					Vector2I doorIngress = (door.Key.X, door.Key.Y);

					// If the warp is not accessible from the start location, skip it. This prevents NPCs from trying to path in,
					// for instance, the backwoods to get to the bus.
					var length = GetPointDistance(qLocation.Location, doorIngress);
					if (!length.HasValue) {
						continue;
					}

					// Try to find the door's egress position, and process the node.
					try {
						var warp = qLocation.Location.getWarpFromDoor(door.Key, dummyNPC);
						Vector2I? doorEgress = (warp is null) ? null : (warp.TargetX, warp.TargetY);
						ProcessNeighbor(neighbor, doorIngress, doorEgress, length.Value);
					}
					catch (Exception) {
						ProcessNeighbor(neighbor, doorIngress, null, length.Value);
					}
				}
			}
		}
		catch (Exception ex) {
			Debug.Error("Exception generating warp points route list", ex);
		}

		return null; // Also no path
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private static bool ExploreWarpPointsImpl(GameLocation startLocation, List<string> route, ConcurrentBag<List<string>> routeList, Dictionary<string, GameLocation?> locations) {
		var foundTargetsSet = new HashSet<string>(locations.Count) { startLocation.Name };

		// Iterate over each location, performing a recursive Dijkstra traversal on each.
		foreach (var location in Game1.locations) {
			// If we've already found a path to this location, there is no reason to process it again.
			if (!foundTargetsSet.Add(location.Name)) {
				continue;
			}

			var result = Dijkstra(startLocation, location, locations);
			if (result is null) {
				continue;
			}
			routeList.Add(result);

			// Repeatedly remove the last element from the route, and add it into the routeList. This allows us to bypass having to recalculate for smaller paths in many cases,
			// as we've already calculated them.
			for (int len = result.Count - 1; len >= 2; --len) {
				if (!foundTargetsSet.Add(result[len - 1])) {
					break;
				}
				var subList = result.GetRange(0, len);
				routeList.Add(subList);
			}
		}

		return true;
	}
}

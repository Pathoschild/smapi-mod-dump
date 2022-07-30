/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Hashing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Harmonize.Patches.Game.Pathfinding;

internal static partial class Pathfinding {
	private static readonly object PathLock = new();

	private static readonly RouteMap FasterRouteMap = new();

	private static readonly string[] MaleLocations = { "BathHouse_MensLocker" };
	private static readonly string[] FemaleLocations = { "BathHouse_WomensLocker" };

	[StructLayout(LayoutKind.Auto)]
	private readonly struct RouteKey : IEquatable<RouteKey> {
		internal readonly string Start;
		internal readonly string End;
		private readonly ulong LongHash;
		private readonly int Hash;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal RouteKey(string start, string end) {
			Start = start;
			End = end;

			LongHash = HashUtility.Combine(start, end);
			Hash = HashCode.Combine(start, end);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() => Hash;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in RouteKey other)
		{
			return LongHash == other.LongHash && Start == other.Start && End == other.End;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly bool IEquatable<RouteKey>.Equals(RouteKey other) {
			return LongHash == other.LongHash && Start == other.Start && End == other.End;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object? obj)
		{
			return obj is RouteKey other && Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in RouteKey left, in RouteKey right) => left.Equals(right);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in RouteKey left, in RouteKey right) => !left.Equals(right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator RouteKey(ValueTuple<string, string> value) => new(value.Item1, value.Item2);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator RouteKey(List<string> route) => new(route[0], route[^1]);
	}

	[StructLayout(LayoutKind.Auto)]
	private readonly struct RouteList {
		internal readonly ConcurrentBag<List<string>> General = new();
		internal readonly ConcurrentBag<List<string>> Male = new();
		internal readonly ConcurrentBag<List<string>> Female = new();
		internal readonly int Capacity;

		public RouteList(int capacity) {
			Capacity = capacity;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	private readonly struct RouteMap {
		internal readonly Dictionary<RouteKey, List<string>> General = new();
		internal readonly Dictionary<RouteKey, List<string>> Male = new();
		internal readonly Dictionary<RouteKey, List<string>> Female = new();

		public RouteMap() {
		}

		internal static void Add(
			Dictionary<RouteKey, List<string>> map, in RouteKey key, List<string> route
		) {
			if (!map.TryAdd(key, route)) {
				map[key] = route;
			}
		}

		internal List<List<string>> Add(in RouteList routeList) {
			foreach (var route in routeList.General) {
				Add(General, route, route);
			}

			foreach (var route in routeList.Male) {
				Add(Male, route, route);
			}

			foreach (var route in routeList.Female) {
				Add(Female, route, route);
			}

			var allRoutes = new List<List<string>>(General.Count + Male.Count + Female.Count);

			foreach (var route in General.Values) {
				allRoutes.Add(route);
			}

			foreach (var route in Male.Values) {
				allRoutes.Add(route);
			}

			foreach (var route in Female.Values) {
				allRoutes.Add(route);
			}

			return allRoutes;
		}

		private static bool TryGetRoute(
			Dictionary<RouteKey, List<string>> map,
			in RouteKey key,
			[NotNullWhen(true)] out List<string>? route
		) {
			return map.TryGetValue(key, out route);
		}

		internal bool TryGetRoute(in RouteKey key, Gender gender, [NotNullWhen(true)] out List<string>? route) {
			switch (gender) {
				case Gender.Male:
					if (TryGetRoute(Male, key, out route)) {
						return true;
					}
					break;
				case Gender.Female:
					if (TryGetRoute(Female, key, out route)) {
						return true;
					}
					break;
			}

			return TryGetRoute(General, key, out route);
		}

		internal readonly void Clear() {
			General.Clear();
			Male.Clear();
			Female.Clear();
		}

		private static void ClearFrom(Dictionary<RouteKey, List<string>> map, string location) {
			using var toRemoveDisposable = ObjectPoolExt.Take<List<RouteKey>>(l => l.Clear());
			var toRemove = toRemoveDisposable.Value;
			toRemove.Capacity = map.Count;
			foreach (var (key, value) in map) {
				if (key.Start == location) {
					toRemove.Add(key);
				}
			}

			foreach (var key in toRemove) {
				map.Remove(key);
			}
		}

		internal readonly void Clear(string location) {
			ClearFrom(General, location);
			ClearFrom(Male, location);
			ClearFrom(Female, location);
		}
	}

	private enum Gender : int {
		Male = 0,
		Female = 1
	}
}

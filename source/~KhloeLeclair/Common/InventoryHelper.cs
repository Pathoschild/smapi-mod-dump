/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_INVENTORY

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;

using Netcode;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.Common;

public record struct LocatedInventory(object Source, GameLocation? Location);

public static class InventoryHelper {

	#region Discovery

	public static List<LocatedInventory> DiscoverInventories(
		Vector2 source,
		GameLocation? location,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		Func<object, bool>? checkConnector,
		int distanceLimit = 5,
		int scanLimit = 100,
		int targetLimit = 20,
		bool includeSource = true,
		bool includeDiagonal = true,
		bool includeBuildings = false
	) {
		return DiscoverInventories(
			new AbsolutePosition(location, source),
			who,
			getProvider,
			checkConnector,
			distanceLimit,
			scanLimit,
			targetLimit,
			includeSource,
			includeDiagonal,
			includeBuildings
		);
	}

	public static List<LocatedInventory> DiscoverInventories(
		Rectangle source,
		GameLocation? location,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		Func<object, bool>? checkConnector,
		int distanceLimit = 5,
		int scanLimit = 100,
		int targetLimit = 20,
		bool includeSource = true,
		bool includeDiagonal = true,
		bool includeBuildings = false,
		int expandSource = 0
	) {
		List<AbsolutePosition> positions = [];

		for (int x = -expandSource; x < source.Width + expandSource; x++) {
			for (int y = -expandSource; y < source.Height + expandSource; y++) {
				positions.Add(new(
					location,
					new(
						source.X + x,
						source.Y + y
					)
				));
			}
		}

		return DiscoverInventories(
			positions,
			who,
			getProvider,
			checkConnector,
			distanceLimit,
			scanLimit,
			targetLimit,
			includeSource,
			includeDiagonal,
			includeBuildings
		);
	}

	public static List<LocatedInventory> DiscoverInventories(
		AbsolutePosition source,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		Func<object, bool>? checkConnector,
		int distanceLimit = 5,
		int scanLimit = 100,
		int targetLimit = 20,
		bool includeSource = true,
		bool includeDiagonal = true,
		bool includeBuildings = false,
		int expandSource = 0
	) {
		List<AbsolutePosition> potentials = [];

		if (expandSource == 0)
			potentials.Add(source);
		else {
			for (int x = -expandSource; x < expandSource; x++) {
				for (int y = -expandSource; y < expandSource; y++) {
					potentials.Add(new(source.Location, new Vector2(source.Position.X + x, source.Position.Y + y)));
				}
			}
		}

		Dictionary<AbsolutePosition, Vector2> origins = new() {
			[source] = source.Position
		};

		AddPotentials(source.Position, source.Position, source.Location, potentials, origins, distanceLimit, includeDiagonal);

		return WalkPotentials(
			potentials,
			origins,
			includeSource ? 0 : 1,
			who,
			getProvider,
			checkConnector,
			distanceLimit,
			scanLimit,
			targetLimit,
			includeDiagonal,
			includeBuildings,
			null
		);
	}

	public static List<LocatedInventory> DiscoverArea(
		GameLocation location,
		Vector2 position,
		int radius,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		bool discover_buildings = false,
		int targetLimit = 20
	) {
		// What's the most efficient way to handle this?
		int area = (int) (Math.PI * radius * radius);
		int total = 1 + location.Objects.Length + location.furniture.Count;

		if (area >= total)
			return DiscoverArea_CheckAll(location, position, radius, who, getProvider, discover_buildings, targetLimit);

		return DiscoverArea_IterArea(location, position, radius, who, getProvider, discover_buildings, targetLimit);
	}

	private static List<LocatedInventory> DiscoverArea_CheckAll(
		GameLocation location,
		Vector2 position,
		int radius,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		bool discover_buildings = false,
		int targetLimit = 20
	) {
		List<LocatedInventory> result = [];
		IInventoryProvider? provider;

		int radiusSquared = radius * radius;

		Chest? fridge = location.GetFridge(false);
		Vector2? fridgePos = location.GetFridgePosition()?.ToVector2();

		foreach (object thing in GetAllThingsInLocation(location, buildings: discover_buildings)) {
			provider = getProvider(thing);
			if (provider != null && provider.IsValid(thing, location, who)) {
				Vector2? pos = thing == fridge ? fridgePos : thing switch {
					Building bld => bld.GetBoundingBox().GetNearestPoint(position),
					Furniture furn => furn.GetBoundingBox().GetNearestPoint(position),
					SObject obj => obj.TileLocation,
					_ => null
				};

				if (pos.HasValue && Vector2.DistanceSquared(position, pos.Value) < radiusSquared) {
					result.Add(new(thing, location));
					if (result.Count >= targetLimit)
						return result;
				}
			}

			if (discover_buildings && thing is Building building && building.GetIndoors() is GameLocation loc)
				WalkIntoMap(result, loc, who, getProvider, 1000, targetLimit);
		}

		return result;
	}

	private static List<LocatedInventory> DiscoverArea_IterArea(
		GameLocation location,
		Vector2 position,
		int radius,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		bool discover_buildings = false,
		int targetLimit = 20
	) {
		List<LocatedInventory> result = [];
		IInventoryProvider? provider;

		// Check the fridge.
		var fpos = location.GetFridgePosition();
		if (fpos != null && position.Distance(fpos.Value.X, fpos.Value.Y) <= radius) {
			var fridge = location.GetFridge(false);
			provider = getProvider(fridge);
			if (provider != null && provider.IsValid(fridge, location, who))
				result.Add(new(fridge, location));
		}

		foreach (var pos in position.IterArea(radius, false)) {
			int count = result.Count;
			if (TileHelper.GetObjectAtPosition(location, pos, out SObject? obj)) {
				provider = getProvider(obj);
				if (provider != null && provider.IsValid(obj, location, who))
					result.Add(new(obj, location));
			}

			if (discover_buildings && location.getBuildingAt(pos) is Building bld) {
				provider = getProvider(bld);
				if (provider != null && provider.IsValid(bld, location, who))
					result.Add(new(bld, location));

				if (discover_buildings && bld.GetIndoors() is GameLocation loc)
					WalkIntoMap(result, loc, who, getProvider, 1000, targetLimit);
			}

			/*if (location.terrainFeatures.TryGetValue(pos, out TerrainFeature? feature)) {
				provider = getProvider(feature);
				if (provider != null && provider.IsValid(feature, location, who))
					result.Add(new(feature, location));
			}*/

			if (location.GetFurnitureAt(pos) is SObject furn) {
				provider = getProvider(furn);
				if (provider != null && provider.IsValid(furn, location, who))
					result.Add(new(furn, location));
			}

			if (result.Count >= targetLimit)
				break;
		}

		return result;
	}

	public static List<LocatedInventory> DiscoverAllLocations(
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		int scanLimit = 100,
		int targetLimit = 20,
		bool discover_buildings = false
	) {
		// We don't use the normal walker for this.
		List<LocatedInventory> result = [];

		int i = 0;
		IInventoryProvider? provider;

		IEnumerable<GameLocation> locations;
		if (Game1.IsMasterGame)
			locations = Game1.locations;
		else
			locations = Game1.Multiplayer.activeLocations();

		foreach (var location in locations) {
			foreach (object obj in GetAllThingsInLocation(location, buildings: discover_buildings)) {
				provider = getProvider(obj);
				if (provider != null && provider.IsValid(obj, location, who))
					result.Add(new(obj, location));

				i++;
				if (result.Count >= targetLimit)
					return result;
			}
		}

		return result;
	}

	private static IEnumerable<object> GetAllThingsInLocation(GameLocation location, bool objects = true, bool buildings = false, bool furniture = true, bool features = false) {
		if (location is null)
			yield break;

		if (objects && location.GetFridge(false) is Chest chest)
			yield return chest;

		if (objects)
			foreach (var obj in location.Objects.Values) {
				if (obj is not null)
					yield return obj;
			}

		if (buildings)
			foreach (var obj in location.buildings) {
				if (obj is not null)
					yield return obj;
			}

		if (furniture)
			foreach (var obj in location.furniture) {
				if (obj is not null)
					yield return obj;
			}

		if (features)
			foreach (var obj in location.terrainFeatures.Values) {
				if (obj is not null)
					yield return obj;
			}
	}

	public static List<LocatedInventory> DiscoverLocation(
		GameLocation location,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		int scanLimit = 100,
		int targetLimit = 20,
		bool discover_buildings = false,
		bool enter_buildings = false
	) {
		// We don't use the normal walker for this.
		List<LocatedInventory> result = [];

		int i = 0;

		foreach (object obj in GetAllThingsInLocation(location, buildings: discover_buildings)) {
			IInventoryProvider? provider = getProvider(obj);
			if (provider != null && provider.IsValid(obj, location, who))
				result.Add(new(obj, location));

			i++;
			if (i >= scanLimit || result.Count >= targetLimit)
				return result;
		}

		// TODO: Better handling of the scanLimit.
		if (enter_buildings)
			foreach (var building in location.buildings) {
				if (building is not null && building.GetIndoors() is GameLocation loc) {
					var output = DiscoverLocation(loc, who, getProvider, scanLimit - i, targetLimit - result.Count, discover_buildings, false);
					result.AddRange(output);
					if (result.Count >= targetLimit)
						break;
				}
			}

		return result;
	}

	public static List<LocatedInventory> DiscoverInventories(
		Rectangle source,
		GameLocation? location,
		IEnumerable<LocatedInventory>? sources,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		Func<object, bool>? checkConnector,
		int distanceLimit = 5,
		int scanLimit = 100,
		int targetLimit = 20,
		bool includeSource = true,
		bool includeDiagonal = true,
		bool includeBuildings = false,
		int expandSource = 0
	) {
		List<AbsolutePosition> positions = [];

		for (int x = -expandSource; x < source.Width + expandSource; x++) {
			for (int y = -expandSource; y < source.Height + expandSource; y++) {
				positions.Add(new(
					location,
					new(
						source.X + x,
						source.Y + y
					)
				));
			}
		}

		return DiscoverInventories(
			sources: sources,
			who: who,
			getProvider: getProvider,
			checkConnector: checkConnector,
			distanceLimit: distanceLimit,
			scanLimit: scanLimit,
			targetLimit: targetLimit,
			includeDiagonal: includeDiagonal,
			includeSource: includeSource,
			includeBuildings: includeBuildings,
			extra: positions
		);
	}

	public static List<LocatedInventory> DiscoverInventories(
		IEnumerable<LocatedInventory>? sources,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		Func<object, bool>? checkConnector,
		int distanceLimit = 5,
		int scanLimit = 100,
		int targetLimit = 20,
		bool includeSource = true,
		bool includeDiagonal = true,
		bool includeBuildings = false,
		IEnumerable<AbsolutePosition>? extra = null
	) {
		List<AbsolutePosition> potentials = [];
		Dictionary<AbsolutePosition, Vector2> origins = [];

		if (extra != null) {
			foreach (var entry in extra) {
				potentials.Add(entry);
				origins[entry] = entry.Position;
			}
		}

		List<LocatedInventory> extra_located = [];

		if (sources != null)
			foreach (LocatedInventory source in sources) {
				var provider = getProvider(source.Source);
				if (provider != null && provider.IsValid(source.Source, source.Location, who)) {
					var rect = provider.GetMultiTileRegion(source.Source, source.Location, who);
					if (rect.HasValue) {
						for (int x = 0; x < rect.Value.Width; x++) {
							for (int y = 0; y < rect.Value.Height; y++) {
								AbsolutePosition abs = new(source.Location, new(
									rect.Value.X + x,
									rect.Value.Y + y
								));

								potentials.Add(abs);
								origins[abs] = abs.Position;
							}
						}

					} else {
						var pos = provider.GetTilePosition(source.Source, source.Location, who);
						if (pos.HasValue) {
							AbsolutePosition abs = new(source.Location, pos.Value);
							potentials.Add(abs);
							origins[abs] = abs.Position;
						} else {
							// We couldn't find it, but we need to assume it's
							// still valid.
							extra_located.Add(new LocatedInventory(source.Source, source.Location));
						}
					}
				}
			}

		int count = potentials.Count;

		for (int i = 0; i < count; i++) {
			var potential = potentials[i];
			AddPotentials(
				potential.Position,
				potential.Position,
				potential.Location,
				potentials,
				origins,
				distanceLimit,
				includeDiagonal
			);
		}

		return WalkPotentials(
			potentials,
			origins,
			includeSource ? 0 : count,
			who,
			getProvider,
			checkConnector,
			distanceLimit,
			scanLimit,
			targetLimit,
			includeDiagonal,
			includeBuildings,
			extra_located
		);
	}

	public static List<LocatedInventory> DiscoverInventories(
		IEnumerable<AbsolutePosition> sources,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		Func<object, bool>? checkConnector,
		int distanceLimit = 5,
		int scanLimit = 100,
		int targetLimit = 20,
		bool includeSource = true,
		bool includeDiagonal = true,
		bool includeBuildings = false
	) {
		List<AbsolutePosition> potentials = new(sources);
		Dictionary<AbsolutePosition, Vector2> origins = [];

		foreach (AbsolutePosition source in potentials)
			origins[source] = source.Position;

		int count = potentials.Count;

		for (int i = 0; i < count; i++) {
			var potential = potentials[i];
			AddPotentials(
				potential.Position,
				potential.Position,
				potential.Location,
				potentials,
				origins,
				distanceLimit,
				includeDiagonal
			);
		}

		return WalkPotentials(
			potentials,
			origins,
			includeSource ? 0 : count,
			who,
			getProvider,
			checkConnector,
			distanceLimit,
			scanLimit,
			targetLimit,
			includeDiagonal,
			includeBuildings,
			null
		);
	}

	private static void AddPotentials(
		Vector2 source,
		Vector2 origin,
		GameLocation? location,
		IList<AbsolutePosition> potentials,
		IDictionary<AbsolutePosition, Vector2> origins,
		int distanceLimit,
		bool includeDiagonal
	) {
		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if (x == 0 && y == 0)
					continue;

				if (!includeDiagonal && x != 0 && y != 0)
					continue;

				int kx = (int) source.X + x;
				int ky = (int) source.Y + y;

				if (Math.Abs(origin.X - kx) > distanceLimit || Math.Abs(origin.Y - ky) > distanceLimit)
					continue;

				AbsolutePosition abs = new(location, new(kx, ky));
				if (!potentials.Contains(abs)) {
					potentials.Add(abs);
					origins[abs] = origin;
				}
			}
		}
	}

	private static int WalkIntoMap(
		List<LocatedInventory> result,
		GameLocation indoors,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		int scanLimit,
		int targetLimit
	) {
		// Inside maps only.
		if (indoors.IsOutdoors)
			return 0;

		int i = 0;
		IInventoryProvider? provider;

		// Iterate over all our objects.
		foreach (object obj in GetAllThingsInLocation(indoors)) {
			provider = getProvider(obj);
			if (provider != null && provider.IsValid(obj, indoors, who))
				result.Add(new(obj, indoors));

			if (result.Count >= targetLimit)
				return i;

			i++;
			if (i >= scanLimit)
				return i;
		}

		// Iterate over terrain features.
		/*foreach (var obj in indoors.terrainFeatures.Values) {
			if (obj is not null) {
				provider = getProvider(obj);
				if (provider != null && provider.IsValid(obj, indoors, who))
					result.Add(new(obj, indoors));

				if (result.Count >= targetLimit)
					return i;
			}

			i++;
			if (i >= scanLimit)
				return i;
		}*/

		return i;
	}

	private static IEnumerable<SObject> GetMapObjects(GameLocation location) {
		if (location.GetFridge() is SObject sobj)
			yield return sobj;

		foreach (var obj in location.Objects.Values)
			if (obj is not null)
				yield return obj;

		foreach (var obj in location.furniture)
			if (obj is not null)
				yield return obj;
	}

	private static List<LocatedInventory> WalkPotentials(
		List<AbsolutePosition> potentials,
		Dictionary<AbsolutePosition, Vector2> origins,
		int start,
		Farmer? who,
		Func<object, IInventoryProvider?> getProvider,
		Func<object, bool>? checkConnector,
		int distanceLimit,
		int scanLimit,
		int targetLimit,
		bool includeDiagonal,
		bool includeBuildings,
		List<LocatedInventory>? extra
	) {
		List<LocatedInventory> result = [];
		HashSet<GameLocation> visited_buildings = [];

		if (extra is not null)
			result.AddRange(extra);

		int i = start;

		if (includeBuildings) {
			foreach (var location in origins.Keys.Select(key => key.Location).Distinct()) {
				if (location is not null && !location.IsOutdoors && visited_buildings.Add(location))
					scanLimit -= WalkIntoMap(result, location, who, getProvider, scanLimit - i, targetLimit);

				if (result.Count >= targetLimit)
					return result;
			}
		}

		while (i < potentials.Count && i < scanLimit) {
			AbsolutePosition abs = potentials[i++];
			SObject? obj;
			SObject? furn;
			TerrainFeature? feature;
			Building? building;

			if (abs.Location != null) {
				TileHelper.GetObjectAtPosition(abs.Location, abs.Position, out obj);
				abs.Location.terrainFeatures.TryGetValue(abs.Position, out feature);
				furn = abs.Location.GetFurnitureAt(abs.Position);
				building = abs.Location.getBuildingAt(abs.Position);
			} else {
				feature = null;
				obj = null;
				furn = null;
				building = null;
			}

			bool want_neighbors = false;
			IInventoryProvider? provider;

			if (building != null) {
				// First off, we want to check if there are any providers to handle the
				// building directly.
				provider = getProvider(building);
				if (provider != null && provider.IsValid(building, abs.Location, who)) {
					result.Add(new(building, abs.Location));
					// We don't do connections from buildings, so no setting want_neighbors.
				}

				// Next, walk into the building if we want to do so.
				if (includeBuildings && building.HasIndoors() && building.GetIndoors() is GameLocation indoors && visited_buildings.Add(indoors))
					scanLimit -= WalkIntoMap(result, indoors, who, getProvider, scanLimit - i, targetLimit);
			}

			if (obj != null) {
				provider = getProvider(obj);
				if (provider != null && provider.IsValid(obj, abs.Location, who)) {
					result.Add(new(obj, abs.Location));
					want_neighbors = true;
				} else if (checkConnector != null && checkConnector(obj))
					want_neighbors = true;
			}

			if (feature != null) {
				provider = getProvider(feature);
				if (provider != null && provider.IsValid(feature, abs.Location, who)) {
					result.Add(new(feature, abs.Location));
					want_neighbors = true;
				} else if (!want_neighbors && checkConnector != null && checkConnector(feature))
					want_neighbors = true;
			}

			if (furn != null) {
				provider = getProvider(furn);
				if (provider != null && provider.IsValid(furn, abs.Location, who)) {
					result.Add(new(furn, abs.Location));
					want_neighbors = true;
				} else if (!want_neighbors && checkConnector != null && checkConnector(furn))
					want_neighbors = true;
			}

			if (result.Count >= targetLimit)
				break;

			if (want_neighbors)
				AddPotentials(abs.Position, origins[abs], abs.Location, potentials, origins, distanceLimit, includeDiagonal);
		}

		return result;
	}

	public static void DeduplicateInventories(ref IList<LocatedInventory> inventories, Func<object, IInventoryProvider?> getProvider) {
		HashSet<object> objects = [];
		HashSet<IInventory> seenInventories = new() { Game1.player.Items };

		for (int i = 0; i < inventories.Count; i++) {
			LocatedInventory inv = inventories[i];
			if (objects.Contains(inv.Source)) {
				inventories.RemoveAt(i);
				i--;
				continue;

			} else
				objects.Add(inv.Source);

			if (getProvider(inv.Source) is IInventoryProvider prov && prov.GetInventory(inv.Source, inv.Location, Game1.player) is IInventory iinv) {
				if (seenInventories.Contains(iinv)) {
					inventories.RemoveAt(i);
					i--;
				} else
					seenInventories.Add(iinv);
			}
		}
	}

	public static HashSet<INetRoot> GetActiveRoots() {
		HashSet<INetRoot> roots = new() {
			Game1.netWorldState,
			Game1.player.teamRoot
		};

		foreach (var root in Game1.Multiplayer.farmerRoots())
			roots.Add(root);

		if (Game1.IsClient) {
			foreach (var loc in Game1.Multiplayer.activeLocations())
				if (loc.Root is not null)
					roots.Add(loc.Root);
		} else {
			Utility.ForEachLocation(loc => {
				if (loc.Root is not null)
					roots.Add(loc.Root);

				return true;
			}, includeInteriors: false, includeGenerated: true);
		}

		return roots;
	}

	public static void RemoveInvalidInventories(ref IList<LocatedInventory> inventories, Func<object, bool> isInvalid) {
		for (int i = 0; i < inventories.Count; i++) {
			if (isInvalid(inventories[i].Source)) {
				inventories.RemoveAt(i);
				i--;
			}
		}
	}

	public static void RemoveInactiveInventories(ref IList<LocatedInventory> inventories, Func<object, IInventoryProvider?> getProvider) {
		var roots = GetActiveRoots();
		var places = Context.IsMainPlayer ? null : Game1.Multiplayer.activeLocations().ToHashSet();

		for (int i = 0; i < inventories.Count; i++) {
			LocatedInventory inv = inventories[i];
			if (places != null && inv.Location is GameLocation loc && !places.Contains(loc)) {
				inventories.RemoveAt(i);
				i--;
				continue;
			}

			var provider = getProvider(inv.Source);
			var mutex = provider?.GetMutex(inv.Source, inv.Location, Game1.player);
			if (mutex?.NetFields?.Root != null && !roots.Contains(mutex.NetFields.Root)) {
				inventories.RemoveAt(i);
				i--;
			}
		}
	}

	public static bool DoesLocationContain(GameLocation? location, object? obj) {
		if (location == null || obj == null)
			return false;

		if (location.GetFridge(false) == obj)
			return true;

		if (obj is Building bld && location.buildings.Contains(bld))
			return true;

		if (obj is Furniture furn && location.furniture.Contains(furn))
			return true;

		if (obj is TerrainFeature feature && location.terrainFeatures.Values.Contains(feature))
			return true;

		return location.Objects.Values.Contains(obj);
	}

	public static List<LocatedInventory> LocateInventories(
		IEnumerable<object> inventories,
		IEnumerable<GameLocation> locations,
		Func<object, IInventoryProvider?> getProvider,
		GameLocation? first,
		bool nullLocationValid = false
	) {
		List<LocatedInventory> result = [];

		foreach (object obj in inventories) {
			IInventoryProvider? provider = getProvider(obj);
			if (provider == null)
				continue;

			GameLocation? loc = null;

			if (first != null && DoesLocationContain(first, obj))
				loc = first;
			else {
				foreach (GameLocation location in locations) {
					if (location != first && DoesLocationContain(location, obj)) {
						loc = location;
						break;
					}
				}
			}

			if (loc != null || nullLocationValid)
				result.Add(new(obj, loc));
		}

		return result;
	}

	#endregion

	#region Unsafe Access

	public static List<IBCInventory> GetUnsafeInventories(
		IEnumerable<object> inventories,
		Func<object, IInventoryProvider?> getProvider,
		GameLocation? location,
		Farmer? who,
		bool nullLocationValid = false
	) {
		List<LocatedInventory> located = [];
		foreach (object obj in inventories) {
			if (obj is LocatedInventory inv)
				located.Add(inv);
			else
				located.Add(new(obj, location));
		}

		return GetUnsafeInventories(located, getProvider, who, nullLocationValid);
	}

	public static IEnumerable<Item> GetInventoryItems(
		IEnumerable<LocatedInventory> inventories,
		Func<object, IInventoryProvider?> getProvider,
		Farmer? who
	) {
		foreach (LocatedInventory loc in inventories) {
			if (loc.Source is not Item sourceItem)
				continue;

			IInventoryProvider? provider = getProvider(loc.Source);
			if (provider == null || !provider.IsValid(loc.Source, loc.Location, who))
				continue;

			// Try to get the mutex. If we can't, and the mutex is required,
			// then skip this entry.
			NetMutex? mutex = provider.GetMutex(loc.Source, loc.Location, who);
			if (mutex == null && provider.IsMutexRequired(loc.Source, loc.Location, who))
				continue;

			// This one seems valid, then.
			yield return sourceItem;
		}
	}

	public static List<IBCInventory> GetUnsafeInventories(
		IEnumerable<LocatedInventory> inventories,
		Func<object, IInventoryProvider?> getProvider,
		Farmer? who,
		bool nullLocationValid = false
	) {
		List<IBCInventory> result = [];

		foreach (LocatedInventory loc in inventories) {
			if (loc.Location == null && !nullLocationValid)
				continue;

			IInventoryProvider? provider = getProvider(loc.Source);
			if (provider == null || !provider.IsValid(loc.Source, loc.Location, who))
				continue;

			// Try to get the mutex. If we can't, and the mutex is required,
			// then skip this entry.
			NetMutex? mutex = provider.GetMutex(loc.Source, loc.Location, who);
			if (mutex == null && provider.IsMutexRequired(loc.Source, loc.Location, who))
				continue;

			// We don't care about the state of the mutex until we try
			// using it, and this method isn't about using it, so...
			// ignore them for now.

			WorkingInventory entry = new(loc.Source, provider, mutex, loc.Location, who);
			result.Add(entry);
		}

		return result;
	}

	#endregion

	#region Mutex Handling
#if COMMON_MUTEX

	public static void WithInventories(
		IEnumerable<LocatedInventory>? inventories,
		Func<object, IInventoryProvider?> getProvider,
		Farmer? who,
		Action<IList<IBCInventory>> withLocks,
		bool nullLocationValid = false
	) {
		WithInventories(inventories, getProvider, who, (locked, onDone) => {
			try {
				withLocks(locked);
			} catch (Exception) {
				onDone();
				throw;
			}

			onDone();
		}, nullLocationValid);
	}

	public static void WithInventories(
		IEnumerable<object>? inventories,
		Func<object, IInventoryProvider?> getProvider,
		GameLocation location,
		Farmer? who,
		Action<IList<IBCInventory>> withLocks,
		bool nullLocationValid = false
	) {
		List<LocatedInventory>? located;
		if (inventories == null)
			located = null;
		else {
			located = [];
			foreach (object obj in inventories) {
				if (obj is LocatedInventory inv)
					located.Add(inv);
				else
					located.Add(new(obj, location));
			}
		}

		WithInventories(located, getProvider, who, (locked, onDone) => {
			try {
				withLocks(locked);
			} catch (Exception) {
				onDone();
				throw;
			}

			onDone();
		}, nullLocationValid);
	}

	public static void WithInventories(
		IEnumerable<LocatedInventory>? inventories,
		Func<object, IInventoryProvider?> getProvider,
		Farmer? who,
		Action<IList<IBCInventory>, Action> withLocks,
		bool nullLocationValid = false,
		IModHelper? helper = null
	) {
		List<IBCInventory> locked = [];
		List<IBCInventory> lockable = [];

		List<IBCInventory> acquirable = [];

		if (inventories != null)
			foreach (LocatedInventory loc in inventories) {
				if (loc.Location == null && !nullLocationValid)
					continue;

				IInventoryProvider? provider = getProvider(loc.Source);
				if (provider == null || !provider.IsValid(loc.Source, loc.Location, who))
					continue;

				// If we can't get a mutex and a mutex is required, then abort.
				NetMutex? mutex = provider.GetMutex(loc.Source, loc.Location, who);
				if (mutex == null && provider.IsMutexRequired(loc.Source, loc.Location, who))
					continue;

				// Check the current state of the mutex. If someone else has
				// it locked, then we can't ensure safety. Abort.
				bool mlocked;

				if (mutex != null) {
					mlocked = mutex.IsLocked();
					if (mlocked && !mutex.IsLockHeld())
						continue;
				} else
					mlocked = true;

				WorkingInventory entry = new(loc.Source, provider, mutex, loc.Location, who);

				// How about acquire events?
				if (provider is IEventedInventoryProvider eip)
					acquirable.Add(entry);

				if (mlocked)
					locked.Add(entry);
				else
					lockable.Add(entry);
			}

		void OnDone(Action onDone) {
			if (lockable.Count == 0) {
				withLocks(locked, onDone);
				return;
			}

			List<NetMutex> mutexes = lockable.Where(entry => entry.Mutex != null).Select(entry => entry.Mutex!).Distinct().ToList();

			AdvancedMultipleMutexRequest? mmr = null;
			mmr = new AdvancedMultipleMutexRequest(
				mutexes,
				() => {
					locked.AddRange(lockable);
					withLocks(locked, () => {
						mmr?.ReleaseLock();
						mmr = null;
						onDone();
					});
				},
				() => {
					withLocks(locked, onDone);
				},
				helper: helper);
		}

		if (acquirable.Count > 0) {
			MultipleEventedInventoryExclusiveRequest? meier = null;
			meier = new MultipleEventedInventoryExclusiveRequest(
				acquirable,
				onSuccess: () => {
					OnDone(() => {
						meier?.ReleaseLock();
						meier = null;
					});
				},
				onFailure: () => {
					// Remove the acquirable inventories, because
					// they weren't locked.
					lockable.RemoveAll(inv => acquirable.Contains(inv));
					locked.RemoveAll(inv => acquirable.Contains(inv));

					// Now continue on.
					OnDone(() => { });
				},
				helper: helper);

			meier.RequestLock();

		} else
			OnDone(() => { });
	}

#endif
	#endregion

	#region Recipes and Crafting

	/// <summary>
	/// Consume an item from a list of items.
	/// </summary>
	/// <param name="matcher">A method matching the item to consume.</param>
	/// <param name="amount">The quantity to consume.</param>
	/// <param name="items">The list of items to consume the item from.</param>
	/// <param name="nullified">Whether or not any slots in the list are set
	/// to null.</param>
	/// <param name="passed_quality">Whether or not any matching slots were passed
	/// up because they exceeded the <see cref="max_quality"/></param>
	/// <param name="max_quality">The maximum quality of item to consume.</param>
	/// <param name="matchingItems">An optional list of items that, if present, we will
	/// only consume item instances in that list.</param>
	/// <param name="consumedItems">An optional list to store consumed Item stacks in.</param>
	/// <returns>The number of items remaining to consume.</returns>
	public static int ConsumeItem(Func<Item, bool> matcher, int amount, IList<Item?> items, out bool nullified, out bool passed_quality, int max_quality = int.MaxValue, IList<Item>? matchingItems = null, IList<Item>? consumedItems = null) {

		nullified = false;
		passed_quality = false;

		for (int idx = 0; idx < items.Count; idx++) {
			Item? item = items[idx];
			if (item == null || !matcher(item))
				continue;

			if (matchingItems is not null && !matchingItems.Contains(item))
				continue;

			int quality = item is SObject obj ? obj.Quality : 0;
			if (quality > max_quality) {
				passed_quality = true;
				continue;
			}

			int count = Math.Min(amount, item.Stack);
			amount -= count;

			if (consumedItems != null && count >= 0) {
				var other = item.getOne();
				other.Stack = count;
				consumedItems.Add(other);
			}

			if (item.Stack <= count) {
				items[idx] = null;
				nullified = true;

			} else
				item.Stack -= count;

			if (amount <= 0)
				return amount;
		}

		return amount;
	}

	public static int CountItem(Func<Item, bool> matcher, Farmer? who, IEnumerable<Item?>? items, out bool passed_quality, int max_quality = int.MaxValue, int? limit = null, IList<Item>? matchingItems = null) {
		int amount;

		if (who is not null)
			amount = CountItem(matcher, who.Items, out passed_quality, max_quality: max_quality, limit: limit, matchingItems: matchingItems);
		else {
			amount = 0;
			passed_quality = false;
		}

		if (limit is not null && amount >= limit)
			return amount;

		if (items is not null) {
			amount += CountItem(matcher, items, out bool pq, max_quality: max_quality, limit: limit is not null ? limit - amount : null, matchingItems: matchingItems);
			passed_quality |= pq;
		}

		return amount;
	}

	public static int CountItem(Func<Item, bool> matcher, IEnumerable<Item?> items, out bool passed_quality, int max_quality = int.MaxValue, int? limit = null, IList<Item>? matchingItems = null) {
		passed_quality = false;
		int amount = 0;

		foreach (Item? item in items) {
			if (item == null || !matcher(item))
				continue;

			int quality = item is SObject obj ? obj.Quality : 0;
			if (quality > max_quality) {
				passed_quality = true;
				continue;
			}

			if (item.Stack > 0) {
				matchingItems?.Add(item);

				amount += item.Stack;
				if (limit is not null && amount >= limit)
					return amount;
			}
		}

		return amount;
	}

	/// <summary>
	/// Consume matching items from a player, and also from a set of
	/// <see cref="IBCInventory"/> instances.
	/// </summary>
	/// <param name="items">An enumeration of <see cref="KeyValuePair{int,int}"/>
	/// instances where the first integer is the item ID to match and the
	/// second integer is the quantity to consume.
	/// <param name="who">The player to consume items from, if any.</param>
	/// <param name="inventories">An enumeration of <see cref="IBCInventory"/>
	/// instances to consume items from.</param>
	/// <param name="max_quality">The maximum quality of item to consume.</param>
	/// <param name="low_quality_first">Whether or not to consume low quality
	/// items first.</param>
	/// <param name="matchingItems">An optional list of item instances that,
	/// if included, we will only consume item instances in that list.</param>
	/// <param name="consumedItems">An optional list to append a list of
	/// consumed items to.</param>
	public static void ConsumeItems(IEnumerable<KeyValuePair<string, int>> items, Farmer? who, IEnumerable<IBCInventory>? inventories, int max_quality = int.MaxValue, bool low_quality_first = false, IList<Item>? matchingItems = null, IList<Item>? consumedItems = null) {
		if (items is null)
			return;

		ConsumeItems(
			items.Select<KeyValuePair<string, int>, (Func<Item, bool>, int)>(x => (item => CraftingRecipe.ItemMatchesForCrafting(item, x.Key), x.Value)),
			who,
			inventories,
			max_quality,
			low_quality_first,
			matchingItems,
			consumedItems
		);
	}

	internal static (IEnumerable<IBCInventory>, bool[])? GlobalModified;

	/// <summary>
	/// Consume matching items from a player, and also from a set of
	/// <see cref="IBCInventory"/> instances.
	/// </summary>
	/// <param name="items">An enumeration of tuples where the function
	/// matches items, and the integer is the quantity to consume.</param>
	/// <param name="who">The player to consume items from, if any.</param>
	/// <param name="inventories">An enumeration of <see cref="IBCInventory"/>
	/// instances to consume items from.</param>
	/// <param name="max_quality">The maximum quality of item to consume.</param>
	/// <param name="low_quality_first">Whether or not to consume low quality
	/// items first.</param>
	/// <param name="matchingItems">An optional list of item instances that,
	/// if included, we will only consume item instances in that list.</param>
	/// <param name="consumedItems">An optional list to append a list of
	/// consumed items to.</param>
	public static void ConsumeItems(IEnumerable<(Func<Item, bool>, int)> items, Farmer? who, IEnumerable<IBCInventory>? inventories, int max_quality = int.MaxValue, bool low_quality_first = false, IList<Item>? matchingItems = null, IList<Item>? consumedItems = null) {
		IList<IBCInventory>? working = (inventories as IList<IBCInventory>) ?? inventories?.ToList();

		bool is_global_modified = GlobalModified.HasValue && GlobalModified.Value.Item1 == inventories;
		bool[]? modified = working == null
			? null
			: is_global_modified
				? GlobalModified?.Item2
				: new bool[working.Count];

		IList<Item?>?[] invs = working?.Select(val => val.CanExtractItems() ? val.GetItems() : null).ToArray() ?? [];

		foreach ((Func<Item, bool>, int) pair in items) {
			Func<Item, bool> matcher = pair.Item1;
			int remaining = pair.Item2;

			int mq = max_quality;
			if (low_quality_first)
				mq = 0;

			for (int q = mq; q <= max_quality; q++) {
				bool passed;
				if (who != null)
					remaining = ConsumeItem(matcher, remaining, who.Items, out bool m, out passed, q, matchingItems, consumedItems);
				else
					passed = false;

				if (remaining <= 0)
					break;

				if (working != null)
					for (int iidx = 0; iidx < working.Count; iidx++) {
						IList<Item?>? inv = invs[iidx];
						if (inv == null || inv.Count == 0)
							continue;

						remaining = ConsumeItem(matcher, remaining, inv, out bool modded, out bool p, q, matchingItems, consumedItems);
						if (modded)
							modified![iidx] = true;

						if (p)
							passed = true;

						if (remaining <= 0)
							break;
					}

				if (remaining <= 0 || !passed)
					break;
			}
		}

		if (working != null && !is_global_modified)
			for (int idx = 0; idx < modified!.Length; idx++) {
				if (modified[idx])
					working[idx].CleanInventory();
			}
	}

	#endregion

	#region Transfer Items

	public static bool AddToInventories(IList<Item?> items, IEnumerable<IBCInventory> inventories, TransferBehavior behavior, Action<Item, int>? onTransfer = null) {
		if (behavior.Mode == TransferMode.None)
			return false;

		int[] transfered = new int[items.Count];

		foreach (IBCInventory inv in inventories) {
			if (!inv.CanInsertItems())
				continue;

			if (FillInventory(items, inv, behavior, transfered, onTransfer))
				return true;
		}

		return false;
	}

	private static bool FillInventory(IList<Item?> items, IBCInventory inventory, TransferBehavior behavior, int[] transfered, Action<Item, int>? onTransfer = null) {
		bool empty = true;

		for (int idx = 0; idx < items.Count; idx++) {
			Item? item = items[idx];
			if (item != null && item.maximumStackSize() > 1 && inventory.IsItemValid(item)) {
				// How many can we transfer?
				int count = item.Stack;
				int transfer;
				switch (behavior.Mode) {
					case TransferMode.All:
						transfer = count;
						break;
					case TransferMode.AllButQuantity:
						transfer = count - behavior.Quantity;
						break;
					case TransferMode.Half:
						transfer = count / 2;
						break;
					case TransferMode.Quantity:
						transfer = behavior.Quantity;
						break;
					case TransferMode.None:
					default:
						return false;
				}

				transfer -= transfered[idx];
				transfer = Math.Clamp(transfer, 0, count);
				if (transfer == 0)
					continue;

				int final;
				bool had_transfered = transfered[idx] > 0;
				if (AddItemToInventory(item, transfer, inventory) is null) {
					items[idx] = null;
					transfered[idx] += count;
					final = -1;
				} else {
					final = item.Stack;
					transfered[idx] += count - item.Stack;

					if (final != (count - transfer))
						empty = false;
				}

				if (count != final && !had_transfered)
					onTransfer?.Invoke(item, idx);
			} else
				empty = false;
		}

		return empty;
	}

	public static Item? AddItemToInventory(Item item, int quantity, IBCInventory inventory) {
		int initial = item.Stack;
		if (quantity > initial)
			quantity = initial;

		if (quantity <= 0)
			return item.Stack <= 0 ? null : item;

		bool present = false;

		IList<Item?>? items = inventory.GetItems();
		if (items is null)
			return item;

		foreach (Item? oitem in items) {
			if (oitem is not null && oitem.canStackWith(item)) {
				present = true;
				if (oitem.getRemainingStackSpace() > 0) {
					int remainder = item.Stack - quantity;
					item.Stack = quantity;
					item.Stack = oitem.addToStack(item) + remainder;
					quantity -= (initial - item.Stack);
					if (quantity <= 0 || item.Stack <= remainder)
						return item.Stack <= 0 ? null : item;
				}
			}
		}

		if (!present)
			return item;

		for (int idx = items.Count - 1; idx >= 0; idx--) {
			if (items[idx] == null) {
				if (quantity > item.maximumStackSize()) {
					Item obj = items[idx] = item.getOne();

					int removed = item.maximumStackSize();
					obj.Stack = removed;
					item.Stack -= removed;
					quantity -= removed;

				} else if (quantity < item.Stack) {
					Item obj = items[idx] = item.getOne();
					obj.Stack = quantity;
					item.Stack -= quantity;
					return item;

				} else {
					items[idx] = item;
					return null;
				}
			}
		}

		int capacity = inventory.GetActualCapacity();
		while (capacity > 0 && items.Count < capacity) {
			if (quantity > item.maximumStackSize()) {
				Item obj = item.getOne();
				int removed = item.maximumStackSize();
				obj.Stack = removed;
				item.Stack -= removed;
				quantity -= removed;
				items.Add(obj);

			} else if (quantity < item.Stack) {
				Item obj = item.getOne();
				obj.Stack = quantity;
				item.Stack -= quantity;
				items.Add(obj);
				return item;

			} else {
				items.Add(item);
				return null;
			}
		}

		return item;
	}

	#endregion

}

#endif

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Types;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TerrainFeatures;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Common;

public struct LocatedInventory {
	public object Source { get; }
	public GameLocation? Location { get; }

	public LocatedInventory(object source, GameLocation? location) {
		Source = source;
		Location = location;
	}

	public override bool Equals(object? obj) {
		return obj is LocatedInventory inventory &&
			   EqualityComparer<object>.Default.Equals(Source, inventory.Source) &&
			   EqualityComparer<GameLocation>.Default.Equals(Location, inventory.Location);
	}

	public override int GetHashCode() {
		return HashCode.Combine(Source, Location);
	}

	public static bool operator ==(LocatedInventory left, LocatedInventory right) {
		return left.Equals(right);
	}

	public static bool operator !=(LocatedInventory left, LocatedInventory right) {
		return !(left == right);
	}
}

public static class InventoryHelper {

	#region Item Creation

	public static readonly Regex ITEM_REGEX = new(@"^\(([^)]+)\)(.+)$");

	public static string GetItemQualifiedId(Item item) {
		if (item is Boots boots)
			return $"(B){boots.indexInTileSheet.Value}";

		if (item is Furniture furniture)
			return $"(F){furniture.ParentSheetIndex}";

		if (item is Hat hat)
			return $"(H){hat.which.Value}";

		if (item is Clothing clothing) {
			string type;
			switch (clothing.clothesType.Value) {
				case 0: // Shirt
					type = "S";
					break;
				case 1: // Pants
					type = "P";
					break;
				case 2: // Accessories
					type = "A";
					break;
				default:
					throw new ArgumentException($"unknown clothing type \"{clothing.clothesType.Value}\"", nameof(item));
			}

			return $"({type}){clothing.ParentSheetIndex}";
		}

		if (item is MeleeWeapon weapon)
			return $"(W){weapon.InitialParentTileIndex}";

		if (item is SObject sobj) {
			string type = "O";
			if (sobj.bigCraftable.Value)
				type = "BC";

			return $"({type}){sobj.ParentSheetIndex}";
		}

		throw new ArgumentException($"unknown item type", nameof(item));
	}

	[return: MaybeNull]
	public static Item CreateItemById(string id, int amount, int quality = 0, bool allow_null = false) {
		var match = ITEM_REGEX.Match(id);
		string type;
		int nid;

		if (match.Success) {
			type = match.Groups[1].Value;
			if (!int.TryParse(match.Groups[2].Value, out nid))
				throw new ArgumentException("invalid item id", nameof(id));

		} else {
			type = "O";
			if (!int.TryParse(id, out nid))
				throw new ArgumentException("invalid item id", nameof(id));
		}

		Item? result;

		switch(type) {
			case "BC":
				result = new SObject(Vector2.Zero, nid);
				break;
			case "B":
				result = new Boots(nid);
				break;
			case "F":
				result = new Furniture(nid, Vector2.Zero);
				break;
			case "H":
				result = new Hat(nid);
				break;
			case "O":
				result = new SObject(nid, 1);
				break;
			case "P":
			case "S":
				result = new Clothing(nid);
				break;
			case "W":
				result = new MeleeWeapon(nid);
				break;
			default:
				result = null;
				break;
		}

		if (result is SObject sobj)
			sobj.Quality = quality;

		if (result is Item item)
			item.Stack = amount;

		if (result is null && !allow_null)
			result = new SObject(0, 1);

		return result;
	}

	#endregion

	#region Item Iteration


	#endregion

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
		bool includeDiagonal = true
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
			includeDiagonal
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
		bool includeDiagonal = true
	) {
		List<AbsolutePosition> positions = new();

		for (int x = 0; x < source.Width; x++) {
			for (int y = 0; y < source.Height; y++) {
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
			includeDiagonal
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
		bool includeDiagonal = true
	) {
		List<AbsolutePosition> potentials = new() {
			source
		};

		Dictionary<AbsolutePosition, Vector2> origins = new();
		origins[source] = source.Position;

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
			includeDiagonal
		);
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
		bool includeDiagonal = true
	) {
		List<AbsolutePosition> positions = new();

		for (int x = 0; x < source.Width; x++) {
			for (int y = 0; y < source.Height; y++) {
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
		IEnumerable<AbsolutePosition>? extra = null
	) {
		List<AbsolutePosition> potentials = new();
		Dictionary<AbsolutePosition, Vector2> origins = new();

		if (extra != null) {
			foreach (var entry in extra) {
				potentials.Add(entry);
				origins[entry] = entry.Position;
			}
		}

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
			includeDiagonal
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
		bool includeDiagonal = true
	) {
		List<AbsolutePosition> potentials = new(sources);
		Dictionary<AbsolutePosition, Vector2> origins = new();

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
			includeDiagonal
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
		for(int x = -1; x < 2; x++) {
			for(int y = -1; y < 2; y++) {
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
		bool includeDiagonal
	) {
		List<LocatedInventory> result = new();

		int i = start;

		while(i < potentials.Count && i < scanLimit) {
			AbsolutePosition abs = potentials[i++];

			SObject? obj;
			SObject? furn;
			TerrainFeature? feature;
			if (abs.Location != null) {
				TileHelper.GetObjectAtPosition(abs.Location, abs.Position, out obj);
				abs.Location.terrainFeatures.TryGetValue(abs.Position, out feature);
				furn = abs.Location.GetFurnitureAt(abs.Position);
			} else {
				feature = null;
				obj = null;
				furn = null;
			}

			bool want_neighbors = false;
			IInventoryProvider? provider;

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

	public static bool DoesLocationContain(GameLocation? location, object? obj) {
		if (location == null)
			return false;

		if (location is FarmHouse farmHouse && farmHouse.fridge.Value == obj)
			return true;
		if (location is IslandFarmHouse islandHouse && islandHouse.fridge.Value == obj)
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
		List<LocatedInventory> result = new();

		foreach (object obj in inventories) {
			IInventoryProvider? provider = getProvider(obj);
			if (provider == null)
				continue;

			GameLocation? loc = null;

			if (first != null && DoesLocationContain(first, obj))
				loc = first;
			else {
				foreach(GameLocation location in locations) {
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

	public static List<IInventory> GetUnsafeInventories(
		IEnumerable<object> inventories,
		Func<object, IInventoryProvider?> getProvider,
		GameLocation? location,
		Farmer? who,
		bool nullLocationValid = false
	) {
		List<LocatedInventory> located = new();
		foreach (object obj in inventories) {
			if (obj is LocatedInventory inv)
				located.Add(inv);
			else
				located.Add(new(obj, location));
		}

		return GetUnsafeInventories(located, getProvider, who, nullLocationValid);
	}

	public static List<IInventory> GetUnsafeInventories(
		IEnumerable<LocatedInventory> inventories,
		Func<object, IInventoryProvider?> getProvider,
		Farmer? who,
		bool nullLocationValid = false
	) {
		List<IInventory> result = new();

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

	public static void WithInventories(
		IEnumerable<LocatedInventory>? inventories,
		Func<object, IInventoryProvider?> getProvider,
		Farmer? who,
		Action<IList<IInventory>> withLocks,
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
		Action<IList<IInventory>> withLocks,
		bool nullLocationValid = false
	) {
		List<LocatedInventory>? located;
		if (inventories == null)
			located = null;
		else {
			located = new();
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
		Action<IList<IInventory>, Action> withLocks,
		bool nullLocationValid = false
	) {
		List<IInventory> locked = new();
		List<IInventory> lockable = new();

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
				if (mlocked)
					locked.Add(entry);
				else
					lockable.Add(entry);
			}

		if (lockable.Count == 0) {
			withLocks(locked, () => { });
			return;
		}

		List<NetMutex> mutexes = lockable.Where(entry => entry.Mutex != null).Select(entry => entry.Mutex!).ToList();
		MultipleMutexRequest? mmr = null;
		mmr = new MultipleMutexRequest(
			mutexes,
			() => {
				locked.AddRange(lockable);
				withLocks(locked, () => {
					mmr?.ReleaseLocks();
					mmr = null;
				});
			},
			() => {
				withLocks(locked, () => { });
			});
	}

	#endregion

	#region Recipes and Crafting

	/// <summary>
	/// Check if an item matches the provided ID, using the same logic
	/// as the crafting system.
	/// </summary>
	/// <param name="id">the ID to check</param>
	/// <param name="item">the Item to check</param>
	/// <returns></returns>
	public static bool DoesItemMatchID(int id, Item? item) {
		if (item is SObject sobj) {
			return
				!sobj.bigCraftable.Value && sobj.ParentSheetIndex == id
				|| item.Category == id
				|| CraftingRecipe.isThereSpecialIngredientRule(sobj, id);
		}

		return false;
	}

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
	/// <returns>The number of items remaining to consume.</returns>
	public static int ConsumeItem(Func<Item, bool> matcher, int amount, IList<Item?> items, out bool nullified, out bool passed_quality, int max_quality = int.MaxValue) {
		nullified = false;
		passed_quality = false;

		for (int idx = items.Count - 1; idx >= 0; --idx) {
			Item? item = items[idx];
			if (item == null)
				continue;

			int quality = item is SObject obj ? obj.Quality : 0;
			if (quality > max_quality) {
				passed_quality = true;
				continue;
			}

			if (matcher(item)) {
				int count = Math.Min(amount, item.Stack);
				amount -= count;

				if (item.Stack <= count) {
					items[idx] = null;
					nullified = true;

				} else
					item.Stack -= count;

				if (amount <= 0)
					return amount;
			}
		}

		return amount;
	}

	/// <summary>
	/// Consume matching items from a player, and also from a set of
	/// <see cref="IInventory"/> instances.
	/// </summary>
	/// <param name="items">An enumeration of <see cref="KeyValuePair{int,int}"/>
	/// instances where the first integer is the item ID to match and the
	/// second integer is the quantity to consume.
	/// <param name="who">The player to consume items from, if any.</param>
	/// <param name="inventories">An enumeration of <see cref="IInventory"/>
	/// instances to consume items from.</param>
	/// <param name="max_quality">The maximum quality of item to consume.</param>
	/// <param name="low_quality_first">Whether or not to consume low quality
	/// items first.</param>
	public static void ConsumeItems(IEnumerable<KeyValuePair<int, int>> items, Farmer? who, IEnumerable<IInventory>? inventories, int max_quality = int.MaxValue, bool low_quality_first = false) {
		if (items is null)
			return;

		ConsumeItems(
			items.Select<KeyValuePair<int, int>, (Func<Item, bool>, int)>(x => (item => DoesItemMatchID(x.Key, item), x.Value)),
			who,
			inventories,
			max_quality,
			low_quality_first
		);
	}

	/// <summary>
	/// Consume matching items from a player, and also from a set of
	/// <see cref="IInventory"/> instances.
	/// </summary>
	/// <param name="items">An enumeration of tuples where the function
	/// matches items, and the integer is the quantity to consume.</param>
	/// <param name="who">The player to consume items from, if any.</param>
	/// <param name="inventories">An enumeration of <see cref="IInventory"/>
	/// instances to consume items from.</param>
	/// <param name="max_quality">The maximum quality of item to consume.</param>
	/// <param name="low_quality_first">Whether or not to consume low quality
	/// items first.</param>
	public static void ConsumeItems(IEnumerable<(Func<Item, bool>, int)> items, Farmer? who, IEnumerable<IInventory>? inventories, int max_quality = int.MaxValue, bool low_quality_first = false) {
		IList<IInventory>? working = (inventories as IList<IInventory>) ?? inventories?.ToList();
		bool[]? modified = working == null ? null : new bool[working.Count];
		IList<Item?>?[] invs = working?.Select(val => val.CanExtractItems() ? val.GetItems() : null).ToArray() ?? Array.Empty<IList<Item?>?>();

		foreach ((Func<Item, bool>, int) pair in items) {
			Func<Item, bool> matcher = pair.Item1;
			int remaining = pair.Item2;

			int mq = max_quality;
			if (low_quality_first)
				mq = 0;

			for (int q = mq; q <= max_quality; q++) {
				bool passed;
				if (who != null)
					remaining = ConsumeItem(matcher, remaining, who.Items, out bool m, out passed, q);
				else
					passed = false;

				if (remaining <= 0)
					break;

				if (working != null)
					for (int iidx = 0; iidx < working.Count; iidx++) {
						IList<Item?>? inv = invs[iidx];
						if (inv == null || inv.Count == 0)
							continue;

						remaining = ConsumeItem(matcher, remaining, inv, out bool modded, out bool p, q);
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

		if (working != null)
			for (int idx = 0; idx < modified!.Length; idx++) {
				if (modified[idx])
					working[idx].CleanInventory();
			}
	}

	#endregion

	#region Transfer Items

	public static bool AddToInventories(IList<Item?> items, IEnumerable<IInventory> inventories, TransferBehavior behavior, Action<Item, int>? onTransfer = null) {
		if (behavior.Mode == TransferMode.None)
			return false;

		int[] transfered = new int[items.Count];

		foreach (IInventory inv in inventories) {
			if (!inv.CanInsertItems())
				continue;

			if (FillInventory(items, inv, behavior, transfered, onTransfer))
				return true;
		}

		return false;
	}

	private static bool FillInventory(IList<Item?> items, IInventory inventory, TransferBehavior behavior, int[] transfered, Action<Item, int>? onTransfer = null) {
		bool empty = true;

		for(int idx = 0; idx < items.Count; idx++) {
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

				if (count != final && ! had_transfered)
					onTransfer?.Invoke(item, idx);
			}
		}

		return empty;
	}

	public static Item? AddItemToInventory(Item item, int quantity, IInventory inventory) {
		int initial = item.Stack;
		if (quantity > initial)
			quantity = initial;

		if (quantity <= 0)
			return item.Stack <= 0 ? null : item;

		bool present = false;

		IList<Item?>? items = inventory.GetItems();
		if (items is null)
			return item;

		foreach(Item? oitem in items) {
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

		for(int idx = items.Count - 1; idx >= 0; idx--) {
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

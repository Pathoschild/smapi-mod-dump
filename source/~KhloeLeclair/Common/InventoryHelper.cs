/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common.Inventory;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TerrainFeatures;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Common {

	public struct LocatedInventory {
		public object Source { get; }
		public GameLocation Location { get; }

		public LocatedInventory(object source, GameLocation location) {
			Source = source;
			Location = location;
		}

		public override bool Equals(object obj) {
			return obj is LocatedInventory inventory &&
				   EqualityComparer<object>.Default.Equals(Source, inventory.Source) &&
				   EqualityComparer<GameLocation>.Default.Equals(Location, inventory.Location);
		}

		public override int GetHashCode() {
			return HashCode.Combine(Source, Location);
		}
	}

	public static class InventoryHelper {

		#region Discovery

		public static List<LocatedInventory> DiscoverInventories(
			Vector2 source,
			GameLocation location,
			Farmer who,
			Func<object, IInventoryProvider> getProvider,
			Func<object, bool> checkConnector,
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
			GameLocation location,
			Farmer who,
			Func<object, IInventoryProvider> getProvider,
			Func<object, bool> checkConnector,
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
			Farmer who,
			Func<object, IInventoryProvider> getProvider,
			Func<object, bool> checkConnector,
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
			GameLocation location,
			IEnumerable<LocatedInventory> sources,
			Farmer who,
			Func<object, IInventoryProvider> getProvider,
			Func<object, bool> checkConnector,
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
			IEnumerable<LocatedInventory> sources,
			Farmer who,
			Func<object, IInventoryProvider> getProvider,
			Func<object, bool> checkConnector,
			int distanceLimit = 5,
			int scanLimit = 100,
			int targetLimit = 20,
			bool includeSource = true,
			bool includeDiagonal = true,
			IEnumerable<AbsolutePosition> extra = null
		) {
			List<AbsolutePosition> potentials = new();
			Dictionary<AbsolutePosition, Vector2> origins = new();

			if (extra != null) {
				foreach (var entry in extra) {
					potentials.Add(entry);
					origins[entry] = entry.Position;
				}
			}

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
			Farmer who,
			Func<object, IInventoryProvider> getProvider,
			Func<object, bool> checkConnector,
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
			GameLocation location,
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
			Farmer who,
			Func<object, IInventoryProvider> getProvider,
			Func<object, bool> checkConnector,
			int distanceLimit,
			int scanLimit,
			int targetLimit,
			bool includeDiagonal
		) {
			List<LocatedInventory> result = new();

			int i = start;
			int limit = 100;

			while(i < potentials.Count && i < limit) {
				AbsolutePosition abs = potentials[i++];

				SObject obj;
				SObject furn;
				TerrainFeature feature;
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
				IInventoryProvider provider;

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

		public static bool DoesLocationContain(GameLocation location, object obj) {
			if (location is FarmHouse farmHouse && farmHouse.fridge.Value == obj)
				return true;
			if (location is IslandFarmHouse islandHouse && islandHouse.fridge.Value == obj)
				return true;
			return location != null && location.Objects.Values.Contains(obj);
		}

		public static List<LocatedInventory> LocateInventories(
			IEnumerable<object> inventories,
			IEnumerable<GameLocation> locations,
			Func<object, IInventoryProvider> getProvider,
			GameLocation first,
			bool nullLocationValid = false
		) {
			List<LocatedInventory> result = new();

			foreach (object obj in inventories) {
				IInventoryProvider provider = getProvider(obj);
				if (provider == null)
					continue;

				GameLocation loc = null;

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
			Func<object, IInventoryProvider> getProvider,
			GameLocation location,
			Farmer who,
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
			Func<object, IInventoryProvider> getProvider,
			Farmer who,
			bool nullLocationValid = false
		) {
			List<IInventory> result = new();

			foreach (LocatedInventory loc in inventories) {
				if (loc.Location == null && !nullLocationValid)
					continue;

				IInventoryProvider provider = getProvider(loc.Source);
				if (provider == null || !provider.IsValid(loc.Source, loc.Location, who))
					continue;

				// If we can't get a mutex, we can't assure safety. Abort.
				NetMutex mutex = provider.GetMutex(loc.Source, loc.Location, who);
				if (mutex == null)
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
			IEnumerable<LocatedInventory> inventories,
			Func<object, IInventoryProvider> getProvider,
			Farmer who,
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
			IEnumerable<object> inventories,
			Func<object, IInventoryProvider> getProvider,
			GameLocation location,
			Farmer who,
			Action<IList<IInventory>> withLocks,
			bool nullLocationValid = false
		) {
			List<LocatedInventory> located = new();
			foreach (object obj in inventories) {
				if (obj is LocatedInventory inv)
					located.Add(inv);
				else
					located.Add(new(obj, location));
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
			IEnumerable<LocatedInventory> inventories,
			Func<object, IInventoryProvider> getProvider,
			Farmer who,
			Action<IList<IInventory>, Action> withLocks,
			bool nullLocationValid = false
		) {
			List<IInventory> locked = new();
			List<IInventory> lockable = new();

			if (inventories != null)
				foreach (LocatedInventory loc in inventories) {
					if (loc.Location == null && !nullLocationValid)
						continue;

					IInventoryProvider provider = getProvider(loc.Source);
					if (provider == null || !provider.IsValid(loc.Source, loc.Location, who))
						continue;

					// If we can't get a mutex, we can't assure safety. Abort.
					NetMutex mutex = provider.GetMutex(loc.Source, loc.Location, who);
					if (mutex == null)
						continue;

					// Check the current state of the mutex. If someone else has
					// it locked, then we can't ensure safety. Abort.
					bool mlocked = mutex.IsLocked();
					if (mlocked && !mutex.IsLockHeld())
						continue;

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

			List<NetMutex> mutexes = lockable.Select(entry => entry.Mutex).ToList();
			MultipleMutexRequest mmr = null;
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

		public static bool DoesPlayerHaveItems(IEnumerable<KeyValuePair<int, int>> items, Farmer who, IList<Item> extra = null) {
			foreach (var pair in items) {
				int num = pair.Value - who.getItemCount(pair.Key, 5);
				if (num > 0 && (extra == null || num - who.getItemCountInList(extra, pair.Key, 5) > 0))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Check if an item matches the provided ID, using the same logic
		/// as the crafting system.
		/// </summary>
		/// <param name="id">the ID to check</param>
		/// <param name="item">the Item to check</param>
		/// <returns></returns>
		public static bool DoesItemMatchID(int id, Item item) {
			if (item is SObject sobj) {
				return
					!sobj.bigCraftable.Value && sobj.ParentSheetIndex == id
					|| item.Category == id
					|| CraftingRecipe.isThereSpecialIngredientRule(sobj, id);
			}

			return false;
		}

		public static void ConsumeIngredients(this CraftingRecipe recipe, Farmer who, IEnumerable<IInventory> inventories) {
			ConsumeItems(recipe.recipeList, who, inventories);
		}

		/// <summary>
		/// Remove a quantity of an item from an inventory.
		/// </summary>
		/// <param name="id">The ID of the item to consume</param>
		/// <param name="amount">The quantity to remove</param>
		/// <param name="items">The inventory we're searching</param>
		/// <param name="nullified">whether or not one or more of the items in the inventory was replaced with null</param>
		/// <returns>the remaining quantity to remove</returns>
		public static int ConsumeItem(int id, int amount, IList<Item> items, out bool nullified, out bool passed_quality, int max_quality = int.MaxValue) {
			nullified = false;
			passed_quality = false;

			for (int idx = items.Count - 1; idx >= 0; --idx) {
				Item item = items[idx];
				if (item == null)
					continue;

				int quality = item is SObject obj ? obj.Quality : 0;
				if (quality > max_quality) {
					passed_quality = true;
					continue;
				}

				if (DoesItemMatchID(id, item)) {
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

		public static int ConsumeItem(Func<Item, bool> matcher, int amount, IList<Item> items, out bool nullified, out bool passed_quality, int max_quality = int.MaxValue) {
			nullified = false;
			passed_quality = false;

			for (int idx = items.Count - 1; idx >= 0; --idx) {
				Item item = items[idx];
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
		/// 
		/// </summary>
		/// <param name="items"></param>
		/// <param name="who"></param>
		/// <param name="inventories"></param>
		/// <param name="max_quality"></param>
		/// <param name="low_quality_first"></param>
		public static void ConsumeItems(IEnumerable<KeyValuePair<int, int>> items, Farmer who, IEnumerable<IInventory> inventories, int max_quality = int.MaxValue, bool low_quality_first = false) {
			IList<IInventory> working = (inventories as IList<IInventory>) ?? inventories?.ToList();
			bool[] modified = working == null ? null : new bool[working.Count];
			IList<Item>[] invs = working?.Select(val => val.CanExtractItems() ? val.GetItems() : null).ToArray();

			foreach (KeyValuePair<int, int> pair in items) {
				int id = pair.Key;
				int remaining = pair.Value;

				int mq = max_quality;
				if (low_quality_first)
					mq = 0;

				for (int q = mq; q <= max_quality; q++) {
					remaining = ConsumeItem(id, remaining, who.Items, out bool m, out bool passed, q);
					if (remaining <= 0)
						break;

					if (working != null)
						for(int iidx = 0; iidx < working.Count; iidx++) {
							IList<Item> inv = invs[iidx];
							if (inv == null || inv.Count == 0)
								continue;

							remaining = ConsumeItem(id, remaining, inv, out bool modded, out bool p, q);
							if (modded)
								modified[iidx] = true;

							if (p)
								passed = true;

							if (remaining <= 0)
								break;
						}

					if (remaining <= 0 || ! passed)
						break;
				}
			}

			if (working != null)
				for (int idx = 0; idx < modified.Length; idx++) {
					if (modified[idx])
						working[idx].CleanInventory();
				}
		}

		public static void ConsumeItems(IEnumerable<Tuple<Func<Item, bool>, int>> items, Farmer who, IEnumerable<IInventory> inventories, int max_quality = int.MaxValue, bool low_quality_first = false) {
			IList<IInventory> working = (inventories as IList<IInventory>) ?? inventories?.ToList();
			bool[] modified = working == null ? null : new bool[working.Count];
			IList<Item>[] invs = working?.Select(val => val.CanExtractItems() ? val.GetItems() : null).ToArray();

			foreach (Tuple<Func<Item, bool>, int> pair in items) {
				Func<Item, bool> matcher = pair.Item1;
				int remaining = pair.Item2;

				int mq = max_quality;
				if (low_quality_first)
					mq = 0;

				for (int q = mq; q <= max_quality; q++) {
					remaining = ConsumeItem(matcher, remaining, who.Items, out bool m, out bool passed, q);
					if (remaining <= 0)
						break;

					if (working != null)
						for (int iidx = 0; iidx < working.Count; iidx++) {
							IList<Item> inv = invs[iidx];
							if (inv == null || inv.Count == 0)
								continue;

							remaining = ConsumeItem(matcher, remaining, inv, out bool modded, out bool p, q);
							if (modded)
								modified[iidx] = true;

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
				for (int idx = 0; idx < modified.Length; idx++) {
					if (modified[idx])
						working[idx].CleanInventory();
				}
		}

		#endregion

	}
}

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

using Leclair.Stardew.Common.Events;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.GameData;
using StardewValley.Internal;

namespace Leclair.Stardew.BetterCrafting.Managers;

public class ItemCacheManager : BaseManager {

	private static readonly string FLOORPAPER = @"Data/AdditionalWallpaperFlooring";

	private static readonly Dictionary<string, string> TYPE_MAPS = new() {
		{ ItemRegistry.type_bigCraftable, @"Data/BigCraftables" },
		{ ItemRegistry.type_boots, @"Data/Boots" },
		{ ItemRegistry.type_floorpaper, FLOORPAPER },
		{ ItemRegistry.type_furniture, @"Data/Furniture" },
		{ ItemRegistry.type_hat, @"Data/hats" },
		{ ItemRegistry.type_mannequin, @"Data/Mannequins" },
		{ ItemRegistry.type_object, @"Data/Objects" },
		{ ItemRegistry.type_pants, @"Data/Pants" },
		{ ItemRegistry.type_shirt, @"Data/Shirts" },
		{ ItemRegistry.type_tool, @"Data/Tools" },
		{ ItemRegistry.type_trinket, @"Data/Trinkets" },
		{ ItemRegistry.type_wallpaper, FLOORPAPER },
		{ ItemRegistry.type_weapon, @"Data/Weapons" }
	};

	private static readonly Dictionary<string, string> REVERSE_TYPE_MAPS = TYPE_MAPS
		.Where(pair => pair.Value != FLOORPAPER)
		.ToDictionary(pair => pair.Value, pair => pair.Key);

	private readonly Dictionary<string, List<Item>?> ItemMaps = new();

	private long LastCachedQuery;
	private readonly PerScreen<Dictionary<long, Item[]>> CachedQueries = new(() => new());
	private readonly PerScreen<GameLocation?> LastLocation = new(() => null);

	public ItemCacheManager(ModEntry mod) : base(mod) { }

	#region Queries

	public long GetNextCachedQueryId() {
		return LastCachedQuery++;
	}

	public Item[] GetItems(long id, ISpawnItemData data) {
		var entries = CachedQueries.Value;

		if (entries.TryGetValue(id, out var items))
			return items;

		items = ItemQueryResolver.TryResolve(
			data,
			new ItemQueryContext(Game1.player.currentLocation, Game1.player, Game1.random),
			avoidRepeat: false,
			logError: (query, error) => {
				Mod.Log($"Error attempting to resolve ingredient with query '{query}': {error}", LogLevel.Error);
			}
		).Where(x => x.Item is Item).Select(x => (Item) x.Item).ToArray();

		entries[id] = items;
		return items;
	}

	#endregion

	#region Events

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach(var name in e.Names) {
			// This path covers two objects.
			if (name.BaseName == FLOORPAPER) {
				Log($"Clearing floors and wallpapers cache.", StardewModdingAPI.LogLevel.Trace);
				ItemMaps.Remove(ItemRegistry.type_floorpaper);
				ItemMaps.Remove(ItemRegistry.type_wallpaper);
				CachedQueries.ResetAllScreens();

				// And the rest...
			} else if (REVERSE_TYPE_MAPS.TryGetValue(name.BaseName, out string? typekey)) {
				Log($"Clearing {typekey} cache.", StardewModdingAPI.LogLevel.Trace);
				ItemMaps.Remove(typekey);
				CachedQueries.ResetAllScreens();
			}
		}
	}

	[Subscriber]
	private void OnNewDay(object? sender, DayStartedEventArgs e) {
		CachedQueries.ResetAllScreens();
	}

	[Subscriber]
	private void OnUpdateTicking(object? sender, WarpedEventArgs e) {
		CachedQueries.Value.Clear();
	}

	#endregion

	private void LoadItems() {
		foreach(string type in TYPE_MAPS.Keys) {
			if (!ItemMaps.ContainsKey(type)) {
				var typedef = ItemRegistry.GetTypeDefinition(type);
				if (typedef is not null) {
					List<Item> result = new();

					foreach (string id in typedef.GetAllIds()) {
						Item? item = ItemRegistry.Create(id, allowNull: true);
						if (item is not null)
							result.Add(item);
					}

					ItemMaps[type] = result;
				} else
					ItemMaps[type] = null;
			}
		}
	}

	private IEnumerable<Item> GetAllUnknownItems() {
		foreach(var typedef in ItemRegistry.ItemTypes) {
			if (!TYPE_MAPS.ContainsKey(typedef.Identifier)) {
				Log($"Unexpected item type: {typedef.Identifier}", StardewModdingAPI.LogLevel.Trace);

				foreach (string id in typedef.GetAllIds()) {
					Item? item = ItemRegistry.Create(id, allowNull: true);
					if (item is not null)
						yield return item;
				}
			}
		}
	}

	public void Invalidate() {
		ItemMaps.Clear();
	}

	public IEnumerable<Item> GetMatchingItems(Func<Item, bool> predicate) {
		// First, make sure we've loaded everything.
		LoadItems();

		foreach(var items in ItemMaps.Values) {
			if (items is not null)
				foreach(var item in items)
					if (predicate(item))
						yield return item;
		}

		foreach(var item in GetAllUnknownItems()) {
			if (predicate(item))
				yield return item;
		}

	}

}

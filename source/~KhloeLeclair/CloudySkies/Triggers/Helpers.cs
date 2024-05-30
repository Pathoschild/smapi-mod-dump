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

using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.CloudySkies;

public static partial class Triggers {

	private static ModEntry Instance => ModEntry.Instance;

	internal static IEnumerable<Vector2> FilterEnumeratedTiles(GameLocation location, List<TargetTileFilter> filters, IEnumerable<Vector2> tiles) {
		if (filters is null || filters.Count == 0)
			return tiles;

		return _FilterEnumeratedTilesImpl(location, filters, tiles);
	}

	private static IEnumerable<Vector2> _FilterEnumeratedTilesImpl(GameLocation location, List<TargetTileFilter> filters, IEnumerable<Vector2> tiles) {
		foreach (var pos in tiles) {
			foreach (var filter in filters) {
				// TODO: Implement more kinds of filter.
				string? prop = location.doesTileHaveProperty((int) pos.X, (int) pos.Y, filter.Property, "Back");
				if (prop is null || (filter.Value != null && prop != filter.Value))
					continue;

				yield return pos;
			}
		}
	}


	internal static bool CheckFertilizer(HoeDirt dirt, string? fertilizerQuery) {
		if (string.IsNullOrEmpty(fertilizerQuery))
			return true;

		string?[] items;
		if (!string.IsNullOrEmpty(dirt.fertilizer.Value) && Instance.intUF is not null && Instance.intUF.IsLoaded)
			items = dirt.fertilizer.Value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		else
			items = [dirt.fertilizer.Value];

		foreach (string? item in items) {
			var input = GetOrCreateInstance(item);
			if (GameStateQuery.CheckConditions(fertilizerQuery, dirt.Location, null, inputItem: input))
				return true;
		}

		return false;
	}


	/// <summary>
	/// Enumerate all the tiles within a given map.
	/// </summary>
	/// <param name="location"></param>
	/// <param name="minX"></param>
	/// <param name="minY"></param>
	/// <param name="maxX"></param>
	/// <param name="maxY"></param>
	/// <returns></returns>
	internal static IEnumerable<Vector2> EnumerateAllTiles(GameLocation location, int minX = 0, int minY = 0, int maxX = int.MaxValue, int maxY = int.MaxValue) {
		int width = location.map.DisplayWidth / 64;
		int height = location.map.DisplayHeight / 64;

		if (maxX >= width)
			maxX = width - 1;
		if (maxY >= height)
			maxY = height - 1;

		if (minX < 0)
			minX = 0;
		if (minY < 0)
			minY = 0;

		for (int x = minX; x <= maxX; x++) {
			for (int y = minY; y <= maxY; y++) {
				yield return new Vector2(x, y);
			}
		}
	}

	internal static readonly Dictionary<string, Item?> CachedItems = new();

	/// <summary>
	/// Get or create an instance of an item, if the provided itemId
	/// is valid. This caches to an internal dictionary that isn't
	/// cleared immediately to hopefully improve performance when
	/// doing bulk processing.
	/// </summary>
	/// <param name="itemId">The item Id to create an item for.</param>
	/// <returns>The Item instance, if one could be created, or <c>null</c></returns>
	internal static Item? GetOrCreateInstance(string? itemId) {
		if (string.IsNullOrEmpty(itemId))
			return null;

		if (CachedItems.TryGetValue(itemId, out Item? item))
			return item;

		item = ItemRegistry.Create(itemId, allowNull: true);
		CachedItems[itemId] = item;
		return item;
	}

	/// <summary>
	/// Enumerate all terrain features of type <typeparamref name="TFeature"/>
	/// in the provided <paramref name="location"/>. If <paramref name="position"/>
	/// is provided, only return features within <paramref name="radius"/>
	/// tiles of the position.
	/// </summary>
	/// <typeparam name="TFeature">The type of terrain feature to return</typeparam>
	/// <param name="location">The location to search</param>
	/// <param name="position">The position to search</param>
	/// <param name="radius">The radius to search around position.</param>
	/// <param name="includeLarge">If this is set to true, also check <see cref="GameLocation.largeTerrainFeatures"/></param>
	/// <returns></returns>
	internal static IEnumerable<TFeature> EnumerateTerrainFeatures<TFeature>(GameLocation location, Vector2? position = null, int radius = 0, bool includeLarge = false) where TFeature : TerrainFeature {
		int radSquared = radius * radius;
		foreach (var feature in location.terrainFeatures.Values) {
			if (feature is TFeature val && (!position.HasValue || Vector2.DistanceSquared(position.Value, feature.Tile) <= radSquared))
				yield return val;
		}

		if (includeLarge && location.largeTerrainFeatures is not null)
			foreach (var feature in location.largeTerrainFeatures) {
				if (feature is TFeature val && (!position.HasValue || Vector2.DistanceSquared(position.Value, val.getBoundingBox().GetNearestPoint(position.Value)) <= radSquared))
					yield return val;
			}
	}

	internal static IEnumerable<(HoeDirt, IndoorPot?)> EnumerateHoeDirtAndPots(GameLocation location, Vector2? position = null, int radius = 0) {
		int radSquared = radius * radius;
		foreach (var feature in location.terrainFeatures.Values)
			if (feature is HoeDirt dirt && (!position.HasValue || Vector2.DistanceSquared(position.Value, dirt.Tile) <= radSquared))
				yield return (dirt, null);

		foreach (var sobj in location.Objects.Values)
			if (sobj is IndoorPot pot && pot.hoeDirt.Value != null && (!position.HasValue || Vector2.DistanceSquared(position.Value, pot.TileLocation) <= radSquared))
				yield return (pot.hoeDirt.Value, pot);
	}

}

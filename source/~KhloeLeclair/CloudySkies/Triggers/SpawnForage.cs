/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;


namespace Leclair.Stardew.CloudySkies;

public static partial class Triggers {

	[TriggerAction]
	private static bool SpawnForage(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		List<TargetTileFilter> filters = [];
		float chance = 1f;
		int max = int.MaxValue;
		bool includeIndoors = false;
		bool includeDefault = false;
		bool ignoreSpawnable = false;
		bool destroyOvernight = false;

		List<SpawnForageData> spawns = [];

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<TargetTileFilter>("--filter", filters.Add)
				.WithDescription("Add a filter for selecting valid tiles.")
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of forage to spawn.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<float>("-c", "--chance", val => chance = val)
				.WithDescription("The percent chance that any given tile spawn forage will be changed. Default: 1.0")
				.WithValidation<float>(val => val >= 0 && val <= 1, "must be value in range 0.0 to 1.0")
			.AddFlag("--include-default", () => includeDefault = true)
				.WithDescription("If this flag is set, include the location's default forage items in the list of potential spawns.")
			.AddFlag("--indoors", () => includeIndoors = true)
				.WithDescription("If this flag is set, indoor locations will not be skipped.")
			.AddFlag("--ignore-spawnable", () => ignoreSpawnable = true)
				.WithDescription("If this flag is set, we will ignore the Spawnable flag of tiles and allow spawning anywhere.")
			.AddFlag("--destroy-overnight", () => destroyOvernight = true)
				.WithDescription("If this flag is set, the spawned forage will be removed at the end of the day.")
			.Add<string>("-i", "--item", val => {
				var added = new SpawnForageData() {
					ItemId = val
				};
				spawns.Add(added);
			})
				.WithDescription("Add a new item to the list of forage to spawn. Supports item queries.")
				.AllowMultiple()
			.Add<string>("-iq", "--item-query", val => spawns.Last().PerItemCondition = val)
				.WithDescription("Adds a per-item condition to the previously added item.")
				.AllowMultiple()
			.Add<int>("-q", "--item-quality", val => spawns.Last().Quality = val)
				.WithDescription("Adds a quality to the previously added item.")
				.AllowMultiple();

		if (!parser.TryParse(args[1..], out error))
			return false;

		if (parser.WantsHelp) {
			Instance.Log($"Usage: {args[0]} {parser.Usage}", LogLevel.Info);
			return true;
		}

		Dictionary<GameLocation, List<SpawnForageData>>? availableSpawns = includeDefault ? [] : null;

		List<SpawnForageData> GetAvailableSpawns(GameLocation location) {
			if (!includeDefault)
				return spawns;

			if (availableSpawns!.TryGetValue(location, out var result))
				return result;

			result = new(spawns);

			var data = location.GetData();
			if (data != null) {
				Season season = location.GetSeason();
				foreach (var spawn in GameLocation.GetData("Default").Forage.Concat(data.Forage)) {
					if (spawn.Season.HasValue && spawn.Season != season)
						continue;
					if (spawn.Condition is null || GameStateQuery.CheckConditions(spawn.Condition, location))
						result.Add(spawn);
				}
			}

			availableSpawns[location] = result;
			return result;
		}

		if (includeDefault || spawns.Count > 0)
			foreach (var entry in targets.SelectMany(x => x)) {
				var loc = entry.Location;
				if (loc is null || (!includeIndoors && !loc.IsOutdoors))
					continue;

				var forage = GetAvailableSpawns(loc);
				if (forage is null || forage.Count == 0)
					continue;

				IEnumerable<Vector2> tiles;
				if (entry.Position.HasValue)
					tiles = entry.Position.Value.IterArea(entry.Radius, false);
				else
					tiles = EnumerateAllTiles(loc);

				ItemQueryContext ctx = new(loc, Game1.player, Game1.random);
				Dictionary<SpawnForageData, IList<ItemQueryResult>> cachedQueryResults = [];

				foreach (var pos in FilterEnumeratedTiles(loc, filters, tiles)) {
					int x = (int) pos.X;
					int y = (int) pos.Y;

					if (!(chance >= 1f || Game1.random.NextSingle() <= chance))
						continue;

					if (loc.Objects.ContainsKey(pos) ||
						loc.IsNoSpawnTile(pos) ||
						(!ignoreSpawnable && loc.doesTileHaveProperty(x, y, "Spawnable", "Back") == null) ||
						loc.doesEitherTileOrTileIndexPropertyEqual(x, y, "Spawnable", "Back", "F") ||
						!loc.CanItemBePlacedHere(pos) ||
						loc.getTileIndexAt(x, y, "AlwaysFront") != -1 ||
						loc.getTileIndexAt(x, y, "AlwaysFront2") != -1 ||
						loc.getTileIndexAt(x, y, "AlwaysFront3") != -1 ||
						loc.getTileIndexAt(x, y, "Front") != -1 ||
						loc.isBehindBush(pos)
					)
						continue;

					// TODO: Maybe determine some way of respecting an entry's
					// chance to spawn while also ensuring something spawns?

					var toSpawn = Game1.random.ChooseFrom(forage);

					if (!cachedQueryResults.TryGetValue(toSpawn, out var result)) {
						result = ItemQueryResolver.TryResolve(toSpawn, ctx, ItemQuerySearchMode.AllOfTypeItem, avoidRepeat: false, logError: (query, error) => {
							Instance.Log($"Failed parsing item query '{query}' for forage: {error}", LogLevel.Error);
						});
						cachedQueryResults[toSpawn] = result;
					}

					var itemToSpawn = Game1.random.ChooseFrom(result);
					if (itemToSpawn.Item is not SObject sobj || sobj.getOne() is not SObject copy)
						continue;

					if (destroyOvernight)
						copy.destroyOvernight = true;

					copy.IsSpawnedObject = true;
					if (loc.dropObject(copy, pos * 64f, Game1.viewport, initialPlacement: true)) {
						max--;
						if (max <= 0)
							break;
					}
				}

				if (max <= 0)
					break;
			}

		// Great success!
		error = null;
		return true;
	}

}

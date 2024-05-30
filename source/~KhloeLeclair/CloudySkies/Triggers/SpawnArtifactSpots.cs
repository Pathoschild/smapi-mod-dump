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

using Newtonsoft.Json;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.Tools;


namespace Leclair.Stardew.CloudySkies;

public static partial class Triggers {

	public const string SAS_ITEM = "leclair.cloudyskies/ItemQuery";

	[TriggerAction]
	private static bool SpawnArtifactSpots(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		List<TargetTileFilter> filters = [];
		float chance = 1f;
		int max = int.MaxValue;
		bool includeIndoors = false;
		bool destroyOvernight = false;

		int maxPerLocation = -1;

		List<ArtifactSpotDropData> spawnable = [];

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<TargetTileFilter>("--filter", filters.Add)
				.WithDescription("Add a filter for selecting valid tiles.")
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of artifact spots to spawn.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<int>("--max-spots", val => maxPerLocation = val)
				.WithDescription("The maximum number of artifact spots any given location should have.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<float>("-c", "--chance", val => chance = val)
				.WithDescription("The percent chance that any given tile will spawn an artifact spot. Default: 1.0")
				.WithValidation<float>(val => val >= 0 && val <= 1, "must be value in range 0.0 to 1.0")
			.AddFlag("--indoors", () => includeIndoors = true)
				.WithDescription("If this flag is set, indoor locations will not be skipped.")
			.AddFlag("--destroy-overnight", () => destroyOvernight = true)
				.WithDescription("If this flag is set, the spawned artifact spots will be removed at the end of the day.")
			.Add<string>("-i", "--item", val => {
				spawnable.Add(new() {
					ItemId = val
				});
			})
				.WithDescription("Adds a specific item for the added artifact spot(s) to spawn to the spawn list. Supports item queries.")
				.AllowMultiple()
			.Add<string>("-iq", "--item-query", val => spawnable.Last().PerItemCondition = val)
				.WithDescription("Adds a per-item condition to the last added item.")
				.AllowMultiple()
			.Add<int>("-q", "--item-quality", val => spawnable.Last().Quality = val)
				.WithDescription("Adds a quality to the last added item.")
				.AllowMultiple()
			.Add<int>("--item-min", val => spawnable.Last().MinStack = val)
				.WithDescription("Sets the minimum number of items to drop of the last added item.")
				.AllowMultiple()
			.Add<int>("--item-max", val => spawnable.Last().MaxStack = val)
				.WithDescription("Sets the maximum number of items to drop of the last added item.")
				.AllowMultiple(); ;

		if (!parser.TryParse(args[1..], out error))
			return false;

		if (parser.WantsHelp) {
			Instance.Log($"Usage: {args[0]} {parser.Usage}", LogLevel.Info);
			return true;
		}

		string?[] spawnableStrings = new string?[spawnable.Count];

		Dictionary<GameLocation, int> ArtifactSpotCounts = [];

		int CountSpots(GameLocation location) {
			if (ArtifactSpotCounts.TryGetValue(location, out int count))
				return count;

			count = 0;
			foreach (var feature in location.Objects.Values)
				if (feature.QualifiedItemId == "(O)590" || feature.QualifiedItemId == "(O)SeedSpot")
					count++;

			ArtifactSpotCounts[location] = count;
			return count;
		}

		foreach (var entry in targets.SelectMany(x => x)) {
			var loc = entry.Location;
			if (loc is null || (!includeIndoors && !loc.IsOutdoors))
				continue;

			// Make sure there is something to spawn.
			if (spawnable.Count == 0) {
				var data = loc.GetData();
				if (data?.ArtifactSpots is null || data.ArtifactSpots.Count == 0)
					continue;
			}

			int count = 0;
			if (maxPerLocation > 0)
				count = CountSpots(loc);

			if (count >= maxPerLocation)
				continue;

			IEnumerable<Vector2> tiles;
			if (entry.Position.HasValue)
				tiles = entry.Position.Value.IterArea(entry.Radius, false);
			else
				tiles = EnumerateAllTiles(loc);

			foreach (var pos in FilterEnumeratedTiles(loc, filters, tiles)) {
				int x = (int) pos.X;
				int y = (int) pos.Y;

				if (!(chance >= 1f || Game1.random.NextSingle() <= chance))
					continue;

				if (!(loc.CanItemBePlacedHere(pos) &&
					!loc.IsTileOccupiedBy(pos) &&
					loc.getTileIndexAt(x, y, "AlwaysFront") == -1 &&
					loc.getTileIndexAt(x, y, "Front") == -1 &&
					!loc.isBehindBush(pos) &&
					(
						loc.doesTileHaveProperty(x, y, "Diggable", "Back") != null ||
						(
							loc.GetSeason() == Season.Winter &&
							loc.doesTileHaveProperty(x, y, "Type", "Back") != null &&
							loc.doesTileHaveProperty(x, y, "Type", "Back").Equals("Grass")
						)
					)
				))
					continue;

				SObject sobj = ItemRegistry.Create<SObject>("(O)590");

				if (destroyOvernight)
					sobj.destroyOvernight = true;

				loc.Objects.Add(pos, sobj);

				if (spawnable.Count > 0) {
					int i = Game1.random.Next(spawnable.Count);
					string? serialized = spawnableStrings[i];
					if (serialized is null) {
						serialized = JsonConvert.SerializeObject(spawnable[i]);
						spawnableStrings[i] = serialized;
					}

					sobj.modData[SAS_ITEM] = serialized;
				}

				max--;
				if (max <= 0)
					break;

				if (maxPerLocation > 0) {
					count++;
					ArtifactSpotCounts[loc] = count;

					if (count >= maxPerLocation)
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


	public static bool DigUpCustomArtifactSpot(GameLocation location, int x, int y, Farmer who, SObject sobj) {
		if (!sobj.modData.TryGetValue(SAS_ITEM, out string? itemQuery) || string.IsNullOrEmpty(itemQuery))
			return false;

		var toSpawn = JsonConvert.DeserializeObject<ArtifactSpotDropData>(itemQuery);
		if (toSpawn is null)
			return false;

		Vector2 tilePos = new Vector2(x * 64, y * 64);
		ItemQueryContext ctx = new(location, who, Game1.random);

		Item item = ItemQueryResolver.TryResolveRandomItem(toSpawn, ctx, logError: (query, error) => {
			Instance.Log($"Error parsing item query '{query}' for artifact spot: {error}", LogLevel.Error);
		});

		if (item is null)
			return false;

		if (toSpawn.OneDebrisPerDrop && item.Stack > 1)
			Game1.createMultipleItemDebris(item, tilePos, -1, location);
		else
			Game1.createItemDebris(item, tilePos, Game1.random.Next(4), location);

		bool hasGenerousEnchantment = (who?.CurrentTool as Hoe)?.hasEnchantmentOfType<GenerousEnchantment>() ?? false;
		if (hasGenerousEnchantment && toSpawn.ApplyGenerousEnchantment && Game1.random.NextBool()) {
			item = item.getOne();
			item = (Item) ItemQueryResolver.ApplyItemFields(item, toSpawn, ctx);
			if (toSpawn.OneDebrisPerDrop && item.Stack > 1)
				Game1.createMultipleItemDebris(item, tilePos, -1, location);
			else
				Game1.createItemDebris(item, tilePos, Game1.random.Next(4), location);
		}

		return true;
	}

}

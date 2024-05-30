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
using StardewValley.TerrainFeatures;


namespace Leclair.Stardew.CloudySkies;

public static partial class Triggers {

	[TriggerAction]
	public static bool GrowGiantCrops(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		string? query = null;
		string? fertilizerQuery = null;
		float chance = 1f;
		int max = int.MaxValue;
		bool includeIndoors = false;
		bool allowImmature = false;
		bool ignoreSize = false;
		bool ignoreLocationRequirement = false;

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of crops to change.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.AddFlag("--allow-immature", () => allowImmature = true)
				.WithDescription("Allow crops that aren't yet fully grown to become giant.")
			.AddFlag("--ignore-size", () => ignoreSize = true)
				.WithDescription("Ignore size requirements when growing giant crops.")
			.AddFlag("--allow-anywhere", () => ignoreLocationRequirement = true)
				.WithDescription("Ignore location-specific AllowGiantCrop flags.")
			.Add<float>("-c", "--chance", val => chance = val)
				.WithDescription("The percent chance that any given crop will be changed. Default: 1.0")
				.WithValidation<float>(val => val >= 0 && val <= 1, "must be value in range 0.0 to 1.0")
			.AddFlag("--indoors", () => includeIndoors = true)
				.WithDescription("If this flag is set, indoor locations will not be skipped.")
			.Add<string>("-q", "--query", val => query = val)
				.WithDescription("An optional Game State Query for filtering which crops are affected.")
			.Add<string>("--fertilizer-query", val => fertilizerQuery = val)
				.WithDescription("An optional Game State Query for filtering which crops are affected based on fertilizer.");

		if (!parser.TryParse(args[1..], out error))
			return false;

		if (parser.WantsHelp) {
			Instance.Log($"Usage: {args[0]} {parser.Usage}", LogLevel.Info);
			return true;
		}

		// Now, loop through all the locations and grow everything.
		foreach (var entry in targets.SelectMany(x => x)) {
			var loc = entry.Location;
			if (loc is null || (!includeIndoors && !loc.IsOutdoors))
				continue;

			if (!ignoreLocationRequirement && !(loc is Farm || loc.HasMapPropertyWithValue("AllowGiantCrops")))
				continue;

			foreach (var dirt in EnumerateTerrainFeatures<HoeDirt>(loc, entry.Position, entry.Radius)) {
				if (dirt.crop is null || dirt.crop.dead.Value || !(chance >= 1f || Game1.random.NextSingle() <= chance))
					continue;

				if (!allowImmature && (dirt.crop.currentPhase.Value != dirt.crop.phaseDays.Count - 1))
					continue;

				if (!dirt.crop.TryGetGiantCrops(out var crops))
					continue;

				if (!string.IsNullOrEmpty(query)) {
					var target = GetOrCreateInstance(dirt.crop.indexOfHarvest.Value);
					var input = GetOrCreateInstance(dirt.crop.netSeedIndex.Value);

					if (!GameStateQuery.CheckConditions(query, loc, null, targetItem: target, inputItem: input))
						continue;
				}

				if (!CheckFertilizer(dirt, fertilizerQuery))
					continue;

				// Find a matching crop.
				foreach (var pair in crops) {
					var giantCrop = pair.Value;
					if (!string.IsNullOrEmpty(giantCrop.Condition) && !GameStateQuery.CheckConditions(giantCrop.Condition, loc))
						continue;

					bool valid = true;
					for (int yOffset = 0; yOffset < giantCrop.TileSize.Y; yOffset++) {
						for (int xOffset = 0; xOffset < giantCrop.TileSize.X; xOffset++) {
							Vector2 pos = new(dirt.Tile.X + xOffset, dirt.Tile.Y + yOffset);
							if (loc.terrainFeatures.TryGetValue(pos, out var feature)) {
								// TODO: Better check so empty HoeDirt won't block with --ignore-size
								if (feature is not HoeDirt dirt2) {
									valid = false;
									break;
								}

								if (dirt2.crop?.indexOfHarvest?.Value == dirt.crop.indexOfHarvest.Value)
									continue;
								else if (dirt2.crop != null || !ignoreSize || loc.IsTileBlockedBy(pos, ~(CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.TerrainFeatures))) {
									valid = false;
									break;
								}

								foreach (var clump in loc.resourceClumps) {
									if (clump.occupiesTile((int) pos.X, (int) pos.Y)) {
										valid = false;
										break;
									}
								}

								if (!valid)
									break;

								if (loc.largeTerrainFeatures is not null)
									foreach (var feat in loc.largeTerrainFeatures) {
										if (feat.getBoundingBox().Contains(pos)) {
											valid = false;
											break;
										}
									}

								if (!valid)
									break;

							} else if (ignoreSize) {
								if (loc.doesTileHaveProperty((int) pos.X, (int) pos.Y, "Diggable", "Back") == null || loc.IsTileBlockedBy(pos, ~(CollisionMask.Characters | CollisionMask.Farmers))) {
									valid = false;
									break;
								}

							} else {
								valid = false;
								break;
							}
						}

						if (!valid)
							break;
					}

					if (!valid)
						continue;

					for (int yOffset = 0; yOffset < giantCrop.TileSize.Y; yOffset++) {
						for (int xOffset = 0; xOffset < giantCrop.TileSize.X; xOffset++) {
							Vector2 pos = new Vector2(dirt.Tile.X + xOffset, dirt.Tile.Y + yOffset);
							if (loc.terrainFeatures.TryGetValue(pos, out var feature) && feature is HoeDirt dirt2)
								dirt2.crop = null;
						}
					}

					loc.resourceClumps.Add(new GiantCrop(pair.Key, dirt.Tile));
					max--;
					break;
				}

				if (max <= 0)
					break;
			}

			if (max <= 0)
				break;
		}

		// Great success!
		error = null;
		return true;
	}

}

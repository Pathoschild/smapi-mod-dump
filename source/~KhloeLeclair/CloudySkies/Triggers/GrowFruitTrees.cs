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

using StardewModdingAPI;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.TerrainFeatures;


namespace Leclair.Stardew.CloudySkies;

public static partial class Triggers {

	[TriggerAction]
	public static bool GrowFruitTrees(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		string? query = null;
		float chance = 1f;
		int steps = 1;
		int max = int.MaxValue;
		int maxFruit = int.MaxValue;
		bool includeIndoors = false;

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of fruit trees to change.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<int>("-day", "--days", val => steps = val)
				.WithDescription("How many days of growth should each fruit tree experience. Default: 1")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<int>("--max-fruit", val => maxFruit = val)
				.WithDescription("The maximum number of fruit to grow on any given fruit tree. Default: 3")
				.WithValidation<int>(val => val >= 0 && val <= 3, "must be in range 0 to 3")
			.Add<float>("-c", "--chance", val => chance = val)
				.WithDescription("The percent chance that any given fruit tree will be changed. Default: 1.0")
				.WithValidation<float>(val => val >= 0 && val <= 1, "must be value in range 0.0 to 1.0")
			.AddFlag("--indoors", () => includeIndoors = true)
				.WithDescription("If this flag is set, indoor locations will not be skipped.")
			.Add<string>("-q", "--query", val => query = val)
				.WithDescription("An optional Game State Query for filtering which fruit trees are affected.");

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

			foreach (var fruitTree in EnumerateTerrainFeatures<FruitTree>(loc, entry.Position, entry.Radius)) {
				if (fruitTree.stump.Value)
					continue;

				if (!(chance >= 1f || Game1.random.NextSingle() <= chance))
					continue;

				if (!string.IsNullOrEmpty(query)) {
					Item? target = fruitTree.fruit.FirstOrDefault();
					Item? input = GetOrCreateInstance(fruitTree.treeId.Value);

					if (!GameStateQuery.CheckConditions(query, loc, null, targetItem: target, inputItem: input))
						continue;
				}

				int days = steps;
				bool blocked = FruitTree.IsGrowthBlocked(fruitTree.Tile, fruitTree.Location);
				bool changed = false;

				while (days-- > 0) {
					bool updated = false;
					if (!blocked && fruitTree!.daysUntilMature.Value > 0) {
						fruitTree.daysUntilMature.Value -= fruitTree.growthRate.Value;
						fruitTree.growthStage.Value = FruitTree.DaysUntilMatureToGrowthStage(fruitTree.daysUntilMature.Value);
						updated = true;
					}

					int fruit = fruitTree.fruit.Count;
					if (!fruitTree.stump.Value && fruit < maxFruit) {
						fruitTree.TryAddFruit();
						if (fruitTree.fruit.Count != fruit)
							updated = true;
					}

					if (!updated)
						break;
					else
						changed = true;
				}

				if (changed)
					max--;
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

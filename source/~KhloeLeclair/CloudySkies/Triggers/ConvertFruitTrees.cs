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
	public static bool ConvertFruitTrees(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		string treeId = string.Empty;
		string? query = null;
		float chance = 1f;
		int max = int.MaxValue;
		int setDays = -1;
		bool includeIndoors = false;
		bool changeFruit = false;
		bool onlyMature = false;

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<string>("FruitTreeId", val => treeId = val)
				.IsRequired()
				.WithDescription("The Id to convert the matching fruit tree(s) to. Must match an entry in Data/FruitTrees")
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of fruit trees to change.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<float>("-c", "--chance", val => chance = val)
				.WithDescription("The percent chance that any given fruit tree will be changed. Default: 1.0")
				.WithValidation<float>(val => val >= 0 && val <= 1, "must be value in range 0.0 to 1.0")
			.AddFlag("--indoors", () => includeIndoors = true)
				.WithDescription("If this flag is set, indoor locations will not be skipped.")
			.AddFlag("--change-fruit", () => changeFruit = true)
				.WithDescription("If this flag is set, any existing fruit will be changed to match the new tree type.")
			.AddFlag("--only-mature", () => onlyMature = true)
				.WithDescription("If this flag is set, only mature fruit trees will be changed.")
			.Add<int>("--set-days", val => setDays = val)
				.WithDescription("When changing a fruit tree, set its days until mature to this value.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<string>("-q", "--query", val => query = val)
				.WithDescription("An optional Game State Query for filtering which fruit trees are affected.");

		if (!parser.TryParse(args[1..], out error))
			return false;

		if (parser.WantsHelp) {
			Instance.Log($"Usage: {args[0]} {parser.Usage}", LogLevel.Info);
			return true;
		}

		if (!DataLoader.FruitTrees(Game1.content).TryGetValue(treeId, out var treeData)) {
			error = $"Invalid tree Id: {treeId}";
			return false;
		}

		// Now, loop through all the locations and grow everything.
		foreach (var entry in targets.SelectMany(x => x)) {
			var loc = entry.Location;
			if (loc is null || (!includeIndoors && !loc.IsOutdoors))
				continue;

			foreach (var fruitTree in EnumerateTerrainFeatures<FruitTree>(loc, entry.Position, entry.Radius)) {
				if (fruitTree.treeId.Value == treeId || fruitTree.stump.Value)
					continue;

				if (onlyMature && fruitTree.daysUntilMature.Value > 0)
					continue;

				if (!(chance >= 1f || Game1.random.NextSingle() <= chance))
					continue;

				if (!string.IsNullOrEmpty(query)) {
					Item? target = fruitTree.fruit.FirstOrDefault();
					Item? input = GetOrCreateInstance(fruitTree.treeId.Value);

					if (!GameStateQuery.CheckConditions(query, loc, null, targetItem: target, inputItem: input))
						continue;
				}

				fruitTree.treeId.Value = treeId;

				if (setDays >= 0) {
					fruitTree.daysUntilMature.Value = setDays;
					fruitTree.growthStage.Value = FruitTree.DaysUntilMatureToGrowthStage(setDays);

					// Remove fruit from non-mature trees.
					if (setDays > 0)
						fruitTree.fruit.Clear();
				}

				fruitTree.loadSprite();

				if (changeFruit) {
					int existing = fruitTree.fruit.Count;
					fruitTree.fruit.Clear();
					int count = 0;

					while (count < existing) {
						int old_count = count;
						fruitTree.TryAddFruit();
						count = fruitTree.fruit.Count;
						if (count == old_count)
							break;
					}
				}

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

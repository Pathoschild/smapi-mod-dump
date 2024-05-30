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

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.TerrainFeatures;


namespace Leclair.Stardew.CloudySkies;

public static partial class Triggers {

	[TriggerAction]
	public static bool GrowTrees(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		string? query = null;
		float chance = 1f;
		int steps = 1;
		int max = int.MaxValue;
		int maxStage = Tree.treeStage;
		bool includeIndoors = false;

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of trees to change.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<int>("-s", "--stages", val => steps = val)
				.WithDescription("How many stages of growth should each tree experience. Default: 1")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<int>("--max-stage", val => maxStage = val)
				.WithDescription("The maximum stage a tree is allowed to reach. Default: 5")
				.WithValidation<int>(val => val >= 0, "must be greater than or equal to 0")
			.Add<float>("-c", "--chance", val => chance = val)
				.WithDescription("The percent chance that any given tree will be changed. Default: 1.0")
				.WithValidation<float>(val => val >= 0 && val <= 1, "must be value in range 0.0 to 1.0")
			.AddFlag("--indoors", () => includeIndoors = true)
				.WithDescription("If this flag is set, indoor locations will not be skipped.")
			.Add<string>("-q", "--query", val => query = val)
				.WithDescription("An optional Game State Query for filtering which trees are affected.");

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

			foreach (var tree in EnumerateTerrainFeatures<Tree>(loc, entry.Position, entry.Radius)) {
				int maxSize = Math.Min(tree.GetMaxSizeHere(), maxStage);
				if (maxSize <= tree.growthStage.Value)
					continue;

				if (!(chance >= 1f || Game1.random.NextSingle() <= chance))
					continue;

				if (!string.IsNullOrEmpty(query)) {
					var data = tree.GetData();
					var input = GetOrCreateInstance(data?.SeedItemId);

					if (!GameStateQuery.CheckConditions(query, loc, null, targetItem: null, inputItem: input))
						continue;
				}

				int newStage = tree.growthStage.Value + steps;
				if (newStage > maxSize)
					newStage = maxSize;

				if (newStage != tree.growthStage.Value) {
					tree.growthStage.Value = newStage;
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

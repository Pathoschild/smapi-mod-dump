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

namespace Leclair.Stardew.CloudySkies;

public static partial class Triggers {

	[TriggerAction]
	public static bool GrowCrops(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		string? query = null;
		string? fertilizerQuery = null;
		float chance = 1f;
		int steps = 1;
		int maxSteps = -1;
		int max = int.MaxValue;
		bool includeIndoors = false;

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of crops to change.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<int>("-d", "--days", val => steps = val)
				.WithDescription("How many days of growth should each crop experience. Default: 1")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<int>("--max-days", val => maxSteps = val)
				.WithDescription("Maximum number of days of growth for each crop. If greater than --days, each crop grows a random number of days within the range.")
				.WithValidation<int>(val => val >= steps, "must be greater than or equal to --days")
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

		bool has_uf = Instance.intUF?.IsLoaded ?? false;

		// Now, loop through all the locations and grow everything.
		foreach (var entry in targets.SelectMany(x => x)) {
			var loc = entry.Location;
			if (loc is null || (!includeIndoors && !loc.IsOutdoors))
				continue;

			foreach (var (dirt, pot) in EnumerateHoeDirtAndPots(loc, entry.Position, entry.Radius)) {
				if (dirt.crop is null || dirt.crop.dead.Value || !(chance >= 1f || Game1.random.NextSingle() <= chance))
					continue;

				if (!string.IsNullOrEmpty(query)) {
					var target = GetOrCreateInstance(dirt.crop.indexOfHarvest.Value);
					var input = GetOrCreateInstance(dirt.crop.netSeedIndex.Value);

					if (!GameStateQuery.CheckConditions(query, loc, null, targetItem: target, inputItem: input))
						continue;
				}

				if (!CheckFertilizer(dirt, fertilizerQuery))
					continue;

				int days = steps;
				if (maxSteps > days)
					days = Game1.random.Next(days, maxSteps + 1);

				while (days-- > 0 && dirt.crop != null && !dirt.crop.dead.Value)
					dirt.crop.newDay(dirt.state.Value == 2 ? 2 : 1);

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

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
	public static bool UnFertilizeDirt(string[] args, TriggerActionContext context, out string? error) {
		List<IEnumerable<TargetPosition>> targets = [];
		string? query = null;
		string? fertilizerId = null;
		string? fertilizerQuery = null;
		float chance = 1f;
		int max = int.MaxValue;
		bool includeIndoors = false;

		var parser = ArgumentParser.New()
			.AddHelpFlag()
			.AddPositional<IEnumerable<TargetPosition>>("Target", targets.Add)
				.IsRequired()
				.AllowMultiple()
			.Add<string>("-f", "--fertilizer", val => fertilizerId = val)
				.WithDescription("If set, only remove the fertilizer with this ID from dirt tiles.")
			.Add<int>("--max", null, val => max = val)
				.WithDescription("The maximum number of dirt tiles to change.")
				.WithValidation<int>(val => val > 0, "must be greater than 0")
			.Add<float>("-c", "--chance", val => chance = val)
				.WithDescription("The percent chance that any given dirt tiles will be changed. Default: 1.0")
				.WithValidation<float>(val => val >= 0 && val <= 1, "must be value in range 0.0 to 1.0")
			.AddFlag("--indoors", () => includeIndoors = true)
				.WithDescription("If this flag is set, indoor locations will not be skipped.")
			.Add<string>("-q", "--query", val => query = val)
				.WithDescription("An optional Game State Query for filtering which dirt tiles are affected.")
			.Add<string>("--fertilizer-query", val => fertilizerQuery = val)
				.WithDescription("An optional Game State Query for filtering which dirt tiles are affected based on fertilizer.");

		if (!parser.TryParse(args[1..], out error))
			return false;

		if (parser.WantsHelp) {
			Instance.Log($"Usage: {args[0]} {parser.Usage}", LogLevel.Info);
			return true;
		}

		if (!string.IsNullOrEmpty(fertilizerId)) {
			fertilizerId = ItemRegistry.QualifyItemId(fertilizerId);
			if (ItemRegistry.GetData(fertilizerId) is null) {
				error = $"Invalid item Id for fertilizer: {fertilizerId}";
				return false;
			}
		}

		if (Instance.intUF is null) {
			error = "Invalid internal state. Cannot process fertilizer.";
			return false;
		}

		// Now, loop through all the locations and un-fertilize everything.
		foreach (var entry in targets.SelectMany(x => x)) {
			var loc = entry.Location;
			if (loc is null || (!includeIndoors && !loc.IsOutdoors))
				continue;

			foreach (var (dirt, pot) in EnumerateHoeDirtAndPots(loc, entry.Position, entry.Radius)) {
				if (dirt.fertilizer.Value == null || !(chance >= 1f || Game1.random.NextSingle() <= chance))
					continue;

				if (!string.IsNullOrEmpty(query)) {
					var target = dirt.crop is null ? null : GetOrCreateInstance(dirt.crop.indexOfHarvest.Value);
					var input = dirt.crop is null ? null : GetOrCreateInstance(dirt.crop.netSeedIndex.Value);

					if (!GameStateQuery.CheckConditions(query, loc, null, targetItem: target, inputItem: input))
						continue;
				}

				if (!CheckFertilizer(dirt, fertilizerQuery))
					continue;

				// Remove the fertilizer.
				if (!Instance.intUF.RemoveFertilizer(dirt, fertilizerId, Game1.player))
					continue;

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

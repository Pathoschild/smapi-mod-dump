/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shockah.SeasonAffixes;

internal interface IAffixSetGenerator
{
	IEnumerable<IReadOnlySet<ISeasonAffix>> Generate(OrdinalSeason season);
}

internal static class IAffixSetGeneratorExt
{
	public static IAffixSetGenerator WeightedRandom(this IAffixSetGenerator affixSetGenerator, Random random, IAffixSetWeightProvider weightProvider)
		=> new WeightedRandomAffixSetGenerator(affixSetGenerator, random, weightProvider);

	public static IAffixSetGenerator AsLittleAsPossible(this IAffixSetGenerator affixSetGenerator)
		=> new AsLittleAsPossibleAffixSetGenerator(affixSetGenerator);

	public static IAffixSetGenerator AvoidingDuplicatesBetweenChoices(this IAffixSetGenerator affixSetGenerator)
		=> new AvoidingDuplicatesBetweenChoicesAffixSetGenerator(affixSetGenerator);

	public static IAffixSetGenerator Benchmarking(this IAffixSetGenerator affixSetGenerator, IMonitor monitor, string tag, LogLevel logLevel = LogLevel.Debug)
		=> new BenchmarkingAffixSetGenerator(affixSetGenerator, monitor, tag, logLevel);
}

internal sealed class AllCombinationsAffixSetGenerator : IAffixSetGenerator
{
	private IAffixesProvider AffixesProvider { get; init; }
	private IAffixScoreProvider ScoreProvider { get; init; }
	private IReadOnlyList<Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, bool?>> ConflictInfoProviders { get; init; }
	private IReadOnlySet<ISeasonAffix> OtherAffixes { get; init; }
	private int Positivity { get; init; }
	private int Negativity { get; init; }
	private int MaxAffixes { get; init; }

	public AllCombinationsAffixSetGenerator(IAffixesProvider affixesProvider, IAffixScoreProvider scoreProvider, IReadOnlyList<Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, bool?>> conflictInfoProviders, IReadOnlySet<ISeasonAffix> otherAffixes, int positivity, int negativity, int maxAffixes)
	{
		this.AffixesProvider = affixesProvider;
		this.ScoreProvider = scoreProvider;
		this.ConflictInfoProviders = conflictInfoProviders;
		this.OtherAffixes = otherAffixes;
		this.Positivity = positivity;
		this.Negativity = negativity;
		this.MaxAffixes = maxAffixes;
	}

	public IEnumerable<IReadOnlySet<ISeasonAffix>> Generate(OrdinalSeason season)
	{
		var allAffixes = AffixesProvider.Affixes
			.OrderByDescending(a => ScoreProvider.GetPositivity(a, season) + ScoreProvider.GetNegativity(a, season))
			.ThenByDescending(a => ScoreProvider.GetPositivity(a, season) - ScoreProvider.GetNegativity(a, season))
			.ToArray();
		var affixPositivities = allAffixes.Select(a => ScoreProvider.GetPositivity(a, season)).ToArray();
		var affixNegativities = allAffixes.Select(a => ScoreProvider.GetNegativity(a, season)).ToArray();
		return GetAllCombinations(season, allAffixes, 0, affixPositivities, affixNegativities, new(0), 0, 0);
	}

	private bool IsConflicting(OrdinalSeason season, HashSet<ISeasonAffix> combination)
	{
		var toCheck = combination;
		if (OtherAffixes.Count != 0)
			toCheck = toCheck.Union(OtherAffixes).ToHashSet();

		foreach (var provider in ConflictInfoProviders)
		{
			var result = provider(toCheck, season);
			if (result is not null)
				return result.Value;
		}
		return false;
	}

	private IEnumerable<HashSet<ISeasonAffix>> GetAllCombinations(OrdinalSeason season, ISeasonAffix[] allAffixes, int allAffixesIndex, int[] affixPositivities, int[] affixNegativities, HashSet<ISeasonAffix> combination, int currentPositivity, int currentNegativity)
	{
		if (currentPositivity > Positivity || currentNegativity > Negativity || combination.Count > MaxAffixes)
			yield break;
		if (IsConflicting(season, combination))
			yield break;
		if (currentPositivity == Positivity && currentNegativity == Negativity)
		{
			yield return combination;
			yield break;
		}
		if (allAffixesIndex >= allAffixes.Length)
			yield break;

		IEnumerable<int> baseEnumerable = Enumerable.Range(allAffixesIndex, allAffixes.Length - allAffixesIndex);
		if (combination.Count == 0 && Positivity + Negativity >= 3)
			baseEnumerable = baseEnumerable.AsParallel().AsOrdered();
		var enumerable = baseEnumerable.SelectMany(i => GetAllCombinations(season, allAffixes, i + 1, affixPositivities, affixNegativities, new HashSet<ISeasonAffix>(combination) { allAffixes[i] }, currentPositivity + affixPositivities[i], currentNegativity + affixNegativities[i]));
		foreach (var result in enumerable)
			yield return result;
	}
}

internal sealed class WeightedRandomAffixSetGenerator : IAffixSetGenerator
{
	private IAffixSetGenerator AffixSetGenerator { get; init; }
	private Random Random { get; init; }
	private IAffixSetWeightProvider WeightProvider { get; init; }

	public WeightedRandomAffixSetGenerator(IAffixSetGenerator affixSetGenerator, Random random, IAffixSetWeightProvider weightProvider)
	{
		this.AffixSetGenerator = affixSetGenerator;
		this.Random = random;
		this.WeightProvider = weightProvider;
	}

	public IEnumerable<IReadOnlySet<ISeasonAffix>> Generate(OrdinalSeason season)
	{
		var weightedItems = AffixSetGenerator.Generate(season)
			.Select(combination => new WeightedItem<IReadOnlySet<ISeasonAffix>>(WeightProvider.GetWeight(combination, season), combination))
			.ToList();

		if (weightedItems.Count == 0)
			yield break;
		double maxWeight = weightedItems.Max(item => item.Weight);

		WeightedRandom<IReadOnlySet<ISeasonAffix>> weightedRandom = new(weightedItems.Where(item => item.Weight >= maxWeight / 100));
		foreach (var result in weightedRandom.GetConsumingEnumerable(Random))
			yield return result;
	}
}

internal sealed class AsLittleAsPossibleAffixSetGenerator : IAffixSetGenerator
{
	private IAffixSetGenerator AffixSetGenerator { get; init; }

	public AsLittleAsPossibleAffixSetGenerator(IAffixSetGenerator affixSetGenerator)
	{
		this.AffixSetGenerator = affixSetGenerator;
	}

	public IEnumerable<IReadOnlySet<ISeasonAffix>> Generate(OrdinalSeason season)
	{
		var remainingResults = AffixSetGenerator.Generate(season).ToList();

		int currentAllowedCount = 0;
		while (remainingResults.Count != 0)
		{
			for (int i = 0; i < remainingResults.Count; i++)
			{
				if (remainingResults[i].Count != currentAllowedCount)
					continue;
				yield return remainingResults[i];
				remainingResults.RemoveAt(i--);
			}
			currentAllowedCount++;
		}
	}
}

internal sealed class AvoidingDuplicatesBetweenChoicesAffixSetGenerator : IAffixSetGenerator
{
	private IAffixSetGenerator AffixSetGenerator { get; init; }

	public AvoidingDuplicatesBetweenChoicesAffixSetGenerator(IAffixSetGenerator affixSetGenerator)
	{
		this.AffixSetGenerator = affixSetGenerator;
	}

	public IEnumerable<IReadOnlySet<ISeasonAffix>> Generate(OrdinalSeason season)
	{
		List<HashSet<string>> yielded = new();
		var remainingResults = AffixSetGenerator.Generate(season).ToList();

		int allowedDuplicates = 0;
		while (remainingResults.Count != 0)
		{
			for (int i = 0; i < remainingResults.Count; i++)
			{
				var ids = remainingResults[i].Select(a => a.UniqueID).ToHashSet();
				foreach (var yieldedEntry in yielded)
				{
					if (yieldedEntry.Intersect(ids).Count() > allowedDuplicates)
						goto remainingResultsContinue;
				}
				yielded.Add(ids);
				yield return remainingResults[i];
				remainingResults.RemoveAt(i--);

				remainingResultsContinue:;
			}

			allowedDuplicates++;
		}
	}
}

internal sealed class BenchmarkingAffixSetGenerator : IAffixSetGenerator
{
	private IAffixSetGenerator AffixSetGenerator { get; init; }
	private IMonitor Monitor { get; init; }
	private string Tag { get; init; }
	private LogLevel LogLevel { get; init; }

	public BenchmarkingAffixSetGenerator(IAffixSetGenerator affixSetGenerator, IMonitor monitor, string tag, LogLevel logLevel = LogLevel.Debug)
	{
		this.AffixSetGenerator = affixSetGenerator;
		this.Monitor = monitor;
		this.Tag = tag;
		this.LogLevel = logLevel;
	}

	public IEnumerable<IReadOnlySet<ISeasonAffix>> Generate(OrdinalSeason season)
	{
		Monitor.Log($"[{Tag}] Generating affix sets...", LogLevel);
		Stopwatch stopwatch = Stopwatch.StartNew();
		int index = 0;

		foreach (var result in AffixSetGenerator.Generate(season))
		{
#if DEBUG
			if (index < 10 || (index < 100 && index % 10 == 0) || (index < 1000 && index % 100 == 0) || (index < 10000 && index % 1000 == 0) || (index < 100000 && index % 10000 == 0))
				Monitor.Log($"> [{Tag}] Generated affix set #{index + 1}, took {stopwatch.ElapsedMilliseconds}ms", LogLevel);
#endif
			yield return result;
			index++;
		}
#if DEBUG
		Monitor.Log($"> [{Tag}] Done generating affix sets after {index} results, took {stopwatch.ElapsedMilliseconds}ms.", LogLevel);
#endif
	}
}
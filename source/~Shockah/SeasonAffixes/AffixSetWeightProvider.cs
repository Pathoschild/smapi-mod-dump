/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.SeasonAffixes;

internal interface IAffixSetWeightProvider
{
	double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season);
}

internal static class IAffixSetWeightProviderExt
{
	public static IAffixSetWeightProvider MultiplyingBy(this IAffixSetWeightProvider provider, IAffixSetWeightProvider anotherProvider)
		=> new MultiplyingAffixSetWeightProvider(provider, anotherProvider);
}

internal sealed class DefaultProbabilityAffixSetWeightProvider : IAffixSetWeightProvider
{
	private IAffixProbabilityWeightProvider ProbabilityWeightProvider { get; init; }

	public DefaultProbabilityAffixSetWeightProvider(IAffixProbabilityWeightProvider probabilityWeightProvider)
	{
		this.ProbabilityWeightProvider = probabilityWeightProvider;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		=> combination.Count == 0
		? 0
		: combination.Average(a => ProbabilityWeightProvider.GetProbabilityWeight(a, season));
}

internal sealed class MultiplyingAffixSetWeightProvider : IAffixSetWeightProvider
{
	private IAffixSetWeightProvider Base { get; init; }
	private IAffixSetWeightProvider Multiplier { get; init; }

	public MultiplyingAffixSetWeightProvider(IAffixSetWeightProvider @base, IAffixSetWeightProvider multiplier)
	{
		this.Base = @base;
		this.Multiplier = multiplier;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
	{
		double weight = Base.GetWeight(combination, season);
		if (weight > 0)
			weight *= Multiplier.GetWeight(combination, season);
		return weight;
	}
}

internal sealed class DelegateAffixSetWeightProvider : IAffixSetWeightProvider
{
	private Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double> Delegate { get; init; }

	public DelegateAffixSetWeightProvider(Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double> @delegate)
	{
		this.Delegate = @delegate;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		=> Delegate(combination, season);
}

internal sealed class ConfigAffixSetWeightProvider : IAffixSetWeightProvider
{
	private IReadOnlyDictionary<string, double> AffixWeights { get; init; }

	public ConfigAffixSetWeightProvider(IReadOnlyDictionary<string, double> affixWeights)
	{
		this.AffixWeights = affixWeights;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		=> combination.Count == 0
		? 0
		: combination.Average(a => AffixWeights.TryGetValue(a.UniqueID, out var weight) ? weight : 1.0);
}

internal sealed class CustomAffixSetWeightProvider : IAffixSetWeightProvider
{
	private IReadOnlyList<Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double?>> CustomProviders { get; init; }

	public CustomAffixSetWeightProvider(IReadOnlyList<Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double?>> customProviders)
	{
		this.CustomProviders = customProviders;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
	{
		var totalWeight = 1.0;
		foreach (var provider in CustomProviders)
		{
			var weight = provider(combination, season);
			if (weight is not null)
			{
				totalWeight *= weight.Value;
				if (totalWeight <= 0)
					break;
			}
		}
		return totalWeight;
	}
}

internal sealed class PairingUpTagsAffixSetWeightProvider : IAffixSetWeightProvider
{
	private IAffixScoreProvider ScoreProvider { get; init; }
	private IAffixTagPairCandidateProvider TagPairCandidateProvider { get; init; }
	private Func<int, double> UnpairedAffixMultiplier { get; init; }
	private double OneSidedPairedAffixesMultiplier { get; init; }
	private int PairedAffixLimit { get; init; }
	private double TooManyPairedAffixesMultiplier { get; init; }

	public PairingUpTagsAffixSetWeightProvider(
		IAffixScoreProvider scoreProvider,
		IAffixTagPairCandidateProvider tagPairCandidateProvider,
		Func<int, double> unpairedAffixMultiplier,
		double oneSidedPairedAffixesMultiplier,
		int pairedAffixLimit,
		double tooManyPairedAffixesMultiplier
	)
	{
		this.ScoreProvider = scoreProvider;
		this.TagPairCandidateProvider = tagPairCandidateProvider;
		this.UnpairedAffixMultiplier = unpairedAffixMultiplier;
		this.OneSidedPairedAffixesMultiplier = oneSidedPairedAffixesMultiplier;
		this.PairedAffixLimit = pairedAffixLimit;
		this.TooManyPairedAffixesMultiplier = tooManyPairedAffixesMultiplier;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
	{
		var weight = 1.0;
		var relatedAffixDictionary = combination.ToDictionary(a => a, a => combination.Where(a2 => a2.Tags.Any(t => a.Tags.Contains(t))).ToList());
		foreach (var (affix, relatedAffixes) in relatedAffixDictionary)
		{
			if (relatedAffixes.Count == 1)
			{
				if (affix.Tags.Count > 0 && ScoreProvider.GetPositivity(affix, season) == 0 || ScoreProvider.GetNegativity(affix, season) == 0)
				{
					int possibleTagPairAffixes = TagPairCandidateProvider.GetTagPairCandidatesForAffix(affix, season).Count;
					weight *= UnpairedAffixMultiplier(possibleTagPairAffixes);
				}
			}
			else
			{
				if (relatedAffixes.All(a => ScoreProvider.GetPositivity(a, season) == 0) || relatedAffixes.All(a => ScoreProvider.GetNegativity(a, season) == 0))
					weight *= OneSidedPairedAffixesMultiplier;
				if (relatedAffixes.Count >= PairedAffixLimit)
					weight *= TooManyPairedAffixesMultiplier;
			}
		}
		return weight;
	}
}

internal sealed class AvoidingChoiceHistoryDuplicatesAffixSetWeightProvider : IAffixSetWeightProvider
{
	private double RepeatedWeight { get; init; }

	public AvoidingChoiceHistoryDuplicatesAffixSetWeightProvider(double repeatedWeight)
	{
		this.RepeatedWeight = repeatedWeight;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
	{
		foreach (var affix in combination)
			foreach (var step in SeasonAffixes.Instance.SaveData.AffixChoiceHistory)
				if (step.Contains(affix))
					return RepeatedWeight;
		return 1;
	}
}

internal sealed class AvoidingSetChoiceHistoryDuplicatesAffixSetWeightProvider : IAffixSetWeightProvider
{
	private double RepeatedWeight { get; init; }

	public AvoidingSetChoiceHistoryDuplicatesAffixSetWeightProvider(double repeatedWeight)
	{
		this.RepeatedWeight = repeatedWeight;
	}

	public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
	{
		foreach (var step in SeasonAffixes.Instance.SaveData.AffixSetChoiceHistory)
			foreach (var stepCombination in step)
				if (stepCombination.SetEquals(combination))
					return RepeatedWeight;
		return 1;
	}
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Shockah.SeasonAffixes;

internal interface IAffixesProvider
{
	IEnumerable<ISeasonAffix> Affixes { get; }
}

internal static class IAffixesProviderExt
{
	public static IAffixesProvider Effective(this IAffixesProvider provider, IAffixScoreProvider scoreProvider, OrdinalSeason season)
		=> new EffectiveAffixesProvider(provider, scoreProvider, season);

	public static IAffixesProvider ApplicableToSeason(this IAffixesProvider provider, IAffixProbabilityWeightProvider probabilityWeightProvider, OrdinalSeason season)
		=> new ApplicableToSeasonAffixesProvider(provider, probabilityWeightProvider, season);

	public static IAffixesProvider Excluding(this IAffixesProvider provider, IReadOnlySet<ISeasonAffix> excludedAffixes)
		=> new ExcludingAffixesProvider(provider, excludedAffixes);
}

internal sealed class AffixesProvider : IAffixesProvider
{
	public IEnumerable<ISeasonAffix> Affixes { get; init; }

	public AffixesProvider(IEnumerable<ISeasonAffix> affixes)
	{
		this.Affixes = affixes;
	}
}

internal sealed class CompoundAffixesProvider : IAffixesProvider
{
	private IEnumerable<IAffixesProvider> Providers { get; init; }

	public IEnumerable<ISeasonAffix> Affixes
	{
		get
		{
			foreach (var provider in Providers)
				foreach (var affix in provider.Affixes)
					yield return affix;
		}
	}

	public CompoundAffixesProvider(params IAffixesProvider[] providers) : this((IEnumerable<IAffixesProvider>)providers) { }

	public CompoundAffixesProvider(IEnumerable<IAffixesProvider> providers)
	{
		this.Providers = providers;
	}
}

internal sealed class EffectiveAffixesProvider : IAffixesProvider
{
	private IAffixesProvider Wrapped { get; init; }
	private IAffixScoreProvider ScoreProvider { get; init; }
	private OrdinalSeason Season { get; init; }

	public IEnumerable<ISeasonAffix> Affixes =>
		Wrapped.Affixes
			.Where(affix => ScoreProvider.GetPositivity(affix, Season) > 0 || ScoreProvider.GetNegativity(affix, Season) > 0);

	public EffectiveAffixesProvider(IAffixesProvider wrapped, IAffixScoreProvider scoreProvider, OrdinalSeason season)
	{
		this.Wrapped = wrapped;
		this.ScoreProvider = scoreProvider;
		this.Season = season;
	}
}

internal sealed class ApplicableToSeasonAffixesProvider : IAffixesProvider
{
	private IAffixesProvider Wrapped { get; init; }
	private IAffixProbabilityWeightProvider ProbabilityWeightProvider { get; init; }
	private OrdinalSeason Season { get; init; }

	public IEnumerable<ISeasonAffix> Affixes =>
		Wrapped.Affixes
			.Where(affix => ProbabilityWeightProvider.GetProbabilityWeight(affix, Season) > 0);

	public ApplicableToSeasonAffixesProvider(IAffixesProvider wrapped, IAffixProbabilityWeightProvider probabilityWeightProvider, OrdinalSeason season)
	{
		this.Wrapped = wrapped;
		this.ProbabilityWeightProvider = probabilityWeightProvider;
		this.Season = season;
	}
}

internal sealed class ExcludingAffixesProvider : IAffixesProvider
{
	private IAffixesProvider Wrapped { get; init; }
	private IReadOnlySet<ISeasonAffix> ExcludedAffixes { get; init; }

	public IEnumerable<ISeasonAffix> Affixes =>
		Wrapped.Affixes
			.Where(affix => !ExcludedAffixes.Contains(affix));

	public ExcludingAffixesProvider(IAffixesProvider wrapped, IReadOnlySet<ISeasonAffix> excludedAffixes)
	{
		this.Wrapped = wrapped;
		this.ExcludedAffixes = excludedAffixes;
	}
}
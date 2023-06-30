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

namespace Shockah.SeasonAffixes;

internal interface IAffixProbabilityWeightProvider
{
	double GetProbabilityWeight(ISeasonAffix affix, OrdinalSeason season);
}

internal static class IAffixProbabilityWeightProviderExt
{
	public static IAffixProbabilityWeightProvider Caching(this IAffixProbabilityWeightProvider provider)
		=> new CachingAffixProbabilityWeightProvider(provider);
}

internal sealed class DefaultAffixProbabilityWeightProvider : IAffixProbabilityWeightProvider
{
	public double GetProbabilityWeight(ISeasonAffix affix, OrdinalSeason season)
		=> affix.GetProbabilityWeight(season);
}

internal sealed class CachingAffixProbabilityWeightProvider : IAffixProbabilityWeightProvider
{
	private readonly struct CacheKey : IEquatable<CacheKey>
	{
		public ISeasonAffix Affix { get; init; }
		public OrdinalSeason Season { get; init; }

		public CacheKey(ISeasonAffix affix, OrdinalSeason season)
		{
			this.Affix = affix;
			this.Season = season;
		}

		public bool Equals(CacheKey other)
			=> Affix.Equals(other.Affix) && Season == other.Season;

		public override bool Equals(object? obj)
			=> obj is CacheKey key && Equals(key);

		public override int GetHashCode()
			=> (Affix.UniqueID, Season).GetHashCode();
	}

	private IAffixProbabilityWeightProvider Provider { get; init; }
	private Dictionary<CacheKey, double> ProbabilityWeight { get; init; } = new();

	public CachingAffixProbabilityWeightProvider(IAffixProbabilityWeightProvider provider)
	{
		this.Provider = provider;
	}

	public double GetProbabilityWeight(ISeasonAffix affix, OrdinalSeason season)
	{
		CacheKey key = new(affix, season);
		if (!ProbabilityWeight.TryGetValue(key, out var value))
		{
			value = Provider.GetProbabilityWeight(affix, season);
			ProbabilityWeight[key] = value;
		}
		return value;
	}
}
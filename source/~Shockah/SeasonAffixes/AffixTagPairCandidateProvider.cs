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

internal interface IAffixTagPairCandidateProvider
{
	IReadOnlySet<ISeasonAffix> GetTagPairCandidatesForAffix(ISeasonAffix affix, OrdinalSeason season);
}

internal static class IAffixTagPairCandidateProviderExt
{
	public static IAffixTagPairCandidateProvider Caching(this IAffixTagPairCandidateProvider provider)
		=> new CachingAffixTagPairCandidateProvider(provider);
}

internal sealed class FunctionAffixTagPairCandidateProvider : IAffixTagPairCandidateProvider
{
	private Func<ISeasonAffix, OrdinalSeason, IReadOnlySet<ISeasonAffix>> Function { get; init; }

	public FunctionAffixTagPairCandidateProvider(Func<ISeasonAffix, OrdinalSeason, IReadOnlySet<ISeasonAffix>> function)
	{
		this.Function = function;
	}

	public IReadOnlySet<ISeasonAffix> GetTagPairCandidatesForAffix(ISeasonAffix affix, OrdinalSeason season)
		=> Function(affix, season);
}

internal sealed class CachingAffixTagPairCandidateProvider : IAffixTagPairCandidateProvider
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

	private IAffixTagPairCandidateProvider Provider { get; init; }
	private Dictionary<CacheKey, IReadOnlySet<ISeasonAffix>> TagPairCandidates { get; init; } = new();

	public CachingAffixTagPairCandidateProvider(IAffixTagPairCandidateProvider provider)
	{
		this.Provider = provider;
	}

	public IReadOnlySet<ISeasonAffix> GetTagPairCandidatesForAffix(ISeasonAffix affix, OrdinalSeason season)
	{
		CacheKey key = new(affix, season);
		if (!TagPairCandidates.TryGetValue(key, out var value))
		{
			value = Provider.GetTagPairCandidatesForAffix(affix, season);
			TagPairCandidates[key] = value;
		}
		return value;
	}
}
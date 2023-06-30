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

internal interface IAffixScoreProvider
{
	int GetPositivity(ISeasonAffix affix, OrdinalSeason season);
	int GetNegativity(ISeasonAffix affix, OrdinalSeason season);
}

internal static class IAffixScoreProviderExt
{
	public static IAffixScoreProvider Caching(this IAffixScoreProvider provider)
		=> new CachingAffixScoreProvider(provider);
}

internal sealed class DefaultAffixScoreProvider : IAffixScoreProvider
{
	public int GetPositivity(ISeasonAffix affix, OrdinalSeason season)
		=> affix.GetPositivity(season);

	public int GetNegativity(ISeasonAffix affix, OrdinalSeason season)
		=> affix.GetNegativity(season);
}

internal sealed class CachingAffixScoreProvider : IAffixScoreProvider
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

	private IAffixScoreProvider Provider { get; init; }
	private Dictionary<CacheKey, int> Positivity { get; init; } = new();
	private Dictionary<CacheKey, int> Negativity { get; init; } = new();

	public CachingAffixScoreProvider(IAffixScoreProvider provider)
	{
		this.Provider = provider;
	}

	public int GetPositivity(ISeasonAffix affix, OrdinalSeason season)
	{
		CacheKey key = new(affix, season);
		if (!Positivity.TryGetValue(key, out var positivity))
		{
			positivity = Provider.GetPositivity(affix, season);
			Positivity[key] = positivity;
		}
		return positivity;
	}

	public int GetNegativity(ISeasonAffix affix, OrdinalSeason season)
	{
		CacheKey key = new(affix, season);
		if (!Negativity.TryGetValue(key, out var negativity))
		{
			negativity = Provider.GetNegativity(affix, season);
			Negativity[key] = negativity;
		}
		return negativity;
	}
}
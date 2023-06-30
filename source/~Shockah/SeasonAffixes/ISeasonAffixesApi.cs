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

#nullable enable

namespace Shockah.SeasonAffixes;

public interface ISeasonAffixesApi
{
	ModConfig Config { get; }

	event Action<ISeasonAffix>? AffixRegistered;
	event Action<ISeasonAffix>? AffixUnregistered;
	event Action<AffixActivationEvent>? AffixActivated;
	event Action<AffixActivationEvent>? AffixDeactivated;

	IReadOnlyDictionary<string, ISeasonAffix> AllAffixes { get; }
	IReadOnlySet<ISeasonAffix> ActiveAffixes { get; }
	IReadOnlySet<ISeasonAffix> ActiveSeasonalAffixes { get; }
	IReadOnlySet<ISeasonAffix> ActivePermanentAffixes { get; }

	ISeasonAffix? GetAffix(string uniqueID);
	bool IsAffixActive(string uniqueID);
	bool IsAffixActive(Func<ISeasonAffix, bool> predicate);

	void RegisterAffix(ISeasonAffix affix);
	void UnregisterAffix(ISeasonAffix affix);
	void RegisterAffixConflictInfoProvider(Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, bool?> provider);
	void UnregisterAffixConflictInfoProvider(Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, bool?> provider);
	void RegisterAffixCombinationWeightProvider(Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double?> provider);
	void UnregisterAffixCombinationWeightProvider(Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double?> provider);

	void ActivateAffix(ISeasonAffix affix, AffixActivationContext context);
	void DeactivateAffix(ISeasonAffix affix, AffixActivationContext context);
	void DeactivateAllAffixes(AffixActivationContext context);
	void ChangeActiveAffixes(IEnumerable<ISeasonAffix> affixes, AffixActivationContext context);

	IReadOnlyList<ISeasonAffix> GetUIOrderedAffixes(OrdinalSeason season, IEnumerable<ISeasonAffix> affixes);
	string GetSeasonName(IReadOnlyList<ISeasonAffix> affixes);
	string GetSeasonDescription(IReadOnlyList<ISeasonAffix> affixes);

	void QueueOvernightAffixChoice();

	IReadOnlySet<ISeasonAffix> GetAllPossibleAffixesForSeason(OrdinalSeason season);
	IReadOnlySet<ISeasonAffix> GetTagPairCandidatesForAffix(ISeasonAffix affix, OrdinalSeason season);
}